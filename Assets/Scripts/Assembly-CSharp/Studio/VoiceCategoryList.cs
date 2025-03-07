using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class VoiceCategoryList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private VoiceList voiceList;

		private int group = -1;

		private int select = -1;

		private Dictionary<int, StudioNode> dicNode;

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

		public void InitList(int _group)
		{
			if (!Utility.SetStruct(ref group, _group))
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
			dicNode = new Dictionary<int, StudioNode>();
			foreach (KeyValuePair<int, string> item in Singleton<Info>.Instance.dicVoiceGroupCategory[_group].dicCategory)
			{
				if (CheckCategory(_group, item.Key))
				{
					GameObject gameObject = Object.Instantiate(objectPrefab);
					if (!gameObject.activeSelf)
					{
						gameObject.SetActive(true);
					}
					gameObject.transform.SetParent(transformRoot, false);
					StudioNode component = gameObject.GetComponent<StudioNode>();
					int no = item.Key;
					component.addOnClick = delegate
					{
						OnSelect(no);
					};
					component.text = item.Value;
					dicNode.Add(item.Key, component);
				}
			}
			select = -1;
			active = true;
			voiceList.active = false;
		}

		private bool CheckCategory(int _group, int _category)
		{
			Dictionary<int, Dictionary<int, Info.LoadCommonInfo>> value = null;
			if (!Singleton<Info>.Instance.dicVoiceLoadInfo.TryGetValue(_group, out value))
			{
				return false;
			}
			return value.ContainsKey(_category);
		}

		private void OnSelect(int _no)
		{
			int key = select;
			if (Utility.SetStruct(ref select, _no))
			{
				voiceList.InitList(group, _no);
				StudioNode value = null;
				if (dicNode.TryGetValue(key, out value))
				{
					value.select = false;
				}
				if (dicNode.TryGetValue(select, out value))
				{
					value.select = true;
				}
			}
		}
	}
}
