using System;

namespace MessagePack.Decoders
{
	internal class InvalidInt32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new InvalidInt32();

		private InvalidInt32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
