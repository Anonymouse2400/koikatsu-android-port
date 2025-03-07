using Manager;
using UnityEngine;

public class HTest3PSonyu : HTestActionBase
{
	private enum VoicePlayShuffleKind
	{
		Idle = 0,
		InsertIdle = 1,
		Loop = 2,
		Loop70 = 3,
		Finish = 4,
		WS_After = 5,
		FemaleAfter = 6,
		OutSideAfter = 7,
		Siru = 8,
		Again = 9
	}

	private bool isAuto;

	private float oldGaugeMale;

	private float oldGaugeFemale;

	private int rePlay;

	private int kindInsert;

	private int gotoInAfter;

	private bool isSW;

	private bool[] is70Voices = new bool[2];

	private ShuffleRand[] voicePlayShuffle = new ShuffleRand[15];

	public HTest3PSonyu(HFlag.DeliveryMember _membar)
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
		SetPlay("Idle");
		rePlay = 0;
		flags.isAnalPlay = false;
		sprite.Houshi3PInitRely();
		isAuto = false;
		sprite.sonyu3P.clickPadCtrl.OnChangeValue(isAuto);
		sprite.imageSpeedSlliderCover70.enabled = false;
		sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
		isSW = false;
		flags.isCondom = sprite.sonyu3P.isCondom;
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
			hand1.Proc(hand.SelectKindTouch != HandCtrl.AibuColliderKind.none);
		}
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
		int num = (flags.nowAnimationInfo.isFemaleInitiative ? 38 : 0);
		int num2 = flags.nowAnimationInfo.id % 2;
		int num3 = num2 ^ 1;
		if (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("A_Idle"))
		{
			if (flags.isDebug)
			{
				if (flags.selectAnimationListInfo != null)
				{
					if (animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
					{
						flags.voiceWait = false;
					}
					return true;
				}
			}
			else if (IsVoiceWait(true))
			{
				return false;
			}
			if (!hand.IsAction())
			{
				flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			}
			flags.MaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge);
			bool flag = false;
			switch (rePlay)
			{
			case 0:
				if (flags.isDenialvoiceWait && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
				{
					if (!flags.isCondom)
					{
						flags.isCondom = true;
						male.chaFile.status.visibleGomu = true;
						sprite.SetCondom3P(true);
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
				else if (!hand.IsAction() && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime())
				{
					flags.voice.playVoices[voicePlayShuffle[0].Get()] = 800 + num;
				}
				break;
			case 1:
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag = true;
				}
				else if (!flags.isDebug && IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
				{
					flag = true;
				}
				if (flag)
				{
					flags.voiceWait = false;
					SetPlay("Insert");
					sprite.SetSonyu3PActionButtonActive(false);
					flags.isAnalPlay = false;
					if (!flags.isCondom)
					{
						flags.SetNameInsert();
					}
					flags.AddSonyuKokanPlay();
					if (kindInsert == 0)
					{
						flags.voice.playVoices[num2] = 807 + num;
					}
					else
					{
						flags.voice.playVoices[num2] = 808 + num;
					}
				}
				break;
			case 2:
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag = true;
				}
				else if (!flags.isDebug && IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
				{
					flag = true;
				}
				if (flag)
				{
					flags.voiceWait = false;
					SetPlay("A_Insert");
					sprite.SetSonyu3PActionButtonActive(false);
					flags.isAnalPlay = true;
					flags.AddSonyuAnalPlay();
					if (kindInsert == 0)
					{
						flags.voice.playVoices[num2] = 807 + num;
					}
					else
					{
						flags.voice.playVoices[num2] = 808 + num;
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
				sprite.SetSonyu3PActionButtonActive(true, 6);
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(0);
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
			if (flags.isDebug)
			{
				if (flags.selectAnimationListInfo != null)
				{
					if (animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
					{
						flags.voiceWait = false;
						if (!flags.isAnalPlay)
						{
							flags.SetInsertKokanVoiceCondition();
						}
						else
						{
							flags.SetInsertAnalVoiceCondition();
						}
					}
					return true;
				}
			}
			else if (IsVoiceWait(true))
			{
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
				sprite.SetSonyu3PActionButtonActive(false, 6);
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
				for (int i = 0; i < 2; i++)
				{
					if (IsVoicePlay(i))
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[i]);
					}
				}
			}
			else if (flags.click == HFlag.ClickKind.modeChange)
			{
				isAuto = !isAuto;
				sprite.sonyu3P.clickPadCtrl.OnChangeValue(isAuto);
				sprite.SetSonyu3PActionButtonActive(false, 6);
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(1);
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
				for (int j = 0; j < 2; j++)
				{
					if (IsVoicePlay(j))
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[j]);
					}
				}
			}
			else if (flags.click == HFlag.ClickKind.pull)
			{
				SetPlay(flags.isAnalPlay ? "A_Idle" : "Idle");
				sprite.SetSonyu3PStart();
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
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
				flags.voice.playVoices[num2] = (flags.isAnalPlay ? 806 : 803) + num;
				if (IsVoicePlay(num3))
				{
					Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num3]);
				}
			}
			else if (!hand.IsAction() && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime())
			{
				flags.voice.playVoices[voicePlayShuffle[1].Get()] = 809 + num;
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
			bool flag2 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag2 = true;
			}
			else if (!flags.isDebug && IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
			{
				flag2 = true;
			}
			if (flag2)
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
					flags.voice.playVoices[num2] = 824 + num;
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
					flags.voice.playVoices[num2] = 834 + num;
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
					gotoInAfter = 0;
					flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 825 + num);
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
					gotoInAfter = 0;
					flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 825 + num);
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
					gotoInAfter = 0;
					flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 826 + num);
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
					gotoInAfter = 0;
					flags.voice.playVoices[0] = (flags.voice.playVoices[1] = 826 + num);
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
			bool flag3 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag3 = true;
			}
			else if (!flags.isDebug && (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breathShort || voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breathShort || (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]) && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[1]))))
			{
				flag3 = true;
			}
			if (flag3)
			{
				for (int k = 0; k < voice.nowVoices.Length; k++)
				{
					if (voice.nowVoices[k].state == HVoiceCtrl.VoiceKind.breathShort && voice.nowVoices[k ^ 1].state == HVoiceCtrl.VoiceKind.voice)
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[k ^ 1]);
					}
				}
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
			bool flag4 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag4 = true;
			}
			else if (!flags.isDebug && (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breathShort || voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breathShort || (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]) && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[1]))))
			{
				flag4 = true;
			}
			if (flag4)
			{
				for (int l = 0; l < voice.nowVoices.Length; l++)
				{
					if (voice.nowVoices[l].state == HVoiceCtrl.VoiceKind.breathShort && voice.nowVoices[l ^ 1].state == HVoiceCtrl.VoiceKind.voice)
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[l ^ 1]);
					}
				}
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
			bool flag5 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag5 = true;
			}
			else if (!flags.isDebug && (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breathShort || voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breathShort || (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]) && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[1]))))
			{
				flag5 = true;
			}
			if (flag5)
			{
				for (int m = 0; m < voice.nowVoices.Length; m++)
				{
					if (voice.nowVoices[m].state == HVoiceCtrl.VoiceKind.breathShort && voice.nowVoices[m ^ 1].state == HVoiceCtrl.VoiceKind.voice)
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[m ^ 1]);
					}
				}
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
			bool flag6 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag6 = true;
			}
			else if (!flags.isDebug && (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breathShort || voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breathShort || (!Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]) && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[1]))))
			{
				flag6 = true;
			}
			if (flag6)
			{
				for (int n = 0; n < voice.nowVoices.Length; n++)
				{
					if (voice.nowVoices[n].state == HVoiceCtrl.VoiceKind.breathShort && voice.nowVoices[n ^ 1].state == HVoiceCtrl.VoiceKind.voice)
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[n ^ 1]);
					}
				}
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
			bool flag7 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag7 = true;
			}
			else if (!flags.isDebug && (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breathShort || voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breathShort || !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[0]) || !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[1])))
			{
				flag7 = true;
			}
			if (flag7)
			{
				for (int num4 = 0; num4 < voice.nowVoices.Length; num4++)
				{
					if (voice.nowVoices[num4].state == HVoiceCtrl.VoiceKind.breathShort && voice.nowVoices[num4 ^ 1].state == HVoiceCtrl.VoiceKind.voice)
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num4 ^ 1]);
					}
				}
				sprite.SetSonyu3PActionButtonActive(true, 6);
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(0);
				SetPlay(flags.isAnalPlay ? "A_IN_A" : "IN_A");
				flags.voice.SetSonyuWaitTime(false);
				flags.voice.SetSonyuIdleTime();
				flags.voice.gentles[0] = false;
				flags.voice.gentles[1] = false;
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
				sprite.SetCondom3P(false);
			}
		}
		else if (animatorStateInfo.IsName("M_OUT_Loop") || animatorStateInfo.IsName("A_M_OUT_Loop"))
		{
			FinishGaugeDown(0);
			bool flag8 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag8 = true;
			}
			else if (!flags.isDebug && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && animatorStateInfo.normalizedTime > 5f)
			{
				flag8 = true;
			}
			if (flag8)
			{
				SetPlay(flags.isAnalPlay ? "A_OUT_A" : "OUT_A");
				sprite.SetSonyu3PStart();
				flags.timeWaitSonyu.SetIdleTime();
				flags.voice.SetSonyuWaitTime(false);
				flags.voice.SetSonyuIdleTime();
				flags.voice.gentles[0] = false;
				flags.voice.gentles[1] = false;
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
			{
				bool flag9 = false;
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag9 = true;
				}
				else if (animatorStateInfo.normalizedTime > 6f)
				{
					flag9 = true;
				}
				if (flag9)
				{
					flags.voice.gentles[0] = false;
					flags.voice.gentles[1] = false;
					flags.voice.SetSonyuWaitTime(false);
					flags.voice.SetSonyuIdleTime();
					if (flags.finish == HFlag.FinishKind.orgW)
					{
						flags.voice.playVoices[voicePlayShuffle[5].Get()] = 828 + num;
					}
					else
					{
						flags.voice.playVoices[voicePlayShuffle[5].Get()] = 827 + num;
						is70Voices[1] = false;
					}
					gotoInAfter = 1;
					is70Voices[0] = false;
				}
				break;
			}
			case 1:
				if (flags.isDebug || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath))
				{
					sprite.SetSonyu3PActionButtonActive(true, 6);
					sprite.sonyu3P.imageCtrlPad.OnChangeValue(0);
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
			{
				bool flag10 = false;
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag10 = true;
				}
				else if (animatorStateInfo.normalizedTime > 6f)
				{
					flag10 = true;
				}
				if (flag10)
				{
					flags.voice.gentles[0] = false;
					flags.voice.gentles[1] = false;
					flags.voice.SetSonyuWaitTime(false);
					flags.voice.SetSonyuIdleTime();
					if (flags.finish == HFlag.FinishKind.orgS)
					{
						flags.voice.playVoices[voicePlayShuffle[5].Get()] = 830 + num;
					}
					else
					{
						flags.voice.playVoices[voicePlayShuffle[5].Get()] = 829 + num;
						is70Voices[1] = false;
					}
					gotoInAfter = 1;
					is70Voices[0] = false;
				}
				break;
			}
			case 1:
				if (flags.isDebug || (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath))
				{
					sprite.SetSonyu3PActionButtonActive(true, 6);
					sprite.sonyu3P.imageCtrlPad.OnChangeValue(0);
					SetPlay(flags.isAnalPlay ? "A_IN_A" : "IN_A");
					rePlay = 0;
					gotoInAfter = 0;
				}
				break;
			}
		}
		else if (animatorStateInfo.IsName("IN_A") || animatorStateInfo.IsName("A_IN_A"))
		{
			if (flags.isDebug)
			{
				if (flags.selectAnimationListInfo != null)
				{
					if (animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
					{
						flags.voiceWait = false;
					}
					return true;
				}
			}
			else if (IsVoiceWait(true))
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
					int num5 = voicePlayShuffle[9].Get();
					flags.voice.playVoices[num5] = 14;
					flags.voiceWait = true;
					if (IsVoicePlay(num5 ^ 1))
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num5 ^ 1]);
					}
				}
				else if (flags.click == HFlag.ClickKind.modeChange)
				{
					isAuto = !isAuto;
					sprite.sonyu3P.clickPadCtrl.OnChangeValue(isAuto);
					sprite.sonyu3P.imageCtrlPad.OnChangeValue(1);
					rePlay = 1;
					int num6 = voicePlayShuffle[9].Get();
					flags.voice.playVoices[num6] = 14;
					flags.voiceWait = true;
					if (IsVoicePlay(num6 ^ 1))
					{
						Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num6 ^ 1]);
					}
				}
				else if (flags.click == HFlag.ClickKind.pull)
				{
					sprite.SetSonyu3PActionButtonActive(false);
					sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
					SetPlay(flags.isAnalPlay ? "A_Pull" : "Pull");
					meta.Clear();
					flags.voice.playShorts[0] = (flags.isAnalPlay ? 8 : 7);
					flags.voice.isShortsPlayTouchWeak[0] = false;
					flags.voice.isAfterVoicePlay = false;
				}
				else if (!hand.IsAction() && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime() && !flags.voice.isAfterVoicePlay)
				{
					if (flags.finish == HFlag.FinishKind.inside || flags.finish == HFlag.FinishKind.outside)
					{
						flags.voice.playVoices[num2] = 831 + num;
					}
					else if (flags.finish == HFlag.FinishKind.sameW || flags.finish == HFlag.FinishKind.sameS)
					{
						flags.voice.playVoices[num2] = 832 + num;
					}
					else
					{
						flags.voice.playVoices[num2 ^ 1] = 833 + num;
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
			{
				bool flag11 = false;
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag11 = true;
				}
				else if (!flags.isDebug && IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
				{
					flag11 = true;
				}
				if (flag11)
				{
					flags.voiceWait = false;
					flags.voice.isAfterVoicePlay = false;
					if (!isAuto)
					{
						flags.SpeedUpClick(flags.rateSpeedUp, 1f);
					}
					sprite.SetSonyu3PActionButtonActive(false, 6);
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
		}
		else if (animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("A_OUT_A"))
		{
			if (flags.isDebug && flags.selectAnimationListInfo != null)
			{
				if (animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flags.voiceWait = false;
				}
				return true;
			}
			if (IsVoiceWait(true))
			{
				return false;
			}
			if (!hand.IsAction())
			{
				flags.FemaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge, false, false);
			}
			flags.MaleGaugeUp((0f - Time.deltaTime) * flags.rateDecreaseGauge);
			bool flag12 = false;
			switch (rePlay)
			{
			case 0:
				if (flags.isDenialvoiceWait && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath)
				{
					if (!flags.isCondom)
					{
						flags.isCondom = true;
						male.chaFile.status.visibleGomu = true;
						sprite.SetCondom3P(true);
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
					if (flags.click == HFlag.ClickKind.Idle || (!flags.isDebug && flags.timeWaitSonyu.IsIdleTime()))
					{
						SetPlay(flags.isAnalPlay ? "A_Idle" : "Idle");
						flags.voice.SetSonyuWaitTime(true);
						flags.voice.SetSonyuIdleTime();
						flags.voice.isAfterVoicePlay = false;
						flags.finish = HFlag.FinishKind.none;
						meta.Clear();
					}
					else if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime() && !flags.voice.isAfterVoicePlay)
					{
						if (flags.finish == HFlag.FinishKind.outside)
						{
							flags.voice.playVoices[voicePlayShuffle[6].Get()] = 835 + num;
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
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag12 = true;
				}
				else if (!flags.isDebug && IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
				{
					flag12 = true;
				}
				if (flag12)
				{
					flags.voiceWait = false;
					flags.voice.isAfterVoicePlay = false;
					SetPlay("Insert");
					sprite.SetSonyu3PActionButtonActive(false);
					meta.Clear();
					flags.finish = HFlag.FinishKind.none;
					flags.isAnalPlay = false;
					if (!flags.isCondom)
					{
						flags.SetNameInsert();
					}
					if (kindInsert == 0)
					{
						flags.voice.playVoices[num2] = 807 + num;
					}
					else
					{
						flags.voice.playVoices[num2] = 808 + num;
					}
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				break;
			case 2:
				if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
				{
					flag12 = true;
				}
				else if (!flags.isDebug && IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
				{
					flag12 = true;
				}
				if (flag12)
				{
					flags.voiceWait = false;
					flags.voice.isAfterVoicePlay = false;
					SetPlay("A_Insert");
					sprite.SetSonyu3PActionButtonActive(false);
					meta.Clear();
					flags.finish = HFlag.FinishKind.none;
					flags.isAnalPlay = true;
					if (kindInsert == 0)
					{
						flags.voice.playVoices[num2] = 807 + num;
					}
					else
					{
						flags.voice.playVoices[num2] = 808 + num;
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
					flags.voice.playVoices[voicePlayShuffle[8].Get()] = 836 + num;
				}
				else
				{
					SetPlay(flags.isAnalPlay ? "A_OUT_A" : "OUT_A");
					sprite.SetSonyu3PStart();
					sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
					flags.voice.SetSonyuIdleTime();
					flags.timeWaitSonyu.SetIdleTime();
					rePlay = 0;
				}
			}
		}
		else if (animatorStateInfo.IsName("Drop") || animatorStateInfo.IsName("A_Drop"))
		{
			bool flag13 = false;
			if (flags.isDebug && animatorStateInfo.normalizedTime > (float)flags.debugForceLoop)
			{
				flag13 = true;
			}
			else if (!flags.isDebug && voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath)
			{
				flag13 = true;
			}
			if (flag13)
			{
				SetPlay(flags.isAnalPlay ? "A_OUT_A" : "OUT_A");
				sprite.SetSonyu3PStart();
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
				flags.voice.SetSonyuIdleTime();
				flags.timeWaitSonyu.SetIdleTime();
				flags.isInsideFinish = false;
				rePlay = 0;
			}
		}
		flags.motion = CalcFluctuation(0, flags.motion);
		flags.motion1 = CalcFluctuation(1, flags.motion1);
		SetAnimatorFloat("speed", flags.speed);
		SetAnimatorFloat("speedHand", 1f + flags.speedItem);
		SetAnimatorFloat("Breast", female.GetShapeBodyValue(4), false);
		SetDefaultAnimatorFloat();
		SetAnimatorFloat(female, "motion1", (flags.nowAnimationInfo.numCtrl != 1) ? flags.motion : flags.motion1);
		SetAnimatorFloat(female1, "motion1", (flags.nowAnimationInfo.numCtrl != 1) ? flags.motion : flags.motion1);
		float shapeBodyValue = female.GetShapeBodyValue(0);
		float shapeBodyValue2 = female1.GetShapeBodyValue(0);
		item.SetAnimatorParamFloat("height", (num2 != 0) ? shapeBodyValue2 : shapeBodyValue);
		item.SetAnimatorParamFloat("height1", (num2 != 0) ? shapeBodyValue : shapeBodyValue2);
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
		int num = (flags.nowAnimationInfo.isFemaleInitiative ? 38 : 0);
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
				sprite.SetSonyu3PActionButtonActive(false);
				sprite.SetSonyu3PActionButtonActive(true, 6);
				SetPlay(flags.isAnalPlay ? "A_InsertIdle" : "InsertIdle");
			}
			else if (MathfEx.RangeEqualOn(HFlag.ClickKind.inside, flags.click, HFlag.ClickKind.outside) || MathfEx.RangeEqualOn(HFlag.ClickKind.orgW, flags.click, HFlag.ClickKind.sameS) || flags.gaugeFemale >= 100f)
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
				else if (MathfEx.RangeEqualOn(HFlag.ClickKind.orgW, flags.click, HFlag.ClickKind.sameS))
				{
					flags.finish = (HFlag.FinishKind)(3 + flags.click - 36);
				}
				else
				{
					flags.finish = ((flags.gaugeMale < 70f || sprite.IsSonyu3PAutoFinish()) ? (isSW ? HFlag.FinishKind.orgS : HFlag.FinishKind.orgW) : (isSW ? HFlag.FinishKind.sameS : HFlag.FinishKind.sameW));
				}
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
			}
			else if (flags.click == HFlag.ClickKind.motionchange)
			{
				SetPlay((!flags.isAnalPlay) ? ((!_loop) ? "SLoop" : "WLoop") : ((!_loop) ? "A_SLoop" : "A_WLoop"));
				isSW = !_loop;
			}
			else if (flags.click == HFlag.ClickKind.modeChange)
			{
				isAuto = !isAuto;
				sprite.sonyu3P.clickPadCtrl.OnChangeValue(isAuto);
				sprite.sonyu3P.imageCtrlPad.OnChangeValue(isAuto ? 1 : 0);
			}
			else if (flags.speedCalc == 0f && !isAuto)
			{
				sprite.SetSonyu3PActionButtonActive(false);
				sprite.SetSonyu3PActionButtonActive(true, 6);
				SetPlay(flags.isAnalPlay ? "A_InsertIdle" : "InsertIdle");
			}
			else if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath && flags.voice.IsSonyuIdleTime())
			{
				if (flags.gaugeFemale >= 70f && !flags.voice.isFemale70PercentageVoicePlay)
				{
					flags.voice.playVoices[voicePlayShuffle[3].Get()] = (flags.voice.speedMotion ? 815 : 814) + num;
					flags.voice.isFemale70PercentageVoicePlay = true;
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				else if (flags.gaugeMale >= 70f && !flags.voice.isMale70PercentageVoicePlay)
				{
					flags.voice.playVoices[voicePlayShuffle[3].Get()] = (flags.voice.speedMotion ? 817 : 816) + num;
					flags.voice.isMale70PercentageVoicePlay = true;
					flags.voice.SetSonyuWaitTime(true);
					flags.voice.SetSonyuIdleTime();
				}
				else if (!flags.voice.isFemale70PercentageVoicePlay && !flags.voice.isMale70PercentageVoicePlay)
				{
					flags.voice.playVoices[voicePlayShuffle[2].Get()] = ((!_loop) ? (flags.voice.speedMotion ? 811 : 810) : (flags.voice.speedMotion ? 813 : 812)) + num;
				}
			}
		}
		else if (voice.nowVoices[0].state == HVoiceCtrl.VoiceKind.breath && voice.nowVoices[1].state == HVoiceCtrl.VoiceKind.breath)
		{
			SetPlay(flags.isAnalPlay ? "A_OLoop" : "OLoop");
			isAuto = false;
			flags.speedCalc = 0f;
			flags.timeNoClick = 0f;
			flags.speedUpClac = Vector2.zero;
			flags.timeSpeedUpStartCalc = 0f;
			sprite.SetSonyu3PActionButtonActive(false);
			sprite.sonyu3P.clickPadCtrl.OnChangeValue(isAuto);
			sprite.sonyu3P.imageCtrlPad.OnChangeValue(-1);
			oldGaugeMale = flags.gaugeMale;
			oldGaugeFemale = flags.gaugeFemale;
			int num2 = flags.nowAnimationInfo.id % 2;
			if (flags.finish == HFlag.FinishKind.orgW || flags.finish == HFlag.FinishKind.orgS)
			{
				flags.voice.playVoices[num2] = 818 + num;
			}
			else if (flags.finish == HFlag.FinishKind.sameS || flags.finish == HFlag.FinishKind.sameW)
			{
				flags.voice.playVoices[num2] = 819 + num;
			}
			else if (kindInsert == 0)
			{
				if (flags.finish == HFlag.FinishKind.inside)
				{
					flags.voice.playVoices[voicePlayShuffle[4].Get()] = ((!flags.isCondom) ? 820 : 823) + num;
				}
				else if (flags.finish == HFlag.FinishKind.outside)
				{
					flags.voice.playVoices[voicePlayShuffle[4].Get()] = 821 + num;
				}
			}
			else
			{
				flags.voice.playVoices[num2] = 822 + num;
			}
		}
		SetFluctuation(0, flags.motion);
		SetFluctuation(1, flags.motion1);
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
