namespace MessagePack.Formatters
{
	public class Int16ArrayFormatter : IMessagePackFormatter<short[]>
	{
		public static readonly Int16ArrayFormatter Instance = new Int16ArrayFormatter();

		private Int16ArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, short[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteInt16(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public short[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			short[] array = new short[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadInt16(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
