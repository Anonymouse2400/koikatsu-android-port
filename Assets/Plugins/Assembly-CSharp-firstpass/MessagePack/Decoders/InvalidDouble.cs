using System;

namespace MessagePack.Decoders
{
	internal class InvalidDouble : IDoubleDecoder
	{
		internal static readonly IDoubleDecoder Instance = new InvalidDouble();

		private InvalidDouble()
		{
		}

		public double Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
