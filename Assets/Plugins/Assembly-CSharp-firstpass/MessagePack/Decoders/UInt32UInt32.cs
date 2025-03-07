namespace MessagePack.Decoders
{
	internal class UInt32UInt32 : IUInt32Decoder
	{
		internal static readonly IUInt32Decoder Instance = new UInt32UInt32();

		private UInt32UInt32()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return (uint)((bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4]);
		}
	}
}
