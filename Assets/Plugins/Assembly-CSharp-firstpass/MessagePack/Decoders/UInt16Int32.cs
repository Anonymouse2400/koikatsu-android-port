namespace MessagePack.Decoders
{
	internal class UInt16Int32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new UInt16Int32();

		private UInt16Int32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (bytes[offset + 1] << 8) | bytes[offset + 2];
		}
	}
}
