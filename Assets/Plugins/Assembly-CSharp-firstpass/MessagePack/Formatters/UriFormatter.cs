using System;

namespace MessagePack.Formatters
{
	public class UriFormatter : IMessagePackFormatter<Uri>
	{
		public static readonly IMessagePackFormatter<Uri> Instance = new UriFormatter();

		private UriFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, Uri value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteString(ref bytes, offset, value.ToString());
		}

		public Uri Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return new Uri(MessagePackBinary.ReadString(bytes, offset, out readSize));
		}
	}
}
