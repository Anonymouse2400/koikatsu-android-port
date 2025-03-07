namespace MessagePack.Formatters
{
	public class UInt16Formatter : IMessagePackFormatter<ushort>
	{
		public static readonly UInt16Formatter Instance = new UInt16Formatter();

		private UInt16Formatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, ushort value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteUInt16(ref bytes, offset, value);
		}

		public ushort Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
		}
	}
}
