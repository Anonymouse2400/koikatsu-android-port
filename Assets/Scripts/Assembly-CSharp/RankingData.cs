using System;
using System.IO;
using Illusion;

public sealed class RankingData
{
	public const int NUM = 26;

	public const int INIT_RANK = 999;

	private const string path = "save";

	private const string FileName = "ranking.dat";

	private Version Version = new Version(0, 0, 1);

	private string _userName = string.Empty;

	private uint _peepToiletCount;

	private uint _peepShowerCount;

	private uint _peepOnanismCount;

	private uint _easyPlaceHCount;

	private uint _orgasmCount;

	private uint _inEjaculationCount;

	private uint _inEjaculationKokanNamaCount;

	private uint _inEjaculationAnalCount;

	private uint _outEjaculationCount;

	private uint _angerCount;

	private uint _dateCount;

	private uint _myRoomHCount;

	private uint _bustGauge;

	private uint _hipGauge;

	private uint _asokoGauge;

	private uint _adultToyGauge;

	private uint _staffCount;

	private uint _girlfriendCount;

	private uint _expNumCount;

	private float _playTimeCalc;

	private uint _playTime;

	private uint _angerLockerCount;

	private uint _angerToiletCount;

	private uint _angerShowerCount;

	private uint _physicalCount;

	private uint _intellectCount;

	private uint _hentaiCount;

	private int[] _prevRanks;

	public static string Path
	{
		get
		{
			return UserData.Path + "save";
		}
	}

	public int[] prevRanks
	{
		get
		{
			return _prevRanks;
		}
	}

	public string userName
	{
		get
		{
			return _userName;
		}
		set
		{
			_userName = value;
		}
	}

	public uint peepToiletCount
	{
		get
		{
			return _peepToiletCount;
		}
		set
		{
			ValueCheck(ref _peepToiletCount, value);
		}
	}

	public uint peepShowerCount
	{
		get
		{
			return _peepShowerCount;
		}
		set
		{
			ValueCheck(ref _peepShowerCount, value);
		}
	}

	public uint peepOnanismCount
	{
		get
		{
			return _peepOnanismCount;
		}
		set
		{
			ValueCheck(ref _peepOnanismCount, value);
		}
	}

	public uint easyPlaceHCount
	{
		get
		{
			return _easyPlaceHCount;
		}
		set
		{
			ValueCheck(ref _easyPlaceHCount, value);
		}
	}

	public uint orgasmCount
	{
		get
		{
			return _orgasmCount;
		}
		set
		{
			ValueCheck(ref _orgasmCount, value);
		}
	}

	public uint inEjaculationCount
	{
		get
		{
			return _inEjaculationCount;
		}
		set
		{
			ValueCheck(ref _inEjaculationCount, value);
		}
	}

	public uint inEjaculationKokanNamaCount
	{
		get
		{
			return _inEjaculationKokanNamaCount;
		}
		set
		{
			ValueCheck(ref _inEjaculationKokanNamaCount, value);
		}
	}

	public uint inEjaculationAnalCount
	{
		get
		{
			return _inEjaculationAnalCount;
		}
		set
		{
			ValueCheck(ref _inEjaculationAnalCount, value);
		}
	}

	public uint outEjaculationCount
	{
		get
		{
			return _outEjaculationCount;
		}
		set
		{
			ValueCheck(ref _outEjaculationCount, value);
		}
	}

	public uint angerCount
	{
		get
		{
			return _angerCount;
		}
		set
		{
			ValueCheck(ref _angerCount, value);
		}
	}

	public uint dateCount
	{
		get
		{
			return _dateCount;
		}
		set
		{
			ValueCheck(ref _dateCount, value);
		}
	}

	public uint myRoomHCount
	{
		get
		{
			return _myRoomHCount;
		}
		set
		{
			ValueCheck(ref _myRoomHCount, value);
		}
	}

	public uint bustGauge
	{
		get
		{
			return _bustGauge;
		}
		set
		{
			ValueCheck(ref _bustGauge, value);
		}
	}

	public uint hipGauge
	{
		get
		{
			return _hipGauge;
		}
		set
		{
			ValueCheck(ref _hipGauge, value);
		}
	}

	public uint asokoGauge
	{
		get
		{
			return _asokoGauge;
		}
		set
		{
			ValueCheck(ref _asokoGauge, value);
		}
	}

	public uint adultToyGauge
	{
		get
		{
			return _adultToyGauge;
		}
		set
		{
			ValueCheck(ref _adultToyGauge, value);
		}
	}

	public uint staffCount
	{
		get
		{
			return _staffCount;
		}
		set
		{
			ValueCheck(ref _staffCount, value);
		}
	}

	public uint girlfriendCount
	{
		get
		{
			return _girlfriendCount;
		}
		set
		{
			ValueCheck(ref _girlfriendCount, value);
		}
	}

	public uint expNumCount
	{
		get
		{
			return _expNumCount;
		}
		set
		{
			ValueCheck(ref _expNumCount, value);
		}
	}

	public float playTimeCalc
	{
		get
		{
			return _playTimeCalc;
		}
		set
		{
			_playTimeCalc = value;
		}
	}

	public uint playTime
	{
		get
		{
			return _playTime;
		}
		set
		{
			ValueCheck(ref _playTime, value);
		}
	}

	public uint angerLockerCount
	{
		get
		{
			return _angerLockerCount;
		}
		set
		{
			ValueCheck(ref _angerLockerCount, value);
		}
	}

	public uint angerToiletCount
	{
		get
		{
			return _angerToiletCount;
		}
		set
		{
			ValueCheck(ref _angerToiletCount, value);
		}
	}

	public uint angerShowerCount
	{
		get
		{
			return _angerShowerCount;
		}
		set
		{
			ValueCheck(ref _angerShowerCount, value);
		}
	}

	public uint physicalCount
	{
		get
		{
			return _physicalCount;
		}
		set
		{
			ValueCheck(ref _physicalCount, value);
		}
	}

	public uint intellectCount
	{
		get
		{
			return _intellectCount;
		}
		set
		{
			ValueCheck(ref _intellectCount, value);
		}
	}

	public uint hentaiCount
	{
		get
		{
			return _hentaiCount;
		}
		set
		{
			ValueCheck(ref _hentaiCount, value);
		}
	}

	public RankingData()
	{
		_prevRanks = new int[26];
		for (int i = 0; i < 26; i++)
		{
			_prevRanks[i] = 999;
		}
	}

	public static void ValueCheck(ref uint src, uint value)
	{
		if (src > value)
		{
			src = uint.MaxValue;
		}
		else
		{
			src = value;
		}
	}

	public void ResetValue(int no)
	{
		switch (no)
		{
		case 0:
			_peepToiletCount = 0u;
			break;
		case 1:
			_peepShowerCount = 0u;
			break;
		case 2:
			_peepOnanismCount = 0u;
			break;
		case 3:
			_easyPlaceHCount = 0u;
			break;
		case 4:
			_orgasmCount = 0u;
			break;
		case 5:
			_inEjaculationCount = 0u;
			break;
		case 6:
			_inEjaculationKokanNamaCount = 0u;
			break;
		case 7:
			_inEjaculationAnalCount = 0u;
			break;
		case 8:
			_outEjaculationCount = 0u;
			break;
		case 9:
			_angerCount = 0u;
			break;
		case 10:
			_dateCount = 0u;
			break;
		case 11:
			_myRoomHCount = 0u;
			break;
		case 12:
			_bustGauge = 0u;
			break;
		case 13:
			_hipGauge = 0u;
			break;
		case 14:
			_asokoGauge = 0u;
			break;
		case 15:
			_adultToyGauge = 0u;
			break;
		case 16:
			_staffCount = 0u;
			break;
		case 17:
			_girlfriendCount = 0u;
			break;
		case 18:
			_expNumCount = 0u;
			break;
		case 19:
			_playTime = 0u;
			break;
		case 20:
			_angerLockerCount = 0u;
			break;
		case 21:
			_angerToiletCount = 0u;
			break;
		case 22:
			_angerShowerCount = 0u;
			break;
		case 23:
			_physicalCount = 0u;
			break;
		case 24:
			_intellectCount = 0u;
			break;
		case 25:
			_hentaiCount = 0u;
			break;
		}
	}

	public void Save()
	{
		Utils.File.OpenWrite(UserData.Create("save") + "ranking.dat", false, delegate(FileStream f)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(f))
			{
				binaryWriter.Write(Version.ToString());
				binaryWriter.Write(_userName);
				binaryWriter.Write(_peepToiletCount);
				binaryWriter.Write(_peepShowerCount);
				binaryWriter.Write(_peepOnanismCount);
				binaryWriter.Write(_easyPlaceHCount);
				binaryWriter.Write(_orgasmCount);
				binaryWriter.Write(_inEjaculationCount);
				binaryWriter.Write(_inEjaculationKokanNamaCount);
				binaryWriter.Write(_inEjaculationAnalCount);
				binaryWriter.Write(_outEjaculationCount);
				binaryWriter.Write(_angerCount);
				binaryWriter.Write(_dateCount);
				binaryWriter.Write(_myRoomHCount);
				binaryWriter.Write(_bustGauge);
				binaryWriter.Write(_hipGauge);
				binaryWriter.Write(_asokoGauge);
				binaryWriter.Write(_adultToyGauge);
				binaryWriter.Write(_staffCount);
				binaryWriter.Write(_girlfriendCount);
				binaryWriter.Write(_expNumCount);
				binaryWriter.Write(_playTimeCalc);
				binaryWriter.Write(_playTime);
				binaryWriter.Write(_angerLockerCount);
				binaryWriter.Write(_angerToiletCount);
				binaryWriter.Write(_angerShowerCount);
				binaryWriter.Write(_physicalCount);
				binaryWriter.Write(_intellectCount);
				binaryWriter.Write(_hentaiCount);
				binaryWriter.Write(_prevRanks.Length);
				for (int i = 0; i < _prevRanks.Length; i++)
				{
					binaryWriter.Write(_prevRanks[i]);
				}
			}
		});
	}

	public bool Load()
	{
		return Utils.File.OpenRead(Path + '/' + "ranking.dat", delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				Version version = new Version(binaryReader.ReadString());
				_userName = binaryReader.ReadString();
				_peepToiletCount = binaryReader.ReadUInt32();
				_peepShowerCount = binaryReader.ReadUInt32();
				_peepOnanismCount = binaryReader.ReadUInt32();
				_easyPlaceHCount = binaryReader.ReadUInt32();
				_orgasmCount = binaryReader.ReadUInt32();
				_inEjaculationCount = binaryReader.ReadUInt32();
				_inEjaculationKokanNamaCount = binaryReader.ReadUInt32();
				_inEjaculationAnalCount = binaryReader.ReadUInt32();
				_outEjaculationCount = binaryReader.ReadUInt32();
				_angerCount = binaryReader.ReadUInt32();
				_dateCount = binaryReader.ReadUInt32();
				_myRoomHCount = binaryReader.ReadUInt32();
				_bustGauge = binaryReader.ReadUInt32();
				_hipGauge = binaryReader.ReadUInt32();
				_asokoGauge = binaryReader.ReadUInt32();
				_adultToyGauge = binaryReader.ReadUInt32();
				_staffCount = binaryReader.ReadUInt32();
				_girlfriendCount = binaryReader.ReadUInt32();
				_expNumCount = binaryReader.ReadUInt32();
				_playTimeCalc = binaryReader.ReadSingle();
				_playTime = binaryReader.ReadUInt32();
				_angerLockerCount = binaryReader.ReadUInt32();
				_angerToiletCount = binaryReader.ReadUInt32();
				_angerShowerCount = binaryReader.ReadUInt32();
				_physicalCount = binaryReader.ReadUInt32();
				_intellectCount = binaryReader.ReadUInt32();
				_hentaiCount = binaryReader.ReadUInt32();
				if (version.CompareTo(new Version(0, 0, 1)) >= 0)
				{
					int num = binaryReader.ReadInt32();
					if (_prevRanks.Length < num)
					{
						_prevRanks = new int[num];
					}
					for (int i = 0; i < num; i++)
					{
						_prevRanks[i] = binaryReader.ReadInt32();
					}
				}
			}
		});
	}
}
