using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class LogoList : MonoBehaviour
	{
		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private Image imageLogo;

		[SerializeField]
		private Sprite[] spriteLogo;

		[SerializeField]
		private string[] strName;

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
			int logo = Studio.optionSystem.logo;
			ListNode value = null;
			if (dicNode.TryGetValue(logo, out value))
			{
				value.select = true;
				select = logo;
			}
			else if (dicNode.TryGetValue(-1, out value))
			{
				value.select = true;
				select = -1;
			}
		}

		public void OnClick(int _no)
		{
			Studio.optionSystem.logo = _no;
			UpdateLogo();
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
			for (int i = 0; i < strName.Length; i++)
			{
				AddNode(i, strName[i]);
			}
			ListNode value = null;
			if (dicNode.TryGetValue(Studio.optionSystem.logo, out value))
			{
				value.select = true;
			}
			UpdateLogo();
			select = Studio.optionSystem.logo;
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

		private void UpdateLogo()
		{
			Sprite sprite = spriteLogo.SafeGet(Studio.optionSystem.logo);
			imageLogo.sprite = sprite;
			imageLogo.color = ((!(sprite == null)) ? Color.white : Color.clear);
		}
	}
}
