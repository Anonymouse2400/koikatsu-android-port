using UnityEngine;

namespace ADV.Commands.Camera
{
	public class LerpSetTarget : LerpSet
	{
		private Transform target;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Time", "Name", "Dir" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					"0",
					string.Empty,
					"0"
				};
			}
		}

		protected override void Analytics(string[] args, TextScenario scenario)
		{
			int num = 0;
			float.TryParse(args[num++], out time);
			target = GetTarget(args[num++].Split(','));
			string s = args[num++];
			float result = 0f;
			if (float.TryParse(s, out result))
			{
				calcDir = result;
			}
			if (!(target == null))
			{
				calcPos = target.position;
				calcAng = target.eulerAngles;
			}
		}
	}
}
