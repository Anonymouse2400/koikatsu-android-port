namespace MessagePack.Formatters
{
	public class UInt64ArrayFormatter : IMessagePackFormatter<ulong[]>
	{
		public static readonly UInt64ArrayFormatter Instance = new UInt64ArrayFormatter();

		private UInt64ArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, ulong[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteUInt64(ref bytes, offset, value[i]);
			}
			return offset - num;
		}

		public ulong[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			ulong[] array = new ulong[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
