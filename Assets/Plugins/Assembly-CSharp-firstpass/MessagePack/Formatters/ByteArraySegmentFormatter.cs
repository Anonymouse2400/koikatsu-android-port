using System;

namespace MessagePack.Formatters
{
	public class ByteArraySegmentFormatter : IMessagePackFormatter<ArraySegment<byte>>
	{
		public static readonly ByteArraySegmentFormatter Instance = new ByteArraySegmentFormatter();

		private ByteArraySegmentFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, ArraySegment<byte> value, IFormatterResolver formatterResolver)
		{
			if (value.Array == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteBytes(ref bytes, offset, value.Array, value.Offset, value.Count);
		}

		public ArraySegment<byte> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return default(ArraySegment<byte>);
			}
			byte[] array = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
			return new ArraySegment<byte>(array, 0, array.Length);
		}
	}
}
