namespace MessagePack.Formatters
{
	public class NullableSingleFormatter : IMessagePackFormatter<float?>
	{
		public static readonly NullableSingleFormatter Instance = new NullableSingleFormatter();

		private NullableSingleFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, float? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteSingle(ref bytes, offset, value.Value);
		}

		public float? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadSingle(bytes, offset, out readSize);
		}
	}
}
