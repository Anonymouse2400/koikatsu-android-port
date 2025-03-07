using System.Collections;

namespace MessagePack.Formatters
{
	public class BitArrayFormatter : IMessagePackFormatter<BitArray>
	{
		public static readonly IMessagePackFormatter<BitArray> Instance = new BitArrayFormatter();

		private BitArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, BitArray value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			int length = value.Length;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, length);
			for (int i = 0; i < length; i++)
			{
				offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value.Get(i));
			}
			return offset - num;
		}

		public BitArray Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			BitArray bitArray = new BitArray(num2);
			for (int i = 0; i < num2; i++)
			{
				bitArray[i] = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return bitArray;
		}
	}
}
