using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion;

public sealed class WeddingData
{
	private Version Version = new Version(1, 0, 0);

	private const string path = "save";

	private const string FileName = "wedding.dat";

	private HashSet<int> _personality = new HashSet<int>();

	public static string Path
	{
		get
		{
			return UserData.Path + "save";
		}
	}

	public HashSet<int> personality
	{
		get
		{
			return _personality;
		}
	}

	public void Save()
	{
		if (!_personality.Any())
		{
			return;
		}
		Utils.File.OpenWrite(UserData.Create("save") + "wedding.dat", false, delegate(FileStream f)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(f))
			{
				binaryWriter.Write(Version.ToString());
				binaryWriter.Write(SaveData.Version.ToString());
				binaryWriter.Write(_personality.Count);
				foreach (int item in _personality)
				{
					binaryWriter.Write(item);
				}
			}
		});
	}

	public bool Load()
	{
		return Utils.File.OpenRead(Path + '/' + "wedding.dat", delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				Version version = new Version(binaryReader.ReadString());
				Version version2 = new Version(binaryReader.ReadString());
				_personality = new HashSet<int>();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					_personality.Add(binaryReader.ReadInt32());
				}
			}
		});
	}
}
