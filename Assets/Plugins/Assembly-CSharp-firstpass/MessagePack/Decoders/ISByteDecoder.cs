namespace MessagePack.Decoders
{
	internal interface ISByteDecoder
	{
		sbyte Read(byte[] bytes, int offset, out int readSize);
	}
}
