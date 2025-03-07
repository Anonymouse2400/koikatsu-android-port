namespace MessagePack.Formatters
{
	public class NullableSByteFormatter : IMessagePackFormatter<sbyte?>
	{
		public static readonly NullableSByteFormatter Instance = new NullableSByteFormatter();

		private NullableSByteFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, sbyte? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteSByte(ref bytes, offset, value.Value);
		}

		public sbyte? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
		}
	}
}
