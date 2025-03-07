public static class DebugData
{
	private static FileData fileDat = new FileData("!Debug");

	public static string Path
	{
		get
		{
			return fileDat.Path;
		}
	}

	public static string Create(string name)
	{
		return fileDat.Create(name);
	}
}
