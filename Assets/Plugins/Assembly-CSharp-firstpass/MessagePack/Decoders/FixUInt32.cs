namespace MessagePack.Decoders
{
	internal class FixUInt32 : IUInt32Decoder
	{
		internal static readonly IUInt32Decoder Instance = new FixUInt32();

		private FixUInt32()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return bytes[offset];
		}
	}
}
