using System;
using System.Reflection;
using System.Reflection.Emit;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class DynamicEnumResolver : IFormatterResolver
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
					if (typeInfo.IsEnum)
					{
						object formatterDynamic = Instance.GetFormatterDynamic(typeInfo.AsType());
						if (formatterDynamic != null)
						{
							formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(typeof(StaticNullableFormatter<>).MakeGenericType(typeInfo.AsType()), formatterDynamic);
						}
					}
				}
				else if (typeInfo.IsEnum)
				{
					TypeInfo typeInfo2 = BuildType(typeof(T));
					formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(typeInfo2.AsType());
				}
			}
		}

		public static readonly DynamicEnumResolver Instance;

		private const string ModuleName = "MessagePack.Resolvers.DynamicEnumResolver";

		private static readonly DynamicAssembly assembly;

		private DynamicEnumResolver()
		{
		}

		static DynamicEnumResolver()
		{
			Instance = new DynamicEnumResolver();
			assembly = new DynamicAssembly("MessagePack.Resolvers.DynamicEnumResolver");
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}

		private static TypeInfo BuildType(Type enumType)
		{
			Type underlyingType = Enum.GetUnderlyingType(enumType);
			Type type = typeof(IMessagePackFormatter<>).MakeGenericType(enumType);
			TypeBuilder typeBuilder = assembly.ModuleBuilder.DefineType("MessagePack.Formatters." + enumType.FullName.Replace(".", "_") + "Formatter", TypeAttributes.Public | TypeAttributes.Sealed, null, new Type[1] { type });
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual, typeof(int), new Type[4]
			{
				typeof(byte[]).MakeByRefType(),
				typeof(int),
				enumType,
				typeof(IFormatterResolver)
			});
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Ldarg_2);
			iLGenerator.Emit(OpCodes.Ldarg_3);
			iLGenerator.Emit(OpCodes.Call, typeof(MessagePackBinary).GetTypeInfo().GetDeclaredMethod("Write" + underlyingType.Name));
			iLGenerator.Emit(OpCodes.Ret);
			MethodBuilder methodBuilder2 = typeBuilder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual, enumType, new Type[4]
			{
				typeof(byte[]),
				typeof(int),
				typeof(IFormatterResolver),
				typeof(int).MakeByRefType()
			});
			ILGenerator iLGenerator2 = methodBuilder2.GetILGenerator();
			iLGenerator2.Emit(OpCodes.Ldarg_1);
			iLGenerator2.Emit(OpCodes.Ldarg_2);
			iLGenerator2.Emit(OpCodes.Ldarg_S, (byte)4);
			iLGenerator2.Emit(OpCodes.Call, typeof(MessagePackBinary).GetTypeInfo().GetDeclaredMethod("Read" + underlyingType.Name));
			iLGenerator2.Emit(OpCodes.Ret);
			return typeBuilder.CreateTypeInfo();
		}
	}
}
