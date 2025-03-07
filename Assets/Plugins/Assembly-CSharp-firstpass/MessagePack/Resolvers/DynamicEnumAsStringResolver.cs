using System;
using System.Reflection;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class DynamicEnumAsStringResolver : IFormatterResolver
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
					formatter = new EnumAsStringFormatter<T>();
				}
			}
		}

		public static readonly IFormatterResolver Instance = new DynamicEnumAsStringResolver();

		private DynamicEnumAsStringResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
