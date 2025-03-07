using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class BuiltinResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				formatter = (IMessagePackFormatter<T>)BuiltinResolverGetFormatterHelper.GetFormatter(typeof(T));
			}
		}

		public static readonly IFormatterResolver Instance = new BuiltinResolver();

		private BuiltinResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
