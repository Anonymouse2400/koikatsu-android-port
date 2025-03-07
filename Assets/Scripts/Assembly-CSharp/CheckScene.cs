using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using Illusion.Game;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckScene : MonoBehaviour
{
	public class Parameter
	{
		public enum InteractableType
		{
			None = 0,
			Loading = 1,
			LoadingFade = 2
		}

		public InteractableType type;

		public bool isOnePush = true;

		public bool isYesFocus = true;

		public string Title { get; set; }

		public UnityAction Yes { get; set; }

		public UnityAction No { get; set; }

		public string YesText { get; set; }

		public string NoText { get; set; }

		public Action Init { get; set; }

		public Action End { get; set; }
	}

	[Label("キャンバス")]
	[SerializeField]
	private Canvas Canvas;

	[Label("OKボタン")]
	[SerializeField]
	private Button OK;

	[Label("Yesボタン")]
	[SerializeField]
	private Button Yes;

	[SerializeField]
	[Label("Noボタン")]
	private Button No;

	[SerializeField]
	[Label("確認テキスト")]
	private TextMeshProUGUI Title;

	private CanvasGroup canvasGroup;

	private float timeScale = 1f;

	private Parameter parameter;

	public Button OKButton
	{
		get
		{
			return OK;
		}
	}

	public Button YesButton
	{
		get
		{
			return Yes;
		}
	}

	public Button NoButton
	{
		get
		{
			return No;
		}
	}

	public TextMeshProUGUI TitleText
	{
		get
		{
			return Title;
		}
	}

	public void Set(Parameter parameter)
	{
		this.parameter = parameter;
	}

	private void Awake()
	{
		timeScale = Time.timeScale;
		Time.timeScale = 0f;
		OK.gameObject.SetActive(false);
		Yes.gameObject.SetActive(false);
		No.gameObject.SetActive(false);
		canvasGroup = Canvas.GetComponent<CanvasGroup>();
	}

	private IEnumerator Start()
	{
		base.enabled = false;
		yield return new WaitWhile(() => parameter == null);
		if (canvasGroup != null)
		{
			switch (parameter.type)
			{
			case Parameter.InteractableType.Loading:
				this.ObserveEveryValueChanged((CheckScene _) => !Singleton<Scene>.Instance.IsNowLoading).Subscribe(delegate(bool isOn)
				{
					canvasGroup.interactable = isOn;
				});
				break;
			case Parameter.InteractableType.LoadingFade:
				this.ObserveEveryValueChanged((CheckScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
				{
					canvasGroup.interactable = isOn;
				});
				break;
			}
		}
		parameter.Init.Call();
		Canvas canvas = Title.canvas;
		Canvas[] sorter = (from p in UnityEngine.Object.FindObjectsOfType<Canvas>()
			where p != canvas
			select p).ToArray();
		if (sorter.Any())
		{
			canvas.sortingOrder = sorter.Max((Canvas p) => p.sortingOrder) + 1;
		}
		if (!parameter.Title.IsNullOrEmpty())
		{
			Title.text = parameter.Title;
		}
		Action<Button, UnityAction, string> AddEvent = delegate(Button bt, UnityAction act, string text)
		{
			bt.gameObject.SetActive(true);
			bt.OnClickAsObservable().Subscribe(delegate
			{
				act();
			});
			if (!text.IsNullOrEmpty())
			{
				bt.GetComponentInChildren<Text>().text = text;
			}
		};
		if (!parameter.Yes.IsNullOrEmpty())
		{
			if (parameter.No.IsNullOrEmpty())
			{
				AddEvent(OK, parameter.Yes, parameter.YesText);
			}
			else
			{
				AddEvent(Yes, parameter.Yes, parameter.YesText);
				AddEvent(No, parameter.No, parameter.NoText);
			}
		}
		if (!OK.onClick.IsNullOrEmpty())
		{
			OK.gameObject.SetActive(true);
		}
		if (!Yes.onClick.IsNullOrEmpty())
		{
			Yes.gameObject.SetActive(true);
		}
		if (!No.onClick.IsNullOrEmpty())
		{
			No.gameObject.SetActive(true);
		}
		Button[] buttons = new Button[3] { OK, Yes, No };
		if (parameter.isOnePush)
		{
			OK.OnClickAsObservable().Merge(Yes.OnClickAsObservable(), No.OnClickAsObservable()).Subscribe(delegate
			{
				Button[] array = buttons;
				foreach (Button button in array)
				{
					button.interactable = false;
				}
			});
		}
		IEnumerable<Button> enters = new Button[2] { OK, Yes }.Where((Button p) => p.gameObject.activeSelf);
		IEnumerable<Button> cancels = new Button[1] { No }.Where((Button p) => p.gameObject.activeSelf);
		foreach (Button item in enters)
		{
			item.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		}
		foreach (Button item2 in cancels)
		{
			item2.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		}
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where buttons.Any((Button p) => p.interactable)
			select _).Subscribe(delegate
		{
			if (parameter.isOnePush)
			{
				Button[] array2 = buttons;
				foreach (Button button2 in array2)
				{
					button2.interactable = false;
				}
			}
			if (!parameter.No.IsNullOrEmpty())
			{
				parameter.No();
			}
			else if (!parameter.Yes.IsNullOrEmpty())
			{
				parameter.Yes();
			}
		});
		base.enabled = true;
	}

	private void OnDestroy()
	{
		Time.timeScale = timeScale;
		if (parameter != null)
		{
			parameter.End.Call();
		}
	}
}
