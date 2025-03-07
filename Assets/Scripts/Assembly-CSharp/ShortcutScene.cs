using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H;
using Manager;
using UniRx;
using UnityEngine;

public class ShortcutScene : BaseLoader
{
	private const int WAIT_NO = -1;

	[SerializeField]
	private IntReactiveProperty _page = new IntReactiveProperty(-1);

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private SpriteChangeCtrl pages;

	public int page
	{
		get
		{
			return _page.Value;
		}
		set
		{
			_page.Value = value;
		}
	}

	public void Unload()
	{
		if (Singleton<Scene>.Instance.NowSceneNames[0] == "Shortcut")
		{
			Singleton<Scene>.Instance.UnLoad();
		}
	}

	private IEnumerator Start()
	{
		yield return new WaitWhile(() => _page.Value == -1);
		IEnumerable<Canvas> sorter = from p in Object.FindObjectsOfType<Canvas>()
			where p != canvas
			select p;
		if (sorter.Any())
		{
			canvas.sortingOrder = sorter.Max((Canvas p) => p.sortingOrder) + 1;
		}
		_page.Subscribe(delegate(int no)
		{
			pages.OnChangeValue(no);
		});
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			Unload();
		}
	}
}
