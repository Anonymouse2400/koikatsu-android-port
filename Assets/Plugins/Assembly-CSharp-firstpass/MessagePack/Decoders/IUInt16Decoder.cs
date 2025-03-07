namespace MessagePack.Decoders
{
	internal interface IUInt16Decoder
	{
		ushort Read(byte[] bytes, int offset, out int readSize);
	}
}
