using System.Collections.Generic;
using UnityEngine;

namespace Studio
{
	public class LightList : MonoBehaviour
	{
		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private Transform transformRoot;

		public void OnClick(int _no)
		{
			Singleton<Studio>.Instance.AddLight(_no);
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
		}

		private void Awake()
		{
			foreach (KeyValuePair<int, Info.LightLoadInfo> item in Singleton<Info>.Instance.dicLightLoadInfo)
			{
				AddNode(item.Key, item.Value.name);
			}
		}
	}
}
