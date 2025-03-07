using System;

namespace MessagePack.Formatters
{
	public class ArraySegmentFormatter<T> : IMessagePackFormatter<ArraySegment<T>>
	{
		public int Serialize(ref byte[] bytes, int offset, ArraySegment<T> value, IFormatterResolver formatterResolver)
		{
			if (value.Array == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Count);
			T[] array = value.Array;
			for (int i = 0; i < value.Count; i++)
			{
				T value2 = array[value.Offset + i];
				offset += formatterWithVerify.Serialize(ref bytes, offset, value2, formatterResolver);
			}
			return offset - num;
		}

		public ArraySegment<T> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return default(ArraySegment<T>);
			}
			T[] array = formatterResolver.GetFormatterWithVerify<T[]>().Deserialize(bytes, offset, formatterResolver, out readSize);
			return new ArraySegment<T>(array, 0, array.Length);
		}
	}
}
