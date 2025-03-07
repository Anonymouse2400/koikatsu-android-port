using System.Collections;
using System.Collections.Generic;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace WavInfoControl
{
	public class LipDataCreate : MonoBehaviour
	{
		[SerializeField]
		private Text text;

		[SerializeField]
		private AudioSource sound;

		[SerializeField]
		private float correct = 2f;

		[Button("Create", "波形情報作成", new object[] { })]
		public int create;

		[Button("Play", "波形情報再生", new object[] { })]
		public int play;

		private float beforeVolume;

		private float pos;

		public WavInfoData wavInfo;

		private void Create()
		{
			if (!(null == sound))
			{
				StartCoroutine(CreateCoroutine());
			}
		}

		private IEnumerator CreateCoroutine()
		{
			wavInfo = new WavInfoData();
			pos = 0f;
			sound.Play();
			List<float> lstVal = new List<float>();
			while (sound.clip.length > pos)
			{
				sound.time = pos;
				float val = GetAudioWaveValue(sound, correct);
				text.text = string.Format("時間:{0:000.00}\n現在:{1:000.00},\u3000値：{2:0.00}", sound.clip.length, sound.time, val);
				lstVal.Add(val);
				pos += wavInfo.span;
				yield return null;
			}
			lstVal.Add(0f);
			wavInfo.value = lstVal.ToArray();
			sound.Stop();
			SaveInfo();
		}

		private void Play()
		{
			if (LoadInfo())
			{
				StartCoroutine(PlayCoroutine());
			}
		}

		private IEnumerator PlayCoroutine()
		{
			bool first = true;
			sound.Play();
			while (sound.isPlaying || first)
			{
				first = false;
				float val = wavInfo.GetValue(sound.time);
				text.text = string.Format("時間:{0:000.00}\n現在:{1:000.00},\u3000値：{2:0.00}", sound.clip.length, sound.time, val);
				yield return null;
			}
			sound.Stop();
		}

		private float RMS(ref float[] samples)
		{
			float num = 0f;
			for (int i = 0; i < samples.Length; i++)
			{
				num += samples[i] * samples[i];
			}
			num /= (float)samples.Length;
			return Mathf.Sqrt(num);
		}

		public float GetAudioWaveValue(AudioSource audioSource, float correct = 2f)
		{
			float result = 0f;
			if (!audioSource.clip)
			{
				return result;
			}
			if (audioSource.isPlaying)
			{
				float[] samples = new float[1024];
				float num = 0f;
				float max = 1f;
				audioSource.GetOutputData(samples, 0);
				num = RMS(ref samples);
				float num2 = Mathf.Clamp(num * correct, 0f, max);
				result = ((!(num2 < beforeVolume)) ? ((num2 + beforeVolume) * 0.5f) : (num2 * 0.2f + beforeVolume * 0.8f));
				beforeVolume = num2;
			}
			return result;
		}

		public bool SaveInfo()
		{
			string path = Application.dataPath + "/../WorkSpace/soundinfo.bytes";
			if (wavInfo == null)
			{
				return false;
			}
			return wavInfo.Save(path);
		}

		public bool LoadInfo()
		{
			string path = Application.dataPath + "/../WorkSpace/soundinfo.bytes";
			wavInfo = new WavInfoData();
			return wavInfo.Load(path);
		}
	}
}
