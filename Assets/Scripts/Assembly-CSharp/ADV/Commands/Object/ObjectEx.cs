using System.Linq;
using UnityEngine;

namespace ADV.Commands.Object
{
	internal static class ObjectEx
	{
		public static Transform FindRoot(string findType, CommandController commandController)
		{
			Transform result = null;
			if (!findType.IsNullOrEmpty())
			{
				int result2;
				result = ((!int.TryParse(findType, out result2)) ? commandController.Objects[findType].transform : commandController.Character.GetChild(result2));
			}
			return result;
		}

		public static Transform FindChild(Transform root, string name)
		{
			return root.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform t) => t.name == name);
		}

		public static Transform FindGet(string findType, string childName, string otherRootName, CommandController commandController)
		{
			Transform transform = FindRoot(findType, commandController);
			if (transform == null)
			{
				GameObject gameObject = GameObject.Find(otherRootName);
				transform = gameObject.transform;
			}
			if (!childName.IsNullOrEmpty())
			{
				transform = FindChild(transform, childName);
			}
			return transform;
		}
	}
}
