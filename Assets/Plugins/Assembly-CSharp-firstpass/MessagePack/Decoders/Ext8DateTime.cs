using System;
using MessagePack.Internal;

namespace MessagePack.Decoders
{
	internal class Ext8DateTime : IDateTimeDecoder
	{
		internal static readonly IDateTimeDecoder Instance = new Ext8DateTime();

		private Ext8DateTime()
		{
		}

		public DateTime Read(byte[] bytes, int offset, out int readSize)
		{
			byte b = bytes[checked(offset + 1)];
			sbyte b2 = (sbyte)bytes[offset + 2];
			if (b != 12 || b2 != -1)
			{
				throw new InvalidOperationException(string.Format("typeCode is invalid. typeCode:{0}", b2));
			}
			uint num = (uint)((bytes[offset + 3] << 24) | (bytes[offset + 4] << 16) | (bytes[offset + 5] << 8) | bytes[offset + 6]);
			long num2 = ((long)(int)bytes[offset + 7] << 56) | ((long)(int)bytes[offset + 8] << 48) | ((long)(int)bytes[offset + 9] << 40) | ((long)(int)bytes[offset + 10] << 32) | ((long)(int)bytes[offset + 11] << 24) | ((long)(int)bytes[offset + 12] << 16) | ((long)(int)bytes[offset + 13] << 8) | (int)bytes[offset + 14];
			readSize = 15;
			return DateTimeConstants.UnixEpoch.AddSeconds(num2).AddTicks(num / 100);
		}
	}
}
