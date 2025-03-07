using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("Input")]
	public class CheckMousePick : ConditionTask
	{
		public ButtonKeys buttonKey;

		[LayerField]
		public int layer;

		[BlackboardOnly]
		public BBParameter<GameObject> saveGoAs;

		[BlackboardOnly]
		public BBParameter<float> saveDistanceAs;

		[BlackboardOnly]
		public BBParameter<Vector3> savePosAs;

		private RaycastHit hit;

		protected override string info
		{
			get
			{
				string text = buttonKey.ToString() + " Click";
				if (!string.IsNullOrEmpty(savePosAs.name))
				{
					text += string.Format("\n<i>(SavePos As {0})</i>", savePosAs);
				}
				if (!string.IsNullOrEmpty(saveGoAs.name))
				{
					text += string.Format("\n<i>(SaveGo As {0})</i>", saveGoAs);
				}
				return text;
			}
		}

		protected override bool OnCheck()
		{
			if (Input.GetMouseButtonDown((int)buttonKey) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity, 1 << layer))
			{
				saveGoAs.value = hit.collider.gameObject;
				saveDistanceAs.value = hit.distance;
				savePosAs.value = hit.point;
				return true;
			}
			return false;
		}
	}
}
