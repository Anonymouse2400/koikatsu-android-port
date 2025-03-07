using Manager;
using UnityEngine;

public class H3PHoushi : HActionBase
{
	private enum VoicePlayShuffleKind
	{
		Idle = 0,
		Loop = 1,
		OLoop = 2,
		Finish = 3,
		OutSide = 4,
		After = 5,
		Again = 6
	}

	private int rePlay;

	private bool nextWait;

	private AutoRely rely;

	private bool autoStart;

	private ShuffleRand[] voicePlayShuffle = new ShuffleRand[10];

	public H3PHoushi(HFlag.DeliveryMember _membar)
		: base(_membar)
	{
		for (int i = 0; i < voicePlayShuffle.Length; i++)
		{
			voicePlayShuffle[i] = new ShuffleRand();
			voicePlayShuffle[i].Init(2);
		}
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
			sprite.SetHoushi3PStart();
			sprite.houshi3P.clickPadCtrl.OnChangeValue(false);
			sprite.houshi3P.imageCtrlPad.OnChangeValue(0);
			nextWait = false;
			flags.speedCalc = 0f;
			break;
		case 1:
			SetPlay("WLoop");
			sprite.SetHoushi3PActionButtonActive(true, 2);
			sprite.SetHoushi3PActionButtonActive(true, 3);
			sprite.houshi3P.clickPadCtrl.OnChangeValue(true);
			sprite.houshi3P.imageCtrlPad.OnChangeValue(1);
			flags.speedHoushi = 0;
			flags.SetHoushiPlay();
			break;
		case 2:
			SetPlay("OLoop");
			sprite.SetHoushi3PActionButtonActive(false);
			sprite.SetHoushi3PAutoFinish(true);
			sprite.houshi3P.clickPadCtrl.OnChangeValue(true);
			sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
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
			hand1.Proc(hand.SelectKindTouch != HandCtrl.AibuColliderKind.none);
		}
		if (!hand.IsAction())
		{
			SetMoveCameraFlag();
		}
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
				if (IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
				{
					flags.voiceWait = false;
					if (flags.selectAnimationListInfo != null)
					{
						flags.click = ((flags.selectAnimationListInfo.mode == HFlag.EMode.houshi) ? ((!(flags.gaugeMale >= 70f)) ? HFlag.ClickKind.insert : HFlag.ClickKind.insert_voice) : HFlag.ClickKind.none);
					}
					else
					{
						SetPlay("WLoop");
						sprite.SetHoushi3PActionButtonActive(true, 2);
						sprite.SetHoushi3PActionButtonActive(true, 3);
						sprite.houshi3P.clickPadCtrl.OnChangeValue(true);
						sprite.houshi3P.imageCtrlPad.OnChangeValue(1);
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
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsHoushiIdleTime())
				{
					flags.voice.playVoices[voicePlayShuffle[0].Get()] = 700;
				}
				if (flags.rely && rely.IsReStart())
				{
					autoStart = true;
				}
			}
		}
		else if (animatorStateInfo.IsName("Stop_Idle"))
		{
			if (IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
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
					sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
				}
				else if (flags.click == HFlag.ClickKind.inside || num == 0)
				{
					if (num == 0)
					{
						SetPlay("IN_Start");
						sprite.SetHoushi3PActionButtonActive(false);
						sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
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
						flags.voice.playVoices[0] = 713;
						flags.voice.playVoices[1] = 713;
					}
					else
					{
						rePlay = 1;
						int num2 = voicePlayShuffle[3].Get();
						flags.voice.playVoices[num2] = 707;
						flags.voiceWait = true;
						if (IsVoicePlay(num2 ^ 1))
						{
							Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num2 ^ 1]);
						}
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
						sprite.SetHoushi3PActionButtonActive(false);
						sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
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
						flags.voice.playVoices[voicePlayShuffle[4].Get()] = 708;
					}
					else
					{
						rePlay = 2;
						int num3 = voicePlayShuffle[3].Get();
						flags.voice.playVoices[num3] = 706;
						flags.voiceWait = true;
						if (IsVoicePlay(num3 ^ 1))
						{
							Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num3 ^ 1]);
						}
					}
				}
				break;
			}
			case 1:
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange)
				{
					SetPlay("Stop_Idle");
					nextWait = flags.click == HFlag.ClickKind.actionChange;
					sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
				}
				else
				{
					if (!IsCheckVoicePlay(0) || !IsCheckVoicePlay(1))
					{
						break;
					}
					flags.voiceWait = false;
					SetPlay("IN_Start");
					sprite.SetHoushi3PActionButtonActive(false);
					sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
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
					flags.voice.playVoices[0] = 713;
					flags.voice.playVoices[1] = 713;
				}
				break;
			case 2:
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.actionChange)
				{
					SetPlay("Stop_Idle");
					nextWait = flags.click == HFlag.ClickKind.actionChange;
					sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
				}
				else
				{
					if (!IsCheckVoicePlay(0) || !IsCheckVoicePlay(1))
					{
						break;
					}
					flags.voiceWait = false;
					SetPlay("M_OUT_Start");
					sprite.SetHoushi3PActionButtonActive(false);
					sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
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
					flags.voice.playVoices[voicePlayShuffle[4].Get()] = 708;
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
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breathShort || voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breathShort || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath))
			{
				for (int i = 0; i < voice.nowVoices.Length; i++)
				{
					if (voice.nowVoices[i].state == HVoiceCtrl.VoiceKind.breathShort && voice.nowVoices[i ^ 1].state == HVoiceCtrl.VoiceKind.voice)
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[i ^ 1]);
					}
				}
				SetPlay("Oral_Idle_IN");
			}
		}
		else if (animatorStateInfo.IsName("Oral_Idle_IN"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay("Oral_Idle", false);
				sprite.SetHoushi3PActionButtonActive(true, 4);
				sprite.SetHoushi3PActionButtonActive(true, 5);
				flags.timeWaitHoushi.SetIdleTime();
				flags.gaugeMale = 0f;
			}
		}
		else if (animatorStateInfo.IsName("Oral_Idle"))
		{
			int num4 = -1;
			if (flags.timeWaitHoushi.IsIdleTime())
			{
				num4 = Random.Range(0, 99) % 2;
			}
			if (flags.click == HFlag.ClickKind.drink || num4 == 0)
			{
				flags.AddHoushiDrink();
				SetPlay("Drink_IN");
				sprite.SetHoushi3PActionButtonActive(false);
			}
			else if (flags.click == HFlag.ClickKind.vomit || num4 == 1)
			{
				flags.AddHoushiVomit();
				SetPlay("Vomit_IN");
				sprite.SetHoushi3PActionButtonActive(false);
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
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && animatorStateInfo.normalizedTime > 2f)
			{
				SetPlay("OUT_A");
				sprite.SetHoushi3PActionButtonActive(true, 6);
				sprite.CreateActionList();
				flags.voice.gentles[0] = false;
				flags.voice.gentles[1] = false;
				rePlay = 0;
				flags.voice.SetHoushiIdleTime();
				autoStart = false;
				flags.gaugeMale = 0f;
				flags.voice.playVoices[voicePlayShuffle[5].Get()] = 709;
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
				sprite.SetHoushi3PActionButtonActive(true, 6);
				sprite.CreateActionList();
				flags.voice.gentles[0] = false;
				flags.voice.gentles[1] = false;
				rePlay = 0;
				flags.voice.SetHoushiIdleTime();
				autoStart = false;
				flags.voice.playVoices[voicePlayShuffle[5].Get()] = 710;
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
				sprite.SetHoushi3PActionButtonActive(true, 6);
				sprite.CreateActionList();
				flags.voice.gentles[0] = false;
				flags.voice.gentles[1] = false;
				rePlay = 0;
				flags.voice.SetHoushiIdleTime();
				autoStart = false;
				flags.voice.playVoices[voicePlayShuffle[5].Get()] = 711;
				flags.voice.isAfterVoicePlay = true;
			}
		}
		else if (animatorStateInfo.IsName("Vomit_A"))
		{
			AfterProc(animatorStateInfo, 2);
		}
		flags.motion = CalcFluctuation(0, flags.motion);
		flags.motion1 = CalcFluctuation(1, flags.motion1);
		SetAnimatorFloat("speed", flags.speed);
		SetAnimatorFloat("speedHand", 1f + flags.speedItem);
		SetAnimatorFloat("Breast", female.GetShapeBodyValue(4));
		SetDefaultAnimatorFloat();
		SetAnimatorFloat(female1, "motion", (flags.nowAnimationInfo.numCtrl != 2 && flags.nowAnimationInfo.numCtrl != 3) ? flags.motion : flags.motion1);
		return true;
	}

	public override bool LateProc()
	{
		hand.LateProc();
		hand1.LateProc();
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
			sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
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
				sprite.SetHoushi3PActionButtonActive(false);
				sprite.SetHoushi3PAutoFinish(true);
				sprite.houshi3P.imageCtrlPad.OnChangeValue(-1);
				flags.speedCalc = 0f;
				flags.timeNoClick = 0f;
				flags.speedUpClac = Vector2.zero;
				flags.timeSpeedUpStartCalc = 0f;
				rePlay = 0;
				flags.voice.loopMotionAorB = false;
				flags.voice.SetHoushiIdleTime();
				flags.voice.playVoices[voicePlayShuffle[2].Get()] = 705;
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
		if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsHoushiIdleTime())
		{
			if (!flags.voice.loopMotionAorB)
			{
				flags.voice.playVoices[voicePlayShuffle[1].Get()] = (flags.voice.speedMotion ? 702 : 701);
			}
			else
			{
				flags.voice.playVoices[voicePlayShuffle[1].Get()] = (flags.voice.speedMotion ? 704 : 703);
			}
		}
		SetFluctuation(0, flags.motion);
		SetFluctuation(1, flags.motion1);
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
					int num = voicePlayShuffle[6].Get();
					flags.voice.playVoices[num] = 13;
					if (IsVoicePlay(num ^ 1))
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num ^ 1]);
					}
				}
			}
			else if (!hand.IsAction() && flags.rely && rely.IsReStart())
			{
				autoStart = true;
			}
			break;
		case 1:
			if (IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
			{
				flags.voiceWait = false;
				flags.voice.isAfterVoicePlay = false;
				SetPlay((!nextWait) ? "WLoop" : "Idle");
				sprite.SetHoushi3PActionButtonActive(false, 6);
				sprite.SetHoushi3PActionButtonActive(true, 2);
				sprite.SetHoushi3PActionButtonActive(true, 3);
				sprite.houshi3P.imageCtrlPad.OnChangeValue(1);
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
