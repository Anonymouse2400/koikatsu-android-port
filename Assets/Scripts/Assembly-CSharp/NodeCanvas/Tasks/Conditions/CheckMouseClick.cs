using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[EventReceiver(new string[] { "OnMouseDown", "OnMouseUp" })]
	[Category("System Events")]
	public class CheckMouseClick : ConditionTask<Collider>
	{
		public MouseClickEvent checkType;

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

		public void OnMouseDown()
		{
			if (checkType == MouseClickEvent.MouseDown)
			{
				YieldReturn(true);
			}
		}

		public void OnMouseUp()
		{
			if (checkType == MouseClickEvent.MouseUp)
			{
				YieldReturn(true);
			}
		}
	}
}
