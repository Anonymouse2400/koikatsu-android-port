using System;

namespace MessagePack.Formatters
{
	public class DateTimeFormatter : IMessagePackFormatter<DateTime>
	{
		public static readonly DateTimeFormatter Instance = new DateTimeFormatter();

		private DateTimeFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, DateTime value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteDateTime(ref bytes, offset, value);
		}

		public DateTime Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
		}
	}
}
