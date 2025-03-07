using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Studio
{
	public class Info : Singleton<Info>
	{
		public class FileInfo
		{
			public string manifest = string.Empty;

			public string bundlePath = string.Empty;

			public string fileName = string.Empty;

			public bool check
			{
				get
				{
					return !bundlePath.IsNullOrEmpty() & !fileName.IsNullOrEmpty();
				}
			}

			public void Clear()
			{
				manifest = string.Empty;
				bundlePath = string.Empty;
				fileName = string.Empty;
			}
		}

		public class LoadCommonInfo : FileInfo
		{
			public string name = string.Empty;
		}

		public class GroupInfo
		{
			public string name = string.Empty;

			public Dictionary<int, string> dicCategory = new Dictionary<int, string>();
		}

		public class BoneInfo
		{
			public int no;

			public string bone = string.Empty;

			public string name = string.Empty;

			public int group = -1;

			public int level;
		}

		public class ItemLoadInfo : LoadCommonInfo
		{
			public string childRoot;

			public bool isAnime;

			public bool[] color;

			public bool[] pattren;

			public bool isScale;

			public bool isEmission;

			public bool isGlass;

			public List<string> bones;

			public bool isColor
			{
				get
				{
					return color.Any();
				}
			}

			public bool isPattren
			{
				get
				{
					return pattren.Any();
				}
			}

			public ItemLoadInfo(List<string> _lst)
			{
				name = _lst[3];
				manifest = _lst[4];
				bundlePath = _lst[5];
				fileName = _lst[6];
				childRoot = _lst[7];
				isAnime = bool.Parse(_lst[8]);
				color = new bool[3]
				{
					bool.Parse(_lst[9]),
					bool.Parse(_lst[11]),
					bool.Parse(_lst[13])
				};
				pattren = new bool[3]
				{
					bool.Parse(_lst[10]),
					bool.Parse(_lst[12]),
					bool.Parse(_lst[14])
				};
				isScale = bool.Parse(_lst[15]);
				bool.TryParse(_lst.SafeGet(16), out isEmission);
				bool.TryParse(_lst.SafeGet(17), out isGlass);
			}
		}

		public class AccessoryPointInfo
		{
			public int group { get; private set; }

			public string key { get; private set; }

			public string name { get; private set; }

			public AccessoryPointInfo(int _group, string _key, string _name)
			{
				group = _group;
				key = _key;
				name = _name;
			}
		}

		public class LightLoadInfo : LoadCommonInfo
		{
			public enum Target
			{
				All = 0,
				Chara = 1,
				Map = 2
			}

			public int no;

			public Target target;
		}

		public class ParentageInfo
		{
			public string parent = string.Empty;

			public string child = string.Empty;
		}

		public class OptionItemInfo : FileInfo
		{
			public FileInfo anmInfo;

			public ParentageInfo[] parentageInfo;
		}

		public class AnimeLoadInfo : LoadCommonInfo
		{
			public string clip = string.Empty;

			public List<OptionItemInfo> option;

			public static List<OptionItemInfo> LoadOption(List<string> _list, int _start)
			{
				List<OptionItemInfo> list = new List<OptionItemInfo>();
				int num = _start;
				while (true)
				{
					OptionItemInfo info = new OptionItemInfo();
					if (!_list.SafeProc(num++, delegate(string _s)
					{
						info.bundlePath = _s;
					}) || !_list.SafeProc(num++, delegate(string _s)
					{
						info.fileName = _s;
					}) || !_list.SafeProc(num++, delegate(string _s)
					{
						info.manifest = _s;
					}))
					{
						break;
					}
					info.anmInfo = new FileInfo();
					info.anmInfo.bundlePath = _list.SafeGet(num++);
					info.anmInfo.fileName = _list.SafeGet(num++);
					info.parentageInfo = AnalysisParentageInfo(_list.SafeGet(num++));
					list.Add(info);
				}
				return list;
			}

			private static ParentageInfo[] AnalysisParentageInfo(string _str)
			{
				if (_str.IsNullOrEmpty())
				{
					return null;
				}
				string[] array = _str.Split(',');
				List<ParentageInfo> list = new List<ParentageInfo>();
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('/');
					ParentageInfo parentageInfo = new ParentageInfo();
					parentageInfo.parent = array2[0];
					if (array2.Length > 1)
					{
						parentageInfo.child = array2[1];
					}
					list.Add(parentageInfo);
				}
				return list.ToArray();
			}
		}

		public class HAnimeLoadInfo : AnimeLoadInfo
		{
			public FileInfo overrideFile;

			public int breastLayer = -1;

			public bool[] dynamic = new bool[2] { true, true };

			public bool isMotion;

			public bool[] pv = new bool[4] { true, true, true, true };

			public FileInfo yureFile;

			public bool isBreastLayer
			{
				get
				{
					return breastLayer != -1;
				}
			}

			public HAnimeLoadInfo(List<string> _list)
			{
				name = _list.SafeGet(3);
				bundlePath = _list.SafeGet(4);
				fileName = _list.SafeGet(5);
				overrideFile = new FileInfo
				{
					bundlePath = _list.SafeGet(6),
					fileName = _list.SafeGet(7)
				};
				clip = _list.SafeGet(8);
				int.TryParse(_list.SafeGet(9), out breastLayer);
				dynamic = (from s in _list.Skip(10).Take(2)
					select !bool.Parse(s)).ToArray();
				isMotion = bool.Parse(_list.SafeGet(12));
				for (int i = 0; i < 4; i++)
				{
					bool.TryParse(_list.SafeGet(13 + i), out pv[i]);
				}
				yureFile = new FileInfo
				{
					bundlePath = _list.SafeGet(17),
					fileName = _list.SafeGet(18)
				};
			}
		}

		public class HandAnimeInfo : LoadCommonInfo
		{
			public string clip = string.Empty;
		}

		public class MapLoadInfo : LoadCommonInfo
		{
			public FileInfo vanish;

			public MapLoadInfo(List<string> _list)
			{
				name = _list[1];
				bundlePath = _list[2];
				fileName = _list[3];
				manifest = _list.SafeGet(4);
				vanish = new FileInfo();
				vanish.bundlePath = _list.SafeGet(5);
				vanish.fileName = _list.SafeGet(6);
			}
		}

		private class FileCheck
		{
			private Dictionary<string, bool> dicConfirmed;

			public FileCheck()
			{
				dicConfirmed = new Dictionary<string, bool>();
			}

			public bool Check(string _path)
			{
				if (_path.IsNullOrEmpty())
				{
					return false;
				}
				bool value = false;
				if (dicConfirmed.TryGetValue(_path, out value))
				{
					return value;
				}
				value = !AssetBundleCheck.IsSimulation && File.Exists(AssetBundleManager.BaseDownloadingURL + _path);
				dicConfirmed.Add(_path, value);
				return value;
			}
		}

		public class WaitTime
		{
			private const float intervalTime = 0.03f;

			private float nextFrameTime;

			public bool isOver
			{
				get
				{
					return Time.realtimeSinceStartup >= nextFrameTime;
				}
			}

			public WaitTime()
			{
				Next();
			}

			public void Next()
			{
				nextFrameTime = Time.realtimeSinceStartup + 0.03f;
			}
		}

		private class FileListInfo
		{
			public Dictionary<string, string[]> dicFile;

			public FileListInfo(List<string> _list)
			{
				dicFile = new Dictionary<string, string[]>();
				foreach (string item in _list)
				{
					dicFile.Add(item, AssetBundleCheck.GetAllFileName(item));
				}
			}

			public bool Check(string _path, string _file)
			{
				string[] value = null;
				if (!AssetBundleCheck.IsSimulation)
				{
					_file = _file.ToLower();
				}
				return dicFile.TryGetValue(_path, out value) && value.Contains(_file);
			}
		}

		private delegate void LoadAnimeInfoCoroutineFunc(ExcelData _ed, Dictionary<int, Dictionary<int, Dictionary<int, AnimeLoadInfo>>> _dic);

		public Dictionary<int, BoneInfo> dicBoneInfo = new Dictionary<int, BoneInfo>();

		public Dictionary<int, GroupInfo> dicItemGroupCategory = new Dictionary<int, GroupInfo>();

		public Dictionary<int, Dictionary<int, Dictionary<int, ItemLoadInfo>>> dicItemLoadInfo = new Dictionary<int, Dictionary<int, Dictionary<int, ItemLoadInfo>>>();

		public Dictionary<int, string> dicAccessoryGroup = new Dictionary<int, string>();

		public Dictionary<int, AccessoryPointInfo> dicAccessoryPointInfo = new Dictionary<int, AccessoryPointInfo>();

		private ExcelData m_AccessoryPoint;

		private int _accessoryPointNum = -1;

		private ExcelData m_AccessoryPointGroup;

		public Dictionary<int, LightLoadInfo> dicLightLoadInfo = new Dictionary<int, LightLoadInfo>();

		public Dictionary<int, GroupInfo> dicAGroupCategory = new Dictionary<int, GroupInfo>();

		public Dictionary<int, Dictionary<int, Dictionary<int, AnimeLoadInfo>>> dicAnimeLoadInfo = new Dictionary<int, Dictionary<int, Dictionary<int, AnimeLoadInfo>>>();

		public Dictionary<int, HandAnimeInfo>[] dicHandAnime = new Dictionary<int, HandAnimeInfo>[2]
		{
			new Dictionary<int, HandAnimeInfo>(),
			new Dictionary<int, HandAnimeInfo>()
		};

		public Dictionary<int, GroupInfo> dicVoiceGroupCategory = new Dictionary<int, GroupInfo>();

		public Dictionary<int, Dictionary<int, Dictionary<int, LoadCommonInfo>>> dicVoiceLoadInfo = new Dictionary<int, Dictionary<int, Dictionary<int, LoadCommonInfo>>>();

		public Dictionary<int, LoadCommonInfo> dicBGMLoadInfo = new Dictionary<int, LoadCommonInfo>();

		public Dictionary<int, LoadCommonInfo> dicENVLoadInfo = new Dictionary<int, LoadCommonInfo>();

		public Dictionary<int, MapLoadInfo> dicMapLoadInfo = new Dictionary<int, MapLoadInfo>();

		public Dictionary<int, LoadCommonInfo> dicFilterLoadInfo = new Dictionary<int, LoadCommonInfo>();

		private FileCheck fileCheck;

		private WaitTime waitTime;

		public ExcelData accessoryPoint
		{
			get
			{
				return m_AccessoryPoint;
			}
		}

		public int accessoryPointNum
		{
			get
			{
				if (_accessoryPointNum < 0)
				{
					int work = -1;
					_accessoryPointNum = accessoryPoint.list.Skip(1).Count((ExcelData.Param p) => int.TryParse(p.list.SafeGet(0), out work));
				}
				return _accessoryPointNum;
			}
		}

		public ExcelData accessoryPointGroup
		{
			get
			{
				return m_AccessoryPointGroup;
			}
		}

		public bool isLoadList { get; private set; }

		public IEnumerator LoadExcelDataCoroutine()
		{
			if (isLoadList)
			{
				yield break;
			}
			fileCheck = new FileCheck();
			waitTime = new WaitTime();
			dicBoneInfo.Clear();
			dicItemGroupCategory.Clear();
			dicItemLoadInfo.Clear();
			dicLightLoadInfo.Clear();
			dicAGroupCategory.Clear();
			dicAnimeLoadInfo.Clear();
			dicHandAnime[0].Clear();
			dicHandAnime[1].Clear();
			dicVoiceGroupCategory.Clear();
			dicVoiceLoadInfo.Clear();
			dicFilterLoadInfo.Clear();
			if (waitTime.isOver)
			{
				yield return null;
				waitTime.Next();
			}
			List<string> pathList = CommonLib.GetAssetBundleNameListFromPath("studio/info/", true);
			pathList.Sort();
			if (waitTime.isOver)
			{
				yield return null;
				waitTime.Next();
			}
			FileListInfo fli = new FileListInfo(pathList);
			for (int i = 0; i < pathList.Count; i++)
			{
				string bundlePath = pathList[i];
				string fileName = Path.GetFileNameWithoutExtension(bundlePath);
				if (fli.Check(bundlePath, "AccessoryPointGroup_" + fileName))
				{
					LoadAccessoryGroupInfo(LoadExcelData(bundlePath, "AccessoryPointGroup_" + fileName));
				}
				if (fli.Check(bundlePath, "AccessoryPoint_" + fileName))
				{
					LoadAccessoryPointInfo(LoadExcelData(bundlePath, "AccessoryPoint_" + fileName));
				}
				if (fli.Check(bundlePath, "Bone_" + fileName))
				{
					LoadBoneInfo(LoadExcelData(bundlePath, "Bone_" + fileName), dicBoneInfo);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadHandAnimeInfo(bundlePath, "HandAnime_(\\d*)_(\\d*)", dicHandAnime);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				if (fli.Check(bundlePath, "ItemGroup_" + fileName))
				{
					LoadAnimeGroupInfo(LoadExcelData(bundlePath, "ItemGroup_" + fileName), dicItemGroupCategory);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadAnimeCategoryInfo(bundlePath, "ItemCategory_(\\d*)_(\\d*)", dicItemGroupCategory);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				yield return LoadItemLoadInfoCoroutine(bundlePath, "ItemList_(\\d*)_(\\d*)_(\\d*)");
				LoadItemBoneInfo(bundlePath, "ItemBoneList_(\\d*)_(\\d*)");
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				if (fli.Check(bundlePath, "Light_" + fileName))
				{
					LoadLightLoadInfo(LoadExcelData(bundlePath, "Light_" + fileName));
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				if (fli.Check(bundlePath, "AnimeGroup_" + fileName))
				{
					LoadAnimeGroupInfo(LoadExcelData(bundlePath, "AnimeGroup_" + fileName), dicAGroupCategory);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadAnimeCategoryInfo(bundlePath, "AnimeCategory_(\\d*)_(\\d*)", dicAGroupCategory);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				yield return LoadAnimeLoadInfoCoroutine(bundlePath, "Anime_(\\d*)_(\\d*)_(\\d*)", dicAnimeLoadInfo, LoadAnimeLoadInfo);
				yield return LoadAnimeLoadInfoCoroutine(bundlePath, "HAnime_(\\d*)_(\\d*)_(\\d*)", dicAnimeLoadInfo, LoadHAnimeLoadInfo);
				if (fli.Check(bundlePath, "VoiceGroup_" + fileName))
				{
					LoadAnimeGroupInfo(LoadExcelData(bundlePath, "VoiceGroup_" + fileName), dicVoiceGroupCategory);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadAnimeCategoryInfo(bundlePath, "VoiceCategory_(\\d*)_(\\d*)", dicVoiceGroupCategory);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				yield return LoadVoiceLoadInfoCoroutine(bundlePath, "Voice_(\\d*)_(\\d*)_(\\d*)");
				if (fli.Check(bundlePath, "BGM_" + fileName))
				{
					LoadSoundLoadInfo(LoadExcelData(bundlePath, "BGM_" + fileName), dicBGMLoadInfo);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				if (fli.Check(bundlePath, "Map_" + fileName))
				{
					LoadMapLoadInfo(LoadExcelData(bundlePath, "Map_" + fileName), dicMapLoadInfo);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				if (fli.Check(bundlePath, "Filter_" + fileName))
				{
					LoadSoundLoadInfo(LoadExcelData(bundlePath, "Filter_" + fileName), dicFilterLoadInfo);
				}
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				AssetBundleManager.UnloadAssetBundle(bundlePath, true);
			}
			fileCheck = null;
			waitTime = null;
			isLoadList = true;
		}

		public LoadCommonInfo GetVoiceInfo(int _group, int _category, int _no)
		{
			Dictionary<int, Dictionary<int, LoadCommonInfo>> value = null;
			if (!dicVoiceLoadInfo.TryGetValue(_group, out value))
			{
				return null;
			}
			Dictionary<int, LoadCommonInfo> value2 = null;
			if (!value.TryGetValue(_category, out value2))
			{
				return null;
			}
			LoadCommonInfo value3 = null;
			return (!value2.TryGetValue(_no, out value3)) ? null : value3;
		}

		private ExcelData LoadExcelData(string _bundlePath, string _fileName)
		{
			string text = string.Empty;
			if (AssetBundleCheck.IsSimulation)
			{
				if (!AssetBundleCheck.FindFile(_bundlePath, _fileName))
				{
					return null;
				}
			}
			else
			{
				bool flag = false;
				foreach (KeyValuePair<string, AssetBundleManager.BundlePack> item in AssetBundleManager.ManifestBundlePack.Where((KeyValuePair<string, AssetBundleManager.BundlePack> v) => Regex.Match(v.Key, "studio(\\d*)").Success))
				{
					flag |= item.Value.AssetBundleManifest.GetAllAssetBundles().ToList().FindIndex((string s) => s == _bundlePath) != -1;
					if (flag)
					{
						text = item.Key;
						break;
					}
				}
				if (!flag)
				{
					return null;
				}
			}
			string assetBundleName = _bundlePath;
			string manifestName = text;
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(assetBundleName, _fileName, false, manifestName);
			if (null == excelData)
			{
				return null;
			}
			return excelData;
		}

		private string[] FindAllAssetName(string _bundlePath, string _regex)
		{
			string[] result = null;
			if (AssetBundleCheck.IsSimulation)
			{
				result = AssetBundleCheck.FindAllAssetName(_bundlePath, _regex, false, RegexOptions.IgnoreCase);
			}
			else
			{
				foreach (KeyValuePair<string, AssetBundleManager.BundlePack> item in AssetBundleManager.ManifestBundlePack.Where((KeyValuePair<string, AssetBundleManager.BundlePack> v) => Regex.Match(v.Key, "studio(\\d*)").Success))
				{
					if (item.Value.AssetBundleManifest.GetAllAssetBundles().ToList().FindIndex((string s) => s == _bundlePath) == -1)
					{
						continue;
					}
					LoadedAssetBundle value = null;
					if (!item.Value.LoadedAssetBundles.TryGetValue(_bundlePath, out value))
					{
						value = AssetBundleManager.LoadAssetBundle(_bundlePath, false, item.Key);
						if (value == null)
						{
							break;
						}
					}
					result = (from s in value.m_AssetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension)
						where Regex.Match(s, _regex, RegexOptions.IgnoreCase).Success
						select s).ToArray();
					value = null;
					break;
				}
			}
			return result;
		}

		private void LoadAccessoryGroupInfo(ExcelData _ed)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					string text = item.SafeGet(1);
					if (!text.IsNullOrEmpty())
					{
						dicAccessoryGroup[result] = text;
					}
				}
			}
			m_AccessoryPointGroup = _ed;
		}

		private void LoadAccessoryPointInfo(ExcelData _ed)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					int result2 = -1;
					if (int.TryParse(item.SafeGet(1), out result2))
					{
						dicAccessoryPointInfo[result] = new AccessoryPointInfo(result2, item.SafeGet(2), item.SafeGet(3));
					}
				}
			}
			m_AccessoryPoint = _ed;
		}

		private void LoadBoneInfo(ExcelData _ed, Dictionary<int, BoneInfo> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int num = 0;
				int result = -1;
				if (int.TryParse(item[num++], out result))
				{
					string text = item[num++];
					if (!text.IsNullOrEmpty())
					{
						_dic[result] = new BoneInfo
						{
							no = result,
							bone = text,
							name = item[num++],
							group = int.Parse(item[num++]),
							level = int.Parse(item[num++])
						};
					}
				}
			}
		}

		private IEnumerator LoadItemLoadInfoCoroutine(string _bundlePath, string _regex)
		{
			string[] files = FindAllAssetName(_bundlePath, _regex);
			if (files.IsNullOrEmpty())
			{
				yield break;
			}
			string checkKey = _regex.Split('_')[0].ToLower();
			SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, string>>> sortDic = new SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, string>>>();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Split('_')[0].ToLower().CompareTo(checkKey) == 0)
				{
					Match match = Regex.Match(files[i], _regex, RegexOptions.IgnoreCase);
					int key = int.Parse(match.Groups[1].Value);
					int key2 = int.Parse(match.Groups[2].Value);
					int key3 = int.Parse(match.Groups[3].Value);
					if (!sortDic.ContainsKey(key2))
					{
						sortDic.Add(key2, new SortedDictionary<int, SortedDictionary<int, string>>());
					}
					if (!sortDic[key2].ContainsKey(key3))
					{
						sortDic[key2].Add(key3, new SortedDictionary<int, string>());
					}
					sortDic[key2][key3].Add(key, files[i]);
				}
			}
			foreach (KeyValuePair<int, SortedDictionary<int, SortedDictionary<int, string>>> item in sortDic)
			{
				foreach (KeyValuePair<int, SortedDictionary<int, string>> item2 in item.Value)
				{
					foreach (KeyValuePair<int, string> item3 in item2.Value)
					{
						LoadItemLoadInfo(LoadExcelData(_bundlePath, item3.Value));
						if (waitTime.isOver)
						{
							yield return null;
							waitTime.Next();
						}
					}
				}
			}
		}

		private void SortDictionary(string[] files, string _regex, SortedDictionary<int, SortedDictionary<int, string>> _sortDic)
		{
			string strB = _regex.Split('_')[0].ToLower();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Split('_')[0].ToLower().CompareTo(strB) == 0)
				{
					Match match = Regex.Match(files[i], _regex, RegexOptions.IgnoreCase);
					int key = int.Parse(match.Groups[1].Value);
					int key2 = int.Parse(match.Groups[2].Value);
					if (!_sortDic.ContainsKey(key))
					{
						_sortDic.Add(key, new SortedDictionary<int, string>());
					}
					_sortDic[key].Add(key2, files[i]);
				}
			}
		}

		private void LoadItemLoadInfo(ExcelData _ed)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int result = -1;
				if (!int.TryParse(item.SafeGet(0), out result))
				{
					break;
				}
				int key = int.Parse(item[1]);
				int key2 = int.Parse(item[2]);
				if (!dicItemLoadInfo.ContainsKey(key))
				{
					dicItemLoadInfo[key] = new Dictionary<int, Dictionary<int, ItemLoadInfo>>();
				}
				if (!dicItemLoadInfo[key].ContainsKey(key2))
				{
					dicItemLoadInfo[key][key2] = new Dictionary<int, ItemLoadInfo>();
				}
				dicItemLoadInfo[key][key2][result] = new ItemLoadInfo(item);
			}
		}

		private void LoadItemBoneInfo(string _bundlePath, string _regex)
		{
			string[] array = FindAllAssetName(_bundlePath, _regex);
			if (array.IsNullOrEmpty())
			{
				return;
			}
			SortedDictionary<int, SortedDictionary<int, string>> sortedDictionary = new SortedDictionary<int, SortedDictionary<int, string>>();
			SortDictionary(array, _regex, sortedDictionary);
			foreach (KeyValuePair<int, SortedDictionary<int, string>> item in sortedDictionary)
			{
				foreach (KeyValuePair<int, string> item2 in item.Value)
				{
					LoadItemBoneInfo(LoadExcelData(_bundlePath, item2.Value), item.Key, item2.Key);
				}
			}
		}

		private void LoadItemBoneInfo(ExcelData _ed, int _group, int _category)
		{
			if (_ed == null)
			{
				return;
			}
			Dictionary<int, Dictionary<int, ItemLoadInfo>> value = null;
			if (!dicItemLoadInfo.TryGetValue(_group, out value))
			{
				return;
			}
			Dictionary<int, ItemLoadInfo> value2 = null;
			if (!value.TryGetValue(_category, out value2))
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				ItemLoadInfo value3 = null;
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result) && value2.TryGetValue(result, out value3))
				{
					value3.bones = (from s in item.Skip(1)
						where !s.IsNullOrEmpty()
						select s).ToList();
				}
			}
		}

		private void LoadLightLoadInfo(ExcelData _ed)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int num = 0;
				LightLoadInfo lightLoadInfo = new LightLoadInfo();
				lightLoadInfo.no = int.Parse(item[num++]);
				lightLoadInfo.name = item[num++];
				lightLoadInfo.manifest = item[num++];
				lightLoadInfo.bundlePath = item[num++];
				lightLoadInfo.fileName = item[num++];
				lightLoadInfo.target = (LightLoadInfo.Target)int.Parse(item[num++]);
				dicLightLoadInfo[lightLoadInfo.no] = lightLoadInfo;
			}
		}

		private void LoadAnimeGroupInfo(ExcelData _ed, Dictionary<int, GroupInfo> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					string text = item[1];
					GroupInfo value = null;
					if (_dic.TryGetValue(result, out value))
					{
						value.name = text;
						continue;
					}
					value = new GroupInfo();
					value.name = text;
					_dic.Add(result, value);
				}
			}
		}

		private void LoadAnimeCategoryInfo(string _bundlePath, string _regex, Dictionary<int, GroupInfo> _dic)
		{
			string[] array = FindAllAssetName(_bundlePath, _regex);
			if (array.IsNullOrEmpty())
			{
				return;
			}
			string strB = _regex.Split('_')[0].ToLower();
			SortedDictionary<int, SortedDictionary<int, string>> sortedDictionary = new SortedDictionary<int, SortedDictionary<int, string>>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Split('_')[0].ToLower().CompareTo(strB) == 0)
				{
					Match match = Regex.Match(array[i], _regex, RegexOptions.IgnoreCase);
					int key = int.Parse(match.Groups[1].Value);
					int key2 = int.Parse(match.Groups[2].Value);
					if (!sortedDictionary.ContainsKey(key2))
					{
						sortedDictionary.Add(key2, new SortedDictionary<int, string>());
					}
					sortedDictionary[key2].Add(key, array[i]);
				}
			}
			foreach (KeyValuePair<int, SortedDictionary<int, string>> item in sortedDictionary)
			{
				if (!_dic.ContainsKey(item.Key))
				{
					continue;
				}
				foreach (KeyValuePair<int, string> item2 in item.Value)
				{
					LoadAnimeCategoryInfo(LoadExcelData(_bundlePath, item2.Value), _dic[item.Key]);
				}
			}
		}

		private void LoadAnimeCategoryInfo(ExcelData _ed, GroupInfo _info)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					_info.dicCategory[result] = item[1];
				}
			}
		}

		private IEnumerator LoadAnimeLoadInfoCoroutine(string _bundlePath, string _regex, Dictionary<int, Dictionary<int, Dictionary<int, AnimeLoadInfo>>> _dic, LoadAnimeInfoCoroutineFunc _func)
		{
			string[] files = FindAllAssetName(_bundlePath, _regex);
			if (files.IsNullOrEmpty())
			{
				yield break;
			}
			string checkKey = _regex.Split('_')[0].ToLower();
			SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, string>>> sortDic = new SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, string>>>();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Split('_')[0].ToLower().CompareTo(checkKey) == 0)
				{
					Match match = Regex.Match(files[i], _regex, RegexOptions.IgnoreCase);
					int key = int.Parse(match.Groups[1].Value);
					int key2 = int.Parse(match.Groups[2].Value);
					int key3 = int.Parse(match.Groups[3].Value);
					if (!sortDic.ContainsKey(key2))
					{
						sortDic.Add(key2, new SortedDictionary<int, SortedDictionary<int, string>>());
					}
					if (!sortDic[key2].ContainsKey(key3))
					{
						sortDic[key2].Add(key3, new SortedDictionary<int, string>());
					}
					sortDic[key2][key3].Add(key, files[i]);
				}
			}
			foreach (KeyValuePair<int, SortedDictionary<int, SortedDictionary<int, string>>> item in sortDic)
			{
				foreach (KeyValuePair<int, SortedDictionary<int, string>> item2 in item.Value)
				{
					foreach (KeyValuePair<int, string> item3 in item2.Value)
					{
						_func(LoadExcelData(_bundlePath, item3.Value), _dic);
						if (waitTime.isOver)
						{
							yield return null;
							waitTime.Next();
						}
					}
				}
			}
		}

		private void LoadAnimeLoadInfo(ExcelData _ed, Dictionary<int, Dictionary<int, Dictionary<int, AnimeLoadInfo>>> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(2)
				select v.list)
			{
				if (!fileCheck.Check(item.SafeGet(4)))
				{
					continue;
				}
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					int key = int.Parse(item.SafeGet(1));
					int key2 = int.Parse(item.SafeGet(2));
					if (!_dic.ContainsKey(key))
					{
						_dic.Add(key, new Dictionary<int, Dictionary<int, AnimeLoadInfo>>());
					}
					if (!_dic[key].ContainsKey(key2))
					{
						_dic[key].Add(key2, new Dictionary<int, AnimeLoadInfo>());
					}
					_dic[key][key2][result] = new AnimeLoadInfo
					{
						name = item.SafeGet(3),
						bundlePath = item.SafeGet(4),
						fileName = item.SafeGet(5),
						clip = item.SafeGet(6),
						option = AnimeLoadInfo.LoadOption(item, 7)
					};
				}
			}
		}

		private void LoadHAnimeLoadInfo(ExcelData _ed, Dictionary<int, Dictionary<int, Dictionary<int, AnimeLoadInfo>>> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(2)
				select v.list)
			{
				if (!fileCheck.Check(item.SafeGet(4)))
				{
					continue;
				}
				int result = 0;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					int key = int.Parse(item.SafeGet(1));
					int key2 = int.Parse(item.SafeGet(2));
					if (!_dic.ContainsKey(key))
					{
						_dic.Add(key, new Dictionary<int, Dictionary<int, AnimeLoadInfo>>());
					}
					if (!_dic[key].ContainsKey(key2))
					{
						_dic[key].Add(key2, new Dictionary<int, AnimeLoadInfo>());
					}
					_dic[key][key2][result] = new HAnimeLoadInfo(item);
				}
			}
		}

		private void LoadHandAnimeInfo(string _bundlePath, string _regex, Dictionary<int, HandAnimeInfo>[] _dic)
		{
			string[] array = FindAllAssetName(_bundlePath, _regex);
			if (array.IsNullOrEmpty())
			{
				return;
			}
			string strB = _regex.Split('_')[0].ToLower();
			SortedDictionary<int, SortedDictionary<int, string>> sortedDictionary = new SortedDictionary<int, SortedDictionary<int, string>>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Split('_')[0].ToLower().CompareTo(strB) == 0)
				{
					Match match = Regex.Match(array[i], _regex, RegexOptions.IgnoreCase);
					int key = int.Parse(match.Groups[1].Value);
					int key2 = int.Parse(match.Groups[2].Value);
					if (!sortedDictionary.ContainsKey(key))
					{
						sortedDictionary.Add(key, new SortedDictionary<int, string>());
					}
					sortedDictionary[key].Add(key2, array[i]);
				}
			}
			foreach (KeyValuePair<int, SortedDictionary<int, string>> item in sortedDictionary)
			{
				foreach (KeyValuePair<int, string> item2 in item.Value)
				{
					LoadHandAnimeInfo(LoadExcelData(_bundlePath, item2.Value), _dic[item.Key]);
				}
			}
		}

		private void LoadHandAnimeInfo(ExcelData _ed, Dictionary<int, HandAnimeInfo> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					_dic[result] = new HandAnimeInfo
					{
						name = item.SafeGet(1),
						bundlePath = item.SafeGet(2),
						fileName = item.SafeGet(3),
						clip = item.SafeGet(4)
					};
				}
			}
		}

		private IEnumerator LoadVoiceLoadInfoCoroutine(string _bundlePath, string _regex)
		{
			string[] files = FindAllAssetName(_bundlePath, _regex);
			if (files.IsNullOrEmpty())
			{
				yield break;
			}
			string checkKey = _regex.Split('_')[0].ToLower();
			SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, string>>> sortDic = new SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, string>>>();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Split('_')[0].ToLower().CompareTo(checkKey) == 0)
				{
					Match match = Regex.Match(files[i], _regex, RegexOptions.IgnoreCase);
					int key = int.Parse(match.Groups[1].Value);
					int key2 = int.Parse(match.Groups[2].Value);
					int key3 = int.Parse(match.Groups[3].Value);
					if (!sortDic.ContainsKey(key))
					{
						sortDic.Add(key, new SortedDictionary<int, SortedDictionary<int, string>>());
					}
					if (!sortDic[key].ContainsKey(key2))
					{
						sortDic[key].Add(key2, new SortedDictionary<int, string>());
					}
					sortDic[key][key2].Add(key3, files[i]);
				}
			}
			foreach (KeyValuePair<int, SortedDictionary<int, SortedDictionary<int, string>>> item in sortDic)
			{
				foreach (KeyValuePair<int, SortedDictionary<int, string>> item2 in item.Value)
				{
					foreach (KeyValuePair<int, string> item3 in item2.Value)
					{
						LoadVoiceLoadInfo(LoadExcelData(_bundlePath, item3.Value));
						if (waitTime.isOver)
						{
							yield return null;
							waitTime.Next();
						}
					}
				}
			}
		}

		private void LoadVoiceLoadInfo(ExcelData _ed)
		{
			foreach (List<string> item in from v in _ed.list.Skip(2)
				select v.list)
			{
				LoadVoiceLoadInfo(item);
			}
		}

		private LoadCommonInfo LoadVoiceLoadInfo(List<string> _list)
		{
			int num = 0;
			LoadCommonInfo loadCommonInfo = new LoadCommonInfo();
			int result = -1;
			if (!int.TryParse(_list[num++], out result))
			{
				return null;
			}
			int result2 = -1;
			if (!int.TryParse(_list[num++], out result2))
			{
				return null;
			}
			int result3 = -1;
			if (!int.TryParse(_list[num++], out result3))
			{
				return null;
			}
			loadCommonInfo.name = _list[num++];
			loadCommonInfo.bundlePath = _list[num++];
			loadCommonInfo.fileName = _list[num++];
			if (!dicVoiceLoadInfo.ContainsKey(result2))
			{
				dicVoiceLoadInfo.Add(result2, new Dictionary<int, Dictionary<int, LoadCommonInfo>>());
			}
			if (!dicVoiceLoadInfo[result2].ContainsKey(result3))
			{
				dicVoiceLoadInfo[result2].Add(result3, new Dictionary<int, LoadCommonInfo>());
			}
			dicVoiceLoadInfo[result2][result3][result] = loadCommonInfo;
			return loadCommonInfo;
		}

		private void LoadSoundLoadInfo(ExcelData _ed, Dictionary<int, LoadCommonInfo> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(1)
				select v.list)
			{
				int num = 0;
				int result = -1;
				if (int.TryParse(item[num++], out result))
				{
					_dic[result] = new LoadCommonInfo
					{
						name = item[num++],
						bundlePath = item[num++],
						fileName = item[num++]
					};
				}
			}
		}

		private void LoadMapLoadInfo(ExcelData _ed, Dictionary<int, MapLoadInfo> _dic)
		{
			if (_ed == null)
			{
				return;
			}
			foreach (List<string> item in from v in _ed.list.Skip(2)
				select v.list)
			{
				int num = 0;
				int result = -1;
				if (int.TryParse(item[num++], out result))
				{
					_dic[result] = new MapLoadInfo(item);
				}
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
				isLoadList = false;
			}
		}
	}
}
