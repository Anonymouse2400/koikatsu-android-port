using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class VoiceList : MonoBehaviour
	{
		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private GameObject objectPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private VoiceControl voiceControl;

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
				}
				if (!base.gameObject.activeSelf)
				{
					group = -1;
					category = -1;
				}
			}
		}

		public void InitList(int _group, int _category)
		{
			if (!Utility.SetStruct(ref group, _group) && !Utility.SetStruct(ref category, _category))
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
			foreach (KeyValuePair<int, Info.LoadCommonInfo> item in Singleton<Info>.Instance.dicVoiceLoadInfo[_group][_category])
			{
				GameObject gameObject = Object.Instantiate(objectPrefab);
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				gameObject.transform.SetParent(transformRoot, false);
				VoiceNode component = gameObject.GetComponent<VoiceNode>();
				int no = item.Key;
				component.addOnClick = delegate
				{
					OnSelect(no);
				};
				component.text = item.Value.name;
			}
			active = true;
			group = _group;
			category = _category;
		}

		private void OnSelect(int _no)
		{
			OCIChar[] array = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey
				select Studio.GetCtrlInfo(v) as OCIChar into v
				where v != null
				select v).ToArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].AddVoice(group, category, _no);
			}
			voiceControl.InitList();
		}
	}
}
