using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class AnimeList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private MPCharCtrl mpCharCtrl;

		private AnimeGroupList.SEX sex = AnimeGroupList.SEX.Unknown;

		private int group = -1;

		private int category = -1;

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
				}
			}
		}

		public void InitList(AnimeGroupList.SEX _sex, int _group, int _category)
		{
			int childCount = transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			scrollRect.verticalNormalizedPosition = 1f;
			dicNode = new Dictionary<int, Image>();
			foreach (KeyValuePair<int, Info.AnimeLoadInfo> item in Singleton<Info>.Instance.dicAnimeLoadInfo[_group][_category])
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
			sex = _sex;
			group = _group;
			category = _category;
			select = -1;
			active = true;
		}

		private void OnSelect(int _no)
		{
			mpCharCtrl.LoadAnime(sex, group, category, _no);
			int key = select;
			if (Utility.SetStruct(ref select, _no))
			{
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
