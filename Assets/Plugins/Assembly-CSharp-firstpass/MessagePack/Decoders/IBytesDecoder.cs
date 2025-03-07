namespace MessagePack.Decoders
{
	internal interface IBytesDecoder
	{
		byte[] Read(byte[] bytes, int offset, out int readSize);
	}
}
