namespace MessagePack.Formatters
{
	public class UInt64Formatter : IMessagePackFormatter<ulong>
	{
		public static readonly UInt64Formatter Instance = new UInt64Formatter();

		private UInt64Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, ulong value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteUInt64(ref bytes, offset, value);
		}

		public ulong Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
		}
	}
}
