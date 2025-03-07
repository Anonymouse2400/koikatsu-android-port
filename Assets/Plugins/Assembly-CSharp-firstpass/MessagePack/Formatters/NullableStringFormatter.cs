namespace MessagePack.Formatters
{
	public class NullableStringFormatter : IMessagePackFormatter<string>
	{
		public static readonly NullableStringFormatter Instance = new NullableStringFormatter();

		private NullableStringFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, string value, IFormatterResolver typeResolver)
		{
			return MessagePackBinary.WriteString(ref bytes, offset, value);
		}

		public string Deserialize(byte[] bytes, int offset, IFormatterResolver typeResolver, out int readSize)
		{
			return MessagePackBinary.ReadString(bytes, offset, out readSize);
		}
	}
}
