using System;
using FBSAssist;
using UnityEngine;

public class CharaControl : MonoBehaviour
{
	[Serializable]
	public class FaceExpression
	{
		public string type = string.Empty;

		public int eyebrow;

		public int eyes;

		public int mouth;
	}

	[SerializeField]
	private AudioSource audioSingle;

	[SerializeField]
	private FaceBlendShape faceblendshape;

	[SerializeField]
	private FaceExpression[] expression;

	private AudioAssist audioAssist = new AudioAssist();

	private FBSCtrlEyebrow eyebrowCtrl;

	private FBSCtrlEyes eyesCtrl;

	private FBSCtrlMouth mouthCtrl;

	private void Awake()
	{
		if (null != faceblendshape)
		{
			eyebrowCtrl = faceblendshape.EyebrowCtrl;
			eyesCtrl = faceblendshape.EyesCtrl;
			mouthCtrl = faceblendshape.MouthCtrl;
		}
	}

	private void Update()
	{
		float rate = VoiceControl();
		MouthFormControl(rate);
	}

	private void MouthFormControl(float rate)
	{
	}

	private float VoiceControl()
	{
		float num = 0f;
		float correct = 5f;
		if ((bool)audioSingle && audioSingle.isPlaying)
		{
			num = audioAssist.GetAudioWaveValue(audioSingle, correct);
		}
		if ((bool)faceblendshape)
		{
			faceblendshape.SetVoiceVaule(num);
		}
		return num;
	}

	public void ChangeFaceExpression(int type)
	{
		if (expression != null && expression.Length > type)
		{
			ChangeEyebrowPtn(expression[type].eyebrow);
			ChangeEyesPtn(expression[type].eyes);
			ChangeMouthPtn(expression[type].mouth);
		}
	}

	public void ChangeEyebrowPtn(int ptn, bool blend = true)
	{
		if (eyebrowCtrl != null)
		{
			eyebrowCtrl.ChangePtn(ptn, blend);
		}
	}

	public void ChangeEyesPtn(int ptn, bool blend = true)
	{
		if (eyesCtrl != null)
		{
			eyesCtrl.ChangePtn(ptn, blend);
		}
	}

	public void ChangeMouthPtn(int ptn, bool blend = true)
	{
		if (mouthCtrl != null)
		{
			mouthCtrl.ChangePtn(ptn, blend);
		}
	}
}
