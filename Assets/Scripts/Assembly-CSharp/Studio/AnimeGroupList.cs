using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class AnimeGroupList : MonoBehaviour
	{
		public enum SEX
		{
			Male = 0,
			Female = 1,
			Unknown = 2
		}

		public SEX sex = SEX.Unknown;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private AnimeCategoryList animeCategoryList;

		[SerializeField]
		private AnimeList animeList;

		private int select = -1;

		private Dictionary<int, Image> dicNode;

		private bool isInit;

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
						animeCategoryList.active = false;
					}
				}
			}
		}

		public void InitList(SEX _sex)
		{
			if (isInit)
			{
				return;
			}
			int childCount = transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			scrollRect.verticalNormalizedPosition = 1f;
			dicNode = new Dictionary<int, Image>();
			sex = _sex;
			foreach (KeyValuePair<int, Info.GroupInfo> item in Singleton<Info>.Instance.dicAGroupCategory)
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
				component.text = item.Value.name;
				dicNode.Add(item.Key, component.image);
			}
			select = -1;
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
			animeCategoryList.active = false;
			animeList.active = false;
			isInit = true;
		}

		private void OnSelect(int _no)
		{
			int key = select;
			if (Utility.SetStruct(ref select, _no))
			{
				animeCategoryList.InitList(sex, _no);
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
