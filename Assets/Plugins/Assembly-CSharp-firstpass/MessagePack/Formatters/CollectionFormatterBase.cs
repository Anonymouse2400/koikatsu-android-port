using System;
using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public abstract class CollectionFormatterBase<TElement, TIntermediate, TEnumerator, TCollection> : IMessagePackFormatter<TCollection> where TEnumerator : IEnumerator<TElement> where TCollection : IEnumerable<TElement>
	{
		public int Serialize(ref byte[] bytes, int offset, TCollection value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			TElement[] array = value as TElement[];
			if (array != null)
			{
				int num = offset;
				IMessagePackFormatter<TElement> formatterWithVerify = formatterResolver.GetFormatterWithVerify<TElement>();
				offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, array.Length);
				TElement[] array2 = array;
				foreach (TElement value2 in array2)
				{
					offset += formatterWithVerify.Serialize(ref bytes, offset, value2, formatterResolver);
				}
				return offset - num;
			}
			int num2 = offset;
			IMessagePackFormatter<TElement> formatterWithVerify2 = formatterResolver.GetFormatterWithVerify<TElement>();
			int? count = GetCount(value);
			if (count.HasValue)
			{
				offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count.Value);
				TEnumerator sourceEnumerator = GetSourceEnumerator(value);
				try
				{
					while (sourceEnumerator.MoveNext())
					{
						offset += formatterWithVerify2.Serialize(ref bytes, offset, sourceEnumerator.Current, formatterResolver);
					}
				}
				finally
				{
					sourceEnumerator.Dispose();
				}
				return offset - num2;
			}
			int num3 = offset;
			int num4 = 0;
			int num5 = 0;
			offset += 3;
			TEnumerator sourceEnumerator2 = GetSourceEnumerator(value);
			try
			{
				while (sourceEnumerator2.MoveNext())
				{
					num4++;
					int num6 = formatterWithVerify2.Serialize(ref bytes, offset, sourceEnumerator2.Current, formatterResolver);
					num5 += num6;
					offset += num6;
				}
			}
			finally
			{
				sourceEnumerator2.Dispose();
			}
			int arrayHeaderLength = MessagePackBinary.GetArrayHeaderLength(num4);
			if (arrayHeaderLength != 3)
			{
				offset = ((arrayHeaderLength != 1) ? (offset + 2) : (offset - 2));
				MessagePackBinary.EnsureCapacity(ref bytes, offset, arrayHeaderLength);
				Buffer.BlockCopy(bytes, num3 + 3, bytes, num3 + arrayHeaderLength, num5);
			}
			MessagePackBinary.WriteArrayHeader(ref bytes, num3, num4);
			return offset - num2;
		}

		public TCollection Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return default(TCollection);
			}
			int num = offset;
			IMessagePackFormatter<TElement> formatterWithVerify = formatterResolver.GetFormatterWithVerify<TElement>();
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			TIntermediate val = Create(num2);
			for (int i = 0; i < num2; i++)
			{
				Add(val, i, formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize));
				offset += readSize;
			}
			readSize = offset - num;
			return Complete(val);
		}

		protected virtual int? GetCount(TCollection sequence)
		{
			ICollection<TElement> collection = sequence as ICollection<TElement>;
			if (collection != null)
			{
				return collection.Count;
			}
			return null;
		}

		protected abstract TEnumerator GetSourceEnumerator(TCollection source);

		protected abstract TIntermediate Create(int count);

		protected abstract void Add(TIntermediate collection, int index, TElement value);

		protected abstract TCollection Complete(TIntermediate intermediateCollection);
	}
	public abstract class CollectionFormatterBase<TElement, TIntermediate, TCollection> : CollectionFormatterBase<TElement, TIntermediate, IEnumerator<TElement>, TCollection> where TCollection : IEnumerable<TElement>
	{
		protected override IEnumerator<TElement> GetSourceEnumerator(TCollection source)
		{
			return source.GetEnumerator();
		}
	}
	public abstract class CollectionFormatterBase<TElement, TCollection> : CollectionFormatterBase<TElement, TCollection, TCollection> where TCollection : IEnumerable<TElement>
	{
		protected sealed override TCollection Complete(TCollection intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
