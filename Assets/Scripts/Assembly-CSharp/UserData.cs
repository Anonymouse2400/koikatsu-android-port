public static class UserData
{
	private static FileData fileDat = new FileData("UserData");

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
