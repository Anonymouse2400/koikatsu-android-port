using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Manager;
using UnityEngine;

public class LoadSound : LoadAudioBase
{
	public Manager.Sound.Type type = Manager.Sound.Type.GameSE2D;

	public bool isAssetEqualPlay = true;

	public override IEnumerator _Init()
	{
		if (base.initialized)
		{
			yield break;
		}
		if (!isAssetEqualPlay && Singleton<Manager.Sound>.Instance.FindAsset(type, assetName, assetBundleName) != null)
		{
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				if (parent.GetComponent<AudioSource>() != null)
				{
					isReleaseClip = false;
					Object.Destroy(parent.gameObject);
					base.initialized = true;
					yield break;
				}
			}
			else if (base.transform.GetComponent<LoadAudioBase>() != null)
			{
				isReleaseClip = false;
				Object.Destroy(base.gameObject);
				base.initialized = true;
				yield break;
			}
		}
		if (isAsync)
		{
			yield return StartCoroutine(_003C_Init_003E__BaseCallProxy0());
		}
		else
		{
			StartCoroutine(_003C_Init_003E__BaseCallProxy0());
		}
		if (!(base.clip == null))
		{
			Singleton<Manager.Sound>.Instance.Bind(this);
			base.name = "Sound LoadEnd";
		}
	}

	protected override IEnumerator Start()
	{
		base.enabled = false;
		if (base.audioSource == null)
		{
			yield return StartCoroutine(_Init());
		}
		while (!base.isLoadEnd)
		{
			yield return null;
		}
		if (!(base.clip == null))
		{
			GameObject fadeOut = null;
			if (type == Manager.Sound.Type.BGM)
			{
				fadeTime = Mathf.Max(fadeTime, 0.01f);
				fadeOut = Singleton<Manager.Sound>.Instance.currentBGM;
				Singleton<Manager.Sound>.Instance.currentBGM = base.audioSource.gameObject;
			}
			StartCoroutine(Play(fadeOut));
			base.enabled = true;
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003C_Init_003E__BaseCallProxy0()
	{
		return base._Init();
	}
}
