namespace MessagePack.Formatters
{
	public class Int64Formatter : IMessagePackFormatter<long>
	{
		public static readonly Int64Formatter Instance = new Int64Formatter();

		private Int64Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, long value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteInt64(ref bytes, offset, value);
		}

		public long Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
		}
	}
}
