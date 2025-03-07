namespace MessagePack.Formatters
{
	public class Int16Formatter : IMessagePackFormatter<short>
	{
		public static readonly Int16Formatter Instance = new Int16Formatter();

		private Int16Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, short value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteInt16(ref bytes, offset, value);
		}

		public short Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadInt16(bytes, offset, out readSize);
		}
	}
}
