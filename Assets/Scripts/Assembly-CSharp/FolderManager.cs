using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FolderManager
{
	public static bool FolderSearch(string loadDir, out string[] path_array, string searchPattern = "*.*")
	{
		if (!Directory.Exists(loadDir))
		{
			Directory.CreateDirectory(loadDir);
		}
		DateTime oldDateTime = new DateTime(2014, 12, 1, 0, 0, 0);
		List<SortData> list = (from p in Directory.GetFiles(loadDir, searchPattern).Select((string path, int index) => new { path, index })
			select new SortData(p.index, (Directory.GetCreationTime(p.path) - oldDateTime).TotalSeconds, p.path)).ToList();
		list.Sort(SortData.CompareDateTimeDesc);
		path_array = list.Select((SortData p) => p.path).ToArray();
		return path_array.Length > 0;
	}
}
