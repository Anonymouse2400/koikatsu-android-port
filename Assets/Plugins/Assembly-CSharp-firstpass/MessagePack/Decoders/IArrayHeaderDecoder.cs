namespace MessagePack.Decoders
{
	internal interface IArrayHeaderDecoder
	{
		uint Read(byte[] bytes, int offset, out int readSize);
	}
}
