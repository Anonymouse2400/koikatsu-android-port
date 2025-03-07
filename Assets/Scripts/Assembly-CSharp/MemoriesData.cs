using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion;
using Manager;

public sealed class MemoriesData
{
	public class EtcData
	{
		public bool initialized;

		public byte[] pngData;

		public EtcData()
		{
		}

		public EtcData(Version version, BinaryReader r)
		{
			Load(version, r);
		}

		public void Save(BinaryWriter w)
		{
			w.Write(initialized);
			int num = ((pngData != null) ? pngData.Length : 0);
			w.Write(num);
			if (num != 0)
			{
				w.Write(pngData);
			}
		}

		public void Load(Version version, BinaryReader r)
		{
			initialized = r.ReadBoolean();
			int num = r.ReadInt32();
			if (num != 0)
			{
				pngData = r.ReadBytes(num);
			}
		}
	}

	public const int PLAYER_FIX_ID = -100;

	private Version Version = new Version(1, 0, 2);

	private const string path = "save";

	private const string FileName = "memories.dat";

	public Dictionary<int, SaveData.Heroine> heroineDic = new Dictionary<int, SaveData.Heroine>();

	public Dictionary<int, EtcData> etcData;

	public SaveData.Player player = new SaveData.Player();

	public static string Path
	{
		get
		{
			return UserData.Path + "save";
		}
	}

	public void Initialize()
	{
		etcData = new Dictionary<int, EtcData>();
		CreateFixHeroines();
		CreatePlayer();
	}

	public void CreateFixHeroines()
	{
		List<SaveData.Heroine> list = Game.CreateFixCharaList(heroineDic.Keys.ToArray());
		foreach (SaveData.Heroine item in list)
		{
			int fixCharaID = item.fixCharaID;
			heroineDic[fixCharaID] = item;
			EtcData value;
			if (!etcData.TryGetValue(fixCharaID, out value))
			{
				etcData[fixCharaID] = new EtcData();
			}
		}
	}

	public void CreatePlayer()
	{
		Game.LoadFromTextAsset(player);
		etcData[-100] = new EtcData();
	}

	public void Save()
	{
		Utils.File.OpenWrite(UserData.Create("save") + "memories.dat", false, delegate(FileStream f)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(f))
			{
				binaryWriter.Write(Version.ToString());
				binaryWriter.Write(SaveData.Version.ToString());
				player.Save(binaryWriter);
				binaryWriter.Write(heroineDic.Count);
				foreach (KeyValuePair<int, SaveData.Heroine> item in heroineDic)
				{
					binaryWriter.Write(item.Key);
					item.Value.Save(binaryWriter);
				}
				binaryWriter.Write(etcData.Count);
				foreach (KeyValuePair<int, EtcData> etcDatum in etcData)
				{
					binaryWriter.Write(etcDatum.Key);
					etcDatum.Value.Save(binaryWriter);
				}
			}
		});
	}

	public bool Load()
	{
		return Utils.File.OpenRead(Path + '/' + "memories.dat", delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				Version version = new Version(binaryReader.ReadString());
				Version version2 = new Version(1, 0, 0);
				if (version.CompareTo(new Version(1, 0, 2)) >= 0)
				{
					version2 = new Version(binaryReader.ReadString());
				}
				player = new SaveData.Player(version2, binaryReader);
				heroineDic = new Dictionary<int, SaveData.Heroine>();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int key = binaryReader.ReadInt32();
					heroineDic[key] = new SaveData.Heroine(version2, binaryReader);
				}
				if (version.CompareTo(new Version(1, 0, 1)) >= 0)
				{
					num = binaryReader.ReadInt32();
					for (int j = 0; j < num; j++)
					{
						int key2 = binaryReader.ReadInt32();
						etcData[key2] = new EtcData(version, binaryReader);
					}
				}
			}
		});
	}
}
