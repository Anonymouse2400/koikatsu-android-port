using System;

namespace MessagePack.Formatters
{
	public class ThreeDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,]>
	{
		private const int ArrayLength = 4;

		public int Serialize(ref byte[] bytes, int offset, T[,,] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int length = value.GetLength(0);
			int length2 = value.GetLength(1);
			int length3 = value.GetLength(2);
			int num = offset;
			IMessagePackFormatter<T> formatterWithVerify = formatterResolver.GetFormatterWithVerify<T>();
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 4);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length2);
			offset += MessagePackBinary.WriteInt32(ref bytes, offset, length3);
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			int length4 = value.GetLength(0);
			int length5 = value.GetLength(1);
			int length6 = value.GetLength(2);
			for (int i = 0; i < length4; i++)
			{
				for (int j = 0; j < length5; j++)
				{
					for (int k = 0; k < length6; k++)
					{
						T value2 = value[i, j, k];
						offset += formatterWithVerify.Serialize(ref bytes, offset, value2, formatterResolver);
					}
				}
			}
			return offset - num;
		}

		public T[,,] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
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
			if (num2 != 4)
			{
				throw new InvalidOperationException("Invalid T[,,] format");
			}
			int num3 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num4 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num5 = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
			offset += readSize;
			int num6 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			T[,,] array = new T[num3, num4, num5];
			int num7 = 0;
			int num8 = 0;
			int num9 = -1;
			for (int i = 0; i < num6; i++)
			{
				if (num9 < num5 - 1)
				{
					num9++;
				}
				else if (num8 < num4 - 1)
				{
					num9 = 0;
					num8++;
				}
				else
				{
					num9 = 0;
					num8 = 0;
					num7++;
				}
				array[num7, num8, num9] = formatterWithVerify.Deserialize(bytes, offset, formatterResolver, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
