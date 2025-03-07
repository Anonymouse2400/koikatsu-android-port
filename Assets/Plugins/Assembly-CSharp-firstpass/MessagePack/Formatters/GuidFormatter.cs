using System;

namespace MessagePack.Formatters
{
	public class GuidFormatter : IMessagePackFormatter<Guid>
	{
		public static readonly IMessagePackFormatter<Guid> Instance = new GuidFormatter();

		private GuidFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, Guid value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteString(ref bytes, offset, value.ToString());
		}

		public Guid Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return new Guid(MessagePackBinary.ReadString(bytes, offset, out readSize));
		}
	}
}
