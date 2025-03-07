namespace MessagePack.Formatters
{
	public class NullableInt16Formatter : IMessagePackFormatter<short?>
	{
		public static readonly NullableInt16Formatter Instance = new NullableInt16Formatter();

		private NullableInt16Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, short? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteInt16(ref bytes, offset, value.Value);
		}

		public short? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadInt16(bytes, offset, out readSize);
		}
	}
}
