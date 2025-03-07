namespace MessagePack.Formatters
{
	public class NullableUInt32Formatter : IMessagePackFormatter<uint?>
	{
		public static readonly NullableUInt32Formatter Instance = new NullableUInt32Formatter();

		private NullableUInt32Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, uint? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteUInt32(ref bytes, offset, value.Value);
		}

		public uint? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
		}
	}
}
