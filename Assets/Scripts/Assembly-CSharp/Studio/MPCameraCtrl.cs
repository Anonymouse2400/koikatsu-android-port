using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MPCameraCtrl : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField inputName;

		[SerializeField]
		private Toggle toggleActive;

		private OCICamera m_OCICamera;

		private bool m_Active;

		private bool isUpdateInfo;

		public OCICamera ociCamera
		{
			get
			{
				return m_OCICamera;
			}
			set
			{
				m_OCICamera = value;
				UpdateInfo();
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
					base.gameObject.SetActive(m_OCICamera != null);
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
		}

		public bool Deselect(OCICamera _ociCamera)
		{
			if (m_OCICamera != _ociCamera)
			{
				return false;
			}
			ociCamera = null;
			active = false;
			return true;
		}

		public void UpdateInfo()
		{
			if (m_OCICamera != null)
			{
				isUpdateInfo = true;
				inputName.text = m_OCICamera.name;
				toggleActive.isOn = m_OCICamera.cameraInfo.active;
				isUpdateInfo = false;
			}
		}

		private void OnEndEditName(string _value)
		{
			if (!isUpdateInfo)
			{
				m_OCICamera.name = _value;
				Singleton<Studio>.Instance.cameraSelector.Init();
			}
		}

		private void OnValueChangedActive(bool _value)
		{
			if (!isUpdateInfo)
			{
				Singleton<Studio>.Instance.ChangeCamera(m_OCICamera, _value);
			}
		}

		private void Start()
		{
			inputName.onEndEdit.AddListener(OnEndEditName);
			toggleActive.onValueChanged.AddListener(OnValueChangedActive);
		}
	}
}
