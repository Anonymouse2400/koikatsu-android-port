using UnityEngine;

namespace Studio
{
	[AddComponentMenu("Studio/Manager/Input Field", 1000)]
	public class InputFieldManager : Singleton<InputFieldManager>
	{
		private StudioInputField m_StudioInputField;

		public static StudioInputField studioInputField
		{
			set
			{
				if (Singleton<InputFieldManager>.IsInstance())
				{
					Singleton<InputFieldManager>.Instance.m_StudioInputField = value;
					Singleton<InputFieldManager>.Instance.enabled = value != null;
				}
			}
		}

		public static bool isFocused
		{
			get
			{
				return Singleton<InputFieldManager>.IsInstance() && (bool)Singleton<InputFieldManager>.Instance.m_StudioInputField && Singleton<InputFieldManager>.Instance.m_StudioInputField.isFocused;
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
				studioInputField = null;
			}
		}

		private void Update()
		{
			if ((bool)m_StudioInputField && m_StudioInputField.isFocused && Input.anyKey && !Input.GetMouseButton(0))
			{
				Input.ResetInputAxes();
			}
		}
	}
}
