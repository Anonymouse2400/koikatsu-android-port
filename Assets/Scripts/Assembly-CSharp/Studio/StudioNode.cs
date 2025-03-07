using SceneAssist;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Studio
{
	public class StudioNode : PointerAction
	{
		protected enum ClickSound
		{
			NoSound = 0,
			OK = 1
		}

		[SerializeField]
		protected Button m_Button;

		[SerializeField]
		protected Image m_ImageButton;

		[SerializeField]
		protected Text m_Text;

		[SerializeField]
		private TextMeshProUGUI _textMesh;

		[SerializeField]
		protected ClickSound clickSound;

		protected bool m_Select;

		public Button buttonUI
		{
			get
			{
				return m_Button;
			}
		}

		public Image imageButton
		{
			get
			{
				if (m_ImageButton == null)
				{
					m_ImageButton = m_Button.image;
				}
				return m_ImageButton;
			}
		}

		public Text textUI
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
				if ((bool)m_Text)
				{
					m_Text.text = value;
				}
				if ((bool)_textMesh)
				{
					_textMesh.text = value;
				}
			}
		}

		public Color textColor
		{
			set
			{
				if ((bool)m_Text)
				{
					m_Text.color = value;
				}
				if ((bool)_textMesh)
				{
					_textMesh.color = value;
				}
			}
		}

		public bool select
		{
			get
			{
				return m_Select;
			}
			set
			{
				if (Utility.SetStruct(ref m_Select, value))
				{
					imageButton.color = ((!m_Select) ? Color.white : Color.green);
				}
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
