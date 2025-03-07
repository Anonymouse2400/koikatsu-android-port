using System;
using MessagePack.Formatters;

namespace MessagePack.Resolvers
{
	public class CompositeResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				isFreezed = true;
				IFormatterResolver[] resolvers = CompositeResolver.resolvers;
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

		public static readonly CompositeResolver Instance = new CompositeResolver();

		private static bool isFreezed = false;

		private static IFormatterResolver[] resolvers = new IFormatterResolver[0];

		private CompositeResolver()
		{
		}

		public static void Register(params IFormatterResolver[] resolvers)
		{
			if (isFreezed)
			{
				throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
			}
			CompositeResolver.resolvers = resolvers;
		}

		public static void RegisterAndSetAsDefault(params IFormatterResolver[] resolvers)
		{
			Register(resolvers);
			MessagePackSerializer.SetDefaultResolver(Instance);
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
