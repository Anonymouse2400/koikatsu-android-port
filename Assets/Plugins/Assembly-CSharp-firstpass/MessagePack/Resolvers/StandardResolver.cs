using MessagePack.Formatters;
using MessagePack.Unity;

namespace MessagePack.Resolvers
{
	public class StandardResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				IFormatterResolver[] resolvers = StandardResolver.resolvers;
				foreach (IFormatterResolver formatterResolver in resolvers)
				{
					IMessagePackFormatter<T> messagePackFormatter = formatterResolver.GetFormatter<T>();
					if (messagePackFormatter != null)
					{
						formatter = messagePackFormatter;
						break;
					}
				}
			}
		}

		public static readonly IFormatterResolver Instance = new StandardResolver();

		private static readonly IFormatterResolver[] resolvers = new IFormatterResolver[7]
		{
			BuiltinResolver.Instance,
			UnityResolver.Instance,
			DynamicEnumResolver.Instance,
			DynamicGenericResolver.Instance,
			DynamicUnionResolver.Instance,
			DynamicObjectResolver.Instance,
			PrimitiveObjectResolver.Instance
		};

		private StandardResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
