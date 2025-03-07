using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion;

public sealed class TutorialData
{
	private Version Version = new Version(1, 0, 0);

	private const string path = "save";

	private const string FileName = "tutorial.dat";

	public static string Path
	{
		get
		{
			return UserData.Path + "save";
		}
	}

	public HashSet<int> data { get; private set; }

	public TutorialData()
	{
		data = new HashSet<int>();
	}

	public void Save()
	{
		if (!data.Any())
		{
			return;
		}
		Utils.File.OpenWrite(UserData.Create("save") + "tutorial.dat", false, delegate(FileStream f)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(f))
			{
				binaryWriter.Write(Version.ToString());
				binaryWriter.Write(SaveData.Version.ToString());
				binaryWriter.Write(data.Count);
				foreach (int datum in data)
				{
					binaryWriter.Write(datum);
				}
			}
		});
	}

	public bool Load()
	{
		return Utils.File.OpenRead(Path + '/' + "tutorial.dat", delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				Version version = new Version(binaryReader.ReadString());
				Version version2 = new Version(binaryReader.ReadString());
				data.Clear();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					data.Add(binaryReader.ReadInt32());
				}
			}
		});
	}
}
