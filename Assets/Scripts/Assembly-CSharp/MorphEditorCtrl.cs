using IllusionUtility.GetUtility;
using MorphAssist;
using UnityEngine;

public class MorphEditorCtrl : MonoBehaviour
{
	private AudioAssist audioAssist;

	private bool[,] clickButton = new bool[3, 2];

	private bool playVoice;

	private bool clickPlay;

	private Morphing morphing;

	private MorphCtrlEyebrow EyebrowCtrl;

	private MorphCtrlEyes EyesCtrl;

	private MorphCtrlMouth MouthCtrl;

	private void Awake()
	{
		GameObject gameObject = base.transform.FindTop();
		GameObject gameObject2 = gameObject.transform.FindLoop("MorphCtrl");
		morphing = (Morphing)gameObject2.GetComponent("Morphing");
		EyebrowCtrl = morphing.EyebrowCtrl;
		EyesCtrl = morphing.EyesCtrl;
		MouthCtrl = morphing.MouthCtrl;
		audioAssist = new AudioAssist();
	}

	private void Start()
	{
	}

	private void Update()
	{
		float audioWaveValue = audioAssist.GetAudioWaveValue(GetComponent<AudioSource>());
		if (clickButton[0, 0])
		{
			int ptn = Mathf.Max(0, EyebrowCtrl.NowPtn - 1);
			EyebrowCtrl.ChangePtn(ptn, true);
			clickButton[0, 0] = false;
		}
		else if (clickButton[0, 1])
		{
			int ptn2 = Mathf.Min(EyebrowCtrl.GetMaxPtn() - 1, EyebrowCtrl.NowPtn + 1);
			EyebrowCtrl.ChangePtn(ptn2, true);
			clickButton[0, 1] = false;
		}
		if (clickButton[1, 0])
		{
			int ptn3 = Mathf.Max(0, EyesCtrl.NowPtn - 1);
			EyesCtrl.ChangePtn(ptn3, true);
			clickButton[1, 0] = false;
		}
		else if (clickButton[1, 1])
		{
			int ptn4 = Mathf.Min(EyesCtrl.GetMaxPtn() - 1, EyesCtrl.NowPtn + 1);
			EyesCtrl.ChangePtn(ptn4, true);
			clickButton[1, 1] = false;
		}
		if (clickButton[2, 0])
		{
			int ptn5 = Mathf.Max(0, MouthCtrl.NowPtn - 1);
			MouthCtrl.ChangePtn(ptn5, true);
			clickButton[2, 0] = false;
		}
		else if (clickButton[2, 1])
		{
			int ptn6 = Mathf.Min(MouthCtrl.GetMaxPtn() - 1, MouthCtrl.NowPtn + 1);
			MouthCtrl.ChangePtn(ptn6, true);
			clickButton[2, 1] = false;
		}
		morphing.SetVoiceVaule(audioWaveValue);
		if (clickPlay)
		{
			if (playVoice)
			{
				playVoice = false;
				GetComponent<AudioSource>().Stop();
			}
			else
			{
				playVoice = true;
				GetComponent<AudioSource>().Play();
			}
			clickPlay = false;
		}
	}

	private void PushEyebrowBackPtn()
	{
		clickButton[0, 0] = true;
	}

	private void PushEyebrowNextPtn()
	{
		clickButton[0, 1] = true;
	}

	private void PushEyesBackPtn()
	{
		clickButton[1, 0] = true;
	}

	private void PushEyesNextPtn()
	{
		clickButton[1, 1] = true;
	}

	private void PushMouthBackPtn()
	{
		clickButton[2, 0] = true;
	}

	private void PushMouthNextPtn()
	{
		clickButton[2, 1] = true;
	}

	private void PushPlayVoice()
	{
		clickPlay = true;
	}
}
