using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : BaseLoader
{
	[Serializable]
	private class PageInfo
	{
		public Button btnNext;

		public Button btnPrev;

		public TextMeshProUGUI textNowPage;

		public TextMeshProUGUI textPage;

		public GameObject objColone;

		public void SetVisible(bool _visible)
		{
			btnNext.gameObject.SetActive(_visible);
			btnPrev.gameObject.SetActive(_visible);
			textNowPage.gameObject.SetActive(_visible);
			textPage.gameObject.SetActive(_visible);
			objColone.SetActive(_visible);
		}
	}

	private const int WAIT_NO = -1;

	[SerializeField]
	private BoolReactiveProperty _isAll = new BoolReactiveProperty(false);

	[SerializeField]
	private IntReactiveProperty _nowTutorial = new IntReactiveProperty(-1);

	[SerializeField]
	private List<SpriteChangeCtrl> imageMain = new List<SpriteChangeCtrl>();

	[SerializeField]
	private PageInfo pageInfo = new PageInfo();

	[SerializeField]
	private RectTransform kindRoot;

	[SerializeField]
	[Header("add20表示用")]
	private Toggle _add20Toggle;

	private IntReactiveProperty _page;

	private int pageMax;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	public int nowTutorial
	{
		get
		{
			return _nowTutorial.Value;
		}
		set
		{
			_nowTutorial.Value = value;
		}
	}

	public bool isAll
	{
		get
		{
			return _isAll.Value;
		}
		set
		{
			_isAll.Value = value;
		}
	}

	private int page
	{
		get
		{
			return _page.Value;
		}
		set
		{
			_page.Value = Mathf.Clamp(value, 0, pageMax - 1);
			if (value == pageMax)
			{
				EndScene(false);
			}
		}
	}

	private Dictionary<int, Data.Param> translateQuestionTitle
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
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.TUTORIAL));
		}
	}

	public void EndScene()
	{
		EndScene(_isAll.Value);
	}

	public void EndScene(bool isSkip)
	{
		if (!(Singleton<Scene>.Instance.NowSceneNames[0] != "Tutorial"))
		{
			if (isSkip)
			{
				Singleton<Scene>.Instance.UnLoad();
			}
			else
			{
				Observable.FromCoroutine(_EndScene).Subscribe();
			}
		}
	}

	private IEnumerator Start()
	{
		if (_add20Toggle != null)
		{
			_add20Toggle.gameObject.SetActiveIfDifferent(Game.isAdd20);
		}
		kindRoot.gameObject.SetActiveIfDifferent(false);
		yield return new WaitWhile(() => _nowTutorial.Value == -1);
		Canvas canvas = GetComponent<Canvas>();
		IEnumerable<Canvas> sorter = from p in UnityEngine.Object.FindObjectsOfType<Canvas>()
			where p != canvas
			select p;
		if (sorter.Any())
		{
			canvas.sortingOrder = sorter.Max((Canvas p) => p.sortingOrder) + 1;
		}
		if (kindRoot != null)
		{
			_isAll.Subscribe(delegate(bool isOn)
			{
				kindRoot.gameObject.SetActiveIfDifferent(isOn);
			});
			Toggle[] array = (from p in kindRoot.Children()
				select p.GetComponent<Toggle>()).ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				array[j].isOn = j == _nowTutorial.Value;
			}
			(from list in array.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).Subscribe(delegate(int i)
			{
				_nowTutorial.Value = i;
			});
			_nowTutorial.Skip(1).Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		}
		pageInfo.btnNext.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
			page++;
		});
		pageInfo.btnPrev.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
			page--;
		});
		_nowTutorial.Subscribe(delegate(int no)
		{
			SpriteChangeCtrl sp = null;
			for (int k = 0; k < imageMain.Count; k++)
			{
				bool flag = k == no;
				imageMain[k].gameObject.SetActive(flag);
				if (flag)
				{
					sp = imageMain[k];
				}
			}
			if (sp == null)
			{
				pageMax = 0;
				pageInfo.SetVisible(false);
				TextMeshProUGUI textPage = pageInfo.textPage;
				string text = pageMax.ToString();
				pageInfo.textNowPage.text = text;
				textPage.text = text;
				_page = null;
			}
			else
			{
				if (Singleton<Game>.IsInstance())
				{
					Singleton<Game>.Instance.glSaveData.tutorialHash.Add(no);
					Singleton<Game>.Instance.tutorialData.data.Add(no);
				}
				pageMax = sp.GetCount();
				pageInfo.SetVisible(pageMax > 1);
				pageInfo.textPage.text = pageMax.ToString();
				_page = new IntReactiveProperty(0);
				_page.Subscribe(delegate(int p)
				{
					sp.OnChangeValue(p);
				});
				_page.SubscribeWithState(pageInfo.textNowPage, delegate(int i, TextMeshProUGUI t)
				{
					t.text = (i + 1).ToString();
				});
				_page.Select((int i) => i > 0).SubscribeToInteractable(pageInfo.btnPrev);
			}
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			select _).Subscribe(delegate
		{
			EndScene();
		});
	}

	private IEnumerator _EndScene()
	{
		CheckScene.Parameter param = new CheckScene.Parameter();
		param.Yes = delegate
		{
			Singleton<Scene>.Instance.UnLoad();
			Singleton<Scene>.Instance.UnLoad();
		};
		param.No = delegate
		{
			Singleton<Scene>.Instance.UnLoad();
		};
		param.Title = translateQuestionTitle.SafeGetText(0) ?? "チュートリアルを終了しますか？";
		yield return Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).StartAsCoroutine();
	}
}
