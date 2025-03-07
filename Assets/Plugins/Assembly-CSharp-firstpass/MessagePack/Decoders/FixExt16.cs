namespace MessagePack.Decoders
{
	internal class FixExt16 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new FixExt16();

		private FixExt16()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 18;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			byte[] data = new byte[16]
			{
				bytes[offset + 2],
				bytes[offset + 3],
				bytes[offset + 4],
				bytes[offset + 5],
				bytes[offset + 6],
				bytes[offset + 7],
				bytes[offset + 8],
				bytes[offset + 9],
				bytes[offset + 10],
				bytes[offset + 11],
				bytes[offset + 12],
				bytes[offset + 13],
				bytes[offset + 14],
				bytes[offset + 15],
				bytes[offset + 16],
				bytes[offset + 17]
			};
			return new ExtensionResult(typeCode, data);
		}
	}
}
