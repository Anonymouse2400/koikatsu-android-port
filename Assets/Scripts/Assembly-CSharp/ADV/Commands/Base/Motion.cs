using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class Motion : CommandBase
	{
		public class Data : TextScenario.IMotion, TextScenario.IChara
		{
			public int no { get; private set; }

			public string stateName { get; private set; }

			public string assetBundleName { get; private set; }

			public string assetName { get; private set; }

			public string ikAssetBundleName { get; private set; }

			public string ikAssetName { get; private set; }

			public string shakeAssetBundleName { get; private set; }

			public string shakeAssetName { get; private set; }

			public string overrideAssetBundleName { get; private set; }

			public string overrideAssetName { get; private set; }

			public int[] layerNo { get; private set; }

			public Data(string[] args, ref int cnt)
			{
				try
				{
					no = int.Parse(args[cnt++]);
					stateName = args[cnt++];
					assetBundleName = args.SafeGet(cnt++);
					assetName = args.SafeGet(cnt++);
					ikAssetBundleName = args.SafeGet(cnt++);
					ikAssetName = args.SafeGet(cnt++);
					shakeAssetBundleName = args.SafeGet(cnt++);
					shakeAssetName = args.SafeGet(cnt++);
					overrideAssetBundleName = args.SafeGet(cnt++);
					overrideAssetName = args.SafeGet(cnt++);
					args.SafeProc(cnt++, delegate(string s)
					{
						layerNo = s.Split(',').Select(int.Parse).ToArray();
					});
				}
				catch (Exception)
				{
				}
			}

			public Data[] Get()
			{
				string[] array = stateName.Split(',');
				if (array.Length == 1)
				{
					return new Data[1] { this };
				}
				List<string[]> list = new List<string[]>();
				list.Add(array);
				list.Add(assetBundleName.Split(','));
				list.Add(assetName.Split(','));
				list.Add(ikAssetBundleName.Split(','));
				list.Add(ikAssetName.Split(','));
				list.Add(shakeAssetBundleName.Split(','));
				list.Add(shakeAssetName.Split(','));
				list.Add(overrideAssetBundleName.Split(','));
				list.Add(overrideAssetName.Split(','));
				List<string[]> row = new List<string[]>();
				for (int j = 0; j < array.Length; j++)
				{
					List<string> list2 = new List<string>();
					list2.Add(no.ToString());
					foreach (string[] item in list)
					{
						list2.Add(item.SafeGet(j) ?? string.Empty);
					}
					list2.Add(string.Join(",", layerNo.Select((int no) => no.ToString()).ToArray()));
					row.Add(list2.ToArray());
				}
				return (from i in Enumerable.Range(0, array.Length)
					select new Data(row[i], ref i)).ToArray();
			}

			public void Play(TextScenario scenario)
			{
				GetChara(scenario).MotionPlay(this, false);
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
				return new string[11]
				{
					"No", "State", "Bundle", "Asset", "IKBundle", "IKAsset", "ShakeBundle", "ShakeAsset", "OverrideBundle", "OverrideAsset",
					"LayerNo"
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

		public static List<Data> Convert(ref string[] args, TextScenario scenario, int argsLen)
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
					string[] value;
					if (check != null && scenario.commandController.motionDic.TryGetValue(check, out value))
					{
						string[] array = Enumerable.Repeat(string.Empty, argsLen - 1).ToArray();
						int num = Mathf.Min(array.Length, value.Length);
						for (int i = 0; i < num; i++)
						{
							if (value[i] != null)
							{
								array[i] = value[i];
							}
						}
						args = args.Take(cnt + 1).Concat(array).Concat(args.Skip(cnt + 2))
							.ToArray();
					}
					list.Add(new Data(args, ref cnt));
				}
			}
			return list;
		}

		public override void Do()
		{
			base.Do();
			base.scenario.currentCharaData.CreateMotionList();
			base.scenario.currentCharaData.motionList.Add(Convert(ref args, base.scenario, ArgsLabel.Length).ToArray());
		}
	}
}
