using System;
using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class KeyValuePairFormatter<TKey, TValue> : IMessagePackFormatter<KeyValuePair<TKey, TValue>>
	{
		public int Serialize(ref byte[] bytes, int offset, KeyValuePair<TKey, TValue> value, IFormatterResolver formatterResolver)
		{
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 2);
			offset += formatterResolver.GetFormatterWithVerify<TKey>().Serialize(ref bytes, offset, value.Key, formatterResolver);
			offset += formatterResolver.GetFormatterWithVerify<TValue>().Serialize(ref bytes, offset, value.Value, formatterResolver);
			return offset - num;
		}

		public KeyValuePair<TKey, TValue> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			if (num2 != 2)
			{
				throw new InvalidOperationException("Invalid KeyValuePair format.");
			}
			TKey key = formatterResolver.GetFormatterWithVerify<TKey>().Deserialize(bytes, offset, formatterResolver, out readSize);
			offset += readSize;
			TValue value = formatterResolver.GetFormatterWithVerify<TValue>().Deserialize(bytes, offset, formatterResolver, out readSize);
			offset += readSize;
			readSize = offset - num;
			return new KeyValuePair<TKey, TValue>(key, value);
		}
	}
}
