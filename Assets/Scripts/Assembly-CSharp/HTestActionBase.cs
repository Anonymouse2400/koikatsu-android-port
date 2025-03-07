using System.Collections.Generic;
using Manager;
using UnityEngine;

public class HTestActionBase
{
	protected ChaControl female;

	protected ChaControl female1;

	protected ChaControl male;

	protected HFlag flags;

	protected CrossFade fade;

	protected HSprite sprite;

	protected List<MotionIK> lstMotionIK;

	protected HandCtrl hand;

	protected HandCtrl hand1;

	protected YureCtrl yure;

	protected YureCtrl yure1;

	protected ItemObject item;

	protected HitCollisionEnableCtrl hitcolFemale;

	protected HitCollisionEnableCtrl hitcolFemale1;

	protected HitCollisionEnableCtrl hitcolMale;

	protected MetaballCtrl meta;

	protected ParentObjectCtrl parentObjectFemale;

	protected ParentObjectCtrl parentObjectFemale1;

	protected ParentObjectCtrl parentObjectMale;

	protected HVoiceCtrl voice;

	protected HSeCtrl se;

	protected HParticleCtrl particle;

	protected AnimatorLayerCtrl alCtrl;

	protected AnimatorLayerCtrl alCtrl1;

	protected float[] timeAutoMotions = new float[2];

	protected float[] timeAutoMotionCalcs = new float[2];

	protected float[] timeMotions = new float[2];

	protected float[] timeMotionCalcs = new float[2];

	protected bool[] enableMotions = new bool[2];

	protected bool[] allowMotions = new bool[2] { true, true };

	protected Vector2[] lerpMotions = new Vector2[2]
	{
		Vector2.zero,
		Vector2.zero
	};

	public HTestActionBase(HFlag.DeliveryMember _membar)
	{
		female = _membar.chaFemale;
		female1 = _membar.chaFemale1;
		male = _membar.chaMale;
		flags = _membar.ctrlFlag;
		fade = _membar.fade;
		sprite = _membar.sprite;
		lstMotionIK = _membar.lstMotionIK;
		hand = _membar.hand;
		hand1 = _membar.hand1;
		yure = _membar.yure;
		yure1 = _membar.yure1;
		item = _membar.item;
		hitcolFemale = _membar.hitcolFemale;
		hitcolFemale1 = _membar.hitcolFemale1;
		hitcolMale = _membar.hitcolMale;
		meta = _membar.meta;
		parentObjectFemale = _membar.parentObjectFemale;
		parentObjectFemale1 = _membar.parentObjectFemale1;
		parentObjectMale = _membar.parentObjectMale;
		voice = _membar.voice;
		se = _membar.se;
		particle = _membar.particle;
		alCtrl = _membar.alCtrl;
		alCtrl1 = _membar.alCtrl1;
	}

	public virtual bool Proc()
	{
		return true;
	}

	public virtual bool LateProc()
	{
		return true;
	}

	public virtual bool MotionChange(int _motion)
	{
		return true;
	}

	public bool SetPlay(string _nextAnimation, bool _fade = true)
	{
		flags.nowAnimStateName = _nextAnimation;
		if (_fade)
		{
			fade.FadeStart();
		}
		female.setPlay(_nextAnimation, 0);
		if ((bool)female1)
		{
			female1.setPlay(_nextAnimation, 0);
		}
		if (male.visibleAll)
		{
			male.setPlay(_nextAnimation, 0);
		}
		lstMotionIK.ForEach(delegate(MotionIK motionIK)
		{
			motionIK.Calc(_nextAnimation);
		});
		HSceneProc.FemaleParameter paramFemale = flags.nowAnimationInfo.paramFemale;
		HSceneProc.FemaleParameter paramFemale2 = flags.nowAnimationInfo.paramFemale1;
		yure.Proc(_nextAnimation, !paramFemale.isYure);
		if (yure1 != null)
		{
			yure1.Proc(_nextAnimation, !paramFemale2.isYure);
		}
		hand.SetAnimation(_nextAnimation);
		hand1.SetAnimation(_nextAnimation);
		item.SetPlay(_nextAnimation);
		hitcolFemale.SetPlay(_nextAnimation);
		hitcolFemale1.SetPlay(_nextAnimation);
		hitcolMale.SetPlay(_nextAnimation);
		parentObjectFemale.Proc(_nextAnimation);
		if ((bool)parentObjectFemale1)
		{
			parentObjectFemale1.Proc(_nextAnimation);
		}
		parentObjectMale.Proc(_nextAnimation);
		alCtrl.Proc(_nextAnimation);
		alCtrl1.Proc(_nextAnimation);
		return true;
	}

	protected bool SetAnimatorFloat(string _param, float _value, bool _isMale = true, bool _isFemale1 = true)
	{
		female.setAnimatorParamFloat(_param, _value);
		if ((bool)female1 && _isFemale1)
		{
			female1.setAnimatorParamFloat(_param, _value);
		}
		if (male.visibleAll && _isMale)
		{
			male.setAnimatorParamFloat(_param, _value);
		}
		item.SetAnimatorParamFloat(_param, _value);
		return true;
	}

	protected bool SetAnimatorFloat(ChaControl _female, string _param, float _value)
	{
		_female.setAnimatorParamFloat(_param, _value);
		return true;
	}

	protected bool SetDefaultAnimatorFloat()
	{
		SetAnimatorFloat("motion", flags.motion);
		SetAnimatorFloat("height", female.GetShapeBodyValue(0), true, false);
		if ((bool)female1)
		{
			SetAnimatorFloat(female1, "height", female1.GetShapeBodyValue(0));
		}
		female.setAnimatorParamFloat("muneL_X", flags.xy[0].x);
		female.setAnimatorParamFloat("muneL_Y", flags.xy[0].y);
		female.setAnimatorParamFloat("muneR_X", flags.xy[1].x);
		female.setAnimatorParamFloat("muneR_Y", flags.xy[1].y);
		female.setAnimatorParamFloat("kokan_X", flags.xy[2].x);
		female.setAnimatorParamFloat("kokan_Y", flags.xy[2].y);
		female.setAnimatorParamFloat("anal_X", flags.xy[3].x);
		female.setAnimatorParamFloat("anal_Y", flags.xy[3].y);
		female.setAnimatorParamFloat("siriL_X", flags.xy[4].x);
		female.setAnimatorParamFloat("siriL_Y", flags.xy[4].y);
		female.setAnimatorParamFloat("siriR_X", flags.xy[5].x);
		female.setAnimatorParamFloat("siriR_Y", flags.xy[5].y);
		hand.SetItemAnimatorParam();
		return true;
	}

	protected bool IsBodyTouch()
	{
		return hand.GetUseItemNumber().Count != 0;
	}

	protected bool IsCheckVoicePlay(int _index)
	{
		return voice.nowVoices[_index].state == HVoiceCtrl.VoiceKind.breath || (voice.nowVoices[_index].state == HVoiceCtrl.VoiceKind.voice && !Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[_index]));
	}

	protected bool IsVoiceWait(bool _is3P = false)
	{
		if (flags.selectAnimationListInfo == null)
		{
			return false;
		}
		if (!_is3P)
		{
			if (IsCheckVoicePlay(0))
			{
				flags.voiceWait = false;
			}
		}
		else if (IsCheckVoicePlay(0) && IsCheckVoicePlay(1))
		{
			flags.voiceWait = false;
		}
		return true;
	}

	protected bool IsVoicePlay(int _index)
	{
		return voice.nowVoices[_index].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[_index]);
	}

	protected void SetMoveCameraFlag()
	{
		if (!GlobalMethod.IsCameraMoveFlag(flags.ctrlCamera) && !Input.GetMouseButton(0))
		{
			GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, true);
		}
	}
}
