using System.IO;
using UnityEngine;

public class FileData
{
	private string rootName = string.Empty;

	public string Path
	{
		get
		{
			string text = ((!Application.isEditor && Application.platform != RuntimePlatform.WindowsPlayer) ? (Application.persistentDataPath + "/") : (Application.dataPath + "/../"));
			if (rootName != string.Empty)
			{
				text = text + rootName + '/';
			}
			return text;
		}
	}

	public FileData(string rootName = "")
	{
		this.rootName = rootName;
	}

	public string Create(string name)
	{
		string text = Path + name;
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text + '/';
	}
}
