namespace MessagePack.Formatters
{
	public class ArrayFormatter<T> : IMessagePackFormatter<T[]>
	{
		public int Serialize(ref byte[] bytes, int offset, T[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += formatterWithVerify.Serialize(ref bytes, offset, value[i], formatterResolver);
			}
			return offset - num;
		}

		public T[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			T[] array = new T[num2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
