using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TimeUIControl : MonoBehaviour
{
	[SerializeField]
	private GameObject[] objTimeZone;

	[SerializeField]
	private Image imgGage01;

	[SerializeField]
	private Image imgGage02;

	[SerializeField]
	private Button btnNext;

	[SerializeField]
	[RangeReactiveProperty(0f, 5f)]
	private IntReactiveProperty _dayOfTheWeek = new IntReactiveProperty(0);

	[RangeReactiveProperty(0f, 4f)]
	[SerializeField]
	private IntReactiveProperty _timeZone = new IntReactiveProperty(0);

	[SerializeField]
	[RangeReactiveProperty(0f, 1f)]
	private FloatReactiveProperty _timePass = new FloatReactiveProperty(0f);

	[SerializeField]
	private GameObject _weekParent;

	[SerializeField]
	private GameObject _timeParent;

	public int dayOfTheWeek
	{
		get
		{
			return _dayOfTheWeek.Value;
		}
		set
		{
			_dayOfTheWeek.Value = value;
		}
	}

	public int timeZone
	{
		get
		{
			return _timeZone.Value;
		}
		set
		{
			_timeZone.Value = value;
		}
	}

	public float timePass
	{
		get
		{
			return _timePass.Value;
		}
		set
		{
			_timePass.Value = value;
		}
	}

	private void Start()
	{
		bool isSaturday = false;
		List<GameObject> source = _weekParent.Children();
		GameObject[] array = source.Take(5).ToArray();
		IEnumerable<GameObject> source2 = source.Skip(array.Length);
		GameObject[] weeks = array.Concat(source2.Take(2)).ToArray();
		_dayOfTheWeek.Subscribe(delegate(int idx)
		{
			isSaturday = idx == 5;
			for (int j = 0; j < weeks.Length; j++)
			{
				weeks[j].SetActiveIfDifferent(j == idx);
			}
		});
		GameObject[] objTimeZoneSat = _timeParent.Children().Skip(objTimeZone.Length).ToArray();
		_timeZone.Subscribe(delegate(int idx)
		{
			if (!isSaturday || idx == 0 || idx == 4 || objTimeZoneSat.Length == 0)
			{
				for (int k = 0; k < objTimeZone.Length; k++)
				{
					objTimeZone[k].SetActiveIfDifferent(k == idx);
				}
				for (int l = 0; l < objTimeZoneSat.Length; l++)
				{
					objTimeZoneSat[l].SetActiveIfDifferent(false);
				}
			}
			else
			{
				for (int m = 0; m < objTimeZone.Length; m++)
				{
					objTimeZone[m].SetActiveIfDifferent(false);
				}
				for (int n = 0; n < objTimeZoneSat.Length; n++)
				{
					objTimeZoneSat[n].SetActiveIfDifferent(n == idx - 1);
				}
			}
		});
		_timePass.Subscribe(delegate(float val)
		{
			val = Mathf.Clamp(val, 0f, 1f);
			float fillAmount = 1f - Mathf.InverseLerp(0f, 0.7f, Mathf.Min(0.7f, val));
			imgGage01.fillAmount = fillAmount;
			float fillAmount2 = 1f - Mathf.InverseLerp(0.7f, 1f, Mathf.Max(0.7f, val));
			imgGage02.fillAmount = fillAmount2;
		});
		ReadOnlyReactiveProperty<bool> source3 = _timeZone.Select((int i) => i != 0 && i != 4).CombineLatest(_timePass.Select((float f) => f < 1f), (bool x, bool y) => x && y).DistinctUntilChanged()
			.ToReadOnlyReactiveProperty();
		source3.SubscribeToInteractable(btnNext);
		btnNext.OnClickAsObservable().Subscribe(delegate
		{
			Singleton<Game>.Instance.actScene.Cycle.ActionEnd();
		});
	}

	private void Reset()
	{
		Transform transform = base.gameObject.transform.Find("week");
		if ((bool)transform)
		{
			_weekParent = transform.gameObject;
		}
		Transform transform2 = base.gameObject.transform.Find("timezone");
		if ((bool)transform2)
		{
			string[] array = new string[5] { "timezone00", "timezone01", "timezone02", "timezone03", "timezone04" };
			objTimeZone = new GameObject[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				objTimeZone[i] = transform2.Find(array[i]).gameObject;
			}
		}
		Transform transform3 = base.gameObject.transform.Find("gagetop/gagemask/gageN01");
		if ((bool)transform3)
		{
			imgGage01 = transform3.GetComponent<Image>();
		}
		Transform transform4 = base.gameObject.transform.Find("gagetop/gagemask/gageN02");
		if ((bool)transform4)
		{
			imgGage02 = transform4.GetComponent<Image>();
		}
		Transform transform5 = base.gameObject.transform.Find("btnNext");
		if ((bool)transform5)
		{
			btnNext = transform5.GetComponent<Button>();
		}
	}
}
