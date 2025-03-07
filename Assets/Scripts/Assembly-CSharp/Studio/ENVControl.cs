using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ENVControl : MonoBehaviour
	{
		[SerializeField]
		private GameObject objectNode;

		[SerializeField]
		private Transform transformRoot;

		[SerializeField]
		private Button buttonRepeat;

		[SerializeField]
		private Sprite[] spriteRepeat;

		[SerializeField]
		private Button buttonStop;

		[SerializeField]
		private Button buttonPlay;

		[SerializeField]
		private Image imagePlayNow;

		[SerializeField]
		private Button buttonPause;

		[SerializeField]
		private Button buttonExpansion;

		[SerializeField]
		private Sprite[] spriteExpansion;

		[SerializeField]
		private GameObject objBottom;

		private Dictionary<int, StudioNode> dicNode = new Dictionary<int, StudioNode>();

		private void OnClickRepeat()
		{
			Singleton<Studio>.Instance.envCtrl.repeat = ((Singleton<Studio>.Instance.envCtrl.repeat == BGMCtrl.Repeat.None) ? BGMCtrl.Repeat.All : BGMCtrl.Repeat.None);
			buttonRepeat.image.sprite = spriteRepeat[(Singleton<Studio>.Instance.envCtrl.repeat != 0) ? 1 : 0];
		}

		private void OnClickStop()
		{
			Singleton<Studio>.Instance.envCtrl.Stop();
		}

		private void OnClickPlay()
		{
			Singleton<Studio>.Instance.envCtrl.Play();
		}

		private void OnClickPause()
		{
		}

		private void OnClickExpansion()
		{
			objBottom.SetActive(!objBottom.activeSelf);
			buttonExpansion.image.sprite = spriteExpansion[objBottom.activeSelf ? 1 : 0];
		}

		private void OnClickSelect(int _idx)
		{
			StudioNode value = null;
			if (dicNode.TryGetValue(Singleton<Studio>.Instance.envCtrl.no, out value))
			{
				value.select = false;
			}
			Singleton<Studio>.Instance.envCtrl.no = _idx;
			if (dicNode.TryGetValue(Singleton<Studio>.Instance.envCtrl.no, out value))
			{
				value.select = true;
			}
		}

		private void InitList()
		{
			for (int i = 0; i < transformRoot.childCount; i++)
			{
				Object.Destroy(transformRoot.GetChild(i).gameObject);
			}
			transformRoot.DetachChildren();
			foreach (KeyValuePair<int, Info.LoadCommonInfo> item in Singleton<Info>.Instance.dicENVLoadInfo)
			{
				GameObject gameObject = Object.Instantiate(objectNode);
				gameObject.transform.SetParent(transformRoot, false);
				StudioNode component = gameObject.GetComponent<StudioNode>();
				component.active = true;
				int idx = item.Key;
				component.addOnClick = delegate
				{
					OnClickSelect(idx);
				};
				component.text = item.Value.name;
				dicNode.Add(idx, component);
			}
			StudioNode value = null;
			if (dicNode.TryGetValue(Singleton<Studio>.Instance.envCtrl.no, out value))
			{
				value.select = true;
			}
		}

		private void Awake()
		{
			buttonRepeat.onClick.AddListener(OnClickRepeat);
			buttonStop.onClick.AddListener(OnClickStop);
			buttonPlay.onClick.AddListener(OnClickPlay);
			buttonPause.onClick.AddListener(OnClickPause);
			buttonExpansion.onClick.AddListener(OnClickExpansion);
			InitList();
		}

		private void OnEnable()
		{
			buttonRepeat.image.sprite = spriteRepeat[(Singleton<Studio>.Instance.envCtrl.repeat != 0) ? 1 : 0];
			foreach (KeyValuePair<int, StudioNode> item in dicNode)
			{
				item.Value.select = false;
			}
			StudioNode value = null;
			if (dicNode.TryGetValue(Singleton<Studio>.Instance.envCtrl.no, out value))
			{
				value.select = true;
			}
		}

		private void Update()
		{
			imagePlayNow.enabled = Singleton<Studio>.Instance.envCtrl.play;
		}
	}
}
