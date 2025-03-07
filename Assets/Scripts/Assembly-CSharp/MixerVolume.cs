using UnityEngine;
using UnityEngine.Audio;

public static class MixerVolume
{
	public enum Names
	{
		MasterVolume = 0,
		BGMVolume = 1,
		PCMVolume = 2,
		ENVVolume = 3,
		GameSEVolume = 4,
		SystemSEVolume = 5
	}

	public static void Set(AudioMixer mixer, Names name, float volume)
	{
		float value = 20f * Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f));
		mixer.SetFloat(name.ToString(), Mathf.Clamp(value, -80f, 0f));
	}

	public static float Get(AudioMixer mixer, Names name)
	{
		float value = 0f;
		if (!mixer.GetFloat(name.ToString(), out value))
		{
			return 0f;
		}
		return Mathf.InverseLerp(-80f, 0f, value);
	}
}
