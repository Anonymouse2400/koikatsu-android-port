using System.Collections.Generic;
using UnityEngine;

namespace Studio
{
	public class FilterList : MonoBehaviour
	{
		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private Transform transformRoot;

		private int select = -1;

		private Dictionary<int, ListNode> dicNode = new Dictionary<int, ListNode>();

		private bool isInit;

		public void UpdateInfo()
		{
			if (!isInit)
			{
				return;
			}
			foreach (KeyValuePair<int, ListNode> item in dicNode)
			{
				item.Value.select = false;
			}
			ListNode value = null;
			if (dicNode.TryGetValue(Singleton<Studio>.Instance.sceneInfo.aceNo, out value))
			{
				value.select = true;
				select = Singleton<Studio>.Instance.sceneInfo.aceNo;
			}
			else if (dicNode.TryGetValue(-1, out value))
			{
				value.select = true;
				select = -1;
			}
		}

		public void OnClick(int _no)
		{
			Singleton<Studio>.Instance.SetACE(_no);
			ListNode value = null;
			if (dicNode.TryGetValue(select, out value))
			{
				value.select = false;
			}
			if (dicNode.TryGetValue(_no, out value))
			{
				value.select = true;
			}
			select = _no;
		}

		public void Init()
		{
			foreach (KeyValuePair<int, Info.LoadCommonInfo> item in Singleton<Info>.Instance.dicFilterLoadInfo)
			{
				AddNode(item.Key, item.Value.name);
			}
			ListNode value = null;
			if (dicNode.TryGetValue(Singleton<Studio>.Instance.sceneInfo.aceNo, out value))
			{
				value.select = true;
			}
			select = Singleton<Studio>.Instance.sceneInfo.aceNo;
			isInit = true;
		}

		private void AddNode(int _key, string _name)
		{
			GameObject gameObject = Object.Instantiate(objectNode);
			gameObject.transform.SetParent(transformRoot, false);
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			ListNode component = gameObject.GetComponent<ListNode>();
			int key = _key;
			component.AddActionToButton(delegate
			{
				OnClick(key);
			});
			component.text = _name;
			dicNode.Add(key, component);
		}
	}
}
