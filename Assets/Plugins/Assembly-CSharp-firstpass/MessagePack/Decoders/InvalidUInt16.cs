using System;

namespace MessagePack.Decoders
{
	internal class InvalidUInt16 : IUInt16Decoder
	{
		internal static readonly IUInt16Decoder Instance = new InvalidUInt16();

		private InvalidUInt16()
		{
		}

		public ushort Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
