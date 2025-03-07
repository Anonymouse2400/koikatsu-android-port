namespace MessagePack.Decoders
{
	internal interface IMapHeaderDecoder
	{
		uint Read(byte[] bytes, int offset, out int readSize);
	}
}
