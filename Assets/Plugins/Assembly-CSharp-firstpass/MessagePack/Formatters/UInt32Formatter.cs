namespace MessagePack.Formatters
{
	public class UInt32Formatter : IMessagePackFormatter<uint>
	{
		public static readonly UInt32Formatter Instance = new UInt32Formatter();

		private UInt32Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, uint value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteUInt32(ref bytes, offset, value);
		}

		public uint Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
		}
	}
}
