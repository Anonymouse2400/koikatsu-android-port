using System;
using Illusion.CustomAttributes;
using Illusion.Extensions;

namespace ADV
{
	[Serializable]
	public class Regulate
	{
		public enum Control
		{
			Next = 1,
			ClickNext = 2,
			Skip = 4,
			Auto = 8,
			AutoForce = 0x10
		}

		[EnumFlags]
		public Control control;

		private TextScenario scenario;

		public Regulate(TextScenario scenario)
		{
			this.scenario = scenario;
		}

		public void AddRegulate(Control regulate)
		{
			control = (Control)control.Add(regulate);
		}

		public void SubRegulate(Control regulate)
		{
			control = (Control)control.Sub(regulate);
		}

		public void SetRegulate(Control regulate)
		{
			control = regulate;
			if (control == (Control)0)
			{
				scenario.isSkip = false;
				scenario.isAuto = false;
			}
		}
	}
}
