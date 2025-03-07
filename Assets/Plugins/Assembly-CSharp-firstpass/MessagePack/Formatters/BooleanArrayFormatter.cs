namespace MessagePack.Formatters
{
	public class BooleanArrayFormatter : IMessagePackFormatter<bool[]>
	{
		public static readonly BooleanArrayFormatter Instance = new BooleanArrayFormatter();

		private BooleanArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, bool[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public bool[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			bool[] array = new bool[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
