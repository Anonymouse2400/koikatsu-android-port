using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;

public class ChaFile
{
	public int loadProductNo;

	public Version loadVersion = new Version(ChaFileDefine.ChaFileVersion.ToString());

	public byte[] pngData;

	public byte[] facePngData;

	public ChaFileCustom custom;

	public ChaFileParameter parameter;

	public ChaFileStatus status;

	private int lastLoadErrorCode;

	public string charaFileName { get; protected set; }

	public ChaFileCoordinate[] coordinate { get; set; }

	public ChaFile()
	{
		custom = new ChaFileCustom();
		coordinate = new ChaFileCoordinate[Enum.GetNames(typeof(ChaFileDefine.CoordinateType)).Length];
		for (int i = 0; i < coordinate.Length; i++)
		{
			coordinate[i] = new ChaFileCoordinate();
		}
		parameter = new ChaFileParameter();
		status = new ChaFileStatus();
		lastLoadErrorCode = 0;
	}

	public int GetLastErrorCode()
	{
		return lastLoadErrorCode;
	}

	protected bool SaveFile(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		charaFileName = Path.GetFileName(path);
		using (FileStream st = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			return SaveFile(st, true);
		}
	}

	protected bool SaveFile(Stream st, bool savePng)
	{
		using (BinaryWriter bw = new BinaryWriter(st))
		{
			return SaveFile(bw, savePng);
		}
	}

	protected bool SaveFile(BinaryWriter bw, bool savePng)
	{
		if (savePng && pngData != null)
		{
			bw.Write(pngData);
		}
		bw.Write(100);
		bw.Write("【KoiKatuCharaS】");
		bw.Write(ChaFileDefine.ChaFileVersion.ToString());
		int num = 0;
		if (facePngData != null)
		{
			num = facePngData.Length;
		}
		bw.Write(num);
		if (num != 0)
		{
			bw.Write(facePngData);
		}
		byte[] customBytes = GetCustomBytes();
		byte[] coordinateBytes = GetCoordinateBytes();
		byte[] parameterBytes = GetParameterBytes();
		byte[] statusBytes = GetStatusBytes();
		int num2 = 4;
		long num3 = 0L;
		string[] array = new string[4]
		{
			ChaFileCustom.BlockName,
			ChaFileCoordinate.BlockName,
			ChaFileParameter.BlockName,
			ChaFileStatus.BlockName
		};
		string[] array2 = new string[4]
		{
			ChaFileDefine.ChaFileCustomVersion.ToString(),
			ChaFileDefine.ChaFileCoordinateVersion.ToString(),
			ChaFileDefine.ChaFileParameterVersion.ToString(),
			ChaFileDefine.ChaFileStatusVersion.ToString()
		};
		long[] array3 = new long[num2];
		array3[0] = ((customBytes != null) ? customBytes.Length : 0);
		array3[1] = ((coordinateBytes != null) ? coordinateBytes.Length : 0);
		array3[2] = ((parameterBytes != null) ? parameterBytes.Length : 0);
		array3[3] = ((statusBytes != null) ? statusBytes.Length : 0);
		long[] array4 = new long[4]
		{
			num3,
			num3 + array3[0],
			num3 + array3[0] + array3[1],
			num3 + array3[0] + array3[1] + array3[2]
		};
		BlockHeader blockHeader = new BlockHeader();
		for (int i = 0; i < num2; i++)
		{
			BlockHeader.Info info = new BlockHeader.Info();
			info.name = array[i];
			info.version = array2[i];
			info.size = array3[i];
			info.pos = array4[i];
			BlockHeader.Info item = info;
			blockHeader.lstInfo.Add(item);
		}
		byte[] array5 = MessagePackSerializer.Serialize(blockHeader);
		bw.Write(array5.Length);
		bw.Write(array5);
		long num4 = 0L;
		long[] array6 = array3;
		foreach (long num5 in array6)
		{
			num4 += num5;
		}
		bw.Write(num4);
		bw.Write(customBytes);
		bw.Write(coordinateBytes);
		bw.Write(parameterBytes);
		bw.Write(statusBytes);
		return true;
	}

	protected bool LoadFile(string path, bool noLoadPNG = false, bool noLoadStatus = true)
	{
		if (!File.Exists(path))
		{
			lastLoadErrorCode = -6;
			return false;
		}
		charaFileName = Path.GetFileName(path);
		using (FileStream st = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			return LoadFile(st, noLoadPNG, noLoadStatus);
		}
	}

	protected bool LoadFile(Stream st, bool noLoadPNG = false, bool noLoadStatus = true)
	{
		using (BinaryReader br = new BinaryReader(st))
		{
			return LoadFile(br, noLoadPNG, noLoadStatus);
		}
	}

	protected bool LoadFile(BinaryReader br, bool noLoadPNG = false, bool noLoadStatus = true)
	{
		long pngSize = PngFile.GetPngSize(br);
		if (pngSize != 0)
		{
			if (noLoadPNG)
			{
				br.BaseStream.Seek(pngSize, SeekOrigin.Current);
			}
			else
			{
				pngData = br.ReadBytes((int)pngSize);
			}
			if (br.BaseStream.Length - br.BaseStream.Position == 0)
			{
				lastLoadErrorCode = -5;
				return false;
			}
		}
		try
		{
			loadProductNo = br.ReadInt32();
			if (loadProductNo > 100)
			{
				lastLoadErrorCode = -3;
				return false;
			}
			string text = br.ReadString();
			if (text != "【KoiKatuCharaS】")
			{
				lastLoadErrorCode = -1;
				return false;
			}
			loadVersion = new Version(br.ReadString());
			if (0 > ChaFileDefine.ChaFileVersion.CompareTo(loadVersion))
			{
				lastLoadErrorCode = -2;
				return false;
			}
			int num = br.ReadInt32();
			if (num != 0)
			{
				facePngData = br.ReadBytes(num);
			}
			int count = br.ReadInt32();
			byte[] bytes = br.ReadBytes(count);
			BlockHeader blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes);
			long num2 = br.ReadInt64();
			long position = br.BaseStream.Position;
			BlockHeader.Info info = blockHeader.SearchInfo(ChaFileCustom.BlockName);
			if (info != null)
			{
				Version version = new Version(info.version);
				if (0 > ChaFileDefine.ChaFileCustomVersion.CompareTo(version))
				{
					lastLoadErrorCode = -2;
				}
				else
				{
					br.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
					byte[] data = br.ReadBytes((int)info.size);
					SetCustomBytes(data, version);
				}
			}
			info = blockHeader.SearchInfo(ChaFileCoordinate.BlockName);
			if (info != null)
			{
				Version version2 = new Version(info.version);
				if (0 > ChaFileDefine.ChaFileCoordinateVersion.CompareTo(version2))
				{
					lastLoadErrorCode = -2;
				}
				else
				{
					br.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
					byte[] data2 = br.ReadBytes((int)info.size);
					SetCoordinateBytes(data2, version2);
				}
			}
			info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
			if (info != null)
			{
				Version value = new Version(info.version);
				if (0 > ChaFileDefine.ChaFileParameterVersion.CompareTo(value))
				{
					lastLoadErrorCode = -2;
				}
				else
				{
					br.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
					byte[] parameterBytes = br.ReadBytes((int)info.size);
					SetParameterBytes(parameterBytes);
				}
			}
			if (!noLoadStatus)
			{
				info = blockHeader.SearchInfo(ChaFileStatus.BlockName);
				if (info != null)
				{
					Version value2 = new Version(info.version);
					if (0 > ChaFileDefine.ChaFileStatusVersion.CompareTo(value2))
					{
						lastLoadErrorCode = -2;
					}
					else
					{
						br.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
						byte[] statusBytes = br.ReadBytes((int)info.size);
						SetStatusBytes(statusBytes);
					}
				}
			}
			br.BaseStream.Seek(position + num2, SeekOrigin.Begin);
		}
		catch (EndOfStreamException)
		{
			lastLoadErrorCode = -999;
			return false;
		}
		lastLoadErrorCode = 0;
		return true;
	}

	public bool AssignCoordinate(ChaFileDefine.CoordinateType type, ChaFileCoordinate srcCoorde)
	{
		if (srcCoorde == null)
		{
			return false;
		}
		byte[] data = srcCoorde.SaveBytes();
		coordinate[(int)type].LoadBytes(data, srcCoorde.loadVersion);
		return true;
	}

	public byte[] GetCustomBytes()
	{
		return GetCustomBytes(custom);
	}

	public static byte[] GetCustomBytes(ChaFileCustom _custom)
	{
		return _custom.SaveBytes();
	}

	public byte[] GetCoordinateBytes()
	{
		return GetCoordinateBytes(coordinate);
	}

	public static byte[] GetCoordinateBytes(ChaFileCoordinate[] _coordinate)
	{
		if (_coordinate.Length == 0)
		{
			return null;
		}
		List<byte[]> list = new List<byte[]>();
		foreach (ChaFileCoordinate chaFileCoordinate in _coordinate)
		{
			list.Add(chaFileCoordinate.SaveBytes());
		}
		return MessagePackSerializer.Serialize(list);
	}

	public byte[] GetParameterBytes()
	{
		return GetParameterBytes(parameter);
	}

	public static byte[] GetParameterBytes(ChaFileParameter _parameter)
	{
		return MessagePackSerializer.Serialize(_parameter);
	}

	public byte[] GetStatusBytes()
	{
		return GetStatusBytes(status);
	}

	public static byte[] GetStatusBytes(ChaFileStatus _status)
	{
		return MessagePackSerializer.Serialize(_status);
	}

	public void SetCustomBytes(byte[] data, Version ver)
	{
		custom.LoadBytes(data, ver);
	}

	public void SetCoordinateBytes(byte[] data, Version ver)
	{
		List<byte[]> list = MessagePackSerializer.Deserialize<List<byte[]>>(data);
		for (int i = 0; i < coordinate.Length && list.Count > i; i++)
		{
			coordinate[i].LoadBytes(list[i], ver);
		}
	}

	public void SetParameterBytes(byte[] data)
	{
		ChaFileParameter chaFileParameter = MessagePackSerializer.Deserialize<ChaFileParameter>(data);
		chaFileParameter.ComplementWithVersion();
		parameter.Copy(chaFileParameter);
	}

	public void SetStatusBytes(byte[] data)
	{
		ChaFileStatus chaFileStatus = MessagePackSerializer.Deserialize<ChaFileStatus>(data);
		chaFileStatus.ComplementWithVersion();
		status.Copy(chaFileStatus);
	}

	public static void CopyChaFile(ChaFile dst, ChaFile src)
	{
		dst.CopyAll(src);
	}

	public void CopyAll(ChaFile _chafile)
	{
		CopyCustom(_chafile.custom);
		CopyCoordinate(_chafile.coordinate);
		CopyParameter(_chafile.parameter);
		CopyStatus(_chafile.status);
	}

	public void CopyCustom(ChaFileCustom _custom)
	{
		byte[] customBytes = GetCustomBytes(_custom);
		SetCustomBytes(customBytes, ChaFileDefine.ChaFileCustomVersion);
	}

	public void CopyCoordinate(ChaFileCoordinate[] _coordinate)
	{
		byte[] coordinateBytes = GetCoordinateBytes(_coordinate);
		SetCoordinateBytes(coordinateBytes, ChaFileDefine.ChaFileCoordinateVersion);
	}

	public void CopyParameter(ChaFileParameter _parameter)
	{
		byte[] parameterBytes = GetParameterBytes(_parameter);
		SetParameterBytes(parameterBytes);
	}

	public void CopyStatus(ChaFileStatus _status)
	{
		byte[] statusBytes = GetStatusBytes(_status);
		SetStatusBytes(statusBytes);
	}
}
