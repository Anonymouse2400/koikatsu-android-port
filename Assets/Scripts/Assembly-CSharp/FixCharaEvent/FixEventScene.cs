using System;
using System.Collections;
using System.Collections.Generic;
using ADV;
using ActionGame;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FixCharaEvent
{
	public class FixEventScene : BaseLoader
	{
		private class ADVParam : SceneParameter
		{
			public bool isInVisibleChara = true;

			public ADVParam(MonoBehaviour scene)
				: base(scene)
			{
			}

			public override void Init(Data data)
			{
				ADVScene aDVScene = SceneParameter.advScene;
				TextScenario scenario = aDVScene.Scenario;
				scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
				scenario.LoadBundleName = data.bundleName;
				scenario.LoadAssetName = data.assetName;
				aDVScene.Stand.SetPositionAndRotation(data.position, data.rotation);
				scenario.heroineList = data.heroineList;
				scenario.transferList = data.transferList;
				if (!data.heroineList.IsNullOrEmpty())
				{
					scenario.currentChara = new CharaData(data.heroineList[0], scenario, null);
				}
				if (data.camera != null)
				{
					scenario.BackCamera.transform.SetPositionAndRotation(data.camera.position, data.camera.rotation);
				}
				float fadeInTime = data.fadeInTime;
				if (fadeInTime > 0f)
				{
					aDVScene.fadeTime = fadeInTime;
				}
				else
				{
					isInVisibleChara = false;
				}
			}

			public override void Release()
			{
				ADVScene aDVScene = SceneParameter.advScene;
				aDVScene.gameObject.SetActive(false);
			}

			public override void WaitEndProc()
			{
			}
		}

		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		private RectTransform characterView;

		[SerializeField]
		private Button enterButton;

		[SerializeField]
		private Button returnButton;

		protected override void Awake()
		{
			base.Awake();
			ParameterList.Add(new ADVParam(this));
		}

		private void OnDestroy()
		{
			ParameterList.Remove(this);
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			ReactiveProperty<Tuple<int, EventInfo.Param>> eventParam = new ReactiveProperty<Tuple<int, EventInfo.Param>>();
			Dictionary<int, List<EventInfo.Param>> evInfos = new Dictionary<int, List<EventInfo.Param>>();
			CommonLib.GetAssetBundleNameListFromPath("action/list/event/", true).ForEach(delegate(string file)
			{
				EventInfo[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(EventInfo)).GetAllAssets<EventInfo>();
				foreach (EventInfo eventInfo in allAssets)
				{
					int result;
					int.TryParse(eventInfo.name, out result);
					List<EventInfo.Param> value;
					if (!evInfos.TryGetValue(result, out value))
					{
						value = (evInfos[result] = new List<EventInfo.Param>());
					}
					value.AddRange(eventInfo.param);
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			ReactiveProperty<Button> enterEventColor = new ReactiveProperty<Button>();
			Color enterColor = Color.yellow;
			enterEventColor.Scan(delegate(Button prev, Button current)
			{
				if (prev != null)
				{
					prev.colors = current.colors;
				}
				ColorBlock colors = current.colors;
				colors.normalColor = enterColor;
				colors.highlightedColor = enterColor;
				current.colors = colors;
				return current;
			}).Subscribe();
			List<SaveData.Heroine> list2 = Game.CreateFixCharaList();
			foreach (KeyValuePair<int, List<EventInfo.Param>> item3 in evInfos)
			{
				int key = item3.Key;
				if (key >= 0)
				{
					continue;
				}
				SaveData.Heroine heroine = list2.Find((SaveData.Heroine p) => p.fixCharaID == key);
				if (heroine == null)
				{
					continue;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(characterView.gameObject, characterView.parent, false);
				gameObject.SetActive(true);
				ImageButton component = gameObject.GetComponent<ImageButton>();
				RawImage faceImage = component.faceImage;
				Rect rect = faceImage.rectTransform.rect;
				Texture2D texture2D = new Texture2D((int)rect.width, (int)rect.height);
				texture2D.LoadImage(heroine.charFile.facePngData);
				faceImage.texture = texture2D;
				faceImage.enabled = true;
				foreach (EventInfo.Param param2 in item3.Value)
				{
					Button button = UnityEngine.Object.Instantiate(component.button, component.button.transform.parent, false);
					button.gameObject.SetActive(true);
					button.GetComponentInChildren<Text>().text = param2.Name;
					(from _ in button.OnPointerClickAsObservable()
						select param2).Subscribe(delegate(EventInfo.Param p)
					{
						enterEventColor.Value = button;
						eventParam.Value = Tuple.Create(key, p);
					});
				}
			}
			eventParam.Select((Tuple<int, EventInfo.Param> param) => param.Item2 != null).SubscribeToInteractable(enterButton);
			enterButton.OnClickAsObservable().Subscribe(delegate
			{
				Observable.FromCoroutine((CancellationToken __) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In)).Subscribe(delegate
				{
					EventSystem.current.SetSelectedGameObject(null);
					SaveData.Heroine heroine2 = new SaveData.Heroine(true);
					TextAsset textAsset = null;
					int item = eventParam.Value.Item1;
					EventInfo.Param item2 = eventParam.Value.Item2;
					foreach (string item4 in CommonLib.GetAssetBundleNameListFromPath("action/fixchara/", true))
					{
						TextAsset[] allAssets2 = AssetBundleManager.LoadAllAsset(item4, typeof(TextAsset)).GetAllAssets<TextAsset>();
						foreach (TextAsset textAsset2 in allAssets2)
						{
							int result2;
							int.TryParse(textAsset2.name.Replace("c", string.Empty), out result2);
							if (item == result2)
							{
								textAsset = textAsset2;
								break;
							}
						}
						AssetBundleManager.UnloadAssetBundle(item4, false);
						if (textAsset != null)
						{
							break;
						}
					}
					Game.LoadFromTextAsset(item, heroine2, textAsset);
					List<Program.Transfer> list3 = Program.Transfer.NewList();
					Program.SetParam(heroine2, list3);
					SaveData.Player player = Singleton<Game>.Instance.Player;
					Game.LoadFromTextAsset(player);
					Program.SetParam(player, list3);
					list3.Add(Program.Transfer.Create(true, Command.CameraLock, bool.FalseString));
					list3.Add(Program.Transfer.Open(item2.Bundle, item2.Asset));
					StartCoroutine(Program.Open(new Data
					{
						position = Vector3.zero,
						rotation = Quaternion.identity,
						scene = this,
						transferList = list3,
						heroineList = new List<SaveData.Heroine> { heroine2 }
					}, new Program.OpenDataProc
					{
						onLoad = delegate
						{
							StartCoroutine(Wait());
						}
					}));
				});
			});
			returnButton.OnClickAsObservable().Subscribe(delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			});
			base.enabled = true;
			yield break;
		}

		private  IEnumerator Wait()
		{
			canvas.enabled = false;
			yield return null;
            yield return new WaitWhile(() => Program.isADVScene);
            yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			canvas.enabled = true;
			Singleton<Manager.Sound>.Instance.StopBGM(0.8f);
		}
	}
}
