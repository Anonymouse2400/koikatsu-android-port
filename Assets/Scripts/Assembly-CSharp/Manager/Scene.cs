using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager
{
	public sealed class Scene : Singleton<Scene>
	{
		public class SceneStack<T> : Stack<T> where T : Data
		{
			private List<string> nowSceneNameList = new List<string>();

			public List<string> NowSceneNameList
			{
				get
				{
					return nowSceneNameList;
				}
			}

			public SceneStack(T item)
			{
				base.Push(item);
				nowSceneNameList.Push(item.levelName);
			}

			public new void Push(T item)
			{
				base.Push(item);
				if (!item.isAdd)
				{
					nowSceneNameList.Clear();
				}
				nowSceneNameList.Push(item.levelName);
				item.Load();
			}

			public new T Pop()
			{
				T val = base.Pop();
				if (nowSceneNameList.Any())
				{
					nowSceneNameList.Pop();
				}
				if (!nowSceneNameList.Any())
				{
					using (Enumerator enumerator = GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							T current = enumerator.Current;
							nowSceneNameList.Add(current.levelName);
							if (!current.isAdd)
							{
								break;
							}
						}
					}
				}
				AssetBundleManager.UnloadAssetBundle(val.assetBundleName, false, val.manifestFileName);
				return val;
			}
		}

		public class Data
		{
			public enum FadeType
			{
				None = 0,
				InOut = 1,
				In = 2,
				Out = 3
			}

			public enum UnloadType
			{
				Success = 0,
				Fail = 1,
				Loaded = 2
			}

			public FadeType fadeType;

			public string assetBundleName = string.Empty;

			public string levelName = string.Empty;

			public bool isAdd;

			public bool isAsync;

			public bool isOverlap;

			public string manifestFileName;

			public bool isLoading;

			public bool isFade
			{
				set
				{
					fadeType = (value ? FadeType.InOut : FadeType.None);
				}
			}

			public bool isFadeIn
			{
				get
				{
					return fadeType == FadeType.InOut || fadeType == FadeType.In;
				}
			}

			public bool isFadeOut
			{
				get
				{
					return fadeType == FadeType.InOut || fadeType == FadeType.Out;
				}
			}

			public AsyncOperation operation { get; private set; }

			public Action onLoad { get; set; }

			public Func<IEnumerator> onFadeIn { get; set; }

			public Func<IEnumerator> onFadeOut { get; set; }

			public AssetBundleLoadLevelOperation assetBundleOperation { get; private set; }

			public AsyncOperation Unload()
			{
				if (!isAdd)
				{
					return null;
				}
				return SceneManager.UnloadSceneAsync(levelName);
			}

			public void Load()
			{
				if (!assetBundleName.IsNullOrEmpty())
				{
					if (!isAsync)
					{
						AssetBundleManager.LoadLevel(assetBundleName, levelName, isAdd, manifestFileName);
					}
					else
					{
						assetBundleOperation = AssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdd, manifestFileName) as AssetBundleLoadLevelOperation;
					}
				}
				else if (!isAsync)
				{
					SceneManager.LoadScene(levelName, isAdd ? LoadSceneMode.Additive : LoadSceneMode.Single);
				}
				else
				{
					operation = SceneManager.LoadSceneAsync(levelName, isAdd ? LoadSceneMode.Additive : LoadSceneMode.Single);
				}
			}
		}

		[Serializable]
		public class FogData
		{
			public FogMode mode = FogMode.Exponential;

			public bool use;

			public Color color = Color.clear;

			public float density = 0.01f;

			public float start;

			public float end = 1000f;

			public void Change()
			{
				RenderSettings.fog = use;
				RenderSettings.fogMode = mode;
				RenderSettings.fogColor = color;
				RenderSettings.fogDensity = density;
				RenderSettings.fogStartDistance = start;
				RenderSettings.fogEndDistance = end;
			}
		}

		private Color initFadeColor;

		[SerializeField]
		private Image nowLoadingImage;

		[SerializeField]
		private Slider progressSlider;

		private SceneStack<Data> sceneStack;

		private Stack<Data> loadStack;

		private int loadingCount;

		private const float FadeWaitTime = 0.1f;

		private Data _baseScene;

		public bool isGameEndCheck;

		public bool isSkipGameExit;

		public GameObject commonSpace { get; private set; }

		public GameObject manager { get; private set; }

		public SceneFade sceneFade { get; private set; }

		public Data baseScene
		{
			get
			{
				return _baseScene;
			}
		}

		public static bool isGameEnd { get; private set; }

		public static bool isReturnTitle { get; set; }

		public static UnityEngine.SceneManagement.Scene ActiveScene
		{
			get
			{
				return SceneManager.GetActiveScene();
			}
			set
			{
				SceneManager.SetActiveScene(value);
			}
		}

		public bool IsOverlap
		{
			get
			{
				return sceneStack.Any() && sceneStack.Peek().isOverlap;
			}
		}

		public bool IsExit
		{
			get
			{
				return AddSceneName == "Exit";
			}
		}

		public bool IsNowLoading
		{
			get
			{
				if (loadStack.Any())
				{
					return true;
				}
				bool result = false;
				foreach (Data item in sceneStack)
				{
					if (item.isLoading)
					{
						result = true;
						break;
					}
					if (item.operation != null && !item.operation.isDone)
					{
						result = true;
						break;
					}
				}
				return result;
			}
		}

		public bool IsNowLoadingFade
		{
			get
			{
				return IsNowLoading || sceneFade.IsFadeNow;
			}
		}

		public bool IsFadeNow
		{
			get
			{
				return sceneFade.IsFadeNow;
			}
		}

		public string LoadSceneName
		{
			get
			{
				return sceneStack.NowSceneNameList.Last();
			}
		}

		public string AddSceneName
		{
			get
			{
				return (sceneStack.NowSceneNameList.Count <= 1) ? string.Empty : sceneStack.NowSceneNameList[0];
			}
		}

		public string AddSceneNameOverlapRemoved
		{
			get
			{
				foreach (Data item in sceneStack)
				{
					if (item.isOverlap)
					{
						continue;
					}
					if (!item.isAdd)
					{
						return string.Empty;
					}
					return item.levelName;
				}
				return string.Empty;
			}
		}

		public string PrevLoadSceneName
		{
			get
			{
				bool flag = false;
				foreach (Data item in sceneStack)
				{
					if (!item.isAdd)
					{
						if (flag)
						{
							return item.levelName;
						}
						flag = true;
					}
				}
				return string.Empty;
			}
		}

		public string PrevAddSceneName
		{
			get
			{
				return (sceneStack.NowSceneNameList.Count <= 2) ? string.Empty : sceneStack.NowSceneNameList[1];
			}
		}

		public List<string> NowSceneNames
		{
			get
			{
				return sceneStack.NowSceneNameList;
			}
		}

		protected override void Awake()
		{
			if (!CheckInstance())
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			GameObject asset = AssetBundleManager.LoadAsset("scene/scenemanager.unity3d", "scenemanager", typeof(GameObject)).GetAsset<GameObject>();
			manager = UnityEngine.Object.Instantiate(asset, base.transform, false);
			manager.name = asset.name;
			nowLoadingImage = manager.GetComponentsInChildren<Image>(true).FirstOrDefault((Image p) => p.name == "NowLoading");
			progressSlider = manager.GetComponentsInChildren<Slider>(true).FirstOrDefault((Slider p) => p.name == "Progress");
			nowLoadingImage.SafeProc(delegate(Image t)
			{
				t.gameObject.SetActive(false);
			});
			progressSlider.SafeProc(delegate(Slider t)
			{
				t.gameObject.SetActive(false);
			});
			sceneFade = manager.GetComponentInChildren<SceneFade>(true);
			initFadeColor = sceneFade._Color;
			sceneFade._Fade = SimpleFade.Fade.Out;
			sceneFade.ForceEnd();
			AssetBundleManager.UnloadAssetBundle("scene/scenemanager.unity3d", false);
			sceneStack = new SceneStack<Data>(new Data
			{
				levelName = ActiveScene.name,
				isAdd = false
			});
			loadStack = new Stack<Data>();
			CreateSpace();
			isGameEndCheck = true;
			isSkipGameExit = false;
		}

		private void OnApplicationQuit()
		{
			if (NowSceneNames.Contains("Exit"))
			{
				GameExit();
			}
			else if (isSkipGameExit)
			{
				Application.CancelQuit();
			}
			else if (isGameEndCheck)
			{
				isGameEndCheck = false;
				bool flag = true;
				flag &= !IsExit;
				flag &= !Application.isEditor;
				flag &= !IsNowLoading;
				flag &= !IsFadeNow;
				flag &= LoadSceneName != "Init";
				if (flag & (LoadSceneName != "Logo"))
				{
					Application.CancelQuit();
					LoadReserve(new Data
					{
						levelName = "Exit",
						isAdd = true,
						isOverlap = true
					}, false);
					isSkipGameExit = true;
				}
				else
				{
					GameExit();
				}
			}
			else
			{
				GameExit();
			}
		}

		private void GameExit()
		{
			isGameEnd = true;
			if (Singleton<Config>.IsInstance())
			{
				Singleton<Config>.Instance.Save();
			}
			if (Singleton<Voice>.IsInstance())
			{
				Singleton<Voice>.Instance.Save();
			}
		}

		private IEnumerator LoadSet(Data data)
		{
			data.isLoading = true;
			while (IsOverlap)
			{
				yield return (!IsExit) ? StartCoroutine(UnLoad(false)) : null;
			}
			sceneStack.Push(data);
			if (data.operation != null)
			{
				data.operation.AsObservable().Subscribe(delegate
				{
					data.isLoading = false;
					data.onLoad.Call();
				});
			}
			int count = 1;
			if (loadingCount != 0)
			{
				count = loadingCount;
			}
			int sceneNum = loadingCount - loadStack.Count;
			if (data.assetBundleOperation != null)
			{
				count++;
				while (data.assetBundleOperation.Request == null)
				{
					progressSlider.SafeProc(delegate(Slider t)
					{
						t.value = (AssetBundleManager.Progress + (float)sceneNum) / (float)count;
					});
					yield return null;
				}
				while (!data.assetBundleOperation.Request.isDone)
				{
					yield return null;
					progressSlider.SafeProc(delegate(Slider t)
					{
						t.value = (AssetBundleManager.Progress + data.assetBundleOperation.Request.progress + (float)sceneNum) / (float)count;
					});
				}
			}
			else if (data.operation != null)
			{
				while (!data.operation.isDone)
				{
					yield return null;
					progressSlider.SafeProc(delegate(Slider t)
					{
						t.value = (data.operation.progress + (float)sceneNum) / (float)count;
					});
				}
			}
			if (data.operation == null)
			{
				data.isLoading = false;
				if (!data.onLoad.IsNullOrEmpty())
				{
					yield return null;
					data.onLoad.Call();
				}
			}
		}

		private void SetImageAlpha(Image image, float alpha)
		{
			if (!(image == null))
			{
				Color color = image.color;
				color.a = alpha;
				image.color = color;
			}
		}

		public void LoadBaseScene(Data data)
		{
			StartCoroutine(LoadBaseSceneCoroutine(data));
		}

		public IEnumerator LoadBaseSceneCoroutine(Data data)
		{
			data.isAdd = true;
			if (data.isFadeIn)
			{
				yield return StartCoroutine(Fade(SimpleFade.Fade.In));
				if (data.onFadeIn != null)
				{
					yield return Observable.FromCoroutine(data.onFadeIn).StartAsCoroutine();
				}
			}
			yield return StartCoroutine(UnloadBaseSceneCoroutine(LoadSceneName));
			data.Load();
			yield return StartCoroutine(UnloadBaseSceneCoroutine(data.levelName));
			_baseScene = data;
			data.onLoad.Call();
			if (data.isFadeOut)
			{
				if (data.onFadeOut != null)
				{
					yield return Observable.FromCoroutine(data.onFadeOut).StartAsCoroutine();
				}
				yield return StartCoroutine(Fade(SimpleFade.Fade.Out));
			}
		}

		public void UnloadBaseScene()
		{
			StartCoroutine(UnloadBaseSceneCoroutine(LoadSceneName));
		}

		public IEnumerator UnloadBaseSceneCoroutine(string levelName)
		{
			if (_baseScene != null)
			{
				if (GetScene(_baseScene.levelName).IsValid())
				{
					yield return SceneManager.UnloadSceneAsync(_baseScene.levelName);
				}
				AssetBundleManager.UnloadAssetBundle(_baseScene.assetBundleName, true);
			}
			_baseScene = null;
			UnityEngine.SceneManagement.Scene scene = GetScene(levelName);
			yield return new WaitWhile(() => !scene.isLoaded);
			ActiveScene = scene;
			Resources.UnloadUnusedAssets();
		}

		public static void MapSettingChange(LightMapDataObject lightMap, FogData fog = null)
		{
			if (lightMap != null)
			{
				lightMap.Change();
			}
			if (fog != null)
			{
				fog.Change();
			}
		}

		public static UnityEngine.SceneManagement.Scene GetScene(string levelName)
		{
			return SceneManager.GetSceneByName(levelName);
		}

		public static GameObject[] GetRootGameObjects(string sceneName)
		{
			UnityEngine.SceneManagement.Scene scene = GetScene(sceneName);
			return scene.isLoaded ? scene.GetRootGameObjects() : null;
		}

		public static T GetRootComponent<T>(string sceneName) where T : Component
		{
			GameObject[] rootGameObjects = GetRootGameObjects(sceneName);
			if (rootGameObjects == null)
			{
				return (T)null;
			}
			GameObject[] array = rootGameObjects;
			foreach (GameObject gameObject in array)
			{
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			GameObject[] array2 = rootGameObjects;
			foreach (GameObject gameObject2 in array2)
			{
				T[] componentsInChildren = gameObject2.GetComponentsInChildren<T>(true);
				if (!componentsInChildren.IsNullOrEmpty())
				{
					return componentsInChildren[0];
				}
			}
			return (T)null;
		}

		public void GameEnd(bool _isCheck = true)
		{
			isGameEndCheck = _isCheck;
			Application.Quit();
		}

		public void LoadReserve(Data data, bool isLoadingImageDraw)
		{
			StartCoroutine(LoadStart(data, isLoadingImageDraw));
		}

		public IEnumerator LoadSceneBack(bool isNowSceneRemove, bool isAddSceneLoad)
		{
			if (isNowSceneRemove && sceneStack.Count > 1)
			{
				sceneStack.Pop();
			}
			do
			{
				loadStack.Push(sceneStack.Pop());
			}
			while (loadStack.Peek().isAdd);
			loadingCount = loadStack.Count;
			while (loadStack.Any())
			{
				Data data = loadStack.Pop();
				if (!isAddSceneLoad)
				{
					loadStack.Clear();
				}
				yield return StartCoroutine(LoadStart(data));
			}
			loadingCount = 0;
		}

		public void UnloadAddScene()
		{
			while (sceneStack.Peek().isAdd)
			{
				sceneStack.Peek().Unload();
				sceneStack.Pop();
			}
			Resources.UnloadUnusedAssets();
		}

		public IEnumerator UnloadAddScene(bool isFade, bool isLoadingImageDraw = false)
		{
			if (isFade)
			{
				yield return StartCoroutine(Fade(SimpleFade.Fade.In));
				DrawImageAndProgress(0f, 1f, isLoadingImageDraw);
				UnloadAddScene();
				if (sceneFade._Fade == SimpleFade.Fade.In)
				{
					yield return StartCoroutine(Fade(SimpleFade.Fade.Out, delegate
					{
						Scene scene = this;
						float a = sceneFade._Color.a;
						scene.DrawImageAndProgress(-1f, a, isLoadingImageDraw);
					}));
					Scene scene2 = this;
					bool isLoadingImageDraw2 = isLoadingImageDraw;
					scene2.DrawImageAndProgress(-1f, -1f, isLoadingImageDraw2);
				}
			}
			else
			{
				UnloadAddScene();
			}
		}

		public void Reload()
		{
			StartCoroutine(LoadSceneBack(false, true));
		}

		public bool UnLoad()
		{
			bool flag = false;
			if (sceneStack.Count <= 1)
			{
				return false;
			}
			Data scene = sceneStack.Peek();
			AsyncOperation asyncOperation = scene.Unload();
			sceneStack.Pop();
			Data.UnloadType unloadType = Data.UnloadType.Success;
			if (asyncOperation != null)
			{
				if (NowSceneNames.Any((string s) => s == scene.levelName))
				{
					unloadType = Data.UnloadType.Loaded;
				}
			}
			else
			{
				unloadType = Data.UnloadType.Fail;
			}
			switch (unloadType)
			{
			case Data.UnloadType.Fail:
				if (AddSceneName.IsNullOrEmpty())
				{
					StartCoroutine(LoadStart(sceneStack.Pop()));
				}
				else
				{
					flag = true;
				}
				break;
			case Data.UnloadType.Loaded:
				flag = true;
				break;
			}
			if (flag)
			{
				do
				{
					loadStack.Push(sceneStack.Pop());
				}
				while (loadStack.Peek().isAdd);
				while (loadStack.Any())
				{
					Data data = loadStack.Pop();
					bool isAsync = data.isAsync;
					data.isAsync = false;
					sceneStack.Push(data);
					data.isAsync = isAsync;
				}
			}
			return true;
		}

		public IEnumerator UnLoad(bool isLoadBack)
		{
			if (sceneStack.Count <= 1)
			{
				yield break;
			}
			bool isLoadSceneBack = false;
			bool isNowSceneRemove = false;
			if (isLoadBack)
			{
				isLoadSceneBack = true;
				isNowSceneRemove = true;
			}
			else
			{
				Data scene = sceneStack.Peek();
				AsyncOperation asyncOperation = scene.Unload();
				sceneStack.Pop();
				Data.UnloadType unloadType = Data.UnloadType.Success;
				if (asyncOperation != null)
				{
					if (NowSceneNames.Any((string s) => s == scene.levelName))
					{
						unloadType = Data.UnloadType.Loaded;
					}
				}
				else
				{
					unloadType = Data.UnloadType.Fail;
				}
				if (unloadType != 0)
				{
					isLoadSceneBack = true;
				}
			}
			if (isLoadSceneBack)
			{
				yield return StartCoroutine(LoadSceneBack(isNowSceneRemove, true));
			}
		}

		public IEnumerator UnLoad(string levelName, Action<bool> act = null)
		{
			bool isFind = IsFind(levelName);
			act.Call(isFind);
			if (isFind)
			{
				RollBack(levelName);
				yield return StartCoroutine(LoadSceneBack(false, true));
			}
		}

		public bool IsFind(string levelName)
		{
			return sceneStack.Any((Data scene) => scene.levelName == levelName);
		}

		public void RollBack(string levelName)
		{
			while (sceneStack.Peek().levelName != levelName)
			{
				sceneStack.Pop();
			}
		}

		public bool StartFade(int _fadeType, Color _color, float _inTime = 1f, float _outTime = 1f, float _waitTime = 1f)
		{
			if (sceneFade == null)
			{
				return false;
			}
			switch (_fadeType)
			{
			case 0:
				sceneFade._Color = _color;
				sceneFade.FadeSet(SimpleFade.Fade.In, _inTime);
				break;
			case 1:
				sceneFade._Color = _color;
				sceneFade.FadeSet(SimpleFade.Fade.Out, _outTime);
				break;
			case 2:
			{
				SimpleFade.FadeInOut fadeInOut = new SimpleFade.FadeInOut();
				fadeInOut.inColor = _color;
				fadeInOut.outColor = _color;
				fadeInOut.inTime = _inTime;
				fadeInOut.outTime = _outTime;
				fadeInOut.waitTime = _waitTime;
				SimpleFade.FadeInOut set = fadeInOut;
				sceneFade.FadeInOutSet(set);
				break;
			}
			}
			return true;
		}

		public void SetFadeColor(Color _color)
		{
			sceneFade.SafeProc(delegate(SceneFade fade)
			{
				fade._Color = _color;
			});
		}

		public void SetFadeColorDefault()
		{
			sceneFade.SafeProc(delegate(SceneFade fade)
			{
				float a = fade._Color.a;
				fade._Color = initFadeColor;
				fade._Color.a = a;
			});
		}

		public void CreateSpace()
		{
			UnityEngine.Object.Destroy(commonSpace);
			commonSpace = new GameObject("CommonSpace");
			UnityEngine.Object.DontDestroyOnLoad(commonSpace);
		}

		public void SpaceRegister(Transform trans, bool worldPositionStays = false)
		{
			trans.SetParent(commonSpace.transform, worldPositionStays);
		}

		public IEnumerator Fade(SimpleFade.Fade fade, Action fadeWaitProc = null)
		{
			sceneFade.FadeSet(fade);
			while (!sceneFade.IsEnd)
			{
				yield return null;
				fadeWaitProc.Call();
			}
		}

		public void DrawImageAndProgress(float value = -1f, float alpha = -1f, bool isLoadingImageDraw = true)
		{
			bool isDraw = value >= 0f;
			progressSlider.SafeProc(delegate(Slider t)
			{
				t.value = ((!isDraw) ? 0f : value);
				t.gameObject.SetActive(isDraw);
			});
			if (alpha < 0f)
			{
				alpha = (isDraw ? 1 : 0);
			}
			SetImageAlpha(nowLoadingImage, alpha);
			nowLoadingImage.SafeProc(delegate(Image t)
			{
				t.gameObject.SetActive(isLoadingImageDraw && alpha > 0f);
			});
		}

		public IEnumerator LoadStart(Data data, bool isLoadingImageDraw = true)
		{
			if (data.isFadeIn && !sceneFade.IsFadeNow)
			{
				if (Time.timeScale == 0f)
				{
					Time.timeScale = 1f;
				}
				yield return StartCoroutine(Fade(SimpleFade.Fade.In));
				yield return new WaitForSeconds(0.1f);
				DrawImageAndProgress((!data.isAsync) ? (-1) : 0, 1f, isLoadingImageDraw);
			}
			if (data.isFadeIn && data.onFadeIn != null)
			{
				yield return Observable.FromCoroutine(data.onFadeIn).StartAsCoroutine();
			}
			yield return StartCoroutine(LoadSet(data));
			if (data.isFadeOut && data.onFadeOut != null)
			{
				yield return Observable.FromCoroutine(data.onFadeOut).StartAsCoroutine();
			}
			if (loadStack.Any())
			{
				yield break;
			}
			Resources.UnloadUnusedAssets();
			if (data.isFadeOut && sceneFade._Fade == SimpleFade.Fade.In)
			{
				yield return new WaitForSeconds(0.1f);
				yield return StartCoroutine(Fade(SimpleFade.Fade.Out, delegate
				{
					Scene scene = this;
					float a = sceneFade._Color.a;
					scene.DrawImageAndProgress(-1f, a, isLoadingImageDraw);
				}));
			}
			Scene scene2 = this;
			bool isLoadingImageDraw2 = isLoadingImageDraw;
			scene2.DrawImageAndProgress(-1f, -1f, isLoadingImageDraw2);
		}
	}
}
