using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ActionGame.Chara;
using ActionGame.Chara.Mover;
using Illusion;
using Illusion.Extensions;
using Manager;
using UniRx;
using UnityEngine;

namespace ActionGame
{
	public class ActionControl
	{
		private class DesireSaveData
		{
			public int schoolClass { get; private set; }

			public int schoolClassIndex { get; private set; }

			public Dictionary<int, int> desire { get; private set; }

			public DesireSaveData(int _schoolClass, int _schoolClassIndex, Dictionary<int, int> _desire)
			{
				schoolClass = _schoolClass;
				schoolClassIndex = _schoolClassIndex;
				desire = _desire;
			}

			public bool Check(SaveData.Heroine _data)
			{
				return _data.schoolClass == schoolClass && _data.schoolClassIndex == schoolClassIndex;
			}
		}

		public class ResultInfo
		{
			public WaitPoint point;

			public int pointIndex = -1;

			public SaveData.Heroine heroine;

			public bool isPriority;

			public float actionTime = 5f;

			public float totalTime = 10f;

			public int actionNo = -1;

			public string actionName = string.Empty;

			public string layerName = string.Empty;

			public AgentSpeeder.Mode move;

			public HashSet<int> notMap;

			public int[] notAction;

			public int plansNo = -1;

			public string plansName = string.Empty;
		}

		private class CorrectionInfo
		{
			public int no { get; private set; }

			public int[] values { get; private set; }

			public int value
			{
				get
				{
					return (values.Length < 2) ? values[0] : UnityEngine.Random.Range(values[0], values[1]);
				}
			}

			public CorrectionInfo(int _no, params int[] _value)
			{
				no = _no;
				values = _value.ToArray();
			}
		}

		private class TimeValue
		{
			public float min { get; private set; }

			public float max { get; private set; }

			public float value
			{
				get
				{
					return UnityEngine.Random.Range(min, max);
				}
			}

			public TimeValue(string _str)
			{
				if (_str.IsNullOrEmpty())
				{
					min = 10f;
					max = 30f;
					return;
				}
				string[] array = _str.Split(',');
				min = float.Parse(array[0]);
				max = ((array.Length < 2) ? min : float.Parse(array[1]));
			}
		}

		private abstract class BaseInfo
		{
			public string name = string.Empty;

			public int priority = 100;

			public bool isPriority;

			public bool isHinder;

			public int target;

			public string layer = string.Empty;

			private TimeValue _actionTime;

			private TimeValue _totalTime;

			public List<CorrectionInfo> correction;

			public List<int> move;

			public bool isFirst;

			public float actionTime
			{
				get
				{
					return _actionTime.value;
				}
			}

			public float totalTime
			{
				get
				{
					return _totalTime.value;
				}
			}

			public AgentSpeeder.Mode moveType
			{
				get
				{
					if (move.IsNullOrEmpty())
					{
						return (AgentSpeeder.Mode)RandomMove(70, 30);
					}
					return (AgentSpeeder.Mode)((move.Count != 1) ? RandomMove(move[0], move[1]) : RandomMove(move[0], 100 - move[0]));
				}
			}

			public BaseInfo(List<string> _list)
			{
				name = _list[1];
				priority = int.Parse(_list[3]);
				isPriority = bool.Parse(_list[4]);
				isHinder = bool.Parse(_list[5]);
				target = int.Parse(_list[6]);
				layer = _list[7];
				_actionTime = new TimeValue(_list[8]);
				_totalTime = new TimeValue(_list[9]);
				string text = _list.SafeGet(10);
				if (!text.IsNullOrEmpty())
				{
					correction = new List<CorrectionInfo>();
					string[] array = text.Split(',');
					foreach (string text2 in array)
					{
						string[] array2 = text2.Split('=');
						correction.Add(new CorrectionInfo(int.Parse(array2[0]), array2[1].Split('/').Select(int.Parse).ToArray()));
					}
				}
				string text3 = _list.SafeGet(11);
				if (!text3.IsNullOrEmpty())
				{
					move = text3.Split(',').Select(int.Parse).ToList();
				}
				bool.TryParse(_list.SafeGet(12), out isFirst);
			}

			public bool Check(int _no, SaveData.Heroine _heroine)
			{
				ChaFileParameter.Awnser awnser = _heroine.chaCtrl.GetAwnser();
				switch (_no)
				{
				case 8:
					return !_heroine.isAnger;
				case 15:
					return awnser.fashionable;
				case 18:
					return awnser.exercise | _heroine.chaCtrl.GetAttribute(15);
				case 21:
					return awnser.study;
				default:
					return true;
				}
			}

			private IEnumerable<AgentSpeeder.Mode> Merge(AgentSpeeder.Mode[] _ary1, AgentSpeeder.Mode[] _ary2)
			{
				for (int i = 0; i < _ary1.Length; i++)
				{
					yield return _ary1[i];
				}
				for (int j = 0; j < _ary2.Length; j++)
				{
					yield return _ary2[j];
				}
			}

			private int RandomMove(params int[] _array)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < _array.Length; i++)
				{
					int element = i;
					list.AddRange(Enumerable.Repeat(element, _array[i]));
				}
				return list.Shuffle().First();
			}
		}

		private class PriorityActionInfo : BaseInfo
		{
			public int add;

			public PriorityActionInfo(List<string> _list)
				: base(_list)
			{
				add = int.Parse(_list[2]);
			}
		}

		private class GenericActionInfo : BaseInfo
		{
			public int probability;

			public GenericActionInfo(List<string> _list)
				: base(_list)
			{
				probability = int.Parse(_list[2]);
			}
		}

		private class PersonalityInfo
		{
			public int[] correction;

			public Dictionary<int, string[]> dicAction;

			public Dictionary<int, string[]> dicClub;

			public HashSet<int> hashNotAction;

			public Dictionary<int, int[]> dicMove;

			public PersonalityInfo()
			{
				dicAction = new Dictionary<int, string[]>();
				dicClub = new Dictionary<int, string[]>();
				hashNotAction = new HashSet<int>();
				dicMove = new Dictionary<int, int[]>();
			}

			public AgentSpeeder.Mode GetMoveType(int _no)
			{
				int[] value = null;
				if (!dicMove.TryGetValue(_no, out value))
				{
					return AgentSpeeder.Mode.Crouch;
				}
				int num = Array.FindIndex(value, (int v) => v == 100);
				if (num != -1)
				{
					return (AgentSpeeder.Mode)num;
				}
				Dictionary<AgentSpeeder.Mode, int> dictionary = new Dictionary<AgentSpeeder.Mode, int>();
				dictionary.Add(AgentSpeeder.Mode.Walk, value[0]);
				dictionary.Add(AgentSpeeder.Mode.Run, value[1]);
				return Utils.ProbabilityCalclator.DetermineFromDict(dictionary);
			}
		}

		private class CustomInfo
		{
			public List<CorrectionInfo> baseCorrection;

			public List<CorrectionInfo> addCorrection;

			public List<int> priorityMap;

			public CustomInfo(string _base, string _add, string _map)
			{
				baseCorrection = new List<CorrectionInfo>();
				if (!_base.IsNullOrEmpty())
				{
					string[] array = _base.Split(',');
					foreach (string text in array)
					{
						string[] array2 = text.Split('=');
						baseCorrection.Add(new CorrectionInfo(int.Parse(array2[0]), array2[1].Split('/').Select(int.Parse).ToArray()));
					}
				}
				if (!_add.IsNullOrEmpty())
				{
					addCorrection = new List<CorrectionInfo>();
					string[] array3 = _add.Split(',');
					foreach (string text2 in array3)
					{
						string[] array4 = text2.Split('=');
						addCorrection.Add(new CorrectionInfo(int.Parse(array4[0]), array4[1].Split('/').Select(int.Parse).ToArray()));
					}
				}
				if (!_map.IsNullOrEmpty())
				{
					priorityMap = _map.Split(',').Select(int.Parse).ToList();
				}
			}
		}

		private class MapInfo
		{
			public int[] priority;

			public Dictionary<int, int[]> clas;

			public Dictionary<int, int[]> club;

			public int this[int _t, int _cla, int _clb]
			{
				get
				{
					return priority[_t] + clas[_cla][_t] + club[_clb][_t];
				}
			}

			public MapInfo()
			{
				clas = new Dictionary<int, int[]>();
				club = new Dictionary<int, int[]>();
			}
		}

		public class AddValue : Manager.Communication.RangeValue
		{
			public int key { get; private set; }

			public float value
			{
				get
				{
					return UnityEngine.Random.Range(base.min, base.max);
				}
			}

			public AddValue(string _str)
			{
				string[] array = _str.Split('=');
				key = int.Parse(array[0]);
				Set(array[1]);
			}
		}

		private class CorrectConditionInfo
		{
			public int kind { get; private set; }

			public HashSet<int> stage { get; private set; }

			public Manager.Communication.RangeValue favor { get; private set; }

			public Manager.Communication.RangeValue lewdness { get; private set; }

			public HashSet<int> timezone { get; private set; }

			public HashSet<int> personality { get; private set; }

			public List<AddValue> addValue { get; private set; }

			public CorrectConditionInfo(List<string> _list)
			{
				int count = 1;
				kind = int.Parse(_list[count++]);
				stage = Convert(_list[count++]);
				favor = new Manager.Communication.RangeValue(_list[count++]);
				lewdness = new Manager.Communication.RangeValue(_list[count++]);
				timezone = Convert(_list[count++]);
				personality = Convert(_list[count++]);
				addValue = (from s in _list.Skip(count)
					where !s.IsNullOrEmpty()
					select new AddValue(s)).ToList();
			}

			private HashSet<int> Convert(string _str)
			{
				string[] array = _str.Split(',');
				int num = int.Parse(array[0]);
				if (num == -1)
				{
					return new HashSet<int>();
				}
				return array.Select(int.Parse).ToHashSet();
			}

			public void Reflect(DesireInfo _info, SaveData.Heroine _heroine, Cycle.Type _timezone, params int[] _not)
			{
				if ((stage.Count != 0 && !stage.Contains(_heroine.relation)) || (!favor.through && !favor.Check(_heroine.favor)) || (!lewdness.through && !lewdness.Check(_heroine.lewdness)) || (timezone.Count != 0 && !timezone.Contains((int)_timezone)) || (personality.Count != 0 && !personality.Contains(_heroine.personality)))
				{
					return;
				}
				foreach (AddValue item in addValue)
				{
					float num = item.value;
					if (!_not.IsNullOrEmpty() && _not.Contains(item.key))
					{
						continue;
					}
					switch (item.key)
					{
					case 8:
						if (_heroine.relation < 1)
						{
							continue;
						}
						break;
					case 5:
						if (!_heroine.chaCtrl.GetAttribute(4))
						{
							if (_heroine.relation <= 0 || _heroine.hCount <= 0)
							{
								continue;
							}
							if (_heroine.isStaff && item.key == 5)
							{
								num = (int)(num * Singleton<Manager.Communication>.Instance.staffBenefitsInfo.desire);
							}
						}
						break;
					}
					switch (item.key)
					{
					case 5:
						num = Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
						break;
					case 8:
						num = Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
						break;
					}
					_info[item.key] += (int)num;
				}
			}
		}

		private class DesireParam
		{
			public int no { get; private set; }

			public int desire { get; set; }

			public bool isPriority { get; private set; }

			public int priority { get; private set; }

			public DesireParam(int _no, bool _isPriority, int _desire, int _priority = 0)
			{
				no = _no;
				desire = _desire;
				isPriority = _isPriority;
				priority = _priority;
			}
		}

		private class DesireInfo
		{
			private int _action = -1;

			private Queue<int> _queueAction;

			private Queue<int> _queuePriority;

			private HashSet<int> hashPriority;

			public Dictionary<int, DesireParam> dicDesire { get; private set; }

			public int action
			{
				get
				{
					return _action;
				}
				set
				{
					_action = value;
					_queueAction.Enqueue(value);
					if (_queueAction.Count > 10)
					{
						_queueAction.Dequeue();
					}
					if (_queuePriority.Contains(value))
					{
						_queuePriority.Clear();
					}
				}
			}

			public bool isPriority { get; set; }

			public bool isDress { get; set; }

			public bool isLunch { get; set; }

			public int clubID { get; private set; }

			public WaitPoint nextPoint { get; private set; }

			public SaveData.Heroine heroine { get; private set; }

			public Queue<int> queueAction
			{
				get
				{
					return _queueAction;
				}
			}

			private int queuePriority
			{
				set
				{
					_queuePriority.Enqueue(value);
					if (_queuePriority.Count > 1)
					{
						hashPriority.Add(_queuePriority.Dequeue());
					}
				}
			}

			public int this[int i]
			{
				get
				{
					DesireParam value = null;
					return dicDesire.TryGetValue(i, out value) ? value.desire : 0;
				}
				set
				{
					if (dicDesire.ContainsKey(i))
					{
						dicDesire[i].desire = ((!dicDesire[i].isPriority) ? value : Mathf.Clamp(value, 0, 100));
					}
				}
			}

			public DesireInfo(Dictionary<int, BaseInfo> _base, PersonalityInfo _personality, int _clubOrg)
			{
				dicDesire = _base.Select((KeyValuePair<int, BaseInfo> v) => new DesireParam(v.Key, v.Value.isPriority, (!(v.Value is PriorityActionInfo)) ? ((v.Value as GenericActionInfo).probability + _personality.correction.SafeGet(v.Key)) : (v.Value as PriorityActionInfo).add)).ToDictionary((DesireParam v) => v.no, (DesireParam v) => v);
				_queueAction = new Queue<int>();
				_queuePriority = new Queue<int>();
				hashPriority = new HashSet<int>();
				action = -1;
				isPriority = false;
				clubID = _clubOrg;
			}

			public int Sort(ref bool _reset, SaveData.Heroine _heroine, params int[] _not)
			{
				bool flag = false;
				ChaFileDefine.CoordinateType coordinate = (ChaFileDefine.CoordinateType)_heroine.chaCtrl.chaFile.status.coordinateType;
				KeyValuePair<int, DesireParam> value = (from v in dicDesire
					where v.Value.isPriority
					where _not == null || !_not.Contains(v.Key)
					where Check(v.Key, _heroine)
					where Coordinate(v.Key, coordinate)
					where !_queuePriority.Contains(v.Key)
					orderby v.Value.desire descending, v.Value.priority descending
					select v).FirstOrDefault();
				_reset = flag;
				int num2 = (queuePriority = ((!value.IsDefault()) ? value.Key : (-1)));
				if (num2 != -1 && hashPriority.Contains(num2))
				{
					hashPriority.Remove(num2);
					_reset = true;
				}
				return num2;
			}

			public void SetWaitPoint(WaitPoint wp, NPC _npc)
			{
				if (nextPoint != null)
				{
					nextPoint.UnReserve();
				}
				nextPoint = wp;
				if (nextPoint != null)
				{
					nextPoint.Reserve(_npc);
				}
				heroine = null;
			}

			public void SetHeroine(SaveData.Heroine _heroine)
			{
				SetWaitPoint(null, null);
				heroine = _heroine;
			}

			public void Refresh()
			{
				if (nextPoint != null)
				{
					nextPoint.UnReserve();
				}
				nextPoint = null;
				heroine = null;
				action = -1;
				isPriority = false;
				isDress = false;
				isLunch = false;
				_queueAction.Clear();
			}

			public bool Check(int _key, SaveData.Heroine _heroine)
			{
				DesireParam value = null;
				bool flag = dicDesire.TryGetValue(_key, out value) && (!value.isPriority || value.desire >= 100);
				switch (_key)
				{
				case 5:
					return flag & !_heroine.isAnger;
				case 26:
					return flag & _heroine.parameter.attribute.likeGirls;
				default:
					return flag;
				}
			}

			public bool Coordinate(int _key, ChaFileDefine.CoordinateType _coordinate)
			{
				if (_key == 6)
				{
					return clubID == 0 || _coordinate == ChaFileDefine.CoordinateType.Club;
				}
				return true;
			}
		}

		private static List<DesireSaveData> listSaveData;

		private Dictionary<int, BaseInfo> dicBaseInfo;

		private Dictionary<int, PersonalityInfo> dicPersonalityInfo;

		private Dictionary<int, CustomInfo> dicCustomInfo;

		private Dictionary<int, MapInfo> dicMapInfo;

		private Dictionary<int, bool> dicNotCovered;

		private Dictionary<int, bool> dicSwimsuitBanArea;

		private Dictionary<int, TimeValue> dicActionTime;

		private Dictionary<int, HashSet<int>> dicClubMotion;

		private Dictionary<int, CorrectConditionInfo> dicCorrectConditionInfo;

		private Dictionary<SaveData.Heroine, DesireInfo> dicTarget;

		private int firstNum;

		private const int firstMinimum = 10;

		private int firstMinimumSaturday = 3;

		private HashSet<int> notCovered
		{
			get
			{
				return (from v in dicNotCovered
					where !v.Value
					select v.Key).ToHashSet();
			}
		}

		private HashSet<int> swimsuitBanArea
		{
			get
			{
				return (from v in dicSwimsuitBanArea
					where !v.Value
					select v.Key).ToHashSet();
			}
		}

		private Cycle.Type timezone { get; set; }

		public int GetDesire(int actionNo, SaveData.Heroine heroine)
		{
			DesireInfo value;
			if (!dicTarget.TryGetValue(heroine, out value))
			{
				return -1;
			}
			DesireParam value2;
			if (!value.dicDesire.TryGetValue(actionNo, out value2))
			{
				return -1;
			}
			return value2.desire;
		}

		public bool SetDesire(int actionNo, SaveData.Heroine heroine, int value)
		{
			DesireInfo value2;
			if (!dicTarget.TryGetValue(heroine, out value2))
			{
				return false;
			}
			DesireParam value3;
			if (!value2.dicDesire.TryGetValue(actionNo, out value3))
			{
				return false;
			}
			switch (actionNo)
			{
			case 5:
				value = (int)Mathf.Lerp(0f, value, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
				break;
			case 8:
				value = (int)Mathf.Lerp(0f, value, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
				break;
			}
			value3.desire = value;
			return true;
		}

		public bool AddDesire(int actionNo, SaveData.Heroine heroine, int value)
		{
			DesireInfo value2;
			if (!dicTarget.TryGetValue(heroine, out value2))
			{
				return false;
			}
			DesireParam value3;
			if (!value2.dicDesire.TryGetValue(actionNo, out value3))
			{
				return false;
			}
			switch (actionNo)
			{
			case 5:
				value = (int)Mathf.Lerp(0f, value, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
				break;
			case 8:
				value = (int)Mathf.Lerp(0f, value, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
				break;
			}
			value3.desire = Mathf.Max(0, value3.desire + value);
			return true;
		}

		public bool Init()
		{
			dicBaseInfo = new Dictionary<int, BaseInfo>();
			dicPersonalityInfo = new Dictionary<int, PersonalityInfo>();
			dicCustomInfo = new Dictionary<int, CustomInfo>();
			dicTarget = new Dictionary<SaveData.Heroine, DesireInfo>();
			dicMapInfo = new Dictionary<int, MapInfo>();
			dicNotCovered = new Dictionary<int, bool>();
			dicSwimsuitBanArea = new Dictionary<int, bool>();
			dicActionTime = new Dictionary<int, TimeValue>();
			dicClubMotion = new Dictionary<int, HashSet<int>>();
			dicCorrectConditionInfo = new Dictionary<int, CorrectConditionInfo>();
			LoadData();
			foreach (SaveData.Heroine item in Singleton<Game>.Instance.saveData.heroineList.Where((SaveData.Heroine v) => v.fixCharaID == 0))
			{
				dicTarget[item] = new DesireInfo(dicBaseInfo, dicPersonalityInfo[item.personality], Game.GetClubInfo(item, true).ID);
			}
			if (listSaveData != null)
			{
				foreach (KeyValuePair<SaveData.Heroine, DesireInfo> v2 in dicTarget)
				{
					DesireSaveData desireSaveData = listSaveData.Find((DesireSaveData d) => d.Check(v2.Key));
					if (desireSaveData == null)
					{
						continue;
					}
					foreach (KeyValuePair<int, int> item2 in desireSaveData.desire)
					{
						v2.Value[item2.Key] = item2.Value;
					}
				}
			}
			listSaveData = null;
			return true;
		}

		public void Refresh()
		{
			List<SaveData.Heroine> targets = Singleton<Game>.Instance.saveData.heroineList.Where((SaveData.Heroine v) => v.fixCharaID == 0).ToList();
			List<KeyValuePair<SaveData.Heroine, DesireInfo>> list = dicTarget.Where((KeyValuePair<SaveData.Heroine, DesireInfo> v) => !targets.Contains(v.Key)).ToList();
			foreach (KeyValuePair<SaveData.Heroine, DesireInfo> item in list)
			{
				dicTarget.Remove(item.Key);
			}
			List<SaveData.Heroine> list2 = targets.Where((SaveData.Heroine v) => !dicTarget.ContainsKey(v)).ToList();
			foreach (SaveData.Heroine item2 in list2)
			{
				dicTarget[item2] = new DesireInfo(dicBaseInfo, dicPersonalityInfo[item2.personality], Game.GetClubInfo(item2, true).ID);
			}
		}

		public void NextTime(params int[] _not)
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			timezone = actScene.Cycle.nowType;
			firstNum = 0;
			firstMinimumSaturday = UnityEngine.Random.Range(3, 7);
			foreach (KeyValuePair<SaveData.Heroine, DesireInfo> item in dicTarget.Where((KeyValuePair<SaveData.Heroine, DesireInfo> v) => v.Key.root != null))
			{
				SaveData.Heroine key = item.Key;
				item.Value.Refresh();
				PersonalityInfo value = null;
				dicPersonalityInfo.TryGetValue(key.personality, out value);
				foreach (KeyValuePair<int, BaseInfo> item2 in dicBaseInfo.Where((KeyValuePair<int, BaseInfo> v) => v.Value.isPriority))
				{
					PriorityActionInfo priorityActionInfo = item2.Value as PriorityActionInfo;
					if (priorityActionInfo == null)
					{
						continue;
					}
					int num = priorityActionInfo.add + value.correction.SafeGet(item2.Key);
					if (key.isStaff && item2.Key == 5)
					{
						num = (int)((float)num * Singleton<Manager.Communication>.Instance.staffBenefitsInfo.desire);
					}
					if (_not.IsNullOrEmpty() || !_not.Contains(item2.Key))
					{
						switch (item2.Key)
						{
						case 5:
							num = (int)Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
							break;
						case 8:
							num = (int)Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
							break;
						}
						item.Value[item2.Key] += num;
					}
				}
				foreach (var item3 in from v in key.chaCtrl.GetAttribute().Select((bool b, int i) => new { b, i })
					where v.b
					select v)
				{
					CustomInfo value2 = null;
					if (!dicCustomInfo.TryGetValue(item3.i, out value2) || value2.baseCorrection.IsNullOrEmpty())
					{
						continue;
					}
					foreach (CorrectionInfo item4 in value2.baseCorrection)
					{
						int num2 = item4.value;
						if (key.isStaff && item4.no == 5)
						{
							num2 = (int)((float)num2 * Singleton<Manager.Communication>.Instance.staffBenefitsInfo.desire);
						}
						if (_not.IsNullOrEmpty() || !_not.Contains(item4.no))
						{
							switch (item4.no)
							{
							case 5:
								num2 = (int)Mathf.Lerp(0f, num2, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
								break;
							case 8:
								num2 = (int)Mathf.Lerp(0f, num2, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
								break;
							}
							item.Value[item4.no] += num2;
						}
					}
				}
				if (actScene.Cycle.nowWeek != Cycle.Week.Holiday)
				{
					switch (timezone)
					{
					case Cycle.Type.LunchTime:
					{
						bool flag = false;
						if (actScene.Cycle.nowWeek != Cycle.Week.Saturday)
						{
							if (string.Compare("教室", actScene.Cycle.GetNowLessones(key.schoolClass)[0]) != 0)
							{
								item.Value[0] += 100;
							}
							AddLunch(item.Value);
						}
						item.Value[6] = 0;
						break;
					}
					case Cycle.Type.StaffTime:
						item.Value[3] = 0;
						item.Value[0] = 100;
						item.Value[6] = 100;
						break;
					case Cycle.Type.AfterSchool:
						item.Value[0] = 100;
						break;
					}
				}
				foreach (KeyValuePair<int, CorrectConditionInfo> item5 in dicCorrectConditionInfo.Where((KeyValuePair<int, CorrectConditionInfo> v) => v.Value.kind == 0))
				{
					item5.Value.Reflect(item.Value, key, timezone, _not);
				}
			}
		}

		public ResultInfo FirstAction(SaveData.Heroine _heroine)
		{
			DesireInfo value = null;
			if (!dicTarget.TryGetValue(_heroine, out value))
			{
				return null;
			}
			bool flag = false;
			ActionScene actionScene = ((!Singleton<Game>.IsInstance()) ? null : Singleton<Game>.Instance.actScene);
			if (actionScene != null)
			{
				flag = actionScene.Cycle.nowWeek == Cycle.Week.Saturday;
			}
			bool flag2 = false;
			switch (timezone)
			{
			case Cycle.Type.AfterSchool:
				flag2 = !_heroine.isStaff || (firstNum >= 10 && Utils.ProbabilityCalclator.DetectFromPercent(50f));
				break;
			default:
				flag2 = !flag && _heroine.schoolClass == 1 && firstNum > 10;
				break;
			case Cycle.Type.StaffTime:
				break;
			}
			if (!flag2)
			{
				if (!flag)
				{
					if (_heroine.schoolClass == 1)
					{
						firstNum++;
					}
				}
				else
				{
					switch (timezone)
					{
					case Cycle.Type.LunchTime:
						firstNum++;
						break;
					case Cycle.Type.StaffTime:
					case Cycle.Type.AfterSchool:
						if (_heroine.schoolClass == 1)
						{
							firstNum++;
						}
						break;
					}
				}
				NPC npc = ((!(actionScene != null)) ? null : actionScene.npcList.FirstOrDefault((NPC v) => v.charaData == _heroine));
				PersonalityInfo personalityInfo = dicPersonalityInfo[_heroine.personality];
				switch (timezone)
				{
				default:
				{
					int mapNo2;
					if (!flag)
					{
						mapNo2 = actionScene.Map.ConvertMapNo(actionScene.Cycle.GetNowLessonesRename(_heroine.schoolClass)[0]);
					}
					else
					{
						int num = 33;
						mapNo2 = num;
						if (firstNum > firstMinimumSaturday)
						{
							ResultInfo resultInfo2 = NextAction(_heroine, -1, (from v in dicBaseInfo
								where !v.Value.isFirst
								select v.Key into v
								where v <= 22
								select v).ToArray());
							if (resultInfo2 != null && resultInfo2.point != null && resultInfo2.point.MapNo != num)
							{
								return resultInfo2;
							}
							foreach (KeyValuePair<int, global::MapInfo.Param> item in actionScene.Map.infoDic.Shuffle())
							{
								global::MapInfo.Param value2 = item.Value;
								if (value2.No != num && value2.isGate && !value2.isWarning)
								{
									Dictionary<string, List<Tuple<WaitPoint, int>>> pointDictionary = GetPointDictionary(value2.No, personalityInfo.hashNotAction);
									if (pointDictionary != null && pointDictionary.Values.Any())
									{
										mapNo2 = value2.No;
										break;
									}
								}
							}
						}
					}
					ResultInfo resultInfo3 = FirstAction(_heroine, value, personalityInfo, npc, mapNo2);
					return (resultInfo3 != null) ? resultInfo3 : CreateWaitAction(_heroine, npc, value, personalityInfo, mapNo2, -1, 5f, 50f);
				}
				case Cycle.Type.StaffTime:
				{
					int mapNo3 = ((!_heroine.isStaff) ? actionScene.Map.ConvertMapNo(Game.GetClubInfo(_heroine, true).Place) : 22);
					ResultInfo resultInfo4 = FirstAction(_heroine, value, personalityInfo, npc, mapNo3);
					if (resultInfo4 == null)
					{
						resultInfo4 = CreateWaitAction(_heroine, npc, value, personalityInfo, mapNo3, -1, 5f, 50f);
					}
					return resultInfo4;
				}
				case Cycle.Type.AfterSchool:
				{
					int mapNo = actionScene.Map.ConvertMapNo(Game.GetClubInfo(_heroine, true).Place);
					ResultInfo resultInfo = FirstAction(_heroine, value, personalityInfo, npc, mapNo);
					if (resultInfo == null)
					{
						resultInfo = CreateWaitAction(_heroine, npc, value, personalityInfo, mapNo, -1, 5f, 50f);
					}
					return resultInfo;
				}
				}
			}
			int mapID = ((timezone != Cycle.Type.LunchTime) ? (-1) : ((_heroine.schoolClass != 1) ? (-1) : Singleton<Game>.Instance.actScene.Map.no));
			ResultInfo result = NextAction(_heroine, mapID, (from v in dicBaseInfo
				where !v.Value.isFirst
				select v.Key).ToArray());
			Cycle.Type type = timezone;
			if ((type == Cycle.Type.StaffTime || type == Cycle.Type.AfterSchool) && (bool)value.nextPoint && CheckClubMap(value.nextPoint.MapNo, _heroine))
			{
				value[0] = 0;
			}
			return result;
		}

		public void AddDesire(SaveData.Heroine _heroine, params int[] _not)
		{
			DesireInfo value = null;
			if (!dicTarget.TryGetValue(_heroine, out value))
			{
				return;
			}
			PersonalityInfo value2 = null;
			dicPersonalityInfo.TryGetValue(_heroine.personality, out value2);
			BaseInfo value3 = null;
			if (dicBaseInfo.TryGetValue(value.action, out value3) && !value3.correction.IsNullOrEmpty())
			{
				foreach (CorrectionInfo item in value3.correction)
				{
					if (_not.IsNullOrEmpty() || !_not.Contains(item.no))
					{
						int num = item.value;
						switch (item.no)
						{
						case 5:
							num = (int)Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
							break;
						case 8:
							num = (int)Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
							break;
						}
						value[item.no] += num;
					}
				}
			}
			foreach (KeyValuePair<int, BaseInfo> item2 in dicBaseInfo.Where((KeyValuePair<int, BaseInfo> v) => v.Value.isPriority))
			{
				switch (item2.Key)
				{
				case 8:
					if (_heroine.relation < 1)
					{
						continue;
					}
					break;
				case 5:
					if (_heroine.chaCtrl.GetAttribute(4) || (_heroine.relation > 0 && _heroine.hCount > 0))
					{
						break;
					}
					continue;
				}
				if (_not.IsNullOrEmpty() || !_not.Contains(item2.Key))
				{
					PriorityActionInfo priorityActionInfo = item2.Value as PriorityActionInfo;
					int num2 = priorityActionInfo.add + ((value2 != null) ? value2.correction.SafeGet(item2.Key) : 0);
					if (_heroine.isStaff && item2.Key == 5)
					{
						num2 = (int)((float)num2 * Singleton<Manager.Communication>.Instance.staffBenefitsInfo.desire);
					}
					switch (item2.Key)
					{
					case 5:
						num2 = (int)Mathf.Lerp(0f, num2, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
						break;
					case 8:
						num2 = (int)Mathf.Lerp(0f, num2, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
						break;
					}
					value[item2.Key] += num2;
				}
			}
			int[] pa = (from v in dicBaseInfo
				where v.Value.isPriority
				select v.Key).ToArray();
			foreach (var item3 in from v in _heroine.chaCtrl.GetAttribute().Select((bool b, int i) => new { b, i })
				where v.b
				select v)
			{
				CustomInfo value4 = null;
				if (!dicCustomInfo.TryGetValue(item3.i, out value4) || value4.addCorrection.IsNullOrEmpty())
				{
					continue;
				}
				foreach (CorrectionInfo item4 in value4.addCorrection.Where((CorrectionInfo v) => pa.Contains(v.no)))
				{
					int num3 = item4.value;
					if (_not.IsNullOrEmpty() || !_not.Contains(item4.no))
					{
						if (_heroine.isStaff && item4.no == 5)
						{
							num3 = (int)((float)num3 * Singleton<Manager.Communication>.Instance.staffBenefitsInfo.desire);
						}
						switch (item4.no)
						{
						case 5:
							num3 = (int)Mathf.Lerp(0f, num3, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
							break;
						case 8:
							num3 = (int)Mathf.Lerp(0f, num3, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
							break;
						}
						value[item4.no] += num3;
					}
				}
			}
			ChaFileParameter.Awnser awnser = _heroine.chaCtrl.GetAwnser();
			if (awnser.eat)
			{
				value[3] += 5;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			switch (timezone)
			{
			case Cycle.Type.LunchTime:
			{
				bool flag = false;
				if (actScene.Cycle.nowWeek == Cycle.Week.Saturday)
				{
					break;
				}
				if (!value.isDress && Mathf.InverseLerp(0f, 500f, actScene.Cycle.timer) >= 1f / 6f)
				{
					if (string.Compare("教室", actScene.Cycle.GetNowLessones(_heroine.schoolClass)[1]) != 0)
					{
						value[0] += 100;
					}
					value.isDress = true;
				}
				AddLunch(value);
				break;
			}
			case Cycle.Type.StaffTime:
				if (value.clubID != 0)
				{
					ChaFileDefine.CoordinateType coordinateType = (ChaFileDefine.CoordinateType)_heroine.chaCtrl.chaFile.status.coordinateType;
					if (coordinateType == ChaFileDefine.CoordinateType.Club)
					{
						value[6] = 100;
					}
					else
					{
						value[0] = 100;
					}
				}
				break;
			}
			foreach (KeyValuePair<int, CorrectConditionInfo> item5 in dicCorrectConditionInfo.Where((KeyValuePair<int, CorrectConditionInfo> v) => v.Value.kind == 1))
			{
				item5.Value.Reflect(value, _heroine, timezone, _not);
			}
			if (_heroine.talkTime <= 0)
			{
				value[8] = 0;
			}
			if (!(_heroine.talkEvent.Contains(0) | _heroine.talkEvent.Contains(1)))
			{
				value[5] = 0;
				value[8] = 0;
			}
		}

		public ResultInfo NextAction(SaveData.Heroine _heroine, params int[] _not)
		{
			return NextAction(_heroine, -1, _not);
		}

		public ResultInfo SetAction(SaveData.Heroine _heroine, int _actionNo, int _mapID = -1)
		{
			DesireInfo value = null;
			if (!dicTarget.TryGetValue(_heroine, out value))
			{
				return null;
			}
			return GetAction(_heroine, _actionNo, value, false, _mapID);
		}

		public ResultInfo EscapeAction(SaveData.Heroine _heroine, bool _isAnger)
		{
			DesireInfo value = null;
			if (!dicTarget.TryGetValue(_heroine, out value))
			{
				return null;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			NPC nPC = ((!(actScene != null)) ? null : actScene.npcList.FirstOrDefault((NPC v) => v.charaData == _heroine));
			if (actScene == null)
			{
				return null;
			}
			int num = 20;
			BaseInfo value2 = null;
			if (!dicBaseInfo.TryGetValue(num, out value2))
			{
				return null;
			}
			PersonalityInfo personalityInfo = dicPersonalityInfo[_heroine.personality];
			List<Tuple<WaitPoint, int>> pointList = GetPointList(value2.layer, personalityInfo.hashNotAction);
			int mapNo = nPC.mapNo;
			Tuple<WaitPoint, int> tuple = pointList.Where((Tuple<WaitPoint, int> v) => v.Item1.MapNo != mapNo).Shuffle().FirstOrDefault();
			if (tuple.Item1 == null)
			{
				return null;
			}
			value.SetWaitPoint(tuple.Item1, nPC);
			value.action = num;
			value.isPriority = !value2.isHinder;
			if (_isAnger && !_heroine.isAnger)
			{
				_heroine.isDate = false;
				_heroine.isAnger = true;
				_heroine.anger += 100;
				Singleton<Game>.Instance.rankSaveData.angerCount++;
			}
			AgentSpeeder.Mode mode = personalityInfo.GetMoveType(num);
			if (mode == AgentSpeeder.Mode.Crouch)
			{
				mode = ((value2 == null) ? AgentSpeeder.Mode.Run : value2.moveType);
			}
			ResultInfo resultInfo = new ResultInfo();
			resultInfo.point = value.nextPoint;
			resultInfo.pointIndex = tuple.Item2;
			resultInfo.heroine = value.heroine;
			resultInfo.isPriority = true;
			resultInfo.actionTime = ((value2 == null) ? 5f : value2.actionTime);
			resultInfo.totalTime = ((value2 == null) ? 10f : value2.totalTime);
			resultInfo.move = mode;
			resultInfo.actionNo = num;
			resultInfo.actionName = ((value2 == null) ? "逃げ" : value2.name);
			resultInfo.layerName = value2.layer;
			resultInfo.notAction = personalityInfo.hashNotAction.ToArray();
			return resultInfo;
		}

		public float GetActionTime(int _id)
		{
			TimeValue value = null;
			return (!dicActionTime.TryGetValue(_id, out value)) ? ((float)UnityEngine.Random.Range(10, 30)) : value.value;
		}

		public bool SetDesire(SaveData.Heroine _heroine, int _desire, int _value = 0)
		{
			DesireInfo value = null;
			if (!dicTarget.TryGetValue(_heroine, out value))
			{
				return false;
			}
			DesireParam value2 = null;
			if (!value.dicDesire.TryGetValue(_desire, out value2))
			{
				return false;
			}
			value2.desire = _value;
			return true;
		}

		public ResultInfo SetLesbian(SaveData.Heroine _main, SaveData.Heroine _target)
		{
			DesireInfo value = null;
			if (!dicTarget.TryGetValue(_target, out value))
			{
				return null;
			}
			value.SetHeroine(_main);
			return CreateAction(_target, value, dicPersonalityInfo[_target.personality], 27, string.Empty, -1);
		}

		public void Save(BinaryWriter _writer)
		{
			_writer.Write(dicTarget.Count);
			foreach (KeyValuePair<SaveData.Heroine, DesireInfo> item in dicTarget)
			{
				_writer.Write(item.Key.schoolClass);
				_writer.Write(item.Key.schoolClassIndex);
				_writer.Write(item.Value.dicDesire.Count);
				foreach (KeyValuePair<int, DesireParam> item2 in item.Value.dicDesire)
				{
					_writer.Write(item2.Value.no);
					_writer.Write(item2.Value.desire);
				}
			}
		}

		public static void Load(BinaryReader _reader, Version _version)
		{
			listSaveData = new List<DesireSaveData>();
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int schoolClass = _reader.ReadInt32();
				int schoolClassIndex = _reader.ReadInt32();
				int num2 = _reader.ReadInt32();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int j = 0; j < num2; j++)
				{
					dictionary.Add(_reader.ReadInt32(), _reader.ReadInt32());
				}
				listSaveData.Add(new DesireSaveData(schoolClass, schoolClassIndex, dictionary));
			}
		}

		private void LoadData()
		{
			List<string> list = (from s in CommonLib.GetAssetBundleNameListFromPath("action/actioncontrol/", true)
				where Regex.Match(Path.GetFileNameWithoutExtension(s), "(\\d*)").Success
				select s).ToList();
			list.Sort();
			foreach (string item in list)
			{
				LoadBaseInfo(item);
				LoadPersonalityInfo(item);
				LoadPersonalityNotActionInfo(item);
				LoadPersonalityMoveInfo(item);
				LoadCustomInfo(item);
				LoadActionInfo(item);
				LoadMapInfo(item);
				LoadMapClassInfo(item);
				LoadMapClubInfo(item);
				LoadMapNotCoveredInfo(item);
				LoadSwimsuitBanAreaInfo(item);
				LoadActionTimeInfo(item);
				LoadClubMotionInfo(item);
				LoadCorrectConditionInfo(item);
			}
		}

		private void LoadBaseInfo(string _path)
		{
			string text = "base_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				if (bool.Parse(item[4]))
				{
					dicBaseInfo[int.Parse(item[0])] = new PriorityActionInfo(item);
				}
				else
				{
					dicBaseInfo[int.Parse(item[0])] = new GenericActionInfo(item);
				}
			}
		}

		private void LoadPersonalityInfo(string _path)
		{
			string text = "personality_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				dicPersonalityInfo[int.Parse(item[1])] = new PersonalityInfo
				{
					correction = item.Skip(2).Select(int.Parse).ToArray()
				};
			}
		}

		private void LoadCustomInfo(string _path)
		{
			string text = "custom_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				dicCustomInfo[int.Parse(item[1])] = new CustomInfo(item.SafeGet(2), item.SafeGet(3), item.SafeGet(4));
			}
		}

		private void LoadActionInfo(string _path)
		{
			string reg = "action_c(\\d*)";
			IEnumerable<string> enumerable = from s in AssetBundleCheck.GetAllAssetName(_path).Select(Path.GetFileNameWithoutExtension)
				where Regex.Match(s, reg).Success
				select s;
			foreach (string item in enumerable)
			{
				if (!AssetBundleCheck.IsFile(_path, item))
				{
					break;
				}
				ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, item, false, string.Empty);
				if (excelData == null)
				{
					continue;
				}
				PersonalityInfo personalityInfo = dicPersonalityInfo[int.Parse(Regex.Match(item, reg).Groups[1].Value)];
				foreach (List<string> item2 in from p in excelData.list.Skip(1)
					select p.list)
				{
					personalityInfo.dicAction[int.Parse(item2[2])] = item2.Skip(3).ToArray();
				}
			}
		}

		private void LoadClubInfo(string _path)
		{
			string reg = "club_c(\\d*)";
			IEnumerable<string> enumerable = from s in AssetBundleCheck.GetAllAssetName(_path).Select(Path.GetFileNameWithoutExtension)
				where Regex.Match(s, reg).Success
				select s;
			foreach (string item in enumerable)
			{
				if (!AssetBundleCheck.IsFile(_path, item))
				{
					break;
				}
				ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, item, false, string.Empty);
				if (excelData == null)
				{
					continue;
				}
				PersonalityInfo personalityInfo = dicPersonalityInfo[int.Parse(Regex.Match(item, reg).Groups[1].Value)];
				foreach (List<string> item2 in from p in excelData.list.Skip(1)
					select p.list)
				{
					personalityInfo.dicClub[int.Parse(item2[1])] = item2.Skip(2).ToArray();
				}
			}
		}

		private void LoadMapInfo(string _path)
		{
			string text = "map_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				dicMapInfo[int.Parse(item[1])] = new MapInfo
				{
					priority = item.Skip(2).Select(int.Parse).ToArray()
				};
			}
		}

		private void LoadMapClassInfo(string _path)
		{
			string reg = "mapclass_c(\\d*)";
			IEnumerable<string> enumerable = from s in AssetBundleCheck.GetAllAssetName(_path).Select(Path.GetFileNameWithoutExtension)
				where Regex.Match(s, reg).Success
				select s;
			foreach (string item in enumerable)
			{
				if (!AssetBundleCheck.IsFile(_path, item))
				{
					break;
				}
				ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, item, false, string.Empty);
				if (excelData == null)
				{
					continue;
				}
				int key = int.Parse(Regex.Match(item, reg).Groups[1].Value);
				foreach (List<string> item2 in from p in excelData.list.Skip(1)
					select p.list)
				{
					dicMapInfo[int.Parse(item2[1])].clas[key] = item2.Skip(2).Select(int.Parse).ToArray();
				}
			}
		}

		private void LoadMapClubInfo(string _path)
		{
			string reg = "mapclub_c(\\d*)";
			IEnumerable<string> enumerable = from s in AssetBundleCheck.GetAllAssetName(_path).Select(Path.GetFileNameWithoutExtension)
				where Regex.Match(s, reg).Success
				select s;
			foreach (string item in enumerable)
			{
				if (!AssetBundleCheck.IsFile(_path, item))
				{
					break;
				}
				ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, item, false, string.Empty);
				if (excelData == null)
				{
					continue;
				}
				int key = int.Parse(Regex.Match(item, reg).Groups[1].Value);
				foreach (List<string> item2 in from p in excelData.list.Skip(1)
					select p.list)
				{
					dicMapInfo[int.Parse(item2[1])].club[key] = item2.Skip(2).Select(int.Parse).ToArray();
				}
			}
		}

		private void LoadMapNotCoveredInfo(string _path)
		{
			string text = "notcovered_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				if (int.TryParse(item[1], out result))
				{
					dicNotCovered[result] = bool.Parse(item[2]);
				}
			}
		}

		private void LoadActionTimeInfo(string _path)
		{
			string text = "actiontime_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				if (int.TryParse(item[0], out result))
				{
					dicActionTime[result] = new TimeValue(item.SafeGet(4));
				}
			}
		}

		private void LoadSwimsuitBanAreaInfo(string _path)
		{
			string text = "swimsuitbanarea_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				if (int.TryParse(item[1], out result))
				{
					dicSwimsuitBanArea[result] = bool.Parse(item[2]);
				}
			}
		}

		private void LoadClubMotionInfo(string _path)
		{
			string text = "club_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				if (int.TryParse(item[1], out result))
				{
					int result2;
					dicClubMotion[result] = (from s in item.Skip(2)
						select (!int.TryParse(s, out result2)) ? (-1) : result2 into id
						where id != -1
						select id).ToHashSet();
				}
			}
		}

		private void LoadCorrectConditionInfo(string _path)
		{
			string text = "correctcondition_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				if (int.TryParse(item[0], out result))
				{
					dicCorrectConditionInfo[result] = new CorrectConditionInfo(item);
				}
			}
		}

		private void LoadPersonalityNotActionInfo(string _path)
		{
			string text = "personalitynotaction_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
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
				PersonalityInfo value = null;
				int result = -1;
				if (int.TryParse(item.SafeGet(1), out result) && dicPersonalityInfo.TryGetValue(result, out value))
				{
					value.hashNotAction = (from s in item.Skip(2)
						where !s.IsNullOrEmpty()
						select s).Select(int.Parse).ToHashSet();
				}
			}
		}

		private void LoadPersonalityMoveInfo(string _path)
		{
			string text = "personalitymove_" + Path.GetFileNameWithoutExtension(_path);
			if (!AssetBundleCheck.IsFile(_path, text) || !AssetBundleCheck.GetAllAssetName(_path, false).Contains(text))
			{
				return;
			}
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, text, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			int[] index = (from s in excelData.list[0].list.Skip(2)
				where !s.IsNullOrEmpty()
				select s).Select(int.Parse).ToArray();
			foreach (List<string> item in from p in excelData.list.Skip(2)
				select p.list)
			{
				PersonalityInfo value = null;
				int result = -1;
				if (int.TryParse(item.SafeGet(1), out result) && dicPersonalityInfo.TryGetValue(result, out value))
				{
					value.dicMove = (from s in item.Skip(2)
						where !s.IsNullOrEmpty()
						select s).Select((string s, int i) => new { s, i }).ToDictionary(m => index[m.i], m => m.s.Split(',').Select(int.Parse).ToArray());
				}
			}
		}

		private Tuple<WaitPoint, int> GetPoint(string _layer, SaveData.Heroine _heroine, int _club, int _mapNo = -1, Dictionary<int, int> _dicRe = null, int[] _notMapNo = null)
		{
			bool swimsuit = false;
			switch ((ChaFileDefine.CoordinateType)_heroine.chaCtrl.chaFile.status.coordinateType)
			{
			case ChaFileDefine.CoordinateType.Swim:
				if (_heroine.chaCtrl.GetNowClothesType() == 2)
				{
					swimsuit = true;
				}
				break;
			case ChaFileDefine.CoordinateType.Club:
				if (_heroine.clubActivities == 1 && _heroine.chaCtrl.GetNowClothesType() == 2)
				{
					swimsuit = true;
				}
				break;
			}
			ClubInfo.Param clubInfo = Game.GetClubInfo(_heroine, Utils.ProbabilityCalclator.DetectFromPercent(20f));
			PersonalityInfo personalityInfo = dicPersonalityInfo[_heroine.personality];
			string[] source = new string[2] { "Masturbation", "Lesbian" };
			List<Tuple<WaitPoint, int>> list = ((!(_heroine.chaCtrl.GetAttribute(14) | source.Contains(_layer))) ? null : GetSinglePointList(_layer, personalityInfo.hashNotAction, clubInfo.ID, swimsuit, _notMapNo));
			if (list.IsNullOrEmpty())
			{
				list = GetPointList(_layer, personalityInfo.hashNotAction, clubInfo.ID, swimsuit, _notMapNo);
			}
			Tuple<WaitPoint, int> result = ((_mapNo == -1) ? new Tuple<WaitPoint, int>(null, -1) : list.Where((Tuple<WaitPoint, int> v) => v.Item1.MapNo == _mapNo).Shuffle().FirstOrDefault());
			if (result.Item1 != null && result.Item2 != -1)
			{
				return result;
			}
			if (_heroine.chaCtrl.GetAttribute(13))
			{
				return list.Shuffle().FirstOrDefault();
			}
			foreach (var item in (from v in _heroine.chaCtrl.GetAttribute().Select((bool b, int i) => new { b, i })
				where v.b
				select v).Shuffle())
			{
				CustomInfo value = null;
				if (!dicCustomInfo.TryGetValue(item.i, out value) || value.priorityMap.IsNullOrEmpty())
				{
					continue;
				}
				foreach (int map in value.priorityMap.Shuffle())
				{
					Tuple<WaitPoint, int> result2 = list.Where((Tuple<WaitPoint, int> i) => i.Item1.MapNo == map).Shuffle().FirstOrDefault();
					if (result2.Item1 == null)
					{
						continue;
					}
					return result2;
				}
			}
			foreach (int map2 in RandomSort(Timezone(timezone), _heroine.schoolClass, _club, _dicRe))
			{
				Tuple<WaitPoint, int> result3 = list.Where((Tuple<WaitPoint, int> v) => v.Item1.MapNo == map2).Shuffle().FirstOrDefault();
				if (result3.Item1 == null)
				{
					continue;
				}
				return result3;
			}
			return new Tuple<WaitPoint, int>(null, -1);
		}

		private List<Tuple<WaitPoint, int>> GetPointList(string _layer, HashSet<int> _notAction, int _club = -1, bool _swimsuit = false, int[] _notMap = null)
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			bool flag = _layer == "Club";
			List<Tuple<WaitPoint, int>> list = new List<Tuple<WaitPoint, int>>();
			HashSet<int> array = notCovered;
			if (_swimsuit)
			{
				List<int> list2 = array.ToList();
				list2.AddRange(swimsuitBanArea.ToList());
				array = list2.ToHashSet();
			}
			if (!_notMap.IsNullOrEmpty())
			{
				List<int> list3 = array.ToList();
				list3.AddRange(_notMap);
				array = list3.ToHashSet();
			}
			foreach (List<WaitPoint> value in actScene.PosSet.waitPointDic.Values)
			{
				foreach (WaitPoint item in value.Where((WaitPoint v) => !array.Contains(v.MapNo)))
				{
					int num = item.parameterList.FindIndex((WaitPoint.Parameter p) => p.layer == _layer);
					if (num == -1)
					{
						continue;
					}
					if (flag)
					{
						HashSet<int> cm = dicClubMotion[_club];
						if (!item.parameterList[num].motionList.Any((WaitPoint.Parameter.Motion m) => cm.Contains(m.ID)))
						{
							continue;
						}
					}
					IEnumerable<WaitPoint.Parameter.Motion> source = item.parameterList[num].motionList.Where((WaitPoint.Parameter.Motion m) => !_notAction.Contains(m.ID));
					if (source.Count() != 0)
					{
						list.Add(new Tuple<WaitPoint, int>(item, num));
					}
				}
			}
			return list.Where((Tuple<WaitPoint, int> v) => !v.Item1.isReserved).ToList();
		}

		private List<Tuple<WaitPoint, int>> GetSinglePointList(string _layer, HashSet<int> _notAction, int _club, bool _swimsuit, int[] _notMap)
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			HashSet<int> array = notCovered;
			if (_swimsuit)
			{
				List<int> list = array.ToList();
				list.AddRange(swimsuitBanArea.ToList());
				array = list.ToHashSet();
			}
			if (!_notMap.IsNullOrEmpty())
			{
				List<int> list2 = array.ToList();
				list2.AddRange(_notMap);
				array = list2.ToHashSet();
			}
			Fix fixChara = actScene.fixChara;
			if (fixChara != null && fixChara.isCharaLoad)
			{
				array.Add(fixChara.mapNo);
			}
			Dictionary<int, List<WaitPoint>> dictionary = (from v in actScene.PosSet.waitPointDic
				where !array.Contains(v.Key)
				where !v.Value.Any((WaitPoint w) => w.isReserved)
				select v).ToDictionary((KeyValuePair<int, List<WaitPoint>> v) => v.Key, (KeyValuePair<int, List<WaitPoint>> v) => v.Value);
			if (dictionary.Count() == 0)
			{
				return null;
			}
			bool flag = _layer == "Club";
			List<Tuple<WaitPoint, int>> list3 = new List<Tuple<WaitPoint, int>>();
			foreach (List<WaitPoint> value in dictionary.Values)
			{
				foreach (WaitPoint item in value)
				{
					foreach (var item2 in from p in item.parameterList.Select((WaitPoint.Parameter v, int i) => new { v, i })
						where p.v.layer == _layer
						select p)
					{
						if (flag)
						{
							HashSet<int> cm = dicClubMotion[_club];
							if (!item2.v.motionList.Any((WaitPoint.Parameter.Motion m) => cm.Contains(m.ID)))
							{
								continue;
							}
						}
						IEnumerable<WaitPoint.Parameter.Motion> source = item2.v.motionList.Where((WaitPoint.Parameter.Motion m) => !_notAction.Contains(m.ID));
						if (source.Count() != 0)
						{
							int i2 = item2.i;
							list3.Add(new Tuple<WaitPoint, int>(item, i2));
						}
					}
				}
			}
			return list3.Where((Tuple<WaitPoint, int> v) => !v.Item1.isReserved).ToList();
		}

		private int Timezone(Cycle.Type _timezone)
		{
			switch (_timezone)
			{
			default:
				return 0;
			case Cycle.Type.StaffTime:
				return 1;
			case Cycle.Type.AfterSchool:
				return 2;
			}
		}

		private List<int> RandomSort(int _timezone, int _class, int _club, Dictionary<int, int> _dicRe)
		{
			Dictionary<int, int> dictionary = dicMapInfo.Shuffle().ToDictionary((KeyValuePair<int, MapInfo> v) => v.Key, delegate(KeyValuePair<int, MapInfo> v)
			{
				int num = v.Value[_timezone, _class, _club];
				int value = 0;
				if (_dicRe == null || _dicRe.TryGetValue(v.Key, out value))
				{
				}
				num += value;
				return (num <= 0) ? 1 : num;
			});
			List<int> list = new List<int>();
			while (dictionary.Count != 0)
			{
				int num2 = RandomValue(dictionary);
				list.Add(num2);
				dictionary.Remove(num2);
			}
			return list;
		}

		private int RandomValue(Dictionary<int, int> _dictionary)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, int> item in _dictionary)
			{
				int key = item.Key;
				list.AddRange(Enumerable.Repeat(key, item.Value));
			}
			return list.Shuffle().First();
		}

		private bool CheckClubMap(int _map, SaveData.Heroine _heroine)
		{
			ClubInfo.Param clubInfo = Game.GetClubInfo(_heroine, true);
			return clubInfo.ID == 0 || _map == Singleton<Game>.Instance.actScene.Map.ConvertMapNo(clubInfo.Place);
		}

		private Dictionary<int, int> GetRev(SaveData.Heroine _heroine)
		{
			return null;
		}

		private void AddLunch(DesireInfo _desire)
		{
			if (!_desire.isLunch && !Utils.ProbabilityCalclator.DetectFromPercent(50f))
			{
				_desire[3] += 100;
				_desire.isLunch = true;
			}
		}

		private Dictionary<string, List<Tuple<WaitPoint, int>>> GetPointDictionary(int _mapNo, HashSet<int> _notAction)
		{
			CharacterPosSet posSet = Singleton<Game>.Instance.actScene.PosSet;
			List<WaitPoint> value = null;
			if (!posSet.waitPointDic.TryGetValue(_mapNo, out value))
			{
				return null;
			}
			Dictionary<string, List<Tuple<WaitPoint, int>>> dictionary = new Dictionary<string, List<Tuple<WaitPoint, int>>>();
			foreach (WaitPoint item2 in value.Where((WaitPoint v) => !v.isReserved))
			{
				for (int i = 0; i < item2.parameterList.Count; i++)
				{
					WaitPoint.Parameter parameter = item2.parameterList[i];
					if (parameter.motionList.Where((WaitPoint.Parameter.Motion m) => !_notAction.Contains(m.ID)).Count() != 0)
					{
						if (!dictionary.ContainsKey(parameter.layer))
						{
							dictionary.Add(parameter.layer, new List<Tuple<WaitPoint, int>>());
						}
						List<Tuple<WaitPoint, int>> list = dictionary[parameter.layer];
						int item = i;
						list.Add(new Tuple<WaitPoint, int>(item2, item));
					}
				}
			}
			return dictionary;
		}

		private ResultInfo NextAction(SaveData.Heroine _heroine, int _mapID, params int[] _not)
		{
			DesireInfo di = null;
			if (!dicTarget.TryGetValue(_heroine, out di))
			{
				return null;
			}
			AddDesire(_heroine, _not);
			bool _reset = false;
			int num = di.Sort(ref _reset, _heroine, _not);
			if (num == -1)
			{
				Dictionary<int, int> targetDict = (from v in dicBaseInfo
					where !v.Value.isPriority
					where _not == null || !_not.Contains(v.Key)
					where v.Value.Check(v.Key, _heroine)
					select new Tuple<int, int>(v.Key, (v.Value as GenericActionInfo).probability + di[v.Key]) into v
					where v.Item2 > 0
					select v).ToDictionary((Tuple<int, int> v) => v.Item1, (Tuple<int, int> v) => v.Item2);
				num = Utils.ProbabilityCalclator.DetermineFromDict(targetDict);
			}
			return GetAction(_heroine, num, di, _reset, _mapID);
		}

		private ResultInfo GetAction(SaveData.Heroine _heroine, int _actionNo, DesireInfo _di, bool _reset, int _mapID)
		{
			ActionScene actionScene = ((!Singleton<Game>.IsInstance()) ? null : Singleton<Game>.Instance.actScene);
			NPC nPC = ((!(actionScene != null)) ? null : actionScene.npcList.FirstOrDefault((NPC v) => v.charaData == _heroine));
			PersonalityInfo pi = dicPersonalityInfo[_heroine.personality];
			int id = -1;
			string layer = string.Empty;
			bool flag = true;
			switch (dicBaseInfo[_actionNo].target)
			{
			case 0:
			case 3:
			{
				layer = dicBaseInfo[_actionNo].layer;
				Tuple<WaitPoint, int> point = GetPoint(layer, _heroine, _di.clubID, _mapID, GetRev(_heroine));
				if (point.Item1 == null)
				{
					flag = false;
					break;
				}
				_di.SetWaitPoint(point.Item1, nPC);
				id = point.Item2;
				break;
			}
			case 1:
			{
				KeyValuePair<SaveData.Heroine, DesireInfo> keyValuePair = (from v in dicTarget
					where v.Key != _heroine
					where !v.Value.isPriority
					select v).Shuffle().FirstOrDefault();
				if (keyValuePair.Key == null)
				{
					flag = false;
				}
				else
				{
					_di.SetHeroine(keyValuePair.Key);
				}
				break;
			}
			case 2:
				_di.SetHeroine(null);
				break;
			}
			if (!flag)
			{
				int mapNo = ((!(nPC != null)) ? (-1) : nPC.mapNo);
				ResultInfo result = CreateWaitAction(_heroine, nPC, _di, pi, mapNo, _actionNo, 10f, 30f, _reset);
				if (_reset && ResetDesire(_heroine, _di, _actionNo) && _actionNo == 0)
				{
					_heroine.NextCoordinate();
				}
				return result;
			}
			ResetDesire(_heroine, _di, _actionNo);
			return CreateAction(_heroine, _di, pi, _actionNo, layer, id);
		}

		private ResultInfo FirstAction(SaveData.Heroine _heroine, DesireInfo _di, PersonalityInfo _pi, NPC _npc, int _mapNo)
		{
			ChaFileDefine.CoordinateType coordinateType = (ChaFileDefine.CoordinateType)_heroine.chaCtrl.chaFile.status.coordinateType;
			Dictionary<string, List<Tuple<WaitPoint, int>>> dicPoint = GetPointDictionary(_mapNo, _pi.hashNotAction);
			KeyValuePair<int, BaseInfo> value = (from v in dicBaseInfo
				where v.Value.isFirst
				where v.Value.isPriority
				where _di.Check(v.Key, _heroine)
				orderby v.Value.priority descending
				select v).FirstOrDefault(delegate(KeyValuePair<int, BaseInfo> v)
			{
				bool flag = dicPoint != null && dicPoint.ContainsKey(v.Value.layer);
				if (flag && v.Value.layer == "Club")
				{
					HashSet<int> cm = dicClubMotion[_heroine.clubActivities];
					flag = dicPoint[v.Value.layer].Any((Tuple<WaitPoint, int> w) => w.Item1.parameterList[w.Item2].motionList.Any((WaitPoint.Parameter.Motion m) => cm.Contains(m.ID)));
				}
				return flag;
			});
			if (value.IsDefault())
			{
				value = (from v in dicBaseInfo
					where v.Value.isFirst
					where !v.Value.isPriority
					where v.Value.Check(v.Key, _heroine)
					where (v.Value as GenericActionInfo).probability + _di[v.Key] > 0
					select v).Shuffle().FirstOrDefault((KeyValuePair<int, BaseInfo> v) => dicPoint != null && dicPoint.ContainsKey(v.Value.layer));
			}
			if (value.IsDefault())
			{
				return null;
			}
			ResetDesire(_heroine, _di, value.Key);
			Tuple<WaitPoint, int> tuple = dicPoint[value.Value.layer].Shuffle().First();
			_di.SetWaitPoint(tuple.Item1, _npc);
			return CreateAction(_heroine, _di, _pi, value.Key, value.Value.layer, tuple.Item2);
		}

		private ResultInfo CreateAction(SaveData.Heroine _heroine, DesireInfo _di, PersonalityInfo _pi, int _key, string _layer, int _id)
		{
			_di.action = _key;
			_di.isPriority = _key != -1 && !dicBaseInfo[_key].isHinder;
			BaseInfo value = null;
			dicBaseInfo.TryGetValue(_key, out value);
			AgentSpeeder.Mode mode = _pi.GetMoveType(_key);
			if (mode == AgentSpeeder.Mode.Crouch)
			{
				mode = ((value != null) ? value.moveType : AgentSpeeder.Mode.Walk);
			}
			ResultInfo resultInfo = new ResultInfo();
			resultInfo.point = _di.nextPoint;
			resultInfo.pointIndex = _id;
			resultInfo.heroine = _di.heroine;
			resultInfo.isPriority = value != null && value.isPriority;
			resultInfo.actionTime = ((value == null) ? 10f : value.actionTime);
			resultInfo.totalTime = ((value == null) ? 10f : value.totalTime);
			resultInfo.move = mode;
			resultInfo.actionNo = _key;
			resultInfo.actionName = ((value == null) ? "ウロウロ" : value.name);
			resultInfo.layerName = _layer;
			resultInfo.notMap = GetNotMap(_heroine);
			resultInfo.notAction = _pi.hashNotAction.ToArray();
			return resultInfo;
		}

		private ResultInfo CreateWaitAction(SaveData.Heroine _heroine, NPC _npc, DesireInfo _di, PersonalityInfo _pi, int _mapNo, int _failureNo, float _min, float _max, bool _reset = false)
		{
			int[] array = null;
			if (_reset)
			{
				int no = Singleton<Game>.Instance.actScene.Map.no;
				_mapNo = ((_mapNo != no) ? _mapNo : (-1));
				array = new int[1] { no };
			}
			WaitPoint waitPoint = null;
			int num = -1;
			Dictionary<string, List<Tuple<WaitPoint, int>>> pointDictionary = GetPointDictionary(_mapNo, _pi.hashNotAction);
			if (pointDictionary != null && pointDictionary.ContainsKey("Wait"))
			{
				Tuple<WaitPoint, int> tuple = pointDictionary["Wait"].Shuffle().First();
				waitPoint = tuple.Item1;
				num = tuple.Item2;
			}
			else
			{
				string layer = "Wait";
				int clubID = _di.clubID;
				int mapNo = _mapNo;
				int[] notMapNo = array;
				Tuple<WaitPoint, int> point = GetPoint(layer, _heroine, clubID, mapNo, null, notMapNo);
				waitPoint = point.Item1;
				num = point.Item2;
			}
			_di.SetWaitPoint(waitPoint, _npc);
			_di.action = -1;
			_di.isPriority = false;
			BaseInfo value = null;
			dicBaseInfo.TryGetValue(_failureNo, out value);
			float num2 = UnityEngine.Random.Range(_min, _max);
			ResultInfo resultInfo = new ResultInfo();
			resultInfo.point = _di.nextPoint;
			resultInfo.pointIndex = num;
			resultInfo.heroine = null;
			resultInfo.isPriority = _reset;
			resultInfo.actionTime = num2;
			resultInfo.totalTime = num2;
			resultInfo.move = (_reset ? AgentSpeeder.Mode.Run : AgentSpeeder.Mode.Walk);
			resultInfo.actionNo = -1;
			resultInfo.actionName = "ウロウロ";
			resultInfo.layerName = "Wait";
			resultInfo.plansNo = _failureNo;
			resultInfo.plansName = ((value == null) ? string.Empty : value.name);
			resultInfo.notMap = GetNotMap(_heroine);
			resultInfo.notAction = _pi.hashNotAction.ToArray();
			return resultInfo;
		}

		private bool CheckSwimsuit(SaveData.Heroine _heroine)
		{
			switch ((ChaFileDefine.CoordinateType)_heroine.chaCtrl.chaFile.status.coordinateType)
			{
			case ChaFileDefine.CoordinateType.Swim:
				return _heroine.chaCtrl.GetNowClothesType() == 2;
			case ChaFileDefine.CoordinateType.Club:
				return _heroine.clubActivities == 1 && _heroine.chaCtrl.GetNowClothesType() == 2;
			default:
				return false;
			}
		}

		private HashSet<int> GetNotMap(SaveData.Heroine _heroine)
		{
			return CheckSwimsuit(_heroine) ? swimsuitBanArea : null;
		}

		private bool ResetDesire(SaveData.Heroine _heroine, DesireInfo _desire, int _key)
		{
			if (dicBaseInfo[_key].isPriority)
			{
				_desire[_key] = 0;
				switch (_key)
				{
				case 4:
					_desire[5] /= 2;
					break;
				case 5:
					_desire[4] /= 2;
					break;
				}
				return true;
			}
			PersonalityInfo personalityInfo = dicPersonalityInfo[_heroine.personality];
			GenericActionInfo genericActionInfo = dicBaseInfo[_key] as GenericActionInfo;
			int num = ((genericActionInfo == null) ? 5 : genericActionInfo.probability) + personalityInfo.correction.SafeGet(_key);
			switch (_key)
			{
			case 5:
				num = (int)Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionH));
				break;
			case 8:
				num = (int)Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.AIActionCorrectionTalk));
				break;
			}
			_desire[_key] = num;
			return false;
		}
	}
}
