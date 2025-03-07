using System.Collections.Generic;
using ADV.Commands.Base;

namespace ADV.Commands.Chara
{
	public class ExpressionIcon : ADV.Commands.Base.ExpressionIcon
	{
		public override void Do()
		{
			List<TextScenario.IChara> list = new List<TextScenario.IChara>();
			int cnt = 0;
			if (args.Length > 1)
			{
				while (!args.IsNullOrEmpty(cnt))
				{
					list.Add(new Data(args, ref cnt));
				}
			}
			foreach (CharaData value in base.scenario.commandController.Characters.Values)
			{
				value.faceIcon.Release();
			}
			list.ForEach(delegate(TextScenario.IChara p)
			{
				p.Play(base.scenario);
			});
		}
	}
}
