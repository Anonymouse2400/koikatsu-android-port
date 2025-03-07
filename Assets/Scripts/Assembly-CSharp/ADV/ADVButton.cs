using System;
using System.Linq;
using Illusion.Component.UI;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADV
{
	public class ADVButton : MonoBehaviour
	{
		[SerializeField]
		private MainScenario scenario;

		[SerializeField]
		private Toggle skip;

		[SerializeField]
		private Toggle auto;

		[SerializeField]
		private Button backLog;

		[SerializeField]
		private Button voice;

		[SerializeField]
		private Button config;

		[SerializeField]
		private Button close;

		[SerializeField]
		private CanvasGroup canvasGroup;

		[SerializeField]
		private BoolReactiveProperty isVisible = new BoolReactiveProperty();

		private Selectable[] sels;

		private SelectUI[] selUIs;

		public bool isSelect
		{
			get
			{
				return selUIs != null && selUIs.Any((SelectUI p) => p.isSelect);
			}
		}

		public bool isFocus
		{
			get
			{
				return selUIs != null && selUIs.Any((SelectUI p) => p.isFocus);
			}
		}

		public Toggle Skip
		{
			get
			{
				return skip;
			}
		}

		public Toggle Auto
		{
			get
			{
				return auto;
			}
		}

		public Button BackLog
		{
			get
			{
				return backLog;
			}
		}

		public Button Voice
		{
			get
			{
				return voice;
			}
		}

		public Button Config
		{
			get
			{
				return config;
			}
		}

		public Button Close
		{
			get
			{
				return close;
			}
		}

		public bool IsVisible
		{
			get
			{
				return isVisible.Value;
			}
			set
			{
				isVisible.Value = value;
			}
		}

		private void ButtonVisible(bool isOn)
		{
			if (isOn)
			{
				canvasGroup.alpha = 1f;
				canvasGroup.blocksRaycasts = true;
			}
			else
			{
				canvasGroup.alpha = 0f;
				canvasGroup.blocksRaycasts = false;
			}
		}

		private void Awake()
		{
			sels = new Selectable[6] { skip, auto, backLog, voice, config, close };
			Skip.isOn = false;
			Auto.isOn = false;
		}

		private void Start()
		{
			isVisible.TakeUntilDestroy(this).Subscribe(ButtonVisible);
			selUIs = sels.Select(NullCheck.GetOrAddComponent<SelectUI>).ToArray();
			Skip.isOn = scenario.isSkip;
			this.ObserveEveryValueChanged((ADVButton _) => !scenario.regulate.control.HasFlag(Regulate.Control.Skip)).SubscribeToInteractable(Skip);
			Skip.OnDisableAsObservable().Subscribe(delegate
			{
				Skip.isOn = false;
			});
			Skip.onValueChanged.AsObservable().Subscribe(delegate(bool isOn)
			{
				if (isOn)
				{
					Utils.Sound.Play(SystemSE.sel);
				}
				scenario.isSkip = isOn;
			});
			Auto.isOn = scenario.isAuto;
			this.ObserveEveryValueChanged((ADVButton _) => !scenario.regulate.control.HasFlag(Regulate.Control.Auto)).SubscribeToInteractable(Auto);
			Auto.OnDisableAsObservable().Subscribe(delegate
			{
				Auto.isOn = false;
			});
			Auto.onValueChanged.AsObservable().Subscribe(delegate(bool isOn)
			{
				if (isOn)
				{
					Utils.Sound.Play(SystemSE.sel);
				}
				scenario.isAuto = isOn;
			});
			this.UpdateAsObservable().Subscribe(delegate
			{
				if (scenario.isSkip != skip.isOn)
				{
					skip.isOn = scenario.isSkip;
				}
				if (scenario.isAuto != auto.isOn)
				{
					auto.isOn = scenario.isAuto;
				}
			});
			BackLog.OnClickAsObservable().Subscribe(delegate
			{
				scenario.mode = MainScenario.Mode.BackLog;
				Utils.Sound.Play(SystemSE.sel);
			});
			Func<bool> voiceRegCheck = () => !scenario.isAuto && !scenario.currentCharaData.isKaraoke && !scenario.currentCharaData.voiceList.IsNullOrEmpty();
			this.ObserveEveryValueChanged((ADVButton _) => voiceRegCheck()).SubscribeToInteractable(Voice);
			Voice.OnClickAsObservable().Subscribe(delegate
			{
				scenario.VoicePlay(scenario.currentCharaData.voiceList);
			});
			string levelName = "Config";
			(from _ in Config.OnClickAsObservable()
				where !Singleton<Scene>.Instance.NowSceneNames.Contains(levelName)
				select _).Subscribe(delegate
			{
				EventSystem.current.SetSelectedGameObject(null);
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = levelName,
					isAdd = true
				}, false);
				Utils.Sound.Play(SystemSE.sel);
			});
			Close.OnClickAsObservable().Subscribe(delegate
			{
				scenario.mode = MainScenario.Mode.WindowNone;
				Utils.Sound.Play(SystemSE.sel);
			});
		}
	}
}
