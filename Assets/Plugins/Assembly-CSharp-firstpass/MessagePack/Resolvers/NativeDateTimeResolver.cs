using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
	public class NativeDateTimeResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				formatter = (IMessagePackFormatter<T>)NativeDateTimeResolverGetFormatterHelper.GetFormatter(typeof(T));
			}
		}

		public static readonly IFormatterResolver Instance = new NativeDateTimeResolver();

		private NativeDateTimeResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
