using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class AnimeCategoryList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private AnimeList animeList;

		private AnimeGroupList.SEX sex = AnimeGroupList.SEX.Unknown;

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
						animeList.active = false;
					}
				}
			}
		}

		public void InitList(AnimeGroupList.SEX _sex, int _group)
		{
			int childCount = transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			scrollRect.verticalNormalizedPosition = 1f;
			dicNode = new Dictionary<int, Image>();
			foreach (KeyValuePair<int, string> item in Singleton<Info>.Instance.dicAGroupCategory[_group].dicCategory)
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
				dicNode.Add(item.Key, component.image);
			}
			select = -1;
			group = _group;
			sex = _sex;
			active = true;
			animeList.active = false;
		}

		private bool CheckCategory(int _group, int _category, Dictionary<int, Dictionary<int, Dictionary<int, Info.AnimeLoadInfo>>> _dic)
		{
			Dictionary<int, Dictionary<int, Info.AnimeLoadInfo>> value = null;
			if (_dic.TryGetValue(_group, out value))
			{
				return value.ContainsKey(_category);
			}
			return false;
		}

		private void OnSelect(int _no)
		{
			int key = select;
			if (Utility.SetStruct(ref select, _no))
			{
				animeList.InitList(sex, group, _no);
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
