namespace MessagePack.Decoders
{
	internal interface IInt16Decoder
	{
		short Read(byte[] bytes, int offset, out int readSize);
	}
}
