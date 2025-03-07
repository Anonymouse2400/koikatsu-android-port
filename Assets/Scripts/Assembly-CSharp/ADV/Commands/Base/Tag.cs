namespace ADV.Commands.Base
{
	public class Tag : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Label" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}
	}
}
