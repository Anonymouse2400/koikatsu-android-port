namespace MessagePack.Formatters
{
	public class NullableInt32Formatter : IMessagePackFormatter<int?>
	{
		public static readonly NullableInt32Formatter Instance = new NullableInt32Formatter();

		private NullableInt32Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, int? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteInt32(ref bytes, offset, value.Value);
		}

		public int? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		}
	}
}
