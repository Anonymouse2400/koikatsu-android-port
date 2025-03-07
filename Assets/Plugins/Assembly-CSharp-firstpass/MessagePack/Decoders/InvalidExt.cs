using System;

namespace MessagePack.Decoders
{
	internal class InvalidExt : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new InvalidExt();

		private InvalidExt()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
