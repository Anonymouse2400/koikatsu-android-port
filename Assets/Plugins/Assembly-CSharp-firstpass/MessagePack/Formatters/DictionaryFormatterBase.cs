using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public abstract class DictionaryFormatterBase<TKey, TValue, TIntermediate, TDictionary> : IMessagePackFormatter<TDictionary> where TDictionary : IDictionary<TKey, TValue>
	{
		public int Serialize(ref byte[] bytes, int offset, TDictionary value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			IMessagePackFormatter<TKey> formatterWithVerify = formatterResolver.GetFormatterWithVerify<TKey>();
			IMessagePackFormatter<TValue> formatterWithVerify2 = formatterResolver.GetFormatterWithVerify<TValue>();
			int count = value.Count;
			offset += MessagePackBinary.WriteMapHeader(ref bytes, offset, count);
			IEnumerator<KeyValuePair<TKey, TValue>> enumerator = value.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					offset += formatterWithVerify.Serialize(ref bytes, offset, current.Key, formatterResolver);
					offset += formatterWithVerify2.Serialize(ref bytes, offset, current.Value, formatterResolver);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
			return offset - num;
		}

		public TDictionary Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return default(TDictionary);
			}
			int num = offset;
			IMessagePackFormatter<TKey> formatterWithVerify = formatterResolver.GetFormatterWithVerify<TKey>();
			IMessagePackFormatter<TValue> formatterWithVerify2 = formatterResolver.GetFormatterWithVerify<TValue>();
			int num2 = MessagePackBinary.ReadMapHeader(bytes, offset, out readSize);
			offset += readSize;
			TIntermediate val = Create(num2);
			for (int i = 0; i < num2; i++)
			{
				TKey key = formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize);
				offset += readSize;
				TValue value = formatterWithVerify2.Deserialize(bytes, offset, formatterResolver, out readSize);
				offset += readSize;
				Add(val, i, key, value);
			}
			readSize = offset - num;
			return Complete(val);
		}

		protected abstract TIntermediate Create(int count);

		protected abstract void Add(TIntermediate collection, int index, TKey key, TValue value);

		protected abstract TDictionary Complete(TIntermediate intermediateCollection);
	}
}
