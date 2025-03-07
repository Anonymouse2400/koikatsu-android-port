using System;
using System.Collections.Generic;
using System.IO;
using Manager;
using UnityEngine;

namespace Studio
{
	public class VoiceCtrl
	{
		public class VoiceInfo
		{
			public int group { get; private set; }

			public int category { get; private set; }

			public int no { get; private set; }

			public VoiceInfo(int _group, int _category, int _no)
			{
				group = _group;
				category = _category;
				no = _no;
			}
		}

		public enum Repeat
		{
			None = 0,
			All = 1,
			Select = 2
		}

		public const string savePath = "studio/voicelist";

		public const string saveExtension = ".dat";

		public const string saveIdentifyingCode = "【voice】";

		public List<VoiceInfo> list = new List<VoiceInfo>();

		public Repeat repeat;

		public int index = -1;

		private Transform m_TransformHead;

		private VoiceEndChecker voiceEndChecker;

		public OCIChar ociChar { get; set; }

		public Transform transVoice { get; private set; }

		public bool isPlay
		{
			get
			{
				return Singleton<Voice>.IsInstance() && Singleton<Voice>.Instance.IsVoiceCheck(personality, transHead);
			}
		}

		private int personality
		{
			get
			{
				return (ociChar != null) ? ociChar.charInfo.fileParam.personality : 0;
			}
		}

		private float pitch
		{
			get
			{
				return (ociChar == null) ? 1f : ociChar.charInfo.fileParam.voicePitch;
			}
		}

		private Transform transHead
		{
			get
			{
				if (m_TransformHead == null)
				{
					GameObject gameObject = ((ociChar == null) ? null : ociChar.charInfo.GetReferenceInfo(ChaReference.RefObjKey.HeadParent));
					m_TransformHead = ((!(gameObject != null)) ? null : gameObject.transform);
				}
				return m_TransformHead;
			}
		}

		public bool Play(int _idx)
		{
			if (!Singleton<Info>.IsInstance())
			{
				return false;
			}
			if (list.Count == 0)
			{
				return false;
			}
			if (!MathfEx.RangeEqualOn(0, _idx, list.Count - 1))
			{
				index = -1;
				return false;
			}
			Stop();
			VoiceInfo voiceInfo = list[_idx];
			Info.LoadCommonInfo loadInfo = GetLoadInfo(voiceInfo.group, voiceInfo.category, voiceInfo.no);
			if (loadInfo == null)
			{
				return false;
			}
			Voice instance = Singleton<Voice>.Instance;
			int no = personality;
			string bundlePath = loadInfo.bundlePath;
			string fileName = loadInfo.fileName;
			float num = pitch;
			Transform voiceTrans = transHead;
			transVoice = instance.Play(no, bundlePath, fileName, num, 0f, 0f, true, voiceTrans);
			if (transVoice == null)
			{
				return false;
			}
			index = _idx;
			voiceEndChecker = transVoice.gameObject.AddComponent<VoiceEndChecker>();
			VoiceEndChecker obj = voiceEndChecker;
			obj.onEndFunc = (VoiceEndChecker.OnEndFunc)Delegate.Combine(obj.onEndFunc, new VoiceEndChecker.OnEndFunc(NextVoicePlay));
			ociChar.SetVoice();
			return true;
		}

		public void Stop()
		{
			if (voiceEndChecker != null)
			{
				voiceEndChecker.onEndFunc = null;
			}
			if (transVoice != null)
			{
				Singleton<Voice>.Instance.Stop(personality, transHead);
			}
			transVoice = null;
			ociChar.SetVoice();
		}

		public void Save(BinaryWriter _writer, Version _version)
		{
			int count = list.Count;
			_writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				VoiceInfo voiceInfo = list[i];
				_writer.Write(voiceInfo.group);
				_writer.Write(voiceInfo.category);
				_writer.Write(voiceInfo.no);
			}
			_writer.Write((int)repeat);
		}

		public void Load(BinaryReader _reader, Version _version)
		{
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int group = _reader.ReadInt32();
				int category = _reader.ReadInt32();
				int no = _reader.ReadInt32();
				if (GetLoadInfo(group, category, no) != null)
				{
					list.Add(new VoiceInfo(group, category, no));
				}
			}
			repeat = (Repeat)_reader.ReadInt32();
		}

		public void SaveList(string _name)
		{
			string path = UserData.Create("studio/voicelist") + Utility.GetCurrentTime() + ".dat";
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					binaryWriter.Write("【voice】");
					binaryWriter.Write(_name);
					int count = list.Count;
					binaryWriter.Write(count);
					for (int i = 0; i < count; i++)
					{
						VoiceInfo voiceInfo = list[i];
						binaryWriter.Write(voiceInfo.group);
						binaryWriter.Write(voiceInfo.category);
						binaryWriter.Write(voiceInfo.no);
					}
				}
			}
		}

		public bool LoadList(string _path, bool _import = false)
		{
			if (!_import)
			{
				list.Clear();
			}
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					if (string.Compare(binaryReader.ReadString(), "【voice】") != 0)
					{
						return false;
					}
					binaryReader.ReadString();
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						int group = binaryReader.ReadInt32();
						int category = binaryReader.ReadInt32();
						int no = binaryReader.ReadInt32();
						if (GetLoadInfo(group, category, no) != null)
						{
							list.Add(new VoiceInfo(group, category, no));
						}
					}
				}
			}
			return true;
		}

		public static string LoadListName(string _path)
		{
			string empty = string.Empty;
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					if (string.Compare(binaryReader.ReadString(), "【voice】") != 0)
					{
						return string.Empty;
					}
					return binaryReader.ReadString();
				}
			}
		}

		public static bool CheckIdentifyingCode(string _path)
		{
			bool result = true;
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					if (string.Compare(binaryReader.ReadString(), "【voice】") != 0)
					{
						result = false;
					}
				}
			}
			return result;
		}

		private Info.LoadCommonInfo GetLoadInfo(int _group, int _category, int _no)
		{
			Dictionary<int, Dictionary<int, Info.LoadCommonInfo>> value = null;
			if (!Singleton<Info>.Instance.dicVoiceLoadInfo.TryGetValue(_group, out value))
			{
				return null;
			}
			Dictionary<int, Info.LoadCommonInfo> value2 = null;
			if (!value.TryGetValue(_category, out value2))
			{
				return null;
			}
			Info.LoadCommonInfo value3 = null;
			if (!value2.TryGetValue(_no, out value3))
			{
				return null;
			}
			return value3;
		}

		private void NextVoicePlay()
		{
			transVoice = null;
			switch (repeat)
			{
			case Repeat.None:
				index++;
				Play(index);
				break;
			case Repeat.All:
				if (list.Count != 0)
				{
					index = (index + 1) % list.Count;
					Play(index);
				}
				break;
			case Repeat.Select:
				Play(index);
				break;
			}
		}
	}
}
