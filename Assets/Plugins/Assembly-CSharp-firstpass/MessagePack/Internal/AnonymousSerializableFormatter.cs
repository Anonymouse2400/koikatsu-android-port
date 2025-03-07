using System;
using MessagePack.Formatters;

namespace MessagePack.Internal
{
	internal class AnonymousSerializableFormatter<T> : IMessagePackFormatter<T>
	{
		private readonly SerializeDelegate<T> serialize;

		public AnonymousSerializableFormatter(SerializeDelegate<T> serialize)
		{
			this.serialize = serialize;
		}

		public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
		{
			return serialize(ref bytes, offset, value, formatterResolver);
		}

		public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			throw new NotSupportedException("Anonymous Formatter does not support Deserialize.");
		}
	}
}
