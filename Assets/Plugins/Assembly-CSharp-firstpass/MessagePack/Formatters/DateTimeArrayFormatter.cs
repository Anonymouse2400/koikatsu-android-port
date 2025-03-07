using System;

namespace MessagePack.Formatters
{
	public class DateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
	{
		public static readonly DateTimeArrayFormatter Instance = new DateTimeArrayFormatter();

		private DateTimeArrayFormatter()
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
				offset += MessagePackBinary.WriteDateTime(ref bytes, offset, value[i]);
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
				array[i] = MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
				offset += readSize;
			}
			readSize = offset - num;
			return array;
		}
	}
}
