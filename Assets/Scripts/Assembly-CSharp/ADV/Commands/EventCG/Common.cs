using ADV.EventCG;
using UnityEngine;

namespace ADV.Commands.EventCG
{
	internal static class Common
	{
		public static bool Release(TextScenario scenario)
		{
			bool flag = scenario.commandController.EventCGRoot.childCount > 0;
			if (flag)
			{
				Transform child = scenario.commandController.EventCGRoot.GetChild(0);
				ADV.EventCG.Data component = child.GetComponent<ADV.EventCG.Data>();
				if (component != null)
				{
					component.ItemClear();
					component.Restore();
				}
				UnityEngine.Object.Destroy(child.gameObject);
				child.name += "(Destroyed)";
				child.parent = null;
			}
			return flag;
		}
	}
}
