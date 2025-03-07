using System;
using MessagePack.Formatters;
using UnityEngine;

namespace MessagePack.Unity
{
	public sealed class BoundsFormatter : IMessagePackFormatter<Bounds>
	{
		public int Serialize(ref byte[] bytes, int offset, Bounds value, IFormatterResolver formatterResolver)
		{
			int num = offset;
			offset += MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 2);
			offset += formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref bytes, offset, value.center, formatterResolver);
			offset += formatterResolver.GetFormatterWithVerify<Vector3>().Serialize(ref bytes, offset, value.size, formatterResolver);
			return offset - num;
		}

		public Bounds Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				throw new InvalidOperationException("typecode is null, struct not supported");
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			Vector3 center = default(Vector3);
			Vector3 size = default(Vector3);
			for (int i = 0; i < num2; i++)
			{
				switch (i)
				{
				case 0:
					center = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(bytes, offset, formatterResolver, out readSize);
					break;
				case 1:
					size = formatterResolver.GetFormatterWithVerify<Vector3>().Deserialize(bytes, offset, formatterResolver, out readSize);
					break;
				default:
					readSize = MessagePackBinary.ReadNextBlock(bytes, offset);
					break;
				}
				offset += readSize;
			}
			readSize = offset - num;
			return new Bounds(center, size);
		}
	}
}
