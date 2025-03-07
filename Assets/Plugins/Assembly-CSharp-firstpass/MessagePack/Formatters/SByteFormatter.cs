namespace MessagePack.Formatters
{
	public class SByteFormatter : IMessagePackFormatter<sbyte>
	{
		public static readonly SByteFormatter Instance = new SByteFormatter();

		private SByteFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, sbyte value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteSByte(ref bytes, offset, value);
		}

		public sbyte Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
		}
	}
}
