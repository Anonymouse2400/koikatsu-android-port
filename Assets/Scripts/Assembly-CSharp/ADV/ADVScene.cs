using System;
using System.Linq;
using ADV.Backup;
using ActionGame;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace ADV
{
	public class ADVScene : BaseLoader
	{
		private FadeData bkFadeDat;

		private CameraData bkCamDat;

		private bool backDOFConfigEnable;

		[SerializeField]
		private TextScenario scenario;

		[SerializeField]
		private Transform stand;

		[SerializeField]
		private ADVFade advFade;

		[SerializeField]
		private EMTransition emtFade;

		private Camera advCamera;

		private bool isReleased;

		private const string CreateCameraName = "FrontCamera";

		private const string CameraAssetName = "ActionCamera";

		private IDisposable updateDis;

		public TextScenario Scenario
		{
			get
			{
				return scenario;
			}
		}

		public Transform Stand
		{
			get
			{
				return stand;
			}
		}

		public ADVFade AdvFade
		{
			get
			{
				return advFade;
			}
		}

		public EMTransition EMTFade
		{
			get
			{
				return emtFade;
			}
		}

		public ActionMap Map { get; set; }

		public MonoBehaviour nowScene { get; set; }

		public string startAddSceneName { get; private set; }

		public CorrectLightAngle correctLightAngle { get; private set; }

		public CameraEffector cameraEffector { get; private set; }

		private CameraEffectorConfig cameraEffectorConfig { get; set; }

		public DepthOfField dof
		{
			get
			{
				return cameraEffector.dof;
			}
		}

		public Blur blur
		{
			get
			{
				return cameraEffector.blur;
			}
		}

		public float? fadeTime { get; set; }

		private void Init()
		{
			isReleased = false;
			fadeTime = null;
			stand.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			bkFadeDat = new FadeData(Singleton<Scene>.Instance.sceneFade);
			scenario.AdvCamera = (advCamera = Camera.main);
			if (advCamera == null)
			{
				GameObject gameObject = Load<GameObject>("camera/action.unity3d", "ActionCamera", true);
				scenario.AdvCamera = (advCamera = gameObject.GetComponent<Camera>());
				advCamera.name = "FrontCamera";
				Singleton<Manager.Sound>.Instance.Listener = advCamera.transform;
				AssetBundleManager.UnloadAssetBundle("camera/action.unity3d", false);
			}
			cameraEffector = Singleton<Game>.Instance.cameraEffector;
			bkCamDat = new CameraData(advCamera);
			if (blur != null)
			{
				blur.iterations = 0;
				blur.blurSpread = 0.5f;
				blur.enabled = false;
			}
			cameraEffectorConfig = cameraEffector.config;
			backDOFConfigEnable = cameraEffectorConfig.useDOF;
			cameraEffectorConfig.SetDOF(false);
			scenario.isAspect = scenario.isAspect;
			correctLightAngle = scenario.AdvCamera.GetComponent<CorrectLightAngle>();
			if (correctLightAngle != null)
			{
				correctLightAngle.condition = delegate
				{
					ChaControl[] array = (from p in scenario.commandController.Characters.Values
						select p.chaCtrl into p
						where p != null
						where p.visibleAll
						select p).ToArray();
					return (array.Length != 1) ? null : array[0];
				};
			}
			ParameterList.Init();
			if (Map == null)
			{
				Map = base.gameObject.AddComponent<ActionMap>();
			}
			if (scenario.isWait || fadeTime.HasValue)
			{
				float time = 0f;
				if (fadeTime.HasValue)
				{
					time = fadeTime.Value;
				}
				advFade.CrossFadeAlpha(true, 1f, time, true);
				scenario.isWait = true;
				scenario.isWindowImage = false;
				updateDis = (from _ in this.UpdateAsObservable()
					where advFade.IsFadeInEnd
					select _).Take(1).Subscribe(delegate
				{
					scenario.isWait = false;
					scenario.isWindowImage = true;
				}, ParameterList.WaitEndProc);
			}
			scenario.ConfigProc();
		}

		public void Release()
		{
			if (Scene.isGameEnd || isReleased)
			{
				return;
			}
			isReleased = true;
			if (updateDis != null)
			{
				updateDis.Dispose();
			}
			updateDis = null;
			scenario.Release();
			if (!Singleton<Scene>.IsInstance())
			{
				return;
			}
			if (bkFadeDat != null)
			{
				bkFadeDat.Load(Singleton<Scene>.Instance.sceneFade);
			}
			if (advCamera != null)
			{
				if (advCamera.name == "FrontCamera")
				{
					UnityEngine.Object.Destroy(advCamera.gameObject);
				}
				else if (bkCamDat != null)
				{
					bkCamDat.Load(advCamera);
					if (cameraEffectorConfig != null)
					{
						cameraEffectorConfig.SetDOF(backDOFConfigEnable);
					}
				}
			}
			ActionMap component = base.gameObject.GetComponent<ActionMap>();
			if (component != null && Singleton<Scene>.IsInstance())
			{
				Singleton<Scene>.Instance.UnloadBaseScene();
			}
			ParameterList.Release();
			if (correctLightAngle != null)
			{
				correctLightAngle.condition = null;
				correctLightAngle.offset = Vector2.zero;
				correctLightAngle.enabled = true;
				correctLightAngle = null;
			}
		}

		private void OnEnable()
		{
			SceneParameter.advScene = this;
			scenario.OnInitializedAsync.TakeUntilDestroy(this).Subscribe(delegate
			{
				Init();
			});
			startAddSceneName = (Singleton<Scene>.IsInstance() ? Singleton<Scene>.Instance.AddSceneNameOverlapRemoved : string.Empty);
		}

		private void OnDisable()
		{
			SceneParameter.advScene = null;
		}

		private void Update()
		{
			if (Singleton<Scene>.Instance.AddSceneName == "Config")
			{
				scenario.ConfigProc();
			}
		}
	}
}
