using System;
using System.IO;
using Manager;
using UnityEngine;

namespace Studio
{
	public class OutsideSoundCtrl
	{
		public const string dataPath = "audio";

		private BGMCtrl.Repeat m_Repeat = BGMCtrl.Repeat.All;

		private string m_FileName = string.Empty;

		private bool m_Play;

		private AudioSource audioSource;

		public BGMCtrl.Repeat repeat
		{
			get
			{
				return m_Repeat;
			}
			set
			{
				if (Utility.SetStruct(ref m_Repeat, value) && (bool)audioSource)
				{
					audioSource.loop = repeat == BGMCtrl.Repeat.All;
				}
			}
		}

		public string fileName
		{
			get
			{
				return m_FileName;
			}
			set
			{
				if (m_FileName != value)
				{
					Play(value);
				}
			}
		}

		public bool play
		{
			get
			{
				return m_Play;
			}
			set
			{
				if (Utility.SetStruct(ref m_Play, value))
				{
					if (m_Play)
					{
						Play();
					}
					else
					{
						Stop();
					}
				}
			}
		}

		public void Play()
		{
			m_Play = true;
			Play(m_FileName);
		}

		public void Play(string _file)
		{
			m_FileName = _file;
			if (!m_Play)
			{
				return;
			}
			string path = UserData.Create("audio") + _file;
			if (File.Exists(path))
			{
				if (Singleton<Studio>.Instance.bgmCtrl.play)
				{
					Singleton<Studio>.Instance.bgmCtrl.Stop();
				}
				Singleton<Manager.Sound>.Instance.StopBGM();
				AudioClip audioClip = ExternalAudioClip.Load(path);
				if (!(audioClip == null))
				{
					Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.BGM, audioClip);
				}
			}
		}

		public void Stop()
		{
			m_Play = false;
			Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.BGM);
		}

		public void Save(BinaryWriter _writer, Version _version)
		{
			_writer.Write((int)m_Repeat);
			_writer.Write(m_FileName);
			_writer.Write(m_Play);
		}

		public void Load(BinaryReader _reader, Version _version)
		{
			m_Repeat = (BGMCtrl.Repeat)_reader.ReadInt32();
			m_FileName = _reader.ReadString();
			m_Play = _reader.ReadBoolean();
		}
	}
}
