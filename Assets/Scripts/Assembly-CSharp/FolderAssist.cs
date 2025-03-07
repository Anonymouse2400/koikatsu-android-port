using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FolderAssist
{
	public class FileInfo
	{
		private string fullPath = string.Empty;

		private string fileName = string.Empty;

		public string FullPath
		{
			get
			{
				return fullPath;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		public DateTime time { get; private set; }

		public FileInfo(string fullPath, string fileName, DateTime? time = null)
		{
			this.fullPath = fullPath;
			this.fileName = fileName;
			if (time.HasValue)
			{
				this.time = time.Value;
			}
		}

		public FileInfo(FileInfo src)
		{
			Copy(src);
		}

		public void Copy(FileInfo src)
		{
			fullPath = src.FullPath;
			fileName = src.FileName;
			UpdateTime(src);
		}

		public void UpdateTime(FileInfo info)
		{
			UpdateTime(info.time);
		}

		public void UpdateTime(DateTime time)
		{
			this.time = time;
		}
	}

	private List<FileInfo> _lstFile = new List<FileInfo>();

	public const string TIME_FORMAT = "yyyyMMddHHmmssfff";

	public List<FileInfo> lstFile
	{
		get
		{
			return _lstFile;
		}
	}

	public static string TimeStamp(DateTime dateTime)
	{
		return dateTime.ToString("yyyyMMddHHmmssfff");
	}

	public static FileInfo[] CreateFolderInfoToArray(string folder, string searchPattern, bool getFiles = true)
	{
		if (!Directory.Exists(folder))
		{
			return null;
		}
		string[] source = ((!getFiles) ? Directory.GetDirectories(folder) : Directory.GetFiles(folder, searchPattern));
		if (!source.Any())
		{
			return null;
		}
		return source.Select((string path) => new FileInfo(path, getFiles ? Path.GetFileNameWithoutExtension(path) : string.Empty, File.GetLastWriteTime(path))).ToArray();
	}

	public static FileInfo[] CreateFolderInfoExToArray(string folder, params string[] searchPattern)
	{
		if (!Directory.Exists(folder))
		{
			return null;
		}
		string[] source = searchPattern.SelectMany((string pattern) => Directory.GetFiles(folder, pattern)).ToArray();
		if (!source.Any())
		{
			return null;
		}
		return source.Select((string path) => new FileInfo(path, Path.GetFileNameWithoutExtension(path), File.GetLastWriteTime(path))).ToArray();
	}

	public int GetFileCount()
	{
		return lstFile.Count;
	}

	public bool CreateFolderInfo(string folder, string searchPattern, bool getFiles = true, bool clear = true)
	{
		if (clear)
		{
			lstFile.Clear();
		}
		FileInfo[] array = CreateFolderInfoToArray(folder, searchPattern, getFiles);
		bool flag = array != null;
		if (flag)
		{
			lstFile.AddRange(array);
		}
		return flag;
	}

	public bool CreateFolderInfoEx(string folder, string[] searchPattern, bool clear = true)
	{
		if (clear)
		{
			lstFile.Clear();
		}
		FileInfo[] array = CreateFolderInfoExToArray(folder, searchPattern);
		bool flag = array != null;
		if (flag)
		{
			lstFile.AddRange(array);
		}
		return flag;
	}

	public int GetIndexFromFileName(string filename)
	{
		return lstFile.FindIndex((FileInfo file) => file.FileName == filename);
	}

	public void SortName(bool ascend = true)
	{
		if (!lstFile.Any())
		{
			return;
		}
		if (ascend)
		{
			lstFile.Sort((FileInfo a, FileInfo b) => a.FileName.CompareTo(b.FileName));
		}
		else
		{
			lstFile.Sort((FileInfo a, FileInfo b) => b.FileName.CompareTo(a.FileName));
		}
	}

	public void SortDate(bool ascend = true)
	{
		if (!lstFile.Any())
		{
			return;
		}
		if (ascend)
		{
			lstFile.Sort((FileInfo a, FileInfo b) => a.time.CompareTo(b.time));
		}
		else
		{
			lstFile.Sort((FileInfo a, FileInfo b) => b.time.CompareTo(a.time));
		}
	}
}
