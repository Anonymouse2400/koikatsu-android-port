using System;

namespace MessagePack.Formatters
{
	public class OldSpecStringFormatter : IMessagePackFormatter<string>
	{
		public static readonly OldSpecStringFormatter Instance = new OldSpecStringFormatter();

		private OldSpecStringFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, string value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			MessagePackBinary.EnsureCapacity(ref bytes, offset, StringEncoding.UTF8.GetMaxByteCount(value.Length) + 5);
			int num = ((value.Length <= 31) ? 1 : ((value.Length > 65535) ? 5 : 3));
			int num2 = offset + num;
			int bytes2 = StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, num2);
			if (bytes2 <= 31)
			{
				if (num != 1)
				{
					Buffer.BlockCopy(bytes, num2, bytes, offset + 1, bytes2);
				}
				bytes[offset] = (byte)(0xA0 | bytes2);
				return bytes2 + 1;
			}
			if (bytes2 <= 65535)
			{
				if (num != 3)
				{
					Buffer.BlockCopy(bytes, num2, bytes, offset + 3, bytes2);
				}
				bytes[offset] = 218;
				bytes[offset + 1] = (byte)(bytes2 >> 8);
				bytes[offset + 2] = (byte)bytes2;
				return bytes2 + 3;
			}
			if (num != 5)
			{
				Buffer.BlockCopy(bytes, num2, bytes, offset + 5, bytes2);
			}
			bytes[offset] = 219;
			bytes[offset + 1] = (byte)(bytes2 >> 24);
			bytes[offset + 2] = (byte)(bytes2 >> 16);
			bytes[offset + 3] = (byte)(bytes2 >> 8);
			bytes[offset + 4] = (byte)bytes2;
			return bytes2 + 5;
		}

		public string Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			return MessagePackBinary.ReadString(bytes, offset, out readSize);
		}
	}
}
