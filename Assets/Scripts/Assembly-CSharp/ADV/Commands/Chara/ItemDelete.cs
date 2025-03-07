namespace ADV.Commands.Chara
{
	public class ItemDelete : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "ItemNo" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					int.MaxValue.ToString(),
					"0"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int key = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			chara.itemDic[key].Delete();
			chara.itemDic.Remove(key);
		}
	}
}
