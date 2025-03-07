namespace MessagePack.Decoders
{
	internal class FixInt32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new FixInt32();

		private FixInt32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return bytes[offset];
		}
	}
}
