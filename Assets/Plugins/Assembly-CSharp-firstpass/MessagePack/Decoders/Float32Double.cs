namespace MessagePack.Decoders
{
	internal class Float32Double : IDoubleDecoder
	{
		internal static readonly IDoubleDecoder Instance = new Float32Double();

		private Float32Double()
		{
		}

		public double Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return new Float32Bits(bytes, offset + 1).Value;
		}
	}
}
