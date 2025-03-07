namespace MessagePack.Decoders
{
	internal class FixExt1 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new FixExt1();

		private FixExt1()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			byte[] data = new byte[1] { bytes[offset + 2] };
			return new ExtensionResult(typeCode, data);
		}
	}
}
