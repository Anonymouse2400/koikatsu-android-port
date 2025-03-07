using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame;
using Localize.Translate;
using Manager;
using UnityEngine;

namespace ADV
{
	public static class Program
	{
		[Serializable]
		public class Transfer
		{
			public int line { get; private set; }

			public ScenarioData.Param param { get; private set; }

			public Transfer(ScenarioData.Param param)
			{
				line = -1;
				this.param = param;
			}

			public static List<Transfer> NewList(bool multi = true, bool isSceneRegulate = false)
			{
				List<Transfer> list = new List<Transfer>();
				list.Add(Create(multi, Command.SceneFadeRegulate, isSceneRegulate.ToString()));
				return list;
			}

			public static Transfer Create(bool multi, Command command, params string[] args)
			{
				return new Transfer(new ScenarioData.Param(multi, command, args));
			}

			public static Transfer VAR(params string[] args)
			{
				return Create(true, Command.VAR, args);
			}

			public static Transfer Open(params string[] args)
			{
				return Create(false, Command.Open, args);
			}

			public static Transfer Close()
			{
				return Create(false, Command.Close, (string[])null);
			}

			public static Transfer Text(params string[] args)
			{
				return Create(false, Command.Text, args);
			}

			public static Transfer Voice(params string[] args)
			{
				return Create(true, Command.Voice, args);
			}

			public static Transfer Motion(params string[] args)
			{
				return Create(true, Command.Motion, args);
			}

			public static Transfer Expression(params string[] args)
			{
				return Create(true, Command.Expression, args);
			}

			public static Transfer ExpressionIcon(params string[] args)
			{
				return Create(true, Command.ExpressionIcon, args);
			}
		}

		public class OpenDataProc
		{
			public Action onLoad { get; set; }

			public Func<IEnumerator> onFadeIn { get; set; }

			public Func<IEnumerator> onFadeOut { get; set; }

			public Scene.Data.FadeType fadeType { get; set; }
		}

		public const string BASE_FOV = "23";

		public static bool isADVActionActive
		{
			get
			{
				bool result = false;
				if (Singleton<Game>.IsInstance())
				{
					ActionScene actScene = Singleton<Game>.Instance.actScene;
					if (actScene != null && actScene.AdvScene != null)
					{
						result = actScene.AdvScene.gameObject.activeSelf;
					}
				}
				return result;
			}
		}

		public static bool isADVScene
		{
			get
			{
				return Singleton<Scene>.Instance.NowSceneNames.Contains("ADV");
			}
		}

		public static bool isADVProcessing
		{
			get
			{
				return isADVActionActive || isADVScene;
			}
		}

		public static void SetParam(SaveData.Player player, SaveData.Heroine heroine, List<Transfer> list)
		{
			SetParam(player, list);
			SetParam(heroine, list);
		}

		public static void SetParam(SaveData.CharaData chara, List<Transfer> list)
		{
			if (chara != null)
			{
				chara.SetADVParam(list);
			}
		}

		public static string ScenarioBundle(string file)
		{
			return Localize.Translate.Manager.AdvScenarioPath + file + ".unity3d";
		}

		public static void SetNull(Transform transform, int version, string mapName, string nullName, BaseMap map)
		{
			Vector3 position;
			Quaternion rotation;
			if (GetNull(version, mapName, nullName, map, out position, out rotation))
			{
				transform.SetPositionAndRotation(position, rotation);
			}
		}

		public static bool GetNull(int version, string mapName, string nullName, BaseMap map, out Vector3 position, out Quaternion rotation)
		{
			bool result = false;
			position = Vector3.zero;
			rotation = Quaternion.identity;
			mapName = ((!(mapName == "デフォルト")) ? map.GetParam(mapName).AssetName.ToLower() : "default");
			string assetBundleName = string.Format("{0}{1:00}/{2}{3}", "map/advpos/", version, mapName, ".unity3d");
			GameObject asset = AssetBundleManager.LoadAsset(assetBundleName, nullName, typeof(GameObject)).GetAsset<GameObject>();
			if (asset != null)
			{
				Transform transform = asset.transform;
				position = transform.position;
				rotation = transform.rotation;
				result = true;
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleName, false, null, true);
			return result;
		}

		public static string FindADVBundleFilePath(int advNo, SaveData.Heroine heroine)
		{
			string text = null;
			foreach (string item in from bundle in CommonLib.GetAssetBundleNameListFromPath(Localize.Translate.Manager.AdvScenarioPath + heroine.ChaName + "/", true)
				orderby bundle descending
				select bundle)
			{
				ScenarioData[] allAssets = AssetBundleManager.LoadAllAsset(item, typeof(ScenarioData)).GetAllAssets<ScenarioData>();
				foreach (ScenarioData scenarioData in allAssets)
				{
					int result;
					if (int.TryParse(scenarioData.name, out result) && result == advNo)
					{
						text = item;
						break;
					}
				}
				AssetBundleManager.UnloadAssetBundle(item, false);
				if (text != null)
				{
					break;
				}
			}
			return text;
		}

		public static string FindADVBundleFilePath(string fileName, SaveData.Heroine heroine)
		{
			string text = null;
			foreach (string item in from bundle in CommonLib.GetAssetBundleNameListFromPath(Localize.Translate.Manager.AdvScenarioPath + heroine.ChaName + "/", true)
				orderby bundle descending
				select bundle)
			{
				ScenarioData[] allAssets = AssetBundleManager.LoadAllAsset(item, typeof(ScenarioData)).GetAllAssets<ScenarioData>();
				foreach (ScenarioData scenarioData in allAssets)
				{
					if (scenarioData.name == fileName)
					{
						text = item;
						break;
					}
				}
				AssetBundleManager.UnloadAssetBundle(item, false);
				if (text != null)
				{
					break;
				}
			}
			return text;
		}

		public  static IEnumerator Open(Data openData, OpenDataProc proc = null)
		{
			if (Singleton<Game>.IsInstance() && Singleton<Game>.Instance.actScene != null && Singleton<Game>.Instance.actScene.AdvScene != null)
			{
				CommandData orAddComponent = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<CommandData>();
				orAddComponent.data = openData;
				Singleton<Game>.Instance.actScene.AdvScene.gameObject.SetActive(true);
				if (proc != null)
				{
					proc.onLoad.Call();
				}
				yield break;
			}
			OpenADVData open = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenADVData>();
			openData.bundleName = string.Empty;
			openData.assetName = string.Empty;
			open.data = openData;
			open.isLoad = false;
			open.isAsync = false;
			open.isFade = false;
			if (proc != null)
			{
				open.onLoad = proc.onLoad;
				open.onFadeIn = proc.onFadeIn;
				open.onFadeOut = proc.onFadeOut;
				open.fadeType = proc.fadeType;
			}
			yield return new WaitWhile(() => Singleton<Scene>.Instance.IsNowLoadingFade);
			yield return new WaitWhile(() => open != null);
            yield return new WaitWhile(() => Program.isADVScene);

        }

        public  static IEnumerator ADVProcessingCheck()
		{
			if (isADVActionActive)
			{
                yield return new WaitWhile(() => Program.isADVScene);
            }
			else
			{
                yield return new WaitWhile(() => Program.isADVScene);
            }
		}

		public static IEnumerator Wait(string addSceneName)
		{
			if (Singleton<Scene>.IsInstance())
			{
				GameObject commonSpace = Singleton<Scene>.Instance.commonSpace;
				yield return new WaitWhile(() => commonSpace.GetComponent<OpenADVData>() != null);
				yield return new WaitWhile(() => Singleton<Scene>.Instance.AddSceneName != addSceneName);
				yield return ADVProcessingCheck();
			}
		}

		public static void SetADVCharacter(int no, SaveData.CharaData charaData, TextScenario scenario, CharaData.MotionReserver motionReserver = null, bool hiPoly = true)
		{
			scenario.commandController.AddChara(no, charaData, charaData.chaCtrl, motionReserver, hiPoly);
		}
	}
}
