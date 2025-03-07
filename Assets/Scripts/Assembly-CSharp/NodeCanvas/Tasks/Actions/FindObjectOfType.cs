using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Note that this is very slow")]
	[Category("GameObject")]
	public class FindObjectOfType<T> : ActionTask where T : Component
	{
		[BlackboardOnly]
		public BBParameter<T> saveComponentAs;

		[BlackboardOnly]
		public BBParameter<GameObject> saveGameObjectAs;

		protected override void OnExecute()
		{
			T val = Object.FindObjectOfType<T>();
			if (val != null)
			{
				saveComponentAs.value = val;
				saveGameObjectAs.value = val.gameObject;
				EndAction(true);
			}
			else
			{
				EndAction(false);
			}
		}
	}
}
