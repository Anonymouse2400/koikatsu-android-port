namespace MessagePack.Decoders
{
	internal class UInt32Int32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new UInt32Int32();

		private UInt32Int32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return checked((int)((uint)(bytes[offset + 1] << 24) | (uint)(bytes[offset + 2] << 16) | (uint)(bytes[offset + 3] << 8) | bytes[offset + 4]));
		}
	}
}
