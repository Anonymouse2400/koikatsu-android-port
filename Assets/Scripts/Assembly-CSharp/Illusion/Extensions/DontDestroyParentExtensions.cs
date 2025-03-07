using Illusion.Component;
using UnityEngine;

namespace Illusion.Extensions
{
	internal static class DontDestroyParentExtensions
	{
		public static void DontDestroyOnNextLoad(this GameObject _, GameObject target)
		{
			DontDestroyParent.Register(target);
		}

		public static void DontDestroyOnNextLoad(this GameObject _, MonoBehaviour target)
		{
			DontDestroyParent.Register(target);
		}

		public static void DontDestroyOnNextLoad(this MonoBehaviour _, GameObject target)
		{
			DontDestroyParent.Register(target);
		}

		public static void DontDestroyOnNextLoad(this MonoBehaviour _, MonoBehaviour target)
		{
			DontDestroyParent.Register(target);
		}
	}
}
