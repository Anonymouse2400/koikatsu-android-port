using System.Collections.Generic;
using System.IO;
using MessagePack;

[MessagePackObject(true)]
public class ChaListData
{
	[IgnoreMember]
	public static readonly string ChaListDataMark = "【ChaListData】";

	public string mark { get; set; }

	public int categoryNo { get; set; }

	public int distributionNo { get; set; }

	public string filePath { get; set; }

	public List<string> lstKey { get; set; }

	public Dictionary<int, List<string>> dictList { get; set; }

	[IgnoreMember]
	public string fileName
	{
		get
		{
			return Path.GetFileName(filePath);
		}
	}

	public ChaListData()
	{
		mark = string.Empty;
		categoryNo = 0;
		distributionNo = 0;
		filePath = string.Empty;
		lstKey = new List<string>();
		dictList = new Dictionary<int, List<string>>();
	}

	public Dictionary<string, string> GetInfoAll(int id)
	{
		List<string> value = null;
		if (!dictList.TryGetValue(id, out value))
		{
			return null;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int count = lstKey.Count;
		if (value.Count != count)
		{
			return null;
		}
		for (int i = 0; i < count; i++)
		{
			dictionary[lstKey[i]] = value[i];
		}
		return dictionary;
	}

	public string GetInfo(int id, string key)
	{
		List<string> value = null;
		if (!dictList.TryGetValue(id, out value))
		{
			return string.Empty;
		}
		int num = lstKey.IndexOf(key);
		if (num == -1)
		{
			return string.Empty;
		}
		int count = lstKey.Count;
		if (value.Count != count)
		{
			return null;
		}
		return value[num];
	}
}
