namespace MessagePack.Formatters
{
	public class SByteArrayFormatter : IMessagePackFormatter<sbyte[]>
	{
		public static readonly SByteArrayFormatter Instance = new SByteArrayFormatter();

		private SByteArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, sbyte[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteSByte(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public sbyte[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			sbyte[] array = new sbyte[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadSByte(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
