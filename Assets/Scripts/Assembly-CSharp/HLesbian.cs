using Manager;
using UnityEngine;

public class HLesbian : HActionBase
{
	private int voiceGaugeTiming;

	private bool speek;

	private HFlag.TimeWait timeSpeed = new HFlag.TimeWait
	{
		timeIdleMin = 2f,
		timeIdleMax = 5f
	};

	public HLesbian(HFlag.DeliveryMember _membar)
		: base(_membar)
	{
	}

	public override bool MotionChange(int _motion)
	{
		if (_motion == 1)
		{
			SetPlay("WLoop", false);
		}
		flags.timeLesbian.SetIdleTime();
		sprite.imageSpeedSlliderCover70.enabled = false;
		voiceGaugeTiming = Random.Range(0, 3);
		speek = false;
		timeSpeed.MemberInit();
		return true;
	}

	public override bool Proc()
	{
		SetMoveCameraFlag();
		AnimatorStateInfo animatorStateInfo = female.getAnimatorStateInfo(0);
		int numCtrl = flags.nowAnimationInfo.numCtrl;
		if (animatorStateInfo.IsName("WLoop"))
		{
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			if (timeSpeed.IsIdleTime())
			{
				flags.speed = Random.value * flags.speedLesbian;
			}
			if (!speek && (voiceGaugeTiming == 0 || (voiceGaugeTiming == 1 && flags.gaugeFemale >= 10f) || (voiceGaugeTiming == 2 && flags.gaugeFemale >= 20f)))
			{
				flags.voice.playVoices[1] = 600;
				speek = true;
			}
			if (flags.gaugeFemale >= 30f)
			{
				SetPlay("MLoop");
				speek = false;
			}
		}
		else if (animatorStateInfo.IsName("MLoop"))
		{
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			if (timeSpeed.IsIdleTime())
			{
				flags.speed = Random.value * flags.speedLesbian;
			}
			if (!speek && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && (voiceGaugeTiming == 0 || (voiceGaugeTiming == 1 && flags.gaugeFemale >= 40f) || (voiceGaugeTiming == 2 && flags.gaugeFemale >= 50f)))
			{
				flags.voice.playVoices[0] = 601;
				speek = true;
			}
			if (flags.gaugeFemale >= 70f)
			{
				SetPlay("SLoop");
				speek = false;
			}
			SetFluctuation(0, flags.motion);
			SetFluctuation(1, flags.motion1);
		}
		else if (animatorStateInfo.IsName("SLoop"))
		{
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			flags.speed = Mathf.InverseLerp(70f, 100f, flags.gaugeFemale) * flags.speedLesbian;
			if (!speek && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
			{
				flags.voice.playVoices[(numCtrl != 0) ? 1 : Random.Range(0, 2)] = 602;
				speek = true;
			}
			if (flags.gaugeFemale >= 100f)
			{
				SetPlay("OLoop");
				flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 603);
				speek = false;
				flags.speed = 0f;
			}
		}
		else if (animatorStateInfo.IsName("OLoop"))
		{
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[(numCtrl == 0) ? 1u : 0u]))
			{
				SetPlay("Orgasm");
				flags.timeDecreaseGaugeCalc = 0f;
				flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 604);
			}
		}
		else if (animatorStateInfo.IsName("Orgasm"))
		{
			FinishGaugeDown();
			if (animatorStateInfo.normalizedTime >= 1f)
			{
				SetPlay("Orgasm_A");
				flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 605);
			}
		}
		else if (animatorStateInfo.IsName("Orgasm_A"))
		{
			FinishGaugeDown();
			if (animatorStateInfo.normalizedTime >= 6f && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath)
			{
				SetPlay("Orgasm_B");
				flags.timeLesbian.SetIdleTime();
				flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 606);
			}
		}
		else if (animatorStateInfo.IsName("Orgasm_B") && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.timeLesbian.IsIdleTime())
		{
			SetPlay("WLoop");
			voiceGaugeTiming = Random.Range(0, 3);
			speek = false;
		}
		flags.motion = CalcFluctuation(0, flags.motion);
		flags.motion1 = CalcFluctuation(1, flags.motion1);
		SetDefaultAnimatorFloat();
		SetAnimatorFloat(female, "speedBody", flags.speed + 1f);
		SetAnimatorFloat(female, "Breast", female.GetShapeBodyValue(4));
		SetAnimatorFloat(female, "height1", (!female1) ? 0f : female1.GetShapeBodyValue(0));
		SetAnimatorFloat(female, "Breast1", (!female1) ? 0f : female1.GetShapeBodyValue(4));
		SetAnimatorFloat(female, "motion1", flags.motion1);
		if ((bool)female1)
		{
			SetAnimatorFloat(female1, "speedBody", flags.speed + 1f);
			SetAnimatorFloat(female1, "Breast", female1.GetShapeBodyValue(4));
			SetAnimatorFloat(female1, "height1", female.GetShapeBodyValue(0));
			SetAnimatorFloat(female1, "Breast1", female.GetShapeBodyValue(4));
			SetAnimatorFloat(female1, "motion1", flags.motion1);
		}
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

	private void SetFluctuation(int _num, float _motion)
	{
		timeAutoMotionCalcs[_num] += Time.deltaTime;
		if (timeAutoMotions[_num] > timeAutoMotionCalcs[_num] || enableMotions[_num])
		{
			return;
		}
		timeAutoMotions[_num] = Random.Range(flags.timeAutoMotionMin, flags.timeAutoMotionMax);
		timeAutoMotionCalcs[_num] = 0f;
		enableMotions[_num] = true;
		timeMotionCalcs[_num] = 0f;
		float num = 0f;
		if (allowMotions[_num])
		{
			num = 1f - _motion;
			num = ((!(num <= flags.rateMotionMin)) ? (_motion + Random.Range(flags.rateMotionMin, num)) : 1f);
			if (num >= 1f)
			{
				allowMotions[_num] = false;
			}
		}
		else
		{
			num = _motion;
			num = ((!(num <= flags.rateMotionMin)) ? (_motion - Random.Range(flags.rateMotionMin, num)) : 0f);
			if (num <= 0f)
			{
				allowMotions[_num] = true;
			}
		}
		lerpMotions[_num] = new Vector2(_motion, num);
		timeMotions[_num] = Random.Range(flags.timeMotionMin, flags.timeMotionMax);
	}

	private float CalcFluctuation(int _num, float _motion)
	{
		if (!enableMotions[_num])
		{
			return _motion;
		}
		timeMotionCalcs[_num] = Mathf.Clamp(timeMotionCalcs[_num] + Time.deltaTime, 0f, timeMotions[_num]);
		float time = Mathf.Clamp01(timeMotionCalcs[_num] / timeMotions[_num]);
		time = flags.curveMotion.Evaluate(time);
		float result = Mathf.Lerp(lerpMotions[_num].x, lerpMotions[_num].y, time);
		if (time >= 1f)
		{
			enableMotions[_num] = false;
		}
		return result;
	}
}
