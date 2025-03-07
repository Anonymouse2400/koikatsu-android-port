using System;

namespace MessagePack.Decoders
{
	internal class InvalidInt64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new InvalidInt64();

		private InvalidInt64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
