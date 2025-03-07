using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class ListFormatter<T> : IMessagePackFormatter<List<T>>
	{
		public int Serialize(ref byte[] bytes, int offset, List<T> value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			int count = value.Count;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
			for (int i = 0; i < count; i++)
			{
				offset += formatterWithVerify.Serialize(ref bytes, offset, value[i], formatterResolver);
			}
			return offset - num;
		}

		public List<T> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
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
			List<T> list = new List<T>(num2);
			for (int i = 0; i < num2; i++)
			{
				list.Add(formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize));
				offset += readSize;
			}
			readSize = offset - num;
			return list;
		}
	}
}
