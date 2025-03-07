using Manager;
using UnityEngine;

public class HTestMasturbation : HTestActionBase
{
	public HTestMasturbation(HFlag.DeliveryMember _membar)
		: base(_membar)
	{
	}

	public override bool MotionChange(int _motion)
	{
		if (_motion == 1)
		{
			SetPlay("WLoop", false);
		}
		flags.timeMasturbation.SetIdleTime();
		sprite.imageSpeedSlliderCover70.enabled = false;
		return true;
	}

	public override bool Proc()
	{
		SetMoveCameraFlag();
		AnimatorStateInfo currentAnimatorStateInfo = female.animBody.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName("WLoop"))
		{
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			if (flags.gaugeFemale >= 40f)
			{
				SetPlay("MLoop");
				flags.voice.playVoices[0] = 402;
			}
		}
		else if (currentAnimatorStateInfo.IsName("MLoop"))
		{
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			if (flags.gaugeFemale >= 70f)
			{
				SetPlay("SLoop");
				flags.voice.playVoices[0] = 403;
			}
			timeAutoMotionCalcs[0] += Time.deltaTime;
			if (timeAutoMotions[0] <= timeAutoMotionCalcs[0] && !enableMotions[0])
			{
				timeAutoMotions[0] = Random.Range(flags.timeAutoMotionMin, flags.timeAutoMotionMax);
				timeAutoMotionCalcs[0] = 0f;
				enableMotions[0] = true;
				timeMotionCalcs[0] = 0f;
				float num = 0f;
				if (allowMotions[0])
				{
					num = 1f - flags.motion;
					num = ((!(num <= flags.rateMotionMin)) ? (flags.motion + Random.Range(flags.rateMotionMin, num)) : 1f);
					if (num >= 1f)
					{
						allowMotions[0] = false;
					}
				}
				else
				{
					num = flags.motion;
					num = ((!(num <= flags.rateMotionMin)) ? (flags.motion - Random.Range(flags.rateMotionMin, num)) : 0f);
					if (num <= 0f)
					{
						allowMotions[0] = true;
					}
				}
				lerpMotions[0] = new Vector2(flags.motion, num);
				timeMotions[0] = Random.Range(flags.timeMotionMin, flags.timeMotionMax);
			}
		}
		else if (currentAnimatorStateInfo.IsName("SLoop"))
		{
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			if (flags.gaugeFemale >= 100f)
			{
				SetPlay("OLoop");
				flags.voice.playVoices[0] = 404;
			}
		}
		else if (currentAnimatorStateInfo.IsName("OLoop"))
		{
			bool flag = false;
			if (flags.isDebug && currentAnimatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag = true;
			}
			else if (!flags.isDebug && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				flag = true;
			}
			if (flag)
			{
				SetPlay("Orgasm");
				flags.timeDecreaseGaugeCalc = 0f;
				flags.voice.playVoices[0] = 406;
			}
		}
		else if (currentAnimatorStateInfo.IsName("Orgasm"))
		{
			FinishGaugeDown();
			if (currentAnimatorStateInfo.normalizedTime >= 1f)
			{
				SetPlay("Orgasm_A");
			}
		}
		else if (currentAnimatorStateInfo.IsName("Orgasm_A"))
		{
			FinishGaugeDown();
			if (currentAnimatorStateInfo.normalizedTime >= 6f)
			{
				SetPlay("Orgasm_B");
				flags.timeMasturbation.SetIdleTime();
				flags.voice.playVoices[0] = 405;
			}
		}
		else if (currentAnimatorStateInfo.IsName("Orgasm_B") && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.timeMasturbation.IsIdleTime())
		{
			SetPlay("WLoop");
			flags.voice.playVoices[0] = 401;
		}
		if (enableMotions[0])
		{
			timeMotionCalcs[0] = Mathf.Clamp(timeMotionCalcs[0] + Time.deltaTime, 0f, timeMotions[0]);
			float time = Mathf.Clamp01(timeMotionCalcs[0] / timeMotions[0]);
			time = flags.curveMotion.Evaluate(time);
			flags.motion = Mathf.Lerp(lerpMotions[0].x, lerpMotions[0].y, time);
			if (time >= 1f)
			{
				enableMotions[0] = false;
			}
		}
		SetDefaultAnimatorFloat();
		return true;
	}

	public override bool LateProc()
	{
		if (flags.click == HFlag.ClickKind.none && flags.selectAnimationListInfo != null)
		{
			return false;
		}
		return true;
	}

	private void FinishGaugeDown()
	{
		flags.timeDecreaseGaugeCalc += Time.deltaTime;
		flags.timeDecreaseGaugeCalc = Mathf.Clamp(flags.timeDecreaseGaugeCalc + Time.deltaTime, 0f, flags.timeDecreaseGauge);
		float t = flags.timeDecreaseGaugeCalc / flags.timeDecreaseGauge;
		flags.gaugeFemale = Mathf.Lerp(100f, 0f, t);
	}
}
