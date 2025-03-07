using System;
using MessagePack.Formatters;
using UnityEngine;

namespace MessagePack.Unity
{
	public sealed class QuaternionFormatter : IMessagePackFormatter<Quaternion>
	{
		public int Serialize(ref byte[] bytes, int offset, Quaternion value, IFormatterResolver formatterResolver)
		{
			int num = offset;
			offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 4);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.x);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.y);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.z);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.w);
			return offset - num;
		}

		public Quaternion Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				throw new InvalidOperationException("typecode is null, struct not supported");
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float w = 0f;
			for (int i = 0; i < num2; i++)
			{
				switch (i)
				{
				case 0:
					x = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				case 1:
					y = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				case 2:
					z = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				case 3:
					w = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				default:
					readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
					break;
				}
				offset += readSize;
			}
			readSize = offset - num;
			return new Quaternion(x, y, z, w);
		}
	}
}
