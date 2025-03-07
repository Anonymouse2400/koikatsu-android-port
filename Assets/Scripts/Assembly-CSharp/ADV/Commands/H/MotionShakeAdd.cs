using System.Linq;
using UnityEngine;

namespace ADV.Commands.H
{
	public class MotionShakeAdd : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return null;
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void Do()
		{
			base.Do();
			HMotionShake component = base.scenario.GetComponent<HMotionShake>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			component = base.scenario.gameObject.AddComponent<HMotionShake>();
			component.SetCharas(base.scenario.commandController.Characters.Values.Select((CharaData p) => p.chaCtrl).ToArray());
		}
	}
}
