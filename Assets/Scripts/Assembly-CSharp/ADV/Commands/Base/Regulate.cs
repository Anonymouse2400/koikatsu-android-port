using System;
using Illusion;
using Illusion.Extensions;

namespace ADV.Commands.Base
{
	public class Regulate : CommandBase
	{
		private enum Type
		{
			Set = 0,
			Add = 1,
			Sub = 2
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Type", "Regulate" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"Set",
					ADV.Regulate.Control.Next.ToString()
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string self = args[num++];
			int num2 = self.Check(true, Utils.Enum<Type>.Names);
			string self2 = args[num++];
			int index = self2.Check(true, Utils.Enum<ADV.Regulate.Control>.Names);
			Array values = Utils.Enum<ADV.Regulate.Control>.Values;
			ADV.Regulate.Control regulate = (ADV.Regulate.Control)values.GetValue(index);
			switch ((Type)num2)
			{
			case Type.Set:
				base.scenario.regulate.SetRegulate(regulate);
				break;
			case Type.Add:
				base.scenario.regulate.AddRegulate(regulate);
				break;
			case Type.Sub:
				base.scenario.regulate.SubRegulate(regulate);
				break;
			}
		}
	}
}
