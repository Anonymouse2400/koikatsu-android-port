using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Component.UI.ColorPicker
{
	public class PickerRectA : PickerRect
	{
		[SerializeField]
		private Slider sliderA;

		public BoolReactiveProperty isAlpha = new BoolReactiveProperty(true);

		private float alpha;

		public override Color ColorRGB
		{
			get
			{
				Color colorRGB = base.ColorRGB;
				colorRGB.a = Alpha;
				return colorRGB;
			}
			set
			{
				base.ColorRGB = value;
				Alpha = value.a;
			}
		}

		public override float[] RGB
		{
			get
			{
				return base.RGB.Concat(new float[1] { Alpha }).ToArray();
			}
			set
			{
				base.RGB = value;
				if (value.Length >= 4)
				{
					Alpha = value[3];
				}
			}
		}

		public float Alpha
		{
			get
			{
				return alpha;
			}
			set
			{
				alpha = value;
			}
		}

		public override void SetColor(HsvColor hsv, Control ctrlType)
		{
			base.ColorHSV = hsv;
			base.ColorRGB = HsvColor.ToRgb(hsv);
			SetColor(ctrlType);
		}

		public override void SetColor(Color color)
		{
			base.SetColor(color);
			CalcSliderAValue();
		}

		public void CalcSliderAValue()
		{
			if (!(sliderA == null))
			{
				sliderA.value = Alpha;
			}
		}

		protected override void Start()
		{
			_mode.TakeUntilDestroy(this).Subscribe(delegate
			{
				CalcSliderAValue();
			});
			if (sliderA != null)
			{
				isAlpha.TakeUntilDestroy(this).Subscribe(sliderA.gameObject.SetActive);
				sliderA.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					Alpha = value;
					SetColor(ColorRGB, Control.None);
				});
			}
			base.Start();
		}
	}
}
