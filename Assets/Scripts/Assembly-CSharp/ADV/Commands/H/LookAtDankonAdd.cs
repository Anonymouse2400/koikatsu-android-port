using System.Linq;
using UnityEngine;

namespace ADV.Commands.H
{
	public class LookAtDankonAdd : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[6] { "No", "Bundle", "Asset", "BaseBone", "Top", "RefBone" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[6]
				{
					"0",
					string.Empty,
					string.Empty,
					"cm_J_dan101_00",
					"cm_J_dan109_00",
					"cm_J_dan100_00"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			Lookat_dan component = base.scenario.GetComponent<Lookat_dan>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			component = base.scenario.gameObject.AddComponent<Lookat_dan>();
			string[] argToSplit = GetArgToSplit(num++);
			string pathAssetFolder = args[num++];
			string pathFile = args[num++];
			component.nameBaseBone = args[num++];
			component.nameTop = args[num++];
			component.nameRefBone = args[num++];
			component.SetMale(base.scenario.commandController.GetChara(-1).chaCtrl);
			component.SetFemale((from s in argToSplit
				select base.scenario.commandController.GetChara(int.Parse(s)) into p
				select p.chaCtrl).ToArray());
			component.LoadList(pathAssetFolder, pathFile);
		}
	}
}
