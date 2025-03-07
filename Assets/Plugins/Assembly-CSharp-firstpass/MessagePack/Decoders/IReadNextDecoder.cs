namespace MessagePack.Decoders
{
	internal interface IReadNextDecoder
	{
		int Read(byte[] bytes, int offset);
	}
}
