using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ButtonAndButtonGroupExamples : MonoBehaviour
	{
		public string DynamicLabel = "Dynamic label";

		public bool HideButton;

		public bool DisableButton;

		[InlineButton("OnClick", null)]
		public int InlineButton;

		[InlineButton("OnClick", null)]
		[InlineButton("OnClick", null)]
		public int InlineButtons;

		[Button(ButtonSizes.Small)]
		private void DefaultSizedButton()
		{
			OnClick();
		}

		[Button(ButtonSizes.Medium)]
		private void MediumSizedButton()
		{
			OnClick();
		}

		[Button(ButtonSizes.Large)]
		private void LargeSizedButton()
		{
			OnClick();
		}

		[Button(ButtonSizes.Gigantic)]
		private void GiganticSizedButton()
		{
			OnClick();
		}

		[ButtonGroup("My Button Group", 0)]
		private void A()
		{
			OnClick();
		}

		[ButtonGroup("My Button Group", 0)]
		private void B()
		{
			OnClick();
		}

		[ButtonGroup("My Button Group", 0)]
		private void C()
		{
			OnClick();
		}

		[Button(ButtonSizes.Small)]
		[GUIColor(0f, 1f, 0f, 1f)]
		private void ColoredButton()
		{
			OnClick();
		}

		[Button("Custom Button Name", ButtonSizes.Small)]
		private void NamedButton()
		{
			OnClick();
		}

		[Button("$DynamicLabel", ButtonSizes.Small)]
		private void DynamiclyNamedButton()
		{
			OnClick();
		}

		[HideIf("HideButton", true)]
		[DisableIf("DisableButton")]
		[Button(ButtonSizes.Gigantic)]
		[GUIColor(0f, 1f, 0f, 1f)]
		private void ConditionalButton()
		{
			OnClick();
		}

		private void OnClick()
		{
			Debug.Log("Click");
		}
	}
}
