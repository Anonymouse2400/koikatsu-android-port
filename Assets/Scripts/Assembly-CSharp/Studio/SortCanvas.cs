using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class SortCanvas : Singleton<SortCanvas>
	{
		[SerializeField]
		private Canvas[] canvas;

		public static Canvas select
		{
			set
			{
				if (Singleton<SortCanvas>.IsInstance())
				{
					Singleton<SortCanvas>.Instance.OnSelect(value);
				}
			}
		}

		public void OnSelect(Canvas _canvas)
		{
			if (_canvas == null)
			{
				return;
			}
			SortedList<int, Canvas> sortedList = new SortedList<int, Canvas>();
			_canvas.sortingOrder = 10;
			for (int j = 0; j < canvas.Length; j++)
			{
				sortedList.Add(canvas[j].sortingOrder, canvas[j]);
			}
			foreach (var item in sortedList.Select((KeyValuePair<int, Canvas> l, int i) => new { l.Value, i }))
			{
				item.Value.sortingOrder = item.i;
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}
	}
}
