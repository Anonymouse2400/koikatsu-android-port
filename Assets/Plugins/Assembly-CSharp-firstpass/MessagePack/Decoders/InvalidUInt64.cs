using System;

namespace MessagePack.Decoders
{
	internal class InvalidUInt64 : IUInt64Decoder
	{
		internal static readonly IUInt64Decoder Instance = new InvalidUInt64();

		private InvalidUInt64()
		{
		}

		public ulong Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
