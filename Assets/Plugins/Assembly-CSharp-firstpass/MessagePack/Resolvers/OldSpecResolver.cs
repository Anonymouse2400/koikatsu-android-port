using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class OldSpecResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				formatter = (IMessagePackFormatter<T>)OldSpecResolverGetFormatterHelper.GetFormatter(typeof(T));
			}
		}

		public static readonly IFormatterResolver Instance = new OldSpecResolver();

		private OldSpecResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
