using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ItemCategoryList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private ItemList itemList;

		private int group = -1;

		private int select = -1;

		private Dictionary<int, Image> dicNode;

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
					if (!base.gameObject.activeSelf)
					{
						itemList.active = false;
					}
				}
			}
		}

		public void InitList(int _group)
		{
			int childCount = transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			scrollRect.verticalNormalizedPosition = 1f;
			dicNode = new Dictionary<int, Image>();
			foreach (KeyValuePair<int, string> item in Singleton<Info>.Instance.dicItemGroupCategory[_group].dicCategory)
			{
				GameObject gameObject = Object.Instantiate(objectPrefab);
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				gameObject.transform.SetParent(transformRoot, false);
				ListNode component = gameObject.GetComponent<ListNode>();
				int no = item.Key;
				component.AddActionToButton(delegate
				{
					OnSelect(no);
				});
				component.text = item.Value;
				dicNode.Add(item.Key, gameObject.GetComponent<Image>());
			}
			select = -1;
			group = _group;
			active = true;
			itemList.active = false;
		}

		private void OnSelect(int _no)
		{
			int key = select;
			if (Utility.SetStruct(ref select, _no))
			{
				itemList.InitList(group, _no);
				Image value = null;
				if (dicNode.TryGetValue(key, out value) && value != null)
				{
					value.color = Color.white;
				}
				value = null;
				if (dicNode.TryGetValue(select, out value) && value != null)
				{
					value.color = Color.green;
				}
			}
		}
	}
}
