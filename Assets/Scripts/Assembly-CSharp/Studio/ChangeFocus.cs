using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ChangeFocus : MonoBehaviour
	{
		[SerializeField]
		private Selectable[] selectable;

		private int m_Select = -1;

		public int select
		{
			get
			{
				return m_Select;
			}
			set
			{
				m_Select = value;
				base.enabled = m_Select != -1;
			}
		}

		private void ChangeTarget()
		{
			if (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift))
			{
				m_Select--;
				if (m_Select < 0)
				{
					m_Select = selectable.Length - 1;
				}
			}
			else
			{
				m_Select = (m_Select + 1) % selectable.Length;
			}
			if ((bool)selectable[m_Select])
			{
				selectable[m_Select].Select();
			}
		}

		private void Start()
		{
			select = -1;
			(from _ in this.UpdateAsObservable()
				where base.enabled
				where @select != -1
				where Input.GetKeyDown(KeyCode.Tab)
				select _).Subscribe(delegate
			{
				ChangeTarget();
			});
		}
	}
}
