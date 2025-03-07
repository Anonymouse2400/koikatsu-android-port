using System.Text;

namespace MessagePack.Formatters
{
	public class StringBuilderFormatter : IMessagePackFormatter<StringBuilder>
	{
		public static readonly IMessagePackFormatter<StringBuilder> Instance = new StringBuilderFormatter();

		private StringBuilderFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, StringBuilder value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteString(ref bytes, offset, value.ToString());
		}

		public StringBuilder Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return new StringBuilder(MessagePackBinary.ReadString(bytes, offset, out readSize));
		}
	}
}
