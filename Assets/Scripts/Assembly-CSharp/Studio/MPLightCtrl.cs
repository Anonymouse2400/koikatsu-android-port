using System;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MPLightCtrl : MonoBehaviour
	{
		[Serializable]
		private class BackgroundInfo
		{
			public GameObject obj;

			public Sprite[] sprit;

			public Image image;

			public bool active
			{
				set
				{
					if (obj.activeSelf != value)
					{
						obj.SetActive(value);
					}
				}
			}

			public Info.LightLoadInfo.Target target
			{
				set
				{
					image.sprite = sprit[(int)value];
				}
			}
		}

		[Serializable]
		private class ValueInfo
		{
			public GameObject obj;

			public Slider slider;

			public InputField inputField;

			public Button button;

			public bool active
			{
				set
				{
					if (obj.activeSelf != value)
					{
						obj.SetActive(value);
					}
				}
			}
		}

		[SerializeField]
		private BackgroundInfo backgroundInfoDirectional = new BackgroundInfo();

		[SerializeField]
		private BackgroundInfo backgroundInfoPoint = new BackgroundInfo();

		[SerializeField]
		private BackgroundInfo backgroundInfoSpot = new BackgroundInfo();

		[SerializeField]
		private Image imageSample;

		[SerializeField]
		private Button buttonSample;

		[SerializeField]
		private Toggle toggleVisible;

		[SerializeField]
		private Toggle toggleTarget;

		[SerializeField]
		private Toggle toggleShadow;

		[SerializeField]
		private ValueInfo viIntensity;

		[SerializeField]
		private ValueInfo viRange;

		[SerializeField]
		private ValueInfo viSpotAngle;

		private OCILight m_OCILight;

		private bool isUpdateInfo;

		private bool isColorFunc;

		public OCILight ociLight
		{
			get
			{
				return m_OCILight;
			}
			set
			{
				m_OCILight = value;
				if (m_OCILight != null)
				{
					UpdateInfo();
				}
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
				base.gameObject.SetActive(value);
				if (!base.gameObject.activeSelf)
				{
					if (isColorFunc)
					{
						Singleton<Studio>.Instance.colorPalette.Close();
					}
					isColorFunc = false;
				}
			}
		}

		public bool Deselect(OCILight _ociLight)
		{
			if (m_OCILight != _ociLight)
			{
				return false;
			}
			ociLight = null;
			active = false;
			return true;
		}

		private void UpdateInfo()
		{
			isUpdateInfo = true;
			if (m_OCILight != null)
			{
				switch (m_OCILight.lightType)
				{
				case LightType.Point:
					viRange.slider.minValue = 0.1f;
					viRange.slider.maxValue = 100f;
					break;
				case LightType.Spot:
					viRange.slider.minValue = 0.5f;
					viRange.slider.maxValue = 100f;
					break;
				}
				toggleVisible.isOn = m_OCILight.lightInfo.enable;
				toggleTarget.isOn = m_OCILight.lightInfo.drawTarget;
				toggleShadow.isOn = m_OCILight.lightInfo.shadow;
				if ((bool)imageSample)
				{
					imageSample.color = m_OCILight.lightInfo.color;
				}
				viIntensity.slider.value = m_OCILight.lightInfo.intensity;
				viIntensity.inputField.text = m_OCILight.lightInfo.intensity.ToString("0.000");
				viRange.slider.value = m_OCILight.lightInfo.range;
				viRange.inputField.text = m_OCILight.lightInfo.range.ToString("0.000");
				viSpotAngle.slider.value = m_OCILight.lightInfo.spotAngle;
				viSpotAngle.inputField.text = m_OCILight.lightInfo.spotAngle.ToString("0.000");
				switch (m_OCILight.lightType)
				{
				case LightType.Directional:
					backgroundInfoDirectional.active = true;
					backgroundInfoDirectional.target = m_OCILight.lightTarget;
					backgroundInfoPoint.active = false;
					backgroundInfoSpot.active = false;
					viRange.active = false;
					viSpotAngle.active = false;
					break;
				case LightType.Point:
					backgroundInfoDirectional.active = false;
					backgroundInfoPoint.active = true;
					backgroundInfoPoint.target = m_OCILight.lightTarget;
					backgroundInfoSpot.active = false;
					viRange.active = true;
					viSpotAngle.active = false;
					break;
				case LightType.Spot:
					backgroundInfoDirectional.active = false;
					backgroundInfoPoint.active = false;
					backgroundInfoSpot.active = true;
					backgroundInfoSpot.target = m_OCILight.lightTarget;
					viRange.active = true;
					viSpotAngle.active = true;
					break;
				}
				isUpdateInfo = false;
			}
		}

		private void OnClickColor()
		{
			Singleton<Studio>.Instance.colorPalette.Setup("ライト", m_OCILight.lightInfo.color, OnValueChangeColor, false);
			isColorFunc = true;
			Singleton<Studio>.Instance.colorPalette.visible = true;
		}

		private void OnValueChangeColor(Color _color)
		{
			if (m_OCILight != null)
			{
				m_OCILight.SetColor(_color);
			}
			if ((bool)imageSample)
			{
				imageSample.color = _color;
			}
		}

		private void OnValueChangeEnable(bool _value)
		{
			m_OCILight.SetEnable(_value);
		}

		private void OnValueChangeDrawTarget(bool _value)
		{
			m_OCILight.SetDrawTarget(_value);
		}

		private void OnValueChangeShadow(bool _value)
		{
			m_OCILight.SetShadow(_value);
		}

		private void OnValueChangeIntensity(float _value)
		{
			if (!isUpdateInfo && m_OCILight.SetIntensity(Mathf.Clamp(_value, 0.1f, 2f)))
			{
				viIntensity.inputField.text = m_OCILight.lightInfo.intensity.ToString("0.00");
			}
		}

		private void OnEndEditIntensity(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0.1f, 2f);
			m_OCILight.SetIntensity(value);
			viIntensity.inputField.text = m_OCILight.lightInfo.intensity.ToString("0.00");
			viIntensity.slider.value = m_OCILight.lightInfo.intensity;
		}

		private void OnClickIntensity()
		{
			if (m_OCILight.SetIntensity(1f))
			{
				viIntensity.inputField.text = m_OCILight.lightInfo.intensity.ToString("0.00");
				viIntensity.slider.value = m_OCILight.lightInfo.intensity;
			}
		}

		private void OnValueChangeRange(float _value)
		{
			if (!isUpdateInfo && m_OCILight.SetRange(_value))
			{
				viRange.inputField.text = m_OCILight.lightInfo.range.ToString("0.000");
			}
		}

		private void OnEndEditRange(string _text)
		{
			float value = Mathf.Max((m_OCILight.lightType != 0) ? 0.1f : 0.5f, Utility.StringToFloat(_text));
			m_OCILight.SetRange(value);
			viRange.inputField.text = m_OCILight.lightInfo.range.ToString("0.000");
			viRange.slider.value = m_OCILight.lightInfo.range;
		}

		private void OnClickRange()
		{
			if (m_OCILight.SetRange(30f))
			{
				viRange.inputField.text = m_OCILight.lightInfo.range.ToString("0.000");
				viRange.slider.value = m_OCILight.lightInfo.range;
			}
		}

		private void OnValueChangeSpotAngle(float _value)
		{
			if (!isUpdateInfo && m_OCILight.SetSpotAngle(_value))
			{
				viSpotAngle.inputField.text = m_OCILight.lightInfo.spotAngle.ToString("0.000");
			}
		}

		private void OnEndEditSpotAngle(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 1f, 179f);
			m_OCILight.SetSpotAngle(value);
			viSpotAngle.inputField.text = m_OCILight.lightInfo.spotAngle.ToString("0.000");
			viSpotAngle.slider.value = m_OCILight.lightInfo.spotAngle;
		}

		private void OnClickSpotAngle()
		{
			if (m_OCILight.SetSpotAngle(30f))
			{
				viSpotAngle.inputField.text = m_OCILight.lightInfo.spotAngle.ToString("0.000");
				viSpotAngle.slider.value = m_OCILight.lightInfo.spotAngle;
			}
		}

		private void Awake()
		{
			buttonSample.onClick.AddListener(OnClickColor);
			toggleVisible.onValueChanged.AddListener(OnValueChangeEnable);
			toggleTarget.onValueChanged.AddListener(OnValueChangeDrawTarget);
			toggleShadow.onValueChanged.AddListener(OnValueChangeShadow);
			viIntensity.slider.onValueChanged.AddListener(OnValueChangeIntensity);
			viIntensity.inputField.onEndEdit.AddListener(OnEndEditIntensity);
			viIntensity.button.onClick.AddListener(OnClickIntensity);
			viRange.slider.onValueChanged.AddListener(OnValueChangeRange);
			viRange.inputField.onEndEdit.AddListener(OnEndEditRange);
			viRange.button.onClick.AddListener(OnClickRange);
			viSpotAngle.slider.onValueChanged.AddListener(OnValueChangeSpotAngle);
			viSpotAngle.inputField.onEndEdit.AddListener(OnEndEditSpotAngle);
			viSpotAngle.button.onClick.AddListener(OnClickSpotAngle);
			isUpdateInfo = false;
		}
	}
}
