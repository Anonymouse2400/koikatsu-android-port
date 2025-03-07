namespace ADV.Commands.Game
{
	public class BundleCheck : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Variable", "Bundle" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "file", "abdata" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string key = args[num++];
			string bundle = args[num++];
			base.scenario.Vars[key] = new ValData(AssetBundleCheck.IsManifestOrBundle(bundle));
		}
	}
}
