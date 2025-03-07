namespace MessagePack.Decoders
{
	internal interface IByteDecoder
	{
		byte Read(byte[] bytes, int offset, out int readSize);
	}
}
