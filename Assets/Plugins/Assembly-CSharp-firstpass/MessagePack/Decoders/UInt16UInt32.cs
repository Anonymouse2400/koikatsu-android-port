namespace MessagePack.Decoders
{
	internal class UInt16UInt32 : IUInt32Decoder
	{
		internal static readonly IUInt32Decoder Instance = new UInt16UInt32();

		private UInt16UInt32()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (uint)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
