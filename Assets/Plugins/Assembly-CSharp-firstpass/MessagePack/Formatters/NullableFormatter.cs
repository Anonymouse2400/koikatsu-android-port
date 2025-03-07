namespace MessagePack.Formatters
{
	public class NullableFormatter<T> : IMessagePackFormatter<T?> where T : struct
	{
		public int Serialize(ref byte[] bytes, int offset, T? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return formatterResolver.GetFormatterWithVerify<T>().Serialize(ref bytes, offset, value.Value, formatterResolver);
		}

		public T? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return formatterResolver.GetFormatterWithVerify<T>().Deserialize(bytes, offset, formatterResolver, out readSize);
		}
	}
}
