using System;
using System.Linq;
using Illusion.CustomAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Component.UI.ColorPicker
{
	public class PickerRect : MonoBehaviour
	{
		public enum Mode
		{
			Hue = 0,
			Saturation = 1,
			Value = 2,
			Red = 3,
			Green = 4,
			Blue = 5
		}

		public enum Control
		{
			None = 0,
			Rect = 1,
			Slider = 2
		}

		[Serializable]
		public class ModeReactiveProperty : ReactiveProperty<Mode>
		{
			public ModeReactiveProperty()
			{
			}

			public ModeReactiveProperty(Mode initialValue)
				: base(initialValue)
			{
			}
		}

		private float[] _values = new float[Utils.Enum<Mode>.Length];

		[SerializeField]
		protected ModeReactiveProperty _mode = new ModeReactiveProperty(Mode.Hue);

		[SerializeField]
		[NamedArray(typeof(Mode))]
		private Toggle[] modeChangeToggles = new Toggle[Utils.Enum<Mode>.Length];

		[SerializeField]
		public Info info;

		[SerializeField]
		private Slider slider;

		[SerializeField]
		private RectTransform pointer;

		private ImagePack[] imgPack;

		public virtual Color ColorRGB
		{
			get
			{
				float[] rGB = RGB;
				return new Color(rGB[0], rGB[1], rGB[2]);
			}
			set
			{
				RGB = new float[3]
				{
					value[0],
					value[1],
					value[2]
				};
			}
		}

		public virtual float[] RGB
		{
			get
			{
				return new float[3] { Red, Green, Blue };
			}
			set
			{
				Red = value[0];
				Green = value[1];
				Blue = value[2];
			}
		}

		public HsvColor ColorHSV
		{
			get
			{
				float[] hSV = HSV;
				return new HsvColor(hSV[0], hSV[1], hSV[2]);
			}
			set
			{
				HSV = new float[3]
				{
					value[0],
					value[1],
					value[2]
				};
			}
		}

		public float[] HSV
		{
			get
			{
				return new float[3] { Hue, Saturation, Value };
			}
			set
			{
				Hue = value[0];
				Saturation = value[1];
				Value = value[2];
			}
		}

		public float Hue
		{
			get
			{
				return _values[0];
			}
			set
			{
				_values[0] = value;
			}
		}

		public float Saturation
		{
			get
			{
				return _values[1];
			}
			set
			{
				_values[1] = value;
			}
		}

		public float Value
		{
			get
			{
				return _values[2];
			}
			set
			{
				_values[2] = value;
			}
		}

		public float Red
		{
			get
			{
				return _values[3];
			}
			set
			{
				_values[3] = value;
			}
		}

		public float Green
		{
			get
			{
				return _values[4];
			}
			set
			{
				_values[4] = value;
			}
		}

		public float Blue
		{
			get
			{
				return _values[5];
			}
			set
			{
				_values[5] = value;
			}
		}

		public Mode mode
		{
			get
			{
				return _mode.Value;
			}
			set
			{
				_mode.Value = value;
			}
		}

		private float[] RateHSV
		{
			get
			{
				return new float[3]
				{
					Mathf.InverseLerp(0f, 360f, Hue),
					Saturation,
					Value
				};
			}
		}

		public event Action<Color> updateColorAction;

		public void ChangeRectColor()
		{
			ImagePack imagePack = imgPack[0];
			if (imagePack == null || !imagePack.isTex)
			{
				return;
			}
			int num = (int)mode % 3;
			Vector2 size = imagePack.size;
			int num2 = (int)size.x;
			int num3 = (int)size.y;
			Color[] array = new Color[num3 * num2];
			float[] array2 = null;
			int[,] array3 = null;
			switch (mode)
			{
			case Mode.Hue:
			case Mode.Saturation:
			case Mode.Value:
			{
				array2 = RateHSV;
				array3 = new int[3, 3]
				{
					{ 1, 2, 0 },
					{ 0, 2, 1 },
					{ 0, 1, 2 }
				};
				for (int k = 0; k < num3; k++)
				{
					for (int l = 0; l < num2; l++)
					{
						array2[array3[num, 0]] = Mathf.InverseLerp(0f, size.x, l);
						array2[array3[num, 1]] = Mathf.InverseLerp(0f, size.y, k);
						array[k * num2 + l] = HsvColor.ToRgb(360f * array2[0], array2[1], array2[2]);
					}
				}
				break;
			}
			case Mode.Red:
			case Mode.Green:
			case Mode.Blue:
			{
				array2 = RGB;
				array3 = new int[3, 3]
				{
					{ 2, 1, 0 },
					{ 2, 0, 1 },
					{ 0, 1, 2 }
				};
				for (int i = 0; i < num3; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						array2[array3[num, 0]] = Mathf.InverseLerp(0f, size.x, j);
						array2[array3[num, 1]] = Mathf.InverseLerp(0f, size.y, i);
						array[i * num2 + j] = new Color(array2[0], array2[1], array2[2]);
					}
				}
				break;
			}
			}
			imagePack.SetPixels(array);
		}

		public void ChangeSliderColor()
		{
			ImagePack imagePack = imgPack[1];
			if (imagePack == null || !imagePack.isTex)
			{
				return;
			}
			int num = (int)mode % 3;
			Vector2 size = imagePack.size;
			int num2 = (int)size.x;
			int num3 = (int)size.y;
			Color[] array = new Color[num3 * num2];
			float[] array2 = null;
			switch (mode)
			{
			case Mode.Hue:
			case Mode.Saturation:
			case Mode.Value:
			{
				array2 = RateHSV;
				if (mode == Mode.Hue)
				{
					array2[1] = 1f;
					array2[2] = 1f;
				}
				for (int k = 0; k < num3; k++)
				{
					for (int l = 0; l < num2; l++)
					{
						array2[num] = Mathf.InverseLerp(0f, size.y, k);
						array[k * num2 + l] = HsvColor.ToRgb(array2[0] * 360f, array2[1], array2[2]);
					}
				}
				break;
			}
			case Mode.Red:
			case Mode.Green:
			case Mode.Blue:
			{
				array2 = RGB;
				for (int i = 0; i < num3; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						array2[num] = Mathf.InverseLerp(0f, size.y, i);
						array[i * num2 + j] = new Color(array2[0], array2[1], array2[2]);
					}
				}
				break;
			}
			}
			imagePack.SetPixels(array);
		}

		public void CalcRectPointer()
		{
			if (!(pointer == null))
			{
				Rect rect = imgPack[0].rectTransform.rect;
				Action<float[], int, int> action = delegate(float[] val, int x, int y)
				{
					pointer.anchoredPosition = new Vector2(rect.width * val[x], rect.height * val[y]);
				};
				switch (mode)
				{
				case Mode.Hue:
					action(RateHSV, 1, 2);
					break;
				case Mode.Saturation:
					action(RateHSV, 0, 2);
					break;
				case Mode.Value:
					action(RateHSV, 0, 1);
					break;
				case Mode.Red:
					action(RGB, 2, 1);
					break;
				case Mode.Green:
					action(RGB, 2, 0);
					break;
				case Mode.Blue:
					action(RGB, 0, 1);
					break;
				}
			}
		}

		public void CalcSliderValue()
		{
			if (!(slider == null))
			{
				switch (mode)
				{
				case Mode.Hue:
				case Mode.Saturation:
				case Mode.Value:
					slider.value = RateHSV[(int)mode];
					break;
				case Mode.Red:
				case Mode.Green:
				case Mode.Blue:
					slider.value = RGB[(int)mode % 3];
					break;
				}
			}
		}

		public virtual void SetColor(HsvColor hsv, Control ctrlType)
		{
			ColorHSV = hsv;
			ColorRGB = HsvColor.ToRgb(hsv);
			SetColor(ctrlType);
		}

		public virtual void SetColor(Color rgb, Control ctrlType)
		{
			ColorRGB = rgb;
			ColorHSV = HsvColor.FromRgb(rgb);
			SetColor(ctrlType);
		}

		public virtual void SetColor(Control ctrlType)
		{
			switch (ctrlType)
			{
			case Control.Rect:
				ChangeSliderColor();
				break;
			case Control.Slider:
				ChangeRectColor();
				break;
			}
			this.updateColorAction.Call(ColorRGB);
		}

		public virtual void SetColor(Color color)
		{
			ColorRGB = color;
			ColorHSV = HsvColor.FromRgb(color);
			CalcRectPointer();
			CalcSliderValue();
		}

		protected void Awake()
		{
			ColorHSV = new HsvColor(0f, 0f, 1f);
			ColorRGB = Color.white;
			Image[] array = new Image[2]
			{
				info.GetOrAddComponent<Image>(),
				slider.GetOrAddComponent<Image>()
			};
			imgPack = new ImagePack[array.Length];
			for (int i = 0; i < imgPack.Length; i++)
			{
				imgPack[i] = new ImagePack(array[i]);
			}
		}

		protected virtual void Start()
		{
			_mode.TakeUntilDestroy(this).Subscribe(delegate
			{
				CalcRectPointer();
				CalcSliderValue();
				ChangeRectColor();
				ChangeSliderColor();
			});
			if (modeChangeToggles.Any())
			{
				(from item in modeChangeToggles.Select((Toggle toggle, int index) => new
					{
						toggle = toggle,
						mode = (Mode)index
					})
					where item.toggle != null
					select item).ToList().ForEach(item =>
				{
					(from isOn in item.toggle.OnValueChangedAsObservable()
						where isOn
						select isOn).Subscribe(delegate
					{
						mode = item.mode;
					});
				});
			}
			if (slider != null)
			{
				Control ctrl = Control.Slider;
				Action<Func<HsvColor, HsvColor>> hsv = delegate(Func<HsvColor, HsvColor> func)
				{
					SetColor(func(ColorHSV), ctrl);
				};
				Action<Func<Color, Color>> rgb = delegate(Func<Color, Color> func)
				{
					SetColor(func(ColorRGB), ctrl);
				};
				slider.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					switch (mode)
					{
					case Mode.Hue:
						hsv(delegate(HsvColor c)
						{
							c.H = value * 360f;
							return c;
						});
						break;
					case Mode.Saturation:
						hsv(delegate(HsvColor c)
						{
							c.S = value;
							return c;
						});
						break;
					case Mode.Value:
						hsv(delegate(HsvColor c)
						{
							c.V = value;
							return c;
						});
						break;
					case Mode.Red:
						rgb(delegate(Color c)
						{
							c.r = value;
							return c;
						});
						break;
					case Mode.Green:
						rgb(delegate(Color c)
						{
							c.g = value;
							return c;
						});
						break;
					case Mode.Blue:
						rgb(delegate(Color c)
						{
							c.b = value;
							return c;
						});
						break;
					}
				});
			}
			(from _ in this.UpdateAsObservable().SkipWhile((Unit _) => info == null || pointer == null)
				where base.enabled
				where info.isOn
				select info.imagePos).DistinctUntilChanged().Subscribe(delegate(Vector2 pos)
			{
				pointer.anchoredPosition = pos;
				Vector2 imageRate = info.imageRate;
				Control ctrlType = Control.Rect;
				switch (mode)
				{
				case Mode.Hue:
				{
					HsvColor colorHSV3 = ColorHSV;
					colorHSV3.S = imageRate.x;
					colorHSV3.V = imageRate.y;
					SetColor(colorHSV3, ctrlType);
					break;
				}
				case Mode.Saturation:
				{
					HsvColor colorHSV2 = ColorHSV;
					colorHSV2.H = imageRate.x * 360f;
					colorHSV2.V = imageRate.y;
					SetColor(colorHSV2, ctrlType);
					break;
				}
				case Mode.Value:
				{
					HsvColor colorHSV = ColorHSV;
					colorHSV.H = imageRate.x * 360f;
					colorHSV.S = imageRate.y;
					SetColor(colorHSV, ctrlType);
					break;
				}
				case Mode.Red:
				{
					Color colorRGB3 = ColorRGB;
					colorRGB3.b = imageRate.x;
					colorRGB3.g = imageRate.y;
					SetColor(colorRGB3, ctrlType);
					break;
				}
				case Mode.Green:
				{
					Color colorRGB2 = ColorRGB;
					colorRGB2.b = imageRate.x;
					colorRGB2.r = imageRate.y;
					SetColor(colorRGB2, ctrlType);
					break;
				}
				case Mode.Blue:
				{
					Color colorRGB = ColorRGB;
					colorRGB.r = imageRate.x;
					colorRGB.g = imageRate.y;
					SetColor(colorRGB, ctrlType);
					break;
				}
				}
			});
		}

		[ContextMenu("Setup")]
		protected void Setup()
		{
			modeChangeToggles = GetComponentsInChildren<Toggle>();
			info = GetComponentInChildren<Info>();
			if (info.transform.childCount != 0)
			{
				pointer = info.transform.GetChild(0).GetComponentInChildren<RectTransform>();
			}
		}
	}
}
