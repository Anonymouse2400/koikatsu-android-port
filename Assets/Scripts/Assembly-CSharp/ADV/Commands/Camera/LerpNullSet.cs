using UnityEngine;

namespace ADV.Commands.Camera
{
	public class LerpNullSet : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "T", "Start", "End", "Middle" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					"0",
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			Transform transform = base.scenario.AdvCamera.transform;
			int num = 0;
			float num2 = float.Parse(args[num++]);
			string key = args[num++];
			string text = args[num++];
			string text2 = args[num++];
			Transform value;
			base.scenario.commandController.NullDic.TryGetValue(key, out value);
			Transform value2 = null;
			if (!text.IsNullOrEmpty())
			{
				base.scenario.commandController.NullDic.TryGetValue(text, out value2);
			}
			Transform value3 = null;
			if (!text2.IsNullOrEmpty())
			{
				base.scenario.commandController.NullDic.TryGetValue(text2, out value3);
			}
			if (value3 != null)
			{
				transform.transform.SetPositionAndRotation(value3.position, value3.rotation);
			}
			if (value2 == null)
			{
				transform.transform.position = CommandBase.LerpV3(transform.position, value.position, num2);
				transform.eulerAngles = CommandBase.LerpAngleV3(transform.eulerAngles, value.eulerAngles, num2);
			}
			else
			{
				transform.transform.position = MotionIK.GetShapeLerpPositionValue(num2, value.position, value2.position);
				transform.eulerAngles = MotionIK.GetShapeLerpAngleValue(num2, value.eulerAngles, value2.eulerAngles);
			}
		}
	}
}
