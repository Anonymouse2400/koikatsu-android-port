namespace MessagePack.Decoders
{
	internal class Float64Double : IDoubleDecoder
	{
		internal static readonly IDoubleDecoder Instance = new Float64Double();

		private Float64Double()
		{
		}

		public double Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 9;
			return new Float64Bits(bytes, offset + 1).Value;
		}
	}
}
