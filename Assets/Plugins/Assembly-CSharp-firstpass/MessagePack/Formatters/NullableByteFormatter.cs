namespace MessagePack.Formatters
{
	public class NullableByteFormatter : IMessagePackFormatter<byte?>
	{
		public static readonly NullableByteFormatter Instance = new NullableByteFormatter();

		private NullableByteFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, byte? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteByte(ref bytes, offset, value.Value);
		}

		public byte? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadByte(bytes, offset, out readSize);
		}
	}
}
