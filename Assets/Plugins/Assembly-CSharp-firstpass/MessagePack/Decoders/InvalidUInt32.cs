using System;

namespace MessagePack.Decoders
{
	internal class InvalidUInt32 : IUInt32Decoder
	{
		internal static readonly IUInt32Decoder Instance = new InvalidUInt32();

		private InvalidUInt32()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
