using System;
using System.Collections.Generic;
using Manager;

namespace ADV.Commands.Base
{
	public class Expression : CommandBase
	{
		public class Data : Manager.Game.Expression, TextScenario.IExpression, TextScenario.IChara
		{
			public int no { get; private set; }

			public Data(string[] args, ref int cnt)
				: base(args, ref cnt)
			{
			}

			public Data(string[] args)
				: base(args)
			{
			}

			public Data(int no, Manager.Game.Expression src)
			{
				this.no = no;
				src.Copy(this);
			}

			public override void Initialize(string[] args, ref int cnt, bool isThrow = false)
			{
				try
				{
					no = int.Parse(args[cnt++]);
					base.Initialize(args, ref cnt, true);
				}
				catch (Exception)
				{
					if (isThrow)
					{
						throw new Exception(string.Join(",", args));
					}
				}
			}

			public void Play(TextScenario scenario)
			{
				Change(GetChara(scenario).chaCtrl);
			}

			public CharaData GetChara(TextScenario scenario)
			{
				return scenario.commandController.GetChara(no);
			}
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[12]
				{
					"No", "眉", "目", "口", "眉開き", "目開き", "口開き", "視線", "頬赤", "ハイライト",
					"涙", "瞬き"
				};
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { int.MaxValue.ToString() };
			}
		}

		public static List<Data> Convert(ref string[] args, TextScenario scenario)
		{
			List<Data> list = new List<Data>();
			if (args.Length > 1)
			{
				int cnt = 0;
				while (!args.IsNullOrEmpty(cnt))
				{
					string check = null;
					args.SafeProc(cnt + 1, delegate(string s)
					{
						check = s;
					});
					if (check != null)
					{
						int no = int.Parse(args[cnt]);
						Manager.Game.Expression expression = Manager.Game.GetExpression(scenario.commandController.expDic, check);
						if (expression != null)
						{
							list.Add(new Data(no, expression));
							cnt += 2;
							continue;
						}
						CharaData chara = scenario.commandController.GetChara(no);
						ChaControl chaCtrl = chara.chaCtrl;
						int personality = 0;
						if (chara.data is SaveData.Heroine)
						{
							SaveData.Heroine heroine = chara.data as SaveData.Heroine;
							VoiceInfo.Param value;
							personality = ((!Singleton<Manager.Voice>.Instance.voiceInfoDic.TryGetValue(heroine.FixCharaIDOrPersonality, out value)) ? heroine.charFile.parameter.personality : value.No);
						}
						else if (chara.data != null)
						{
							personality = chara.data.charFile.parameter.personality;
						}
						else if (chaCtrl != null)
						{
							personality = chaCtrl.chaFile.parameter.personality;
						}
						if (Singleton<Manager.Game>.IsInstance())
						{
							expression = Singleton<Manager.Game>.Instance.GetExpression(personality, check);
							if (expression != null)
							{
								list.Add(new Data(no, expression));
								cnt += 2;
								continue;
							}
						}
					}
					Data data = new Data(args, ref cnt);
					data.isChangeSkip = true;
					list.Add(data);
				}
			}
			return list;
		}

		public override void Do()
		{
			base.Do();
			base.scenario.currentCharaData.CreateExpressionList();
			base.scenario.currentCharaData.expressionList.Add(Convert(ref args, base.scenario).ToArray());
		}
	}
}
