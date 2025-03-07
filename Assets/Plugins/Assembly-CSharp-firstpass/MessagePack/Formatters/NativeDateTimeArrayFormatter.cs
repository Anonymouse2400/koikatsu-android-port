using System;

namespace MessagePack.Formatters
{
	public class NativeDateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
	{
		public static readonly NativeDateTimeArrayFormatter Instance = new NativeDateTimeArrayFormatter();

		private NativeDateTimeArrayFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, DateTime[] value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				offset += MessagePackBinary.WriteInt64(ref bytes, offset, value[i].ToBinary());
			}
			return offset - num;
		}

		public DateTime[] Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			DateTime[] array = new DateTime[num2];
			for (int i = 0; i < array.Length; i++)
			{
				long dateData = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
				array[i] = DateTime.FromBinary(dateData);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
