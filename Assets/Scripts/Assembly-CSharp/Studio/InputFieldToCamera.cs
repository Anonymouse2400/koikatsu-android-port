using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	public class InputFieldToCamera : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IEventSystemHandler
	{
		[SerializeField]
		private InputField inputField;

		[SerializeField]
		private TMP_InputField inputFieldTMP;

		[SerializeField]
		private Canvas m_Canvas;

		private Canvas canvas
		{
			get
			{
				if (m_Canvas == null)
				{
					m_Canvas = GetComponentInParent<Canvas>();
				}
				return m_Canvas;
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			Singleton<Studio>.Instance.DeselectInputField(inputField, inputFieldTMP);
		}

		public void OnSelect(BaseEventData eventData)
		{
			Singleton<Studio>.Instance.SelectInputField(inputField, inputFieldTMP);
			SortCanvas.select = canvas;
		}

		public void OnSubmit(BaseEventData eventData)
		{
			Singleton<Studio>.Instance.SelectInputField(inputField, inputFieldTMP);
		}
	}
}
