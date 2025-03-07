namespace MessagePack.Formatters
{
	public class NullableUInt64Formatter : IMessagePackFormatter<ulong?>
	{
		public static readonly NullableUInt64Formatter Instance = new NullableUInt64Formatter();

		private NullableUInt64Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, ulong? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteUInt64(ref bytes, offset, value.Value);
		}

		public ulong? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
		}
	}
}
