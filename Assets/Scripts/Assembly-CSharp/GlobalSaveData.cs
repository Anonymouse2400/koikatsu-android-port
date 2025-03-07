using System;
using System.Collections.Generic;
using System.IO;
using Illusion;

[Serializable]
public sealed class GlobalSaveData
{
	private Version Version = new Version(0, 0, 3);

	private const string path = "save";

	private const string FileName = "global.dat";

	private bool _isOpeningEnd;

	public static string Path
	{
		get
		{
			return UserData.Path + "save";
		}
	}

	public Dictionary<int, HashSet<int>> clubContents { get; private set; }

	public Dictionary<int, HashSet<int>> playHList { get; private set; }

	public HashSet<int> tutorialHash { get; private set; }

	public HashSet<int> fixCharaTaked { get; private set; }

	public bool isOpeningEnd
	{
		get
		{
			return _isOpeningEnd;
		}
		set
		{
			_isOpeningEnd = value;
		}
	}

	public GlobalSaveData()
	{
		clubContents = new Dictionary<int, HashSet<int>>();
		playHList = new Dictionary<int, HashSet<int>>();
		playHList[0] = new HashSet<int>(new int[1]);
		playHList[1] = new HashSet<int>(new int[1] { 1 });
		playHList[2] = new HashSet<int>(new int[1]);
		tutorialHash = new HashSet<int>();
		fixCharaTaked = new HashSet<int>();
	}

	public void Save()
	{
		Utils.File.OpenWrite(UserData.Create("save") + "global.dat", false, delegate(FileStream f)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(f))
			{
				binaryWriter.Write(Version.ToString());
				binaryWriter.Write(clubContents.Count);
				foreach (KeyValuePair<int, HashSet<int>> clubContent in clubContents)
				{
					binaryWriter.Write(clubContent.Key);
					binaryWriter.Write(clubContent.Value.Count);
					foreach (int item in clubContent.Value)
					{
						binaryWriter.Write(item);
					}
				}
				binaryWriter.Write(playHList.Count);
				foreach (KeyValuePair<int, HashSet<int>> playH in playHList)
				{
					binaryWriter.Write(playH.Key);
					binaryWriter.Write(playH.Value.Count);
					foreach (int item2 in playH.Value)
					{
						binaryWriter.Write(item2);
					}
				}
				binaryWriter.Write(tutorialHash.Count);
				foreach (int item3 in tutorialHash)
				{
					binaryWriter.Write(item3);
				}
				binaryWriter.Write(fixCharaTaked.Count);
				foreach (int item4 in fixCharaTaked)
				{
					binaryWriter.Write(item4);
				}
				binaryWriter.Write(_isOpeningEnd);
			}
		});
	}

	public bool Load()
	{
		return Utils.File.OpenRead(Path + '/' + "global.dat", delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				Version version = new Version(binaryReader.ReadString());
				if (version >= new Version(0, 0, 0))
				{
					int num = 0;
					clubContents.Clear();
					num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						int key = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						HashSet<int> hashSet = new HashSet<int>();
						for (int j = 0; j < num2; j++)
						{
							hashSet.Add(binaryReader.ReadInt32());
						}
						clubContents.Add(key, hashSet);
					}
					playHList.Clear();
					num = binaryReader.ReadInt32();
					for (int k = 0; k < num; k++)
					{
						int key2 = binaryReader.ReadInt32();
						int num3 = binaryReader.ReadInt32();
						HashSet<int> hashSet2 = new HashSet<int>();
						for (int l = 0; l < num3; l++)
						{
							hashSet2.Add(binaryReader.ReadInt32());
						}
						playHList.Add(key2, hashSet2);
					}
					tutorialHash.Clear();
					if (version.CompareTo(new Version(0, 0, 1)) >= 0)
					{
						num = binaryReader.ReadInt32();
						for (int m = 0; m < num; m++)
						{
							tutorialHash.Add(binaryReader.ReadInt32());
						}
					}
					fixCharaTaked.Clear();
					if (version.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						num = binaryReader.ReadInt32();
						for (int n = 0; n < num; n++)
						{
							fixCharaTaked.Add(binaryReader.ReadInt32());
						}
					}
					if (version.CompareTo(new Version(0, 0, 3)) >= 0)
					{
						_isOpeningEnd = binaryReader.ReadBoolean();
					}
				}
			}
		});
	}
}
