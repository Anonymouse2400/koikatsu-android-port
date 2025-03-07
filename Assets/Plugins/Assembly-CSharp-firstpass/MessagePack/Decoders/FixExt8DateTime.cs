using System;
using MessagePack.Internal;

namespace MessagePack.Decoders
{
	internal class FixExt8DateTime : IDateTimeDecoder
	{
		internal static readonly IDateTimeDecoder Instance = new FixExt8DateTime();

		private FixExt8DateTime()
		{
		}

		public DateTime Read(byte[] bytes, int offset, out int readSize)
		{
			sbyte b = (sbyte)bytes[offset + 1];
			if (b != -1)
			{
				throw new InvalidOperationException(string.Format("typeCode is invalid. typeCode:{0}", b));
			}
			ulong num = ((ulong)bytes[offset + 2] << 56) | ((ulong)bytes[offset + 3] << 48) | ((ulong)bytes[offset + 4] << 40) | ((ulong)bytes[offset + 5] << 32) | ((ulong)bytes[offset + 6] << 24) | ((ulong)bytes[offset + 7] << 16) | ((ulong)bytes[offset + 8] << 8) | bytes[offset + 9];
			long num2 = (long)(num >> 34);
			ulong num3 = num & 0x3FFFFFFFFL;
			readSize = 10;
			return DateTimeConstants.UnixEpoch.AddSeconds(num3).AddTicks(num2 / 100);
		}
	}
}
