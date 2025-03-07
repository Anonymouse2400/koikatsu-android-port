using System;
using MessagePack.Internal;

namespace MessagePack.Decoders
{
	internal class FixExt4DateTime : IDateTimeDecoder
	{
		internal static readonly IDateTimeDecoder Instance = new FixExt4DateTime();

		private FixExt4DateTime()
		{
		}

		public DateTime Read(byte[] bytes, int offset, out int readSize)
		{
			sbyte b = (sbyte)bytes[offset + 1];
			if (b != -1)
			{
				throw new InvalidOperationException(string.Format("typeCode is invalid. typeCode:{0}", b));
			}
			uint num = (uint)((bytes[offset + 2] << 24) | (bytes[offset + 3] << 16) | (bytes[offset + 4] << 8) | bytes[offset + 5]);
			readSize = 6;
			return DateTimeConstants.UnixEpoch.AddSeconds(num);
		}
	}
}
