using Config;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ConfigCtrl : MonoBehaviour
	{
		[SerializeField]
		private Button buttonColor;

		[SerializeField]
		private Button buttonMobColor;

		[SerializeField]
		private Toggle toggleForegroundEyebrow;

		[SerializeField]
		private Toggle toggleForegroundEyes;

		[SerializeField]
		private Toggle toggleShield;

		[SerializeField]
		private Toggle[] toggleSound;

		[SerializeField]
		private Slider[] sliderSound;

		private SoundData[] soundData { get; set; }

		private void OnClickColor()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("背景色"))
			{
				Singleton<Studio>.Instance.colorPalette.visible = false;
			}
			else
			{
				Singleton<Studio>.Instance.colorPalette.Setup("背景色", Manager.Config.EtcData.BackColor, OnValueChangeColor, false);
			}
		}

		private void OnClickMobColor()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("モブ色"))
			{
				Singleton<Studio>.Instance.colorPalette.visible = false;
			}
			else
			{
				Singleton<Studio>.Instance.colorPalette.Setup("モブ色", Manager.Config.AddData.mobColor, OnValueChangeMobColor, true);
			}
		}

		private void OnValueChangeColor(Color _color)
		{
			Manager.Config.EtcData.BackColor = _color;
			buttonColor.image.color = _color;
			Camera.main.backgroundColor = _color;
		}

		private void OnValueChangeMobColor(Color _color)
		{
			Manager.Config.AddData.mobColor = _color;
			buttonMobColor.image.color = _color;
			Singleton<Map>.Instance.ReflectMobColor();
		}

		private void OnValueChangedMute(bool _value, int _idx)
		{
			soundData[_idx].Switch = _value;
			sliderSound[_idx].interactable = _value;
		}

		private void OnValueChangedVolume(float _value, int _idx)
		{
			soundData[_idx].Volume = Mathf.FloorToInt(_value);
		}

		private void Start()
		{
			soundData = new SoundData[6]
			{
				Manager.Config.SoundData.Master,
				Manager.Config.SoundData.BGM,
				Manager.Config.SoundData.GameSE,
				Manager.Config.SoundData.SystemSE,
				Manager.Config.SoundData.ENV,
				Singleton<Voice>.Instance._Config.PCM
			};
			buttonColor.image.color = Manager.Config.EtcData.BackColor;
			buttonMobColor.image.color = Manager.Config.AddData.mobColor;
			toggleShield.isOn = Manager.Config.EtcData.Shield;
			toggleForegroundEyebrow.isOn = Manager.Config.EtcData.ForegroundEyebrow;
			toggleForegroundEyes.isOn = Manager.Config.EtcData.ForegroundEyes;
			for (int i = 0; i < 6; i++)
			{
				toggleSound[i].isOn = soundData[i].Switch;
				sliderSound[i].interactable = soundData[i].Switch;
				sliderSound[i].value = soundData[i].Volume;
			}
			buttonColor.onClick.AddListener(OnClickColor);
			buttonMobColor.onClick.AddListener(OnClickMobColor);
			toggleShield.onValueChanged.AddListener(delegate(bool v)
			{
				Manager.Config.EtcData.Shield = v;
				Singleton<Studio>.Instance.cameraCtrl.isConfigVanish = v;
			});
			toggleForegroundEyebrow.onValueChanged.AddListener(delegate(bool _b)
			{
				Manager.Config.EtcData.ForegroundEyebrow = _b;
			});
			toggleForegroundEyes.onValueChanged.AddListener(delegate(bool _b)
			{
				Manager.Config.EtcData.ForegroundEyes = _b;
			});
			for (int j = 0; j < 6; j++)
			{
				int no = j;
				toggleSound[j].onValueChanged.AddListener(delegate(bool v)
				{
					OnValueChangedMute(v, no);
				});
				sliderSound[j].onValueChanged.AddListener(delegate(float v)
				{
					OnValueChangedVolume(v, no);
				});
			}
		}
	}
}
