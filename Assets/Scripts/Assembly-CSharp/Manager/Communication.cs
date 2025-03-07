using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ActionGame;
using ActionGame.Communication;
using Illusion.Extensions;
using UnityEngine;

namespace Manager
{
	public class Communication : Singleton<Communication>
	{
		private class FileInfo
		{
			public string assetBundle = string.Empty;

			public string file = string.Empty;
		}

		public class ConditionsInfo
		{
			public string name = string.Empty;

			public RangeValue favor;

			public RangeValue lewdness;

			public RangeValue probability;

			public int reference;

			public ConditionsInfo(List<string> _list)
			{
				name = _list.SafeGet(2);
				favor = new RangeValue(_list.SafeGet(3));
				lewdness = new RangeValue(_list.SafeGet(4));
				probability = new RangeValue(_list.SafeGet(5));
				reference = int.Parse(_list.SafeGet(6));
			}
		}

		public class CorrectionInfo
		{
			public float[] preference;

			public float[] physical;

			public float[] intellect;

			public float[] hentai;

			public int[] club;

			public int[] clubSkill;

			public CorrectionInfo()
			{
				preference = new float[3];
				physical = new float[2];
				intellect = new float[2];
				hentai = new float[3];
				club = new int[2];
				clubSkill = new int[3];
			}
		}

		public class StaffBenefitsInfo
		{
			public int[] talk;

			public float[] favor;

			public float confession;

			public float together;

			public float[] h;

			public float desire;

			public float dislike;

			public float breakup;

			public float anotherConfession;

			public StaffBenefitsInfo()
			{
				talk = new int[3] { 1, 1, 1 };
				favor = new float[3];
				h = new float[3];
			}
		}

		public class RangeValue
		{
			public float min { get; protected set; }

			public float max { get; protected set; }

			public float minLimit { get; protected set; }

			public float maxLimit { get; protected set; }

			public bool through
			{
				get
				{
					return min == -1f && max == -1f;
				}
			}

			public RangeValue(float _min = 0f, float _max = 100f)
			{
			}

			public RangeValue(string _str, float _min = 0f, float _max = 100f)
			{
				Set(_str);
				minLimit = _min;
				maxLimit = _max;
			}

			public virtual float Get(float _value)
			{
				return (min == max) ? min : Mathf.Lerp(min, max, Mathf.Clamp01(Mathf.InverseLerp(minLimit, maxLimit, _value)));
			}

			public virtual void Set(string _str)
			{
				string[] array = _str.Split(',');
				min = float.Parse(array[0]);
				float result = 0f;
				max = ((!float.TryParse(array.SafeGet(1), out result)) ? min : result);
			}

			public bool Check(float _value)
			{
				return (min != max) ? MathfEx.RangeEqualOn(min, _value, max) : (_value >= min);
			}
		}

		public class MotionSpeed : RangeValue
		{
			public string[] state { get; private set; }

			public float value
			{
				get
				{
					return Random.Range(base.min, base.max);
				}
			}

			public MotionSpeed(string[] _state, string _min, string _max)
			{
				state = _state;
				base.min = float.Parse(_min);
				base.max = float.Parse(_max);
			}
		}

		public class ProbabilityValue : RangeValue
		{
			public ProbabilityValue(string _str)
				: base(_str)
			{
			}
		}

		private Dictionary<int, FileInfo> dicTalkInfo;

		private Dictionary<int, FileInfo> dicWeekendTalkInfo;

		private Dictionary<int, FileInfo> dicOptionItemsInfo;

		private Dictionary<int, List<List<string>>> dicBestPlace;

		private Dictionary<int, Dictionary<string, string[]>> dicPoseChara;

		private Dictionary<int, Dictionary<int, ConditionsInfo>> dicOccurrence;

		private Dictionary<int, int[]> dicPreference;

		private Dictionary<int, int[]> dicPreferenceCustom;

		public CorrectionInfo correctionInfo { get; private set; }

		public StaffBenefitsInfo staffBenefitsInfo { get; private set; }

		public Dictionary<int, List<MotionSpeed>> dicMotionSpeed { get; private set; }

		public Dictionary<int, Dictionary<int, ProbabilityValue>> dicMaleCondition { get; private set; }

		public bool isInit { get; private set; }

		public bool GetTalkFilePath(int _personality, out string _bundle, out string _file)
		{
			FileInfo value = null;
			Dictionary<int, FileInfo> dictionary = ((!(Singleton<Game>.Instance.actScene == null)) ? ((Singleton<Game>.Instance.actScene.Cycle.nowWeek != Cycle.Week.Saturday) ? dicTalkInfo : dicWeekendTalkInfo) : dicTalkInfo);
			if (dictionary.TryGetValue(_personality, out value))
			{
				_bundle = value.assetBundle;
				_file = value.file;
				return true;
			}
			_bundle = string.Empty;
			_file = string.Empty;
			return false;
		}

		public bool GetOptionItemsFilePath(int _personality, out string _bundle, out string _file)
		{
			FileInfo value = null;
			if (dicOptionItemsInfo.TryGetValue(_personality, out value))
			{
				_bundle = value.assetBundle;
				_file = value.file;
				return true;
			}
			_bundle = string.Empty;
			_file = string.Empty;
			return false;
		}

		public string GetBestPlace(int _personality, int _stage, int _kind)
		{
			List<List<string>> value = null;
			if (!dicBestPlace.TryGetValue(_personality, out value))
			{
				return string.Empty;
			}
			if (value.IsNullOrEmpty() || !MathfEx.RangeEqualOn(0, _stage, value.Count - 1))
			{
				return string.Empty;
			}
			if (value[_stage].IsNullOrEmpty() || !MathfEx.RangeEqualOn(0, _kind, value[_stage].Count - 1))
			{
				return string.Empty;
			}
			return value[_stage][_kind];
		}

		public void ChangeExpression(int _personality, string _name, ChaControl _chaCtrl)
		{
			Game.Expression expression = Singleton<Game>.Instance.GetExpression(_personality, _name);
			if (expression != null)
			{
				expression.Change(_chaCtrl);
			}
		}

		public string[] GetPoseInfo(int _personality, string _pose)
		{
			Dictionary<string, string[]> value = null;
			if (!dicPoseChara.TryGetValue(_personality, out value))
			{
				return null;
			}
			string[] value2 = null;
			if (!value.TryGetValue(_pose, out value2))
			{
				return null;
			}
			return value2;
		}

		public ConditionsInfo GetOccurrenceCondition(int _stage, int _conditions)
		{
			Dictionary<int, ConditionsInfo> value = null;
			if (!dicOccurrence.TryGetValue(_stage, out value))
			{
				return null;
			}
			ConditionsInfo value2 = null;
			if (!value.TryGetValue(_conditions, out value2))
			{
				return null;
			}
			return value2;
		}

		public ProbabilityValue GetCommunicationConndition(int _stage, int _conditions)
		{
			Dictionary<int, ProbabilityValue> value = null;
			if (!dicMaleCondition.TryGetValue(_stage, out value))
			{
				return null;
			}
			ProbabilityValue value2 = null;
			return (!value.TryGetValue(_conditions, out value2)) ? null : value2;
		}

		public bool ChangeParam(SaveData.Heroine _heroine, ChangeValueAbstractInfo _value)
		{
			SaveData.Player player = Singleton<Game>.Instance.Player;
			int[] array = new int[3] { _value.favor, _value.lewdness, _value.anger };
			int relation = _heroine.relation;
			float num = Mathf.Lerp(0f, correctionInfo.preference[0], Preference(_heroine));
			float[] source = new float[3]
			{
				Mathf.Lerp(0f, correctionInfo.intellect[0], Mathf.InverseLerp(0f, 100f, player.intellect)),
				Mathf.Lerp(0f, correctionInfo.physical[0], Mathf.InverseLerp(0f, 100f, player.physical)),
				Mathf.Lerp(0f, correctionInfo.hentai[0], Mathf.InverseLerp(0f, 100f, player.hentai))
			};
			float num2 = ((!_heroine.chaCtrl.GetAttribute(2)) ? 0f : (-0.5f)) + ((!_heroine.chaCtrl.GetAttribute(3)) ? 0f : 0.5f);
			float num3 = ((!_heroine.chaCtrl.GetAttribute(4)) ? 0f : 0.5f) + ((!_heroine.chaCtrl.GetAttribute(5)) ? 0f : 0.5f);
			num2 = ((array[0] >= 0) ? (num + source.Sum() + num2 + ((!_heroine.isStaff) ? 0f : staffBenefitsInfo.favor[relation])) : Mathf.Lerp(0f, correctionInfo.intellect[1], Mathf.InverseLerp(0f, 100f, player.intellect)));
			array[0] += (int)((float)Mathf.Abs(array[0]) * num2);
			array[1] += (int)((float)Mathf.Abs(array[1]) * (num + Mathf.Lerp(0f, correctionInfo.hentai[1], Mathf.InverseLerp(0f, 100f, player.hentai)) + num3));
			if (_heroine.isStaff && relation == 2 && array[2] > 0)
			{
				array[2] -= (int)((float)array[2] * staffBenefitsInfo.dislike);
			}
			_heroine.favor = Mathf.Clamp(_heroine.favor + array[0], 0, 100);
			_heroine.lewdness = Mathf.Clamp(_heroine.lewdness + array[1], 0, 100);
			_heroine.anger = Mathf.Clamp(_heroine.anger + array[2], 0, (!_heroine.talkEvent.Contains(0) && !_heroine.talkEvent.Contains(1)) ? 99 : 100);
			if (!_heroine.isAnger && _heroine.anger >= 100)
			{
				_heroine.isAnger = true;
				_heroine.isDate = false;
				Singleton<Game>.Instance.rankSaveData.angerCount++;
			}
			if (array[0] > 0)
			{
				Singleton<Game>.Instance.saveData.clubReport.comAdd = array[0] * ((!_heroine.isStaff) ? 2 : 4);
			}
			return array[0] > 0 || array[1] > 0;
		}

		public float Preference(SaveData.Heroine _heroine)
		{
			int[] array = PreferenceValue(_heroine);
			SaveData.Player player = Singleton<Game>.Instance.Player;
			int num = 0;
			num += Mathf.Abs(player.parameter.aggressive + 5 - (array[0] + 5));
			num += Mathf.Abs(player.parameter.diligence + 5 - (array[1] + 5));
			num += Mathf.Abs(player.parameter.kindness + 5 - (array[2] + 5));
			return Mathf.InverseLerp(30f, 0f, num);
		}

		public int[] PreferenceValue(SaveData.Heroine _heroine)
		{
			int[] array = dicPreference[_heroine.personality];
			foreach (KeyValuePair<int, int[]> item in dicPreferenceCustom.Where((KeyValuePair<int, int[]> v) => _heroine.chaCtrl.GetAttribute(v.Key)))
			{
				array[0] = Mathf.Clamp(array[0] + item.Value[0], -5, 5);
				array[1] = Mathf.Clamp(array[1] + item.Value[1], -5, 5);
				array[2] = Mathf.Clamp(array[2] + item.Value[2], -5, 5);
			}
			return array;
		}

		public void DecreaseTalkTime(SaveData.Heroine _heroine, int _value)
		{
			_heroine.talkTime = Mathf.Clamp(_heroine.talkTime - _value, 0, _heroine.talkTimeMax);
		}

		public void AddTalkTime()
		{
			int add = correctionInfo.clubSkill[2];
			Singleton<Game>.Instance.HeroineList.ForEach(delegate(SaveData.Heroine p)
			{
				p.talkTime += add;
				p.talkTimeMax += add;
			});
		}

		public Dictionary<string, float> GetMotionSpeed(int _personality)
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			List<MotionSpeed> value = null;
			if (!dicMotionSpeed.TryGetValue(_personality, out value))
			{
				return dictionary;
			}
			foreach (MotionSpeed item in value)
			{
				float value2 = item.value;
				string[] state = item.state;
				foreach (string key in state)
				{
					dictionary[key] = value2;
				}
			}
			return dictionary;
		}

		public bool CheckMoveMotion(string _state, int _personality)
		{
			return CheckMotion(_state, _personality, "Locomotion");
		}

		public bool CheckEscapeMotion(string _state, int _personality)
		{
			return CheckMotion(_state, _personality, "Escape");
		}

		public bool CheckMoveAngerMotion(string _state, int _personality)
		{
			return CheckMotion(_state, _personality, "Locomotion_Anger");
		}

		public bool CheckChangeMoveSpeed(string _state, int _personality)
		{
			return CheckMoveMotion(_state, _personality) | CheckEscapeMotion(_state, _personality) | CheckMoveAngerMotion(_state, _personality);
		}

		private IEnumerator LoadInfo()
		{
			isInit = false;
			List<string> list = (from s in CommonLib.GetAssetBundleNameListFromPath("communication/", true)
				where Regex.Match(Path.GetFileNameWithoutExtension(s), "info_(\\d*)").Success
				select s).ToList();
			list.Sort();
			foreach (string item in list)
			{
				LoadPathInfo(item);
				LoadPlaceInfo(item);
				LoadPose(item);
				LoadOccurrence(item);
				LoadPreference(item);
				LoadPreferenceCustom(item);
				LoadCorrection(item);
				LoadMotionSpeed(item);
				LoadCommunicationCondition(item);
				LoadStaffBenefitsInfo(item);
			}
			isInit = true;
			yield break;
		}

		private void LoadPathInfo(string _path)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, fileNameWithoutExtension))
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, fileNameWithoutExtension, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int key = int.Parse(item[0]);
				dicTalkInfo[key] = new FileInfo
				{
					assetBundle = item[1],
					file = item[2]
				};
				dicOptionItemsInfo[key] = new FileInfo
				{
					assetBundle = item[3],
					file = item[4]
				};
				dicWeekendTalkInfo[key] = new FileInfo
				{
					assetBundle = item.SafeGet(7),
					file = item.SafeGet(8)
				};
			}
		}

		private void LoadPlaceInfo(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("Place_{0:00}", match.Groups[1]);
			if (!AssetBundleCheck.IsFile(_path, text) || text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) == -1)
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				dicBestPlace[int.Parse(item[0])] = new List<List<string>>
				{
					item.Skip(1).Take(3).ToList(),
					item.Skip(4).Take(3).ToList(),
					item.Skip(7).Take(3).ToList()
				};
			}
		}

		private void LoadPose(string _path)
		{
			foreach (Match item in from s in AssetBundleCheck.GetAllAssetName(_path, false)
				select Regex.Match(s, "pose_(\\d*)") into r
				where r.Success
				select r)
			{
				int key = int.Parse(item.Groups[1].Value);
				if (!dicPoseChara.ContainsKey(key))
				{
					dicPoseChara.Add(key, new Dictionary<string, string[]>());
				}
				ExcelData asset = AssetBundleManager.LoadAsset(_path, item.Value, typeof(ExcelData)).GetAsset<ExcelData>();
				foreach (List<string> item2 in from p in asset.list.Skip(1)
					select p.list)
				{
					dicPoseChara[key][item2[0]] = item2.Skip(1).ToArray();
				}
			}
			AssetBundleManager.UnloadAssetBundle(_path, false);
		}

		private void LoadOccurrence(string _path)
		{
			string match = "OccurrenceCondition_(\\d*)";
			if (!AssetBundleCheck.IsSimulation)
			{
				match = match.ToLower();
			}
			foreach (Match item in from s in AssetBundleCheck.GetAllAssetName(_path, false)
				select Regex.Match(s, match) into r
				where r.Success
				select r)
			{
				ExcelData asset = AssetBundleManager.LoadAsset(_path, item.Value, typeof(ExcelData)).GetAsset<ExcelData>();
				foreach (List<string> item2 in from p in asset.list.Skip(1)
					select p.list)
				{
					int key = int.Parse(item2[0]);
					if (!dicOccurrence.ContainsKey(key))
					{
						dicOccurrence.Add(key, new Dictionary<int, ConditionsInfo>());
					}
					int key2 = int.Parse(item2[1]);
					dicOccurrence[key][key2] = new ConditionsInfo(item2);
				}
			}
			AssetBundleManager.UnloadAssetBundle(_path, false);
		}

		private void LoadPreference(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("preference_{0:00}", match.Groups[1]);
			if (!AssetBundleCheck.IsFile(_path, text) || text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) == -1)
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(1), out result))
				{
					dicPreference[result] = item.Skip(2).Take(3).Select(int.Parse)
						.ToArray();
				}
			}
		}

		private void LoadPreferenceCustom(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("custom_{0:00}", match.Groups[1]);
			if (!AssetBundleCheck.IsFile(_path, text) || text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) == -1)
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(1), out result))
				{
					dicPreferenceCustom[result] = item.Skip(2).Take(3).Select(int.Parse)
						.ToArray();
				}
			}
		}

		private void LoadCorrection(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("correction_{0:00}", match.Groups[1]);
			if (AssetBundleCheck.IsFile(_path, text) && text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) != -1)
			{
				ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
				if (!(excelData == null))
				{
					int num = 1;
					correctionInfo.preference[0] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.preference[1] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.preference[2] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.physical[0] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.physical[1] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.intellect[0] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.intellect[1] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.hentai[0] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.hentai[1] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					correctionInfo.hentai[2] = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
					correctionInfo.club[0] = int.Parse(excelData.Get(num++, 2));
					correctionInfo.club[1] = int.Parse(excelData.Get(num++, 2));
					correctionInfo.clubSkill[0] = int.Parse(excelData.Get(num++, 2));
					correctionInfo.clubSkill[1] = int.Parse(excelData.Get(num++, 2));
					correctionInfo.clubSkill[2] = int.Parse(excelData.Get(num++, 2));
				}
			}
		}

		private void LoadMotionSpeed(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("speed_{0:00}", match.Groups[1]);
			if (!AssetBundleCheck.IsFile(_path, text) || text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) == -1)
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			List<string> list = excelData.list[0].list.Where((string s) => !s.IsNullOrEmpty()).ToList();
			foreach (List<string> item in from p in excelData.list.Skip(2)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(1), out result))
				{
					List<MotionSpeed> list2 = new List<MotionSpeed>();
					for (int i = 0; i < list.Count; i++)
					{
						int num = 2 + i * 2;
						list2.Add(new MotionSpeed(list[i].Split('/'), item.SafeGet(num), item.SafeGet(num + 1)));
					}
					dicMotionSpeed[result] = list2;
				}
			}
		}

		private void LoadCommunicationCondition(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("CommunicationCondition_{0:00}", match.Groups[1]);
			if (!AssetBundleCheck.IsFile(_path, text) || text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) == -1)
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item.SafeGet(0), out result))
				{
					if (!dicMaleCondition.ContainsKey(result))
					{
						dicMaleCondition.Add(result, new Dictionary<int, ProbabilityValue>());
					}
					int result2 = -1;
					if (int.TryParse(item.SafeGet(1), out result2))
					{
						dicMaleCondition[result][result2] = new ProbabilityValue(item.SafeGet(3));
					}
				}
			}
		}

		private void LoadStaffBenefitsInfo(string _path)
		{
			Match match = Regex.Match(Path.GetFileNameWithoutExtension(_path), "_(\\d*)");
			if (!match.Success)
			{
				return;
			}
			string text = string.Format("benefits_{0:00}", match.Groups[1]);
			if (AssetBundleCheck.IsFile(_path, text) && text.Check(true, AssetBundleCheck.GetAllAssetName(_path, false)) != -1)
			{
				ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
				if (!(excelData == null))
				{
					int num = 1;
					staffBenefitsInfo.talk[0] = int.Parse(excelData.Get(num++, 2));
					staffBenefitsInfo.favor[0] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					staffBenefitsInfo.talk[1] = int.Parse(excelData.Get(num++, 2));
					staffBenefitsInfo.favor[1] = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					staffBenefitsInfo.confession = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
					staffBenefitsInfo.together = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
					staffBenefitsInfo.h[1] = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
					staffBenefitsInfo.talk[2] = int.Parse(excelData.Get(num++, 2));
					staffBenefitsInfo.desire = 1f + float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					staffBenefitsInfo.dislike = float.Parse(excelData.Get(num++, 2).Split('%')[0]) * 0.01f;
					staffBenefitsInfo.h[2] = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
					staffBenefitsInfo.breakup = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
					staffBenefitsInfo.anotherConfession = float.Parse(excelData.Get(num++, 2).Split('%')[0]);
				}
			}
		}

		private bool CheckMotion(string _state, int _personality, string _key)
		{
			List<MotionSpeed> value = null;
			if (!dicMotionSpeed.TryGetValue(_personality, out value))
			{
				return false;
			}
			return value.Find((MotionSpeed v) => v.state.Contains(_key)).state.Contains(_state);
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
				dicTalkInfo = new Dictionary<int, FileInfo>();
				dicWeekendTalkInfo = new Dictionary<int, FileInfo>();
				dicOptionItemsInfo = new Dictionary<int, FileInfo>();
				dicBestPlace = new Dictionary<int, List<List<string>>>();
				dicPoseChara = new Dictionary<int, Dictionary<string, string[]>>();
				dicOccurrence = new Dictionary<int, Dictionary<int, ConditionsInfo>>();
				dicPreference = new Dictionary<int, int[]>();
				dicPreferenceCustom = new Dictionary<int, int[]>();
				correctionInfo = new CorrectionInfo();
				dicMotionSpeed = new Dictionary<int, List<MotionSpeed>>();
				dicMaleCondition = new Dictionary<int, Dictionary<int, ProbabilityValue>>();
				staffBenefitsInfo = new StaffBenefitsInfo();
				StartCoroutine(LoadInfo());
			}
		}
	}
}
