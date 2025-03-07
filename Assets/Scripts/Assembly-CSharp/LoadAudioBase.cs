using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Manager;
using Sound;
using UnityEngine;

public abstract class LoadAudioBase : AssetLoader
{
	public float delayTime;

	public float fadeTime;

	public int settingNo = -1;

	protected bool isReleaseClip = true;

	public AudioClip clip { get; private set; }

	public AudioSource audioSource { get; private set; }

	public void Init(AudioSource audioSource)
	{
		this.audioSource = audioSource;
		base.transform.SetParent(audioSource.transform, false);
		Init();
	}

	public override IEnumerator _Init()
	{
		if (base.initialized)
		{
			yield break;
		}
		if (isAsync)
		{
			yield return StartCoroutine(_003C_Init_003E__BaseCallProxy0());
		}
		else
		{
			StartCoroutine(_003C_Init_003E__BaseCallProxy0());
		}
		clip = base.loadObject as AudioClip;
		if (clip == null)
		{
			if (audioSource != null)
			{
				Object.Destroy(audioSource.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	protected IEnumerator Play(GameObject fadeOut)
	{
		if (audioSource.playOnAwake)
		{
			yield break;
		}
		yield return new WaitUntil(() => audioSource.clip.loadState == AudioDataLoadState.Loaded);
		if (delayTime > 0f)
		{
			yield return new WaitForSecondsRealtime(delayTime);
		}
		while (!audioSource.isActiveAndEnabled)
		{
			yield return null;
		}
		if (fadeTime > 0f)
		{
			Manager.Sound.PlayFade(fadeOut, audioSource, fadeTime);
		}
		else
		{
			audioSource.Play();
		}
		if (!audioSource.loop)
		{
			float pitch = audioSource.pitch;
			if (pitch == 0f)
			{
				pitch = 1f;
			}
			float endTime = (audioSource.clip.length - audioSource.time) * (1f / pitch);
			if (endTime > 0f)
			{
				yield return new WaitForSecondsRealtime(endTime);
			}
			FadePlayer fadePlay = audioSource.GetComponent<FadePlayer>();
			if (fadePlay != null)
			{
				fadePlay.Stop(fadeTime);
			}
			else
			{
				Object.Destroy(audioSource.gameObject);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (isReleaseClip && Singleton<Manager.Sound>.IsInstance())
		{
			Singleton<Manager.Sound>.Instance.Remove(clip);
		}
	}

	[Conditional("BASE_LOADER_LOG")]
	private void LogError(string str)
	{
	}

	[DebuggerHidden]
	[CompilerGenerated]
	private IEnumerator _003C_Init_003E__BaseCallProxy0()
	{
		return base._Init();
	}
}
