namespace MessagePack.Formatters
{
	public class NullableDoubleFormatter : IMessagePackFormatter<double?>
	{
		public static readonly NullableDoubleFormatter Instance = new NullableDoubleFormatter();

		private NullableDoubleFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, double? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteDouble(ref bytes, offset, value.Value);
		}

		public double? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
		}
	}
}
