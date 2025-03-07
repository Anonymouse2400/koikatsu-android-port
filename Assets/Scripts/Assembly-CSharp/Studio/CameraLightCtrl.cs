using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class CameraLightCtrl : MonoBehaviour
	{
		public class LightInfo
		{
			public Color color = Color.white;

			public float intensity = 1f;

			public float[] rot = new float[2];

			public bool shadow = true;

			public virtual void Init()
			{
				color = Utility.ConvertColor(255, 255, 255);
				intensity = 1f;
				rot[0] = 0f;
				rot[1] = 0f;
				shadow = true;
			}

			public virtual void Save(BinaryWriter _writer, Version _version)
			{
				_writer.Write(JsonUtility.ToJson(color));
				_writer.Write(intensity);
				_writer.Write(rot[0]);
				_writer.Write(rot[1]);
				_writer.Write(shadow);
			}

			public virtual void Load(BinaryReader _reader, Version _version)
			{
				color = JsonUtility.FromJson<Color>(_reader.ReadString());
				intensity = _reader.ReadSingle();
				rot[0] = _reader.ReadSingle();
				rot[1] = _reader.ReadSingle();
				shadow = _reader.ReadBoolean();
			}
		}

		public class MapLightInfo : LightInfo
		{
			public LightType type = LightType.Directional;

			public override void Init()
			{
				base.Init();
				type = LightType.Directional;
			}

			public override void Save(BinaryWriter _writer, Version _version)
			{
				base.Save(_writer, _version);
				_writer.Write((int)type);
			}

			public override void Load(BinaryReader _reader, Version _version)
			{
				base.Load(_reader, _version);
				type = (LightType)_reader.ReadInt32();
			}
		}

		[Serializable]
		private class LightCalc
		{
			public Light light;

			public Transform transRoot;

			public Button buttonColor;

			public Toggle toggleShadow;

			public Slider sliderIntensity;

			public InputField inputIntensity;

			public Button buttonIntensity;

			public Slider[] sliderAxis;

			public InputField[] inputAxis;

			public Button[] buttonAxis;

			private bool isUpdateInfo;

			public bool isInit { get; private set; }

			public void Init()
			{
				if (isInit)
				{
					return;
				}
				buttonColor.onClick.AddListener(OnClickColor);
				toggleShadow.onValueChanged.AddListener(OnValueChangeShadow);
				sliderIntensity.onValueChanged.AddListener(OnValueChangeIntensity);
				inputIntensity.onEndEdit.AddListener(OnEndEditIntensity);
				buttonIntensity.onClick.AddListener(OnClickIntensity);
				for (int i = 0; i < 2; i++)
				{
					int axis = i;
					sliderAxis[axis].onValueChanged.AddListener(delegate(float f)
					{
						OnValueChangeAxis(f, axis);
					});
					inputAxis[axis].onEndEdit.AddListener(delegate(string s)
					{
						OnEndEditAxis(s, axis);
					});
					buttonAxis[axis].onClick.AddListener(delegate
					{
						OnClickAxis(axis);
					});
				}
				Reflect();
				isInit = true;
			}

			public void UpdateUI()
			{
				isUpdateInfo = true;
				buttonColor.image.color = Singleton<Studio>.Instance.sceneInfo.charaLight.color;
				sliderIntensity.value = Singleton<Studio>.Instance.sceneInfo.charaLight.intensity;
				inputIntensity.text = Singleton<Studio>.Instance.sceneInfo.charaLight.intensity.ToString("0.00");
				for (int i = 0; i < 2; i++)
				{
					sliderAxis[i].value = Singleton<Studio>.Instance.sceneInfo.charaLight.rot[i];
					inputAxis[i].text = Singleton<Studio>.Instance.sceneInfo.charaLight.rot[i].ToString("000");
				}
				toggleShadow.isOn = Singleton<Studio>.Instance.sceneInfo.charaLight.shadow;
				isUpdateInfo = false;
			}

			public void Reflect()
			{
				light.color = Singleton<Studio>.Instance.sceneInfo.charaLight.color;
				light.intensity = Singleton<Studio>.Instance.sceneInfo.charaLight.intensity;
				transRoot.localRotation = Quaternion.Euler(Singleton<Studio>.Instance.sceneInfo.charaLight.rot[0], Singleton<Studio>.Instance.sceneInfo.charaLight.rot[1], 0f);
				light.shadows = (Singleton<Studio>.Instance.sceneInfo.charaLight.shadow ? LightShadows.Soft : LightShadows.None);
			}

			private void OnClickColor()
			{
				Singleton<Studio>.Instance.colorPalette.Setup("キャラライト", Singleton<Studio>.Instance.sceneInfo.charaLight.color, OnValueChangeColor, false);
				Singleton<Studio>.Instance.colorPalette.visible = true;
			}

			private void OnValueChangeColor(Color _color)
			{
				buttonColor.image.color = _color;
				Singleton<Studio>.Instance.sceneInfo.charaLight.color = _color;
				Reflect();
			}

			private void OnValueChangeShadow(bool _value)
			{
				if (!isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.charaLight.shadow = _value;
					Reflect();
				}
			}

			private void OnValueChangeIntensity(float _value)
			{
				if (!isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.charaLight.intensity = _value;
					inputIntensity.text = _value.ToString("0.00");
					Reflect();
				}
			}

			private void OnEndEditIntensity(string _text)
			{
				float num = Mathf.Clamp(Utility.StringToFloat(_text), 0.1f, 2f);
				Singleton<Studio>.Instance.sceneInfo.charaLight.intensity = num;
				sliderIntensity.value = num;
				Reflect();
			}

			private void OnClickIntensity()
			{
				Singleton<Studio>.Instance.sceneInfo.charaLight.intensity = 1f;
				sliderIntensity.value = 1f;
				inputIntensity.text = Singleton<Studio>.Instance.sceneInfo.charaLight.intensity.ToString("0.00");
				Reflect();
			}

			private void OnValueChangeAxis(float _value, int _axis)
			{
				if (!isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.charaLight.rot[_axis] = _value;
					inputAxis[_axis].text = _value.ToString("000");
					Reflect();
				}
			}

			private void OnEndEditAxis(string _text, int _axis)
			{
				float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 359f);
				Singleton<Studio>.Instance.sceneInfo.charaLight.rot[_axis] = num;
				sliderAxis[_axis].value = num;
				Reflect();
			}

			private void OnClickAxis(int _axis)
			{
				Singleton<Studio>.Instance.sceneInfo.charaLight.rot[_axis] = 0f;
				sliderAxis[_axis].value = 0f;
				inputAxis[_axis].text = Singleton<Studio>.Instance.sceneInfo.charaLight.rot[_axis].ToString("000");
				Reflect();
			}
		}

		[SerializeField]
		private LightCalc lightChara = new LightCalc();

		public void Init()
		{
			lightChara.Init();
		}

		public void Reflect()
		{
			lightChara.Reflect();
		}

		private void OnEnable()
		{
			lightChara.UpdateUI();
		}

		private void Start()
		{
			Init();
		}
	}
}
