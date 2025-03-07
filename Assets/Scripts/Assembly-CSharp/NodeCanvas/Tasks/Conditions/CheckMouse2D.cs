using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[EventReceiver(new string[] { "OnMouseEnter", "OnMouseExit", "OnMouseOver" })]
	[Name("Check Mouse 2D")]
	[Category("System Events")]
	public class CheckMouse2D : ConditionTask<Collider2D>
	{
		public MouseInteractionTypes checkType;

		protected override string info
		{
			get
			{
				return checkType.ToString();
			}
		}

		protected override bool OnCheck()
		{
			return false;
		}

		public void OnMouseEnter()
		{
			if (checkType == MouseInteractionTypes.MouseEnter)
			{
				YieldReturn(true);
			}
		}

		public void OnMouseExit()
		{
			if (checkType == MouseInteractionTypes.MouseExit)
			{
				YieldReturn(true);
			}
		}

		public void OnMouseOver()
		{
			if (checkType == MouseInteractionTypes.MouseOver)
			{
				YieldReturn(true);
			}
		}
	}
}
