using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ADV;
using Illusion;
using Localize.Translate;
using Manager;
using UniRx;
using UnityEngine;

namespace ActionGame.Communication
{
	public class Info
	{
		public enum Group
		{
			Encounter = 0,
			Introduction = 1,
			Talk = 2,
			Listen = 3,
			Apologize = 4,
			See = 5,
			Touch = 6,
			Interlude = 7,
			LeaveAlone = 8,
			EndConversation = 9,
			RankUp = 10,
			Event = 11
		}

		public class TalkData
		{
			public int favor;

			public int lewdness;

			public int anger;

			public string assetbundle = string.Empty;

			public string file = string.Empty;

			public string text = string.Empty;

			public string pose = string.Empty;

			public string facialExpression = string.Empty;

			public int eyeLook = 1;

			public int neckLook;
		}

		public class BasicInfo
		{
			public int priority;

			public int conditions = -1;

			public int row { get; private set; }

			public BasicInfo(int _row)
			{
				row = _row;
			}
		}

		public class GenericInfo : BasicInfo
		{
			public TalkData[] talk;

			public GenericInfo(int _row, int _num)
				: base(_row)
			{
				talk = new TalkData[_num];
			}

			public GenericInfo(int _row, TalkData[] _talk)
				: base(_row)
			{
				talk = _talk;
			}

			public GenericInfo(int _row, TalkData _talk)
				: base(_row)
			{
				talk = new TalkData[1];
				talk[0] = _talk;
			}
		}

		public class BranchInfo : BasicInfo
		{
			public TalkData introduction;

			public TalkData[] success;

			public TalkData failure;

			public TalkData noInterested;

			public TalkData this[int _idx]
			{
				get
				{
					List<TalkData> list = new List<TalkData>();
					list.AddRange(success);
					list.Add(failure);
					if (noInterested != null)
					{
						list.Add(noInterested);
					}
					return list[_idx];
				}
			}

			public int num
			{
				get
				{
					return success.Length + 1 + ((noInterested != null) ? 1 : 0);
				}
			}

			public BranchInfo(int _row)
				: base(_row)
			{
			}
		}

		public class ChoiceInfo : TalkData
		{
			public string choice = "ああ";
		}

		public class SelectInfo : BasicInfo
		{
			public TalkData introduction;

			public ChoiceInfo[] choice;

			public TalkData failure;

			public int num
			{
				get
				{
					return choice.Length;
				}
			}

			public SelectInfo(int _row, int _num)
				: base(_row)
			{
				choice = new ChoiceInfo[_num];
			}
		}

		private Dictionary<int, string[]> dicOptionDisplayItems;

		private PassingInfo _passingInfo;

		private BasicInfo prevInfo;

		private Queue<int> queueListen;

		private const int startCol = 8;

		public Dictionary<int, Dictionary<Group, Dictionary<int, List<BasicInfo>>>> dicTalkInfo { get; private set; }

		private PassingInfo passingInfo
		{
			get
			{
				return _passingInfo;
			}
		}

		private SaveData.Heroine heroine
		{
			get
			{
				return passingInfo.heroine;
			}
		}

		private int mapNo
		{
			get
			{
				return passingInfo.mapNo;
			}
		}

		private string[] placeNames
		{
			get
			{
				return passingInfo.placeNames;
			}
		}

		private Cycle.Type cycleType
		{
			get
			{
				return passingInfo.cycleType;
			}
		}

		private bool isOtherPeople
		{
			get
			{
				return passingInfo.isOtherPeople;
			}
		}

		private bool isChase
		{
			get
			{
				return passingInfo.isChase;
			}
		}

		private bool isChasePossible
		{
			get
			{
				return passingInfo.isChasePossible;
			}
		}

		private bool isHPossible
		{
			get
			{
				return passingInfo.isHPossible;
			}
		}

		private bool isNotice
		{
			get
			{
				return passingInfo.isNotice;
			}
		}

		private int state
		{
			get
			{
				return passingInfo.state;
			}
		}

		private bool isSports
		{
			get
			{
				ClubInfo.Param clubInfo = Game.GetClubInfo(heroine, true);
				return clubInfo != null && clubInfo.isSports;
			}
		}

		private bool exposure
		{
			get
			{
				return passingInfo.exposure;
			}
		}

		private bool isAttack
		{
			get
			{
				return passingInfo.isAttack;
			}
		}

		private bool femaleOnly
		{
			get
			{
				return passingInfo.femaleOnly;
			}
		}

		private bool isSecondH
		{
			get
			{
				return heroine.talkEvent.Contains(8) | heroine.talkEvent.Contains(12) | (heroine.hCount != 0);
			}
		}

		public void Init(PassingInfo _info)
		{
			_passingInfo = _info;
			LoadData(_info.heroine.FixCharaIDOrPersonality);
			LoadOptionDisplayItems(_info.heroine.FixCharaIDOrPersonality);
			prevInfo = null;
			queueListen = new Queue<int>();
		}

		public bool LoadData(int _personality)
		{
			string _bundle = string.Empty;
			string _file = string.Empty;
			if (!Singleton<Manager.Communication>.Instance.GetTalkFilePath(_personality, out _bundle, out _file))
			{
				return false;
			}
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(_bundle, _file, typeof(ExcelData));
			if (assetBundleLoadAssetOperation.IsEmpty())
			{
				AssetBundleManager.UnloadAssetBundle(_bundle, true);
				return false;
			}
			ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
			dicTalkInfo = new Dictionary<int, Dictionary<Group, Dictionary<int, List<BasicInfo>>>>();
			int _row = 6;
			LoadGenericOnly(asset.list, ref _row, 0, Group.Encounter, 2);
			LoadGenericOnly(asset.list, ref _row, 0, Group.Introduction, 22);
			List<BasicInfo> listCommand = GetListCommand(0, Group.Talk, 0);
			int failureRow = _row + 9;
			for (int i = 0; i < 3; i++)
			{
				listCommand.Add(CreateBranch(asset.list, ref _row, 2, failureRow));
			}
			_row++;
			for (int j = 0; j < 3; j++)
			{
				listCommand.Add(CreateGeneric(asset.list, ref _row, 2));
			}
			List<BasicInfo> listCommand2 = GetListCommand(0, Group.Talk, 1);
			int failureRow2 = _row + 9;
			for (int k = 0; k < 3; k++)
			{
				listCommand2.Add(CreateBranch(asset.list, ref _row, 2, failureRow2));
			}
			_row++;
			for (int l = 0; l < 3; l++)
			{
				listCommand2.Add(CreateGeneric(asset.list, ref _row, 2));
			}
			for (int m = 0; m < 6; m++)
			{
				listCommand2.Add(CreateBranch(asset.list, ref _row, 1, _row + 2));
				_row++;
			}
			List<BasicInfo> listCommand3 = GetListCommand(0, Group.Talk, 2);
			int failureRow3 = _row + 9;
			for (int n = 0; n < 3; n++)
			{
				listCommand3.Add(CreateBranch(asset.list, ref _row, 2, failureRow3));
			}
			_row++;
			for (int num = 0; num < 3; num++)
			{
				listCommand3.Add(CreateGeneric(asset.list, ref _row, 2));
			}
			int num2 = _row + 21 + 1;
			for (int num3 = 0; num3 < 7; num3++)
			{
				List<BasicInfo> listCommand4 = GetListCommand(0, Group.Listen, num3);
				listCommand4.Add(CreateSelect(asset.list, ref _row, 2, (num3 != 0) ? num2 : (-1)));
				if (num3 == 0)
				{
					_row++;
				}
			}
			_row++;
			LoadTalkAndListenAnger(0, asset.list, ref _row);
			LoadGenericOnly(asset.list, ref _row, 0, Group.Apologize, 2);
			LoadTouch(0, Group.See, 2, asset.list, ref _row);
			LoadTouch(0, Group.Touch, 4, asset.list, ref _row);
			LoadGenericOnly(asset.list, ref _row, 0, Group.Interlude, 2);
			LoadGenericOnly(asset.list, ref _row, 0, Group.LeaveAlone, 2);
			LoadGenericOnly(asset.list, ref _row, 0, Group.EndConversation, 7);
			LoadGenericOnly(asset.list, ref _row, 0, Group.EndConversation, 1);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int[] array = new int[4] { 6, 8, 9, 11 };
			for (int num4 = 0; num4 < 4; num4++)
			{
				dictionary.Add(array[num4], _row);
				List<BasicInfo> listCommand5 = GetListCommand(0, Group.Event, array[num4]);
				listCommand5.Add(CreateBranch(asset.list, ref _row, 2, _row + 3));
				_row++;
			}
			LoadGenericOnly(asset.list, ref _row, 1, Group.Introduction, 27);
			List<BasicInfo> listCommand6 = GetListCommand(1, Group.Talk, 0);
			int noInterestedRow = _row + 12;
			for (int num5 = 0; num5 < 4; num5++)
			{
				listCommand6.Add(CreateTalk(asset.list, ref _row, noInterestedRow));
			}
			_row++;
			listCommand6.Add(CreateGeneric(asset.list, ref _row, 2));
			List<BasicInfo> listCommand7 = GetListCommand(1, Group.Talk, 1);
			int noInterestedRow2 = _row + 12;
			for (int num6 = 0; num6 < 4; num6++)
			{
				listCommand7.Add(CreateTalk(asset.list, ref _row, noInterestedRow2));
			}
			_row++;
			listCommand7.Add(CreateGeneric(asset.list, ref _row, 2));
			List<BasicInfo> listCommand8 = GetListCommand(1, Group.Talk, 2);
			int noInterestedRow3 = _row + 12;
			for (int num7 = 0; num7 < 4; num7++)
			{
				listCommand8.Add(CreateTalk(asset.list, ref _row, noInterestedRow3));
			}
			_row++;
			listCommand8.Add(CreateGeneric(asset.list, ref _row, 2));
			int num8 = _row + 12 + 12;
			int[] array2 = new int[7] { 2, 2, 2, 2, 3, 3, 3 };
			for (int num9 = 0; num9 < array2.Length; num9++)
			{
				List<BasicInfo> listCommand9 = GetListCommand(1, Group.Listen, num9);
				listCommand9.Add(CreateSelect(asset.list, ref _row, array2[num9], (num9 != 0) ? num8 : (-1)));
			}
			_row++;
			LoadTalkAndListenAnger(1, asset.list, ref _row);
			LoadGenericOnly(asset.list, ref _row, 1, Group.Apologize, 2);
			LoadTouch(1, Group.See, 2, asset.list, ref _row);
			LoadTouch(1, Group.Touch, 4, asset.list, ref _row);
			LoadGenericOnly(asset.list, ref _row, 1, Group.Interlude, 3);
			LoadGenericOnly(asset.list, ref _row, 1, Group.LeaveAlone, 2);
			LoadGenericOnly(asset.list, ref _row, 1, Group.EndConversation, 7);
			LoadEvent(1, 1, 2, 2, asset.list, ref _row);
			LoadEvent(1, 3, 4, 1, asset.list, ref _row);
			LoadEvent(1, 4, 1, 1, asset.list, ref _row);
			int num10 = _row;
			LoadEvent(1, 5, 1, 1, asset.list, ref _row);
			LoadEvent(1, 6, 1, 1, asset.list, ref _row);
			LoadEvent(1, 7, 2, 1, asset.list, ref _row);
			LoadEvent(1, 8, 1, 1, asset.list, ref _row);
			LoadEvent(1, 9, 1, 1, asset.list, ref _row);
			int num11 = _row;
			bool isReplaceEvent10 = false;
			asset.list.SafeProc(589, delegate(ExcelData.Param p)
			{
				isReplaceEvent10 = p != null && !p.list.IsNullOrEmpty();
			});
			List<BasicInfo> listCommand10 = GetListCommand(1, Group.Event, 10);
			listCommand10.Add(CreateBranch(asset.list, ref _row, 1, _row + 2));
			_row++;
			if (!isReplaceEvent10)
			{
				listCommand10.Add(CreateGeneric(asset.list, _row++));
			}
			else
			{
				_row++;
			}
			List<BasicInfo> listCommand11 = GetListCommand(1, Group.Event, 11);
			listCommand11.Add(CreateBranch(asset.list, ref _row, 1, _row + 2));
			_row++;
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				List<BasicInfo> listCommand12 = GetListCommand(1, Group.Event, item.Key);
				int _startRow = item.Value;
				listCommand12.Add(CreateBranch(asset.list, ref _startRow, 1, _startRow + 3));
			}
			LoadGenericOnly(asset.list, ref _row, 2, Group.Introduction, 27);
			List<BasicInfo> listCommand13 = GetListCommand(2, Group.Talk, 0);
			int noInterestedRow4 = _row + 12;
			for (int num12 = 0; num12 < 4; num12++)
			{
				listCommand13.Add(CreateTalk(asset.list, ref _row, noInterestedRow4));
			}
			_row++;
			for (int num13 = 0; num13 < 2; num13++)
			{
				listCommand13.Add(CreateGeneric(asset.list, ref _row, 2));
			}
			List<BasicInfo> listCommand14 = GetListCommand(2, Group.Talk, 1);
			int noInterestedRow5 = _row + 12;
			for (int num14 = 0; num14 < 4; num14++)
			{
				listCommand14.Add(CreateTalk(asset.list, ref _row, noInterestedRow5));
			}
			_row++;
			for (int num15 = 0; num15 < 2; num15++)
			{
				listCommand14.Add(CreateGeneric(asset.list, ref _row, 2));
			}
			List<BasicInfo> listCommand15 = GetListCommand(2, Group.Talk, 2);
			int noInterestedRow6 = _row + 12;
			for (int num16 = 0; num16 < 4; num16++)
			{
				listCommand15.Add(CreateTalk(asset.list, ref _row, noInterestedRow6));
			}
			_row++;
			for (int num17 = 0; num17 < 2; num17++)
			{
				listCommand15.Add(CreateGeneric(asset.list, ref _row, 2));
			}
			int num18 = 0;
			for (int num19 = 0; num19 < 3; num19++)
			{
				List<BasicInfo> listCommand16 = GetListCommand(2, Group.Listen, num18++);
				listCommand16.Add(CreateSelect(asset.list, ref _row, 2));
			}
			for (int num20 = 0; num20 < 3; num20++)
			{
				List<BasicInfo> listCommand17 = GetListCommand(2, Group.Listen, num18++);
				listCommand17.Add(CreateGeneric(asset.list, ref _row, 3));
			}
			int failureRow4 = _row + 12;
			for (int num21 = 0; num21 < 3; num21++)
			{
				List<BasicInfo> listCommand18 = GetListCommand(2, Group.Listen, num18++);
				listCommand18.Add(CreateSelect(asset.list, ref _row, 3, failureRow4));
			}
			_row++;
			LoadTalkAndListenAnger(2, asset.list, ref _row);
			LoadGenericOnly(asset.list, ref _row, 2, Group.Apologize, 2);
			LoadTouch(2, Group.See, 2, asset.list, ref _row);
			LoadTouch(2, Group.Touch, 4, asset.list, ref _row);
			LoadGenericOnly(asset.list, ref _row, 2, Group.Interlude, 3);
			LoadGenericOnly(asset.list, ref _row, 2, Group.LeaveAlone, 2);
			LoadGenericOnly(asset.list, ref _row, 2, Group.EndConversation, 7);
			List<BasicInfo> listCommand19 = GetListCommand(2, Group.Event, 2);
			for (int num22 = 0; num22 < 4; num22++)
			{
				listCommand19.Add(CreateGeneric(asset.list, _row++));
			}
			List<BasicInfo> listCommand20 = GetListCommand(2, Group.Event, 3);
			for (int num23 = 0; num23 < 2; num23++)
			{
				listCommand20.Add(CreateGeneric(asset.list, _row++));
			}
			listCommand20.Add(CreateSelect(asset.list, ref _row, 2));
			for (int num24 = 0; num24 < 2; num24++)
			{
				listCommand20.Add(CreateBranch(asset.list, ref _row, 1, _row + 2));
				_row++;
			}
			List<BasicInfo> listCommand21 = GetListCommand(2, Group.Event, 4);
			listCommand21.Add(CreateGeneric(asset.list, _row++));
			listCommand21.Add(CreateSelect(asset.list, ref _row, 2));
			List<BasicInfo> listCommand22 = GetListCommand(2, Group.Event, 5);
			listCommand22.Add(CreateGeneric(asset.list, _row++));
			listCommand22.Add(CreateSelect(asset.list, ref _row, 2));
			List<BasicInfo> listCommand23 = GetListCommand(2, Group.Event, 6);
			listCommand23.Add(CreateGeneric(asset.list, _row++));
			listCommand23.Add(CreateSelect(asset.list, ref _row, 2));
			List<BasicInfo> listCommand24 = GetListCommand(2, Group.Event, 7);
			for (int num25 = 0; num25 < 2; num25++)
			{
				listCommand24.Add(CreateGeneric(asset.list, _row++));
			}
			listCommand24.Add(CreateSelect(asset.list, ref _row, 2));
			List<BasicInfo> listCommand25 = GetListCommand(2, Group.Event, 8);
			listCommand25.Add(CreateGeneric(asset.list, _row++));
			listCommand25.Add(CreateSelect(asset.list, ref _row, 2));
			List<BasicInfo> listCommand26 = GetListCommand(2, Group.Event, 9);
			listCommand26.Add(CreateGeneric(asset.list, _row++));
			listCommand26.Add(CreateSelect(asset.list, ref _row, 2));
			List<BasicInfo> listCommand27 = GetListCommand(2, Group.Event, 11);
			listCommand27.Add(CreateBranch(asset.list, ref _row, 1, _row + 2));
			_row++;
			foreach (KeyValuePair<int, int> item2 in dictionary)
			{
				List<BasicInfo> listCommand28 = GetListCommand(2, Group.Event, item2.Key);
				int _startRow2 = item2.Value;
				listCommand28.Add(CreateBranch(asset.list, ref _startRow2, 1, _startRow2 + 3));
			}
			_row += 10;
			List<BasicInfo> listCommand29 = GetListCommand(2, Group.Talk, 0);
			listCommand29.Add(CreateGeneric(asset.list, ref _row, 1));
			listCommand29.Add(CreateTalk(asset.list, ref _row));
			int num26 = (from v in GetDictionaryGroup(2, Group.Listen)
				orderby v.Key
				select v).Last().Key + 1;
			List<BasicInfo> listCommand30 = GetListCommand(2, Group.Listen, num26++);
			listCommand30.Add(CreateGeneric(asset.list, ref _row, 1));
			List<BasicInfo> listCommand31 = GetListCommand(2, Group.Listen, num26++);
			listCommand31.Add(CreateGeneric(asset.list, ref _row, 3));
			List<BasicInfo> listCommand32 = GetListCommand(2, Group.Event, 3);
			listCommand32.Add(CreateGeneric(asset.list, _row++));
			SelectInfo selectInfo = new SelectInfo(_row, 2);
			selectInfo.introduction = LoadData(asset.list[_row++].list, 8, ref selectInfo.priority, ref selectInfo.conditions);
			string[] array3 = new string[2] { "Yes", "No" };
			for (int num27 = 0; num27 < 2; num27++)
			{
				selectInfo.choice[num27] = LoadChoiceData(asset.list[526 + num27].list, 8);
				selectInfo.choice[num27].choice = array3[num27];
			}
			listCommand32.Add(selectInfo);
			LoadGenericOnly(asset.list, ref _row, 0, Group.Introduction, 1);
			_row--;
			LoadGenericOnly(asset.list, ref _row, 1, Group.Introduction, 1);
			_row--;
			LoadGenericOnly(asset.list, ref _row, 2, Group.Introduction, 1);
			LoadGenericOnly(asset.list, ref _row, 2, Group.Introduction, 1);
			LoadGenericOnly(asset.list, ref _row, 0, Group.Introduction, 1);
			LoadGenericOnly(asset.list, ref _row, 1, Group.Introduction, 2);
			LoadGenericOnly(asset.list, ref _row, 2, Group.Introduction, 2);
			int _row2 = num10;
			LoadEvent(0, 5, 1, 0, asset.list, ref _row2);
			List<BasicInfo> listCommand33 = GetListCommand(0, Group.Event, 10);
			int _startRow3 = num11;
			listCommand33.Add(CreateBranch(asset.list, ref _startRow3, 1, _startRow3 + 2));
			List<BasicInfo> listCommand34 = GetListCommand(2, Group.Event, 10);
			int _startRow4 = num11;
			listCommand34.Add(CreateBranch(asset.list, ref _startRow4, 1, _startRow4 + 2));
			_startRow4++;
			if (!isReplaceEvent10)
			{
				listCommand34.Add(CreateGeneric(asset.list, _startRow4));
			}
			if (isReplaceEvent10)
			{
				List<BasicInfo> listCommand35 = GetListCommand(1, Group.Event, 10);
				int _startRow5 = 589;
				listCommand35.Add(CreateGeneric(asset.list, ref _startRow5, 3));
				List<BasicInfo> listCommand36 = GetListCommand(1, Group.Event, 10);
				int _startRow6 = 589;
				listCommand36.Add(CreateGeneric(asset.list, ref _startRow6, 3));
			}
			AssetBundleManager.UnloadAssetBundle(_bundle, true);
			return true;
		}

		public bool LoadOptionDisplayItems(int _personality)
		{
			string _bundle = string.Empty;
			string _file = string.Empty;
			if (!Singleton<Manager.Communication>.Instance.GetOptionItemsFilePath(_personality, out _bundle, out _file))
			{
				return false;
			}
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(_bundle, _file, typeof(ExcelData));
			if (assetBundleLoadAssetOperation.IsEmpty())
			{
				AssetBundleManager.UnloadAssetBundle(_bundle, true);
				return false;
			}
			ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
			dicOptionDisplayItems = new Dictionary<int, string[]>();
			int skip = 0;
			if (Localize.Translate.Manager.isTranslate)
			{
				skip = 3 * Localize.Translate.Manager.Language;
			}
			foreach (var item in (from p in asset.list.Skip(1)
				select p.list).Select(delegate(List<string> row)
			{
				string[] array = row.Skip(1 + skip).Take(3).ToArray();
				return new
				{
					no = int.Parse(row[0]),
					choices = ((!array.Any()) ? row.Skip(1).Take(3).ToArray() : array)
				};
			}))
			{
				dicOptionDisplayItems[item.no] = item.choices;
			}
			AssetBundleManager.UnloadAssetBundle(_bundle, true);
			return true;
		}

		public List<Program.Transfer> GetEncounterADV(bool _isGeneric)
		{
			BasicInfo basicInfo = GetListCommand(0, Group.Encounter, 0)[_isGeneric ? 1 : 0];
			return CreateGenericADV(basicInfo as GenericInfo);
		}

		public List<Program.Transfer> GetIntroductionADV(ref bool _isEnd, ref int _eventNo, out ChangeValueAbstractInfo _outInfo, bool _check = false)
		{
			int num = Mathf.Max(0, GetStage());
			BasicInfo basicInfo = null;
			foreach (BasicInfo item in from v in GetListCommand(num, Group.Introduction, 0)
				where !heroine.talkIntro.Contains(v.conditions)
				orderby v.priority descending
				select v)
			{
				switch (item.conditions)
				{
				case 0:
				{
					int[] source = new int[4] { 14, 15, 16, 19 };
					if (source.Contains(mapNo) && !isOtherPeople)
					{
						basicInfo = item;
					}
					break;
				}
				case 1:
				{
					int[] source2 = new int[4] { 14, 15, 16, 19 };
					if (source2.Contains(mapNo) && isOtherPeople)
					{
						basicInfo = item;
						if (num < 2)
						{
							_isEnd = true;
							Singleton<Game>.Instance.rankSaveData.angerToiletCount++;
						}
					}
					break;
				}
				case 2:
					if (isOtherPeople || mapNo != 46)
					{
						continue;
					}
					basicInfo = item;
					break;
				case 3:
					if ((num >= 1 && !isOtherPeople) || mapNo != 46)
					{
						continue;
					}
					basicInfo = item;
					if (num < 2)
					{
						_isEnd = true;
						Singleton<Game>.Instance.rankSaveData.angerLockerCount++;
					}
					break;
				case 4:
					if (heroine.isAnger && heroine.talkTime <= 0)
					{
						basicInfo = item;
						_isEnd = true;
					}
					break;
				case 5:
					if (heroine.isAnger)
					{
						basicInfo = item;
					}
					break;
				case 6:
					if (heroine.talkTime <= 0)
					{
						basicInfo = item;
						_isEnd = true;
					}
					break;
				case 7:
					if (state == 2)
					{
						basicInfo = item;
						_isEnd = true;
					}
					break;
				case 31:
					if (isOtherPeople || mapNo != 45)
					{
						continue;
					}
					basicInfo = item;
					break;
				case 32:
					if ((num >= 1 && !isOtherPeople) || mapNo != 45)
					{
						continue;
					}
					basicInfo = item;
					if (num < 2)
					{
						_isEnd = true;
						Singleton<Game>.Instance.rankSaveData.angerShowerCount++;
					}
					break;
				case 33:
					if (isAttack && (heroine.talkEvent.Contains(89) | heroine.talkEvent.Contains(90)))
					{
						basicInfo = item;
					}
					break;
				case 30:
					if (isAttack)
					{
						basicInfo = item;
					}
					break;
				case 8:
					if (isOtherPeople || heroine.chaCtrl.GetNowClothesType() % 2 != 1)
					{
						continue;
					}
					basicInfo = item;
					heroine.talkIntro.Add(8);
					break;
				case 9:
					if (!isOtherPeople || heroine.chaCtrl.GetNowClothesType() % 2 != 1)
					{
						continue;
					}
					basicInfo = item;
					break;
				case 10:
					if (isOtherPeople || heroine.chaCtrl.GetNowClothesType() != 2)
					{
						continue;
					}
					basicInfo = item;
					break;
				case 11:
					if (!isOtherPeople || heroine.chaCtrl.GetNowClothesType() != 2)
					{
						continue;
					}
					basicInfo = item;
					break;
				case 12:
				case 13:
				case 14:
				{
					string bestPlace = Singleton<Manager.Communication>.Instance.GetBestPlace(heroine.FixCharaIDOrPersonality, num, item.conditions - 12);
					if (!(IsBestPlaceNameConvert(bestPlace) ? placeNames.Select(BestPlaceNameConvert).Contains(bestPlace) : placeNames.Contains(bestPlace)))
					{
						continue;
					}
					basicInfo = item;
					break;
				}
				case 15:
					if (isChase)
					{
						basicInfo = item;
					}
					break;
				case 16:
					if (femaleOnly)
					{
						_isEnd = true;
						basicInfo = item;
					}
					break;
				case 17:
					if (heroine.isDate)
					{
						basicInfo = item;
					}
					break;
				case 19:
					if (heroine.favor >= 50)
					{
						basicInfo = item;
					}
					break;
				case 20:
					if (heroine.lewdness >= 50)
					{
						basicInfo = item;
					}
					break;
				case 21:
					if (cycleType != Cycle.Type.StaffTime)
					{
						continue;
					}
					if (isSports)
					{
						basicInfo = item;
					}
					break;
				case 22:
					if (cycleType != Cycle.Type.StaffTime)
					{
						continue;
					}
					if (!isSports)
					{
						basicInfo = item;
					}
					break;
				case 23:
					if (cycleType == Cycle.Type.StaffTime)
					{
						basicInfo = item;
					}
					break;
				case 24:
					if (cycleType == Cycle.Type.LunchTime)
					{
						basicInfo = item;
					}
					break;
				case 25:
					if (cycleType == Cycle.Type.AfterSchool)
					{
						basicInfo = item;
					}
					break;
				case 26:
					if (!isNotice)
					{
						basicInfo = item;
					}
					break;
				default:
					basicInfo = item;
					break;
				case 18:
					break;
				}
				if (basicInfo != null)
				{
					break;
				}
			}
			_eventNo = basicInfo.conditions;
			if (!_check && MathfEx.RangeEqualOn(8, _eventNo, 25) && _eventNo != 16)
			{
				heroine.talkIntro.Add(_eventNo);
			}
			_outInfo = new ChangeValueInfo();
			return CreateGenericADV(basicInfo as GenericInfo, _outInfo as ChangeValueInfo, !_isEnd);
		}

		public int[] ConfirmEvent()
		{
			HashSet<int> hashSet = new HashSet<int>();
			int stage = GetStage();
			Func<int, bool, bool>[] array = new Func<int, bool, bool>[3] { ConfirmEvent0, ConfirmEvent1, ConfirmEvent2 };
			foreach (KeyValuePair<int, List<BasicInfo>> item in GetDictionaryGroup(stage, Group.Event))
			{
				foreach (BasicInfo item2 in item.Value.OrderByDescending((BasicInfo o) => o.priority))
				{
					if (array[stage](item2.conditions, false))
					{
						hashSet.Add(item.Key);
					}
					if (hashSet.Contains(item.Key))
					{
						break;
					}
				}
			}
			return hashSet.ToArray();
		}

		public List<Program.Transfer> GetADV(out ChangeValueAbstractInfo _outInfo, Group _group, int _command = -1)
		{
			int stage = GetStage();
			List<BasicInfo> list = null;
			if (_command != -1)
			{
				list = GetListCommand(stage, _group, _command);
			}
			else
			{
				Dictionary<int, List<BasicInfo>> dic = GetDictionaryGroup(stage, _group);
				if (_group == Group.Listen)
				{
					Dictionary<int, List<BasicInfo>>.KeyCollection keys = dic.Keys;
					int num = (from v in keys
						where dic[v].Any((BasicInfo b) => ConditionDeterminationListen(b.conditions))
						where !queueListen.Contains(v)
						select v into i
						orderby Guid.NewGuid()
						select i).First();
					queueListen.Enqueue(num);
					if (queueListen.Count > 2)
					{
						queueListen.Dequeue();
					}
					list = dic[num];
				}
				else
				{
					list = dic[dic.Keys.OrderBy((int i) => Guid.NewGuid()).First()];
				}
			}
			Func<int, bool> func = null;
			switch (_group)
			{
			case Group.Talk:
				func = ConditionDeterminationSpeak;
				break;
			case Group.Listen:
				func = ConditionDeterminationListen;
				break;
			default:
				func = delegate(int _c)
				{
					switch (_c)
					{
					case 0:
						return heroine.isAnger;
					default:
						return true;
					}
				};
				break;
			}
			List<BasicInfo> list2 = new List<BasicInfo>();
			foreach (BasicInfo item in list.OrderByDescending((BasicInfo v) => v.priority))
			{
				if (!list2.IsNullOrEmpty() && list2[0].priority > item.priority)
				{
					break;
				}
				if (func(item.conditions))
				{
					list2.Add(item);
				}
			}
			if (list2.Count > 1 && prevInfo != null)
			{
				list2.Remove(prevInfo);
			}
			BasicInfo basicInfo = (prevInfo = list2.OrderBy((BasicInfo i) => Guid.NewGuid()).First());
			switch (basicInfo.GetType().Name)
			{
			case "GenericInfo":
				_outInfo = new ChangeValueInfo();
				return CreateGenericADV(basicInfo as GenericInfo, _outInfo as ChangeValueInfo);
			case "BranchInfo":
				_outInfo = new ChangeValueInfo();
				return CreateBranchADV(basicInfo as BranchInfo, _outInfo as ChangeValueInfo);
			case "SelectInfo":
				_outInfo = new ChangeValueSelectInfo();
				return CreateSelectADV(basicInfo as SelectInfo, _outInfo as ChangeValueSelectInfo);
			default:
				_outInfo = null;
				return null;
			}
		}

		public List<Program.Transfer> GetEventADV(int _command, ref int _derive, ref bool _first)
		{
			_first = false;
			int stage = GetStage();
			switch (stage)
			{
			case 0:
				switch (_command)
				{
				case 5:
					return GetStage1Branch0(0, _command, ref _derive);
				case 10:
				{
					Manager.Communication.ProbabilityValue communicationConndition7 = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(0, 422);
					return GetStaffBranch((int)communicationConndition7.min, (int)communicationConndition7.max, ref _derive);
				}
				default:
					return GetFirstEventADV(stage, _command, ref _derive, ref _first);
				}
			case 1:
				switch (_command)
				{
				case 1:
				{
					BranchInfo branchInfo2 = GetListCommand(1, Group.Event, _command)[heroine.talkEvent.Contains(7) ? 1 : 0] as BranchInfo;
					Manager.Communication.ProbabilityValue communicationConndition5 = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(1, branchInfo2.conditions);
					int num3 = (int)communicationConndition5.Get(heroine.favor);
					if (heroine.isStaff)
					{
						num3 = (int)Mathf.Clamp((float)num3 + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.confession, 0f, 100f);
						if (Singleton<Game>.Instance.HeroineList.Where((SaveData.Heroine h) => h != heroine).Any((SaveData.Heroine h) => h.isStaff && h.isGirlfriend))
						{
							num3 = (int)Mathf.Clamp((float)num3 + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.anotherConfession, 0f, 100f);
						}
					}
					bool flag5 = RandomBranch(num3, 100 - num3) == 0;
					List<Program.Transfer> result4 = CreateBranchADV(branchInfo2, flag5);
					if (flag5)
					{
						heroine.talkEvent.Add(7);
						heroine.isGirlfriend = true;
						if (!heroine.isFirstGirlfriend)
						{
							heroine.isFirstGirlfriend = true;
							Singleton<Game>.Instance.rankSaveData.girlfriendCount++;
						}
						Singleton<Game>.Instance.Player.girlfriendedCnt++;
					}
					else
					{
						_derive = 0;
					}
					return result4;
				}
				case 3:
				{
					BranchInfo branchInfo3 = null;
					bool flag6 = isSecondH;
					foreach (BasicInfo item in from o in GetListCommand(1, Group.Event, _command)
						orderby o.priority descending
						select o)
					{
						switch (item.conditions)
						{
						case 404:
							if (!flag6)
							{
								branchInfo3 = item as BranchInfo;
							}
							break;
						case 405:
							if (flag6)
							{
								branchInfo3 = item as BranchInfo;
							}
							break;
						case 406:
							if (flag6 && isOtherPeople)
							{
								branchInfo3 = item as BranchInfo;
							}
							break;
						case 407:
							if (flag6 && exposure)
							{
								branchInfo3 = item as BranchInfo;
							}
							break;
						}
						if (branchInfo3 != null)
						{
							break;
						}
					}
					Manager.Communication.ProbabilityValue communicationConndition6 = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(1, branchInfo3.conditions);
					int num4 = (int)communicationConndition6.Get(heroine.lewdness);
					if (heroine.isStaff)
					{
						num4 = (int)Mathf.Clamp((float)num4 + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.h[1], 0f, 100f);
					}
					bool flag7 = RandomBranch(num4, 100 - num4) == 0;
					List<Program.Transfer> result5 = CreateBranchADV(branchInfo3, flag7);
					if (flag7)
					{
						heroine.talkEvent.Add(8);
						heroine.talkEvent.Add(12);
						_derive = 2;
					}
					return result5;
				}
				case 7:
				{
					BranchInfo branchInfo = GetListCommand(1, Group.Event, _command)[heroine.talkEvent.Contains(9) ? 1 : 0] as BranchInfo;
					Manager.Communication.ProbabilityValue communicationConndition3 = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(1, branchInfo.conditions);
					int num2 = (int)communicationConndition3.Get(heroine.favor);
					bool flag4 = RandomBranch(num2, 100 - num2) == 0;
					List<Program.Transfer> result3 = CreateBranchADV(branchInfo, flag4);
					if (flag4)
					{
						heroine.talkEvent.Add(9);
						heroine.isDate = true;
						Singleton<Game>.Instance.actScene.Cycle.dateHeroine = heroine;
					}
					return result3;
				}
				case 4:
					_first = !heroine.talkEvent.Contains(79);
					return GetStage1Branch0(1, _command, ref _derive);
				case 5:
					return GetStage1Branch0(1, _command, ref _derive);
				case 6:
					return (!heroine.talkEvent.Contains(3)) ? GetFirstEventADV(stage, _command, ref _derive, ref _first) : GetStage1Branch1(_command, ref _derive);
				case 8:
					return (!heroine.talkEvent.Contains(4)) ? GetFirstEventADV(stage, _command, ref _derive, ref _first) : GetStage1Branch0(1, _command, ref _derive);
				case 9:
					return (!heroine.talkEvent.Contains(5)) ? GetFirstEventADV(stage, _command, ref _derive, ref _first) : GetStage1Branch0(1, _command, ref _derive);
				case 10:
				{
					Manager.Communication.ProbabilityValue communicationConndition4 = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(1, 422);
					return GetStaffBranch((int)communicationConndition4.min, (int)communicationConndition4.max, ref _derive);
				}
				case 11:
					return (!heroine.talkEvent.Contains(6)) ? GetFirstEventADV(stage, _command, ref _derive, ref _first) : GetStage1Branch1(_command, ref _derive);
				}
				break;
			case 2:
				switch (_command)
				{
				case 2:
				{
					bool isAnger = heroine.isAnger;
					heroine.isGirlfriend = false;
					heroine.isDate = false;
					heroine.isAnger = true;
					heroine.anger = 100;
					heroine.talkTime = 0;
					_derive = 8;
					return CreateGenericADV(GetListCommand(2, Group.Event, _command)[(!isAnger) ? 1 : 0] as GenericInfo);
				}
				case 3:
				{
					bool flag2 = isSecondH;
					foreach (BasicInfo item2 in from o in GetListCommand(2, Group.Event, _command)
						orderby o.priority descending
						select o)
					{
						Manager.Communication.ProbabilityValue communicationConndition2 = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(2, item2.conditions);
						if (communicationConndition2 == null)
						{
							continue;
						}
						int num = (int)communicationConndition2.Get(heroine.lewdness);
						if (heroine.isStaff)
						{
							num = (int)Mathf.Clamp((float)num + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.h[2], 0f, 100f);
						}
						bool flag3 = RandomBranch(num, 100 - num) == 0;
						switch (item2.conditions)
						{
						case 304:
							if (flag2)
							{
								break;
							}
							heroine.talkEvent.Add(8);
							heroine.talkEvent.Add(12);
							_derive = 2;
							return CreateGenericADV(item2 as GenericInfo);
						case 305:
							if (!flag2)
							{
								break;
							}
							_derive = 2;
							return CreateGenericADV(item2 as GenericInfo);
						case 307:
							if (!flag2 || !isOtherPeople)
							{
								break;
							}
							if (flag3)
							{
								_derive = 2;
							}
							return CreateBranchADV(item2 as BranchInfo, flag3);
						case 308:
							if (!flag2 || !exposure)
							{
								break;
							}
							if (flag3)
							{
								_derive = 2;
							}
							return CreateBranchADV(item2 as BranchInfo, flag3);
						}
					}
					return null;
				}
				case 7:
				{
					GenericInfo info2 = GetListCommand(2, Group.Event, _command)[heroine.talkEvent.Contains(13) ? 1 : 0] as GenericInfo;
					List<Program.Transfer> result2 = CreateGenericADV(info2);
					heroine.talkEvent.Add(13);
					heroine.isDate = true;
					Singleton<Game>.Instance.actScene.Cycle.dateHeroine = heroine;
					return result2;
				}
				case 4:
					_first = !heroine.talkEvent.Contains(79);
					return GetStage2Generic(_command, ref _derive);
				case 5:
					return GetStage2Generic(_command, ref _derive);
				case 6:
					if (heroine.talkEvent.Contains(3))
					{
						_derive = 5;
						return CreateGenericADV(GetListCommand(2, Group.Event, _command)[0] as GenericInfo);
					}
					return GetFirstEventADV(stage, _command, ref _derive, ref _first);
				case 8:
					return (!heroine.talkEvent.Contains(4)) ? GetFirstEventADV(stage, _command, ref _derive, ref _first) : GetStage2Generic(_command, ref _derive);
				case 9:
					return (!heroine.talkEvent.Contains(5)) ? GetFirstEventADV(stage, _command, ref _derive, ref _first) : GetStage2Generic(_command, ref _derive);
				case 10:
				{
					Manager.Communication.ProbabilityValue communicationConndition = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(2, 422);
					return GetStaffBranch((int)communicationConndition.min, (int)communicationConndition.max, ref _derive);
				}
				case 11:
					if (heroine.talkEvent.Contains(6))
					{
						BranchInfo info = GetListCommand(2, Group.Event, _command)[0] as BranchInfo;
						bool flag = !heroine.isAnger;
						List<Program.Transfer> result = CreateBranchADV(info, flag);
						if (flag)
						{
							_derive = 1;
						}
						return result;
					}
					return GetFirstEventADV(stage, _command, ref _derive, ref _first);
				}
				break;
			}
			return null;
		}

		public List<Program.Transfer> GetEventFromHeroineADV(out ChangeValueAbstractInfo _outInfo, ref int _derive)
		{
			int stage = GetStage();
			Dictionary<int, BasicInfo> dictionary = new Dictionary<int, BasicInfo>();
			Func<int, bool, bool>[] array = new Func<int, bool, bool>[3] { ConfirmEvent0, ConfirmEvent1, ConfirmEvent2 };
			foreach (KeyValuePair<int, List<BasicInfo>> item in GetDictionaryGroup(stage, Group.Event))
			{
				foreach (BasicInfo item2 in item.Value.OrderByDescending((BasicInfo o) => o.priority))
				{
					if (array[stage](item2.conditions, true))
					{
						dictionary.Add(item.Key, item2);
					}
					if (dictionary.ContainsKey(item.Key))
					{
						break;
					}
				}
			}
			if (dictionary.Count == 0)
			{
				_outInfo = null;
				return null;
			}
			BasicInfo value = dictionary.ElementAt(UnityEngine.Random.Range(0, dictionary.Count)).Value;
			return GetHeroineADV(value, stage, out _outInfo, ref _derive);
		}

		public List<Program.Transfer> GetEventFromHeroineADV(int _command, out ChangeValueAbstractInfo _outInfo, ref int _derive)
		{
			int stage = GetStage();
			int[,] con = new int[4, 3]
			{
				{ 0, 0, 0 },
				{ 0, 0, 0 },
				{ 0, 0, 0 },
				{ 0, 102, 202 }
			};
			if (_command == 3)
			{
				stage = Mathf.Clamp(stage, 1, 2);
			}
			return GetHeroineADV(GetDictionaryGroup(stage, Group.Event)[_command].Find((BasicInfo v) => v.conditions == con[_command, stage]), stage, out _outInfo, ref _derive);
		}

		public List<Program.Transfer> GetInterludeADV(ChangeValueAbstractInfo _cvInfo)
		{
			int stage = GetStage();
			BasicInfo basicInfo = null;
			foreach (BasicInfo item in from v in GetListCommand(stage, Group.Interlude, 0)
				orderby v.priority descending
				select v)
			{
				switch (item.conditions)
				{
				case 1:
					if (!heroine.isAnger && _cvInfo != null && _cvInfo.lewdness > 0 && Utils.ProbabilityCalclator.DetectFromPercent(30f))
					{
						basicInfo = item;
					}
					break;
				case 0:
					if (!heroine.isAnger && _cvInfo != null && _cvInfo.favor > 0 && Utils.ProbabilityCalclator.DetectFromPercent(30f))
					{
						basicInfo = item;
					}
					break;
				default:
					if (!heroine.isAnger && Utils.ProbabilityCalclator.DetectFromPercent(30f))
					{
						basicInfo = item;
					}
					break;
				}
				if (basicInfo != null)
				{
					break;
				}
			}
			if (basicInfo != null)
			{
				return CreateGenericADV(basicInfo as GenericInfo);
			}
			return null;
		}

		public List<Program.Transfer> GetLeaveAloneADV()
		{
			int stage = GetStage();
			BasicInfo basicInfo = null;
			foreach (BasicInfo item in from v in GetListCommand(stage, Group.LeaveAlone, 0)
				orderby v.priority descending
				select v)
			{
				if (item.conditions == 0)
				{
					if (heroine.isAnger)
					{
						basicInfo = item;
					}
				}
				else
				{
					basicInfo = item;
				}
				if (basicInfo != null)
				{
					break;
				}
			}
			return CreateGenericADV(basicInfo as GenericInfo);
		}

		public List<Program.Transfer> GetEndConversationADV(bool _oldAnger, bool _endSelect, out int _endNo)
		{
			int stage = GetStage();
			BasicInfo basicInfo = null;
			bool flag = _endSelect | (heroine.talkTime <= 0);
			_endNo = -1;
			foreach (BasicInfo item in from v in GetListCommand(stage, Group.EndConversation, 0)
				orderby v.priority descending
				select v)
			{
				switch (item.conditions)
				{
				case 0:
					if (!_oldAnger && heroine.isAnger)
					{
						basicInfo = item;
						_endNo = 0;
					}
					break;
				case 1:
					if (flag && heroine.isAnger)
					{
						basicInfo = item;
						_endNo = 1;
					}
					break;
				case 2:
					if (flag && heroine.favor >= 50)
					{
						basicInfo = item;
						_endNo = 2;
					}
					break;
				case 3:
					if (flag && heroine.lewdness >= 50)
					{
						basicInfo = item;
						_endNo = 3;
					}
					break;
				case 4:
					if (_endSelect && Mathf.InverseLerp(0f, heroine.talkTimeMax, heroine.talkTime) >= 0.5f)
					{
						basicInfo = item;
						_endNo = 4;
					}
					break;
				case 5:
					if (_endSelect && Mathf.InverseLerp(0f, heroine.talkTimeMax, heroine.talkTime) < 0.5f)
					{
						basicInfo = item;
						_endNo = 5;
					}
					break;
				case 10:
					if (heroine.favor >= 100)
					{
						basicInfo = item;
						_endNo = 10;
					}
					break;
				default:
					if (heroine.talkTime <= 0)
					{
						basicInfo = item;
					}
					break;
				}
				if (basicInfo != null)
				{
					break;
				}
			}
			if (basicInfo != null)
			{
				return CreateGenericADV(basicInfo as GenericInfo);
			}
			return null;
		}

		public TouchReactionInfo GetTouchVoice(bool _touch, int _parts, int _stage, out ChangeValueAbstractInfo _cvInfo)
		{
			TouchReactionInfo touchReactionInfo = new TouchReactionInfo();
			int stage = GetStage();
			bool isAnger = heroine.isAnger;
			GenericInfo genericInfo = dicTalkInfo[stage][(!_touch) ? Group.See : Group.Touch][(!isAnger) ? _parts : 0][(!isAnger) ? _stage : 3] as GenericInfo;
			TalkData talkData = genericInfo.talk[0];
			touchReactionInfo.assetbundle = talkData.assetbundle;
			touchReactionInfo.file = talkData.file;
			touchReactionInfo.facialExpression = talkData.facialExpression;
			touchReactionInfo.eyesLook = talkData.eyeLook;
			touchReactionInfo.neckLook = talkData.neckLook;
			touchReactionInfo.pose = talkData.pose;
			_cvInfo = new ChangeValueInfo(talkData.favor, talkData.lewdness, talkData.anger);
			return touchReactionInfo;
		}

		public LeaveAloneReactionInfo GetLeaveAloneVoice()
		{
			BasicInfo basicInfo = null;
			foreach (BasicInfo item in from v in GetListCommand(GetStage(), Group.LeaveAlone, 0)
				orderby v.priority descending
				select v)
			{
				basicInfo = ((item.conditions != 0) ? item : ((!heroine.isAnger) ? null : item));
				if (basicInfo != null)
				{
					break;
				}
			}
			return new LeaveAloneReactionInfo(basicInfo as GenericInfo);
		}

		public InterludeReactionInfo GetInterludeVoice(ChangeValueAbstractInfo _cvInfo)
		{
			BasicInfo basicInfo = null;
			foreach (BasicInfo item in from v in GetListCommand(GetStage(), Group.Interlude, 0)
				orderby v.priority descending
				select v)
			{
				switch (item.conditions)
				{
				case 1:
					if (!heroine.isAnger && _cvInfo != null && _cvInfo.lewdness > 0 && Utils.ProbabilityCalclator.DetectFromPercent(30f))
					{
						basicInfo = item;
					}
					break;
				case 0:
					if (!heroine.isAnger && _cvInfo != null && _cvInfo.favor > 0 && Utils.ProbabilityCalclator.DetectFromPercent(30f))
					{
						basicInfo = item;
					}
					break;
				default:
					if (!heroine.isAnger && Utils.ProbabilityCalclator.DetectFromPercent(30f))
					{
						basicInfo = item;
					}
					break;
				}
				if (basicInfo != null)
				{
					break;
				}
			}
			return new InterludeReactionInfo(basicInfo as GenericInfo);
		}

		public List<Program.Transfer> CreateGenericADV(GenericInfo _info, bool _close = true, bool _motion = true)
		{
			List<Program.Transfer> list = CreateNewList();
			TalkData[] talk = _info.talk;
			foreach (TalkData talk2 in talk)
			{
				AddTransfer(list, talk2, _motion);
			}
			if (_close)
			{
				list.Add(Program.Transfer.Close());
			}
			return list;
		}

		public List<Program.Transfer> CreateSelectADV(SelectInfo _info, int _select)
		{
			List<Program.Transfer> list = CreateNewList();
			AddTransferHeroinText(list, _info.introduction);
			string[] choice = null;
			bool b = dicOptionDisplayItems.TryGetValue(_info.conditions, out choice);
			int num = _info.choice.Length;
			if (num == 2 || num == 3)
			{
				var array = (from i in Enumerable.Range(0, num)
					select new
					{
						repSrc = "sel" + i,
						tag = "tag" + i,
						repDst = ((!b) ? _info.choice[i].choice : choice[i])
					}).ToArray();
				var array2 = array;
				foreach (var anon in array2)
				{
					list.Add(Program.Transfer.Create(false, Command.Replace, anon.repSrc, anon.repDst));
				}
				list.Add(Program.Transfer.Create(false, Command.Choice, new string[1] { bool.TrueString }.Concat(array.Select(x => string.Format("[{0}],{1}", x.repSrc, x.tag))).ToArray()));
			}
			for (int k = 0; k < num; k++)
			{
				TalkData talk = ((_info.failure != null) ? ((k != _select) ? _info.failure : _info.choice[k]) : _info.choice[k]);
				list.Add(Program.Transfer.Create(false, Command.Tag, "tag" + k));
				list.Add(Program.Transfer.Create(false, Command.Calc, "Result", "=", k.ToString()));
				AddTransferHeroinText(list, talk);
				list.Add(Program.Transfer.Close());
			}
			return list;
		}

		public List<Program.Transfer> CreateBranchADV(BranchInfo _info, int _branch)
		{
			List<Program.Transfer> list = CreateNewList();
			AddTransfer(list, _info.introduction);
			AddTransfer(list, _info[_branch]);
			list.Add(Program.Transfer.Close());
			return list;
		}

		private TalkData LoadData(List<string> _src, int _col)
		{
			int _priority = 0;
			int _conditions = -1;
			return LoadData(_src, _col, ref _priority, ref _conditions);
		}

		private TalkData LoadData(List<string> _src, int _col, ref int _priority, ref int _conditions)
		{
			int result = 0;
			_priority = (int.TryParse(_src[_col++], out result) ? result : 0);
			_conditions = ((!int.TryParse(_src[_col++], out result)) ? (-1) : result);
			TalkData talkData = new TalkData();
			LoadTalkData(talkData, _src, _col);
			return talkData;
		}

		private void LoadTalkData(TalkData _data, List<string> _src, int _col)
		{
			int result = 0;
			_data.favor = (int.TryParse(_src[_col++], out result) ? result : 0);
			_data.lewdness = (int.TryParse(_src[_col++], out result) ? result : 0);
			_data.anger = (int.TryParse(_src[_col++], out result) ? result : 0);
			_data.assetbundle = _src[_col++];
			_data.file = _src[_col++];
			_data.text = _src[_col++];
			_src.SafeProc(_col++, delegate(string s)
			{
				_data.pose = s;
			});
			_src.SafeProc(_col++, delegate(string s)
			{
				_data.facialExpression = s;
			});
			_src.SafeProc(_col++, delegate(string s)
			{
				_data.eyeLook = int.Parse(s);
			});
			_src.SafeProc(_col++, delegate(string s)
			{
				_data.neckLook = int.Parse(s);
			});
			if (Localize.Translate.Manager.isTranslate)
			{
				int num = Localize.Translate.Manager.Language - 1;
				_src.SafeProc(_col + num, delegate(string s)
				{
					_data.text = s;
				});
			}
		}

		private Dictionary<int, List<BasicInfo>> GetDictionaryGroup(int _stage, Group _group)
		{
			if (!dicTalkInfo.ContainsKey(_stage))
			{
				dicTalkInfo.Add(_stage, new Dictionary<Group, Dictionary<int, List<BasicInfo>>>());
			}
			Dictionary<Group, Dictionary<int, List<BasicInfo>>> dictionary = dicTalkInfo[_stage];
			if (!dictionary.ContainsKey(_group))
			{
				dictionary.Add(_group, new Dictionary<int, List<BasicInfo>>());
			}
			return dictionary[_group];
		}

		private List<BasicInfo> GetListCommand(int _stage, Group _group, int _command)
		{
			Dictionary<int, List<BasicInfo>> dictionaryGroup = GetDictionaryGroup(_stage, _group);
			if (!dictionaryGroup.ContainsKey(_command))
			{
				dictionaryGroup.Add(_command, new List<BasicInfo>());
			}
			return dictionaryGroup[_command];
		}

		private BranchInfo CreateBranch(List<ExcelData.Param> _list, ref int _startRow, int _successNum, int _failureRow)
		{
			BranchInfo branchInfo = new BranchInfo(_startRow);
			branchInfo.success = new TalkData[_successNum];
			BranchInfo branchInfo2 = branchInfo;
			branchInfo2.introduction = LoadData(_list[_startRow++].list, 8, ref branchInfo2.priority, ref branchInfo2.conditions);
			for (int i = 0; i < _successNum; i++)
			{
				branchInfo2.success[i] = LoadData(_list[_startRow++].list, 8);
			}
			branchInfo2.failure = LoadData(_list[_failureRow].list, 8);
			return branchInfo2;
		}

		private BranchInfo CreateTalk(List<ExcelData.Param> _list, ref int _startRow, int _noInterestedRow = -1)
		{
			BranchInfo branchInfo = new BranchInfo(_startRow);
			branchInfo.success = new TalkData[1];
			BranchInfo branchInfo2 = branchInfo;
			branchInfo2.introduction = LoadData(_list[_startRow++].list, 8, ref branchInfo2.priority, ref branchInfo2.conditions);
			branchInfo2.success[0] = LoadData(_list[_startRow++].list, 8);
			branchInfo2.failure = LoadData(_list[_startRow++].list, 8);
			if (_noInterestedRow != -1)
			{
				branchInfo2.noInterested = LoadData(_list[_noInterestedRow].list, 8);
			}
			return branchInfo2;
		}

		private void LoadGenericOnly(List<ExcelData.Param> _list, ref int _row, int _stage, Group _group, int _num)
		{
			List<BasicInfo> listCommand = GetListCommand(_stage, _group, 0);
			int _priority = 0;
			int _conditions = -1;
			for (int i = 0; i < _num; i++)
			{
				listCommand.Add(new GenericInfo(_row, LoadData(_list[_row++].list, 8, ref _priority, ref _conditions))
				{
					priority = _priority,
					conditions = _conditions
				});
			}
		}

		private GenericInfo CreateGeneric(List<ExcelData.Param> _list, ref int _startRow, int _num)
		{
			GenericInfo genericInfo = new GenericInfo(_startRow, _num);
			for (int i = 0; i < _num; i++)
			{
				genericInfo.talk[i] = ((i != 0) ? LoadData(_list[_startRow++].list, 8) : LoadData(_list[_startRow++].list, 8, ref genericInfo.priority, ref genericInfo.conditions));
			}
			return genericInfo;
		}

		private GenericInfo CreateGeneric(List<ExcelData.Param> _list, int _row)
		{
			GenericInfo genericInfo = new GenericInfo(_row, 1);
			genericInfo.talk[0] = LoadData(_list[_row].list, 8, ref genericInfo.priority, ref genericInfo.conditions);
			return genericInfo;
		}

		private SelectInfo CreateSelect(List<ExcelData.Param> _list, ref int _startRow, int _num, int _failureRow = -1)
		{
			SelectInfo selectInfo = new SelectInfo(_startRow, _num);
			selectInfo.introduction = LoadData(_list[_startRow++].list, 8, ref selectInfo.priority, ref selectInfo.conditions);
			for (int i = 0; i < _num; i++)
			{
				selectInfo.choice[i] = LoadChoiceData(_list[_startRow++].list, 8);
			}
			if (_failureRow != -1)
			{
				selectInfo.failure = LoadData(_list[_failureRow].list, 8);
			}
			return selectInfo;
		}

		private ChoiceInfo LoadChoiceData(List<string> _src, int _col)
		{
			ChoiceInfo choiceInfo = new ChoiceInfo();
			_col += 2;
			LoadTalkData(choiceInfo, _src, _col);
			return choiceInfo;
		}

		private void LoadTalkAndListenAnger(int _stage, List<ExcelData.Param> _list, ref int _row)
		{
			foreach (KeyValuePair<int, List<BasicInfo>> item in GetDictionaryGroup(_stage, Group.Talk))
			{
				item.Value.Add(CreateGeneric(_list, _row));
			}
			foreach (KeyValuePair<int, List<BasicInfo>> item2 in GetDictionaryGroup(_stage, Group.Listen))
			{
				item2.Value.Add(CreateGeneric(_list, _row));
			}
			_row++;
		}

		private void LoadTouch(int _stage, Group _group, int _num, List<ExcelData.Param> _list, ref int _row)
		{
			int row = _row + 3 * _num;
			for (int i = 0; i < _num; i++)
			{
				List<BasicInfo> listCommand = GetListCommand(_stage, _group, i);
				for (int j = 0; j < 3; j++)
				{
					listCommand.Add(CreateGeneric(_list, _row++));
				}
				listCommand.Add(CreateGeneric(_list, row));
			}
			_row++;
		}

		private void LoadEvent(int _stage, int _command, int _b, int _s, List<ExcelData.Param> _list, ref int _row)
		{
			List<BasicInfo> listCommand = GetListCommand(_stage, Group.Event, _command);
			for (int i = 0; i < _b; i++)
			{
				listCommand.Add(CreateBranch(_list, ref _row, 1, _row + 2));
				_row++;
			}
			for (int j = 0; j < _s; j++)
			{
				listCommand.Add(CreateSelect(_list, ref _row, 2));
			}
		}

		private List<Program.Transfer> CreateSelectADV(SelectInfo _info, ChangeValueSelectInfo _cvsInfo)
		{
			List<Program.Transfer> list = CreateNewList();
			AddTransferHeroinText(list, _info.introduction);
			string[] choice = null;
			bool b = dicOptionDisplayItems.TryGetValue(_info.conditions, out choice);
			int num = _info.choice.Length;
			if (num == 2 || num == 3)
			{
				var array = (from i in Enumerable.Range(0, num)
					select new
					{
						repSrc = "sel" + i,
						tag = "tag" + i,
						repDst = ((!b) ? _info.choice[i].choice : choice[i])
					}).ToArray();
				var array2 = array;
				foreach (var anon in array2)
				{
					list.Add(Program.Transfer.Create(false, Command.Replace, anon.repSrc, anon.repDst));
				}
				list.Add(Program.Transfer.Create(false, Command.Choice, new string[1] { bool.TrueString }.Concat(array.Select(x => string.Format("[{0}],{1}", x.repSrc, x.tag))).ToArray()));
			}
			_cvsInfo.Init(num);
			int num3 = (_cvsInfo.success = CheckSelectConditions(_info.conditions));
			for (int k = 0; k < num; k++)
			{
				TalkData talkData = ((_info.failure != null) ? ((k != num3) ? _info.failure : _info.choice[k]) : _info.choice[k]);
				list.Add(Program.Transfer.Create(false, Command.Tag, "tag" + k));
				list.Add(Program.Transfer.Create(false, Command.Calc, "Result", "=", k.ToString()));
				AddTransferHeroinText(list, talkData);
				list.Add(Program.Transfer.Close());
				_cvsInfo[k, 0] = talkData.favor;
				_cvsInfo[k, 1] = talkData.lewdness;
				_cvsInfo[k, 2] = talkData.anger;
			}
			return list;
		}

		private int CheckSelectConditions(int _conditions)
		{
			ChaFileParameter.Awnser awnser = heroine.chaCtrl.GetAwnser();
			int result = 0;
			switch (_conditions)
			{
			case 2:
				result = ((!awnser.animal) ? 1 : 0);
				break;
			case 3:
				result = ((!awnser.eat) ? 1 : 0);
				break;
			case 4:
				result = ((!awnser.cook) ? 1 : 0);
				break;
			case 5:
				result = ((!awnser.exercise) ? 1 : 0);
				break;
			case 6:
				result = ((!awnser.study) ? 1 : 0);
				break;
			case 7:
				result = ((!awnser.fashionable) ? 1 : 0);
				break;
			case 9:
				result = ((!awnser.blackCoffee) ? 1 : 0);
				break;
			case 10:
				result = ((!awnser.spicy) ? 1 : 0);
				break;
			case 11:
				result = ((!awnser.sweet) ? 1 : 0);
				break;
			case 12:
			{
				int[] array3 = new int[3] { 2, 1, 0 };
				result = array3[heroine.chaCtrl.GetBustCategory()];
				break;
			}
			case 13:
			{
				int[] array2 = new int[3] { 2, 1, 0 };
				result = array2[heroine.chaCtrl.GetHeightCategory()];
				break;
			}
			case 14:
			{
				int[] array = new int[3] { 2, 1, 0 };
				result = array[heroine.chaCtrl.GetWaistCategory()];
				break;
			}
			}
			return result;
		}

		private List<Program.Transfer> CreateGenericADV(GenericInfo _info, ChangeValueInfo _cvInfo, bool _motion = true)
		{
			TalkData talkData = _info.talk[0];
			_cvInfo[0] = talkData.favor;
			_cvInfo[1] = talkData.lewdness;
			_cvInfo[2] = talkData.anger;
			bool motion = _motion;
			return CreateGenericADV(_info, true, motion);
		}

		private List<Program.Transfer> CreateBranchADV(BranchInfo _info, ChangeValueInfo _cvInfo)
		{
			List<Program.Transfer> list = CreateNewList();
			AddTransfer(list, _info.introduction);
			TalkData talkData = null;
			switch (_info.conditions)
			{
			case 7:
				talkData = _info[(Singleton<Manager.Communication>.Instance.PreferenceValue(heroine)[0] <= 3) ? 1 : 0];
				break;
			case 8:
				talkData = _info[(Singleton<Manager.Communication>.Instance.PreferenceValue(heroine)[0] >= -3) ? 1 : 0];
				break;
			case 9:
				talkData = _info[(Singleton<Manager.Communication>.Instance.PreferenceValue(heroine)[1] <= 3) ? 1 : 0];
				break;
			case 10:
				talkData = _info[(Singleton<Manager.Communication>.Instance.PreferenceValue(heroine)[1] >= -3) ? 1 : 0];
				break;
			case 11:
				talkData = _info[(Singleton<Manager.Communication>.Instance.PreferenceValue(heroine)[2] <= 3) ? 1 : 0];
				break;
			case 12:
				talkData = _info[(Singleton<Manager.Communication>.Instance.PreferenceValue(heroine)[2] >= -3) ? 1 : 0];
				break;
			case 41:
				talkData = _info[heroine.isAnger ? 1 : 0];
				break;
			default:
				if (MathfEx.RangeEqualOn(1, _info.conditions, heroine.talkTemper.Length))
				{
					talkData = _info[heroine.talkTemper[_info.conditions - 1]];
					break;
				}
				switch (_info.success.Length)
				{
				case 1:
					talkData = ((_info.noInterested == null) ? _info[RandomBranch(30, 70)] : _info[RandomBranch(30, 30, 40)]);
					break;
				case 2:
					talkData = _info[RandomBranch(20, 40, 40)];
					break;
				}
				break;
			}
			AddTransfer(list, talkData);
			list.Add(Program.Transfer.Close());
			_cvInfo[0] = talkData.favor;
			_cvInfo[1] = talkData.lewdness;
			_cvInfo[2] = talkData.anger;
			return list;
		}

		private List<Program.Transfer> CreateBranchADV(BranchInfo _info, bool _success, bool _close = true)
		{
			List<Program.Transfer> list = CreateNewList();
			AddTransfer(list, _info.introduction);
			AddTransfer(list, (!_success) ? _info.failure : _info.success[0]);
			if (_close)
			{
				list.Add(Program.Transfer.Close());
			}
			return list;
		}

		private void AddTransfer(List<Program.Transfer> _list, TalkData _talk, bool _motion = true)
		{
			if (_talk.file.IsNullOrEmpty())
			{
				_list.Add(Program.Transfer.Text("[P名]", _talk.text));
			}
			else if (_talk.text.IsNullOrEmpty())
			{
				int result = 0;
				if (int.TryParse(_talk.file, out result))
				{
					heroine.talkEvent.Add(result);
				}
				_list.Add(Program.Transfer.Open(_talk.assetbundle, _talk.file, bool.TrueString));
			}
			else
			{
				AddTransferHeroinText(_list, _talk, _motion);
			}
		}

		private void AddTransferHeroinText(List<Program.Transfer> _list, TalkData _talk, bool _motion = true)
		{
			_list.Add(Program.Transfer.Create(true, Command.CharaLookEyes, "0", _talk.eyeLook.ToString(), "0"));
			_list.Add(Program.Transfer.Create(true, Command.CharaLookNeck, "0", _talk.neckLook.ToString(), "0"));
			if (_motion && !_talk.pose.IsNullOrEmpty())
			{
				string[] poseInfo = Singleton<Manager.Communication>.Instance.GetPoseInfo(heroine.FixCharaIDOrPersonality, _talk.pose);
				if (!poseInfo.IsNullOrEmpty())
				{
					_list.Add(Program.Transfer.Motion("0", poseInfo[2], poseInfo[0], poseInfo[1], poseInfo.SafeGet(3), poseInfo.SafeGet(4)));
				}
			}
			if (!_talk.facialExpression.IsNullOrEmpty())
			{
				_list.Add(Program.Transfer.Expression("0", _talk.facialExpression));
			}
			string[] array = _talk.file.Split(',');
			foreach (string text in array)
			{
				if (Regex.Match(text, "callName(\\d*)").Success)
				{
					_list.Add(Program.Transfer.Voice("0", "callBundle", text));
				}
				else
				{
					_list.Add(Program.Transfer.Voice("0", _talk.assetbundle, text));
				}
			}
			_list.Add(Program.Transfer.Text("[H名]", _talk.text));
		}

		private int GetStage()
		{
			if (heroine.isGirlfriend)
			{
				return 2;
			}
			if (heroine.talkEvent.Contains(2))
			{
				return 1;
			}
			if (heroine.talkEvent.Contains(0) || heroine.talkEvent.Contains(1))
			{
				return 0;
			}
			return -1;
		}

		private int RandomBranch(params int[] _values)
		{
			int num = Array.FindIndex(_values, (int v) => v == 100);
			if (num != -1)
			{
				return num;
			}
			Dictionary<int, int> targetDict = _values.Select((int value, int index) => new { value, index }).ToDictionary(v => v.index, v => v.value);
			return Utils.ProbabilityCalclator.DetermineFromDict(targetDict);
		}

		private List<Program.Transfer> CreateNewList()
		{
			List<Program.Transfer> list = new List<Program.Transfer>();
			if (passingInfo.player != null)
			{
				Program.SetParam(passingInfo.player, heroine, list);
			}
			else
			{
				Program.SetParam(heroine, list);
			}
			return list;
		}

		private bool ConditionDeterminationSpeak(int _conditions)
		{
			switch (_conditions)
			{
			case 0:
				return heroine.isAnger;
			case -1:
				return true;
			case 40:
				if (heroine.isNickNameNormal)
				{
					return false;
				}
				return !heroine.talkEvent.Contains(89);
			case 41:
				if (heroine.isNickNameNormal)
				{
					return false;
				}
				return heroine.talkEvent.Contains(89);
			default:
				return true;
			}
		}

		private bool ConditionDeterminationListen(int _conditions)
		{
			switch (_conditions)
			{
			case 0:
				return heroine.isAnger;
			case -1:
				return true;
			case 18:
				if (!heroine.isNickNameNormal)
				{
					return false;
				}
				return !heroine.talkEvent.Contains(90);
			case 19:
				if (!heroine.isNickNameNormal)
				{
					return false;
				}
				return heroine.talkEvent.Contains(90);
			default:
				return true;
			}
		}

		private bool ConfirmEvent0(int _conditions, bool _heroine)
		{
			if (_heroine)
			{
				return false;
			}
			switch (_conditions)
			{
			case 0:
				if (cycleType != Cycle.Type.AfterSchool || heroine.talkCommand.Contains(4))
				{
					break;
				}
				return true;
			case 1:
				if (heroine.talkCommand.Contains(6))
				{
					break;
				}
				return true;
			case 2:
				if (heroine.talkCommand.Contains(7))
				{
					break;
				}
				return true;
			case 3:
				if (!isChasePossible || heroine.talkTime <= 1)
				{
					break;
				}
				return true;
			case 411:
				if (_heroine || cycleType != Cycle.Type.StaffTime || !heroine.isStaff || heroine.talkCommand.Contains(3))
				{
					break;
				}
				return true;
			case 422:
				if (_heroine || heroine.isStaff || heroine.talkCommand.Contains(8))
				{
					break;
				}
				return true;
			}
			return false;
		}

		private bool ConfirmEvent1(int _conditions, bool _heroine)
		{
			bool flag = heroine.talkTime <= 0;
			switch (_conditions)
			{
			case 0:
				if (_heroine || cycleType != Cycle.Type.AfterSchool || heroine.talkEvent.Contains(3) || heroine.talkCommand.Contains(4))
				{
					break;
				}
				return true;
			case 1:
				if (_heroine || heroine.talkEvent.Contains(4) || heroine.talkCommand.Contains(6))
				{
					break;
				}
				return true;
			case 2:
				if (_heroine || heroine.talkEvent.Contains(5) || heroine.talkCommand.Contains(7))
				{
					break;
				}
				return true;
			case 3:
				if (_heroine || !isChasePossible || heroine.talkEvent.Contains(6) || heroine.talkTime <= 1)
				{
					break;
				}
				return true;
			case 400:
				if (_heroine || heroine.talkEvent.Contains(7) || heroine.talkCommand.Contains(0))
				{
					break;
				}
				return true;
			case 401:
				if (_heroine || !heroine.talkEvent.Contains(7) || heroine.talkCommand.Contains(0))
				{
					break;
				}
				return true;
			case 404:
				if (_heroine || !isHPossible || isSecondH || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 405:
				if (_heroine || !isHPossible || !isSecondH || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 406:
				if (_heroine || !isHPossible || !isSecondH || !isOtherPeople || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 407:
				if (_heroine || !isHPossible || !isSecondH || !exposure || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 409:
				if (_heroine || cycleType != Cycle.Type.LunchTime || heroine.isLunch || heroine.talkCommand.Contains(2))
				{
					break;
				}
				return true;
			case 411:
				if (_heroine || cycleType != Cycle.Type.StaffTime || !heroine.isStaff || heroine.talkCommand.Contains(3))
				{
					break;
				}
				return true;
			case 413:
				if (_heroine || cycleType != Cycle.Type.AfterSchool || !heroine.talkEvent.Contains(3) || heroine.talkCommand.Contains(4))
				{
					break;
				}
				return true;
			case 415:
				if (_heroine || CheckEventFlag(9, 13) || heroine.isDate || heroine.talkCommand.Contains(5))
				{
					break;
				}
				return true;
			case 416:
				if (_heroine || !CheckEventFlag(9, 13) || heroine.isDate || heroine.talkCommand.Contains(5))
				{
					break;
				}
				return true;
			case 418:
				if (_heroine || !heroine.talkEvent.Contains(4) || heroine.talkCommand.Contains(6))
				{
					break;
				}
				return true;
			case 420:
				if (_heroine || !heroine.talkEvent.Contains(5) || heroine.talkCommand.Contains(7))
				{
					break;
				}
				return true;
			case 422:
				if (_heroine || heroine.isStaff || heroine.talkCommand.Contains(8))
				{
					break;
				}
				return true;
			case 424:
				if (_heroine || !isChasePossible || !heroine.talkEvent.Contains(6) || heroine.talkTime <= 1)
				{
					break;
				}
				return true;
			case 100:
				if (!_heroine || heroine.confessed || heroine.isAnger || heroine.talkCommand.Contains(0) || flag)
				{
					break;
				}
				return CheckConditions(1, _conditions);
			case 101:
				if (!_heroine || !heroine.confessed || heroine.isAnger || heroine.talkCommand.Contains(0) || flag)
				{
					break;
				}
				return CheckConditions(1, _conditions);
			case 102:
				if (!_heroine || !isHPossible || heroine.isAnger || heroine.talkCommand.Contains(1) || !flag)
				{
					break;
				}
				return CheckConditions(1, _conditions);
			case 103:
				if (!_heroine || cycleType != Cycle.Type.LunchTime || heroine.isLunch || heroine.isAnger || heroine.talkCommand.Contains(2) || flag)
				{
					break;
				}
				return CheckConditions(1, _conditions);
			case 104:
				if (!_heroine || cycleType != Cycle.Type.StaffTime || !heroine.isStaff || heroine.isAnger || heroine.talkCommand.Contains(3) || flag)
				{
					break;
				}
				return CheckConditions(1, _conditions);
			case 105:
				if (!_heroine || cycleType != Cycle.Type.AfterSchool || !heroine.talkEvent.Contains(3) || heroine.isAnger || heroine.talkCommand.Contains(4) || flag)
				{
					break;
				}
				return CheckConditions(1, _conditions);
			case 106:
				if (!_heroine || heroine.talkCommand.Contains(5) || !flag)
				{
					break;
				}
				return !heroine.isDate & !heroine.isAnger & CheckConditions(1, _conditions);
			case 107:
				if (!_heroine || !heroine.talkEvent.Contains(4) || heroine.talkCommand.Contains(6) || flag)
				{
					break;
				}
				return !heroine.isAnger & CheckConditions(1, _conditions);
			case 108:
				if (!_heroine || !heroine.talkEvent.Contains(5) || heroine.talkCommand.Contains(7) || flag)
				{
					break;
				}
				return !heroine.isAnger & CheckConditions(1, _conditions);
			case 109:
				if (!_heroine || heroine.talkCommand.Contains(8) || flag)
				{
					break;
				}
				return !heroine.isStaff & !heroine.isAnger & CheckConditions(1, _conditions);
			}
			return false;
		}

		private bool ConfirmEvent2(int _conditions, bool _heroine)
		{
			bool flag = heroine.talkTime <= 0;
			switch (_conditions)
			{
			case 0:
				if (_heroine || cycleType != Cycle.Type.AfterSchool || heroine.talkEvent.Contains(3) || heroine.talkCommand.Contains(4))
				{
					break;
				}
				return true;
			case 1:
				if (_heroine || heroine.talkEvent.Contains(4) || heroine.talkCommand.Contains(6))
				{
					break;
				}
				return true;
			case 2:
				if (_heroine || heroine.talkEvent.Contains(5) || heroine.talkCommand.Contains(7))
				{
					break;
				}
				return true;
			case 3:
				if (_heroine || !isChasePossible || heroine.talkEvent.Contains(6) || heroine.talkTime <= 1)
				{
					break;
				}
				return true;
			case 300:
				if (_heroine || !heroine.isAnger)
				{
					break;
				}
				return true;
			case 301:
				if (_heroine || heroine.isAnger)
				{
					break;
				}
				return true;
			case 304:
				if (_heroine || !isHPossible || isSecondH || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 305:
				if (_heroine || !isHPossible || !isSecondH || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 307:
				if (_heroine || !isHPossible || !isSecondH || !isOtherPeople || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 308:
				if (_heroine || !isHPossible || !isSecondH || !exposure || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return true;
			case 309:
				if (_heroine || cycleType != Cycle.Type.LunchTime || heroine.isLunch || heroine.talkCommand.Contains(2))
				{
					break;
				}
				return true;
			case 311:
				if (_heroine || cycleType != Cycle.Type.StaffTime || !heroine.isStaff || heroine.talkCommand.Contains(3))
				{
					break;
				}
				return true;
			case 313:
				if (_heroine || cycleType != Cycle.Type.AfterSchool || !heroine.talkEvent.Contains(3) || heroine.talkCommand.Contains(4))
				{
					break;
				}
				return true;
			case 315:
				if (_heroine || CheckEventFlag(9, 13) || heroine.isDate || heroine.talkCommand.Contains(5))
				{
					break;
				}
				return true;
			case 316:
				if (_heroine || !CheckEventFlag(9, 13) || heroine.isDate || heroine.talkCommand.Contains(5))
				{
					break;
				}
				return true;
			case 318:
				if (_heroine || !heroine.talkEvent.Contains(4) || heroine.talkCommand.Contains(6))
				{
					break;
				}
				return true;
			case 320:
				if (_heroine || !heroine.talkEvent.Contains(5) || heroine.talkCommand.Contains(7))
				{
					break;
				}
				return true;
			case 322:
				if (_heroine || !isChasePossible || !heroine.talkEvent.Contains(6) || heroine.talkTime <= 1)
				{
					break;
				}
				return true;
			case 323:
				if (_heroine || !isHPossible || !isSecondH || heroine.talkCommand.Contains(1))
				{
					break;
				}
				return CheckEventFlag(89, 90);
			case 422:
				if (_heroine || heroine.isStaff || heroine.talkCommand.Contains(8))
				{
					break;
				}
				return true;
			case 200:
				if (!_heroine || !heroine.isAnger || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions, (!heroine.isStaff) ? 0f : Singleton<Manager.Communication>.Instance.staffBenefitsInfo.breakup);
			case 201:
				if (!_heroine || heroine.isAnger || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions, (!heroine.isStaff) ? 0f : Singleton<Manager.Communication>.Instance.staffBenefitsInfo.breakup);
			case 202:
				if (!_heroine || heroine.isAnger || heroine.talkCommand.Contains(1) || !flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 203:
				if (!_heroine || cycleType != Cycle.Type.LunchTime || heroine.isLunch || heroine.isAnger || heroine.talkCommand.Contains(2) || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 204:
				if (!_heroine || cycleType != Cycle.Type.StaffTime || heroine.isAnger || !heroine.isStaff || heroine.talkCommand.Contains(3) || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 205:
				if (!_heroine || cycleType != Cycle.Type.AfterSchool || !heroine.talkEvent.Contains(3) || heroine.isAnger || heroine.talkCommand.Contains(4) || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 206:
				if (!_heroine || heroine.isDate || heroine.isAnger || heroine.talkCommand.Contains(5) || !flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 207:
				if (!_heroine || !heroine.talkEvent.Contains(4) || heroine.isAnger || heroine.talkCommand.Contains(6) || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 208:
				if (!_heroine || !heroine.talkEvent.Contains(5) || heroine.isAnger || heroine.talkCommand.Contains(7) || flag)
				{
					break;
				}
				return CheckConditions(2, _conditions);
			case 209:
				if (!_heroine || !heroine.talkEvent.Contains(12) || heroine.isAnger || heroine.talkCommand.Contains(1) || !flag)
				{
					break;
				}
				return CheckEventFlag(89, 90) & CheckConditions(2, _conditions);
			case 109:
				if (!_heroine || heroine.talkCommand.Contains(8) || flag)
				{
					break;
				}
				return !heroine.isStaff & !heroine.isAnger & CheckConditions(2, _conditions);
			}
			return false;
		}

		private bool CheckEventFlag(params int[] _array)
		{
			bool flag = false;
			foreach (int item in _array)
			{
				flag |= heroine.talkEvent.Contains(item);
			}
			return flag;
		}

		private bool CheckConditions(int _stage, int _condition, float _rev = 0f)
		{
			Manager.Communication.ConditionsInfo occurrenceCondition = Singleton<Manager.Communication>.Instance.GetOccurrenceCondition(_stage, _condition);
			if (occurrenceCondition == null)
			{
				return true;
			}
			if (!occurrenceCondition.favor.through && !occurrenceCondition.favor.Check(heroine.favor))
			{
				return false;
			}
			if (!occurrenceCondition.lewdness.through && !occurrenceCondition.lewdness.Check(heroine.lewdness))
			{
				return false;
			}
			if (occurrenceCondition.probability.through)
			{
				return true;
			}
			float num = occurrenceCondition.probability.Get((occurrenceCondition.reference != 0) ? heroine.lewdness : heroine.favor);
			num = ((_rev == 0f) ? Mathf.Lerp(0f, num, Mathf.InverseLerp(0f, 100f, Manager.Config.AddData.CommunicationCorrectionHeroineAction)) : Mathf.Clamp(num + _rev, 0f, 100f));
			return Utils.ProbabilityCalclator.DetectFromPercent(num);
		}

		private List<Program.Transfer> GetFirstEventADV(int _stage, int _command, ref int _derive, ref bool _first)
		{
			BranchInfo branchInfo = GetListCommand(0, Group.Event, _command)[0] as BranchInfo;
			List<Program.Transfer> list = CreateNewList();
			AddTransfer(list, branchInfo.introduction);
			int[] array = new int[4] { 6, 8, 9, 11 };
			int num = Array.IndexOf(array, _command) + 3;
			Manager.Communication.ProbabilityValue communicationConndition = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(_stage, branchInfo.conditions);
			int num2 = (int)communicationConndition.Get(heroine.favor);
			if (heroine.isStaff && _stage == 1)
			{
				num2 = (int)Mathf.Clamp((float)num2 + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.together, 0f, 100f);
			}
			bool flag = RandomBranch(num2, 100 - num2) == 0;
			bool flag2 = !heroine.talkEvent.Contains(num);
			AddTransfer(list, (!flag) ? branchInfo.failure : branchInfo.success[heroine.talkEvent.Contains(num) ? 1 : 0]);
			if (flag)
			{
				ResultEnum[] array2 = new ResultEnum[4]
				{
					ResultEnum.GoHome,
					ResultEnum.Study,
					ResultEnum.Exercise,
					ResultEnum.Chase
				};
				_derive = (int)array2[num - 3];
				_first = flag2;
			}
			list.Add(Program.Transfer.Close());
			return list;
		}

		private List<Program.Transfer> GetStage1Branch0(int _stage, int _command, ref int _derive)
		{
			BranchInfo branchInfo = GetListCommand(1, Group.Event, _command)[0] as BranchInfo;
			Manager.Communication.ProbabilityValue communicationConndition = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(_stage, branchInfo.conditions);
			int num = (int)communicationConndition.Get(heroine.favor);
			if (heroine.isStaff && _stage == 1)
			{
				num = (int)Mathf.Clamp((float)num + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.together, 0f, 100f);
			}
			bool flag = RandomBranch(num, 100 - num) == 0;
			List<Program.Transfer> result = CreateBranchADV(branchInfo, flag);
			if (flag)
			{
				int[] array = new int[4] { 8, 9, 4, 5 };
				int num2 = Array.IndexOf(array, _command);
				ResultEnum[] array2 = new ResultEnum[4]
				{
					ResultEnum.Study,
					ResultEnum.Exercise,
					ResultEnum.Lunch,
					ResultEnum.Club
				};
				_derive = (int)array2[num2];
			}
			return result;
		}

		private List<Program.Transfer> GetStage1Branch1(int _command, ref int _derive)
		{
			BranchInfo branchInfo = GetListCommand(1, Group.Event, _command)[0] as BranchInfo;
			Manager.Communication.ProbabilityValue communicationConndition = Singleton<Manager.Communication>.Instance.GetCommunicationConndition(1, branchInfo.conditions);
			int num = (int)communicationConndition.Get(heroine.favor);
			if (heroine.isStaff)
			{
				num = (int)Mathf.Clamp((float)num + Singleton<Manager.Communication>.Instance.staffBenefitsInfo.together, 0f, 100f);
			}
			bool flag = RandomBranch(num, 100 - num) == 0;
			List<Program.Transfer> result = CreateBranchADV(branchInfo, flag);
			if (flag)
			{
				int[] array = new int[3] { 6, 10, 11 };
				int num2 = Array.IndexOf(array, _command);
				ResultEnum[] array2 = new ResultEnum[3]
				{
					ResultEnum.GoHome,
					ResultEnum.None,
					ResultEnum.Chase
				};
				_derive = (int)array2[num2];
				if (_command == 10)
				{
					heroine.talkEvent.Add(10);
					heroine.isStaff = true;
					Singleton<Game>.Instance.rankSaveData.staffCount++;
					Singleton<Game>.Instance.saveData.clubReport.staffAdd = 1;
				}
			}
			return result;
		}

		private List<Program.Transfer> GetStage2Generic(int _command, ref int _derive)
		{
			List<Program.Transfer> result = CreateGenericADV(GetListCommand(2, Group.Event, _command)[0] as GenericInfo);
			int[] array = new int[4] { 8, 9, 4, 5 };
			int num = Array.IndexOf(array, _command);
			ResultEnum[] array2 = new ResultEnum[4]
			{
				ResultEnum.Study,
				ResultEnum.Exercise,
				ResultEnum.Lunch,
				ResultEnum.Club
			};
			_derive = (int)array2[num];
			return result;
		}

		private List<Program.Transfer> GetStaffBranch(int _min, int _max, ref int _derive)
		{
			BranchInfo info = GetListCommand(1, Group.Event, 10)[0] as BranchInfo;
			int num = (int)Mathf.Lerp(_min, _max, Mathf.InverseLerp(0f, 100f, heroine.favor));
			bool flag = RandomBranch(num, 100 - num) == 0;
			List<Program.Transfer> result = CreateBranchADV(info, flag);
			if (flag)
			{
				_derive = 0;
				heroine.talkEvent.Add(10);
				heroine.isStaff = true;
				Singleton<Game>.Instance.rankSaveData.staffCount++;
				Singleton<Game>.Instance.saveData.clubReport.staffAdd = 1;
			}
			return result;
		}

		private List<Program.Transfer> GetHeroineADV(BasicInfo _target, int _stage, out ChangeValueAbstractInfo _outInfo, ref int _derive)
		{
			switch (_stage)
			{
			case 1:
			{
				Tuple<int, int>[] array2 = new Tuple<int, int>[10]
				{
					new Tuple<int, int>(100, 0),
					new Tuple<int, int>(101, 0),
					new Tuple<int, int>(102, 1),
					new Tuple<int, int>(103, 2),
					new Tuple<int, int>(104, 3),
					new Tuple<int, int>(105, 4),
					new Tuple<int, int>(106, 5),
					new Tuple<int, int>(107, 6),
					new Tuple<int, int>(108, 7),
					new Tuple<int, int>(109, 8)
				};
				_derive = array2[Array.FindIndex(array2, (Tuple<int, int> v) => v.Item1 == _target.conditions)].Item2;
				break;
			}
			case 2:
			{
				Tuple<int, int>[] array = new Tuple<int, int>[11]
				{
					new Tuple<int, int>(200, 9),
					new Tuple<int, int>(201, 9),
					new Tuple<int, int>(202, 1),
					new Tuple<int, int>(203, 2),
					new Tuple<int, int>(204, 3),
					new Tuple<int, int>(205, 4),
					new Tuple<int, int>(206, 5),
					new Tuple<int, int>(207, 6),
					new Tuple<int, int>(208, 7),
					new Tuple<int, int>(209, 1),
					new Tuple<int, int>(109, 8)
				};
				_derive = array[Array.FindIndex(array, (Tuple<int, int> v) => v.Item1 == _target.conditions)].Item2;
				break;
			}
			}
			switch (_target.GetType().Name)
			{
			case "GenericInfo":
				_outInfo = new ChangeValueInfo();
				return CreateGenericADV(_target as GenericInfo, _outInfo as ChangeValueInfo);
			case "BranchInfo":
				_outInfo = new ChangeValueInfo();
				return CreateBranchADV(_target as BranchInfo, _outInfo as ChangeValueInfo);
			case "SelectInfo":
				_outInfo = new ChangeValueSelectInfo();
				return CreateSelectADV(_target as SelectInfo, _outInfo as ChangeValueSelectInfo);
			default:
				_outInfo = null;
				return null;
			}
		}

		private static string BestPlaceNameConvert(string placeName)
		{
			switch (placeName)
			{
			case "1F":
			case "2F":
			case "3F":
				return "廊下";
			case "教室1-1":
			case "教室2-1":
			case "教室2-2":
			case "教室3-1":
				return "教室";
			default:
				return placeName;
			}
		}

		private static bool IsBestPlaceNameConvert(string bestPlaceName)
		{
			switch (bestPlaceName)
			{
			case "廊下":
				return true;
			case "教室":
				return true;
			default:
				return false;
			}
		}
	}
}
