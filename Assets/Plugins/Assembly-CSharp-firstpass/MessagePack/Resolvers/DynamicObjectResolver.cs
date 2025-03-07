using System;
using System.Reflection;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class DynamicObjectResolver : IFormatterResolver
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
					TypeInfo typeInfo2 = DynamicObjectTypeBuilder.BuildType(assembly, typeof(T), false);
					if (typeInfo2 != null)
					{
						formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(typeInfo2.AsType());
					}
				}
			}
		}

		public static readonly DynamicObjectResolver Instance;

		private const string ModuleName = "MessagePack.Resolvers.DynamicObjectResolver";

		internal static readonly DynamicAssembly assembly;

		private DynamicObjectResolver()
		{
		}

		static DynamicObjectResolver()
		{
			Instance = new DynamicObjectResolver();
			assembly = new DynamicAssembly("MessagePack.Resolvers.DynamicObjectResolver");
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
