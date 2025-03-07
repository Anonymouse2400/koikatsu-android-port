using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Studio
{
	public class VoicePlayNode : VoiceNode
	{
		[SerializeField]
		private Button buttonDelete;

		private Image m_ImageButton;

		private bool m_Select;

		public UnityAction addOnClickDelete
		{
			set
			{
				buttonDelete.onClick.AddListener(value);
			}
		}

		private Image image
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
					image.color = ((!m_Select) ? Color.white : Color.green);
				}
			}
		}

		public void Destroy()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
