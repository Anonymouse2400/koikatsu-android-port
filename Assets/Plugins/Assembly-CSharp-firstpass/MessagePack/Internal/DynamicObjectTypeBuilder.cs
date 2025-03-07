using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using MessagePack.Formatters;

namespace MessagePack.Internal
{
	internal static class DynamicObjectTypeBuilder
	{
		internal static class MessagePackBinaryTypeInfo
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

		private class DeserializeInfo
		{
			public ObjectSerializationInfo.EmittableMember MemberInfo { get; set; }

			public LocalBuilder LocalField { get; set; }

			public Label SwitchLabel { get; set; }
		}

		private static readonly Regex SubtractFullNameRegex = new Regex(", Version=\\d+.\\d+.\\d+.\\d+, Culture=\\w+, PublicKeyToken=\\w+");

		private static HashSet<Type> ignoreTypes = new HashSet<Type>
		{
			typeof(object),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(decimal),
			typeof(char),
			typeof(string),
			typeof(Guid),
			typeof(TimeSpan),
			typeof(DateTime),
			typeof(DateTimeOffset),
			typeof(Nil)
		};

		private static readonly Type refByte = typeof(byte[]).MakeByRefType();

		private static readonly Type refInt = typeof(int).MakeByRefType();

		private static readonly MethodInfo getFormatterWithVerify = typeof(FormatterResolverExtensions).GetRuntimeMethods().First((MethodInfo x) => x.Name == "GetFormatterWithVerify");

		private static readonly Func<Type, MethodInfo> getSerialize = (Type t) => typeof(IMessagePackFormatter<>).MakeGenericType(t).GetRuntimeMethod("Serialize", new Type[4]
		{
			refByte,
			typeof(int),
			t,
			typeof(IFormatterResolver)
		});

		private static readonly Func<Type, MethodInfo> getDeserialize = (Type t) => typeof(IMessagePackFormatter<>).MakeGenericType(t).GetRuntimeMethod("Deserialize", new Type[4]
		{
			typeof(byte[]),
			typeof(int),
			typeof(IFormatterResolver),
			refInt
		});

		private static readonly ConstructorInfo dictionaryConstructor = typeof(Dictionary<string, int>).GetTypeInfo().DeclaredConstructors.First(delegate(ConstructorInfo x)
		{
			ParameterInfo[] parameters = x.GetParameters();
			return parameters.Length == 1 && parameters[0].ParameterType == typeof(int);
		});

		private static readonly MethodInfo dictionaryAdd = typeof(Dictionary<string, int>).GetRuntimeMethod("Add", new Type[2]
		{
			typeof(string),
			typeof(int)
		});

		private static readonly MethodInfo dictionaryTryGetValue = typeof(Dictionary<string, int>).GetRuntimeMethod("TryGetValue", new Type[2]
		{
			typeof(string),
			refInt
		});

		private static readonly ConstructorInfo invalidOperationExceptionConstructor = typeof(InvalidOperationException).GetTypeInfo().DeclaredConstructors.First(delegate(ConstructorInfo x)
		{
			ParameterInfo[] parameters2 = x.GetParameters();
			return parameters2.Length == 1 && parameters2[0].ParameterType == typeof(string);
		});

		private static readonly MethodInfo onBeforeSerialize = typeof(IMessagePackSerializationCallbackReceiver).GetRuntimeMethod("OnBeforeSerialize", Type.EmptyTypes);

		private static readonly MethodInfo onAfterDeserialize = typeof(IMessagePackSerializationCallbackReceiver).GetRuntimeMethod("OnAfterDeserialize", Type.EmptyTypes);

		private static readonly ConstructorInfo objectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.First((ConstructorInfo x) => x.GetParameters().Length == 0);

		public static TypeInfo BuildType(DynamicAssembly assembly, Type type, bool forceStringKey)
		{
			if (ignoreTypes.Contains(type))
			{
				return null;
			}
			ObjectSerializationInfo objectSerializationInfo = ObjectSerializationInfo.CreateOrNull(type, forceStringKey);
			if (objectSerializationInfo == null)
			{
				return null;
			}
			Type type2 = typeof(IMessagePackFormatter<>).MakeGenericType(type);
			TypeBuilder typeBuilder = assembly.ModuleBuilder.DefineType("MessagePack.Formatters." + SubtractFullNameRegex.Replace(type.FullName, string.Empty).Replace(".", "_") + "Formatter", TypeAttributes.Public | TypeAttributes.Sealed, null, new Type[1] { type2 });
			FieldBuilder dictionaryField = null;
			if (objectSerializationInfo.IsStringKey)
			{
				ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
				dictionaryField = typeBuilder.DefineField("keyMapping", typeof(Dictionary<string, int>), FieldAttributes.Private | FieldAttributes.InitOnly);
				ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
				BuildConstructor(type, objectSerializationInfo, constructorBuilder, dictionaryField, iLGenerator);
			}
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual, typeof(int), new Type[4]
			{
				typeof(byte[]).MakeByRefType(),
				typeof(int),
				type,
				typeof(IFormatterResolver)
			});
			ILGenerator iLGenerator2 = methodBuilder.GetILGenerator();
			BuildSerialize(type, objectSerializationInfo, methodBuilder, iLGenerator2);
			MethodBuilder methodBuilder2 = typeBuilder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual, type, new Type[4]
			{
				typeof(byte[]),
				typeof(int),
				typeof(IFormatterResolver),
				typeof(int).MakeByRefType()
			});
			ILGenerator iLGenerator3 = methodBuilder2.GetILGenerator();
			BuildDeserialize(type, objectSerializationInfo, methodBuilder2, dictionaryField, iLGenerator3);
			return typeBuilder.CreateTypeInfo();
		}

		private static void BuildConstructor(Type type, ObjectSerializationInfo info, ConstructorInfo method, FieldBuilder dictionaryField, ILGenerator il)
		{
			il.EmitLdarg(0);
			il.Emit(OpCodes.Call, objectCtor);
			il.EmitLdarg(0);
			il.EmitLdc_I4(info.Members.Length);
			il.Emit(OpCodes.Newobj, dictionaryConstructor);
			ObjectSerializationInfo.EmittableMember[] members = info.Members;
			foreach (ObjectSerializationInfo.EmittableMember emittableMember in members)
			{
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldstr, emittableMember.StringKey);
				il.EmitLdc_I4(emittableMember.IntKey);
				il.EmitCall(dictionaryAdd);
			}
			il.Emit(OpCodes.Stfld, dictionaryField);
			il.Emit(OpCodes.Ret);
		}

		private static void BuildSerialize(Type type, ObjectSerializationInfo info, MethodInfo method, ILGenerator il)
		{
			if (type.GetTypeInfo().IsClass)
			{
				Label label = il.DefineLabel();
				il.EmitLdarg(3);
				il.Emit(OpCodes.Brtrue_S, label);
				il.EmitLdarg(1);
				il.EmitLdarg(2);
				il.EmitCall(MessagePackBinaryTypeInfo.WriteNil);
				il.Emit(OpCodes.Ret);
				il.MarkLabel(label);
			}
			if (type.GetTypeInfo().ImplementedInterfaces.Any((Type x) => x == typeof(IMessagePackSerializationCallbackReceiver)))
			{
				MethodInfo[] array = (from x in type.GetRuntimeMethods()
					where x.Name == "OnBeforeSerialize"
					select x).ToArray();
				if (array.Length == 1)
				{
					if (info.IsStruct)
					{
						il.EmitLdarga(3);
					}
					else
					{
						il.EmitLdarg(3);
					}
					il.Emit(OpCodes.Call, array[0]);
				}
				else
				{
					il.EmitLdarg(3);
					if (info.IsStruct)
					{
						il.Emit(OpCodes.Box, type);
					}
					il.EmitCall(onBeforeSerialize);
				}
			}
			LocalBuilder local = il.DeclareLocal(typeof(int));
			il.EmitLdarg(2);
			il.EmitStloc(local);
			if (info.IsIntKey)
			{
				int maxKey = (from x in info.Members
					where x.IsReadable
					select x.IntKey).DefaultIfEmpty(-1).Max();
				Dictionary<int, ObjectSerializationInfo.EmittableMember> dictionary = info.Members.Where((ObjectSerializationInfo.EmittableMember x) => x.IsReadable).ToDictionary((ObjectSerializationInfo.EmittableMember x) => x.IntKey);
				EmitOffsetPlusEqual(il, null, delegate
				{
					int num = maxKey + 1;
					il.EmitLdc_I4(num);
					if (num <= 15)
					{
						il.EmitCall(MessagePackBinaryTypeInfo.WriteFixedArrayHeaderUnsafe);
					}
					else
					{
						il.EmitCall(MessagePackBinaryTypeInfo.WriteArrayHeader);
					}
				});
				for (int i = 0; i <= maxKey; i++)
				{
					ObjectSerializationInfo.EmittableMember value;
					if (dictionary.TryGetValue(i, out value))
					{
						EmitSerializeValue(il, type.GetTypeInfo(), value);
						continue;
					}
					EmitOffsetPlusEqual(il, null, delegate
					{
						il.EmitCall(MessagePackBinaryTypeInfo.WriteNil);
					});
				}
			}
			else
			{
				int writeCount = info.Members.Count((ObjectSerializationInfo.EmittableMember x) => x.IsReadable);
				EmitOffsetPlusEqual(il, null, delegate
				{
					il.EmitLdc_I4(writeCount);
					if (writeCount <= 15)
					{
						il.EmitCall(MessagePackBinaryTypeInfo.WriteFixedMapHeaderUnsafe);
					}
					else
					{
						il.EmitCall(MessagePackBinaryTypeInfo.WriteMapHeader);
					}
				});
				foreach (ObjectSerializationInfo.EmittableMember item in info.Members.Where((ObjectSerializationInfo.EmittableMember x) => x.IsReadable))
				{
					if (info.IsStringKey)
					{
						EmitOffsetPlusEqual(il, null, delegate
						{
							il.Emit(OpCodes.Ldstr, item.StringKey);
							il.EmitLdc_I4(StringEncoding.UTF8.GetByteCount(item.StringKey));
							il.EmitCall(MessagePackBinaryTypeInfo.WriteStringUnsafe);
						});
					}
					EmitSerializeValue(il, type.GetTypeInfo(), item);
				}
			}
			il.EmitLdarg(2);
			il.EmitLdloc(local);
			il.Emit(OpCodes.Sub);
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

		private static void EmitSerializeValue(ILGenerator il, TypeInfo type, ObjectSerializationInfo.EmittableMember member)
		{
			Type t = member.Type;
			if (IsOptimizeTargetType(t))
			{
				EmitOffsetPlusEqual(il, null, delegate
				{
					il.EmitLoadArg(type, 3);
					member.EmitLoadValue(il);
					if (t == typeof(byte[]))
					{
						il.EmitCall(MessagePackBinaryTypeInfo.WriteBytes);
					}
					else
					{
						il.EmitCall(MessagePackBinaryTypeInfo.TypeInfo.GetDeclaredMethod("Write" + t.Name));
					}
				});
				return;
			}
			EmitOffsetPlusEqual(il, delegate
			{
				il.EmitLdarg(4);
				il.Emit(OpCodes.Call, getFormatterWithVerify.MakeGenericMethod(t));
			}, delegate
			{
				il.EmitLoadArg(type, 3);
				member.EmitLoadValue(il);
				il.EmitLdarg(4);
				il.EmitCall(getSerialize(t));
			});
		}

		private static void BuildDeserialize(Type type, ObjectSerializationInfo info, MethodBuilder method, FieldBuilder dictionaryField, ILGenerator il)
		{
			Label label = il.DefineLabel();
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitCall(MessagePackBinaryTypeInfo.IsNil);
			il.Emit(OpCodes.Brfalse_S, label);
			if (type.GetTypeInfo().IsClass)
			{
				il.EmitLdarg(4);
				il.EmitLdc_I4(1);
				il.Emit(OpCodes.Stind_I4);
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Ret);
			}
			else
			{
				il.Emit(OpCodes.Ldstr, "typecode is null, struct not supported");
				il.Emit(OpCodes.Newobj, invalidOperationExceptionConstructor);
				il.Emit(OpCodes.Throw);
			}
			il.MarkLabel(label);
			LocalBuilder local = il.DeclareLocal(typeof(int));
			il.EmitLdarg(2);
			il.EmitStloc(local);
			LocalBuilder localBuilder = il.DeclareLocal(typeof(int));
			il.EmitLdarg(1);
			il.EmitLdarg(2);
			il.EmitLdarg(4);
			if (info.IsIntKey)
			{
				il.EmitCall(MessagePackBinaryTypeInfo.ReadArrayHeader);
			}
			else
			{
				il.EmitCall(MessagePackBinaryTypeInfo.ReadMapHeader);
			}
			il.EmitStloc(localBuilder);
			EmitOffsetPlusReadSize(il);
			Label? gotoDefault = null;
			DeserializeInfo[] infoList;
			if (info.IsIntKey)
			{
				int num = info.Members.Select((ObjectSerializationInfo.EmittableMember x) => x.IntKey).DefaultIfEmpty(-1).Max();
				int count = num + 1;
				Dictionary<int, ObjectSerializationInfo.EmittableMember> intKeyMap = info.Members.ToDictionary((ObjectSerializationInfo.EmittableMember x) => x.IntKey);
				infoList = Enumerable.Range(0, count).Select(delegate(int x)
				{
					ObjectSerializationInfo.EmittableMember value;
					if (intKeyMap.TryGetValue(x, out value))
					{
						return new DeserializeInfo
						{
							MemberInfo = value,
							LocalField = il.DeclareLocal(value.Type),
							SwitchLabel = il.DefineLabel()
						};
					}
					if (!gotoDefault.HasValue)
					{
						gotoDefault = il.DefineLabel();
					}
					return new DeserializeInfo
					{
						MemberInfo = null,
						LocalField = null,
						SwitchLabel = gotoDefault.Value
					};
				}).ToArray();
			}
			else
			{
				infoList = info.Members.Select((ObjectSerializationInfo.EmittableMember item) => new DeserializeInfo
				{
					MemberInfo = item,
					LocalField = il.DeclareLocal(item.Type),
					SwitchLabel = il.DefineLabel()
				}).ToArray();
			}
			LocalBuilder key = il.DeclareLocal(typeof(int));
			Label switchDefault = il.DefineLabel();
			Label loopEnd = il.DefineLabel();
			Label stringKeyTrue = il.DefineLabel();
			il.EmitIncrementFor(localBuilder, delegate(LocalBuilder forILocal)
			{
				if (info.IsStringKey)
				{
					il.EmitLdarg(0);
					il.Emit(OpCodes.Ldfld, dictionaryField);
					il.EmitLdarg(1);
					il.EmitLdarg(2);
					il.EmitLdarg(4);
					il.EmitCall(MessagePackBinaryTypeInfo.ReadString);
					il.EmitLdloca(key);
					il.EmitCall(dictionaryTryGetValue);
					EmitOffsetPlusReadSize(il);
					il.Emit(OpCodes.Brtrue_S, stringKeyTrue);
					il.EmitLdarg(4);
					il.EmitLdarg(1);
					il.EmitLdarg(2);
					il.EmitCall(MessagePackBinaryTypeInfo.ReadNextBlock);
					il.Emit(OpCodes.Stind_I4);
					il.Emit(OpCodes.Br, loopEnd);
					il.MarkLabel(stringKeyTrue);
				}
				else
				{
					il.EmitLdloc(forILocal);
					il.EmitStloc(key);
				}
				il.EmitLdloc(key);
				il.Emit(OpCodes.Switch, infoList.Select((DeserializeInfo x) => x.SwitchLabel).ToArray());
				il.MarkLabel(switchDefault);
				il.EmitLdarg(4);
				il.EmitLdarg(1);
				il.EmitLdarg(2);
				il.EmitCall(MessagePackBinaryTypeInfo.ReadNextBlock);
				il.Emit(OpCodes.Stind_I4);
				il.Emit(OpCodes.Br, loopEnd);
				if (gotoDefault.HasValue)
				{
					il.MarkLabel(gotoDefault.Value);
					il.Emit(OpCodes.Br, switchDefault);
				}
				DeserializeInfo[] array = infoList;
				foreach (DeserializeInfo deserializeInfo in array)
				{
					if (deserializeInfo.MemberInfo != null)
					{
						il.MarkLabel(deserializeInfo.SwitchLabel);
						EmitDeserializeValue(il, deserializeInfo);
						il.Emit(OpCodes.Br, loopEnd);
					}
				}
				il.MarkLabel(loopEnd);
				EmitOffsetPlusReadSize(il);
			});
			il.EmitLdarg(4);
			il.EmitLdarg(2);
			il.EmitLdloc(local);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Stind_I4);
			LocalBuilder local2 = EmitNewObject(il, type, info, infoList);
			if (type.GetTypeInfo().ImplementedInterfaces.Any((Type x) => x == typeof(IMessagePackSerializationCallbackReceiver)))
			{
				MethodInfo[] array2 = (from x in type.GetRuntimeMethods()
					where x.Name == "OnAfterDeserialize"
					select x).ToArray();
				if (array2.Length == 1)
				{
					if (info.IsClass)
					{
						il.Emit(OpCodes.Dup);
					}
					else
					{
						il.EmitLdloca(local2);
					}
					il.Emit(OpCodes.Call, array2[0]);
				}
				else
				{
					if (info.IsStruct)
					{
						il.EmitLdloc(local2);
						il.Emit(OpCodes.Box, type);
					}
					else
					{
						il.Emit(OpCodes.Dup);
					}
					il.EmitCall(onAfterDeserialize);
				}
			}
			if (info.IsStruct)
			{
				il.Emit(OpCodes.Ldloc, local2);
			}
			il.Emit(OpCodes.Ret);
		}

		private static void EmitOffsetPlusReadSize(ILGenerator il)
		{
			il.EmitLdarg(2);
			il.EmitLdarg(4);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Add);
			il.EmitStarg(2);
		}

		private static void EmitDeserializeValue(ILGenerator il, DeserializeInfo info)
		{
			ObjectSerializationInfo.EmittableMember memberInfo = info.MemberInfo;
			Type type = memberInfo.Type;
			if (IsOptimizeTargetType(type))
			{
				il.EmitLdarg(1);
				il.EmitLdarg(2);
				il.EmitLdarg(4);
				if (type == typeof(byte[]))
				{
					il.EmitCall(MessagePackBinaryTypeInfo.ReadBytes);
				}
				else
				{
					il.EmitCall(MessagePackBinaryTypeInfo.TypeInfo.GetDeclaredMethod("Read" + type.Name));
				}
			}
			else
			{
				il.EmitLdarg(3);
				il.EmitCall(getFormatterWithVerify.MakeGenericMethod(type));
				il.EmitLdarg(1);
				il.EmitLdarg(2);
				il.EmitLdarg(3);
				il.EmitLdarg(4);
				il.EmitCall(getDeserialize(type));
			}
			il.EmitStloc(info.LocalField);
		}

		private static LocalBuilder EmitNewObject(ILGenerator il, Type type, ObjectSerializationInfo info, DeserializeInfo[] members)
		{
			if (info.IsClass)
			{
				ObjectSerializationInfo.EmittableMember[] constructorParameters = info.ConstructorParameters;
				foreach (ObjectSerializationInfo.EmittableMember item in constructorParameters)
				{
					DeserializeInfo deserializeInfo = members.First((DeserializeInfo x) => x.MemberInfo == item);
					il.EmitLdloc(deserializeInfo.LocalField);
				}
				il.Emit(OpCodes.Newobj, info.BestmatchConstructor);
				foreach (DeserializeInfo item3 in members.Where((DeserializeInfo x) => x.MemberInfo != null && x.MemberInfo.IsWritable))
				{
					il.Emit(OpCodes.Dup);
					il.EmitLdloc(item3.LocalField);
					item3.MemberInfo.EmitStoreValue(il);
				}
				return null;
			}
			LocalBuilder localBuilder = il.DeclareLocal(type);
			if (info.BestmatchConstructor == null)
			{
				il.Emit(OpCodes.Ldloca, localBuilder);
				il.Emit(OpCodes.Initobj, type);
			}
			else
			{
				ObjectSerializationInfo.EmittableMember[] constructorParameters2 = info.ConstructorParameters;
				foreach (ObjectSerializationInfo.EmittableMember item2 in constructorParameters2)
				{
					DeserializeInfo deserializeInfo2 = members.First((DeserializeInfo x) => x.MemberInfo == item2);
					il.EmitLdloc(deserializeInfo2.LocalField);
				}
				il.Emit(OpCodes.Newobj, info.BestmatchConstructor);
				il.Emit(OpCodes.Stloc, localBuilder);
			}
			foreach (DeserializeInfo item4 in members.Where((DeserializeInfo x) => x.MemberInfo != null && x.MemberInfo.IsWritable))
			{
				il.EmitLdloca(localBuilder);
				il.EmitLdloc(item4.LocalField);
				item4.MemberInfo.EmitStoreValue(il);
			}
			return localBuilder;
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
