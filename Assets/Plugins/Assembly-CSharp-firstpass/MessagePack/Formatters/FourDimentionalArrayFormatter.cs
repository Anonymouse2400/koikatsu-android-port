using System;

namespace MessagePack.Formatters
{
	public class FourDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,,]>
	{
		private const int ArrayLength = 5;

		public int Serialize(ref byte[] bytes, int offset, T[,,,] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int length = value.GetLength(0);
			int length2 = value.GetLength(1);
			int length3 = value.GetLength(2);
			int length4 = value.GetLength(3);
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 5);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length2);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length3);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length4);
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			int length5 = value.GetLength(0);
			int length6 = value.GetLength(1);
			int length7 = value.GetLength(2);
			int length8 = value.GetLength(3);
			for (int i = 0; i < length5; i++)
			{
				for (int j = 0; j < length6; j++)
				{
					for (int k = 0; k < length7; k++)
					{
						for (int l = 0; l < length8; l++)
						{
							T value2 = value[i, j, k, l];
							offset += formatterWithVerify.Serialize(ref bytes, offset, value2, formatterResolver);
						}
					}
				}
			}
			return offset - num;
		}

		public T[,,,] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
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
			if (num2 != 5)
			{
				throw new InvalidOperationException("Invalid T[,,,] format");
			}
			int num3 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num4 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num5 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num6 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num7 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			T[,,,] array = new T[num3, num4, num5, num6];
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = -1;
			for (int i = 0; i < num7; i++)
			{
				if (num11 < num6 - 1)
				{
					num11++;
				}
				else if (num10 < num5 - 1)
				{
					num11 = 0;
					num10++;
				}
				else if (num9 < num4 - 1)
				{
					num11 = 0;
					num10 = 0;
					num9++;
				}
				else
				{
					num11 = 0;
					num10 = 0;
					num9 = 0;
					num8++;
				}
				array[num8, num9, num10, num11] = formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
