using UnityEngine;

namespace Studio
{
	public class RouteComponent : MonoBehaviour
	{
		public delegate bool OnCompleteDel();

		public OnCompleteDel onComplete;

		public void OnComplete()
		{
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}
}
