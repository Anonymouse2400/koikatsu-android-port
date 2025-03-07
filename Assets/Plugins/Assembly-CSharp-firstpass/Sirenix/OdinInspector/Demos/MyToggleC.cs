using System;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class MyToggleC
	{
		[ToggleGroup("Enabled", "$Label")]
		public bool Enabled;

		[ToggleGroup("Enabled", 0, null)]
		public float Test;

		public string Label
		{
			get
			{
				return Test.ToString();
			}
		}
	}
}
