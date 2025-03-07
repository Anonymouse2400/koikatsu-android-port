namespace MessagePack.Formatters
{
	public class NullableNilFormatter : IMessagePackFormatter<Nil?>
	{
		public static readonly IMessagePackFormatter<Nil?> Instance = new NullableNilFormatter();

		private NullableNilFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, Nil? value, IFormatterResolver typeResolver)
		{
			return MessagePackBinary.WriteNil(ref bytes, offset);
		}

		public Nil? Deserialize(byte[] bytes, int offset, IFormatterResolver typeResolver, out int readSize)
		{
			return MessagePackBinary.ReadNil(bytes, offset, out readSize);
		}
	}
}
