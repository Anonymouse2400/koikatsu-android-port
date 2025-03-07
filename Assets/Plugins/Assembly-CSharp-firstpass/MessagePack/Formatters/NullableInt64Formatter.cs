namespace MessagePack.Formatters
{
	public class NullableInt64Formatter : IMessagePackFormatter<long?>
	{
		public static readonly NullableInt64Formatter Instance = new NullableInt64Formatter();

		private NullableInt64Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, long? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteInt64(ref bytes, offset, value.Value);
		}

		public long? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
		}
	}
}
