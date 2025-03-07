using System;
using System.Collections;
using System.Linq;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Extensions;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectDateMenuScene : BaseLoader
{
	[Serializable]
	private class GotoEvent
	{
		public enum ConditionType
		{
			Interactable = 0,
			Visible = 1
		}

		[SerializeField]
		private Button _button;

		[SerializeField]
		[Header("発生するADV番号")]
		private int _advNo;

		[Header("条件(親密度以上)")]
		[SerializeField]
		private int _condition;

		[Header("条件を満たさなかった場合の挙動")]
		[SerializeField]
		private ConditionType _conditionType;

		[Header("一回のみ発生")]
		[SerializeField]
		private bool _isOneceEvent;

		[Header("条件(イベント番号)")]
		[SerializeField]
		private int[] _conditionADV;

		public Button button
		{
			get
			{
				return _button;
			}
		}

		public int advNo
		{
			get
			{
				return _advNo;
			}
		}

		public int condition
		{
			get
			{
				return _condition;
			}
		}

		public ConditionType conditionType
		{
			get
			{
				return _conditionType;
			}
		}

		public bool isOneceEvent
		{
			get
			{
				return _isOneceEvent;
			}
		}

		public int[] conditionADV
		{
			get
			{
				return _conditionADV;
			}
		}
	}

	private Subject<int> _result = new Subject<int>();

	[SerializeField]
	private Canvas canvas;

	[Header("親密度表示テキスト")]
	[SerializeField]
	private TextMeshProUGUI intimacyText;

	[SerializeField]
	private GotoEvent[] maps;

	public bool isVisible
	{
		get
		{
			return canvas.enabled;
		}
		set
		{
			canvas.enabled = value;
		}
	}

	public SaveData.Heroine target { get; set; }

	public Subject<int> result
	{
		get
		{
			return _result;
		}
	}

	private IEnumerator Start()
	{
		base.enabled = false;
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((MapSelectDateMenuScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		canvas.enabled = false;
		yield return new WaitWhile(() => target == null);
		target.intimacy = Mathf.Clamp(target.intimacy, 0, 100);
		intimacyText.text = string.Format("{0:0}", target.intimacy);
		GotoEvent[] array = maps;
		foreach (GotoEvent item in array)
		{
			bool flag = target.intimacy >= item.condition;
			if (flag && item.isOneceEvent)
			{
				flag = !target.talkEvent.Contains(item.advNo);
			}
			if (flag)
			{
				flag = !item.conditionADV.Except(target.talkEvent).Any();
			}
			switch (item.conditionType)
			{
			case GotoEvent.ConditionType.Interactable:
				item.button.interactable = flag;
				break;
			case GotoEvent.ConditionType.Visible:
				item.button.gameObject.SetActiveIfDifferent(flag);
				break;
			}
			item.button.SelectSE();
			(from _ in item.button.OnClickAsObservable()
				select item).Take(1).Subscribe(delegate(GotoEvent p)
			{
				_result.OnNext(p.advNo);
				canvas.enabled = false;
				Utils.Sound.Play(SystemSE.ok_s);
			});
		}
		canvas.enabled = true;
		base.enabled = true;
	}
}
