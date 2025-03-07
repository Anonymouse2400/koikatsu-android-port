using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Illusion.Extensions;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Studio
{
	public class Map : Singleton<Map>
	{
		public const string MAP_ROOT_NAME = "Map";

		private List<GameObject> lstOption;

		private Renderer[] mobRenderers;

		public GameObject mapRoot { get; private set; }

		public SunLightInfo sunLightInfo { get; private set; }

		public SunLightInfo.Info.Type sunType
		{
			get
			{
				return (SunLightInfo.Info.Type)Singleton<Studio>.Instance.sceneInfo.sunLightType;
			}
			set
			{
				Singleton<Studio>.Instance.sceneInfo.sunLightType = (int)value;
				if ((bool)sunLightInfo)
				{
					sunLightInfo.Set(value, Singleton<Studio>.Instance.cameraCtrl.mainCmaera);
				}
				Singleton<Studio>.Instance.systemButtonCtrl.MapDependent();
			}
		}

		public bool isSunLightInfo
		{
			get
			{
				return sunLightInfo != null;
			}
		}

		public SunLightInfo.Info mapEffectInfo
		{
			get
			{
				return sunLightInfo.infos.FirstOrDefault((SunLightInfo.Info p) => p.type == sunType);
			}
		}

		public bool isLoading { get; private set; }

		public int no { get; private set; }

		public bool isOption
		{
			get
			{
				return !lstOption.IsNullOrEmpty();
			}
		}

		public bool visibleOption
		{
			get
			{
				return Singleton<Studio>.Instance.sceneInfo.mapOption;
			}
			set
			{
				Singleton<Studio>.Instance.sceneInfo.mapOption = value;
				foreach (GameObject item in lstOption)
				{
					item.SetActiveIfDifferent(value);
				}
			}
		}

		public IEnumerator LoadMapCoroutine(int _no, bool _wait = false)
		{
			if (!Singleton<Info>.Instance.dicMapLoadInfo.ContainsKey(_no))
			{
				ReleaseMap();
			}
			else if (no != _no)
			{
				lstOption.Clear();
				mobRenderers = null;
				if (_wait)
				{
					Singleton<Manager.Scene>.Instance.LoadReserve(new Manager.Scene.Data
					{
						levelName = "StudioWait",
						isAdd = true
					}, false);
				}
				isLoading = true;
				no = _no;
				Info.MapLoadInfo data = Singleton<Info>.Instance.dicMapLoadInfo[_no];
				Singleton<Manager.Scene>.Instance.LoadBaseScene(new Manager.Scene.Data
				{
					assetBundleName = data.bundlePath,
					levelName = data.fileName,
					fadeType = Manager.Scene.Data.FadeType.None
				});
				yield return new WaitWhile(() => isLoading);
				if (_wait)
				{
					Singleton<Manager.Scene>.Instance.UnLoad();
				}
			}
		}

		public void LoadMap(int _no)
		{
			if (!Singleton<Info>.Instance.dicMapLoadInfo.ContainsKey(_no))
			{
				ReleaseMap();
			}
			else if (no != _no)
			{
				lstOption.Clear();
				mobRenderers = null;
				isLoading = true;
				no = _no;
				Info.MapLoadInfo mapLoadInfo = Singleton<Info>.Instance.dicMapLoadInfo[_no];
				Singleton<Manager.Scene>.Instance.LoadBaseScene(new Manager.Scene.Data
				{
					assetBundleName = mapLoadInfo.bundlePath,
					levelName = mapLoadInfo.fileName,
					fadeType = Manager.Scene.Data.FadeType.None
				});
			}
		}

		public void ReleaseMap()
		{
			if (Singleton<Map>.IsInstance())
			{
				mapRoot = null;
				sunLightInfo = null;
				no = -1;
				Singleton<Manager.Scene>.Instance.UnloadBaseScene();
				Singleton<Studio>.Instance.SetSunCaster(Singleton<Studio>.Instance.sceneInfo.sunCaster);
			}
		}

		public void ReflectMobColor()
		{
			if (!mobRenderers.IsNullOrEmpty())
			{
				AdditionalFunctionsSystem addData = Manager.Config.AddData;
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetColor(ChaShader._Color, addData.mobColor);
				Renderer[] array = mobRenderers;
				foreach (Renderer renderer in array)
				{
					renderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
		}

		private void Reserve(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
		{
			mapRoot = newScene.GetRootGameObjects().FirstOrDefault((GameObject p) => p.name == "Map");
			if (mapRoot == null)
			{
				return;
			}
			isLoading = false;
			sunLightInfo = mapRoot.GetComponent<SunLightInfo>();
			sunType = sunType;
			switch (no)
			{
			case 23:
			{
				GameObject gameObject2 = mapRoot.transform.FindLoop("pool_mob");
				MobAutoHideComponent component = gameObject2.GetComponent<MobAutoHideComponent>();
				if (component != null)
				{
					component.enabled = false;
				}
				for (int i = 0; i < gameObject2.transform.childCount; i++)
				{
					lstOption.Add(gameObject2.transform.GetChild(i).gameObject);
				}
				visibleOption = visibleOption;
				break;
			}
			case 38:
			case 39:
			{
				GameObject gameObject = mapRoot.transform.FindLoop("MapMobGroup");
				if ((bool)gameObject)
				{
					lstOption.Add(gameObject);
					mobRenderers = gameObject.GetComponentsInChildren<Renderer>();
					ReflectMobColor();
					visibleOption = visibleOption;
				}
				break;
			}
			}
		}

		protected void OnDestroy()
		{
			SceneManager.activeSceneChanged -= Reserve;
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
				isLoading = false;
				no = -1;
				mapRoot = null;
				sunLightInfo = null;
				lstOption = new List<GameObject>();
				SceneManager.activeSceneChanged += Reserve;
			}
		}
	}
}
