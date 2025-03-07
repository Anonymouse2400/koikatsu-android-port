using System;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class AddButtonCtrl : MonoBehaviour
	{
		[Serializable]
		private class CommonInfo
		{
			public GameObject obj;

			public Button button;

			public bool active
			{
				set
				{
					if (obj.activeSelf != value)
					{
						obj.SetActive(value);
						button.image.color = ((!value) ? Color.white : Color.green);
					}
				}
			}
		}

		[SerializeField]
		private CommonInfo[] commonInfo;

		[SerializeField]
		private Canvas canvas;

		private int select = -1;

		public void OnClick(int _kind)
		{
			select = ((select != _kind) ? _kind : (-1));
			for (int i = 0; i < commonInfo.Length; i++)
			{
				commonInfo[i].active = i == select;
			}
			SortCanvas.select = canvas;
		}

		private void Start()
		{
			select = -1;
		}
	}
}
