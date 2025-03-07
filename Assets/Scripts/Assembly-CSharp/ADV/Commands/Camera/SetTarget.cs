using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetTarget : Base
	{
		protected Transform target;

		protected bool isReset;

		protected float time;

		protected float timer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Name", "isReset", "Time" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					string.Empty,
					bool.TrueString,
					"0"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null)
			{
				target = GetTarget(args[0].Split(','));
				isReset = bool.Parse(args[1]);
				baseCamCtrl.TargetSet(target, isReset);
			}
			timer = float.Parse(args[2]);
		}

		public override bool Process()
		{
			base.Process();
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null)
			{
				baseCamCtrl.TargetSet(target, isReset);
			}
			timer = Mathf.Min(timer + Time.deltaTime, time);
			return timer >= time;
		}
	}
}
