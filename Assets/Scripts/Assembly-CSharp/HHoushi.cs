using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

public class HHoushi : HActionBase
{
	private int rePlay;

	private bool nextWait;

	private AutoRely rely;

	private bool autoStart;

	public HHoushi(HFlag.DeliveryMember _membar)
		: base(_membar)
	{
	}

	public override bool MotionChange(int _motion)
	{
		switch (_motion)
		{
		case 0:
			SetPlay("Idle");
			flags.isCondom = false;
			if ((bool)male && male.chaFile != null)
			{
				male.chaFile.status.visibleGomu = flags.isCondom;
			}
			sprite.SetHoushiStart();
			sprite.houshi.clickPadCtrl.OnChangeValue(false);
			sprite.houshi.imageCtrlPad.OnChangeValue(0);
			nextWait = false;
			flags.speedCalc = 0f;
			break;
		case 1:
			SetPlay("WLoop");
			sprite.SetHoushiActionButtonActive(true, 2);
			sprite.SetHoushiActionButtonActive(true, 3);
			sprite.houshi.clickPadCtrl.OnChangeValue(true);
			sprite.houshi.imageCtrlPad.OnChangeValue(1);
			flags.speedHoushi = 0;
			flags.SetHoushiPlay();
			break;
		case 2:
			SetPlay("OLoop");
			sprite.SetHoushiActionButtonActive(false);
			sprite.SetHoushiAutoFinish(true);
			sprite.houshi.clickPadCtrl.OnChangeValue(true);
			sprite.houshi.imageCtrlPad.OnChangeValue(-1);
			flags.speedCalc = 0f;
			flags.timeNoClick = 0f;
			flags.speedUpClac = Vector2.zero;
			flags.timeSpeedUpStartCalc = 0f;
			break;
		}
		flags.voice.loopMotionAorB = false;
		flags.voice.SetHoushiIdleTime();
		rePlay = 0;
		rely.InitTimer();
		autoStart = false;
		flags.isInsideFinish = false;
		sprite.imageSpeedSlliderCover70.enabled = false;
		if (flags.selectAnimationListInfo == null)
		{
			flags.voice.isFemale70PercentageVoicePlay = false;
			flags.voice.isMale70PercentageVoicePlay = false;
			flags.voice.isAfterVoicePlay = false;
		}
		else if (flags.selectAnimationListInfo.mode != flags.nowAnimationInfo.mode)
		{
			flags.voice.isFemale70PercentageVoicePlay = false;
			flags.voice.isMale70PercentageVoicePlay = false;
			flags.voice.isAfterVoicePlay = false;
		}
		return true;
	}

	public void SetRely(AutoRely _rely)
	{
		rely = _rely;
	}

	public override bool Proc()
	{
		if (flags.selectAnimationListInfo == null)
		{
			hand.Proc();
		}
		flags.WaitSpeedProcItem();
		if (!hand.IsAction())
		{
			SetMoveCameraFlag();
		}
		male.chaFile.status.visibleBodyAlways = !IsBodyTouch();
		AnimatorStateInfo animatorStateInfo = female.getAnimatorStateInfo(0);
		if (animatorStateInfo.IsName("Idle"))
		{
			flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			if (flags.gaugeMale < 70f && flags.voice.isMale70PercentageVoicePlay)
			{
				flags.voice.isMale70PercentageVoicePlay = false;
			}
			if (flags.voiceWait || (!flags.voiceWait && (flags.click == HFlag.ClickKind.speedup || autoStart)))
			{
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
				{
					flags.voiceWait = false;
					if (flags.selectAnimationListInfo != null)
					{
						flags.click = ((flags.selectAnimationListInfo.mode == HFlag.EMode.houshi) ? ((!(flags.gaugeMale >= 70f)) ? HFlag.ClickKind.insert : HFlag.ClickKind.insert_voice) : HFlag.ClickKind.none);
					}
					else
					{
						SetPlay("WLoop");
						sprite.SetHoushiActionButtonActive(true, 2);
						sprite.SetHoushiActionButtonActive(true, 3);
						sprite.houshi.clickPadCtrl.OnChangeValue(true);
						sprite.houshi.imageCtrlPad.OnChangeValue(1);
						flags.speedHoushi = 0;
						flags.SetHoushiPlay();
						flags.voice.SetHoushiIdleTime();
						if (Manager.Config.EtcData.HInitCamera)
						{
							GlobalMethod.LoadCamera(flags.ctrlCamera, "h/list/", flags.nowAnimationInfo.paramFemale.nameCamera);
						}
						else
						{
							GlobalMethod.LoadResetCamera(flags.ctrlCamera, "h/list/", flags.nowAnimationInfo.paramFemale.nameCamera);
						}
					}
				}
			}
			else if (!hand.IsAction())
			{
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsHoushiIdleTime())
				{
					flags.voice.playVoices[0] = 200;
				}
				if (flags.rely && rely.IsReStart())
				{
					autoStart = true;
				}
			}
		}
		else if (animatorStateInfo.IsName("Stop_Idle"))
		{
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
			{
				flags.voiceWait = false;
				if (nextWait)
				{
					flags.click = HFlag.ClickKind.none;
				}
				else if (flags.gaugeMale >= 70f)
				{
					flags.click = HFlag.ClickKind.insert_voice;
				}
				else
				{
					flags.click = HFlag.ClickKind.insert;
				}
			}
		}
		else if (animatorStateInfo.IsName("WLoop"))
		{
			LoopProc(false);
		}
		else if (animatorStateInfo.IsName("SLoop"))
		{
			LoopProc(true);
		}
		else if (animatorStateInfo.IsName("OLoop"))
		{
			switch (rePlay)
			{
			case 0:
			{
				flags.MaleGaugeUp(Time.deltaTime * flags.rateClickGauge);
				int num = -1;
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange)
				{
					SetPlay("Stop_Idle");
					nextWait = flags.click == HFlag.ClickKind.actionChange;
					sprite.houshi.imageCtrlPad.OnChangeValue(-1);
				}
				else if (flags.click == HFlag.ClickKind.inside || num == 0)
				{
					if (num == 0)
					{
						SetPlay("IN_Start");
						sprite.SetHoushiActionButtonActive(false);
						sprite.houshi.imageCtrlPad.OnChangeValue(-1);
						flags.AddHoushiInside();
						flags.timeDecreaseGaugeCalc = 0f;
						if (flags.nowAnimationInfo.sysTaii == 0)
						{
							flags.AddHandFinsh();
							if (flags.nowAnimationInfo.kindHoushi == 2)
							{
								flags.AddPaizuriFinish();
							}
						}
						else if (flags.nowAnimationInfo.sysTaii == 1)
						{
							flags.AddNameFinish();
							if (flags.nowAnimationInfo.kindHoushi == 2)
							{
								flags.AddPaizurinameiFinish();
							}
						}
						else
						{
							flags.AddKuwaeFinish();
							if (flags.nowAnimationInfo.kindHoushi == 2)
							{
								flags.AddPaizurikuwaeFinish();
							}
						}
					}
					else
					{
						rePlay = 1;
						flags.voice.playVoices[0] = 205;
						flags.voiceWait = true;
					}
				}
				else
				{
					if (flags.click != HFlag.ClickKind.outside && num != 1)
					{
						break;
					}
					if (num == 1)
					{
						SetPlay("M_OUT_Start");
						sprite.SetHoushiActionButtonActive(false);
						sprite.houshi.imageCtrlPad.OnChangeValue(-1);
						flags.AddHoushiOutside();
						if (flags.nowAnimationInfo.isSplash)
						{
							flags.AddSplash();
						}
						if (flags.nowAnimationInfo.sysTaii == 0)
						{
							flags.AddHandFinsh();
							if (flags.nowAnimationInfo.kindHoushi == 2)
							{
								flags.AddPaizuriFinish();
							}
						}
						else if (flags.nowAnimationInfo.sysTaii == 1)
						{
							flags.AddNameFinish();
							if (flags.nowAnimationInfo.kindHoushi == 2)
							{
								flags.AddPaizurinameiFinish();
							}
						}
						else
						{
							flags.AddKuwaeFinish();
							if (flags.nowAnimationInfo.kindHoushi == 2)
							{
								flags.AddPaizurikuwaeFinish();
							}
						}
						flags.voice.playVoices[0] = 206;
					}
					else
					{
						rePlay = 2;
						flags.voice.playVoices[0] = 204;
						flags.voiceWait = true;
					}
				}
				break;
			}
			case 1:
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange)
				{
					SetPlay("Stop_Idle");
					nextWait = flags.click == HFlag.ClickKind.actionChange;
					sprite.houshi.imageCtrlPad.OnChangeValue(-1);
				}
				else
				{
					if (voice.nowVoices[0].state != 0 && (voice.nowVoices[0].state != HVoiceCtrl.VoiceKind.voice || Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
					{
						break;
					}
					flags.voiceWait = false;
					SetPlay("IN_Start");
					sprite.SetHoushiActionButtonActive(false);
					sprite.houshi.imageCtrlPad.OnChangeValue(-1);
					flags.timeDecreaseGaugeCalc = 0f;
					flags.AddHoushiInside();
					if (flags.nowAnimationInfo.sysTaii == 0)
					{
						flags.AddHandFinsh();
						if (flags.nowAnimationInfo.kindHoushi == 2)
						{
							flags.AddPaizuriFinish();
						}
					}
					else if (flags.nowAnimationInfo.sysTaii == 1)
					{
						flags.AddNameFinish();
						if (flags.nowAnimationInfo.kindHoushi == 2)
						{
							flags.AddPaizurinameiFinish();
						}
					}
					else
					{
						flags.AddKuwaeFinish();
						if (flags.nowAnimationInfo.kindHoushi == 2)
						{
							flags.AddPaizurikuwaeFinish();
						}
					}
				}
				break;
			case 2:
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange)
				{
					SetPlay("Stop_Idle");
					nextWait = flags.click == HFlag.ClickKind.actionChange;
					sprite.houshi.imageCtrlPad.OnChangeValue(-1);
				}
				else
				{
					if (voice.nowVoices[0].state != 0 && (voice.nowVoices[0].state != HVoiceCtrl.VoiceKind.voice || Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
					{
						break;
					}
					flags.voiceWait = false;
					SetPlay("M_OUT_Start");
					sprite.SetHoushiActionButtonActive(false);
					sprite.houshi.imageCtrlPad.OnChangeValue(-1);
					flags.AddHoushiOutside();
					if (flags.nowAnimationInfo.isSplash)
					{
						flags.AddSplash();
					}
					if (flags.nowAnimationInfo.sysTaii == 0)
					{
						flags.AddHandFinsh();
						if (flags.nowAnimationInfo.kindHoushi == 2)
						{
							flags.AddPaizuriFinish();
						}
					}
					else if (flags.nowAnimationInfo.sysTaii == 1)
					{
						flags.AddNameFinish();
						if (flags.nowAnimationInfo.kindHoushi == 2)
						{
							flags.AddPaizurinameiFinish();
						}
					}
					else
					{
						flags.AddKuwaeFinish();
						if (flags.nowAnimationInfo.kindHoushi == 2)
						{
							flags.AddPaizurikuwaeFinish();
						}
					}
					flags.voice.playVoices[0] = 206;
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("IN_Start"))
		{
			FinishGaugeDown();
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("IN_Loop", false);
			}
		}
		else if (animatorStateInfo.IsName("IN_Loop"))
		{
			FinishGaugeDown();
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				SetPlay("Oral_Idle_IN");
			}
		}
		else if (animatorStateInfo.IsName("Oral_Idle_IN"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("Oral_Idle", false);
				sprite.SetHoushiActionButtonActive(true, 4);
				sprite.SetHoushiActionButtonActive(true, 5);
				flags.timeWaitHoushi.SetIdleTime();
				flags.gaugeMale = 0f;
			}
		}
		else if (animatorStateInfo.IsName("Oral_Idle"))
		{
			int num2 = -1;
			if (flags.timeWaitHoushi.IsIdleTime())
			{
				num2 = Random.Range(0, 99) % 2;
			}
			if (flags.click == HFlag.ClickKind.drink || num2 == 0)
			{
				flags.AddHoushiDrink();
				SetPlay("Drink_IN");
				sprite.SetHoushiActionButtonActive(false);
			}
			else if (flags.click == HFlag.ClickKind.vomit || num2 == 1)
			{
				flags.AddHoushiVomit();
				SetPlay("Vomit_IN");
				sprite.SetHoushiActionButtonActive(false);
			}
		}
		else if (animatorStateInfo.IsName("M_OUT_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("M_OUT_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
			}
		}
		else if (animatorStateInfo.IsName("M_OUT_Loop"))
		{
			FinishGaugeDown();
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && animatorStateInfo.normalizedTime > 2f)
			{
				SetPlay("OUT_A");
				sprite.SetHoushiActionButtonActive(true, 6);
				sprite.CreateActionList();
				flags.voice.gentles[0] = false;
				rePlay = 0;
				flags.voice.SetHoushiIdleTime();
				autoStart = false;
				flags.gaugeMale = 0f;
				if (flags.lstHeroine[0].hCount == 0 && flags.count.houshiDrink + flags.count.houshiOutside + flags.count.houshiVomit == 1 && !flags.isFreeH)
				{
					sprite.SetHelpSprite(2);
				}
				flags.voice.playVoices[0] = 207;
				flags.voice.isAfterVoicePlay = true;
			}
		}
		else if (animatorStateInfo.IsName("OUT_A"))
		{
			AfterProc(animatorStateInfo, 0);
		}
		else if (animatorStateInfo.IsName("Drink_IN"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("Drink", false);
			}
		}
		else if (animatorStateInfo.IsName("Drink"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("Drink_A");
				sprite.SetHoushiActionButtonActive(true, 6);
				sprite.CreateActionList();
				flags.voice.gentles[0] = false;
				rePlay = 0;
				flags.voice.SetHoushiIdleTime();
				autoStart = false;
				if (flags.lstHeroine[0].hCount == 0 && flags.count.houshiDrink + flags.count.houshiOutside + flags.count.houshiVomit == 1 && !flags.isFreeH)
				{
					sprite.SetHelpSprite(2);
				}
				flags.voice.playVoices[0] = 208;
				flags.voice.isAfterVoicePlay = true;
			}
		}
		else if (animatorStateInfo.IsName("Drink_A"))
		{
			AfterProc(animatorStateInfo, 1);
		}
		else if (animatorStateInfo.IsName("Vomit_IN"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("Vomit", false);
			}
		}
		else if (animatorStateInfo.IsName("Vomit"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("Vomit_A");
				sprite.SetHoushiActionButtonActive(true, 6);
				sprite.CreateActionList();
				flags.voice.gentles[0] = false;
				rePlay = 0;
				flags.voice.SetHoushiIdleTime();
				autoStart = false;
				if (flags.lstHeroine[0].hCount == 0 && flags.count.houshiDrink + flags.count.houshiOutside + flags.count.houshiVomit == 1 && !flags.isFreeH)
				{
					sprite.SetHelpSprite(2);
				}
				flags.voice.playVoices[0] = 209;
				flags.voice.isAfterVoicePlay = true;
			}
		}
		else if (animatorStateInfo.IsName("Vomit_A"))
		{
			AfterProc(animatorStateInfo, 2);
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
		SetAnimatorFloat("speed", flags.speed);
		SetAnimatorFloat("speedHand", 1f + flags.speedItem);
		SetAnimatorFloat("Breast", female.GetShapeBodyValue(4));
		SetDefaultAnimatorFloat();
		return true;
	}

	public override bool LateProc()
	{
		hand.LateProc();
		return true;
	}

	private bool LoopProc(bool _loop)
	{
		flags.WaitSpeedProc(true, flags.speedHoushiCurve);
		flags.MaleGaugeUp(Time.deltaTime * flags.rateClickGauge);
		if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange)
		{
			SetPlay("Stop_Idle");
			nextWait = flags.click == HFlag.ClickKind.actionChange;
			sprite.houshi.imageCtrlPad.OnChangeValue(-1);
			return true;
		}
		if (flags.click == HFlag.ClickKind.motionchange)
		{
			flags.voice.loopMotionAorB = !_loop;
			SetPlay((!_loop) ? "SLoop" : "WLoop");
		}
		else
		{
			if (flags.click == HFlag.ClickKind.OLoop || flags.gaugeMale >= 70f)
			{
				SetPlay("OLoop");
				sprite.SetHoushiActionButtonActive(false);
				sprite.SetHoushiAutoFinish(true);
				sprite.houshi.imageCtrlPad.OnChangeValue(-1);
				flags.speedCalc = 0f;
				flags.timeNoClick = 0f;
				flags.speedUpClac = Vector2.zero;
				flags.timeSpeedUpStartCalc = 0f;
				rePlay = 0;
				flags.voice.loopMotionAorB = false;
				flags.voice.SetHoushiIdleTime();
				flags.voice.playVoices[0] = 203;
				return true;
			}
			if (flags.speedHoushi != 0)
			{
				flags.SpeedUpClick((flags.speedHoushi != 1) ? (0f - flags.rateStateSpeedUp) : flags.rateStateSpeedUp, 1f);
			}
		}
		if (sprite.IsCursorOnPad())
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis < 0f)
			{
				flags.SpeedUpClick(0f - flags.rateWheelSpeedUp, 1f);
			}
			else if (axis > 0f)
			{
				flags.SpeedUpClick(flags.rateWheelSpeedUp, 1f);
			}
		}
		if (flags.rely)
		{
			flags.SpeedUpClick(Random.Range(0f - flags.rateWheelSpeedUp, flags.rateWheelSpeedUp), 1f);
			if (rely.IsChangeWS())
			{
				flags.voice.loopMotionAorB = !_loop;
				SetPlay((!_loop) ? "SLoop" : "WLoop");
			}
		}
		if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsHoushiIdleTime())
		{
			flags.voice.playVoices[0] = (flags.voice.speedMotion ? 202 : 201);
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
		return true;
	}

	private void FinishGaugeDown()
	{
		flags.timeDecreaseGaugeCalc += Time.deltaTime;
		flags.timeDecreaseGaugeCalc = Mathf.Clamp(flags.timeDecreaseGaugeCalc + Time.deltaTime, 0f, flags.timeDecreaseGauge);
		float t = flags.timeDecreaseGaugeCalc / flags.timeDecreaseGauge;
		flags.gaugeMale = Mathf.Lerp(100f, 0f, t);
	}

	private void AfterProc(AnimatorStateInfo _asi, int _after)
	{
		switch (rePlay)
		{
		case 0:
			flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			if (flags.click == HFlag.ClickKind.again || flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange || autoStart)
			{
				rePlay = 1;
				flags.voiceWait = true;
				if (autoStart && flags.click != 0)
				{
					autoStart = false;
				}
				nextWait = flags.click == HFlag.ClickKind.actionChange && !autoStart;
				if (flags.click == HFlag.ClickKind.again || autoStart)
				{
					List<int> lstKosoKosoCategory = new List<int> { 1202 };
					flags.voice.playVoices[0] = ((!flags.nowAnimationInfo.lstCategory.Any((HSceneProc.Category c) => lstKosoKosoCategory.Contains(c.category))) ? 1 : 11);
				}
			}
			else if (!hand.IsAction() && flags.rely && rely.IsReStart())
			{
				autoStart = true;
			}
			break;
		case 1:
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
			{
				flags.voiceWait = false;
				flags.voice.isAfterVoicePlay = false;
				SetPlay((!nextWait) ? "WLoop" : "Idle");
				sprite.SetHoushiActionButtonActive(false, 6);
				sprite.SetHoushiActionButtonActive(true, 2);
				sprite.SetHoushiActionButtonActive(true, 3);
				sprite.houshi.imageCtrlPad.OnChangeValue(1);
				flags.speedHoushi = 0;
				meta.Clear();
				rePlay = 0;
				if (flags.selectAnimationListInfo != null && flags.selectAnimationListInfo.mode == HFlag.EMode.houshi && !nextWait)
				{
					flags.click = HFlag.ClickKind.insert;
				}
			}
			break;
		}
	}
}
