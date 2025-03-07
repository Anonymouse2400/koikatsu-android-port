namespace MessagePack.Decoders
{
	internal interface ISingleDecoder
	{
		float Read(byte[] bytes, int offset, out int readSize);
	}
}
