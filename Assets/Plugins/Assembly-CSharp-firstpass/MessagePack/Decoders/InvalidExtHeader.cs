using System;

namespace MessagePack.Decoders
{
	internal class InvalidExtHeader : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new InvalidExtHeader();

		private InvalidExtHeader()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
