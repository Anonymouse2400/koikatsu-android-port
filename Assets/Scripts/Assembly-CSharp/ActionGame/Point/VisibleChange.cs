using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ActionGame.Point
{
	public class VisibleChange : MonoBehaviour
	{
		[SerializeField]
		[Header("表示")]
		private List<GameObject> _visibleList = new List<GameObject>();

		[SerializeField]
		[Header("非表示")]
		private List<GameObject> _unVisibleList = new List<GameObject>();

		private BoolReactiveProperty _visible = new BoolReactiveProperty(true);

		public List<GameObject> visibleList
		{
			get
			{
				return _visibleList;
			}
		}

		public List<GameObject> unVisibleList
		{
			get
			{
				return _unVisibleList;
			}
		}

		public bool visible
		{
			get
			{
				return _visible.Value;
			}
			set
			{
				_visible.Value = value;
			}
		}

		private void Start()
		{
			_visible.TakeUntilDestroy(this).Subscribe(delegate(bool isVisible)
			{
				_visibleList.ForEach(delegate(GameObject go)
				{
					go.SetActive(isVisible);
				});
				_unVisibleList.ForEach(delegate(GameObject go)
				{
					go.SetActive(!isVisible);
				});
			});
		}
	}
}
