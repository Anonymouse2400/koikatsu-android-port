namespace MessagePack.Decoders
{
	internal interface IInt32Decoder
	{
		int Read(byte[] bytes, int offset, out int readSize);
	}
}
