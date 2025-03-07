using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using Config;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class ConfigScene : BaseLoader
{
	[Serializable]
	public class ShortCutGroup
	{
		public string title;

		public string name;

		public int visibleNo;

		[SerializeField]
		private RectTransform[] _trans;

		public RectTransform trans
		{
			get
			{
				return _trans.SafeGet(visibleNo);
			}
		}

		public void VisibleUpdate()
		{
			RectTransform[] array = _trans;
			foreach (RectTransform rectTransform in array)
			{
				rectTransform.gameObject.SetActiveIfDifferent(rectTransform == trans);
			}
		}

		public void Set(RectTransform t)
		{
			visibleNo = -1;
			VisibleUpdate();
			_trans = new RectTransform[1] { t };
			visibleNo = 0;
		}
	}

	[SerializeField]
	private Canvas canvas;

	[Label("コンフィグ項目を写す場所")]
	[SerializeField]
	private RectTransform mainWindow;

	[Label("ショートカットボタンの場所")]
	[SerializeField]
	private RectTransform shortCutButtonBackGround;

	[Label("ショートカットボタンのプレファブ")]
	[SerializeField]
	private Button shortCutButtonPrefab;

	[SerializeField]
	[Tooltip("ショートカットのリンク")]
	private ShortCutGroup[] shortCuts;

	[SerializeField]
	[Tooltip("初めに設定されているボタン")]
	private Button[] buttons;

	private BaseSetting[] settings;

	private bool isEnd;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private bool isConfig
	{
		get
		{
			return Singleton<Scene>.Instance.NowSceneNames[0] == "Config";
		}
	}

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CONFIG));
		}
	}

	public void Unload()
	{
		if (!isConfig || isEnd)
		{
			return;
		}
		isEnd = true;
		Save();
		if (Manager.Config.AddData.AINotPlayerTarget)
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene != null && actScene.npcList != null)
			{
				foreach (AI item in from npc in actScene.npcList
					select npc.AI into p
					where p.ActionNoCheck(5, 8)
					select p)
				{
					item.TimeEnd();
				}
			}
		}
		Singleton<Scene>.Instance.UnLoad();
	}

	private void PlaySE(SystemSE se = SystemSE.sel)
	{
		Utils.Sound.Play(se);
	}

	private IEnumerator Start()
	{
		string[] removeShortCuts = new string[2] { "ADV", "Add" };
		if (Localize.Translate.Manager.isTranslate)
		{
			Dictionary<int, Data.Param> self = uiTranslater.Get(0);
			Dictionary<int, Data.Param> self2 = uiTranslater.Get(8);
			List<Data.Param> list = new List<Data.Param>();
			RectTransform trans = shortCuts.SafeGet(0).trans;
			for (int j = 0; j < shortCuts.Length; j++)
			{
				ShortCutGroup shortCut = shortCuts[j];
				if (!removeShortCuts.Contains(shortCut.name))
				{
					self.SafeGetText(j).SafeProc(delegate(string text)
					{
						shortCut.title = text;
					});
					Data.Param param = self2.Get(j);
					GameObject gameObject = param.Load(false) as GameObject;
					if (!(gameObject == null))
					{
						RectTransform component = UnityEngine.Object.Instantiate(gameObject, mainWindow, false).GetComponent<RectTransform>();
						component.name = gameObject.name;
						shortCut.Set(component);
						list.Add(param);
					}
				}
			}
			Localize.Translate.Manager.Unload(list);
			GraphicSetting graphicSetting = null;
			if (trans != null)
			{
				graphicSetting = trans.GetComponent<GraphicSetting>();
			}
			if (graphicSetting != null && trans != shortCuts.SafeGet(0).trans)
			{
				GraphicSetting component2 = shortCuts.SafeGet(0).trans.GetComponent<GraphicSetting>();
				if (component2 != null)
				{
					RawImage rampRawImage = graphicSetting.rampRawImage;
					RawImage rampRawImage2 = component2.rampRawImage;
					if (rampRawImage != null && rampRawImage2 != null)
					{
						rampRawImage2.texture = rampRawImage.texture;
						rampRawImage2.material = rampRawImage.material;
					}
				}
			}
		}
		else
		{
			ShortCutGroup shortCutGroup = shortCuts.FirstOrDefault((ShortCutGroup p) => p.name == "Add");
			if (shortCutGroup != null && Game.isAdd20)
			{
				shortCutGroup.visibleNo = 1;
			}
		}
		shortCuts = shortCuts.Where((ShortCutGroup p) => !removeShortCuts.Contains(p.name)).ToArray();
		ShortCutGroup[] array = shortCuts;
		foreach (ShortCutGroup shortCutGroup2 in array)
		{
			shortCutGroup2.VisibleUpdate();
		}
		Localize.Translate.Manager.BindFont(shortCutButtonPrefab.GetComponentInChildren<TextMeshProUGUI>(true));
		var shortCutList = shortCuts.Select(delegate(ShortCutGroup p, int i)
		{
			Button button = UnityEngine.Object.Instantiate(shortCutButtonPrefab, shortCutButtonBackGround, false);
			TextMeshProUGUI componentInChildren = button.GetComponentInChildren<TextMeshProUGUI>(true);
			componentInChildren.text = p.title;
			RectTransform rectTransform = button.transform as RectTransform;
			if (rectTransform != null)
			{
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				anchoredPosition.x += rectTransform.sizeDelta.x * (float)i;
				rectTransform.anchoredPosition = anchoredPosition;
			}
			return new
			{
				bt = button,
				name = p.name
			};
		}).ToList();
		settings = shortCuts.Select((ShortCutGroup p) => p.trans.GetComponent<BaseSetting>()).ToArray();
		while (!Manager.Config.initialized)
		{
			yield return null;
		}
		yield return null;
		BaseSetting[] array2 = settings;
		foreach (BaseSetting baseSetting in array2)
		{
			baseSetting.Init();
			baseSetting.UIPresenter();
		}
		mainWindow.GetComponent<Canvas>().SafeProcObject(delegate(Canvas canvas)
		{
			canvas.enabled = true;
			UnityEngine.Object.Destroy(canvas);
		});
		float spacing = mainWindow.GetComponent<VerticalLayoutGroup>().spacing;
		shortCutList.ForEach(sc =>
		{
			sc.bt.OnClickAsObservable().Subscribe(delegate
			{
				ShortCutGroup[] array3 = shortCuts.TakeWhile((ShortCutGroup p) => p.name != sc.name).ToArray();
				float y = Mathf.Min(array3.Sum((ShortCutGroup p) => p.trans.sizeDelta.y) + spacing * (float)array3.Length, mainWindow.rect.height - (mainWindow.parent as RectTransform).rect.height);
				Vector2 anchoredPosition2 = mainWindow.anchoredPosition;
				anchoredPosition2.y = y;
				mainWindow.anchoredPosition = anchoredPosition2;
			});
		});
		shortCutList.ForEach(sc =>
		{
			sc.bt.OnClickAsObservable().Subscribe(delegate
			{
				PlaySE();
			});
		});
		buttons.ToList().ForEach(delegate(Button bt)
		{
			this.ObserveEveryValueChanged((ConfigScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).SubscribeToInteractable(bt);
		});
		Button backButton = buttons.FirstOrDefault((Button p) => p.name == "Back");
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where isConfig
			where !Singleton<Scene>.Instance.IsOverlap && backButton.interactable
			select _).Take(1).Subscribe(delegate
		{
			Unload();
		});
		if (Singleton<GameCursor>.IsInstance())
		{
			Singleton<GameCursor>.Instance.SetCursorLock(false);
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
		}
		Canvas[] sorter = (from p in UnityEngine.Object.FindObjectsOfType<Canvas>()
			where p != canvas
			select p).ToArray();
		if (sorter.Any())
		{
			canvas.sortingOrder = sorter.Max((Canvas p) => p.sortingOrder) + 1;
		}
	}

	public void OnDefault()
	{
		PlaySE();
		CheckScene.Parameter param = new CheckScene.Parameter();
		UnityAction yes = delegate
		{
			if (Singleton<KeyInput>.IsInstance())
			{
				Singleton<KeyInput>.Instance.Reset();
			}
			Singleton<Voice>.Instance.Reset();
			Singleton<Manager.Config>.Instance.Reset();
			BaseSetting[] array = settings;
			foreach (BaseSetting baseSetting in array)
			{
				baseSetting.UIPresenter();
			}
			Singleton<Scene>.Instance.UnLoad();
		};
		param.Yes = yes;
		param.No = delegate
		{
			Singleton<Scene>.Instance.UnLoad();
		};
		param.Title = Localize.Translate.Manager.OtherData.Get(100).Values.FindTagText("RestoreDefault") ?? "設定を初期化しますか？";
		Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).TakeUntilDestroy(this).Subscribe();
	}

	public void OnTitle()
	{
		Save();
		Observable.FromCoroutine((CancellationToken _) => Utils.Scene.ReturnTitle()).TakeUntilDestroy(this).Subscribe();
		PlaySE();
	}

	public void OnGameEnd()
	{
		Utils.Scene.GameEnd();
		PlaySE();
	}

	public void OnBack()
	{
		PlaySE();
		Unload();
	}

	private void Save()
	{
		if (Singleton<KeyInput>.IsInstance())
		{
			Singleton<KeyInput>.Instance.Save();
		}
		Singleton<Voice>.Instance.Save();
		Singleton<Manager.Config>.Instance.Save();
	}
}
