namespace MessagePack.Decoders
{
	internal interface IInt64Decoder
	{
		long Read(byte[] bytes, int offset, out int readSize);
	}
}
