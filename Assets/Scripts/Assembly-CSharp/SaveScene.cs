using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion.Game;
using Localize.Translate;
using Manager;
using SaveLoad;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveScene : BaseLoader
{
	[SerializeField]
	private float resultTime = 3f;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private Button _returnButton;

	[SerializeField]
	private string _saveMessage;

	[SerializeField]
	private string _saveMessageOverride;

	[SerializeField]
	private string _checkMessageSaved;

	[SerializeField]
	private GameFile gameFile;

	private IDisposable disposable;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

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

	private IEnumerator Save(UnityAction yesAction, bool isOverride)
	{
		Utils.Sound.Play(SystemSE.sel);
		CheckScene.Parameter param = new CheckScene.Parameter();
		yesAction = (UnityAction)Delegate.Combine(yesAction, (UnityAction)delegate
		{
			UnLoad();
		});
		yesAction = (UnityAction)Delegate.Combine(yesAction, (UnityAction)delegate
		{
			Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(new CheckScene.Parameter
			{
				Title = _checkMessageSaved,
				Init = delegate
				{
					disposable = (from _ in this.UpdateAsObservable()
						where Input.anyKeyDown
						select _).Timeout(TimeSpan.FromSeconds(resultTime)).Catch((TimeoutException ex) => Observable.Empty<Unit>()).Take(1)
						.Subscribe((Action<Unit>)delegate
						{
						}, (Action)delegate
						{
							UnLoad();
						});
				},
				End = delegate
				{
					if (disposable != null)
					{
						disposable.Dispose();
						disposable = null;
					}
				}
			}, observer)).StartAsCoroutine();
		});
		param.Yes = yesAction;
		param.No = delegate
		{
			UnLoad();
		};
		param.Title = ((!isOverride) ? _saveMessage : _saveMessageOverride);
		yield return Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).StartAsCoroutine();
	}

	private void UnLoad()
	{
		Singleton<Scene>.Instance.UnLoad();
	}

	private void Start()
	{
		this.ObserveEveryValueChanged((SaveScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
		{
			_canvasGroup.interactable = isOn;
		});
		Dictionary<int, Data.Param>.ValueCollection values = translateQuestionTitle.Values;
		values.FindTagText("SaveCheck").SafeProc(delegate(string text)
		{
			_saveMessage = text;
		});
		values.FindTagText("SaveCheckOverride").SafeProc(delegate(string text)
		{
			_saveMessageOverride = text;
		});
		values.FindTagText("Saved").SafeProc(delegate(string text)
		{
			_checkMessageSaved = text;
		});
		gameFile.Initialize(false);
		gameFile.fileButtons.ToList().ForEach(delegate(GameFile.FileButton fb)
		{
			fb.button.OnClickAsObservable().Subscribe(delegate
			{
				UnityAction yesAction = delegate
				{
					Singleton<Game>.Instance.Save(fb.file);
					fb.SetName();
				};
				bool isOverride = Game.SaveFiles.Any((string file) => Path.GetFileName(file) == fb.file);
				Observable.FromCoroutine((CancellationToken c) => Save(yesAction, isOverride)).StartAsCoroutine();
			});
		});
		GraphicRaycaster graphicRaycaster = _canvasGroup.GetComponent<GraphicRaycaster>();
		if (graphicRaycaster != null)
		{
			this.ObserveEveryValueChanged((SaveScene _) => Singleton<Scene>.Instance.NowSceneNames[0] == "Save").Subscribe(delegate(bool isOn)
			{
				graphicRaycaster.enabled = isOn;
			});
		}
		Action returnProc = delegate
		{
			UnLoad();
		};
		_returnButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
			returnProc();
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where _returnButton.interactable
			where Singleton<Scene>.Instance.NowSceneNames[0] == "Save"
			select _).Take(1).Subscribe(delegate
		{
			returnProc();
		});
	}

	private void OnDestroy()
	{
		if (disposable != null)
		{
			disposable.Dispose();
			disposable = null;
		}
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
