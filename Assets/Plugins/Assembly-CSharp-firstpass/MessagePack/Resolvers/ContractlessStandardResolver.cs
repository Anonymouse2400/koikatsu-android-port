using MessagePack.Formatters;

namespace MessagePack.Resolvers
{
	public class ContractlessStandardResolver : IFormatterResolver
	{
		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> formatter;

			static FormatterCache()
			{
				IFormatterResolver[] resolvers = ContractlessStandardResolver.resolvers;
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

		public static readonly IFormatterResolver Instance = new ContractlessStandardResolver();

		private static readonly IFormatterResolver[] resolvers = new IFormatterResolver[2]
		{
			StandardResolver.Instance,
			DynamicContractlessObjectResolver.Instance
		};

		private ContractlessStandardResolver()
		{
		}

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return FormatterCache<T>.formatter;
		}
	}
}
