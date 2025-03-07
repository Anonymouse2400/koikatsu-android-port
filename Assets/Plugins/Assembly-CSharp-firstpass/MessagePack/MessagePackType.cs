namespace MessagePack
{
	public enum MessagePackType : byte
	{
		Unknown = 0,
		Integer = 1,
		Nil = 2,
		Boolean = 3,
		Float = 4,
		String = 5,
		Binary = 6,
		Array = 7,
		Map = 8,
		Extension = 9
	}
}
