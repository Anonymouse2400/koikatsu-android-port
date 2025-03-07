using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MPFolderCtrl : MonoBehaviour
	{
		[SerializeField]
		private InputField inputName;

		private OCIFolder m_OCIFolder;

		private bool m_Active;

		private bool isUpdateInfo;

		public OCIFolder ociFolder
		{
			get
			{
				return m_OCIFolder;
			}
			set
			{
				m_OCIFolder = value;
				if (m_OCIFolder != null)
				{
					UpdateInfo();
				}
			}
		}

		public bool active
		{
			get
			{
				return m_Active;
			}
			set
			{
				m_Active = value;
				if (m_Active)
				{
					base.gameObject.SetActive(m_OCIFolder != null);
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
		}

		public bool Deselect(OCIFolder _ociFolder)
		{
			if (m_OCIFolder != _ociFolder)
			{
				return false;
			}
			ociFolder = null;
			active = false;
			return true;
		}

		private void UpdateInfo()
		{
			if (m_OCIFolder != null)
			{
				isUpdateInfo = true;
				inputName.text = m_OCIFolder.name;
				isUpdateInfo = false;
			}
		}

		private void OnEndEditName(string _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIFolder.name = _value;
			}
		}

		private void Start()
		{
			inputName.onEndEdit.AddListener(OnEndEditName);
		}
	}
}
