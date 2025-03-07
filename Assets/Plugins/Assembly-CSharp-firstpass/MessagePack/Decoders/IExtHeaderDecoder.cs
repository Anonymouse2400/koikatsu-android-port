namespace MessagePack.Decoders
{
	internal interface IExtHeaderDecoder
	{
		ExtensionHeader Read(byte[] bytes, int offset, out int readSize);
	}
}
