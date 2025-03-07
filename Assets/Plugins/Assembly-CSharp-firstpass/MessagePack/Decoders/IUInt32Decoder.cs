namespace MessagePack.Decoders
{
	internal interface IUInt32Decoder
	{
		uint Read(byte[] bytes, int offset, out int readSize);
	}
}
