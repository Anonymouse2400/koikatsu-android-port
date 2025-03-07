using System;
using System.IO;
using Manager;
using UnityEngine;

namespace Studio
{
	public class ENVCtrl
	{
		private BGMCtrl.Repeat m_Repeat = BGMCtrl.Repeat.All;

		private int m_No;

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

		public int no
		{
			get
			{
				return m_No;
			}
			set
			{
				if (m_No != value)
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
			Play(m_No);
		}

		public void Play(int _no)
		{
			m_No = _no;
			if (!m_Play)
			{
				return;
			}
			Info.LoadCommonInfo value = null;
			if (Singleton<Info>.Instance.dicENVLoadInfo.TryGetValue(m_No, out value))
			{
				Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.ENV);
				Transform transform = Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.ENV, value.bundlePath, value.fileName);
				if (!(transform == null))
				{
					audioSource = transform.GetComponent<AudioSource>();
					audioSource.loop = repeat == BGMCtrl.Repeat.All;
					audioSource.spatialBlend = 0f;
				}
			}
		}

		public void Stop()
		{
			m_Play = false;
			Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.ENV);
		}

		public void Save(BinaryWriter _writer, Version _version)
		{
			_writer.Write((int)m_Repeat);
			_writer.Write(m_No);
			_writer.Write(m_Play);
		}

		public void Load(BinaryReader _reader, Version _version)
		{
			m_Repeat = (BGMCtrl.Repeat)_reader.ReadInt32();
			m_No = _reader.ReadInt32();
			m_Play = _reader.ReadBoolean();
		}
	}
}
