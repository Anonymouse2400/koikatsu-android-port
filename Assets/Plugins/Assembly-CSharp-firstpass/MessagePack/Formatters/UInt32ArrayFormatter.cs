namespace MessagePack.Formatters
{
	public class UInt32ArrayFormatter : IMessagePackFormatter<uint[]>
	{
		public static readonly UInt32ArrayFormatter Instance = new UInt32ArrayFormatter();

		private UInt32ArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, uint[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteUInt32(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public uint[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			uint[] array = new uint[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
