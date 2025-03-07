using System;

namespace MessagePack.Decoders
{
	internal class InvalidString : IStringDecoder
	{
		internal static readonly IStringDecoder Instance = new InvalidString();

		private InvalidString()
		{
		}

		public string Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
