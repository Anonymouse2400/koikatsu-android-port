namespace MessagePack.Formatters
{
	public class DoubleArrayFormatter : IMessagePackFormatter<double[]>
	{
		public static readonly DoubleArrayFormatter Instance = new DoubleArrayFormatter();

		private DoubleArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, double[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteDouble(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public double[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			double[] array = new double[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadDouble(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
