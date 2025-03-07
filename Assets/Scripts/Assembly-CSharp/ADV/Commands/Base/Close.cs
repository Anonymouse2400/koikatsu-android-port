using Manager;

namespace ADV.Commands.Base
{
	public class Close : CommandBase
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
			Proc();
		}

		private void Proc()
		{
			bool isADVActionActive = Program.isADVActionActive;
			if (base.scenario.advScene != null)
			{
				base.scenario.advScene.Release();
			}
			else
			{
				base.scenario.gameObject.SetActive(false);
			}
			if (!isADVActionActive && base.scenario.advScene != null)
			{
				Singleton<Manager.Scene>.Instance.UnLoad();
			}
		}
	}
}
