using MessagePack.Formatters;

namespace MessagePack.Unity
{
	public class UnityResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				formatter = (IMessagePackFormatter<T>)UnityResolveryResolverGetFormatterHelper.GetFormatter(typeof(T));
			}
		}

		public static IFormatterResolver Instance = new UnityResolver();

		private UnityResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
