namespace MessagePack.Formatters
{
	public class StaticNullableFormatter<T> : IMessagePackFormatter<T?> where T : struct
	{
		private readonly IMessagePackFormatter<T> underlyingFormatter;

		public StaticNullableFormatter(IMessagePackFormatter<T> underlyingFormatter)
		{
			this.underlyingFormatter = underlyingFormatter;
		}

		public int Serialize(ref byte[] bytes, int offset, T? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return underlyingFormatter.Serialize(ref bytes, offset, value.Value, formatterResolver);
		}

		public T? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return underlyingFormatter.Deserialize(bytes, offset, formatterResolver, out readSize);
		}
	}
}
