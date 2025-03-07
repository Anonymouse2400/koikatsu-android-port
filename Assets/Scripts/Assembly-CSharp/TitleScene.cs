using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
	[Serializable]
	public class ButtonGroup
	{
		public int group;

		public Button button;
	}

	public class ButtonStack<T> : Stack<T> where T : List<Button>
	{
		public new void Push(T item)
		{
			if (base.Count > 0)
			{
				T val = Peek();
				val.ForEach(delegate(Button b)
				{
					b.gameObject.SetActive(false);
				});
			}
			item.ForEach(delegate(Button b)
			{
				b.gameObject.SetActive(true);
			});
			base.Push(item);
		}

		public new T Pop()
		{
			T result = base.Pop();
			result.ForEach(delegate(Button b)
			{
				b.gameObject.SetActive(false);
			});
			if (base.Count > 0)
			{
				T val = Peek();
				val.ForEach(delegate(Button b)
				{
					b.gameObject.SetActive(true);
				});
			}
			return result;
		}
	}

	[SerializeField]
	private GraphicRaycaster graphicRaycaster;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private ButtonGroup[] buttons;

	[SerializeField]
	private Button btnRanking;

	[SerializeField]
	private TextMeshProUGUI version;

	[SerializeField]
	private Image titleLogo;

	private Dictionary<int, List<Button>> buttonDic;

	private ButtonStack<List<Button>> buttonStack = new ButtonStack<List<Button>>();

	private bool isCustomMale;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	[CompilerGenerated]
	private static Dictionary<string, int> _003C_003Ef__switch_0024map4;

	private Dictionary<int, Data.Param> translater
	{
		get
		{
			return uiTranslater.Get(0);
		}
	}

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.TITLE));
		}
	}

	public void OnStart()
	{
		Enter("EntryPlayer");
	}

	public void OnLoad()
	{
		Enter("Load");
	}

	public void OnCustom()
	{
		buttonStack.Push(buttonDic[1]);
		Enter(string.Empty);
	}

	public void OnCustomMale()
	{
		isCustomMale = true;
		Enter("CustomScene");
	}

	public void OnCustomFemale()
	{
		isCustomMale = false;
		Enter("CustomScene");
	}

	public void OnUploader()
	{
		Enter("Uploader");
	}

	public void OnDownloader()
	{
		Enter("Downloader");
	}

	public void OnBack()
	{
		buttonStack.Pop();
		Enter(string.Empty);
	}

	public void OnOther()
	{
		buttonStack.Push(buttonDic[2]);
		Button button = buttonStack.Peek().FirstOrDefault((Button p) => p.name.CompareParts("wedding", true));
		if (button != null)
		{
			bool active = Game.isAdd20 && Singleton<Voice>.Instance.voiceInfoList.Select((VoiceInfo.Param p) => p.No).Intersect(Singleton<Game>.Instance.weddingData.personality).Any();
			button.gameObject.SetActiveIfDifferent(active);
		}
		Enter(string.Empty);
	}

	public void OnOtherFreeH()
	{
		Enter("FreeH");
	}

	public void OnOtherIdolLive()
	{
		Enter("LiveStage");
	}

	public void OnOtherEvent()
	{
		Enter("FixEventSceneEx");
	}

	public void OnConfig()
	{
		Enter("Config");
	}

	public void OnEnd()
	{
		Enter("Exit");
	}

	public void OnWedding()
	{
		Enter("Wedding");
	}

	private void ButtonInteractable(string name, Func<bool> func)
	{
		ButtonGroup[] array = buttons;
		foreach (ButtonGroup buttonGroup in array)
		{
			if (buttonGroup.button.name.IndexOf(name) != -1)
			{
				if (!func.IsNullOrEmpty())
				{
					buttonGroup.button.interactable = func();
				}
				break;
			}
		}
	}

	private void Enter(string next)
	{
		if (!graphicRaycaster.enabled)
		{
			return;
		}
		if (next != null && (next == string.Empty || next == "Load" || next == "Config"))
		{
			Utils.Sound.Play(SystemSE.sel);
		}
		else
		{
			Utils.Sound.Play(SystemSE.ok_s);
		}
		if (next == null)
		{
			return;
		}
		if (_003C_003Ef__switch_0024map4 == null)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
			dictionary.Add("EntryPlayer", 0);
			dictionary.Add("Load", 1);
			dictionary.Add("CustomScene", 2);
			dictionary.Add("Uploader", 3);
			dictionary.Add("Downloader", 4);
			dictionary.Add("FixEventSceneEx", 5);
			dictionary.Add("Config", 6);
			dictionary.Add("FreeH", 7);
			dictionary.Add("LiveStage", 8);
			dictionary.Add("Exit", 9);
			dictionary.Add("Wedding", 10);
			_003C_003Ef__switch_0024map4 = dictionary;
		}
		int value;
		if (!_003C_003Ef__switch_0024map4.TryGetValue(next, out value))
		{
			return;
		}
		switch (value)
		{
		case 0:
		{
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAsync = true;
			data.isFade = true;
			Scene.Data data11 = data;
			Singleton<Scene>.Instance.LoadReserve(data11, true);
			break;
		}
		case 1:
		{
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAdd = true;
			data.onLoad = delegate
			{
				LoadScene rootComponent3 = Scene.GetRootComponent<LoadScene>(next);
				if (!(rootComponent3 == null))
				{
					rootComponent3.onEnter += delegate(string fileName)
					{
						Game.SaveFileName = fileName;
						Singleton<Game>.Instance.Load();
						Singleton<Scene>.Instance.LoadReserve(new Scene.Data
						{
							levelName = "Action",
							fadeType = Scene.Data.FadeType.In
						}, true);
					};
				}
			};
			Scene.Data data9 = data;
			Singleton<Scene>.Instance.LoadReserve(data9, false);
			break;
		}
		case 2:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAdd = false;
			data.isFade = true;
			data.isAsync = true;
			data.onLoad = delegate
			{
				CustomScene rootComponent2 = Scene.GetRootComponent<CustomScene>(next);
				if (!(rootComponent2 == null))
				{
					rootComponent2.modeNew = true;
					rootComponent2.modeSex = ((!isCustomMale) ? ((byte)1) : ((byte)0));
					rootComponent2.chaFileCtrl = null;
				}
			};
			Scene.Data data8 = data;
			Singleton<Scene>.Instance.LoadReserve(data8, true);
			break;
		}
		case 3:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.levelName = "NetworkCheckScene";
			data.isAdd = false;
			data.isFade = true;
			data.isAsync = true;
			data.onLoad = delegate
			{
				NetworkCheckScene rootComponent4 = Scene.GetRootComponent<NetworkCheckScene>("NetworkCheckScene");
				if (!(rootComponent4 == null))
				{
					rootComponent4.nextSceneName = "Uploader";
				}
			};
			Scene.Data data10 = data;
			Singleton<Scene>.Instance.LoadReserve(data10, true);
			break;
		}
		case 4:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.levelName = "NetworkCheckScene";
			data.isAdd = false;
			data.isFade = true;
			data.isAsync = true;
			data.onLoad = delegate
			{
				NetworkCheckScene rootComponent = Scene.GetRootComponent<NetworkCheckScene>("NetworkCheckScene");
				if (!(rootComponent == null))
				{
					rootComponent.nextSceneName = "Downloader";
				}
			};
			Scene.Data data7 = data;
			Singleton<Scene>.Instance.LoadReserve(data7, true);
			break;
		}
		case 5:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAdd = false;
			data.isFade = true;
			data.isAsync = true;
			Scene.Data data6 = data;
			Singleton<Scene>.Instance.LoadReserve(data6, true);
			break;
		}
		case 6:
		{
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAdd = true;
			Scene.Data data5 = data;
			Singleton<Scene>.Instance.LoadReserve(data5, true);
			break;
		}
		case 7:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAdd = false;
			data.isFade = true;
			data.isAsync = true;
			Scene.Data data4 = data;
			Singleton<Scene>.Instance.LoadReserve(data4, true);
			break;
		}
		case 8:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.levelName = next;
			data.isAdd = false;
			data.isFade = true;
			data.isAsync = true;
			Scene.Data data3 = data;
			Singleton<Scene>.Instance.LoadReserve(data3, true);
			break;
		}
		case 9:
			Singleton<Scene>.Instance.GameEnd();
			break;
		case 10:
		{
			graphicRaycaster.enabled = false;
			Scene.Data data = new Scene.Data();
			data.assetBundleName = "scene/20/wedding/main.unity3d";
			data.levelName = next;
			data.isAdd = false;
			data.fadeType = Scene.Data.FadeType.In;
			data.isAsync = true;
			Scene.Data data2 = data;
			Singleton<Scene>.Instance.LoadReserve(data2, true);
			break;
		}
		}
	}

	private  IEnumerator Start()
	{
		base.enabled = false;
		if (Localize.Translate.Manager.isTranslate)
		{
			version.text = string.Empty;
		}
		else
		{
			version.text = "Ver " + Game.Version;
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		if (Localize.Translate.Manager.isTranslate)
		{
			Data.Param param = translater.Values.FindTag("Logo");
			if (param != null)
			{
				Localize.Translate.Manager.Bind(titleLogo, param, true);
			}
		}
		if (Game.isAdd20)
		{
			string assetBundleName = "sprite/20/title.unity3d";
			if (AssetBundleCheck.IsFile(assetBundleName, string.Empty) && titleLogo != null)
			{
				Utils.Bundle.LoadSprite(assetBundleName, "title_logo_after", titleLogo, true);
			}
		}
		buttonDic = buttons.ToLookup((ButtonGroup p) => p.group, (ButtonGroup p) => p.button).ToDictionary((IGrouping<int, Button> p) => p.Key, Enumerable.ToList);
		foreach (List<Button> value in buttonDic.Values)
		{
			value.ForEach(delegate(Button p)
			{
				p.gameObject.SetActive(false);
			});
		}
		buttonStack.Push(buttonDic[0]);
		this.ObserveEveryValueChanged((TitleScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
		{
			canvasGroup.interactable = isOn;
		});
		Singleton<Game>.Instance.NewGame();
		Singleton<Scene>.Instance.UnloadBaseScene();
		Singleton<Scene>.Instance.SetFadeColorDefault();
		Singleton<Scene>.Instance.UnloadAddScene();
		if (Singleton<Character>.IsInstance())
		{
			Singleton<Character>.Instance.EndLoadAssetBundle();
		}
		string[] array = AssetBundleManager.AllLoadedAssetBundleNames.Where((string s) => s.StartsWith("sound/data/pcm/")).ToArray();
		foreach (string assetBundleName2 in array)
		{
			AssetBundleManager.UnloadAssetBundle(assetBundleName2, true);
		}
		if (Scene.isReturnTitle && Singleton<Scene>.Instance.sceneFade._Fade == SimpleFade.Fade.In)
		{
			yield return new WaitUntil(() => Singleton<Scene>.Instance.sceneFade.IsEnd);
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
		}
		Scene.isReturnTitle = false;
        this.ButtonInteractable("Load", () => Game.IsLoadCheck);

        ButtonInteractable("Other", () => true);
		ButtonInteractable("FixEventSceneEx", () => Singleton<Game>.Instance.glSaveData.fixCharaTaked.Any());
		if (btnRanking != null)
		{
			bool rankEntry = Singleton<Game>.Instance.rankSaveData.userName.IsNullOrEmpty();
			TextMeshProUGUI componentInChildren = btnRanking.GetComponentInChildren<TextMeshProUGUI>();
			if (!Localize.Translate.Manager.isTranslate)
			{
				componentInChildren.text = ((!rankEntry) ? "ランキング" : "ランキングに参加");
			}
			else
			{
				componentInChildren.text = translater.Values.ToArray("Rankings").SafeGet((!rankEntry) ? 1 : 0);
			}
			btnRanking.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
				if (rankEntry)
				{
					Scene.Data data = new Scene.Data
					{
						levelName = "RankingEntryScene",
						isAdd = true,
						isFade = true,
						isAsync = true,
						onLoad = delegate
						{
							RankingEntryScene rootComponent = Scene.GetRootComponent<RankingEntryScene>("RankingEntryScene");
							if (!(rootComponent == null))
							{
								rootComponent.backSceneName = "Title";
							}
						}
					};
					Singleton<Scene>.Instance.LoadReserve(data, true);
				}
				else
				{
					Singleton<Scene>.Instance.LoadReserve(new Scene.Data
					{
						levelName = "RankingScene",
						isFade = true
					}, false);
				}
			});
		}
		this.ObserveEveryValueChanged((TitleScene _) => Singleton<Scene>.Instance.AddSceneName).Subscribe(delegate(string sceneName)
		{
			graphicRaycaster.enabled = sceneName == string.Empty;
		});
		List<AudioClip> clipList = new List<AudioClip>();
		AssetBundleData.GetAssetBundleNameListFromPath("sound/data/systemse/titlecall/", true).ForEach(delegate(string file)
		{
			clipList.AddRange(AssetBundleManager.LoadAllAsset(file, typeof(AudioClip)).GetAllAssets<AudioClip>());
			AssetBundleManager.UnloadAssetBundle(file, true);
		});
		clipList.RemoveAll((AudioClip p) => p == null);
		AudioClip clip = clipList.Shuffle().FirstOrDefault();
		(from _ in (from _ in this.UpdateAsObservable()
				where !Singleton<Scene>.Instance.IsFadeNow
				select _).Take(1)
			where clip != null
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(Manager.Sound.Type.SystemSE, clip).OnDestroyAsObservable().Subscribe(delegate
			{
				clipList.ForEach(Resources.UnloadAsset);
				clipList.Clear();
				clip = null;
			});
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where buttonStack.Count > 1
			select _).Subscribe(delegate
		{
			buttonStack.Pop();
		});
		base.enabled = true;
	}
}
