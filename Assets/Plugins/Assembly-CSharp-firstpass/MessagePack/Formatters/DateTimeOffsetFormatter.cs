using System;

namespace MessagePack.Formatters
{
	public class DateTimeOffsetFormatter : IMessagePackFormatter<DateTimeOffset>
	{
		public static readonly IMessagePackFormatter<DateTimeOffset> Instance = new DateTimeOffsetFormatter();

		private DateTimeOffsetFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, DateTimeOffset value, IFormatterResolver formatterResolver)
		{
			int num = offset;
			offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 2);
			offset += MessagePackBinary.WriteDateTime(ref bytes, offset, new DateTime(value.Ticks, DateTimeKind.Utc));
			offset += MessagePackBinary.WriteInt16(ref bytes, offset, (short)value.Offset.TotalMinutes);
			return offset - num;
		}

		public DateTimeOffset Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			int num = offset;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			if (num2 != 2)
			{
				throw new InvalidOperationException("Invalid DateTimeOffset format.");
			}
			DateTime dateTime = MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
			offset += readSize;
			short num3 = MessagePackBinary.ReadInt16(bytes, offset, out readSize);
			offset += readSize;
			readSize = offset - num;
			return new DateTimeOffset(dateTime.Ticks, TimeSpan.FromMinutes(num3));
		}
	}
}
