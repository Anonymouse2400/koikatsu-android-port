using System;
using System.Collections.Generic;
using System.Linq;

namespace MessagePack.Formatters
{
	public class InterfaceGroupingFormatter<TKey, TElement> : IMessagePackFormatter<IGrouping<TKey, TElement>>
	{
		public int Serialize(ref byte[] bytes, int offset, IGrouping<TKey, TElement> value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 2);
			offset += formatterResolver.GetFormatterWithVerify<TKey>().Serialize(ref bytes, offset, value.Key, formatterResolver);
			offset += formatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Serialize(ref bytes, offset, value, formatterResolver);
			return offset - num;
		}

		public IGrouping<TKey, TElement> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			if (num2 != 2)
			{
				throw new InvalidOperationException("Invalid Grouping format.");
			}
			TKey key = formatterResolver.GetFormatterWithVerify<TKey>().Deserialize(bytes, offset, formatterResolver, out readSize);
			offset += readSize;
			IEnumerable<TElement> elements = formatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Deserialize(bytes, offset, formatterResolver, out readSize);
			offset += readSize;
			readSize = offset - num;
			return new Grouping<TKey, TElement>(key, elements);
		}
	}
}
