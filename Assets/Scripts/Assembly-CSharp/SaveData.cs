using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ADV;
using ActionGame;
using ActionGame.Chara;
using Illusion;
using Illusion.Elements.Reference;
using Illusion.Extensions;
using Localize.Translate;
using Manager;
using UnityEngine;

[Serializable]
public sealed class SaveData
{
	[Serializable]
	public abstract class CharaData
	{
		public abstract class Params
		{
			public class Data : Pointer<object>
			{
				public enum Command
				{
					None = 0,
					Replace = 1,
					Int = 2,
					String = 3,
					BOOL = 4
				}

				public Command command { get; private set; }

				public string key { get; private set; }

				public Data(Command command, string key, Func<object> get, Action<object> set = null)
					: base(get, set)
				{
					this.command = command;
					this.key = key;
				}

				public static Command Cast(object o)
				{
					if (o is string)
					{
						return Command.String;
					}
					if (o is int)
					{
						return Command.Int;
					}
					if (o is bool)
					{
						return Command.BOOL;
					}
					return Command.None;
				}

				public static Command Cast(Type type)
				{
					if (type == typeof(string))
					{
						return Command.String;
					}
					if (type == typeof(int))
					{
						return Command.Int;
					}
					if (type == typeof(bool))
					{
						return Command.BOOL;
					}
					return Command.None;
				}
			}

			protected List<Data> _datas;

			public List<Data> datas
			{
				get
				{
					return _datas;
				}
			}

			public CharaData chara { get; private set; }

			public Params(CharaData chara, string header)
			{
				this.chara = chara;
				_datas = new List<Data>();
				_datas.AddRange(from p in Utils.Type.GetPublicFields(chara.GetType())
					where Data.Cast(p.FieldType) != Data.Command.None
					select new Data(Data.Cast(p.FieldType), header + p.Name, () => p.GetValue(chara), delegate(object o)
					{
						p.SetValue(chara, Convert.ChangeType(o, p.FieldType));
					}));
				_datas.AddRange(from p in Utils.Type.GetPublicProperties(chara.GetType())
					where Data.Cast(p.PropertyType) != Data.Command.None
					select new Data(Data.Cast(p.PropertyType), header + p.Name, () => p.GetValue(chara, null)));
			}

			public void CreateCommand(List<Program.Transfer> list)
			{
				int num = -1;
				foreach (Data data in datas)
				{
					num++;
					switch (data.command)
					{
					case Data.Command.Replace:
						list.Add(Program.Transfer.Create(true, Command.Replace, data.key, (string)data.value));
						break;
					case Data.Command.Int:
						list.Add(Program.Transfer.Create(true, Command.VAR, "int", data.key, data.value.ToString()));
						break;
					case Data.Command.String:
						list.Add(Program.Transfer.Create(true, Command.VAR, "string", data.key, data.value.ToString()));
						break;
					case Data.Command.BOOL:
						list.Add(Program.Transfer.Create(true, Command.VAR, "bool", data.key, data.value.ToString()));
						break;
					}
				}
			}

			public IEnumerator<Data> GetEnumerator()
			{
				foreach (Data data in datas)
				{
					yield return data;
				}
			}
		}

		public enum NameType
		{
			Last = 0,
			First = 1,
			Full = 2,
			Nick = 3
		}

		public int callID = -1;

		public string callName = string.Empty;

		public int schoolClass = -1;

		public int schoolClassIndex = -1;

		public static readonly string[] Names = new string[4] { "姓", "名", "名前", "あだ名" };

		public NameType nameType = NameType.First;

		public abstract int voiceNo { get; }

		public GameObject root { get; private set; }

		public Base charaBase { get; private set; }

		public ChaControl chaCtrl { get; private set; }

		public Transform transform
		{
			get
			{
				return (!(root == null)) ? root.transform : null;
			}
		}

		public bool isNickNameNormal
		{
			get
			{
				return !SaveData.GetCallName(this).isSpecial;
			}
		}

		public int callMyID
		{
			get
			{
				int callDefaultID = callID;
				if (callDefaultID == -1)
				{
					if (Singleton<Game>.IsInstance())
					{
						callDefaultID = Singleton<Game>.Instance.Player.callID;
					}
					if (callDefaultID == -1)
					{
						callDefaultID = SaveData.callDefaultID;
					}
				}
				return callDefaultID;
			}
		}

		public string Name
		{
			get
			{
				return parameter.fullname;
			}
		}

		public string lastname
		{
			get
			{
				return parameter.lastname;
			}
		}

		public string firstname
		{
			get
			{
				return parameter.firstname;
			}
		}

		public int personality
		{
			get
			{
				return parameter.personality;
			}
		}

		public int bloodType
		{
			get
			{
				return parameter.bloodType;
			}
		}

		public int birthMonth
		{
			get
			{
				return parameter.birthMonth;
			}
		}

		public int birthDay
		{
			get
			{
				return parameter.birthDay;
			}
		}

		public int clubActivities
		{
			get
			{
				return parameter.clubActivities;
			}
		}

		public string nickname
		{
			get
			{
				return parameter.nickname;
			}
		}

		public float voicePitch
		{
			get
			{
				return parameter.voicePitch;
			}
		}

		public int weakPoint
		{
			get
			{
				return parameter.weakPoint;
			}
		}

		public ChaFileParameter.Denial denial
		{
			get
			{
				return parameter.denial;
			}
		}

		public bool charFileInitialized { get; set; }

		public ChaFileControl charFile { get; private set; }

		public ChaFileParameter parameter
		{
			get
			{
				return charFile.parameter;
			}
		}

		public CharaData(ChaFileControl charFile, bool isRandomize)
		{
			SetCharFile(charFile);
			Create(isRandomize);
		}

		public CharaData(bool isRandomize)
		{
			SetCharFile(new ChaFileControl());
			Create(isRandomize);
		}

		public void SetRoot(GameObject root)
		{
			this.root = root;
			if (root == null)
			{
				charaBase = null;
				chaCtrl = null;
				return;
			}
			Base component = root.GetComponent<Base>();
			if (component != null)
			{
				charaBase = component;
				chaCtrl = charaBase.chaCtrl;
			}
			else
			{
				chaCtrl = root.GetComponent<ChaControl>();
			}
		}

		public string GetCallName()
		{
			return FindCallFileData(this).name;
		}

		public void SetCharFile(ChaFileControl charFile)
		{
			this.charFile = charFile;
			ChaFileUpdate();
		}

		public abstract void ChaFileUpdate();

		public abstract void Initialize();

		private void Create(bool isRandomize)
		{
			Initialize();
			if (isRandomize)
			{
				Randomize();
			}
		}

		public abstract void Randomize();

		public abstract void SetADVParam(List<Program.Transfer> list);

		public abstract void SetADVParam(TextScenario scenario, string key, object o);

		public abstract void SetADVParam(TextScenario scenario);

		public virtual void Save(BinaryWriter w)
		{
			w.Write(schoolClass);
			w.Write(schoolClassIndex);
			charFile.SaveCharaFile(w, false);
			w.Write((int)nameType);
			w.Write(callID);
			w.Write(callName);
		}

		public virtual void Load(Version version, BinaryReader r)
		{
			schoolClass = r.ReadInt32();
			schoolClassIndex = r.ReadInt32();
			charFile.LoadCharaFile(r, true);
			charFileInitialized = true;
			nameType = (NameType)r.ReadInt32();
			callID = r.ReadInt32();
			callName = r.ReadString();
		}
	}

	[Serializable]
	public class Heroine : CharaData
	{
		public new class Params : CharaData.Params
		{
			private Heroine heroine
			{
				get
				{
					return base.chara as Heroine;
				}
			}

			public Params(Heroine heroine)
				: base(heroine, "H_")
			{
				string text = "H_" + Utils.String.GetPropertyName(() => heroine.HExperience);
				_datas.AddRange(new List<Data>
				{
					new Data(Data.Command.Replace, "H", () => string.Empty),
					new Data(Data.Command.Replace, "H" + CharaData.Names[0], () => heroine.lastname),
					new Data(Data.Command.Replace, "H" + CharaData.Names[1], () => heroine.firstname),
					new Data(Data.Command.Replace, "H" + CharaData.Names[2], () => heroine.Name),
					new Data(Data.Command.Replace, "H" + CharaData.Names[3], () => heroine.nickname),
					new Data(Data.Command.Replace, "P" + CharaData.Names[3], () => heroine.callName),
					new Data(Data.Command.String, text, () => heroine.HExperience.ToString()),
					new Data(Data.Command.Int, text + "Index", () => (int)heroine.HExperience)
				});
			}
		}

		public enum HExperienceKind
		{
			初めて = 0,
			不慣れ = 1,
			慣れ = 2,
			淫乱 = 3
		}

		public const string ParamHead = "H";

		public const string ParamHeadName = "H_";

		private int? cachedVoiceNo;

		public int favor;

		public int lewdness;

		public int hCount;

		public bool isStaff;

		public bool isGirlfriend;

		public bool isAnger;

		public int anger;

		public int fixCharaID;

		public bool isTaked;

		public bool isDate;

		public int nickNameTalkCount;

		public int myRoomCount;

		public Dictionary<string, float> motionSpeed;

		private HashSet<int> m_TalkEvent = new HashSet<int>();

		private byte[] m_TalkTemper = new byte[39];

		public bool confessed;

		private int menstruationStartDay;

		private readonly byte menstruationMonth = 15;

		private byte menstruationDay;

		public float[] hAreaExps = new float[6];

		public float[] massageExps = new float[2];

		public bool isVirgin = true;

		public bool isAnalVirgin = true;

		public float countKokanH;

		public float countAnalH;

		public float houshiExp;

		public bool isKiss;

		public int countNamaInsert;

		public int countHoushi;

		public int eventAfterDay;

		public bool isFirstGirlfriend;

		public int intimacy;

		public int talkTimeMax = 4;

		public int talkTime = 4;

		public bool isTalkPoint;

		public bool isLunch;

		public HashSet<int> talkIntro = new HashSet<int>();

		public HashSet<int> talkCommand = new HashSet<int>();

		private Params _param;

		public bool isTeacher
		{
			get
			{
				return fixCharaID >= -4 && fixCharaID <= -1;
			}
		}

		public int FixCharaIDOrPersonality
		{
			get
			{
				return (fixCharaID == 0) ? base.personality : fixCharaID;
			}
		}

		public string ChaName
		{
			get
			{
				return "c" + FixCharaIDOrPersonality.MinusThroughToString("00");
			}
		}

		public string ChaVoice
		{
			get
			{
				return "c" + voiceNo.MinusThroughToString("00");
			}
		}

		public override int voiceNo
		{
			get
			{
				if (!cachedVoiceNo.HasValue)
				{
					VoiceInfo.Param value;
					if (Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(FixCharaIDOrPersonality, out value))
					{
						cachedVoiceNo = value.No;
					}
					else
					{
						cachedVoiceNo = base.personality;
					}
				}
				return cachedVoiceNo.Value;
			}
		}

		public int relation
		{
			get
			{
				if (isGirlfriend)
				{
					return 2;
				}
				if (talkEvent.Contains(2))
				{
					return 1;
				}
				if (talkEvent.Contains(0) || talkEvent.Contains(1))
				{
					return 0;
				}
				return -1;
			}
		}

		public bool isNickNameEvent
		{
			get
			{
				return talkEvent.Contains(89) || talkEvent.Contains(90);
			}
		}

		public HashSet<int> talkEvent
		{
			get
			{
				return m_TalkEvent;
			}
		}

		public byte[] talkTemper
		{
			get
			{
				return m_TalkTemper;
			}
		}

		public byte MenstruationDay
		{
			get
			{
				return (byte)((menstruationDay + menstruationStartDay) % menstruationMonth);
			}
		}

		public int[] coordinates { get; set; }

		public bool[] isDresses { get; set; }

		public int NowCoordinate
		{
			get
			{
				if (isDresses.IsNullOrEmpty())
				{
					return coordinates.SafeGet(0);
				}
				int num = isDresses.Check(false);
				return (num != -1) ? coordinates[num] : coordinates[coordinates.Length - 1];
			}
		}

		public Params param
		{
			get
			{
				return this.GetCache(ref _param, () => new Params(this));
			}
		}

		public HExperienceKind HExperience
		{
			get
			{
				lewdness = Mathf.Clamp(lewdness, 0, 100);
				if (hCount >= 1)
				{
					float[] array = new float[hAreaExps.Length - 1];
					Array.Copy(hAreaExps, 1, array, 0, hAreaExps.Length - 1);
					return (!(countKokanH >= 100f) || !(countAnalH >= 100f) || array.Any((float a) => a < 100f)) ? HExperienceKind.不慣れ : ((lewdness != 100) ? HExperienceKind.慣れ : HExperienceKind.淫乱);
				}
				return HExperienceKind.初めて;
			}
		}

		public Heroine(ChaFileControl charFile, bool isRandomize)
			: base(charFile, isRandomize)
		{
		}

		public Heroine(bool isRandomize)
			: base(isRandomize)
		{
		}

		public Heroine(Version version, BinaryReader r)
			: base(false)
		{
			Load(version, r);
		}

		public override void Randomize()
		{
			menstruationStartDay = UnityEngine.Random.Range(0, menstruationMonth);
		}

		public int GetShuffleEventNo(params int[] events)
		{
			int[] array = events.Except(talkEvent).ToArray();
			return ((!array.Any()) ? events : array).Shuffle().First();
		}

		public void PopInit()
		{
			talkTimeMax = 4 + (isStaff ? Singleton<Communication>.Instance.staffBenefitsInfo.talk[Mathf.Max(0, relation)] : 0);
			talkTime = talkTimeMax;
			isTalkPoint = false;
			isLunch = false;
			talkIntro.Clear();
			talkCommand.Clear();
		}

		public void NextCoordinate()
		{
			int num = isDresses.Check(false);
			if (num != -1)
			{
				isDresses[num] = true;
			}
		}

		public override void SetADVParam(List<Program.Transfer> list)
		{
			param.CreateCommand(list);
			list.Add(Program.Transfer.Create(true, Command.HeroineCallNameChange, (string[])null));
		}

		public override void SetADVParam(TextScenario scenario, string key, object o)
		{
			CharaData.Params.Data data = param.datas.FirstOrDefault((CharaData.Params.Data p) => p.key == key);
			if (data == null)
			{
				return;
			}
			ValData valData = new ValData(ValData.Convert(o, data.value.GetType()));
			data.value = valData.o;
			scenario.Vars[data.key] = valData;
			if (key == "H_" + Utils.String.GetPropertyName(() => callID))
			{
				CallNameChange(scenario);
			}
			foreach (CharaData.Params.Data item in param.datas.Where((CharaData.Params.Data p) => p.command == CharaData.Params.Data.Command.Replace))
			{
				scenario.Replaces[item.key] = (string)item.value;
			}
		}

		private void AfterVarsUpdate(TextScenario scenario, string key, Func<object> func, Type type)
		{
			CharaData.Params.Data data = param.datas.FirstOrDefault((CharaData.Params.Data p) => p.key == key);
			data.value = func();
			scenario.Vars[data.key] = new ValData(ValData.Convert(data.value, type));
		}

		public override void SetADVParam(TextScenario scenario)
		{
			foreach (CharaData.Params.Data item in param.datas.Where((CharaData.Params.Data p) => p.command == CharaData.Params.Data.Command.Int || p.command == CharaData.Params.Data.Command.BOOL || p.command == CharaData.Params.Data.Command.String))
			{
				scenario.Vars[item.key] = new ValData(ValData.Convert(item.value, item.value.GetType()));
			}
			foreach (CharaData.Params.Data item2 in param.datas.Where((CharaData.Params.Data p) => p.command == CharaData.Params.Data.Command.Replace))
			{
				scenario.Replaces[item2.key] = (string)item2.value;
			}
		}

		public void CallNameChange(TextScenario scenario)
		{
			int num = base.callMyID;
			if (num != -1)
			{
				CallFileData callFileData = FindCallFileData(base.personality, num);
				scenario.Vars["callBundle"] = new ValData(callFileData.bundle);
				for (int i = 0; i < 3; i++)
				{
					scenario.Vars["callName" + i] = new ValData(callFileData.GetFileName(i));
				}
				string key = "H_" + Utils.String.GetPropertyName(() => callName);
				callName = callFileData.name;
				scenario.Vars[key] = new ValData(callName);
				scenario.Replaces["P" + CharaData.Names[3]] = callName;
			}
		}

		public void AddMenstruationsDay()
		{
			menstruationDay = (byte)((menstruationDay + 1) % menstruationMonth);
		}

		public void SetMenstruationsDay(int _menstruationDay)
		{
			menstruationDay = (byte)(_menstruationDay % menstruationMonth);
		}

		public void EventAfterDayAdd(int add)
		{
			eventAfterDay = Mathf.Max(0, eventAfterDay.ValueRound(add));
		}

		public void AddHCount()
		{
			hCount = Mathf.Min(hCount + 1, 2147483646);
		}

		public override void Save(BinaryWriter w)
		{
			base.Save(w);
			w.Write(favor);
			w.Write(lewdness);
			w.Write(hCount);
			w.Write(isStaff);
			w.Write(isGirlfriend);
			w.Write(isAnger);
			w.Write(fixCharaID);
			w.Write(isTaked);
			w.Write(isDate);
			w.Write(nickNameTalkCount);
			w.Write(myRoomCount);
			w.Write(menstruationStartDay);
			w.Write(menstruationDay);
			w.Write(hAreaExps.Length);
			float[] array = hAreaExps;
			foreach (float value in array)
			{
				w.Write(value);
			}
			w.Write(massageExps.Length);
			float[] array2 = massageExps;
			foreach (float value2 in array2)
			{
				w.Write(value2);
			}
			w.Write(isVirgin);
			w.Write(isAnalVirgin);
			w.Write(countKokanH);
			w.Write(countAnalH);
			w.Write(isKiss);
			w.Write(countNamaInsert);
			w.Write(countHoushi);
			w.Write(m_TalkEvent.Count);
			foreach (int item in m_TalkEvent)
			{
				w.Write(item);
			}
			w.Write(m_TalkTemper);
			w.Write(confessed);
			w.Write(motionSpeed.Count);
			foreach (KeyValuePair<string, float> item2 in motionSpeed)
			{
				w.Write(item2.Key);
				w.Write(item2.Value);
			}
			w.Write(houshiExp);
			w.Write(eventAfterDay);
			w.Write(isFirstGirlfriend);
			w.Write(intimacy);
		}

		public override void Load(Version version, BinaryReader r)
		{
			base.Load(version, r);
			favor = r.ReadInt32();
			lewdness = r.ReadInt32();
			hCount = r.ReadInt32();
			isStaff = r.ReadBoolean();
			isGirlfriend = r.ReadBoolean();
			isAnger = r.ReadBoolean();
			fixCharaID = r.ReadInt32();
			isTaked = r.ReadBoolean();
			isDate = r.ReadBoolean();
			nickNameTalkCount = r.ReadInt32();
			myRoomCount = r.ReadInt32();
			menstruationStartDay = r.ReadInt32();
			menstruationDay = r.ReadByte();
			int num = r.ReadInt32();
			hAreaExps = new float[num];
			for (int i = 0; i < num; i++)
			{
				hAreaExps[i] = r.ReadSingle();
			}
			num = r.ReadInt32();
			massageExps = new float[num];
			for (int j = 0; j < num; j++)
			{
				massageExps[j] = r.ReadSingle();
			}
			isVirgin = r.ReadBoolean();
			isAnalVirgin = r.ReadBoolean();
			countKokanH = r.ReadSingle();
			countAnalH = r.ReadSingle();
			isKiss = r.ReadBoolean();
			countNamaInsert = r.ReadInt32();
			countHoushi = r.ReadInt32();
			int num2 = r.ReadInt32();
			for (int k = 0; k < num2; k++)
			{
				m_TalkEvent.Add(r.ReadInt32());
			}
			m_TalkTemper = r.ReadBytes(39);
			confessed = r.ReadBoolean();
			if (version.CompareTo(new Version(0, 0, 4)) >= 0)
			{
				motionSpeed.Clear();
				int num3 = r.ReadInt32();
				for (int l = 0; l < num3; l++)
				{
					string key = r.ReadString();
					float value = r.ReadSingle();
					motionSpeed.Add(key, value);
				}
			}
			if (version.CompareTo(new Version(0, 0, 6)) >= 0)
			{
				houshiExp = r.ReadSingle();
			}
			if (version.CompareTo(new Version(0, 0, 7)) >= 0)
			{
				eventAfterDay = r.ReadInt32();
				isFirstGirlfriend = r.ReadBoolean();
			}
			if (version.CompareTo(new Version(1, 0, 1)) >= 0)
			{
				intimacy = r.ReadInt32();
			}
		}

		public override void Initialize()
		{
			int[] source = new int[6] { 7, 8, 9, 10, 11, 12 };
			for (int i = 0; i < m_TalkTemper.Length; i++)
			{
				m_TalkTemper[i] = (byte)UnityEngine.Random.Range(0, (!source.Contains(i - 1)) ? 3 : 2);
			}
		}

		public override void ChaFileUpdate()
		{
			base.charFile.parameter.sex = 1;
			if (Singleton<Communication>.IsInstance())
			{
				motionSpeed = Singleton<Communication>.Instance.GetMotionSpeed(base.personality);
			}
		}
	}

	[Serializable]
	public class Player : CharaData
	{
		public new class Params : CharaData.Params
		{
			private Player player
			{
				get
				{
					return base.chara as Player;
				}
			}

			public Params(Player player)
				: base(player, "P_")
			{
				_datas.AddRange(new List<Data>
				{
					new Data(Data.Command.Replace, "P", () => string.Empty),
					new Data(Data.Command.Replace, "P" + CharaData.Names[0], () => player.lastname),
					new Data(Data.Command.Replace, "P" + CharaData.Names[1], () => player.firstname),
					new Data(Data.Command.Replace, "P" + CharaData.Names[2], () => player.Name)
				});
			}
		}

		public const string ParamHead = "P";

		public const string ParamHeadName = "P_";

		private int? cachedVoiceNo;

		public int intellect;

		public int physical;

		public int hentai;

		public int actionCount = 5;

		public int girlfriendedCnt;

		public float playTimeCalc;

		public uint playTime;

		public int hcount;

		public int hPeopleCount;

		public int orgCount;

		public int changeClothesType;

		private Params _param;

		public override int voiceNo
		{
			get
			{
				if (!cachedVoiceNo.HasValue)
				{
					VoiceInfo.Param value;
					if (Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(-100, out value))
					{
						cachedVoiceNo = value.No;
					}
					else
					{
						cachedVoiceNo = base.personality;
					}
				}
				return cachedVoiceNo.Value;
			}
		}

		public Params param
		{
			get
			{
				return this.GetCache(ref _param, () => new Params(this));
			}
		}

		public Player()
			: base(false)
		{
		}

		public Player(ChaFileControl charFile, bool isRandomize)
			: base(charFile, isRandomize)
		{
		}

		public Player(bool isRandomize)
			: base(isRandomize)
		{
		}

		public Player(Version version, BinaryReader r)
			: base(false)
		{
			Load(version, r);
		}

		public override void Randomize()
		{
		}

		public override void SetADVParam(List<Program.Transfer> list)
		{
			param.CreateCommand(list);
		}

		public override void SetADVParam(TextScenario scenario, string key, object o)
		{
			CharaData.Params.Data data = param.datas.FirstOrDefault((CharaData.Params.Data p) => p.key == key);
			if (data != null)
			{
				ValData valData = new ValData(ValData.Convert(o, data.value.GetType()));
				data.value = valData.o;
				scenario.Vars[data.key] = valData;
			}
		}

		public override void SetADVParam(TextScenario scenario)
		{
			foreach (CharaData.Params.Data item in param.datas.Where((CharaData.Params.Data p) => p.command == CharaData.Params.Data.Command.Int || p.command == CharaData.Params.Data.Command.BOOL || p.command == CharaData.Params.Data.Command.String))
			{
				scenario.Vars[item.key] = new ValData(ValData.Convert(item.value, item.value.GetType()));
			}
			foreach (CharaData.Params.Data item2 in param.datas.Where((CharaData.Params.Data p) => p.command == CharaData.Params.Data.Command.Replace))
			{
				scenario.Replaces[item2.key] = (string)item2.value;
			}
		}

		public void AddHCount()
		{
			hcount = Mathf.Min(10000, hcount + 1);
		}

		public void AddOrgCount(int _org)
		{
			orgCount = Mathf.Min(10000, orgCount + _org);
		}

		public void AddHPeopleCount()
		{
			hPeopleCount = Mathf.Min(10000, hPeopleCount + 1);
		}

		public override void Save(BinaryWriter w)
		{
			base.Save(w);
			w.Write(girlfriendedCnt);
			w.Write(hPeopleCount);
			w.Write(orgCount);
			w.Write(hcount);
			w.Write(intellect);
			w.Write(physical);
			w.Write(hentai);
			w.Write(playTimeCalc);
			w.Write(changeClothesType);
			w.Write(playTime);
		}

		public override void Load(Version version, BinaryReader r)
		{
			base.Load(version, r);
			girlfriendedCnt = r.ReadInt32();
			hPeopleCount = r.ReadInt32();
			orgCount = r.ReadInt32();
			hcount = r.ReadInt32();
			if (version.CompareTo(new Version(0, 0, 1)) >= 0)
			{
				intellect = r.ReadInt32();
				physical = r.ReadInt32();
				hentai = r.ReadInt32();
			}
			if (version.CompareTo(new Version(0, 0, 2)) >= 0)
			{
				playTimeCalc = r.ReadSingle();
			}
			if (version.CompareTo(new Version(0, 0, 9)) >= 0)
			{
				changeClothesType = r.ReadInt32();
			}
			if (version.CompareTo(new Version(0, 0, 12)) >= 0)
			{
				playTime = r.ReadUInt32();
			}
		}

		public override void Initialize()
		{
			schoolClass = 1;
			schoolClassIndex = 20;
			callID = callDefaultID;
		}

		public override void ChaFileUpdate()
		{
			base.charFile.parameter.sex = 0;
		}
	}

	public class CallFileData
	{
		private string format;

		public string name { get; private set; }

		public string bundle { get; private set; }

		public CallFileData(NickName.Param param, string format)
		{
			name = param.Name;
			bundle = param.Bundle;
			this.format = format;
		}

		public string GetFileName(int i)
		{
			return string.Format(format, i);
		}
	}

	public class ClassHeroine
	{
		public int ID
		{
			get
			{
				return _id;
			}
		}

		private int _id { get; set; }

		public ClassHeroine()
		{
			_id = -1;
		}

		public ClassHeroine(int id)
		{
			_id = id;
		}

		public bool Check(CharaData chara)
		{
			if (_id == -1)
			{
				return false;
			}
			return _id == Convert(chara);
		}

		public Heroine Get(List<Heroine> list)
		{
			if (_id == -1)
			{
				return null;
			}
			return list.Find((Heroine item) => Check(item));
		}

		public void Set(CharaData chara)
		{
			_id = Convert(chara);
		}

		private static int Convert(CharaData chara)
		{
			if (chara == null || chara.schoolClass == -1)
			{
				return -1;
			}
			return chara.schoolClass * 100 + chara.schoolClassIndex;
		}
	}

	[Serializable]
	public class ClubReport
	{
		private int _staffAdd;

		private int _comAdd;

		private float _hAdd;

		private int _staff;

		private int _point;

		public int staffAdd
		{
			get
			{
				return _staffAdd;
			}
			set
			{
				_staffAdd = Mathf.Max(0, _staffAdd.ValueRound(value));
			}
		}

		public int comAdd
		{
			get
			{
				return _comAdd;
			}
			set
			{
				_comAdd = Mathf.Max(0, _comAdd.ValueRound(value));
			}
		}

		public float hAdd
		{
			get
			{
				return _hAdd;
			}
			set
			{
				_hAdd = Mathf.Max(0f, Mathf.Clamp(value, 0f, 9999999f));
			}
		}

		public int staff
		{
			get
			{
				return _staff;
			}
			set
			{
				_staff = Mathf.Max(0, _staff.ValueRound(value));
			}
		}

		public int point
		{
			get
			{
				return _point;
			}
			set
			{
				_point = Mathf.Max(0, _point.ValueRound(value));
			}
		}

		public void Save(BinaryWriter _bw)
		{
			_bw.Write(_staffAdd);
			_bw.Write(_comAdd);
			_bw.Write(_hAdd);
			_bw.Write(_staff);
			_bw.Write(_point);
		}

		public void Load(BinaryReader _br, Version _version)
		{
			_staffAdd = _br.ReadInt32();
			_comAdd = _br.ReadInt32();
			if (version.CompareTo(new Version(0, 0, 10)) >= 0)
			{
				_hAdd = _br.ReadSingle();
			}
			else
			{
				_hAdd = _br.ReadInt32();
			}
			_staff = _br.ReadInt32();
			_point = _br.ReadInt32();
		}

		public void ResetAddPoint()
		{
			_staffAdd = 0;
			_comAdd = 0;
			_staff = 0;
			_hAdd = 0f;
		}
	}

	private static Dictionary<string, List<NickName.Param>> _nicNameDic = null;

	private static Version version = new Version(1, 0, 1);

	private const string path = "save/game";

	public const string DefaultAccademyName = "コイカツ女学園";

	public string accademyName = "コイカツ女学園";

	public int emblemID;

	[SerializeField]
	private bool _isOpening = true;

	public int week = 6;

	public Player player = new Player(false);

	public List<Heroine> heroineList = new List<Heroine>();

	public HashSet<int> metPersonality = new HashSet<int>();

	[SerializeField]
	private int _clubPoint;

	public Dictionary<int, HashSet<int>> clubContents = new Dictionary<int, HashSet<int>>();

	public Dictionary<int, int> clubItemContents = new Dictionary<int, int>();

	public ClubReport clubReport = new ClubReport();

	private ClassHeroine _withHeroine = new ClassHeroine();

	private ClassHeroine _dateHeroine = new ClassHeroine();

	public static int callDefaultID { get; set; }

	public static string Path
	{
		get
		{
			return UserData.Path + "save/game";
		}
	}

	public static Dictionary<string, List<NickName.Param>> nicNameDic
	{
		get
		{
			return _nicNameDic ?? (_nicNameDic = LoadNickNameParam());
		}
	}

	public static Version Version
	{
		get
		{
			return version;
		}
	}

	public bool isOpening
	{
		get
		{
			return _isOpening;
		}
		set
		{
			_isOpening = value;
		}
	}

	public int clubPoint
	{
		get
		{
			return _clubPoint;
		}
	}

	public ClassHeroine withHeroine
	{
		get
		{
			return _withHeroine;
		}
	}

	public ClassHeroine dateHeroine
	{
		get
		{
			return _dateHeroine;
		}
	}

	public static void InitializeCall()
	{
	}

	private static Dictionary<string, List<NickName.Param>> LoadNickNameParam()
	{
		Dictionary<string, List<NickName.Param>> dic = new Dictionary<string, List<NickName.Param>>();
		Localize.Translate.Manager.SCENE_ID sceneID = Localize.Translate.Manager.SCENE_ID.NICK_NAME;
		Dictionary<int, Localize.Translate.Data.Param> ltData = Localize.Translate.Manager.LoadScene(sceneID, null).Get(1);
		CommonLib.GetAssetBundleNameListFromPath("etcetra/list/nickname/", true).ForEach(delegate(string file)
		{
			NickName[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(NickName)).GetAllAssets<NickName>();
			foreach (NickName nickName in allAssets)
			{
				List<NickName.Param> value;
				if (!dic.TryGetValue(nickName.name, out value))
				{
					value = (dic[nickName.name] = new List<NickName.Param>());
				}
				value.AddRange(nickName.param.Where((NickName.Param param) => !param.Name.IsNullOrEmpty() && param.ID != -1));
				foreach (NickName.Param param2 in value.Where((NickName.Param p) => p.isSpecial))
				{
					ltData.SafeGetText(param2.ID).SafeProc(delegate(string text)
					{
						param2.Name = text;
					});
				}
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
		Localize.Translate.Manager.DisposeScene(sceneID, false);
		return dic;
	}

	public static NickName.Param GetCallName(CharaData charaData)
	{
		return GetCallName(charaData, charaData.callMyID);
	}

	public static NickName.Param GetCallName(CharaData charaData, int id)
	{
		return GetCallName((!(charaData is Player)) ? charaData.personality : (-1), id);
	}

	public static NickName.Param GetCallName(int personality, int id)
	{
		return GetCallNameList(personality).Find((NickName.Param p) => p.ID == id);
	}

	public static List<NickName.Param> GetCallNameList(CharaData charaData)
	{
		return GetCallNameList((!(charaData is Player)) ? charaData.personality : (-1));
	}

	public static List<NickName.Param> GetCallNameList(int personality)
	{
		return nicNameDic[(personality < 0) ? "player" : string.Format("c{0:00}", personality)];
	}

	public static void CallNormalize(CharaData charaData)
	{
		int callID = charaData.callID;
		if (callID != -1)
		{
			NickName.Param callName = GetCallName(charaData, callID);
			if (callName != null)
			{
				if (charaData.callName != callName.Name)
				{
					charaData.callName = callName.Name;
				}
				return;
			}
		}
		charaData.callID = -1;
		NickName.Param callName2 = GetCallName(charaData);
		if (charaData.callName != callName2.Name)
		{
			charaData.callName = callName2.Name;
		}
	}

	public static CallFileData FindCallFileData(CharaData charaData)
	{
		return FindCallFileData((!(charaData is Player)) ? charaData.personality : (-1), charaData.callMyID);
	}

	public static CallFileData FindCallFileData(int personality, int id)
	{
		List<NickName.Param> callNameList = GetCallNameList(personality);
		NickName.Param param = callNameList.Find((NickName.Param pp) => pp.ID == id);
		string text = "namelist_";
		if (param.isSpecial)
		{
			text += "sp_";
			id -= (from pp in callNameList
				where pp.isSpecial
				select pp.ID).Min();
		}
		return new CallFileData(param, text + personality.ToString("00") + "_{0:00}_" + id.ToString("000"));
	}

	public static string LoadTitle(string fileName)
	{
		string ret = null;
		Utils.File.OpenRead(Path + '/' + fileName, delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				Version version = new Version(binaryReader.ReadString());
				if (version >= new Version(0, 0, 0))
				{
					ret = binaryReader.ReadString();
				}
			}
		});
		return ret;
	}

	public void ClubPointAdd(int add)
	{
		_clubPoint = Mathf.Max(0, _clubPoint.ValueRound(add));
	}

	public void Save(string fileName)
	{
		Save(UserData.Create("save/game"), fileName);
	}

	public void Save(string path, string fileName)
	{
		Utils.File.OpenWrite(path + fileName, false, delegate(FileStream f)
		{
			BinaryWriter w = new BinaryWriter(f);
			try
			{
				w.Write(version.ToString());
				w.Write(accademyName);
				w.Write(emblemID);
				w.Write(isOpening);
				w.Write(week);
				player.Save(w);
				w.Write(heroineList.Count);
				heroineList.ForEach(delegate(Heroine p)
				{
					p.Save(w);
				});
				w.Write(metPersonality.Count);
				foreach (int item in metPersonality)
				{
					w.Write(item);
				}
				w.Write(_clubPoint);
				w.Write(clubContents.Count);
				foreach (KeyValuePair<int, HashSet<int>> clubContent in clubContents)
				{
					w.Write(clubContent.Key);
					w.Write(clubContent.Value.Count);
					foreach (int item2 in clubContent.Value)
					{
						w.Write(item2);
					}
				}
				Dictionary<int, HashSet<int>> dictionary = Singleton<Game>.Instance.glSaveData.clubContents;
				foreach (KeyValuePair<int, HashSet<int>> clubContent2 in clubContents)
				{
					HashSet<int> value;
					if (!dictionary.TryGetValue(clubContent2.Key, out value))
					{
						value = (dictionary[clubContent2.Key] = new HashSet<int>());
					}
					foreach (int item3 in clubContent2.Value)
					{
						value.Add(item3);
					}
				}
				w.Write(clubItemContents.Count);
				foreach (KeyValuePair<int, int> clubItemContent in clubItemContents)
				{
					w.Write(clubItemContent.Key);
					w.Write(clubItemContent.Value);
				}
				clubReport.Save(w);
				w.Write(_withHeroine.ID);
				w.Write(_dateHeroine.ID);
				Singleton<Game>.Instance.actScene.actCtrl.Save(w);
			}
			finally
			{
				if (w != null)
				{
					((IDisposable)w).Dispose();
				}
			}
		});
		Singleton<Game>.Instance.glSaveData.Save();
		Singleton<Game>.Instance.weddingData.Save();
	}

	public bool Load(string fileName)
	{
		return Load(Path + '/', fileName);
	}

	public bool Load(string path, string fileName)
	{
		return Utils.File.OpenRead(path + fileName, delegate(FileStream f)
		{
			using (BinaryReader binaryReader = new BinaryReader(f))
			{
				int num = 0;
				Version version = new Version(binaryReader.ReadString());
				accademyName = binaryReader.ReadString();
				if (version >= new Version(0, 0, 5))
				{
					emblemID = binaryReader.ReadInt32();
				}
				isOpening = binaryReader.ReadBoolean();
				week = binaryReader.ReadInt32();
				week = Mathf.Min(week, Utils.Enum<Cycle.Week>.Length - 1);
				player = new Player(version, binaryReader);
				heroineList = new List<Heroine>();
				num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					heroineList.Add(new Heroine(version, binaryReader));
				}
				CallNormalize(player);
				foreach (Heroine item in heroineList.Where((Heroine item) => item.fixCharaID == 0))
				{
					CallNormalize(item);
				}
				metPersonality = new HashSet<int>();
				num = binaryReader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					metPersonality.Add(binaryReader.ReadInt32());
				}
				_clubPoint = binaryReader.ReadInt32();
				clubContents.Clear();
				num = binaryReader.ReadInt32();
				for (int k = 0; k < num; k++)
				{
					int key = binaryReader.ReadInt32();
					int num2 = binaryReader.ReadInt32();
					HashSet<int> hashSet = new HashSet<int>();
					for (int l = 0; l < num2; l++)
					{
						hashSet.Add(binaryReader.ReadInt32());
					}
					clubContents.Add(key, hashSet);
				}
				if (version.CompareTo(new Version(0, 0, 4)) >= 0)
				{
					clubItemContents.Clear();
					num = binaryReader.ReadInt32();
					for (int m = 0; m < num; m++)
					{
						clubItemContents.Add(binaryReader.ReadInt32(), binaryReader.ReadInt32());
					}
				}
				if (version.CompareTo(new Version(0, 0, 8)) >= 0)
				{
					clubReport.Load(binaryReader, version);
				}
				if (version.CompareTo(new Version(0, 0, 11)) >= 0)
				{
					_withHeroine = new ClassHeroine(binaryReader.ReadInt32());
				}
				if (version.CompareTo(new Version(0, 0, 13)) >= 0)
				{
					_dateHeroine = new ClassHeroine(binaryReader.ReadInt32());
				}
				if (version.CompareTo(new Version(1, 0, 0)) >= 0)
				{
					ActionControl.Load(binaryReader, version);
				}
			}
		});
	}
}
