using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public sealed class ProgressBarExamples : MonoBehaviour
	{
		[InfoBox("The ProgressBar attribute draws a horizontal colored bar, which can also be clicked to change the value.\n\nIt can be used to show how full an inventory might be, or to make a visual indicator for a healthbar. It can even be used to make fighting game style health bars, that stack multiple layers of health.", InfoMessageType.Info, null)]
		[ProgressBar(0.0, 100.0, 0.15f, 0.47f, 0.74f)]
		public int ProgressBar = 50;

		[InfoBox("Using the ColorMember property, you can make a healthbar that changes color, the lower the value gets.", InfoMessageType.Info, null)]
		[Space(15f)]
		[ProgressBar(0.0, 100.0, 0.15f, 0.47f, 0.74f, ColorMember = "GetHealthBarColor")]
		public float HealthBar = 50f;

		[InfoBox("Using both ColorMember and BackgroundColorMember properties, and applying the ProgressBar attribute on a proprety, you can make stacked health, that changes color, when the health is above 100%.\n\nSimilar to what you might see in a fighting game.", InfoMessageType.Info, null)]
		[Range(0f, 300f)]
		[Space(15f)]
		public float StackedHealth;

		[InfoBox("It's also possible to change the size of a healthbar, using the Height property. Or you can specify a custom color, without refering to another color member.", InfoMessageType.Info, null)]
		[PropertyOrder(10)]
		[HideLabel]
		[Space(15f)]
		[ProgressBar(-100.0, 100.0, 1f, 1f, 1f, Height = 30)]
		public short BigProgressBar = 50;

		[HideLabel]
		[ShowInInspector]
		[ProgressBar(0.0, 100.0, 0.15f, 0.47f, 0.74f, ColorMember = "GetStackedHealthColor", BackgroundColorMember = "GetStackHealthBackgroundColor")]
		private float StackedHealthProgressBar
		{
			get
			{
				return StackedHealth - (float)(100 * (int)((StackedHealth - 1f) / 100f));
			}
		}

		private Color GetHealthBarColor(float value)
		{
			return Color.Lerp(Color.Lerp(Color.red, Color.yellow, MathUtilities.LinearStep(0f, 30f, value)), Color.green, MathUtilities.LinearStep(0f, 100f, value));
		}

		private Color GetStackedHealthColor()
		{
			return (StackedHealth > 200f) ? Color.cyan : ((!(StackedHealth > 100f)) ? Color.red : Color.green);
		}

		private Color GetStackHealthBackgroundColor()
		{
			return (StackedHealth > 200f) ? Color.green : ((!(StackedHealth > 100f)) ? new Color(0.16f, 0.16f, 0.16f, 1f) : Color.red);
		}
	}
}
