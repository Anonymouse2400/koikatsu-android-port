using System.Collections.Generic;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class Vector : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Variable", "Type", "X", "Y", "Z" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string key = args[num++];
			string text = args[num++];
			Vector3 value = Vector3.zero;
			Dictionary<string, Vector3> v3Dic = base.scenario.commandController.V3Dic;
			switch (text)
			{
			case "=":
			{
				if (v3Dic.TryGetValue(args[num], out value))
				{
					break;
				}
				for (int j = 0; j < 3; j++)
				{
					int num3 = j + num;
					float result2;
					if (!args.SafeGet(num3).IsNullOrEmpty() && float.TryParse(args[num3], out result2))
					{
						value[j] = result2;
					}
				}
				break;
			}
			case "+=":
			{
				if (!v3Dic.TryGetValue(key, out value))
				{
					break;
				}
				float result3;
				if (float.TryParse(args[num], out result3))
				{
					for (int k = 0; k < 3; k++)
					{
						int num4 = k + num;
						if (!args.SafeGet(num4).IsNullOrEmpty() && float.TryParse(args[num4], out result3))
						{
							value[k] += result3;
						}
					}
				}
				else
				{
					value += v3Dic[args[num++]];
				}
				break;
			}
			case "-=":
			{
				if (!v3Dic.TryGetValue(key, out value))
				{
					break;
				}
				float result;
				if (float.TryParse(args[num], out result))
				{
					for (int i = 0; i < 3; i++)
					{
						int num2 = i + num;
						if (!args.SafeGet(num2).IsNullOrEmpty() && float.TryParse(args[num2], out result))
						{
							value[i] -= result;
						}
					}
				}
				else
				{
					value -= v3Dic[args[num++]];
				}
				break;
			}
			}
			base.scenario.commandController.V3Dic[key] = value;
		}
	}
}
