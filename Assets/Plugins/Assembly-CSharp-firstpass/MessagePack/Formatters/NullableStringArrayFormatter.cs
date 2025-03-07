namespace MessagePack.Formatters
{
	public class NullableStringArrayFormatter : IMessagePackFormatter<string[]>
	{
		public static readonly NullableStringArrayFormatter Instance = new NullableStringArrayFormatter();

		private NullableStringArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, string[] value, IFormatterResolver typeResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteString(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public string[] Deserialize(byte[] bytes, int offset, IFormatterResolver typeResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			string[] array = new string[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadString(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
