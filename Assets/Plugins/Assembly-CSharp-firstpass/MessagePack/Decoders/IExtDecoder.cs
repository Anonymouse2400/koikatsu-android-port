namespace MessagePack.Decoders
{
	internal interface IExtDecoder
	{
		ExtensionResult Read(byte[] bytes, int offset, out int readSize);
	}
}
