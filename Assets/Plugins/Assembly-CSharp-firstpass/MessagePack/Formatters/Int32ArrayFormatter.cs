namespace MessagePack.Formatters
{
	public class Int32ArrayFormatter : IMessagePackFormatter<int[]>
	{
		public static readonly Int32ArrayFormatter Instance = new Int32ArrayFormatter();

		private Int32ArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, int[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteInt32(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public int[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			int[] array = new int[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
