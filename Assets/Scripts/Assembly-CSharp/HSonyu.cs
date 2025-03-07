using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

public class HSonyu : HActionBase
{
	private bool isAuto;

	private float oldGaugeMale;

	private float oldGaugeFemale;

	private int rePlay;

	private int kindInsert;

	private int gotoInAfter;

	private bool isSW;

	private bool[] is70Voices = new bool[2];

	public HSonyu(HFlag.DeliveryMember _membar)
		: base(_membar)
	{
	}

	public override bool MotionChange(int _motion)
	{
		SetPlay("Idle");
		rePlay = 0;
		flags.isAnalPlay = false;
		sprite.HoushiInitRely();
		isAuto = false;
		sprite.sonyu.clickPadCtrl.OnChangeValue(isAuto);
		sprite.imageSpeedSlliderCover70.enabled = false;
		sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
		isSW = false;
		flags.isCondom = sprite.sonyu.isCondom;
		if ((bool)male && male.chaFile != null)
		{
			male.chaFile.status.visibleGomu = flags.isCondom;
		}
		flags.voice.SetSonyuWaitTime(true);
		flags.voice.SetSonyuIdleTime();
		flags.isInsideFinish = false;
		flags.speedCalc = 0f;
		flags.isDenialvoiceWait = false;
		if (flags.selectAnimationListInfo == null)
		{
			flags.voice.isFemale70PercentageVoicePlay = false;
			flags.voice.isMale70PercentageVoicePlay = false;
			flags.voice.isAfterVoicePlay = false;
			is70Voices[0] = false;
			is70Voices[1] = false;
		}
		else if (flags.selectAnimationListInfo.mode != flags.nowAnimationInfo.mode)
		{
			flags.voice.isFemale70PercentageVoicePlay = false;
			flags.voice.isMale70PercentageVoicePlay = false;
			flags.voice.isAfterVoicePlay = false;
			is70Voices[0] = false;
			is70Voices[1] = false;
		}
		return true;
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
		if (flags.gaugeFemale < 70f && flags.voice.isFemale70PercentageVoicePlay)
		{
			flags.voice.isFemale70PercentageVoicePlay = false;
		}
		if (flags.gaugeMale < 70f && flags.voice.isMale70PercentageVoicePlay)
		{
			flags.voice.isMale70PercentageVoicePlay = false;
		}
		if (flags.gaugeFemale >= 70f && !flags.voice.isFemale70PercentageVoicePlay && flags.voice.IsSonyuIdleFlag() && !is70Voices[0])
		{
			flags.voice.SetSonyuWaitTime(false);
			flags.voice.SetSonyuIdleTime();
			is70Voices[0] = true;
		}
		else if (flags.gaugeMale >= 70f && !flags.voice.isMale70PercentageVoicePlay && flags.voice.IsSonyuIdleFlag() && !is70Voices[1])
		{
			flags.voice.SetSonyuWaitTime(false);
			flags.voice.SetSonyuIdleTime();
			is70Voices[1] = true;
		}
		int num = ((Game.isAdd20 && flags.nowAnimationInfo.isFemaleInitiative) ? 38 : 0);
		if (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("A_Idle"))
		{
			if (IsVoiceWait())
			{
				return false;
			}
			if (!hand.IsAction())
			{
				flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			}
			flags.MaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge);
			switch (rePlay)
			{
			case 0:
				if (flags.isDenialvoiceWait && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
				{
					if (!flags.isCondom)
					{
						flags.isCondom = true;
						male.chaFile.status.visibleGomu = true;
						sprite.SetCondom(true);
					}
					flags.isDenialvoiceWait = false;
				}
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.insert_voice)
				{
					rePlay = 1;
					kindInsert = ((flags.click != HFlag.ClickKind.insert) ? 1 : 0);
					flags.voiceWait = true;
				}
				else if (flags.click == HFlag.ClickKind.insert_anal || flags.click == HFlag.ClickKind.insert_anal_voice)
				{
					rePlay = 2;
					kindInsert = ((flags.click != HFlag.ClickKind.insert_anal) ? 1 : 0);
					flags.voiceWait = true;
				}
				else if (!hand.IsAction() && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime())
				{
					flags.voice.playVoices[0] = 300 + num;
				}
				break;
			case 1:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
				{
					flags.voiceWait = false;
					SetPlay("Insert");
					sprite.SetSonyuActionButtonActive(false);
					flags.isAnalPlay = false;
					if (!flags.isCondom)
					{
						flags.SetNameInsert();
					}
					flags.AddSonyuKokanPlay();
					if (kindInsert == 0)
					{
						flags.voice.playVoices[0] = 307 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 308 + num;
					}
				}
				break;
			case 2:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
				{
					flags.voiceWait = false;
					SetPlay("A_Insert");
					sprite.SetSonyuActionButtonActive(false);
					flags.isAnalPlay = true;
					flags.AddSonyuAnalPlay();
					if (kindInsert == 0)
					{
						flags.voice.playVoices[0] = 307 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 308 + num;
					}
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("Insert") || animatorStateInfo.IsName("A_Insert"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_InsertIdle" : "InsertIdle");
				sprite.SetSonyuActionButtonActive(true, 6);
				sprite.sonyu.imageCtrlPad.OnChangeValue(0);
				flags.finish = HFlag.FinishKind.none;
				flags.voice.SetSonyuIdleTime();
				if (!flags.isAnalPlay)
				{
					flags.SetInsertKokan();
				}
				else
				{
					flags.SetInsertAnal();
				}
			}
		}
		else if (animatorStateInfo.IsName("InsertIdle") || animatorStateInfo.IsName("A_InsertIdle"))
		{
			if (IsVoiceWait())
			{
				if (flags.voiceWait)
				{
					if (!flags.isAnalPlay)
					{
						flags.SetInsertKokanVoiceCondition();
					}
					else
					{
						flags.SetInsertAnalVoiceCondition();
					}
				}
				return false;
			}
			if (!hand.IsAction())
			{
				flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			}
			flags.MaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge);
			if (flags.click == HFlag.ClickKind.speedup)
			{
				flags.SpeedUpClick(flags.rateSpeedUp, 1f);
				sprite.SetSonyuActionButtonActive(false, 6);
				SetPlay(flags.isAnalPlay ? "A_WLoop" : "WLoop");
				flags.voice.SetSonyuIdleTime();
				if (!flags.isAnalPlay)
				{
					flags.SetInsertKokanVoiceCondition();
				}
				else
				{
					flags.SetInsertAnalVoiceCondition();
				}
				isSW = false;
			}
			else if (flags.click == HFlag.ClickKind.modeChange)
			{
				isAuto = !isAuto;
				sprite.sonyu.clickPadCtrl.OnChangeValue(isAuto);
				sprite.SetSonyuActionButtonActive(false, 6);
				sprite.sonyu.imageCtrlPad.OnChangeValue(1);
				SetPlay(flags.isAnalPlay ? "A_WLoop" : "WLoop");
				flags.voice.SetSonyuIdleTime();
				if (!flags.isAnalPlay)
				{
					flags.SetInsertKokanVoiceCondition();
				}
				else
				{
					flags.SetInsertAnalVoiceCondition();
				}
				isSW = false;
			}
			else if (flags.click == HFlag.ClickKind.pull)
			{
				SetPlay(flags.isAnalPlay ? "A_Idle" : "Idle");
				sprite.SetSonyuStart();
				sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
				flags.voice.SetSonyuIdleTime();
				if (!flags.isAnalPlay)
				{
					flags.SetInsertKokanVoiceCondition();
				}
				else
				{
					flags.SetInsertAnalVoiceCondition();
				}
				rePlay = 0;
				flags.voice.playVoices[0] = (flags.isAnalPlay ? 306 : 303) + num;
			}
			else if (!hand.IsAction() && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime())
			{
				flags.voice.playVoices[0] = 309 + num;
			}
		}
		else if (animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("A_WLoop"))
		{
			LoopProc(false);
		}
		else if (animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("A_SLoop"))
		{
			LoopProc(true);
		}
		else if (animatorStateInfo.IsName("OLoop") || animatorStateInfo.IsName("A_OLoop"))
		{
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				if (flags.finish == HFlag.FinishKind.inside)
				{
					if (!flags.isAnalPlay)
					{
						if (!flags.isCondom)
						{
							flags.AddSonyuInside();
						}
						else
						{
							flags.AddSonyuCondomInside();
						}
					}
					else if (!flags.isCondom)
					{
						flags.AddSonyuAnalInside();
					}
					else
					{
						flags.AddSonyuAnalCondomInside();
					}
					SetPlay(flags.isAnalPlay ? "A_M_IN_Start" : "M_IN_Start");
					if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && (bool)sprite.objFirstHHelpBase && sprite.objFirstHHelpBase.activeSelf && !flags.isAnalPlay)
					{
						sprite.objFirstHHelpBase.SetActive(false);
					}
					flags.voice.playVoices[0] = 324 + num;
					flags.isInsideFinish = true;
				}
				else if (flags.finish == HFlag.FinishKind.outside)
				{
					if (!flags.isAnalPlay)
					{
						flags.AddSonyuOutside();
					}
					else
					{
						flags.AddSonyuAnalOutside();
					}
					SetPlay(flags.isAnalPlay ? "A_M_OUT_Start" : "M_OUT_Start");
					if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && (bool)sprite.objFirstHHelpBase && sprite.objFirstHHelpBase.activeSelf && !flags.isAnalPlay)
					{
						sprite.objFirstHHelpBase.SetActive(false);
					}
					flags.voice.playVoices[0] = 334 + num;
				}
				else if (flags.finish == HFlag.FinishKind.orgW)
				{
					if (!flags.isAnalPlay)
					{
						flags.AddSonyuOrg();
					}
					else
					{
						flags.AddSonyuAnalOrg();
					}
					SetPlay(flags.isAnalPlay ? "A_WF_IN_Start" : "WF_IN_Start");
					if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && (bool)sprite.objFirstHHelpBase && sprite.objFirstHHelpBase.activeSelf && !flags.isAnalPlay)
					{
						sprite.objFirstHHelpBase.SetActive(false);
					}
					gotoInAfter = 0;
					flags.voice.playVoices[0] = 325 + num;
				}
				else if (flags.finish == HFlag.FinishKind.sameW)
				{
					if (!flags.isAnalPlay)
					{
						if (!flags.isCondom)
						{
							flags.AddSonyuSame();
							flags.AddSonyuInside();
						}
						else
						{
							flags.AddSonyuCondomSame();
							flags.AddSonyuCondomInside();
						}
						flags.AddSonyuOrg();
					}
					else
					{
						if (!flags.isCondom)
						{
							flags.AddSonyuAnalSame();
							flags.AddSonyuAnalInside();
						}
						else
						{
							flags.AddSonyuAnalCondomSame();
							flags.AddSonyuAnalCondomInside();
						}
						flags.AddSonyuAnalOrg();
					}
					SetPlay(flags.isAnalPlay ? "A_WS_IN_Start" : "WS_IN_Start");
					if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && (bool)sprite.objFirstHHelpBase && sprite.objFirstHHelpBase.activeSelf && !flags.isAnalPlay)
					{
						sprite.objFirstHHelpBase.SetActive(false);
					}
					gotoInAfter = 0;
					flags.voice.playVoices[0] = 325 + num;
					flags.isInsideFinish = true;
				}
				else if (flags.finish == HFlag.FinishKind.orgS)
				{
					if (!flags.isAnalPlay)
					{
						flags.AddSonyuOrg();
					}
					else
					{
						flags.AddSonyuAnalOrg();
					}
					SetPlay(flags.isAnalPlay ? "A_SF_IN_Start" : "SF_IN_Start");
					if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && (bool)sprite.objFirstHHelpBase && sprite.objFirstHHelpBase.activeSelf && !flags.isAnalPlay)
					{
						sprite.objFirstHHelpBase.SetActive(false);
					}
					gotoInAfter = 0;
					flags.voice.playVoices[0] = 326 + num;
				}
				else if (flags.finish == HFlag.FinishKind.sameS)
				{
					if (!flags.isAnalPlay)
					{
						if (!flags.isCondom)
						{
							flags.AddSonyuSame();
							flags.AddSonyuInside();
						}
						else
						{
							flags.AddSonyuCondomSame();
							flags.AddSonyuCondomInside();
						}
						flags.AddSonyuOrg();
					}
					else
					{
						if (!flags.isCondom)
						{
							flags.AddSonyuAnalSame();
							flags.AddSonyuAnalInside();
						}
						else
						{
							flags.AddSonyuAnalCondomSame();
							flags.AddSonyuAnalCondomInside();
						}
						flags.AddSonyuAnalOrg();
					}
					SetPlay(flags.isAnalPlay ? "A_SS_IN_Start" : "SS_IN_Start");
					if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && (bool)sprite.objFirstHHelpBase && sprite.objFirstHHelpBase.activeSelf && !flags.isAnalPlay)
					{
						sprite.objFirstHHelpBase.SetActive(false);
					}
					gotoInAfter = 0;
					flags.voice.playVoices[0] = 326 + num;
					flags.isInsideFinish = true;
				}
			}
		}
		else if (animatorStateInfo.IsName("WF_IN_Start") || animatorStateInfo.IsName("A_WF_IN_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_WF_IN_Loop" : "WF_IN_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
			}
		}
		else if (animatorStateInfo.IsName("WF_IN_Loop") || animatorStateInfo.IsName("A_WF_IN_Loop"))
		{
			FinishGaugeDown(1);
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				SetPlay(flags.isAnalPlay ? "A_WS_IN_A" : "WS_IN_A");
				flags.gaugeFemale = 0f;
			}
		}
		else if (animatorStateInfo.IsName("WS_IN_Start") || animatorStateInfo.IsName("A_WS_IN_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_WS_IN_Loop" : "WS_IN_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
			}
		}
		else if (animatorStateInfo.IsName("WS_IN_Loop") || animatorStateInfo.IsName("A_WS_IN_Loop"))
		{
			FinishGaugeDown();
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				SetPlay(flags.isAnalPlay ? "A_WS_IN_A" : "WS_IN_A");
				flags.gaugeMale = 0f;
				flags.gaugeFemale = 0f;
			}
		}
		else if (animatorStateInfo.IsName("SF_IN_Start") || animatorStateInfo.IsName("A_SF_IN_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_SF_IN_Loop" : "SF_IN_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
			}
		}
		else if (animatorStateInfo.IsName("SF_IN_Loop") || animatorStateInfo.IsName("A_SF_IN_Loop"))
		{
			FinishGaugeDown(1);
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				SetPlay(flags.isAnalPlay ? "A_SS_IN_A" : "SS_IN_A");
				flags.gaugeFemale = 0f;
			}
		}
		else if (animatorStateInfo.IsName("SS_IN_Start") || animatorStateInfo.IsName("A_SS_IN_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_SS_IN_Loop" : "SS_IN_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
			}
		}
		else if (animatorStateInfo.IsName("SS_IN_Loop") || animatorStateInfo.IsName("A_SS_IN_Loop"))
		{
			FinishGaugeDown();
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				SetPlay(flags.isAnalPlay ? "A_SS_IN_A" : "SS_IN_A");
				flags.gaugeMale = 0f;
				flags.gaugeFemale = 0f;
			}
		}
		else if (animatorStateInfo.IsName("M_IN_Start") || animatorStateInfo.IsName("A_M_IN_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_M_IN_Loop" : "M_IN_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
			}
		}
		else if (animatorStateInfo.IsName("M_IN_Loop") || animatorStateInfo.IsName("A_M_IN_Loop"))
		{
			FinishGaugeDown(0);
			if (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
			{
				sprite.SetSonyuActionButtonActive(true, 6);
				sprite.sonyu.imageCtrlPad.OnChangeValue(0);
				SetPlay(flags.isAnalPlay ? "A_IN_A" : "IN_A");
				flags.voice.SetSonyuWaitTime(false);
				flags.voice.SetSonyuIdleTime();
				flags.voice.gentles[0] = false;
				rePlay = 0;
				flags.gaugeMale = 0f;
				is70Voices[1] = false;
			}
		}
		else if (animatorStateInfo.IsName("M_OUT_Start") || animatorStateInfo.IsName("A_M_OUT_Start"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				SetPlay(flags.isAnalPlay ? "A_M_OUT_Loop" : "M_OUT_Loop", false);
				flags.timeDecreaseGaugeCalc = 0f;
				male.chaFile.status.visibleGomu = false;
				sprite.SetCondom(false);
			}
		}
		else if (animatorStateInfo.IsName("M_OUT_Loop") || animatorStateInfo.IsName("A_M_OUT_Loop"))
		{
			FinishGaugeDown(0);
			if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && animatorStateInfo.normalizedTime > 5f)
			{
				SetPlay(flags.isAnalPlay ? "A_OUT_A" : "OUT_A");
				sprite.SetSonyuStart();
				flags.timeWaitSonyu.SetIdleTime();
				flags.voice.SetSonyuWaitTime(false);
				flags.voice.SetSonyuIdleTime();
				flags.voice.gentles[0] = false;
				flags.timeWaitSonyu.SetIdleTime();
				flags.isInsideFinish = false;
				rePlay = 0;
				flags.gaugeMale = 0f;
				is70Voices[1] = false;
			}
		}
		else if (animatorStateInfo.IsName("WS_IN_A") || animatorStateInfo.IsName("A_WS_IN_A"))
		{
			switch (gotoInAfter)
			{
			case 0:
				if (animatorStateInfo.normalizedTime > 6f)
				{
					flags.voice.gentles[0] = false;
					flags.voice.SetSonyuWaitTime(false);
					flags.voice.SetSonyuIdleTime();
					if (flags.finish == HFlag.FinishKind.orgW)
					{
						flags.voice.playVoices[0] = 328 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 327 + num;
						is70Voices[1] = false;
					}
					gotoInAfter = 1;
					is70Voices[0] = false;
				}
				break;
			case 1:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
				{
					sprite.SetSonyuActionButtonActive(true, 6);
					sprite.sonyu.imageCtrlPad.OnChangeValue(0);
					SetPlay(flags.isAnalPlay ? "A_IN_A" : "IN_A");
					rePlay = 0;
					gotoInAfter = 0;
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("SS_IN_A") || animatorStateInfo.IsName("A_SS_IN_A"))
		{
			switch (gotoInAfter)
			{
			case 0:
				if (animatorStateInfo.normalizedTime > 6f)
				{
					flags.voice.gentles[0] = false;
					flags.voice.SetSonyuWaitTime(false);
					flags.voice.SetSonyuIdleTime();
					if (flags.finish == HFlag.FinishKind.orgS)
					{
						flags.voice.playVoices[0] = 330 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 329 + num;
						is70Voices[1] = false;
					}
					gotoInAfter = 1;
					is70Voices[0] = false;
				}
				break;
			case 1:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
				{
					sprite.SetSonyuActionButtonActive(true, 6);
					sprite.sonyu.imageCtrlPad.OnChangeValue(0);
					SetPlay(flags.isAnalPlay ? "A_IN_A" : "IN_A");
					rePlay = 0;
					gotoInAfter = 0;
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("IN_A") || animatorStateInfo.IsName("A_IN_A"))
		{
			if (IsVoiceWait())
			{
				return false;
			}
			if (!hand.IsAction())
			{
				flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			}
			flags.MaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge);
			switch (rePlay)
			{
			case 0:
				if (flags.click == HFlag.ClickKind.speedup)
				{
					rePlay = 1;
					List<int> lstKosoKosoCategory = new List<int> { 1205 };
					flags.voice.playVoices[0] = ((!flags.nowAnimationInfo.lstCategory.Any((HSceneProc.Category c) => lstKosoKosoCategory.Contains(c.category))) ? 2 : 12);
					flags.voiceWait = true;
				}
				else if (flags.click == HFlag.ClickKind.modeChange)
				{
					isAuto = !isAuto;
					sprite.sonyu.clickPadCtrl.OnChangeValue(isAuto);
					sprite.sonyu.imageCtrlPad.OnChangeValue(1);
					rePlay = 1;
					List<int> lstKosoKosoCategory2 = new List<int> { 1205 };
					flags.voice.playVoices[0] = ((!flags.nowAnimationInfo.lstCategory.Any((HSceneProc.Category c) => lstKosoKosoCategory2.Contains(c.category))) ? 2 : 12);
					flags.voiceWait = true;
				}
				else if (flags.click == HFlag.ClickKind.pull)
				{
					sprite.SetSonyuActionButtonActive(false);
					sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
					SetPlay(flags.isAnalPlay ? "A_Pull" : "Pull");
					meta.Clear();
					flags.voice.playShorts[0] = (flags.isAnalPlay ? 8 : 7);
					flags.voice.isShortsPlayTouchWeak[0] = false;
					flags.voice.isAfterVoicePlay = false;
				}
				else if (!hand.IsAction() && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime() && !flags.voice.isAfterVoicePlay)
				{
					if (flags.finish == HFlag.FinishKind.inside || flags.finish == HFlag.FinishKind.outside)
					{
						flags.voice.playVoices[0] = 331 + num;
					}
					else if (flags.finish == HFlag.FinishKind.sameW || flags.finish == HFlag.FinishKind.sameS)
					{
						flags.voice.playVoices[0] = 332 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 333 + num;
					}
					if (!flags.voice.IsSonyuIdleFlag())
					{
						flags.voice.SetSonyuWaitTime(true);
						flags.voice.SetSonyuIdleTime();
					}
					flags.voice.isAfterVoicePlay = true;
				}
				break;
			case 1:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
				{
					flags.voiceWait = false;
					flags.voice.isAfterVoicePlay = false;
					if (!isAuto)
					{
						flags.SpeedUpClick(flags.rateSpeedUp, 1f);
					}
					sprite.SetSonyuActionButtonActive(false, 6);
					SetPlay(flags.isAnalPlay ? "A_WLoop" : "WLoop");
					flags.finish = HFlag.FinishKind.none;
					flags.voice.SetSonyuIdleTime();
					meta.Clear();
					isSW = false;
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("A_OUT_A"))
		{
			if (IsVoiceWait())
			{
				return false;
			}
			if (!hand.IsAction())
			{
				flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			}
			flags.MaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge);
			switch (rePlay)
			{
			case 0:
				if (flags.isDenialvoiceWait && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
				{
					if (!flags.isCondom)
					{
						flags.isCondom = true;
						male.chaFile.status.visibleGomu = true;
						sprite.SetCondom(true);
					}
					flags.isDenialvoiceWait = false;
				}
				if (flags.click == HFlag.ClickKind.insert || flags.click == HFlag.ClickKind.insert_voice)
				{
					rePlay = 1;
					kindInsert = ((flags.click != HFlag.ClickKind.insert) ? 1 : 0);
					flags.voiceWait = true;
				}
				else if (flags.click == HFlag.ClickKind.insert_anal || flags.click == HFlag.ClickKind.insert_anal_voice)
				{
					rePlay = 2;
					kindInsert = ((flags.click != HFlag.ClickKind.insert_anal) ? 1 : 0);
					flags.voiceWait = true;
				}
				else
				{
					if (hand.IsAction())
					{
						break;
					}
					if (flags.timeWaitSonyu.IsIdleTime())
					{
						SetPlay(flags.isAnalPlay ? "A_Idle" : "Idle");
						flags.voice.SetSonyuWaitTime(true);
						flags.voice.SetSonyuIdleTime();
						flags.voice.isAfterVoicePlay = false;
						flags.finish = HFlag.FinishKind.none;
						meta.Clear();
					}
					else if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime() && !flags.voice.isAfterVoicePlay)
					{
						if (flags.finish == HFlag.FinishKind.outside)
						{
							flags.voice.playVoices[0] = 335 + num;
						}
						if (!flags.voice.IsSonyuIdleFlag())
						{
							flags.voice.SetSonyuWaitTime(true);
							flags.voice.SetSonyuIdleTime();
						}
						flags.voice.isAfterVoicePlay = true;
					}
				}
				break;
			case 1:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
				{
					flags.voiceWait = false;
					flags.voice.isAfterVoicePlay = false;
					SetPlay("Insert");
					sprite.SetSonyuActionButtonActive(false);
					meta.Clear();
					flags.finish = HFlag.FinishKind.none;
					flags.isAnalPlay = false;
					if (!flags.isCondom)
					{
						flags.SetNameInsert();
					}
					if (kindInsert == 0)
					{
						flags.voice.playVoices[0] = 307 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 308 + num;
					}
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				break;
			case 2:
				if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0])))
				{
					flags.voiceWait = false;
					flags.voice.isAfterVoicePlay = false;
					SetPlay("A_Insert");
					sprite.SetSonyuActionButtonActive(false);
					meta.Clear();
					flags.finish = HFlag.FinishKind.none;
					flags.isAnalPlay = true;
					if (kindInsert == 0)
					{
						flags.voice.playVoices[0] = 307 + num;
					}
					else
					{
						flags.voice.playVoices[0] = 308 + num;
					}
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("Pull") || animatorStateInfo.IsName("A_Pull"))
		{
			if (animatorStateInfo.normalizedTime > 1f)
			{
				if (!flags.isCondom && flags.isInsideFinish)
				{
					SetPlay(flags.isAnalPlay ? "A_Drop" : "Drop");
					if (!flags.isAnalPlay)
					{
						flags.AddSonyuTare();
					}
					else
					{
						flags.AddSonyuAnalTare();
					}
					flags.voice.playVoices[0] = 336 + num;
				}
				else
				{
					SetPlay(flags.isAnalPlay ? "A_OUT_A" : "OUT_A");
					sprite.SetSonyuStart();
					sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
					flags.voice.SetSonyuIdleTime();
					flags.timeWaitSonyu.SetIdleTime();
					rePlay = 0;
				}
			}
		}
		else if ((animatorStateInfo.IsName("Drop") || animatorStateInfo.IsName("A_Drop")) && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]))
		{
			SetPlay(flags.isAnalPlay ? "A_OUT_A" : "OUT_A");
			sprite.SetSonyuStart();
			sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
			flags.voice.SetSonyuIdleTime();
			flags.timeWaitSonyu.SetIdleTime();
			flags.isInsideFinish = false;
			rePlay = 0;
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
		SetAnimatorFloat("Breast", female.GetShapeBodyValue(4), false);
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
		int num = ((Game.isAdd20 && flags.nowAnimationInfo.isFemaleInitiative) ? 38 : 0);
		if (flags.finish == HFlag.FinishKind.none)
		{
			flags.WaitSpeedProc(isAuto, flags.speedSonyuCurve);
			flags.FemaleGaugeUp(Time.deltaTime * flags.rateClickGauge, false, false);
			flags.MaleGaugeUp(Time.deltaTime * flags.rateClickGauge);
			if (flags.click == HFlag.ClickKind.speedup && !isAuto)
			{
				flags.SpeedUpClick(flags.rateSpeedUp, 1f);
			}
			if (isAuto && sprite.IsCursorOnPad())
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
			if (flags.click == HFlag.ClickKind.actionChange)
			{
				sprite.SetSonyuActionButtonActive(false);
				sprite.SetSonyuActionButtonActive(true, 6);
				SetPlay(flags.isAnalPlay ? "A_InsertIdle" : "InsertIdle");
			}
			else if (MathfEx.RangeEqualOn(HFlag.ClickKind.inside, flags.click, HFlag.ClickKind.outside) || flags.gaugeFemale >= 100f)
			{
				if (MathfEx.RangeEqualOn(HFlag.ClickKind.inside, flags.click, HFlag.ClickKind.outside))
				{
					if (flags.click == HFlag.ClickKind.inside)
					{
						flags.finish = ((!(flags.gaugeFemale >= 70f)) ? HFlag.FinishKind.inside : (isSW ? HFlag.FinishKind.sameS : HFlag.FinishKind.sameW));
					}
					else
					{
						flags.finish = HFlag.FinishKind.outside;
					}
					kindInsert = 0;
				}
				else
				{
					flags.finish = ((flags.gaugeMale < 70f || sprite.IsSonyuAutoFinish()) ? (isSW ? HFlag.FinishKind.orgS : HFlag.FinishKind.orgW) : (isSW ? HFlag.FinishKind.sameS : HFlag.FinishKind.sameW));
				}
				sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
			}
			else if (flags.click == HFlag.ClickKind.motionchange)
			{
				SetPlay((!flags.isAnalPlay) ? ((!_loop) ? "SLoop" : "WLoop") : ((!_loop) ? "A_SLoop" : "A_WLoop"));
				isSW = !_loop;
			}
			else if (flags.click == HFlag.ClickKind.modeChange)
			{
				isAuto = !isAuto;
				sprite.sonyu.clickPadCtrl.OnChangeValue(isAuto);
				sprite.sonyu.imageCtrlPad.OnChangeValue(isAuto ? 1 : 0);
			}
			else if (flags.speedCalc == 0f && !isAuto)
			{
				sprite.SetSonyuActionButtonActive(false);
				sprite.SetSonyuActionButtonActive(true, 6);
				SetPlay(flags.isAnalPlay ? "A_InsertIdle" : "InsertIdle");
			}
			else if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime())
			{
				if (flags.gaugeFemale >= 70f && !flags.voice.isFemale70PercentageVoicePlay)
				{
					flags.voice.playVoices[0] = (flags.voice.speedMotion ? 315 : 314) + num;
					flags.voice.isFemale70PercentageVoicePlay = true;
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				else if (flags.gaugeMale >= 70f && !flags.voice.isMale70PercentageVoicePlay)
				{
					flags.voice.playVoices[0] = (flags.voice.speedMotion ? 317 : 316) + num;
					flags.voice.isMale70PercentageVoicePlay = true;
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				else if (!flags.voice.isFemale70PercentageVoicePlay && !flags.voice.isMale70PercentageVoicePlay)
				{
					flags.voice.playVoices[0] = ((!_loop) ? (flags.voice.speedMotion ? 311 : 310) : (flags.voice.speedMotion ? 313 : 312)) + num;
				}
			}
		}
		else if (voice.nowVoices[0].state != HVoiceCtrl.VoiceKind.voice)
		{
			SetPlay(flags.isAnalPlay ? "A_OLoop" : "OLoop");
			isAuto = false;
			flags.speedCalc = 0f;
			flags.timeNoClick = 0f;
			flags.speedUpClac = Vector2.zero;
			flags.timeSpeedUpStartCalc = 0f;
			sprite.SetSonyuActionButtonActive(false);
			sprite.sonyu.clickPadCtrl.OnChangeValue(isAuto);
			sprite.sonyu.imageCtrlPad.OnChangeValue(-1);
			oldGaugeMale = flags.gaugeMale;
			oldGaugeFemale = flags.gaugeFemale;
			if (flags.finish == HFlag.FinishKind.orgW || flags.finish == HFlag.FinishKind.orgS)
			{
				flags.voice.playVoices[0] = 318 + num;
			}
			else if (flags.finish == HFlag.FinishKind.sameS || flags.finish == HFlag.FinishKind.sameW)
			{
				flags.voice.playVoices[0] = 319 + num;
			}
			else if (kindInsert == 0)
			{
				if (flags.finish == HFlag.FinishKind.inside)
				{
					flags.voice.playVoices[0] = ((!flags.isCondom) ? 320 : 323) + num;
				}
				else if (flags.finish == HFlag.FinishKind.outside)
				{
					flags.voice.playVoices[0] = 321 + num;
				}
			}
			else
			{
				flags.voice.playVoices[0] = 322 + num;
			}
		}
		timeAutoMotionCalcs[0] += Time.deltaTime;
		if (timeAutoMotions[0] <= timeAutoMotionCalcs[0] && !enableMotions[0])
		{
			timeAutoMotions[0] = Random.Range(flags.timeAutoMotionMin, flags.timeAutoMotionMax);
			timeAutoMotionCalcs[0] = 0f;
			enableMotions[0] = true;
			timeMotionCalcs[0] = 0f;
			float num2 = 0f;
			if (allowMotions[0])
			{
				num2 = 1f - flags.motion;
				num2 = ((!(num2 <= flags.rateMotionMin)) ? (flags.motion + Random.Range(flags.rateMotionMin, num2)) : 1f);
				if (num2 >= 1f)
				{
					allowMotions[0] = false;
				}
			}
			else
			{
				num2 = flags.motion;
				num2 = ((!(num2 <= flags.rateMotionMin)) ? (flags.motion - Random.Range(flags.rateMotionMin, num2)) : 0f);
				if (num2 <= 0f)
				{
					allowMotions[0] = true;
				}
			}
			lerpMotions[0] = new Vector2(flags.motion, num2);
			timeMotions[0] = Random.Range(flags.timeMotionMin, flags.timeMotionMax);
		}
		return true;
	}

	private void FinishGaugeDown(int _sex = -1)
	{
		flags.timeDecreaseGaugeCalc += Time.deltaTime;
		flags.timeDecreaseGaugeCalc = Mathf.Clamp(flags.timeDecreaseGaugeCalc + Time.deltaTime, 0f, flags.timeDecreaseGauge);
		float t = flags.timeDecreaseGaugeCalc / flags.timeDecreaseGauge;
		if (_sex == 0 || _sex == -1)
		{
			flags.gaugeMale = Mathf.Lerp(oldGaugeMale, 0f, t);
		}
		if (_sex == 1 || _sex == -1)
		{
			flags.gaugeFemale = Mathf.Lerp(oldGaugeFemale, 0f, t);
		}
	}
}
