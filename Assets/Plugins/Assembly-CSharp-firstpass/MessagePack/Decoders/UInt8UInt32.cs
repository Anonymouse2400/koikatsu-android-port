namespace MessagePack.Decoders
{
	internal class UInt8UInt32 : IUInt32Decoder
	{
		internal static readonly IUInt32Decoder Instance = new UInt8UInt32();

		private UInt8UInt32()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return bytes[offset + 1];
		}
	}
}
