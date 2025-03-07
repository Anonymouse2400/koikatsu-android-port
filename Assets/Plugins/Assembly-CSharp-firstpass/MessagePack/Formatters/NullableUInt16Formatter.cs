namespace MessagePack.Formatters
{
	public class NullableUInt16Formatter : IMessagePackFormatter<ushort?>
	{
		public static readonly NullableUInt16Formatter Instance = new NullableUInt16Formatter();

		private NullableUInt16Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, ushort? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteUInt16(ref bytes, offset, value.Value);
		}

		public ushort? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
		}
	}
}
