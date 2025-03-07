using System;

namespace MessagePack.Formatters
{
	public class TimeSpanFormatter : IMessagePackFormatter<TimeSpan>
	{
		public static readonly IMessagePackFormatter<TimeSpan> Instance = new TimeSpanFormatter();

		private TimeSpanFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, TimeSpan value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteInt64(ref bytes, offset, value.Ticks);
		}

		public TimeSpan Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return new TimeSpan(MessagePackBinary.ReadInt64(bytes, offset, out readSize));
		}
	}
}
