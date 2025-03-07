namespace MessagePack.Formatters
{
	public class SingleArrayFormatter : IMessagePackFormatter<float[]>
	{
		public static readonly SingleArrayFormatter Instance = new SingleArrayFormatter();

		private SingleArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, float[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteSingle(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public float[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			float[] array = new float[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
