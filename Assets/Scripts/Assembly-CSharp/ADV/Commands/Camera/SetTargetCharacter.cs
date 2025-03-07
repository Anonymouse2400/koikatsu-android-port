using System.Linq;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetTargetCharacter : SetTarget
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Name", "isReset", "Time", "No" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					string.Empty,
					bool.TrueString,
					"0",
					int.MaxValue.ToString()
				};
			}
		}

		public override void Do()
		{
			int num = 0;
			string targetName = args[num++];
			if (!bool.TryParse(args[num++], out isReset))
			{
				isReset = true;
			}
			time = float.Parse(args[num++]);
			int no = int.Parse(args[num++]);
			target = base.scenario.commandController.GetChara(no).chaCtrl.objTop.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform frame) => frame.name.Compare(targetName, true));
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null && target != null)
			{
				baseCamCtrl.TargetSet(target, isReset);
			}
			timer = 0f;
		}
	}
}
