using System;

namespace MessagePack.Formatters
{
	public class NullableDateTimeFormatter : IMessagePackFormatter<DateTime?>
	{
		public static readonly NullableDateTimeFormatter Instance = new NullableDateTimeFormatter();

		private NullableDateTimeFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, DateTime? value, IFormatterResolver formatterResolver)
		{
			if (!value.HasValue)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteDateTime(ref bytes, offset, value.Value);
		}

		public DateTime? Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
		}
	}
}
