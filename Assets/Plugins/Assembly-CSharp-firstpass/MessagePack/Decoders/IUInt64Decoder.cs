namespace MessagePack.Decoders
{
	internal interface IUInt64Decoder
	{
		ulong Read(byte[] bytes, int offset, out int readSize);
	}
}
