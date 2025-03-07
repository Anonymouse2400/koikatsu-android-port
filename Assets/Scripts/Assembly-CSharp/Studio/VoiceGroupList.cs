using System.Collections.Generic;
using UnityEngine;

namespace Studio
{
	public class VoiceGroupList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private VoiceCategoryList voiceCategoryList;

		[SerializeField]
		private VoiceList voiceList;

		private int select = -1;

		private Dictionary<int, StudioNode> dicNode = new Dictionary<int, StudioNode>();

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

		private void InitList()
		{
			int childCount = transformRoot.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			foreach (KeyValuePair<int, Info.GroupInfo> item in Singleton<Info>.Instance.dicVoiceGroupCategory)
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
				component.text = item.Value.name;
				dicNode.Add(item.Key, component);
			}
			select = -1;
			active = true;
		}

		private void OnSelect(int _no)
		{
			int key = select;
			if (Utility.SetStruct(ref select, _no))
			{
				voiceCategoryList.InitList(_no);
				voiceList.active = false;
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

		private void Start()
		{
			InitList();
		}
	}
}
