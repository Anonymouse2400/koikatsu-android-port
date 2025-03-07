namespace MessagePack.Decoders
{
	internal interface IDoubleDecoder
	{
		double Read(byte[] bytes, int offset, out int readSize);
	}
}
