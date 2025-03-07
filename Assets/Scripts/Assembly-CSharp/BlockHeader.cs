using System.Collections.Generic;
using MessagePack;

[MessagePackObject(true)]
public class BlockHeader
{
	[MessagePackObject(true)]
	public class Info
	{
		public string name { get; set; }

		public string version { get; set; }

		public long pos { get; set; }

		public long size { get; set; }

		public Info()
		{
			name = string.Empty;
			version = string.Empty;
			pos = 0L;
			size = 0L;
		}
	}

	public List<Info> lstInfo { get; set; }

	public BlockHeader()
	{
		lstInfo = new List<Info>();
	}

	public Info SearchInfo(string name)
	{
		return lstInfo.Find((Info n) => n.name == name);
	}
}
