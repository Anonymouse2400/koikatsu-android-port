using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ItemList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private ScrollRect scrollRect;

		private int group = -1;

		private int category = -1;

		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				if (base.gameObject.activeSelf != value)
				{
					base.gameObject.SetActive(value);
					if (!value)
					{
						category = -1;
					}
				}
			}
		}

		public void InitList(int _group, int _category)
		{
			if (group == _group && category == _category)
			{
				return;
			}
			int childCount = transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			scrollRect.verticalNormalizedPosition = 1f;
			foreach (KeyValuePair<int, Info.ItemLoadInfo> item in Singleton<Info>.Instance.dicItemLoadInfo[_group][_category])
			{
				GameObject gameObject = Object.Instantiate(objectNode);
				gameObject.transform.SetParent(transformRoot, false);
				StudioNode component = gameObject.GetComponent<StudioNode>();
				component.active = true;
				int no = item.Key;
				component.addOnClick = delegate
				{
					OnSelect(no);
				};
				component.text = item.Value.name;
				int num = item.Value.color.Count((bool b) => b) + (item.Value.isGlass ? 1 : 0);
				switch (num)
				{
				case 1:
					component.textColor = Color.red;
					break;
				case 2:
					component.textColor = Color.cyan;
					break;
				case 3:
					component.textColor = Color.green;
					break;
				case 4:
					component.textColor = Color.yellow;
					break;
				default:
					component.textColor = Color.white;
					break;
				}
				if (num != 0 && (bool)component.textUI)
				{
					Shadow shadow = component.textUI.gameObject.AddComponent<Shadow>();
					shadow.effectColor = Color.black;
				}
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
			group = _group;
			category = _category;
		}

		private void OnSelect(int _no)
		{
			Singleton<Studio>.Instance.AddItem(group, category, _no);
		}

		private void Awake()
		{
			group = -1;
			category = -1;
		}
	}
}
