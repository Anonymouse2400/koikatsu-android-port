namespace UnityEngine.AI.NavMeshTest
{
	public class Checker : MonoBehaviour
	{
		[Header("計算する")]
		public bool isCalc;

		[Header("計算後に座標をセット")]
		public bool isSet;

		public Vector3 hitPos = Vector3.zero;

		public Vector3 distance = Vector3.zero;

		private Vector3? _hitPos;

		private void OnDrawGizmos()
		{
			if (_hitPos.HasValue)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawRay(_hitPos.Value, Vector3.up * 2f);
			}
			if (!Mathf.Approximately(distance.sqrMagnitude, 0f))
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(base.transform.position, distance);
			}
		}

		protected virtual void Update()
		{
			if (!isCalc)
			{
				return;
			}
			NavMeshHit hit;
			if (NavMesh.SamplePosition(base.transform.position, out hit, 1f, -1))
			{
				_hitPos = (hitPos = hit.position);
				if (isSet)
				{
					base.transform.position = hit.position;
				}
			}
			if (_hitPos.HasValue)
			{
				distance = hitPos - base.transform.position;
			}
		}
	}
}
