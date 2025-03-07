using System;

namespace MessagePack.Decoders
{
	internal class InvalidSingle : ISingleDecoder
	{
		internal static readonly ISingleDecoder Instance = new InvalidSingle();

		private InvalidSingle()
		{
		}

		public float Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
