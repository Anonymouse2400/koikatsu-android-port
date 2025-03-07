using System;
using MessagePack.Formatters;
using UnityEngine;

namespace MessagePack.Unity
{
	public sealed class ColorFormatter : IMessagePackFormatter<Color>
	{
		public int Serialize(ref byte[] bytes, int offset, Color value, IFormatterResolver formatterResolver)
		{
			int num = offset;
			offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 4);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.r);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.g);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.b);
			offset += MessagePackBinary.WriteSingle(ref bytes, offset, value.a);
			return offset - num;
		}

		public Color Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				throw new InvalidOperationException("typecode is null, struct not supported");
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			float r = 0f;
			float g = 0f;
			float b = 0f;
			float a = 0f;
			for (int i = 0; i < num2; i++)
			{
				switch (i)
				{
				case 0:
					r = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				case 1:
					g = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				case 2:
					b = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				case 3:
					a = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
					break;
				default:
					readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
					break;
				}
				offset += readSize;
			}
			readSize = offset - num;
			return new Color(r, g, b, a);
		}
	}
}
