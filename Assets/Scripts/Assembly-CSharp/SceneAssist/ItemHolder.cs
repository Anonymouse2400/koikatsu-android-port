using System.Collections.Generic;
using UnityEngine;

namespace SceneAssist
{
	public class ItemHolder : MonoBehaviour
	{
		public List<GameObject> listItem = new List<GameObject>();

		public ChaControl CharFemale { get; private set; }

		public AnimatorStateInfo NowState
		{
			get
			{
				return CharFemale.getAnimatorStateInfo(0);
			}
		}

		public void PlayAnime(string _name, int _layer = 0)
		{
		}

		public bool LoadItem(string _asset, string _file, string _parent)
		{
			return true;
		}

		public void ReleaseItem(string _name)
		{
			int num = listItem.FindIndex((GameObject o) => o.name == _name);
			if (num >= 0)
			{
				GameObject obj = listItem[num];
				Object.Destroy(obj);
				listItem.RemoveAt(num);
			}
		}

		public void ReleaseAllItem()
		{
			if (listItem == null)
			{
				return;
			}
			for (int i = 0; i < listItem.Count; i++)
			{
				if (listItem[i] != null)
				{
					Object.Destroy(listItem[i]);
				}
			}
			listItem.Clear();
		}

		public void SetVisible(bool _visible)
		{
			for (int i = 0; i < listItem.Count; i++)
			{
				if (!(listItem[i] == null) && listItem[i].activeSelf != _visible)
				{
					listItem[i].SetActive(_visible);
				}
			}
		}

		private void Awake()
		{
			CharFemale = GetComponent<ChaControl>();
		}

		private void OnDestroy()
		{
			ReleaseAllItem();
		}
	}
}
