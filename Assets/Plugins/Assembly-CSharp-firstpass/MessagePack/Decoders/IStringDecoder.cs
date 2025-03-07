namespace MessagePack.Decoders
{
	internal interface IStringDecoder
	{
		string Read(byte[] bytes, int offset, out int readSize);
	}
}
