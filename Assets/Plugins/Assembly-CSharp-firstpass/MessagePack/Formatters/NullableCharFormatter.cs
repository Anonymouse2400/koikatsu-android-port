namespace MessagePack.Formatters
{
	public class NullableCharFormatter : IMessagePackFormatter<char?>
	{
		public static readonly NullableCharFormatter Instance = new NullableCharFormatter();

		private NullableCharFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, char? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteChar(ref bytes, offset, value.Value);
		}

		public char? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadChar(bytes, offset, out readSize);
		}
	}
}
