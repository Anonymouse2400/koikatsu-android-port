using System;
using System.Collections.Generic;
using System.IO;
using Manager;
using UnityEngine;

namespace Studio
{
	public class SceneInfo
	{
		private readonly Version m_Version = new Version(1, 0, 4, 2);

		public Dictionary<int, ObjectInfo> dicObject;

		public int map;

		public ChangeAmount caMap = new ChangeAmount();

		public int sunLightType;

		public bool mapOption;

		public int aceNo;

		public float aceBlend;

		public bool enableAOE;

		public Color aoeColor;

		public float aoeRadius;

		public bool enableBloom;

		public float bloomIntensity;

		public float bloomBlur;

		public float bloomThreshold;

		public bool enableDepth;

		public float depthFocalSize;

		public float depthAperture;

		public bool enableVignette;

		public bool enableFog;

		public Color fogColor;

		public float fogHeight;

		public float fogStartDistance;

		public bool enableSunShafts;

		public Color sunThresholdColor;

		public Color sunColor;

		public int sunCaster;

		public bool enableShadow;

		public bool faceNormal;

		public bool faceShadow;

		public float lineColorG;

		public Color ambientShadow;

		public float lineWidthG;

		public int rampG;

		public float ambientShadowG;

		public CameraControl.CameraData cameraSaveData;

		public CameraControl.CameraData[] cameraData;

		public CameraLightCtrl.LightInfo charaLight = new CameraLightCtrl.LightInfo();

		public CameraLightCtrl.MapLightInfo mapLight = new CameraLightCtrl.MapLightInfo();

		public BGMCtrl bgmCtrl = new BGMCtrl();

		public ENVCtrl envCtrl = new ENVCtrl();

		public OutsideSoundCtrl outsideSoundCtrl = new OutsideSoundCtrl();

		public string background = string.Empty;

		public string frame = string.Empty;

		private HashSet<int> hashIndex;

		private int lightCount;

		public Version version
		{
			get
			{
				return m_Version;
			}
		}

		public Dictionary<int, ObjectInfo> dicImport { get; private set; }

		public Dictionary<int, int> dicChangeKey { get; private set; }

		public bool isLightCheck
		{
			get
			{
				return lightCount < 2;
			}
		}

		public bool isLightLimitOver
		{
			get
			{
				return lightCount > 2;
			}
		}

		public Version dataVersion { get; set; }

		public SceneInfo()
		{
			dicObject = new Dictionary<int, ObjectInfo>();
			cameraData = new CameraControl.CameraData[10];
			for (int i = 0; i < cameraData.Length; i++)
			{
				cameraData[i] = new CameraControl.CameraData();
			}
			hashIndex = new HashSet<int>();
			ChangeAmount changeAmount = caMap;
			changeAmount.onChangePos = (Action)Delegate.Combine(changeAmount.onChangePos, new Action(Singleton<MapCtrl>.Instance.Reflect));
			ChangeAmount changeAmount2 = caMap;
			changeAmount2.onChangeRot = (Action)Delegate.Combine(changeAmount2.onChangeRot, new Action(Singleton<MapCtrl>.Instance.Reflect));
			Init();
		}

		public void Init()
		{
			dicObject.Clear();
			map = -1;
			caMap.Reset();
			sunLightType = 0;
			mapOption = true;
			aceNo = 0;
			aceBlend = 0f;
			enableAOE = true;
			aoeColor = Utility.ConvertColor(180, 180, 180);
			aoeRadius = 0.1f;
			enableBloom = true;
			bloomIntensity = 0.4f;
			bloomThreshold = 0.6f;
			bloomBlur = 0.8f;
			enableDepth = false;
			depthFocalSize = 0.95f;
			depthAperture = 0.6f;
			enableVignette = true;
			enableFog = false;
			fogColor = Utility.ConvertColor(137, 193, 221);
			fogHeight = 1f;
			fogStartDistance = 0f;
			enableSunShafts = false;
			sunThresholdColor = Utility.ConvertColor(128, 128, 128);
			sunColor = Utility.ConvertColor(255, 255, 255);
			sunCaster = -1;
			enableShadow = true;
			faceNormal = false;
			faceShadow = false;
			lineColorG = Manager.Config.EtcData.lineDepth;
			ambientShadow = Utility.ConvertColor(128, 128, 128);
			lineWidthG = Manager.Config.EtcData.lineWidth;
			rampG = Manager.Config.EtcData.rampId;
			ambientShadowG = Manager.Config.EtcData.shadowDepth;
			cameraSaveData = null;
			cameraData = new CameraControl.CameraData[10];
			if (Singleton<Studio>.IsInstance())
			{
				for (int i = 0; i < 10; i++)
				{
					cameraData[i] = Singleton<Studio>.Instance.cameraCtrl.ExportResetData();
				}
			}
			charaLight.Init();
			mapLight.Init();
			bgmCtrl.play = false;
			bgmCtrl.repeat = BGMCtrl.Repeat.All;
			bgmCtrl.no = 0;
			envCtrl.play = false;
			envCtrl.repeat = BGMCtrl.Repeat.All;
			envCtrl.no = 0;
			outsideSoundCtrl.play = false;
			outsideSoundCtrl.repeat = BGMCtrl.Repeat.All;
			outsideSoundCtrl.fileName = string.Empty;
			background = string.Empty;
			frame = string.Empty;
			hashIndex.Clear();
			lightCount = 0;
			dataVersion = m_Version;
		}

		public int GetNewIndex()
		{
			for (int i = 0; MathfEx.RangeEqualOn(0, i, int.MaxValue); i++)
			{
				if (!hashIndex.Contains(i))
				{
					hashIndex.Add(i);
					return i;
				}
			}
			return -1;
		}

		public int CheckNewIndex()
		{
			for (int i = -1; MathfEx.RangeEqualOn(0, i, int.MaxValue); i++)
			{
				if (!hashIndex.Contains(i))
				{
					return i;
				}
			}
			return -1;
		}

		public bool SetNewIndex(int _index)
		{
			return hashIndex.Add(_index);
		}

		public bool DeleteIndex(int _index)
		{
			return hashIndex.Remove(_index);
		}

		public void AddLight()
		{
			lightCount++;
		}

		public void DeleteLight()
		{
			lightCount--;
		}

		public bool Save(string _path)
		{
			using (FileStream output = new FileStream(_path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					byte[] array = null;
					array = Singleton<Studio>.Instance.gameScreenShot.CreatePngScreen(320, 180);
					binaryWriter.Write(array);
					binaryWriter.Write(m_Version.ToString());
					Save(binaryWriter, dicObject);
					binaryWriter.Write(map);
					caMap.Save(binaryWriter);
					binaryWriter.Write(sunLightType);
					binaryWriter.Write(mapOption);
					binaryWriter.Write(aceNo);
					binaryWriter.Write(aceBlend);
					binaryWriter.Write(enableAOE);
					binaryWriter.Write(JsonUtility.ToJson(aoeColor));
					binaryWriter.Write(aoeRadius);
					binaryWriter.Write(enableBloom);
					binaryWriter.Write(bloomIntensity);
					binaryWriter.Write(bloomBlur);
					binaryWriter.Write(bloomThreshold);
					binaryWriter.Write(enableDepth);
					binaryWriter.Write(depthFocalSize);
					binaryWriter.Write(depthAperture);
					binaryWriter.Write(enableVignette);
					binaryWriter.Write(enableFog);
					binaryWriter.Write(JsonUtility.ToJson(fogColor));
					binaryWriter.Write(fogHeight);
					binaryWriter.Write(fogStartDistance);
					binaryWriter.Write(enableSunShafts);
					binaryWriter.Write(JsonUtility.ToJson(sunThresholdColor));
					binaryWriter.Write(JsonUtility.ToJson(sunColor));
					binaryWriter.Write(sunCaster);
					binaryWriter.Write(enableShadow);
					binaryWriter.Write(faceNormal);
					binaryWriter.Write(faceShadow);
					binaryWriter.Write(lineColorG);
					binaryWriter.Write(JsonUtility.ToJson(ambientShadow));
					binaryWriter.Write(lineWidthG);
					binaryWriter.Write(rampG);
					binaryWriter.Write(ambientShadowG);
					cameraSaveData.Save(binaryWriter);
					for (int i = 0; i < 10; i++)
					{
						cameraData[i].Save(binaryWriter);
					}
					charaLight.Save(binaryWriter, m_Version);
					mapLight.Save(binaryWriter, m_Version);
					bgmCtrl.Save(binaryWriter, m_Version);
					envCtrl.Save(binaryWriter, m_Version);
					outsideSoundCtrl.Save(binaryWriter, m_Version);
					binaryWriter.Write(background);
					binaryWriter.Write(frame);
					binaryWriter.Write("【KStudio】");
				}
			}
			return true;
		}

		public void Save(BinaryWriter _writer, Dictionary<int, ObjectInfo> _dicObject)
		{
			int count = _dicObject.Count;
			_writer.Write(count);
			foreach (KeyValuePair<int, ObjectInfo> item in _dicObject)
			{
				_writer.Write(item.Key);
				item.Value.Save(_writer, m_Version);
			}
		}

		public bool Load(string _path)
		{
			Version _dataVersion;
			return Load(_path, out _dataVersion);
		}

		public bool Load(string _path, out Version _dataVersion)
		{
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					PngFile.SkipPng(binaryReader);
					dataVersion = new Version(binaryReader.ReadString());
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						int num2 = binaryReader.ReadInt32();
						int num3 = binaryReader.ReadInt32();
						ObjectInfo objectInfo = null;
						switch (num3)
						{
						case 0:
							objectInfo = new OICharInfo(null, -1);
							break;
						case 1:
							objectInfo = new OIItemInfo(-1, -1, -1, -1);
							break;
						case 2:
							objectInfo = new OILightInfo(-1, -1);
							break;
						case 3:
							objectInfo = new OIFolderInfo(-1);
							break;
						case 4:
							objectInfo = new OIRouteInfo(-1);
							break;
						case 5:
							objectInfo = new OICameraInfo(-1);
							break;
						}
						objectInfo.Load(binaryReader, dataVersion, false);
						dicObject.Add(num2, objectInfo);
						hashIndex.Add(num2);
					}
					map = binaryReader.ReadInt32();
					caMap.Load(binaryReader);
					sunLightType = binaryReader.ReadInt32();
					mapOption = binaryReader.ReadBoolean();
					aceNo = binaryReader.ReadInt32();
					if (dataVersion.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						aceBlend = binaryReader.ReadSingle();
					}
					if (dataVersion.CompareTo(new Version(0, 0, 1)) <= 0)
					{
						binaryReader.ReadBoolean();
						binaryReader.ReadSingle();
						binaryReader.ReadString();
					}
					if (dataVersion.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						enableAOE = binaryReader.ReadBoolean();
						aoeColor = JsonUtility.FromJson<Color>(binaryReader.ReadString());
						aoeRadius = binaryReader.ReadSingle();
					}
					enableBloom = binaryReader.ReadBoolean();
					bloomIntensity = binaryReader.ReadSingle();
					bloomBlur = binaryReader.ReadSingle();
					if (dataVersion.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						bloomThreshold = binaryReader.ReadSingle();
					}
					if (dataVersion.CompareTo(new Version(0, 0, 1)) <= 0)
					{
						binaryReader.ReadBoolean();
					}
					enableDepth = binaryReader.ReadBoolean();
					depthFocalSize = binaryReader.ReadSingle();
					depthAperture = binaryReader.ReadSingle();
					enableVignette = binaryReader.ReadBoolean();
					if (dataVersion.CompareTo(new Version(0, 0, 1)) <= 0)
					{
						binaryReader.ReadSingle();
					}
					enableFog = binaryReader.ReadBoolean();
					if (dataVersion.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						fogColor = JsonUtility.FromJson<Color>(binaryReader.ReadString());
						fogHeight = binaryReader.ReadSingle();
						fogStartDistance = binaryReader.ReadSingle();
					}
					enableSunShafts = binaryReader.ReadBoolean();
					if (dataVersion.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						sunThresholdColor = JsonUtility.FromJson<Color>(binaryReader.ReadString());
						sunColor = JsonUtility.FromJson<Color>(binaryReader.ReadString());
					}
					if (dataVersion.CompareTo(new Version(0, 0, 4)) >= 0)
					{
						sunCaster = binaryReader.ReadInt32();
					}
					if (dataVersion.CompareTo(new Version(0, 0, 2)) >= 0)
					{
						enableShadow = binaryReader.ReadBoolean();
					}
					if (dataVersion.CompareTo(new Version(0, 0, 4)) >= 0)
					{
						faceNormal = binaryReader.ReadBoolean();
						faceShadow = binaryReader.ReadBoolean();
						lineColorG = binaryReader.ReadSingle();
						ambientShadow = JsonUtility.FromJson<Color>(binaryReader.ReadString());
					}
					if (dataVersion.CompareTo(new Version(0, 0, 5)) >= 0)
					{
						lineWidthG = binaryReader.ReadSingle();
						rampG = binaryReader.ReadInt32();
						ambientShadowG = binaryReader.ReadSingle();
					}
					if (cameraSaveData == null)
					{
						cameraSaveData = new CameraControl.CameraData();
					}
					cameraSaveData.Load(binaryReader);
					for (int j = 0; j < 10; j++)
					{
						CameraControl.CameraData cameraData = new CameraControl.CameraData();
						cameraData.Load(binaryReader);
						this.cameraData[j] = cameraData;
					}
					charaLight.Load(binaryReader, dataVersion);
					mapLight.Load(binaryReader, dataVersion);
					bgmCtrl.Load(binaryReader, dataVersion);
					envCtrl.Load(binaryReader, dataVersion);
					outsideSoundCtrl.Load(binaryReader, dataVersion);
					background = binaryReader.ReadString();
					frame = binaryReader.ReadString();
					_dataVersion = dataVersion;
				}
			}
			return true;
		}

		public bool Import(string _path)
		{
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					PngFile.SkipPng(binaryReader);
					Version version = new Version(binaryReader.ReadString());
					Import(binaryReader, version);
				}
			}
			return true;
		}

		public void Import(BinaryReader _reader, Version _version)
		{
			dicImport = new Dictionary<int, ObjectInfo>();
			dicChangeKey = new Dictionary<int, int>();
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int value = _reader.ReadInt32();
				int num2 = _reader.ReadInt32();
				ObjectInfo objectInfo = null;
				bool flag = false;
				switch (num2)
				{
				case 0:
					objectInfo = new OICharInfo(null, Studio.GetNewIndex());
					break;
				case 1:
					objectInfo = new OIItemInfo(-1, -1, -1, Studio.GetNewIndex());
					break;
				case 2:
					flag = !isLightCheck;
					objectInfo = new OILightInfo(-1, Studio.GetNewIndex());
					break;
				case 3:
					objectInfo = new OIFolderInfo(Studio.GetNewIndex());
					break;
				case 4:
					objectInfo = new OIRouteInfo(Studio.GetNewIndex());
					break;
				case 5:
					objectInfo = new OICameraInfo(Studio.GetNewIndex());
					break;
				}
				objectInfo.Load(_reader, _version, true);
				if (flag)
				{
					objectInfo.DeleteKey();
					continue;
				}
				dicObject.Add(objectInfo.dicKey, objectInfo);
				dicImport.Add(objectInfo.dicKey, objectInfo);
				dicChangeKey.Add(objectInfo.dicKey, value);
			}
		}
	}
}
