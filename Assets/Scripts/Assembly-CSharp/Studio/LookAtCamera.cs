using UnityEngine;

namespace Studio
{
	public class LookAtCamera : MonoBehaviour
	{
		private Transform target;

		private void Start()
		{
			target = Camera.main.transform;
		}

		private void Update()
		{
			base.transform.LookAt(target.position, target.up);
		}
	}
}
