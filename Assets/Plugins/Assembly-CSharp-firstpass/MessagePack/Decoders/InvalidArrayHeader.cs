using System;

namespace MessagePack.Decoders
{
	internal class InvalidArrayHeader : IArrayHeaderDecoder
	{
		internal static readonly IArrayHeaderDecoder Instance = new InvalidArrayHeader();

		private InvalidArrayHeader()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
