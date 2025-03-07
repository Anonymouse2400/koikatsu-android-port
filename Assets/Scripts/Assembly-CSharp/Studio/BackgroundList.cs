using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class BackgroundList : MonoBehaviour
	{
		public static string dirName = "bg";

		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private BackgroundCtrl backgroundCtrl;

		private List<string> listPath = new List<string>();

		private Dictionary<int, StudioNode> dicNode = new Dictionary<int, StudioNode>();

		private int select = -1;

		public void UpdateUI()
		{
			SetSelect(select, false);
			select = listPath.FindIndex((string v) => v == Singleton<Studio>.Instance.sceneInfo.background);
			SetSelect(select, true);
		}

		private void OnClickSelect(int _idx)
		{
			SetSelect(select, false);
			select = _idx;
			SetSelect(select, true);
			backgroundCtrl.Load((select == -1) ? string.Empty : listPath[_idx]);
		}

		private void SetSelect(int _idx, bool _flag)
		{
			StudioNode value = null;
			if (dicNode.TryGetValue(_idx, out value))
			{
				value.select = _flag;
			}
		}

		private void InitList()
		{
			for (int i = 0; i < transformRoot.childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			listPath = Directory.GetFiles(UserData.Create(dirName), "*.png").Select(Path.GetFileName).ToList();
			CreateNode(-1, "なし");
			int count = listPath.Count;
			for (int j = 0; j < count; j++)
			{
				CreateNode(j, Path.GetFileNameWithoutExtension(listPath[j]));
			}
			select = listPath.FindIndex((string v) => v == Singleton<Studio>.Instance.sceneInfo.background);
			SetSelect(select, true);
		}

		private void CreateNode(int _idx, string _text)
		{
			GameObject gameObject = Object.Instantiate(objectNode);
			gameObject.transform.SetParent(transformRoot, false);
			StudioNode component = gameObject.GetComponent<StudioNode>();
			component.active = true;
			component.addOnClick = delegate
			{
				OnClickSelect(_idx);
			};
			component.text = _text;
			dicNode.Add(_idx, component);
		}

		private void Start()
		{
			InitList();
		}
	}
}
