using System.Linq;

namespace ADV.Commands.Game
{
	public class CharaPersonal : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Type" };
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

		private static string GetVariable(string s)
		{
			return "[" + s + "]";
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int num2 = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			ChaFileControl charFile = chara.heroine.charFile;
			switch (num2)
			{
			case 0:
				base.scenario.Vars[GetVariable("胸")] = new ValData(GetBustSize(charFile));
				break;
			case 1:
				base.scenario.Vars[GetVariable("身長")] = new ValData(GetHeight(charFile));
				break;
			case 2:
				base.scenario.Vars[GetVariable("体型")] = new ValData(GetWaist(charFile));
				break;
			case 3:
			{
				foreach (var item in ChaInfo.GetAttribute(charFile).Select((bool b, int i) => new { b, i }))
				{
					base.scenario.Vars[GetVariable(GetAttribute(item.i))] = new ValData(item.b);
				}
				break;
			}
			case 4:
			{
				foreach (var item2 in ChaInfo.GetAwnserArry(charFile).Select((bool b, int i) => new { b, i }))
				{
					base.scenario.Vars[GetVariable(GetAnswer(item2.i))] = new ValData(item2.b);
				}
				break;
			}
			case 5:
			{
				foreach (var item3 in ChaInfo.GetDenialArry(charFile).Select((bool b, int i) => new { b, i }))
				{
					base.scenario.Vars[GetVariable(GetDenial(item3.i))] = new ValData(item3.b);
				}
				break;
			}
			}
		}

		private static string GetBustSize(ChaFileControl chaFile)
		{
			float num = chaFile.custom.body.shapeValueBody[4];
			if (num < 0.4f)
			{
				return "小";
			}
			if (num > 0.7f)
			{
				return "大";
			}
			return "中";
		}

		private static string GetHeight(ChaFileControl chaFile)
		{
			float num = chaFile.custom.body.shapeValueBody[0];
			if (num < 0.25f)
			{
				return "低";
			}
			if (num > 0.6f)
			{
				return "高";
			}
			return "中";
		}

		private static string GetWaist(ChaFileControl chaFile)
		{
			float[] shapeValueBody = chaFile.custom.body.shapeValueBody;
			float num = (shapeValueBody[ChaFileDefine.cf_CategoryWaist[0]] + shapeValueBody[ChaFileDefine.cf_CategoryWaist[1]] + shapeValueBody[ChaFileDefine.cf_CategoryWaist[2]]) / 3f;
			if (num < 0.35f)
			{
				return "細";
			}
			if (num > 0.65f)
			{
				return "太";
			}
			return "中";
		}

		private static string GetAttribute(int i)
		{
			switch (i)
			{
			case 0:
				return "頻尿";
			case 1:
				return "はらぺこ";
			case 2:
				return "鈍感";
			case 3:
				return "チョロイ";
			case 4:
				return "ビッチ";
			case 5:
				return "むっつり";
			case 6:
				return "読書好き";
			case 7:
				return "音楽好き";
			case 8:
				return "活発";
			case 9:
				return "受け身";
			case 10:
				return "フレンドリー";
			case 11:
				return "綺麗好き";
			case 12:
				return "怠惰";
			case 13:
				return "神出鬼没";
			case 14:
				return "一人好き";
			case 15:
				return "運動好き";
			case 16:
				return "真面目";
			case 17:
				return "女の子好き";
			default:
				return "error:" + i;
			}
		}

		private static string GetAnswer(int i)
		{
			switch (i)
			{
			case 0:
				return "動物";
			case 1:
				return "食物";
			case 2:
				return "料理";
			case 3:
				return "運動";
			case 4:
				return "勉強";
			case 5:
				return "おしゃれ";
			case 6:
				return "苦い";
			case 7:
				return "辛い";
			case 8:
				return "甘い";
			default:
				return "error:" + i;
			}
		}

		private static string GetDenial(int i)
		{
			switch (i)
			{
			case 0:
				return "キス";
			case 1:
				return "愛撫";
			case 2:
				return "アナル";
			case 3:
				return "電マ";
			case 4:
				return "ゴム";
			default:
				return "error:" + i;
			}
		}
	}
}
