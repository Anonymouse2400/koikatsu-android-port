using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Game;
using Manager;
using UnityEngine;

namespace ActionGame
{
	public class MotionVoice
	{
		public enum ActionKind
		{
			none = -1,
			masturbation_found = 0
		}

		private enum Kind
		{
			none = -1,
			breath = 0,
			voice = 1
		}

		private class Path
		{
			public string aseset;

			public string file;

			public string face;

			public bool isUse;
		}

		private class Breath
		{
			public int group = -1;

			public Dictionary<int, Path> dicPath = new Dictionary<int, Path>();
		}

		private class Voice
		{
			public bool overWirte;

			public int playRate = 100;

			public Dictionary<int, Path> dicPath = new Dictionary<int, Path>();
		}

		private SaveData.Heroine heroine;

		private ChaControl chactrl;

		private Dictionary<string, Breath> dicBreath = new Dictionary<string, Breath>();

		private Dictionary<string, Dictionary<string, Voice>> dicVoice = new Dictionary<string, Dictionary<string, Voice>>();

		private Dictionary<int, Dictionary<int, Path>> dicVoiceAction = new Dictionary<int, Dictionary<int, Path>>();

		private int oldnameHash;

		private float oldNormalizeTime;

		private int nowBreathGroup = -1;

		private Kind nowPlayKind = Kind.none;

		private ActionKind _actionKind = ActionKind.none;

		public ActionKind actionKind
		{
			get
			{
				return _actionKind;
			}
			set
			{
				_actionKind = value;
			}
		}

		public bool Init(SaveData.Heroine _heroine)
		{
			heroine = _heroine;
			oldnameHash = 0;
			oldNormalizeTime = 0f;
			nowBreathGroup = -1;
			nowPlayKind = Kind.none;
			_actionKind = ActionKind.none;
			heroine.SafeProc(delegate(SaveData.Heroine h)
			{
				chactrl = h.chaCtrl;
			});
			LoadBreathList();
			LoadVoiceList();
			return true;
		}

		public bool Proc()
		{
			if (heroine == null || chactrl == null)
			{
				return false;
			}
			AnimatorStateInfo asi = default(AnimatorStateInfo);
			chactrl.animBody.SafeProc(delegate(Animator anim)
			{
				asi = anim.GetCurrentAnimatorStateInfo(0);
			});
			if (VoiceProc(asi))
			{
				return true;
			}
			BreathProc(asi);
			return true;
		}

		private bool LoadBreathList()
		{
			dicBreath.Clear();
			if (heroine == null)
			{
				return false;
			}
			string strLoadFile = "motionvoice_breath_" + heroine.personality.ToString("00") + "_list";
			string text = GlobalMethod.LoadAllListText("action/list/motionvoice/", strLoadFile);
			if (text.IsNullOrEmpty())
			{
				return false;
			}
			string[,] data;
			GlobalMethod.GetListString(text, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				int num = 0;
				string key = data[i, num++];
				if (!dicBreath.ContainsKey(key))
				{
					dicBreath.Add(key, new Breath());
				}
				Breath breath = dicBreath[key];
				breath.group = int.Parse(data[i, num++]);
				while (length2 > num)
				{
					string text2 = data[i, num++];
					if (text2.IsNullOrEmpty())
					{
						break;
					}
					int key2 = int.Parse(text2);
					if (!breath.dicPath.ContainsKey(key2))
					{
						breath.dicPath.Add(key2, new Path());
					}
					Path path = breath.dicPath[key2];
					path.aseset = string.Format(data[i, num++], "c" + heroine.personality.ToString("00"));
					path.file = string.Format(data[i, num++], heroine.personality.ToString("00"));
					path.face = data[i, num++];
				}
			}
			return true;
		}

		private bool LoadVoiceList()
		{
			dicVoice.Clear();
			if (heroine == null)
			{
				return false;
			}
			string strLoadFile = "motionvoice_voice_" + heroine.personality.ToString("00") + "_list";
			string text = GlobalMethod.LoadAllListText("action/list/motionvoice/", strLoadFile);
			if (text.IsNullOrEmpty())
			{
				return false;
			}
			string[,] data;
			GlobalMethod.GetListString(text, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				int num = 0;
				string key = data[i, num++];
				if (!dicVoice.ContainsKey(key))
				{
					dicVoice.Add(key, new Dictionary<string, Voice>());
				}
				Dictionary<string, Voice> dictionary = dicVoice[key];
				string key2 = data[i, num++];
				if (!dictionary.ContainsKey(key2))
				{
					dictionary.Add(key2, new Voice());
				}
				dictionary[key2].overWirte = data[i, num++] == "1";
				string text2 = data[i, num++];
				if (text2.IsNullOrEmpty())
				{
					dictionary.Remove(key2);
					if (dictionary.Count == 0)
					{
						dicVoice.Remove(key);
					}
					continue;
				}
				dictionary[key2].playRate = int.Parse(text2);
				Dictionary<int, Path> dicPath = dictionary[key2].dicPath;
				while (length2 > num)
				{
					string text3 = data[i, num++];
					if (text3.IsNullOrEmpty())
					{
						break;
					}
					int key3 = int.Parse(text3);
					if (!dicPath.ContainsKey(key3))
					{
						dicPath.Add(key3, new Path());
					}
					Path path = dicPath[key3];
					path.aseset = string.Format(data[i, num++], "c" + heroine.personality.ToString("00"));
					path.file = string.Format(data[i, num++], heroine.personality.ToString("00"));
					path.face = data[i, num++];
				}
			}
			return true;
		}

		private bool LoadVoiceActionList()
		{
			dicVoiceAction.Clear();
			if (heroine == null)
			{
				return false;
			}
			string strLoadFile = "motionvoice_voice_action_" + heroine.personality.ToString("00") + "_list";
			string text = GlobalMethod.LoadAllListText("action/list/motionvoice/", strLoadFile);
			if (text.IsNullOrEmpty())
			{
				return false;
			}
			string[,] data;
			GlobalMethod.GetListString(text, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				int num = 0;
				int key = int.Parse(data[i, num++]);
				if (!dicVoiceAction.ContainsKey(key))
				{
					dicVoiceAction.Add(key, new Dictionary<int, Path>());
				}
				Dictionary<int, Path> dictionary = dicVoiceAction[key];
				while (length2 > num)
				{
					string text2 = data[i, num++];
					if (text2.IsNullOrEmpty())
					{
						break;
					}
					int key2 = int.Parse(text2);
					if (!dictionary.ContainsKey(key2))
					{
						dictionary.Add(key2, new Path());
					}
					Path path = dictionary[key2];
					path.aseset = string.Format(data[i, num++], "c" + heroine.personality.ToString("00"));
					path.file = string.Format(data[i, num++], heroine.personality.ToString("00"));
					path.face = data[i, num++];
				}
			}
			return true;
		}

		private bool BreathProc(AnimatorStateInfo _asi)
		{
			if (nowPlayKind == Kind.voice && Singleton<Manager.Voice>.Instance.IsVoiceCheck(heroine.charaBase.Head))
			{
				return false;
			}
			foreach (KeyValuePair<string, Breath> item in dicBreath)
			{
				if (!_asi.IsName(item.Key))
				{
					continue;
				}
				if (nowPlayKind == Kind.breath && nowBreathGroup == item.Value.group && Singleton<Manager.Voice>.Instance.IsVoiceCheck(heroine.charaBase.Head))
				{
					break;
				}
				List<Path> list = new List<Path>();
				foreach (KeyValuePair<int, Path> item2 in item.Value.dicPath)
				{
					if (!item2.Value.isUse)
					{
						list.Add(item2.Value);
					}
				}
				if (list.Count == 0)
				{
					foreach (KeyValuePair<int, Path> item3 in item.Value.dicPath)
					{
						item3.Value.isUse = false;
						list.Add(item3.Value);
					}
				}
				if (list.Count == 0)
				{
					return false;
				}
				Path path = list.OrderBy((Path _) => Guid.NewGuid()).FirstOrDefault();
				Utils.Voice.Setting setting = new Utils.Voice.Setting();
				setting.no = heroine.voiceNo;
				setting.assetBundleName = path.aseset;
				setting.assetName = path.file;
				setting.pitch = heroine.voicePitch;
				setting.voiceTrans = heroine.charaBase.Head;
				Utils.Voice.Setting s = setting;
				heroine.chaCtrl.SetVoiceTransform(Utils.Voice.OnecePlayChara(s));
				path.isUse = true;
				Game.Expression expression = Singleton<Game>.Instance.GetExpression(heroine.FixCharaIDOrPersonality, path.face);
				if (expression != null)
				{
					expression.Change(heroine.chaCtrl);
				}
				nowPlayKind = Kind.breath;
				nowBreathGroup = item.Value.group;
				break;
			}
			return true;
		}

		private bool VoiceProc(AnimatorStateInfo _asi)
		{
			if (oldnameHash != _asi.fullPathHash)
			{
				oldNormalizeTime = 0f;
			}
			float num = _asi.normalizedTime % 1f;
			if (VoiceActionProc())
			{
				oldNormalizeTime = num;
				oldnameHash = _asi.fullPathHash;
				return true;
			}
			foreach (KeyValuePair<string, Dictionary<string, Voice>> item in dicVoice)
			{
				if (!_asi.IsName(item.Key))
				{
					continue;
				}
				foreach (KeyValuePair<string, Voice> item2 in item.Value)
				{
					float num2 = float.Parse(item2.Key);
					if (oldNormalizeTime > num2 || num2 >= num)
					{
						continue;
					}
					if (nowPlayKind == Kind.voice && !item2.Value.overWirte && Singleton<Manager.Voice>.Instance.IsVoiceCheck(heroine.charaBase.Head))
					{
						break;
					}
					GlobalMethod.RatioRand ratioRand = new GlobalMethod.RatioRand();
					ratioRand.Add(0, item2.Value.playRate);
					if (item2.Value.playRate != 100)
					{
						ratioRand.Add(1, 100 - item2.Value.playRate);
					}
					if (ratioRand.Random() != 0)
					{
						break;
					}
					List<Path> list = new List<Path>();
					foreach (KeyValuePair<int, Path> item3 in item2.Value.dicPath)
					{
						if (!item3.Value.isUse)
						{
							list.Add(item3.Value);
						}
					}
					if (list.Count == 0)
					{
						foreach (KeyValuePair<int, Path> item4 in item2.Value.dicPath)
						{
							item4.Value.isUse = false;
							list.Add(item4.Value);
						}
					}
					if (list.Count == 0)
					{
						break;
					}
					Path path = list.OrderBy((Path _) => Guid.NewGuid()).FirstOrDefault();
					Utils.Voice.Setting setting = new Utils.Voice.Setting();
					setting.no = heroine.voiceNo;
					setting.assetBundleName = path.aseset;
					setting.assetName = path.file;
					setting.pitch = heroine.voicePitch;
					setting.voiceTrans = heroine.charaBase.Head;
					Utils.Voice.Setting s = setting;
					heroine.chaCtrl.SetVoiceTransform(Utils.Voice.OnecePlayChara(s));
					path.isUse = true;
					Game.Expression expression = Singleton<Game>.Instance.GetExpression(heroine.FixCharaIDOrPersonality, path.face);
					if (expression != null)
					{
						expression.Change(heroine.chaCtrl);
					}
					oldNormalizeTime = num;
					oldnameHash = _asi.fullPathHash;
					nowPlayKind = Kind.voice;
					return true;
				}
				break;
			}
			oldNormalizeTime = num;
			oldnameHash = _asi.fullPathHash;
			return false;
		}

		private bool VoiceActionProc()
		{
			if (_actionKind == ActionKind.none)
			{
				return false;
			}
			Dictionary<int, Path> value = null;
			if (!dicVoiceAction.TryGetValue((int)_actionKind, out value))
			{
				return false;
			}
			List<Path> list = new List<Path>();
			foreach (Path value2 in value.Values)
			{
				if (!value2.isUse)
				{
					list.Add(value2);
				}
			}
			if (list.Count == 0)
			{
				foreach (Path value3 in value.Values)
				{
					value3.isUse = false;
					list.Add(value3);
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			Path path = list.OrderBy((Path _) => Guid.NewGuid()).FirstOrDefault();
			Utils.Voice.Setting setting = new Utils.Voice.Setting();
			setting.no = heroine.voiceNo;
			setting.assetBundleName = path.aseset;
			setting.assetName = path.file;
			setting.pitch = heroine.voicePitch;
			setting.voiceTrans = heroine.charaBase.Head;
			Utils.Voice.Setting s = setting;
			heroine.chaCtrl.SetVoiceTransform(Utils.Voice.OnecePlayChara(s));
			path.isUse = true;
			Game.Expression expression = Singleton<Game>.Instance.GetExpression(heroine.FixCharaIDOrPersonality, path.face);
			if (expression != null)
			{
				expression.Change(heroine.chaCtrl);
			}
			nowPlayKind = Kind.voice;
			_actionKind = ActionKind.none;
			return true;
		}
	}
}
