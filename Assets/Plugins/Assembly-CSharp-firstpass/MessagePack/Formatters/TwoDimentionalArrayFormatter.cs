using System;

namespace MessagePack.Formatters
{
	public class TwoDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,]>
	{
		private const int ArrayLength = 3;

		public int Serialize(ref byte[] bytes, int offset, T[,] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int length = value.GetLength(0);
			int length2 = value.GetLength(1);
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 3);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length2);
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			int length3 = value.GetLength(0);
			int length4 = value.GetLength(1);
			for (int i = 0; i < length3; i++)
			{
				for (int j = 0; j < length4; j++)
				{
					T value2 = value[i, j];
					offset += formatterWithVerify.Serialize(ref bytes, offset, value2, formatterResolver);
				}
			}
			return offset - num;
		}

		public T[,] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
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
			if (num2 != 3)
			{
				throw new InvalidOperationException("Invalid T[,] format");
			}
			int num3 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num4 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num5 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			T[,] array = new T[num3, num4];
			int num6 = 0;
			int num7 = -1;
			for (int i = 0; i < num5; i++)
			{
				if (num7 < num4 - 1)
				{
					num7++;
				}
				else
				{
					num7 = 0;
					num6++;
				}
				array[num6, num7] = formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
