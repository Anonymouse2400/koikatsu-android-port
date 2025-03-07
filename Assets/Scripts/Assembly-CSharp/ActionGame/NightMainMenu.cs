using System;
using System.Collections;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class NightMainMenu : BaseLoader
	{
		private Subject<Unit> _onLoadSubject = new Subject<Unit>();

		[SerializeField]
		private NightMenuScene nightMenuScene;

		[Label("キャンバス")]
		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		[Label("セーブ")]
		private Button saveButton;

		[SerializeField]
		[Label("ロード")]
		private Button loadButton;

		[Label("登録")]
		[SerializeField]
		private Button entryButton;

		[SerializeField]
		[Label("次へ")]
		private Button nextButton;

		[Label("タイトルに戻る")]
		[SerializeField]
		private Button titleButton;

		[Label("週末にする")]
		[SerializeField]
		private Button nextWeekEndButton;

		public Subject<Unit> onLoadSubject
		{
			get
			{
				return _onLoadSubject;
			}
		}

		private void Start()
		{
			CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
			if (canvasGroup != null)
			{
				this.ObserveEveryValueChanged((NightMainMenu _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
				{
					canvasGroup.interactable = isOn;
				});
			}
			saveButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = "Save",
					isAdd = true
				}, false);
			});
			loadButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
				string levelName = "Load";
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = levelName,
					isAdd = true,
					onLoad = delegate
					{
						LoadScene rootComponent = Scene.GetRootComponent<LoadScene>(levelName);
						if (!(rootComponent == null))
						{
							rootComponent.onEnter += delegate(string fileName)
							{
								Game.SaveFileName = fileName;
								Singleton<Game>.Instance.Load();
								Singleton<Scene>.Instance.UnLoad();
								_onLoadSubject.OnNext(Unit.Default);
								Singleton<Scene>.Instance.UnLoad();
							};
						}
					}
				}, false);
			});
			entryButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
				string levelName2 = "ClassRoomSelect";
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = levelName2,
					isAdd = true,
					isAsync = true,
					onLoad = delegate
					{
						ClassRoomSelectScene rootComponent2 = Scene.GetRootComponent<ClassRoomSelectScene>(levelName2);
						if (!(rootComponent2 == null))
						{
							nightMenuScene.gameObject.SetActive(false);
							rootComponent2.classRoomList.OnDestroyAsObservable().TakeUntilDestroy(nightMenuScene).Subscribe(delegate
							{
								if (nightMenuScene != null)
								{
									nightMenuScene.gameObject.SetActive(true);
								}
							});
						}
					}
				}, false);
			});
			nextButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
				Singleton<Scene>.Instance.UnLoad();
			});
			titleButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
				Observable.FromCoroutine((CancellationToken __) => Utils.Scene.ReturnTitle()).StartAsCoroutine();
			});
			if (!(nextWeekEndButton != null))
			{
				return;
			}
			ActionScene actScene = null;
			if (Singleton<Game>.IsInstance())
			{
				actScene = Singleton<Game>.Instance.actScene;
			}
			if (!(actScene != null))
			{
				return;
			}
			(from week in (from _ in this.UpdateAsObservable().TakeUntilDestroy(actScene)
					select actScene.Cycle.nowWeek).DistinctUntilChanged()
				select week < Cycle.Week.Friday || week == Cycle.Week.Holiday).Subscribe(delegate(bool isActive)
			{
				nextWeekEndButton.gameObject.SetActiveIfDifferent(isActive);
			});
			nextWeekEndButton.OnClickAsObservable().TakeUntilDestroy(actScene).Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
				StartCoroutine(Fader(delegate
				{
					actScene.Cycle.Change(Cycle.Week.Friday);
					actScene.Cycle.withHeroine = null;
				}));
			});
		}

		private IEnumerator Fader(Action act)
		{
			SceneFade sceneFade = Singleton<Scene>.Instance.sceneFade;
			sceneFade._Color = Color.black;
			sceneFade.FadeInOutSet(new SimpleFade.FadeInOut(sceneFade));
			yield return new WaitUntil(() => sceneFade._Fade == SimpleFade.Fade.Out);
			act();
			yield return new WaitUntil(() => sceneFade.IsEnd);
			Singleton<Scene>.Instance.SetFadeColorDefault();
		}
	}
}
