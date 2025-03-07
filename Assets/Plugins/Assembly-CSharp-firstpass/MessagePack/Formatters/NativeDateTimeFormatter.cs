using System;

namespace MessagePack.Formatters
{
	public class NativeDateTimeFormatter : IMessagePackFormatter<DateTime>
	{
		public static readonly NativeDateTimeFormatter Instance = new NativeDateTimeFormatter();

		private NativeDateTimeFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, DateTime value, IFormatterResolver formatterResolver)
		{
			long value2 = value.ToBinary();
			return MessagePackBinary.WriteInt64(ref bytes, offset, value2);
		}

		public DateTime Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.GetMessagePackType(bytes, offset) == MessagePackType.Extension)
			{
				return DateTimeFormatter.Instance.Deserialize(bytes, offset, formatterResolver, out readSize);
			}
			long dateData = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
			return DateTime.FromBinary(dateData);
		}
	}
}
