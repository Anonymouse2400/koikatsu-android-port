using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MessagePack.Formatters;

namespace MessagePack.Internal
{
	internal static class DynamicPrivateFormatterBuilder
	{
		private static readonly Type refByte = typeof(byte[]).MakeByRefType();

		private static readonly MethodInfo getFormatterWithVerify = typeof(FormatterResolverExtensions).GetRuntimeMethods().First((MethodInfo x) => x.Name == "GetFormatterWithVerify");

		private static readonly Func<Type, MethodInfo> getSerialize = (Type t) => typeof(IMessagePackFormatter<>).MakeGenericType(t).GetRuntimeMethod("Serialize", new Type[4]
		{
			refByte,
			typeof(int),
			t,
			typeof(IFormatterResolver)
		});

		public static object BuildFormatter(Type type)
		{
			ObjectSerializationInfo objectSerializationInfo = ObjectSerializationInfo.CreateOrNull(type, true);
			DynamicMethod dynamicMethod = new DynamicMethod("Serialize", typeof(int), new Type[4]
			{
				typeof(byte[]).MakeByRefType(),
				typeof(int),
				type,
				typeof(IFormatterResolver)
			}, type, true);
			ILGenerator il = dynamicMethod.GetILGenerator();
			Label label = il.DefineLabel();
			il.EmitLdarg(2);
			il.Emit(OpCodes.Brtrue_S, label);
			il.EmitLdarg(0);
			il.EmitLdarg(1);
			il.EmitCall(DynamicObjectTypeBuilder.MessagePackBinaryTypeInfo.WriteNil);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(label);
			LocalBuilder local = il.DeclareLocal(typeof(int));
			il.EmitLdarg(1);
			il.EmitStloc(local);
			int writeCount = objectSerializationInfo.Members.Count((ObjectSerializationInfo.EmittableMember x) => x.IsReadable);
			EmitOffsetPlusEqual(il, null, delegate
			{
				il.EmitLdc_I4(writeCount);
				if (writeCount <= 15)
				{
					il.EmitCall(DynamicObjectTypeBuilder.MessagePackBinaryTypeInfo.WriteFixedMapHeaderUnsafe);
				}
				else
				{
					il.EmitCall(DynamicObjectTypeBuilder.MessagePackBinaryTypeInfo.WriteMapHeader);
				}
			});
			foreach (ObjectSerializationInfo.EmittableMember item in objectSerializationInfo.Members.Where((ObjectSerializationInfo.EmittableMember x) => x.IsReadable))
			{
				if (objectSerializationInfo.IsStringKey)
				{
					EmitOffsetPlusEqual(il, null, delegate
					{
						il.Emit(OpCodes.Ldstr, item.StringKey);
						il.EmitLdc_I4(StringEncoding.UTF8.GetByteCount(item.StringKey));
						il.EmitCall(DynamicObjectTypeBuilder.MessagePackBinaryTypeInfo.WriteStringUnsafe);
					});
				}
				EmitSerializeValue(il, type.GetTypeInfo(), item);
			}
			il.EmitLdarg(1);
			il.EmitLdloc(local);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Ret);
			Delegate @delegate = dynamicMethod.CreateDelegate(typeof(SerializeDelegate<>).MakeGenericType(type));
			return Activator.CreateInstance(typeof(AnonymousSerializableFormatter<>).MakeGenericType(type), @delegate);
		}

		private static void EmitOffsetPlusEqual(ILGenerator il, Action loadEmit, Action emit)
		{
			il.EmitLdarg(1);
			if (loadEmit != null)
			{
				loadEmit();
			}
			il.EmitLdarg(0);
			il.EmitLdarg(1);
			emit();
			il.Emit(OpCodes.Add);
			il.EmitStarg(1);
		}

		private static void EmitSerializeValue(ILGenerator il, TypeInfo type, ObjectSerializationInfo.EmittableMember member)
		{
			Type t = member.Type;
			if (IsOptimizeTargetType(t))
			{
				EmitOffsetPlusEqual(il, null, delegate
				{
					il.EmitLoadArg(type, 2);
					member.EmitLoadValue(il);
					if (t == typeof(byte[]))
					{
						il.EmitCall(DynamicObjectTypeBuilder.MessagePackBinaryTypeInfo.WriteBytes);
					}
					else
					{
						il.EmitCall(DynamicObjectTypeBuilder.MessagePackBinaryTypeInfo.TypeInfo.GetDeclaredMethod("Write" + t.Name));
					}
				});
				return;
			}
			EmitOffsetPlusEqual(il, delegate
			{
				il.EmitLdarg(3);
				il.Emit(OpCodes.Call, getFormatterWithVerify.MakeGenericMethod(t));
			}, delegate
			{
				il.EmitLoadArg(type, 2);
				member.EmitLoadValue(il);
				il.EmitLdarg(3);
				il.EmitCall(getSerialize(t));
			});
		}

		private static bool IsOptimizeTargetType(Type type)
		{
			if (type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(char))
			{
				return true;
			}
			return false;
		}
	}
}
