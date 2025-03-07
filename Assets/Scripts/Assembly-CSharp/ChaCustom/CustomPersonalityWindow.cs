using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomPersonalityWindow : MonoBehaviour
	{
		[HideInInspector]
		public CanvasGroup[] canvasGrp;

		[HideInInspector]
		public Image[] imgRaycast;

		[SerializeField]
		private GameObject objTop;

		[SerializeField]
		private GameObject objTemp;

		private Toggle[] tglPersonality;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private Toggle tglReference;

		[SerializeField]
		private CvsChara cvsChara;

		private bool updateWin;

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileParameter param
		{
			get
			{
				return chaCtrl.chaFile.parameter;
			}
		}

		public void UpdateUI()
		{
			updateWin = true;
			int[] array = customBase.dictPersonality.Keys.ToArray();
			int num = Array.IndexOf(array, param.personality);
			for (int i = 0; i < tglPersonality.Length; i++)
			{
				tglPersonality[i].isOn = i == num;
			}
			updateWin = false;
		}

		private void CreateWindow()
		{
			List<Image> list = new List<Image>();
			tglPersonality = new Toggle[customBase.dictPersonality.Keys.Count];
			foreach (var item in customBase.dictPersonality.Select((KeyValuePair<int, string> val, int idx) => new { val, idx }))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objTemp);
				gameObject.name = "tglRbSel_" + item.idx.ToString("00");
				tglPersonality[item.idx] = gameObject.GetComponent<Toggle>();
				Image component = gameObject.GetComponent<Image>();
				if ((bool)component)
				{
					list.Add(component);
				}
				ToggleGroup component2 = objTop.GetComponent<ToggleGroup>();
				tglPersonality[item.idx].group = component2;
				gameObject.transform.SetParent(objTop.transform, false);
				Transform transform = gameObject.transform.Find("textRbSelect");
				if (null != transform)
				{
					TextMeshProUGUI component3 = transform.GetComponent<TextMeshProUGUI>();
					if ((bool)component3)
					{
						component3.text = item.val.Value;
					}
				}
				gameObject.SetActiveIfDifferent(true);
			}
			imgRaycast = list.ToArray();
		}

		private void GetCanvasGroup()
		{
			List<CanvasGroup> list = new List<CanvasGroup>();
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (null != component)
			{
				list.Add(component);
			}
			Transform parent = base.transform.parent;
			if (null == parent)
			{
				return;
			}
			while (true)
			{
				component = parent.GetComponent<CanvasGroup>();
				if (null != component)
				{
					list.Add(component);
				}
				if (null == parent.parent)
				{
					break;
				}
				parent = parent.parent;
			}
			canvasGrp = list.ToArray();
		}

		public void ChangeRaycastTarget(bool enable)
		{
			Image[] array = imgRaycast;
			foreach (Image image in array)
			{
				image.raycastTarget = enable;
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			CreateWindow();
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					if ((bool)tglReference)
					{
						tglReference.isOn = false;
					}
				});
			}
			tglPersonality.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!updateWin && !customBase.updateCustomUI && isOn)
					{
						int[] array = customBase.dictPersonality.Keys.ToArray();
						cvsChara.ChangePersonailty(array[p.index], p.index);
						cvsChara.PlayVoice();
					}
				});
			});
			GetCanvasGroup();
		}

		private void Update()
		{
			if (canvasGrp == null || imgRaycast == null)
			{
				return;
			}
			bool enable = true;
			CanvasGroup[] array = canvasGrp;
			foreach (CanvasGroup canvasGroup in array)
			{
				if (!canvasGroup.blocksRaycasts)
				{
					enable = false;
					break;
				}
			}
			ChangeRaycastTarget(enable);
		}
	}
}
