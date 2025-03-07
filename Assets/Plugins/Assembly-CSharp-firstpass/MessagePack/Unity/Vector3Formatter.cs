using System;
using MessagePack.Formatters;
using UnityEngine;

namespace MessagePack.Unity
{
	public sealed class Vector3Formatter : IMessagePackFormatter<Vector3>
	{
		public int Serialize(ref byte[] bytes, int offset, Vector3 value, IFormatterResolver formatterResolver)
		{
			int num = offset;
			offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 3);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.x);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.y);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.z);
			return offset - num;
		}

		public Vector3 Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
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
				default:
					readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
					break;
				}
				offset += readSize;
			}
			readSize = offset - num;
			return new Vector3(x, y, z);
		}
	}
}
