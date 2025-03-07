namespace MessagePack.Formatters
{
	public class Int32Formatter : IMessagePackFormatter<int>
	{
		public static readonly Int32Formatter Instance = new Int32Formatter();

		private Int32Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, int value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteInt32(ref bytes, offset, value);
		}

		public int Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
		}
	}
}
