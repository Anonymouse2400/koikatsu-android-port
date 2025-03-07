namespace MessagePack.Formatters
{
	public class DoubleFormatter : IMessagePackFormatter<double>
	{
		public static readonly DoubleFormatter Instance = new DoubleFormatter();

		private DoubleFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, double value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteDouble(ref bytes, offset, value);
		}

		public double Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
		}
	}
}
