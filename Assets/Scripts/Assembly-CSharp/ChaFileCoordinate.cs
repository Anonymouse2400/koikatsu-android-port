using System;
using System.IO;
using MessagePack;
using UnityEngine;

public class ChaFileCoordinate : ChaFileAssist
{
	public static readonly string BlockName = "Coordinate";

	public int loadProductNo;

	public Version loadVersion = new Version(ChaFileDefine.ChaFileCoordinateVersion.ToString());

	public ChaFileClothes clothes;

	public ChaFileAccessory accessory;

	public bool enableMakeup;

	public ChaFileMakeup makeup;

	public string coordinateName = string.Empty;

	public byte[] pngData;

	private int lastLoadErrorCode;

	public string coordinateFileName { get; private set; }

	public ChaFileCoordinate()
	{
		MemberInit();
	}

	public int GetLastErrorCode()
	{
		return lastLoadErrorCode;
	}

	public void MemberInit()
	{
		clothes = new ChaFileClothes();
		accessory = new ChaFileAccessory();
		makeup = new ChaFileMakeup();
		coordinateFileName = string.Empty;
		coordinateName = string.Empty;
		pngData = null;
		lastLoadErrorCode = 0;
	}

	public byte[] SaveBytes()
	{
		byte[] array = MessagePackSerializer.Serialize(clothes);
		byte[] array2 = MessagePackSerializer.Serialize(accessory);
		byte[] array3 = MessagePackSerializer.Serialize(makeup);
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(array.Length);
				binaryWriter.Write(array);
				binaryWriter.Write(array2.Length);
				binaryWriter.Write(array2);
				binaryWriter.Write(enableMakeup);
				binaryWriter.Write(array3.Length);
				binaryWriter.Write(array3);
				return memoryStream.ToArray();
			}
		}
	}

	public bool LoadBytes(byte[] data, Version ver)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				try
				{
					int count = binaryReader.ReadInt32();
					byte[] bytes = binaryReader.ReadBytes(count);
					clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes);
					count = binaryReader.ReadInt32();
					bytes = binaryReader.ReadBytes(count);
					accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes);
					enableMakeup = binaryReader.ReadBoolean();
					count = binaryReader.ReadInt32();
					bytes = binaryReader.ReadBytes(count);
					makeup = MessagePackSerializer.Deserialize<ChaFileMakeup>(bytes);
				}
				catch (EndOfStreamException)
				{
					return false;
				}
				clothes.ComplementWithVersion();
				accessory.ComplementWithVersion();
				makeup.ComplementWithVersion();
				return true;
			}
		}
	}

	public void SaveFile(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		coordinateFileName = Path.GetFileName(path);
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				if (pngData != null)
				{
					binaryWriter.Write(pngData);
				}
				binaryWriter.Write(100);
				binaryWriter.Write("【KoiKatuClothes】");
				binaryWriter.Write(ChaFileDefine.ChaFileCoordinateVersion.ToString());
				binaryWriter.Write(coordinateName);
				byte[] array = SaveBytes();
				binaryWriter.Write(array.Length);
				binaryWriter.Write(array);
			}
		}
	}

	public bool LoadFile(TextAsset ta)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			memoryStream.Write(ta.bytes, 0, ta.bytes.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return LoadFile(memoryStream);
		}
	}

	public bool LoadFile(string path)
	{
		if (!File.Exists(path))
		{
			lastLoadErrorCode = -6;
			return false;
		}
		coordinateFileName = Path.GetFileName(path);
		using (FileStream st = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			return LoadFile(st);
		}
	}

	public bool LoadFile(Stream st)
	{
		using (BinaryReader binaryReader = new BinaryReader(st))
		{
			try
			{
				PngFile.SkipPng(binaryReader);
				if (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position == 0)
				{
					lastLoadErrorCode = -5;
					return false;
				}
				loadProductNo = binaryReader.ReadInt32();
				if (loadProductNo > 100)
				{
					lastLoadErrorCode = -3;
					return false;
				}
				string text = binaryReader.ReadString();
				if (text != "【KoiKatuClothes】")
				{
					lastLoadErrorCode = -1;
					return false;
				}
				loadVersion = new Version(binaryReader.ReadString());
				if (0 > ChaFileDefine.ChaFileClothesVersion.CompareTo(loadVersion))
				{
					lastLoadErrorCode = -2;
					return false;
				}
				coordinateName = binaryReader.ReadString();
				int count = binaryReader.ReadInt32();
				byte[] data = binaryReader.ReadBytes(count);
				if (LoadBytes(data, loadVersion))
				{
					lastLoadErrorCode = 0;
					return true;
				}
				lastLoadErrorCode = -999;
				return false;
			}
			catch (EndOfStreamException)
			{
				lastLoadErrorCode = -999;
				return false;
			}
		}
	}

	protected void SaveClothes(string path)
	{
		SaveFileAssist(path, clothes);
	}

	protected void LoadClothes(string path)
	{
		LoadFileAssist(path, clothes);
		clothes.ComplementWithVersion();
	}

	protected void SaveAccessory(string path)
	{
		SaveFileAssist(path, accessory);
	}

	protected void LoadAccessory(string path)
	{
		LoadFileAssist(path, accessory);
		accessory.ComplementWithVersion();
	}

	protected void SaveMakeup(string path)
	{
		SaveFileAssist(path, makeup);
	}

	protected void LoadMakeup(string path)
	{
		LoadFileAssist(path, makeup);
		makeup.ComplementWithVersion();
	}
}
