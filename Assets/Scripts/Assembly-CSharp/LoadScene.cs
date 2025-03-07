using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Game;
using Localize.Translate;
using Manager;
using SaveLoad;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : BaseLoader
{
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private Button _autoButton;

	[SerializeField]
	private Button _returnButton;

	[SerializeField]
	private string _checkMessage;

	[SerializeField]
	private GameFile gameFile;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	public Action onReturn { get; set; }

	private Dictionary<int, Data.Param> translateQuestionTitle
	{
		get
		{
			return uiTranslater.Get(1);
		}
	}

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.SAVE_LOAD));
		}
	}

	public event Action<string> onEnter = delegate
	{
	};

	private IEnumerator NextScene(string fileName)
	{
		Utils.Sound.Play(SystemSE.sel);
		CheckScene.Parameter param = new CheckScene.Parameter();
		param.Yes = delegate
		{
			this.onEnter(fileName);
		};
		param.No = delegate
		{
			Singleton<Scene>.Instance.UnLoad();
		};
		param.Title = _checkMessage;
		yield return Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).StartAsCoroutine();
	}

	private void Start()
	{
		this.ObserveEveryValueChanged((LoadScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
		{
			_canvasGroup.interactable = isOn;
		});
		translateQuestionTitle.Values.FindTagText("LoadCheck").SafeProc(delegate(string text)
		{
			_checkMessage = text;
		});
		gameFile.Initialize(true);
		gameFile.fileButtons.ToList().ForEach(delegate(GameFile.FileButton fb)
		{
			fb.button.OnClickAsObservable().Subscribe(delegate
			{
				Observable.FromCoroutine((CancellationToken __) => NextScene(fb.file)).StartAsCoroutine();
			});
		});
		if (onReturn.IsNullOrEmpty())
		{
			onReturn = delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			};
		}
		_returnButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
			onReturn();
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where _returnButton.interactable
			where Singleton<Scene>.Instance.NowSceneNames[0] == "Load"
			select _).Take(1).Subscribe(delegate
		{
			onReturn();
		});
	}

	private void OnDestroy()
	{
		gameFile.Dispose();
	}

	[ContextMenu("Setup")]
	private void Setup()
	{
		Transform transform = base.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform t) => t.name == "Files");
		if (!(transform != null))
		{
			return;
		}
		List<GameFile.FileButton> list = new List<GameFile.FileButton>();
		foreach (Transform item in transform)
		{
			list.Add(new GameFile.FileButton(item.GetComponent<Button>(), item.GetChild(0).GetComponent<Text>(), item.GetChild(1).GetComponent<Text>()));
		}
		gameFile.SetupFiles(list.ToArray());
	}
}
