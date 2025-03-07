using System.Collections.Generic;
using Illusion.Extensions;
using TMPro;
using UGUI_AssistLibrary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomPushListCtrl : MonoBehaviour
	{
		public delegate void OnPushItemFunc(int index);

		[HideInInspector]
		public CanvasGroup[] canvasGrp;

		[HideInInspector]
		public Image[] imgRaycast;

		[SerializeField]
		private TextMeshProUGUI textDrawName;

		[SerializeField]
		private GameObject objContent;

		[SerializeField]
		private GameObject objTemp;

		private List<CustomPushInfo> lstInfo = new List<CustomPushInfo>();

		public OnPushItemFunc onPushItemFunc;

		public void ClearList()
		{
			lstInfo.Clear();
		}

		public void AddList(int index, string name, string assetBundle, string assetName)
		{
			CustomPushInfo customPushInfo = new CustomPushInfo();
			customPushInfo.index = index;
			customPushInfo.name = name;
			customPushInfo.assetBundle = assetBundle;
			customPushInfo.assetName = assetName;
			lstInfo.Add(customPushInfo);
		}

		public void Create(OnPushItemFunc _onPushItemFunc)
		{
			onPushItemFunc = _onPushItemFunc;
			List<Image> list = new List<Image>();
			for (int num = objContent.transform.childCount - 1; num >= 0; num--)
			{
				Transform child = objContent.transform.GetChild(num);
				Object.Destroy(child.gameObject);
			}
			for (int i = 0; i < lstInfo.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(objTemp);
				CustomPushInfoComponent customPushInfoComponent = gameObject.AddComponent<CustomPushInfoComponent>();
				customPushInfoComponent.info = lstInfo[i];
				gameObject.transform.SetParent(objContent.transform, false);
				SetButtonHandler(gameObject);
				Image component = gameObject.GetComponent<Image>();
				if ((bool)component)
				{
					list.Add(component);
				}
				if ((bool)component)
				{
					Texture2D texture2D = CommonLib.LoadAsset<Texture2D>(lstInfo[i].assetBundle, lstInfo[i].assetName, false, string.Empty);
					if (null != texture2D)
					{
						component.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
					}
				}
				gameObject.SetActiveIfDifferent(true);
			}
			imgRaycast = list.ToArray();
		}

		public void OnPointerClick(GameObject obj)
		{
			if (!(null == obj))
			{
				CustomPushInfoComponent component = obj.GetComponent<CustomPushInfoComponent>();
				if (!(null == component) && onPushItemFunc != null)
				{
					onPushItemFunc(component.info.index);
				}
			}
		}

		public void OnPointerEnter(GameObject obj)
		{
			if (!(null == obj))
			{
				CustomPushInfoComponent component = obj.GetComponent<CustomPushInfoComponent>();
				if (!(null == component) && (bool)textDrawName)
				{
					textDrawName.text = component.info.name;
				}
			}
		}

		public void OnPointerExit()
		{
			if ((bool)textDrawName)
			{
				textDrawName.text = string.Empty;
			}
		}

		private void SetButtonHandler(GameObject obj)
		{
			UIAL_EventTrigger uIAL_EventTrigger = obj.AddComponent<UIAL_EventTrigger>();
			uIAL_EventTrigger.triggers = new List<UIAL_EventTrigger.Entry>();
			UIAL_EventTrigger.Entry entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.buttonType = UIAL_EventTrigger.ButtonType.Left;
			entry.callback.AddListener(delegate
			{
				OnPointerClick(obj);
			});
			uIAL_EventTrigger.triggers.Add(entry);
			if ((bool)textDrawName)
			{
				entry = new UIAL_EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener(delegate
				{
					OnPointerEnter(obj);
				});
				uIAL_EventTrigger.triggers.Add(entry);
				entry = new UIAL_EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener(delegate
				{
					OnPointerExit();
				});
				uIAL_EventTrigger.triggers.Add(entry);
			}
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

		private void Start()
		{
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
