using System;

namespace MessagePack.Decoders
{
	internal class InvalidMapHeader : IMapHeaderDecoder
	{
		internal static readonly IMapHeaderDecoder Instance = new InvalidMapHeader();

		private InvalidMapHeader()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
