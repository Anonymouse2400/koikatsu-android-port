using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Events;

public class HTestPeeping : HTestActionBase
{
	public UnityAction<SaveData.Heroine> OnChange;

	private float oldFrame;

	private PeepingFrameCtrl frameCtrl = new PeepingFrameCtrl();

	private HFlag.TimeWait timeVoice = new HFlag.TimeWait();

	public HTestPeeping(HFlag.DeliveryMember _membar)
		: base(_membar)
	{
		frameCtrl.Init(_membar.chaFemale, _membar.sprite, _membar.particle);
		timeVoice.MemberInit();
	}

	public void SetMapObject(GameObject _objMap)
	{
		frameCtrl.SetMapObj(_objMap);
	}

	public override bool MotionChange(int _motion)
	{
		flags.timeMasturbation.SetIdleTime();
		sprite.imageSpeedSlliderCover70.enabled = false;
		frameCtrl.Load("h/list/", _motion);
		oldFrame = 0f;
		timeVoice.SetIdleTime();
		return true;
	}

	public override bool Proc()
	{
		AnimatorStateInfo currentAnimatorStateInfo = female.animBody.GetCurrentAnimatorStateInfo(0);
		frameCtrl.Proc(currentAnimatorStateInfo);
		float num = currentAnimatorStateInfo.normalizedTime % 1f;
		if (flags.nowAnimationInfo.numCtrl == 0)
		{
			if (!sprite.isFade)
			{
				HSprite.FadeKindProc fadeKindProc = sprite.GetFadeKindProc();
				if (fadeKindProc != HSprite.FadeKindProc.OutEnd)
				{
					SetMoveCameraFlag();
					if (currentAnimatorStateInfo.IsName("In"))
					{
						if (currentAnimatorStateInfo.normalizedTime >= 1f)
						{
							SetPlay("Loop", false);
							flags.voice.playVoices[0] = 501;
						}
					}
					else if (currentAnimatorStateInfo.IsName("Loop") && currentAnimatorStateInfo.normalizedTime >= 2f)
					{
						SetPlay("Out", false);
					}
				}
				else if (fadeKindProc == HSprite.FadeKindProc.OutEnd)
				{
					GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, false);
					female.animBody.speed = 0f;
					item.SetAnimationSpeed(0f);
					if (Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[0]);
					}
					if (!sprite.peeping.imageReplayCheck.gameObject.activeSelf)
					{
						sprite.peeping.imageReplayCheck.gameObject.SetActive(true);
					}
					if (flags.click == HFlag.ClickKind.peeping_restart)
					{
						GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, true);
						SetPlay("In", false);
						sprite.FadeState(HSprite.FadeKind.In, 0.5f);
						sprite.peeping.imageReplayCheck.gameObject.SetActive(false);
						female.animBody.speed = 1f;
						item.SetAnimationSpeed(1f);
						voice.StartCoroutine(InitOldMemberCoroutine());
					}
				}
			}
		}
		else if (flags.nowAnimationInfo.numCtrl == 1)
		{
			if (!sprite.isFade)
			{
				HSprite.FadeKindProc fadeKindProc2 = sprite.GetFadeKindProc();
				if (fadeKindProc2 != HSprite.FadeKindProc.OutEnd)
				{
					if (num > 0.93f)
					{
						sprite.FadeState(HSprite.FadeKind.Out, 1.5f);
					}
					if (oldFrame <= 0.05f && 0.05f < num)
					{
						GlobalMethod.SetAllClothState(female, false, 2, true);
					}
					else if (oldFrame <= 0.21f && 0.21f < num)
					{
						particle.Play(1);
					}
					else if (oldFrame <= 0.82f && 0.82f < num)
					{
						GlobalMethod.SetAllClothState(female, false, 0, true);
					}
				}
				else if (fadeKindProc2 == HSprite.FadeKindProc.OutEnd)
				{
					GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, false);
					if (!sprite.peeping.objPeepingFemaleList.activeSelf)
					{
						sprite.peeping.objPeepingFemaleList.SetActive(true);
					}
					if (flags.click == HFlag.ClickKind.peeping_restart)
					{
						GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, true);
						sprite.FadeState(HSprite.FadeKind.In, 0.5f);
						sprite.peeping.objPeepingFemaleList.SetActive(false);
						OnChange(sprite.GetPeepingFemale());
						SetRePlay(currentAnimatorStateInfo, 0f, false);
						voice.StartCoroutine(InitOldMemberCoroutine());
					}
				}
			}
		}
		else
		{
			SetMoveCameraFlag();
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && timeVoice.IsIdleTime())
			{
				flags.voice.playVoices[0] = 500;
			}
		}
		oldFrame = num;
		SetDefaultAnimatorFloat();
		return true;
	}

	private void SetRePlay(AnimatorStateInfo _ai, float _normalizetime, bool _isFade = true)
	{
		female.syncPlay(_ai.shortNameHash, 0, _normalizetime);
		item.SyncPlay(_ai.shortNameHash, _normalizetime);
		if (_isFade)
		{
			fade.FadeStart(1f);
		}
	}

	private IEnumerator InitOldMemberCoroutine()
	{
		yield return new WaitForEndOfFrame();
		se.InitOldMember(1);
		oldFrame = 0f;
	}
}
