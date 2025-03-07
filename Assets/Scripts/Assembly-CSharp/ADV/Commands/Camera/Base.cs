using System.Linq;
using UnityEngine;

namespace ADV.Commands.Camera
{
	public abstract class Base : CommandBase
	{
		protected Transform GetTarget(params string[] targetNames)
		{
			string targetName = targetNames.SafeGet(0);
			if (targetName.IsNullOrEmpty())
			{
				return null;
			}
			Transform value;
			if (base.scenario.commandController.NullDic.TryGetValue(targetName, out value))
			{
				return value;
			}
			GameObject gameObject = GameObject.Find(targetName);
			if (gameObject != null)
			{
				targetName = targetNames.SafeGet(1);
				if (targetName != null)
				{
					Transform transform = gameObject.GetComponentsInChildren<Transform>().FirstOrDefault((Transform t) => t.name == targetName);
					if (transform != null)
					{
						return transform;
					}
				}
			}
			return (!(gameObject == null)) ? gameObject.transform : null;
		}
	}
}
