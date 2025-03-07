using UnityEngine;

namespace MorphAssist
{
	public class AudioAssist
	{
		private float beforeVolume;

		private float RMS(float[] samples)
		{
			float num = 0f;
			for (int i = 0; i < samples.Length; i++)
			{
				num += samples[i] * samples[i];
			}
			num /= (float)samples.Length;
			return Mathf.Sqrt(num);
		}

		public float GetAudioWaveValue(AudioSource audioSource)
		{
			float result = 0f;
			if (!audioSource.clip)
			{
				return result;
			}
			if (audioSource.isPlaying)
			{
				float[] samples = new float[256];
				float num = 0f;
				float max = 1f;
				audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
				num = RMS(samples);
				result = Mathf.Clamp(num * 50f, 0f, max);
				result = (beforeVolume = (result + beforeVolume) * 0.5f);
			}
			return result;
		}
	}
}
