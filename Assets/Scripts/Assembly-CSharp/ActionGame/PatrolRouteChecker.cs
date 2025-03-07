using System.Linq;
using Illusion;
using UnityEngine;

namespace ActionGame
{
	public class PatrolRouteChecker : MonoBehaviour
	{
		[SerializeField]
		private Transform[] _route;

		public Transform[] route
		{
			get
			{
				return _route;
			}
			set
			{
				_route = value;
			}
		}

		private void OnDrawGizmos()
		{
			if (_route != null)
			{
				Gizmos.color = Color.red;
				Utils.Gizmos.PointLine((from t in _route
					where t != null
					select t.position + Vector3.up * 0.1f).ToArray(), true);
			}
		}
	}
}
