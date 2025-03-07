namespace MessagePack.Decoders
{
	internal class NilBytes : IBytesDecoder
	{
		internal static readonly IBytesDecoder Instance = new NilBytes();

		private NilBytes()
		{
		}

		public byte[] Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return null;
		}
	}
}
