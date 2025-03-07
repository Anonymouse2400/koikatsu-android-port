using SceneAssist;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Studio
{
	public class VoiceNode : PointerAction
	{
		protected enum ClickSound
		{
			NoSound = 0,
			OK = 1
		}

		[SerializeField]
		protected Button m_Button;

		[SerializeField]
		protected TextMeshProUGUI m_Text;

		[SerializeField]
		protected ClickSound clickSound;

		public Button buttonUI
		{
			get
			{
				return m_Button;
			}
		}

		public TextMeshProUGUI textUI
		{
			get
			{
				return m_Text;
			}
		}

		public string text
		{
			get
			{
				return m_Text.text;
			}
			set
			{
				m_Text.text = value;
			}
		}

		public bool interactable
		{
			get
			{
				return m_Button.interactable;
			}
			set
			{
				m_Button.interactable = value;
			}
		}

		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				if (base.gameObject.activeSelf != value)
				{
					base.gameObject.SetActive(value);
				}
			}
		}

		public UnityAction addOnClick
		{
			set
			{
				m_Button.onClick.AddListener(value);
			}
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (interactable)
			{
				base.OnPointerEnter(eventData);
			}
		}

		public virtual void Awake()
		{
			ClickSound clickSound = this.clickSound;
			if (clickSound == ClickSound.OK)
			{
				addOnClick = delegate
				{
					Assist.PlayDecisionSE();
				};
			}
		}
	}
}
