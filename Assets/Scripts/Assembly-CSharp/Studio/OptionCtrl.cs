using System;
using Illusion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class OptionCtrl : MonoBehaviour
	{
		[Serializable]
		private class CommonInfo
		{
			public GameObject root;

			public Button button;

			private Sprite[] sprite;

			public bool active
			{
				get
				{
					return root.activeSelf;
				}
				set
				{
					if (root.SetActiveIfDifferent(value))
					{
						button.image.sprite = sprite[value ? 1 : 0];
					}
				}
			}

			protected bool isUpdateInfo { get; set; }

			public virtual void Init(Sprite[] _sprite)
			{
				button.onClick.AddListener(delegate
				{
					active = !active;
				});
				sprite = _sprite;
				isUpdateInfo = false;
			}

			public virtual void UpdateInfo()
			{
			}
		}

		[Serializable]
		public class InputCombination
		{
			public Slider slider;

			public TMP_InputField input;

			public bool interactable
			{
				set
				{
					input.interactable = value;
					slider.interactable = value;
				}
			}

			public string text
			{
				get
				{
					return input.text;
				}
				set
				{
					input.text = value;
					slider.value = Utility.StringToFloat(value);
				}
			}

			public float value
			{
				get
				{
					return slider.value;
				}
				set
				{
					slider.value = value;
					input.text = value.ToString("0.00");
				}
			}

			public float min
			{
				get
				{
					return slider.minValue;
				}
			}

			public float max
			{
				get
				{
					return slider.maxValue;
				}
			}
		}

		[Serializable]
		private class CharaFKColor : CommonInfo
		{
			public Button[] buttons;

			public Toggle toggleLine;

			public override void Init(Sprite[] _sprite)
			{
				base.Init(_sprite);
				for (int i = 0; i < buttons.Length; i++)
				{
					int no = i;
					buttons[i].onClick.AddListener(delegate
					{
						OnClickColor(no);
					});
				}
				toggleLine.onValueChanged.AddListener(OnValueChangedLine);
			}

			public override void UpdateInfo()
			{
				base.isUpdateInfo = true;
				buttons[0].image.color = Studio.optionSystem.colorFKHair;
				buttons[1].image.color = Studio.optionSystem.colorFKNeck;
				buttons[2].image.color = Studio.optionSystem.colorFKBreast;
				buttons[3].image.color = Studio.optionSystem.colorFKBody;
				buttons[4].image.color = Studio.optionSystem.colorFKRightHand;
				buttons[5].image.color = Studio.optionSystem.colorFKLeftHand;
				buttons[6].image.color = Studio.optionSystem.colorFKSkirt;
				toggleLine.isOn = Studio.optionSystem.lineFK;
				base.isUpdateInfo = false;
			}

			private void OnClickColor(int _idx)
			{
				string[] array = new string[7] { "髪", "首", "胸", "体", "右手", "左手", "スカート" };
				if (Singleton<Studio>.Instance.colorPalette.Check(string.Format("FKカラー {0}", array[_idx])))
				{
					Singleton<Studio>.Instance.colorPalette.visible = false;
					return;
				}
				Singleton<Studio>.Instance.colorPalette.Setup(string.Format("FKカラー {0}", array[_idx]), Studio.optionSystem.GetFKColor(_idx), delegate(Color _c)
				{
					SetColor(_idx, _c);
				}, false);
			}

			private void SetColor(int _idx, Color _color)
			{
				Studio.optionSystem.SetFKColor(_idx, _color);
				buttons[_idx].image.color = _color;
				Singleton<Studio>.Instance.UpdateCharaFKColor();
			}

			private void OnValueChangedLine(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Studio.optionSystem.lineFK = _value;
				}
			}
		}

		[Serializable]
		private class ItemFKColor : CommonInfo
		{
			public Button buttonColor;

			public override void Init(Sprite[] _sprite)
			{
				base.Init(_sprite);
				buttonColor.onClick.AddListener(OnClickColor);
			}

			public override void UpdateInfo()
			{
				base.isUpdateInfo = true;
				buttonColor.image.color = Studio.optionSystem.colorFKItem;
				base.isUpdateInfo = false;
			}

			private void OnClickColor()
			{
				if (Singleton<Studio>.Instance.colorPalette.Check("FKカラー アイテム"))
				{
					Singleton<Studio>.Instance.colorPalette.visible = false;
				}
				else
				{
					Singleton<Studio>.Instance.colorPalette.Setup("FKカラー アイテム", Studio.optionSystem.colorFKItem, SetColor, false);
				}
			}

			private void SetColor(Color _color)
			{
				Studio.optionSystem.colorFKItem = _color;
				buttonColor.image.color = _color;
				Singleton<Studio>.Instance.UpdateItemFKColor();
			}
		}

		[Serializable]
		private class RouteSystem : CommonInfo
		{
			public InputCombination inputWidth = new InputCombination();

			public Toggle[] toggleLimit;

			public override void Init(Sprite[] _sprite)
			{
				base.Init(_sprite);
				inputWidth.slider.onValueChanged.AddListener(OnValueChangedRouteWidth);
				inputWidth.input.onEndEdit.AddListener(OnEndEditRouteWidth);
				toggleLimit[0].onValueChanged.AddListener(delegate(bool _b)
				{
					if (_b)
					{
						Studio.optionSystem.routePointLimit = true;
					}
				});
				toggleLimit[1].onValueChanged.AddListener(delegate(bool _b)
				{
					if (_b)
					{
						Studio.optionSystem.routePointLimit = false;
					}
				});
			}

			public override void UpdateInfo()
			{
				base.isUpdateInfo = true;
				inputWidth.value = Studio.optionSystem._routeLineWidth;
				toggleLimit[(!Studio.optionSystem.routePointLimit) ? 1u : 0u].isOn = true;
				base.isUpdateInfo = false;
			}

			private void OnValueChangedRouteWidth(float _value)
			{
				Studio.optionSystem._routeLineWidth = _value;
				inputWidth.value = _value;
				Singleton<Studio>.Instance.routeControl.ReflectOption();
			}

			private void OnEndEditRouteWidth(string _text)
			{
				float num = Mathf.Clamp(Utility.StringToFloat(_text), inputWidth.min, inputWidth.max);
				Studio.optionSystem._routeLineWidth = num;
				inputWidth.value = num;
				Singleton<Studio>.Instance.routeControl.ReflectOption();
			}
		}

		[Serializable]
		private class Etc : CommonInfo
		{
			public Toggle[] toggleStartup;

			public override void Init(Sprite[] _sprite)
			{
				base.Init(_sprite);
				toggleStartup[0].onValueChanged.AddListener(delegate(bool _b)
				{
					if (_b)
					{
						Studio.optionSystem.startupLoad = true;
					}
				});
				toggleStartup[1].onValueChanged.AddListener(delegate(bool _b)
				{
					if (_b)
					{
						Studio.optionSystem.startupLoad = false;
					}
				});
			}

			public override void UpdateInfo()
			{
				base.isUpdateInfo = true;
				toggleStartup[(!Studio.optionSystem.startupLoad) ? 1u : 0u].isOn = true;
				base.isUpdateInfo = false;
			}
		}

		[SerializeField]
		private InputCombination inputCameraX = new InputCombination();

		[SerializeField]
		private InputCombination inputCameraY = new InputCombination();

		[SerializeField]
		private InputCombination inputCameraSpeed = new InputCombination();

		[SerializeField]
		private InputCombination _inputSize = new InputCombination();

		[SerializeField]
		private InputCombination inputSpeed = new InputCombination();

		[SerializeField]
		private Toggle[] toggleInitialPosition;

		[SerializeField]
		private Toggle[] toggleSelectedState;

		[SerializeField]
		private Toggle[] toggleAutoHide;

		[SerializeField]
		private Toggle[] toggleAutoSelect;

		[SerializeField]
		private Toggle[] toggleSnap;

		[SerializeField]
		private CameraControl cameraControl;

		[SerializeField]
		private Sprite[] spriteActive;

		[SerializeField]
		private CharaFKColor charaFKColor = new CharaFKColor();

		[SerializeField]
		private ItemFKColor itemFKColor = new ItemFKColor();

		[SerializeField]
		private RouteSystem routeSystem = new RouteSystem();

		[SerializeField]
		private Etc etc = new Etc();

		public InputCombination inputSize
		{
			get
			{
				return _inputSize;
			}
		}

		public void UpdateUI()
		{
			inputCameraX.value = Studio.optionSystem.cameraSpeedY;
			inputCameraY.value = Studio.optionSystem.cameraSpeedX;
			inputCameraSpeed.value = Studio.optionSystem.cameraSpeed;
			_inputSize.value = Studio.optionSystem.manipulateSize;
			inputSpeed.value = Studio.optionSystem.manipuleteSpeed;
			toggleInitialPosition[0].isOn = Studio.optionSystem.initialPosition == 0;
			toggleInitialPosition[1].isOn = Studio.optionSystem.initialPosition == 1;
			toggleSelectedState[0].isOn = Studio.optionSystem.selectedState == 0;
			toggleSelectedState[1].isOn = Studio.optionSystem.selectedState == 1;
			toggleAutoHide[0].isOn = !Studio.optionSystem.autoHide;
			toggleAutoHide[1].isOn = Studio.optionSystem.autoHide;
			toggleAutoSelect[0].isOn = Studio.optionSystem.autoSelect;
			toggleAutoSelect[1].isOn = !Studio.optionSystem.autoSelect;
			for (int i = 0; i < toggleSnap.Length; i++)
			{
				toggleSnap[i].isOn = Studio.optionSystem.snap == i;
			}
			charaFKColor.UpdateInfo();
			itemFKColor.UpdateInfo();
			routeSystem.UpdateInfo();
			etc.UpdateInfo();
		}

		public void UpdateUIManipulateSize()
		{
			_inputSize.value = Studio.optionSystem.manipulateSize;
		}

		private void OnValueChangedSelectedState(int _state)
		{
			Studio.optionSystem.selectedState = _state;
			cameraControl.ReflectOption();
		}

		private void OnValueChangedCameraX(float _value)
		{
			Studio.optionSystem.cameraSpeedY = _value;
			inputCameraX.value = _value;
			cameraControl.ReflectOption();
		}

		private void OnEndEditCameraX(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), inputCameraX.min, inputCameraX.max);
			Studio.optionSystem.cameraSpeedY = num;
			inputCameraX.value = num;
			cameraControl.ReflectOption();
		}

		private void OnValueChangedCameraY(float _value)
		{
			Studio.optionSystem.cameraSpeedX = _value;
			inputCameraY.value = _value;
			cameraControl.ReflectOption();
		}

		private void OnEndEditCameraY(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), inputCameraY.min, inputCameraY.max);
			Studio.optionSystem.cameraSpeedX = num;
			inputCameraY.value = num;
			cameraControl.ReflectOption();
		}

		private void OnValueChangedCameraSpeed(float _value)
		{
			Studio.optionSystem.cameraSpeed = _value;
			inputCameraSpeed.value = _value;
			cameraControl.ReflectOption();
		}

		private void OnEndEditCameraSpeed(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), inputCameraSpeed.min, inputCameraSpeed.max);
			Studio.optionSystem.cameraSpeed = num;
			inputCameraSpeed.value = num;
			cameraControl.ReflectOption();
		}

		private void OnValueChangedSize(float _value)
		{
			Studio.optionSystem.manipulateSize = _value;
			_inputSize.value = _value;
			Singleton<GuideObjectManager>.Instance.SetScale();
		}

		private void OnEndEditSize(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), _inputSize.min, _inputSize.max);
			Studio.optionSystem.manipulateSize = num;
			_inputSize.value = num;
			Singleton<GuideObjectManager>.Instance.SetScale();
		}

		private void OnValueChangedSpeed(float _value)
		{
			Studio.optionSystem.manipuleteSpeed = _value;
			inputSpeed.value = _value;
		}

		private void OnEndEditSpeed(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), inputSpeed.min, inputSpeed.max);
			Studio.optionSystem.manipuleteSpeed = num;
			inputSpeed.value = num;
		}

		private void Start()
		{
			UpdateUI();
			inputCameraX.slider.onValueChanged.AddListener(OnValueChangedCameraX);
			inputCameraX.input.onEndEdit.AddListener(OnEndEditCameraX);
			inputCameraY.slider.onValueChanged.AddListener(OnValueChangedCameraY);
			inputCameraY.input.onEndEdit.AddListener(OnEndEditCameraY);
			inputCameraSpeed.slider.onValueChanged.AddListener(OnValueChangedCameraSpeed);
			inputCameraSpeed.input.onEndEdit.AddListener(OnEndEditCameraSpeed);
			_inputSize.slider.onValueChanged.AddListener(OnValueChangedSize);
			_inputSize.input.onEndEdit.AddListener(OnEndEditSize);
			inputSpeed.slider.onValueChanged.AddListener(OnValueChangedSpeed);
			inputSpeed.input.onEndEdit.AddListener(OnEndEditSpeed);
			toggleInitialPosition[0].onValueChanged.AddListener(delegate
			{
				Studio.optionSystem.initialPosition = 0;
			});
			toggleInitialPosition[1].onValueChanged.AddListener(delegate
			{
				Studio.optionSystem.initialPosition = 1;
			});
			toggleSelectedState[0].onValueChanged.AddListener(delegate
			{
				OnValueChangedSelectedState(0);
			});
			toggleSelectedState[1].onValueChanged.AddListener(delegate
			{
				OnValueChangedSelectedState(1);
			});
			toggleAutoHide[0].onValueChanged.AddListener(delegate(bool v)
			{
				Studio.optionSystem.autoHide = !v;
			});
			toggleAutoHide[1].onValueChanged.AddListener(delegate(bool v)
			{
				Studio.optionSystem.autoHide = v;
			});
			toggleAutoSelect[0].onValueChanged.AddListener(delegate(bool v)
			{
				Studio.optionSystem.autoSelect = v;
			});
			toggleAutoSelect[1].onValueChanged.AddListener(delegate(bool v)
			{
				Studio.optionSystem.autoSelect = !v;
			});
			toggleSnap[0].onValueChanged.AddListener(delegate
			{
				Studio.optionSystem.snap = 0;
			});
			toggleSnap[1].onValueChanged.AddListener(delegate
			{
				Studio.optionSystem.snap = 1;
			});
			toggleSnap[2].onValueChanged.AddListener(delegate
			{
				Studio.optionSystem.snap = 2;
			});
			charaFKColor.Init(spriteActive);
			itemFKColor.Init(spriteActive);
			routeSystem.Init(spriteActive);
			etc.Init(spriteActive);
		}
	}
}
