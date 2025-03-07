using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class DynamicGenericResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				formatter = (IMessagePackFormatter<T>)DynamicGenericResolverGetFormatterHelper.GetFormatter(typeof(T));
			}
		}

		public static readonly IFormatterResolver Instance = new DynamicGenericResolver();

		private DynamicGenericResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
