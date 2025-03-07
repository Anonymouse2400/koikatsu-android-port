using System.Globalization;

namespace MessagePack.Formatters
{
	public class DecimalFormatter : IMessagePackFormatter<decimal>
	{
		public static readonly DecimalFormatter Instance = new DecimalFormatter();

		private DecimalFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, decimal value, IFormatterResolver formatterResolver)
		{
			return MessagePackBinary.WriteString(ref bytes, offset, value.ToString(CultureInfo.InvariantCulture));
		}

		public decimal Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return decimal.Parse(MessagePackBinary.ReadString(bytes, offset, out readSize), CultureInfo.InvariantCulture);
		}
	}
}
