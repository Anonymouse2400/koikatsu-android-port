using System;

namespace MessagePack.Formatters
{
	public class VersionFormatter : IMessagePackFormatter<Version>
	{
		public static readonly IMessagePackFormatter<Version> Instance = new VersionFormatter();

		private VersionFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, Version value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			return MessagePackBinary.WriteString(ref bytes, offset, value.ToString());
		}

		public Version Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			if (MessagePackBinary.IsNil(bytes, offset))
			{
				readSize = 1;
				return null;
			}
			return new Version(MessagePackBinary.ReadString(bytes, offset, out readSize));
		}
	}
}
