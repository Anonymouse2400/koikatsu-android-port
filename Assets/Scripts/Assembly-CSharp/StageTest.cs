using System.Collections;
using System.Linq;
using Illusion.Component;
using Illusion.CustomAttributes;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using WavInfoControl;

public class StageTest : BaseLoader
{
	public class animclip
	{
		public string name;

		public float length;
	}

	public GameObject objCameraPrefabs;

	public StageFaceSetting stageFaceChange;

	public RuntimeAnimatorController animDance;

	public Animator cameraAnim;

	public AudioSource singAudioSource;

	public Animator animFace;

	public getPosition stageEffectgetPosition;

	public charaPosition stageEffectcharaPosition;

	public Twinkle stageEffectTwinkle;

	public GameObject objStageFX;

	public AudioSource AudienceAudioSource;

	public StageEyesLineSetting stageEyesLineSetting;

	public Animator animEyeLine;

	public TextAsset tsLip;

	public EMTransition emTransition;

	public GameObject objCharaSelectCamera;

	public HSceneSpriteObjectCategory canvasCategory;

	public spritestage sprite;

	public LiveCharaSelectSprite spriteCharaSelect;

	public LiveFadeSprite fade;

	public WavInfoData waveInfo = new WavInfoData();

	[Header("キャラ選択")]
	public string pngFileName;

	public ChaFileDefine.CoordinateType coordinateType;

	public string clothChangeFullPath = string.Empty;

	[Header("")]
	[Button("Sync", "再生", new object[] { })]
	public int SyncButton;

	[SerializeField]
	[Label("ショートカットキー")]
	private ShortcutKey shortctKey;

	public int countStart;

	private SaveData.Heroine heroine;

	private ChaControl female;

	private GameObject objCamera;

	private Animator[] stageFXAnimators;

	private CrossFade cross;

	private LightShafts[] lightshafts;

	private GameObject objMic;

	private Camera cameraLive;

	private float fadeCalc;

	private IEnumerator Start()
	{
		Utils.Sound.Play(new Utils.Sound.SettingBGM(BGM.Title));
		if (heroine == null)
		{
			heroine = new SaveData.Heroine(false);
			heroine.charFile.LoadFromAssetBundle("action/fixchara/00.unity3d", "c-5");
			heroine.fixCharaID = -5;
			coordinateType = ChaFileDefine.CoordinateType.Club;
		}
		female = Singleton<Character>.Instance.CreateFemale(null, 0, heroine.charFile);
		heroine.SetRoot(female.gameObject);
		female.releaseCustomInputTexture = false;
		heroine.chaCtrl.ChangeCoordinateType(coordinateType);
		if (!clothChangeFullPath.IsNullOrEmpty())
		{
			heroine.chaCtrl.nowCoordinate.LoadFile(clothChangeFullPath);
		}
		female.Load();
		female.fileStatus.shoesType = 1;
		female.ChangeLookEyesPtn(1);
		female.ChangeLookNeckPtn(3);
		female.animBody.runtimeAnimatorController = animDance;
		objCamera = Object.Instantiate(objCameraPrefabs);
		objCamera.transform.SetParent(female.objBodyBone.transform.FindLoop("cf_n_height").transform, false);
		cameraAnim = objCamera.GetComponent<Animator>();
		cross = objCamera.GetComponentInChildren<CrossFade>();
		stageFaceChange.heroine = heroine;
		stageEyesLineSetting.heroine = heroine;
		spriteCharaSelect.heroine = heroine;
		stageEffectgetPosition.cf_j_foot_L = female.objBodyBone.transform.FindLoop("cf_j_foot_L");
		stageEffectgetPosition.cf_j_foot_R = female.objBodyBone.transform.FindLoop("cf_j_foot_R");
		stageEffectTwinkle.cf_j_hand_R = female.objBodyBone.transform.FindLoop("cf_j_hand_R");
		stageEffectTwinkle.cf_j_index04_R = female.objBodyBone.transform.FindLoop("cf_j_index04_R");
		stageEffectcharaPosition.cf_j_root = female.objBodyBone.transform.FindLoop("cf_j_root");
		DepthOfField depth = Camera.main.GetComponent<DepthOfField>();
		depth.focalTransform = female.objBodyBone.transform.FindLoop("cf_j_hips").transform;
		stageFXAnimators = objStageFX.GetComponentsInChildren<Animator>();
		lightshafts = objStageFX.GetComponentsInChildren<LightShafts>();
		cameraLive = objCamera.GetComponentInChildren<Camera>();
		LightShafts[] array = lightshafts;
		foreach (LightShafts lightShafts in array)
		{
			lightShafts.m_Cameras = new Camera[1] { cameraLive };
		}
		female.SetVoiceTransform((!singAudioSource) ? null : singAudioSource.transform);
		waveInfo.Load(tsLip);
		female.wavInfoData = waveInfo;
		objMic = CommonLib.LoadAsset<GameObject>("chara/ao_hand_00.unity3d", "p_acs_mic", true, string.Empty);
		AssetBundleManager.UnloadAssetBundle("chara/ao_hand_00.unity3d", true);
		GameObject objMicParent = female.objBodyBone.transform.FindLoop("a_n_hand_L");
		if ((bool)objMic && (bool)objMicParent)
		{
			objMic.transform.SetParent(objMicParent.transform, false);
			objMic.transform.localPosition = new Vector3(0.01f, 0f, 0f);
			objMic.transform.localRotation = Quaternion.Euler(4f, -4f, 0f);
		}
		spriteCharaSelect.objMic = objMic;
		canvasCategory.SetActiveToggle(1);
		objCamera.SetActive(false);
		if ((bool)cameraAnim)
		{
			cameraAnim.speed = 0f;
		}
		if ((bool)animFace)
		{
			animFace.speed = 0f;
		}
		if ((bool)animEyeLine)
		{
			animEyeLine.speed = 0f;
		}
		stageFXAnimators.ToList().ForEach(delegate(Animator a)
		{
			a.speed = 0f;
		});
		countStart = 0;
		singAudioSource.outputAudioMixerGroup = Manager.Sound.Mixer.FindMatchingGroups(Manager.Sound.Type.BGM.ToString())[0];
		AudienceAudioSource.outputAudioMixerGroup = Manager.Sound.Mixer.FindMatchingGroups(Manager.Sound.Type.BGM.ToString())[0];
		yield return null;
	}

	private void Update()
	{
		switch (countStart)
		{
		case 1:
			if (shortctKey.procList[1].enabled)
			{
				shortctKey.procList[1].enabled = false;
			}
			if (!((double)cameraAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0))
			{
				countStart = 2;
				StartCoroutine(FadeOut(3f));
				Sync();
				cross.FadeStart(0.3f);
				emTransition.flip = !emTransition.flip;
				emTransition.FlipAnimationCurve();
				emTransition.Play();
			}
			break;
		case 2:
			if (Time.timeScale == 0f)
			{
				Time.timeScale = 1f;
			}
			if (!singAudioSource.isPlaying)
			{
				PlayStop();
			}
			break;
		case 3:
			if (fade.GetFadeKindProc() == LiveFadeSprite.FadeKindProc.OutEnd)
			{
				fade.FadeState(LiveFadeSprite.FadeKind.In);
				if ((bool)singAudioSource)
				{
					singAudioSource.Stop();
				}
				if ((bool)AudienceAudioSource)
				{
					AudienceAudioSource.Stop();
				}
				Utils.Sound.Play(new Utils.Sound.SettingBGM(BGM.Title));
				canvasCategory.SetActiveToggle(1);
				objCamera.SetActive(false);
				objCharaSelectCamera.SetActive(true);
				female.ChangeLookEyesPtn(0);
				BaseCameraControl_Ver2 componentInChildren = objCharaSelectCamera.GetComponentInChildren<BaseCameraControl_Ver2>(true);
				if ((bool)componentInChildren)
				{
					componentInChildren.Reset(0);
				}
				shortctKey.procList[1].enabled = true;
				GameObject gameObject = objCamera.transform.FindLoop("n_cam");
				if ((bool)gameObject)
				{
					Vector3 localScale = new Vector3(0.1f, 10000f, 37.85f);
					gameObject.transform.localScale = localScale;
				}
				female.syncPlay("-1", 0, 0f);
				if ((bool)cameraAnim)
				{
					cameraAnim.speed = 0f;
				}
				if ((bool)animFace)
				{
					animFace.Play("face", 0, 0f);
					animFace.speed = 0f;
				}
				if ((bool)animEyeLine)
				{
					animEyeLine.Play("eyesline", 0, 0f);
					animEyeLine.speed = 0f;
				}
				stageFXAnimators.ToList().ForEach(delegate(Animator a)
				{
					a.Play("001", 0, 0f);
					a.speed = 0f;
				});
				countStart = 0;
			}
			break;
		}
	}

	public void Sync()
	{
		female.syncPlay("01", 0, 0f);
		if ((bool)cameraAnim)
		{
			cameraAnim.speed = 1f;
			cameraAnim.Play("c001", 0, 0f);
		}
		if ((bool)animFace)
		{
			animFace.speed = 1f;
			animFace.Play("face", 0, 0f);
		}
		if ((bool)animEyeLine)
		{
			animEyeLine.speed = 1f;
			animEyeLine.Play("eyesline", 0, 0f);
		}
		stageFXAnimators.ToList().ForEach(delegate(Animator a)
		{
			a.speed = 1f;
			a.Play("001", 0, 0f);
		});
		if ((bool)singAudioSource)
		{
			singAudioSource.time = 0f;
			singAudioSource.Play();
		}
	}

	public void PlayStart()
	{
		Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.BGM);
		canvasCategory.SetActiveToggle(0);
		objCamera.SetActive(true);
		objCharaSelectCamera.SetActive(false);
		if ((bool)cameraAnim)
		{
			cameraAnim.speed = 1f;
			cameraAnim.Play("-1", 0, 0f);
		}
		if ((bool)animFace)
		{
			animFace.speed = 0f;
		}
		if ((bool)animEyeLine)
		{
			animEyeLine.speed = 0f;
		}
		stageFXAnimators.ToList().ForEach(delegate(Animator a)
		{
			a.speed = 0f;
		});
		fadeCalc = 0f;
		AudienceAudioSource.volume = 1f;
		AudienceAudioSource.Play();
		DepthOfField component = Camera.main.GetComponent<DepthOfField>();
		component.focalTransform = female.objBodyBone.transform.FindLoop("cf_j_hips").transform;
		female.ChangeLookEyesTarget(0);
		if (!emTransition.flip)
		{
			emTransition.flip = true;
			emTransition.FlipAnimationCurve();
		}
		emTransition.Play();
		countStart = 1;
		shortctKey.procList[1].enabled = false;
	}

	public void PlayStop()
	{
		if (countStart == 1 || countStart == 2)
		{
			fade.FadeState(LiveFadeSprite.FadeKind.Out);
			countStart = 3;
		}
	}

	private IEnumerator FadeOut(float _timeFade)
	{
		while (true)
		{
			fadeCalc = Mathf.Clamp(fadeCalc + Time.deltaTime, 0f, _timeFade);
			AudienceAudioSource.volume = 1f - fadeCalc / _timeFade;
			if (fadeCalc >= _timeFade)
			{
				break;
			}
			yield return null;
		}
	}
}
