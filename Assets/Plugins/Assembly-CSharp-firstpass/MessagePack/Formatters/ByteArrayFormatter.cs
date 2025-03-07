namespace MessagePack.Formatters
{
	public class ByteArrayFormatter : IMessagePackFormatter<byte[]>
	{
		public static readonly ByteArrayFormatter Instance = new ByteArrayFormatter();

		private ByteArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, byte[] value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteBytes(ref bytes, offset, value);
		}

		public byte[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadBytes(bytes, offset, out readSize);
		}
	}
}
