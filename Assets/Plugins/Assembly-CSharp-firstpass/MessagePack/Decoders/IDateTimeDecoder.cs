using System;

namespace MessagePack.Decoders
{
	internal interface IDateTimeDecoder
	{
		DateTime Read(byte[] bytes, int offset, out int readSize);
	}
}
