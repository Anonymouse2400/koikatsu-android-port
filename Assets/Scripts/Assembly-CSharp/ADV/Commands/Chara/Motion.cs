using System.Collections.Generic;
using System.Linq;
using ADV.Commands.Base;

namespace ADV.Commands.Chara
{
	public class Motion : ADV.Commands.Base.Motion
	{
		public override void Do()
		{
			List<Data> list = ADV.Commands.Base.Motion.Convert(ref args, base.scenario, ArgsLabel.Length);
			if (list.Any())
			{
				base.scenario.CrossFadeStart();
			}
			list.ForEach(delegate(Data p)
			{
				p.Play(base.scenario);
			});
		}
	}
}
