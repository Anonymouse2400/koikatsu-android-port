using UnityEngine;

namespace ADV.Commands.Chara
{
	public class LookEyesTarget : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[7] { "No", "Type", "Target", "Rate", "Deg", "Range", "Dis" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[7]
				{
					int.MaxValue.ToString(),
					"0",
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
			int no = int.Parse(args[num++]);
			int targetType = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			GameObject obj = null;
			args.SafeProc(num++, delegate(string s)
			{
				obj = GameObject.Find(s);
			});
			Transform trfTarg = ((!(obj == null)) ? obj.transform : null);
			float rate = 0.5f;
			args.SafeProc(num++, delegate(string s)
			{
				rate = float.Parse(s);
			});
			float rotDeg = 0f;
			args.SafeProc(num++, delegate(string s)
			{
				rotDeg = float.Parse(s);
			});
			float range = 1f;
			args.SafeProc(num++, delegate(string s)
			{
				range = float.Parse(s);
			});
			float dis = 2f;
			args.SafeProc(num++, delegate(string s)
			{
				dis = float.Parse(s);
			});
			chara.chaCtrl.ChangeLookEyesTarget(targetType, trfTarg, rate, rotDeg, range, dis);
		}
	}
}
