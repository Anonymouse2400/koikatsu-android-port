using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class LoadVoice : LoadAudioBase
{
	public int no;

	public Transform voiceTrans;

	public Voice.Type type;

	public bool isPlayEndDelete = true;

	public float pitch = 1f;

	public bool is2D;

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
		if (base.clip == null)
		{
			yield break;
		}
		Singleton<Voice>.Instance.Bind(this);
		base.name = "Voice LoadEnd";
		if (base.audioSource == null)
		{
			Object.Destroy(base.gameObject);
			yield break;
		}
		if (settingNo < 0)
		{
			base.audioSource.loop = !isPlayEndDelete;
			if (!is2D)
			{
				base.audioSource.spatialBlend = ((voiceTrans != null) ? 1 : 0);
			}
			else
			{
				base.audioSource.spatialBlend = 0f;
			}
		}
		base.audioSource.pitch = pitch;
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
		if (base.clip == null)
		{
			yield break;
		}
		StartCoroutine(Play(null));
		this.UpdateAsObservable().TakeUntilDestroy(base.audioSource).Subscribe(delegate
		{
			base.audioSource.volume = Singleton<Voice>.Instance.GetVolume(no);
		});
		if (!(voiceTrans == null))
		{
			Transform audioTrans = base.audioSource.transform;
			this.UpdateAsObservable().TakeUntilDestroy(voiceTrans).TakeUntilDestroy(base.audioSource)
				.Subscribe(delegate
				{
					audioTrans.SetPositionAndRotation(voiceTrans.position, voiceTrans.rotation);
				});
			base.enabled = true;
		}
	}

	[DebuggerHidden]
	[CompilerGenerated]
	private IEnumerator _003C_Init_003E__BaseCallProxy0()
	{
		return base._Init();
	}
}
