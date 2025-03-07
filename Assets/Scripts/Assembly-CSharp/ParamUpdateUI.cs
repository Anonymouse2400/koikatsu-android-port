using System;
using System.Collections.Generic;
using System.Linq;
using Localize.Translate;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ParamUpdateUI : MonoBehaviour
{
	public int physical;

	public int intellect;

	public int hentai;

	public int drop;

	[SerializeField]
	private Image[] imageBack;

	[SerializeField]
	private Animator[] anmPhysical;

	[SerializeField]
	private Animator[] anmIntellect;

	[SerializeField]
	private Animator[] anmHentai;

	[SerializeField]
	private RuntimeAnimatorController[] racPhysical;

	[SerializeField]
	private RuntimeAnimatorController[] racIntellect;

	[SerializeField]
	private RuntimeAnimatorController[] racHentai;

	[SerializeField]
	private Image imageDrop;

	[SerializeField]
	private Sprite[] spriteDrop;

	[Space(4f)]
	[SerializeField]
	private int interval = 50;

	[SerializeField]
	private int intervalCount = 3;

	[SerializeField]
	private float waitTime = 1f;

	private void Setup(Animator[] _animator, RuntimeAnimatorController[] _controller, int _value)
	{
		if (_value == 0)
		{
			return;
		}
		int num = ((_value <= 0) ? 1 : 0);
		for (int i = 0; i < _animator.Length; i++)
		{
			if (Mathf.Abs(_value) > i)
			{
				_animator[i].enabled = true;
				_animator[i].runtimeAnimatorController = _controller[num];
			}
		}
	}

	private void Start()
	{
		SingleAssignmentDisposable disposable = new SingleAssignmentDisposable();
		int count = 0;
		int idx = ((drop != 0) ? 1 : 0);
		if (Singleton<Game>.IsInstance())
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			Dictionary<int, Data.Param>.ValueCollection values = actScene.uiTranslater.Get(5).Values;
			values.FindTags("ImageBack").ToArray().SafeProc(idx, delegate(Data.Param data)
			{
				Localize.Translate.Manager.Bind(imageBack[idx], data, true);
			});
			if (drop > 0)
			{
				int index = drop - 1;
				values.FindTags("Get").ToArray().SafeProc(index, delegate(Data.Param data)
				{
					Sprite sprite = Localize.Translate.Manager.Convert(data.Load(true));
					if (sprite != null)
					{
						spriteDrop[index] = sprite;
					}
				});
			}
		}
		disposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(interval)).Subscribe(delegate
		{
			count++;
			Color color = imageBack[idx].color;
			color.a = Mathf.InverseLerp(0f, intervalCount, count);
			imageBack[idx].color = color;
			if (count >= intervalCount)
			{
				disposable.Dispose();
				Setup(anmPhysical, racPhysical, physical);
				Setup(anmIntellect, racIntellect, intellect);
				Setup(anmHentai, racHentai, hentai);
				if (drop != 0)
				{
					imageDrop.enabled = true;
					imageDrop.sprite = spriteDrop[drop - 1];
				}
				UnityEngine.Object.Destroy(base.gameObject, waitTime);
			}
		}).AddTo(this);
	}
}
