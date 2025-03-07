using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class OutsideSoundControl : MonoBehaviour
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

		private List<string> listPath = new List<string>();

		private Dictionary<int, StudioNode> dicNode = new Dictionary<int, StudioNode>();

		private int select = -1;

		private void OnClickRepeat()
		{
			Singleton<Studio>.Instance.outsideSoundCtrl.repeat = ((Singleton<Studio>.Instance.outsideSoundCtrl.repeat == BGMCtrl.Repeat.None) ? BGMCtrl.Repeat.All : BGMCtrl.Repeat.None);
			buttonRepeat.image.sprite = spriteRepeat[(Singleton<Studio>.Instance.outsideSoundCtrl.repeat != 0) ? 1 : 0];
		}

		private void OnClickStop()
		{
			Singleton<Studio>.Instance.outsideSoundCtrl.Stop();
		}

		private void OnClickPlay()
		{
			Singleton<Studio>.Instance.outsideSoundCtrl.Play();
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
			if (dicNode.TryGetValue(select, out value))
			{
				value.select = false;
			}
			select = _idx;
			if (select != -1)
			{
				Singleton<Studio>.Instance.outsideSoundCtrl.fileName = listPath[_idx];
			}
			if (dicNode.TryGetValue(select, out value))
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
			listPath = Directory.GetFiles(UserData.Create("audio"), "*.wav").Select(Path.GetFileName).ToList();
			int count = listPath.Count;
			for (int j = 0; j < count; j++)
			{
				GameObject gameObject = Object.Instantiate(objectNode);
				gameObject.transform.SetParent(transformRoot, false);
				StudioNode component = gameObject.GetComponent<StudioNode>();
				component.active = true;
				int idx = j;
				component.addOnClick = delegate
				{
					OnClickSelect(idx);
				};
				component.text = Path.GetFileNameWithoutExtension(listPath[j]);
				dicNode.Add(idx, component);
			}
			select = listPath.FindIndex((string v) => v == Singleton<Studio>.Instance.outsideSoundCtrl.fileName);
			StudioNode value = null;
			if (dicNode.TryGetValue(select, out value))
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
			buttonRepeat.image.sprite = spriteRepeat[(Singleton<Studio>.Instance.outsideSoundCtrl.repeat != 0) ? 1 : 0];
		}

		private void Update()
		{
			imagePlayNow.enabled = Singleton<Studio>.Instance.outsideSoundCtrl.play;
		}
	}
}
