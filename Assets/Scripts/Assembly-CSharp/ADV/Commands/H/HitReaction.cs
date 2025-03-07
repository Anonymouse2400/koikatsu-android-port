using System.Linq;
using Illusion.Extensions;
using RootMotion.FinalIK;
using UnityEngine;

namespace ADV.Commands.H
{
	public class HitReaction : CommandBase
	{
		private global::HitReaction hitReaction;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Strength", "Dir", "Hit" };
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
			FullBodyBipedIK component = base.scenario.currentChara.chaCtrl.animBody.GetComponent<FullBodyBipedIK>();
			hitReaction = base.scenario.commandController.HitReaction;
			hitReaction.ik = component;
			int num = 0;
			float strength = 1f;
			args.SafeProc(num++, delegate(string s)
			{
				strength = float.Parse(s);
			});
			int dirSel = 0;
			args.SafeProc(num++, delegate(string s)
			{
				string[] array = new string[4] { "前", "後", "右", "左" };
				if (!int.TryParse(s, out dirSel))
				{
					dirSel = array.Check(s);
				}
			});
			int hitSel = 0;
			args.SafeProc(num++, delegate(string s)
			{
				string[] strs = hitReaction.effectorHit.Select((global::HitReaction.HitPointEffectorParent e) => e.name).ToArray();
				if (!int.TryParse(s, out hitSel))
				{
					hitSel = s.Check(true, strs);
				}
			});
			Vector3 vector = Vector3.zero;
			switch (dirSel)
			{
			case 0:
				vector = -component.transform.forward;
				break;
			case 1:
				vector = component.transform.forward;
				break;
			case 2:
				vector = component.transform.right;
				break;
			case 3:
				vector = -component.transform.right;
				break;
			}
			hitReaction.Hit(hitSel, vector * strength);
		}

		public override bool Process()
		{
			bool flag = base.Process();
			return flag & !hitReaction.IsPlay();
		}
	}
}
