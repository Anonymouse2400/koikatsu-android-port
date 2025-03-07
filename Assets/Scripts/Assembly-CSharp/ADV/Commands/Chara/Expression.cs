using ADV.Commands.Base;

namespace ADV.Commands.Chara
{
	public class Expression : ADV.Commands.Base.Expression
	{
		public override void Do()
		{
			ADV.Commands.Base.Expression.Convert(ref args, base.scenario).ForEach(delegate(Data p)
			{
				p.Play(base.scenario);
			});
		}
	}
}
