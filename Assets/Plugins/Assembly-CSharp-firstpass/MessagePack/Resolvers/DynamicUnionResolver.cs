using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class DynamicUnionResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				TypeInfo typeInfo = typeof(T).GetTypeInfo();
				if (typeInfo.IsNullable())
				{
					typeInfo = typeInfo.GenericTypeArguments[0].GetTypeInfo();
					object formatterDynamic = Instance.GetFormatterDynamic(typeInfo.AsType());
					if (formatterDynamic != null)
					{
						formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(typeof(StaticNullableFormatter<>).MakeGenericType(typeInfo.AsType()), formatterDynamic);
					}
				}
				else
				{
					TypeInfo typeInfo2 = BuildType(typeof(T));
					if (typeInfo2 != null)
					{
						formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(typeInfo2.AsType());
					}
				}
			}
		}

		private static class MessagePackBinaryTypeInfo
		{
			public static TypeInfo TypeInfo;

			public static MethodInfo WriteFixedMapHeaderUnsafe;

			public static MethodInfo WriteFixedArrayHeaderUnsafe;

			public static MethodInfo WriteMapHeader;

			public static MethodInfo WriteArrayHeader;

			public static MethodInfo WritePositiveFixedIntUnsafe;

			public static MethodInfo WriteInt32;

			public static MethodInfo WriteBytes;

			public static MethodInfo WriteNil;

			public static MethodInfo ReadBytes;

			public static MethodInfo ReadInt32;

			public static MethodInfo ReadString;

			public static MethodInfo IsNil;

			public static MethodInfo ReadNextBlock;

			public static MethodInfo WriteStringUnsafe;

			public static MethodInfo ReadArrayHeader;

			public static MethodInfo ReadMapHeader;

			static MessagePackBinaryTypeInfo()
			{
				TypeInfo = typeof(MessagePackBinary).GetTypeInfo();
				WriteFixedMapHeaderUnsafe = typeof(MessagePackBinary).GetRuntimeMethod("WriteFixedMapHeaderUnsafe", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(int)
				});
				WriteFixedArrayHeaderUnsafe = typeof(MessagePackBinary).GetRuntimeMethod("WriteFixedArrayHeaderUnsafe", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(int)
				});
				WriteMapHeader = typeof(MessagePackBinary).GetRuntimeMethod("WriteMapHeader", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(int)
				});
				WriteArrayHeader = typeof(MessagePackBinary).GetRuntimeMethod("WriteArrayHeader", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(int)
				});
				WritePositiveFixedIntUnsafe = typeof(MessagePackBinary).GetRuntimeMethod("WritePositiveFixedIntUnsafe", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(int)
				});
				WriteInt32 = typeof(MessagePackBinary).GetRuntimeMethod("WriteInt32", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(int)
				});
				WriteBytes = typeof(MessagePackBinary).GetRuntimeMethod("WriteBytes", new Type[3]
				{
					refByte,
					typeof(int),
					typeof(byte[])
				});
				WriteNil = typeof(MessagePackBinary).GetRuntimeMethod("WriteNil", new Type[2]
				{
					refByte,
					typeof(int)
				});
				ReadBytes = typeof(MessagePackBinary).GetRuntimeMethod("ReadBytes", new Type[3]
				{
					typeof(byte[]),
					typeof(int),
					refInt
				});
				ReadInt32 = typeof(MessagePackBinary).GetRuntimeMethod("ReadInt32", new Type[3]
				{
					typeof(byte[]),
					typeof(int),
					refInt
				});
				ReadString = typeof(MessagePackBinary).GetRuntimeMethod("ReadString", new Type[3]
				{
					typeof(byte[]),
					typeof(int),
					refInt
				});
				IsNil = typeof(MessagePackBinary).GetRuntimeMethod("IsNil", new Type[2]
				{
					typeof(byte[]),
					typeof(int)
				});
				ReadNextBlock = typeof(MessagePackBinary).GetRuntimeMethod("ReadNextBlock", new Type[2]
				{
					typeof(byte[]),
					typeof(int)
				});
				WriteStringUnsafe = typeof(MessagePackBinary).GetRuntimeMethod("WriteStringUnsafe", new Type[4]
				{
					refByte,
					typeof(int),
					typeof(string),
					typeof(int)
				});
				ReadArrayHeader = typeof(MessagePackBinary).GetRuntimeMethod("ReadArrayHeader", new Type[3]
				{
					typeof(byte[]),
					typeof(int),
					refInt
				});
				ReadMapHeader = typeof(MessagePackBinary).GetRuntimeMethod("ReadMapHeader", new Type[3]
				{
					typeof(byte[]),
					typeof(int),
					refInt
				});
			}
		}

		public static readonly DynamicUnionResolver Instance;

		private const string ModuleName = "MessagePack.Resolvers.DynamicUnionResolver";

		private static readonly DynamicAssembly assembly;

		private static readonly Regex SubtractFullNameRegex;

		private static readonly Type refByte;

		private static readonly Type refInt;

		private static readonly Type refKvp;

		private static readonly MethodInfo getFormatterWithVerify;

		private static readonly Func<Type, MethodInfo> getSerialize;

		private static readonly Func<Type, MethodInfo> getDeserialize;

		private static readonly FieldInfo runtimeTypeHandleEqualityComparer;

		private static readonly ConstructorInfo intIntKeyValuePairConstructor;

		private static readonly ConstructorInfo typeMapDictionaryConstructor;

		private static readonly MethodInfo typeMapDictionaryAdd;

		private static readonly MethodInfo typeMapDictionaryTryGetValue;

		private static readonly ConstructorInfo keyMapDictionaryConstructor;

		private static readonly MethodInfo keyMapDictionaryAdd;

		private static readonly MethodInfo keyMapDictionaryTryGetValue;

		private static readonly MethodInfo objectGetType;

		private static readonly MethodInfo getTypeHandle;

		private static readonly MethodInfo intIntKeyValuePairGetKey;

		private static readonly MethodInfo intIntKeyValuePairGetValue;

		private static readonly ConstructorInfo invalidOperationExceptionConstructor;

		private static readonly ConstructorInfo objectCtor;

		private DynamicUnionResolver()
		{
		}

		static DynamicUnionResolver()
		{
			Instance = new DynamicUnionResolver();
			SubtractFullNameRegex = new Regex(", Version=\\d+.\\d+.\\d+.\\d+, Culture=\\w+, PublicKeyToken=\\w+");
			refByte = typeof(byte[]).MakeByRefType();
			refInt = typeof(int).MakeByRefType();
			refKvp = typeof(KeyValuePair<int, int>).MakeByRefType();
			getFormatterWithVerify = typeof(FormatterResolverExtensions).GetRuntimeMethods().First((MethodInfo x) => x.Name == "GetFormatterWithVerify");
			getSerialize = (Type t) => typeof(IMessagePackFormatter<>).MakeGenericType(t).GetRuntimeMethod("Serialize", new Type[4]
			{
				refByte,
				typeof(int),
				t,
				typeof(IFormatterResolver)
			});
			getDeserialize = (Type t) => typeof(IMessagePackFormatter<>).MakeGenericType(t).GetRuntimeMethod("Deserialize", new Type[4]
			{
				typeof(byte[]),
				typeof(int),
				typeof(IFormatterResolver),
				refInt
			});
			runtimeTypeHandleEqualityComparer = typeof(RuntimeTypeHandleEqualityComparer).GetRuntimeField("Default");
			intIntKeyValuePairConstructor = typeof(KeyValuePair<int, int>).GetTypeInfo().DeclaredConstructors.First((ConstructorInfo x) => x.GetParameters().Length == 2);
			typeMapDictionaryConstructor = typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>).GetTypeInfo().DeclaredConstructors.First(delegate(ConstructorInfo x)
			{
				ParameterInfo[] parameters = x.GetParameters();
				return parameters.Length == 2 && parameters[0].ParameterType == typeof(int);
			});
			typeMapDictionaryAdd = typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>).GetRuntimeMethod("Add", new Type[2]
			{
				typeof(RuntimeTypeHandle),
				typeof(KeyValuePair<int, int>)
			});
			typeMapDictionaryTryGetValue = typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>).GetRuntimeMethod("TryGetValue", new Type[2]
			{
				typeof(RuntimeTypeHandle),
				refKvp
			});
			keyMapDictionaryConstructor = typeof(Dictionary<int, int>).GetTypeInfo().DeclaredConstructors.First(delegate(ConstructorInfo x)
			{
				ParameterInfo[] parameters2 = x.GetParameters();
				return parameters2.Length == 1 && parameters2[0].ParameterType == typeof(int);
			});
			keyMapDictionaryAdd = typeof(Dictionary<int, int>).GetRuntimeMethod("Add", new Type[2]
			{
				typeof(int),
				typeof(int)
			});
			keyMapDictionaryTryGetValue = typeof(Dictionary<int, int>).GetRuntimeMethod("TryGetValue", new Type[2]
			{
				typeof(int),
				refInt
			});
			objectGetType = typeof(object).GetRuntimeMethod("GetType", Type.EmptyTypes);
			getTypeHandle = typeof(Type).GetRuntimeProperty("TypeHandle").GetGetMethod();
			intIntKeyValuePairGetKey = typeof(KeyValuePair<int, int>).GetRuntimeProperty("Key").GetGetMethod();
			intIntKeyValuePairGetValue = typeof(KeyValuePair<int, int>).GetRuntimeProperty("Value").GetGetMethod();
			invalidOperationExceptionConstructor = typeof(InvalidOperationException).GetTypeInfo().DeclaredConstructors.First(delegate(ConstructorInfo x)
			{
				ParameterInfo[] parameters3 = x.GetParameters();
				return parameters3.Length == 1 && parameters3[0].ParameterType == typeof(string);
			});
			objectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.First((ConstructorInfo x) => x.GetParameters().Length == 0);
			assembly = new DynamicAssembly("MessagePack.Resolvers.DynamicUnionResolver");
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}

		private static TypeInfo BuildType(Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			UnionAttribute[] array = (from x in typeInfo.GetCustomAttributes<UnionAttribute>()
				orderby x.Key
				select x).ToArray();
			if (array.Length == 0)
			{
				return null;
			}
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<Type> hashSet2 = new HashSet<Type>();
			UnionAttribute[] array2 = array;
			foreach (UnionAttribute unionAttribute in array2)
			{
				if (!hashSet.Add(unionAttribute.Key))
				{
					throw new MessagePackDynamicUnionResolverException("Same union key has found. Type:" + type.Name + " Key:" + unionAttribute.Key);
				}
				if (!hashSet2.Add(unionAttribute.SubType))
				{
					throw new MessagePackDynamicUnionResolverException("Same union subType has found. Type:" + type.Name + " SubType: " + unionAttribute.SubType);
				}
			}
			Type type2 = typeof(IMessagePackFormatter<>).MakeGenericType(type);
			TypeBuilder typeBuilder = assembly.ModuleBuilder.DefineType("MessagePack.Formatters." + SubtractFullNameRegex.Replace(type.FullName, string.Empty).Replace(".", "_") + "Formatter", TypeAttributes.Public | TypeAttributes.Sealed, null, new Type[1] { type2 });
			FieldBuilder fieldBuilder = null;
			FieldBuilder fieldBuilder2 = null;
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
			fieldBuilder = typeBuilder.DefineField("typeToKeyAndJumpMap", typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>), FieldAttributes.Private | FieldAttributes.InitOnly);
			fieldBuilder2 = typeBuilder.DefineField("keyToJumpMap", typeof(Dictionary<int, int>), FieldAttributes.Private | FieldAttributes.InitOnly);
			ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
			BuildConstructor(type, array, constructorBuilder, fieldBuilder, fieldBuilder2, iLGenerator);
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual, typeof(int), new Type[4]
			{
				typeof(byte[]).MakeByRefType(),
				typeof(int),
				type,
				typeof(IFormatterResolver)
			});
			ILGenerator iLGenerator2 = methodBuilder.GetILGenerator();
			BuildSerialize(type, array, methodBuilder, fieldBuilder, iLGenerator2);
			MethodBuilder methodBuilder2 = typeBuilder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual, type, new Type[4]
			{
				typeof(byte[]),
				typeof(int),
				typeof(IFormatterResolver),
				typeof(int).MakeByRefType()
			});
			ILGenerator iLGenerator3 = methodBuilder2.GetILGenerator();
			BuildDeserialize(type, array, methodBuilder2, fieldBuilder2, iLGenerator3);
			return typeBuilder.CreateTypeInfo();
		}

		private static void BuildConstructor(Type type, UnionAttribute[] infos, ConstructorInfo method, FieldBuilder typeToKeyAndJumpMap, FieldBuilder keyToJumpMap, ILGenerator il)
		{
			il.EmitLdarg(0);
			il.Emit(OpCodes.Call, objectCtor);
			il.EmitLdarg(0);
			il.EmitLdc_I4(infos.Length);
			il.Emit(OpCodes.Ldsfld, runtimeTypeHandleEqualityComparer);
			il.Emit(OpCodes.Newobj, typeMapDictionaryConstructor);
			int num = 0;
			foreach (UnionAttribute unionAttribute in infos)
			{
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldtoken, unionAttribute.SubType);
				il.EmitLdc_I4(unionAttribute.Key);
				il.EmitLdc_I4(num);
				il.Emit(OpCodes.Newobj, intIntKeyValuePairConstructor);
				il.EmitCall(typeMapDictionaryAdd);
				num++;
			}
			il.Emit(OpCodes.Stfld, typeToKeyAndJumpMap);
			il.EmitLdarg(0);
			il.EmitLdc_I4(infos.Length);
			il.Emit(OpCodes.Newobj, keyMapDictionaryConstructor);
			int num2 = 0;
			foreach (UnionAttribute unionAttribute2 in infos)
			{
				il.Emit(OpCodes.Dup);
				il.EmitLdc_I4(unionAttribute2.Key);
				il.EmitLdc_I4(num2);
				il.EmitCall(keyMapDictionaryAdd);
				num2++;
			}
			il.Emit(OpCodes.Stfld, keyToJumpMap);
			il.Emit(OpCodes.Ret);
		}

		private static void BuildSerialize(Type type, UnionAttribute[] infos, MethodBuilder method, FieldBuilder typeToKeyAndJumpMap, ILGenerator il)
		{
			Label label = il.DefineLabel();
			Label label2 = il.DefineLabel();
			il.EmitLdarg(3);
			il.Emit(OpCodes.Brtrue_S, label);
			il.Emit(OpCodes.Br, label2);
			il.MarkLabel(label);
			LocalBuilder keyPair = il.DeclareLocal(typeof(KeyValuePair<int, int>));
			il.EmitLoadThis();
			il.EmitLdfld(typeToKeyAndJumpMap);
			il.EmitLdarg(3);
			il.EmitCall(objectGetType);
			il.EmitCall(getTypeHandle);
			il.EmitLdloca(keyPair);
			il.EmitCall(typeMapDictionaryTryGetValue);
			il.Emit(OpCodes.Brfalse, label2);
			LocalBuilder local = il.DeclareLocal(typeof(int));
			il.EmitLdarg(2);
			il.EmitStloc(local);
			EmitOffsetPlusEqual(il, null, delegate
			{
				il.EmitLdc_I4(2);
				il.EmitCall(MessagePackBinaryTypeInfo.WriteFixedArrayHeaderUnsafe);
			});
			EmitOffsetPlusEqual(il, null, delegate
			{
				il.EmitLdloca(keyPair);
				il.EmitCall(intIntKeyValuePairGetKey);
				il.EmitCall(MessagePackBinaryTypeInfo.WriteInt32);
			});
			Label label3 = il.DefineLabel();
			var array = infos.Select((UnionAttribute x) => new
			{
				Label = il.DefineLabel(),
				Attr = x
			}).ToArray();
			il.EmitLdloca(keyPair);
			il.EmitCall(intIntKeyValuePairGetValue);
			il.Emit(OpCodes.Switch, array.Select(x => x.Label).ToArray());
			il.Emit(OpCodes.Br, label3);
			var array2 = array;
			foreach (var item in array2)
			{
				il.MarkLabel(item.Label);
				EmitOffsetPlusEqual(il, delegate
				{
					il.EmitLdarg(4);
					il.Emit(OpCodes.Call, getFormatterWithVerify.MakeGenericMethod(item.Attr.SubType));
				}, delegate
				{
					il.EmitLdarg(3);
					if (item.Attr.SubType.GetTypeInfo().IsValueType)
					{
						il.Emit(OpCodes.Unbox_Any, item.Attr.SubType);
					}
					else
					{
						il.Emit(OpCodes.Castclass, item.Attr.SubType);
					}
					il.EmitLdarg(4);
					il.Emit(OpCodes.Callvirt, getSerialize(item.Attr.SubType));
				});
				il.Emit(OpCodes.Br, label3);
			}
			il.MarkLabel(label3);
			il.EmitLdarg(2);
			il.EmitLdloc(local);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(label2);
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitCall(MessagePackBinaryTypeInfo.WriteNil);
			il.Emit(OpCodes.Ret);
		}

		private static void EmitOffsetPlusEqual(ILGenerator il, Action loadEmit, Action emit)
		{
			il.EmitLdarg(2);
			if (loadEmit != null)
			{
				loadEmit();
			}
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			emit();
			il.Emit(OpCodes.Add);
			il.EmitStarg(2);
		}

		private static void BuildDeserialize(Type type, UnionAttribute[] infos, MethodBuilder method, FieldBuilder keyToJumpMap, ILGenerator il)
		{
			Label label = il.DefineLabel();
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitCall(MessagePackBinaryTypeInfo.IsNil);
			il.Emit(OpCodes.Brfalse_S, label);
			il.EmitLdarg(4);
			il.EmitLdc_I4(1);
			il.Emit(OpCodes.Stind_I4);
			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(label);
			LocalBuilder local = il.DeclareLocal(typeof(int));
			il.EmitLdarg(2);
			il.EmitStloc(local);
			Label label2 = il.DefineLabel();
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitLdarg(4);
			il.EmitCall(MessagePackBinaryTypeInfo.ReadArrayHeader);
			il.EmitLdc_I4(2);
			il.Emit(OpCodes.Beq_S, label2);
			il.Emit(OpCodes.Ldstr, "Invalid Union data was detected. Type:" + type.FullName);
			il.Emit(OpCodes.Newobj, invalidOperationExceptionConstructor);
			il.Emit(OpCodes.Throw);
			il.MarkLabel(label2);
			EmitOffsetPlusReadSize(il);
			LocalBuilder local2 = il.DeclareLocal(typeof(int));
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitLdarg(4);
			il.EmitCall(MessagePackBinaryTypeInfo.ReadInt32);
			il.EmitStloc(local2);
			EmitOffsetPlusReadSize(il);
			if (!IsZeroStartSequential(infos))
			{
				Label label3 = il.DefineLabel();
				il.EmitLdarg(0);
				il.EmitLdfld(keyToJumpMap);
				il.EmitLdloc(local2);
				il.EmitLdloca(local2);
				il.EmitCall(keyMapDictionaryTryGetValue);
				il.Emit(OpCodes.Brtrue_S, label3);
				il.EmitLdc_I4(-1);
				il.EmitStloc(local2);
				il.MarkLabel(label3);
			}
			LocalBuilder local3 = il.DeclareLocal(type);
			Label label4 = il.DefineLabel();
			il.Emit(OpCodes.Ldnull);
			il.EmitStloc(local3);
			il.Emit(OpCodes.Ldloc, local2);
			var array = infos.Select((UnionAttribute x) => new
			{
				Label = il.DefineLabel(),
				Attr = x
			}).ToArray();
			il.Emit(OpCodes.Switch, array.Select(x => x.Label).ToArray());
			il.EmitLdarg(2);
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitCall(MessagePackBinaryTypeInfo.ReadNextBlock);
			il.Emit(OpCodes.Add);
			il.EmitStarg(2);
			il.Emit(OpCodes.Br, label4);
			var array2 = array;
			foreach (var anon in array2)
			{
				il.MarkLabel(anon.Label);
				il.EmitLdarg(3);
				il.EmitCall(getFormatterWithVerify.MakeGenericMethod(anon.Attr.SubType));
				il.EmitLdarg(1);
				il.EmitLdarg(2);
				il.EmitLdarg(3);
				il.EmitLdarg(4);
				il.EmitCall(getDeserialize(anon.Attr.SubType));
				if (anon.Attr.SubType.GetTypeInfo().IsValueType)
				{
					il.Emit(OpCodes.Box, anon.Attr.SubType);
				}
				il.Emit(OpCodes.Stloc, local3);
				EmitOffsetPlusReadSize(il);
				il.Emit(OpCodes.Br, label4);
			}
			il.MarkLabel(label4);
			il.EmitLdarg(4);
			il.EmitLdarg(2);
			il.EmitLdloc(local);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Stind_I4);
			il.Emit(OpCodes.Ldloc, local3);
			il.Emit(OpCodes.Ret);
		}

		private static bool IsZeroStartSequential(UnionAttribute[] infos)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				if (infos[i].Key != i)
				{
					return false;
				}
			}
			return true;
		}

		private static void EmitOffsetPlusReadSize(ILGenerator il)
		{
			il.EmitLdarg(2);
			il.EmitLdarg(4);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Add);
			il.EmitStarg(2);
		}
	}
}
