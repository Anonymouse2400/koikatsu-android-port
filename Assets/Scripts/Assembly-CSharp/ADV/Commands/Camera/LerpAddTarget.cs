using UnityEngine;

namespace ADV.Commands.Camera
{
	public class LerpAddTarget : LerpAdd
	{
		private Transform target;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Time", "Name", "X,Y,Z", "Pitch,Yaw,Roll", "Dir" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		protected override void Analytics(string[] args, TextScenario scenario)
		{
			int num = 0;
			float.TryParse(args[num++], out time);
			target = GetTarget(args[num++].Split(','));
			string text = args[num++];
			string text2 = args[num++];
			string s = args[num++];
			float result = 0f;
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length && i < 3; i++)
			{
				if (float.TryParse(array[i], out result))
				{
					calcPos[i] = result;
				}
			}
			string[] array2 = text2.Split(',');
			for (int j = 0; j < array2.Length && j < 3; j++)
			{
				if (float.TryParse(array2[j], out result))
				{
					calcAng[j] = result;
				}
			}
			if (float.TryParse(s, out result))
			{
				calcDir = result;
			}
			if (!(target == null))
			{
				endPos = (initPos = target.position);
				endAng = (initAng = target.eulerAngles);
			}
		}
	}
}
