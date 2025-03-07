using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimeLayerWeight : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "LayerNo", "Weight" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "0", "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int layerIndex = int.Parse(args[num++]);
			float weight = float.Parse(args[num++]);
			Animator component = base.scenario.AdvCamera.GetComponent<Animator>();
			if (component != null)
			{
				component.SetLayerWeight(layerIndex, weight);
			}
		}
	}
}
