using System.Collections.Generic;
using UnityEngine;

public class Morphing : MonoBehaviour
{
	public MorphBlinkControl BlinkCtrl;

	public MorphCtrlEyebrow EyebrowCtrl;

	public MorphCtrlEyes EyesCtrl;

	public MorphCtrlMouth MouthCtrl;

	private float voiceValue;

	public EyeLookController EyeLookController;

	[Range(0f, 1f)]
	public float EyeLookUpCorrect = 0.1f;

	[Range(0f, 1f)]
	public float EyeLookDownCorrect = 0.3f;

	[Range(0f, 1f)]
	public float EyeLookSideCorrect = 0.1f;

	private void Awake()
	{
		List<MorphingTargetInfo> list = new List<MorphingTargetInfo>();
		list.Clear();
		EyebrowCtrl.Init(list);
		EyesCtrl.Init(list);
		MouthCtrl.Init(list);
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		BlinkCtrl.CalcBlink();
		float num = 0f;
		if (BlinkCtrl.GetFixedFlags() == 0)
		{
			num = BlinkCtrl.GetOpenRate();
			if ((bool)EyeLookController)
			{
				float angleHRate = EyeLookController.eyeLookScript.GetAngleHRate(EYE_LR.EYE_L);
				float angleVRate = EyeLookController.eyeLookScript.GetAngleVRate();
				float min = 0f - Mathf.Max(EyeLookDownCorrect, EyeLookSideCorrect);
				float num2 = 1f - EyeLookUpCorrect;
				if (num2 > EyesCtrl.OpenMax)
				{
					num2 = EyesCtrl.OpenMax;
				}
				float num3 = 0f;
				num3 = ((!(angleVRate > 0f)) ? (0f - MathfEx.LerpAccel(0f, EyeLookDownCorrect, 0f - angleVRate)) : MathfEx.LerpAccel(0f, EyeLookUpCorrect, angleVRate));
				num3 -= MathfEx.LerpAccel(0f, EyeLookSideCorrect, angleHRate);
				num3 = Mathf.Clamp(num3, min, EyeLookUpCorrect);
				num3 *= 1f - (1f - EyesCtrl.OpenMax);
				EyesCtrl.SetCorrectOpenMax(num2 + num3);
			}
		}
		else
		{
			num = -1f;
		}
		EyebrowCtrl.CalcBlend(num);
		EyesCtrl.CalcBlend(num);
		MouthCtrl.CalcBlend(voiceValue);
	}

	public void SetVoiceVaule(float value)
	{
		voiceValue = value;
	}
}
