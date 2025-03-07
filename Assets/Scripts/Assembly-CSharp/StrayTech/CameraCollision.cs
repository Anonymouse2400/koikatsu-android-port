using UnityEngine;

namespace StrayTech
{
	public class CameraCollision : MonoBehaviourSingleton<CameraCollision>
	{
		public enum CollisionTestType
		{
			SphereCast = 0,
			RayCast = 1
		}

		[Tooltip("Globally toggle camera collision on and off.")]
		[SerializeField]
		private bool _useCameraCollision;

		[Tooltip("Type of camera collsion.")]
		[SerializeField]
		private CollisionTestType _testType = CollisionTestType.RayCast;

		[Tooltip("Radius of the sphere for sphere casts.")]
		[SerializeField]
		private float _sphereRadius = 0.5f;

		[Tooltip("Layers to collide with.")]
		[SerializeField]
		private LayerMask _collisionLayerMask;

		public bool UseCameraCollision
		{
			get
			{
				return _useCameraCollision;
			}
		}

		public CollisionTestType TestType
		{
			get
			{
				return _testType;
			}
		}

		public float SphereRadius
		{
			get
			{
				return _sphereRadius;
			}
		}

		public void PreventCameraCollision(Camera camera)
		{
			if (!_useCameraCollision || MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget == null)
			{
				return;
			}
			Vector3 position = camera.transform.position;
			Vector3 position2 = MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position;
			Vector3 normalized = (position - position2).normalized;
			float maxDistance = Vector3.Distance(position, position2);
			RaycastHit hitInfo;
			switch (_testType)
			{
			case CollisionTestType.SphereCast:
				if (Physics.SphereCast(position2, _sphereRadius, normalized, out hitInfo, maxDistance, _collisionLayerMask))
				{
					camera.transform.position = position2 + normalized * hitInfo.distance;
				}
				break;
			case CollisionTestType.RayCast:
				if (Physics.Raycast(position2, normalized, out hitInfo, maxDistance, _collisionLayerMask))
				{
					camera.transform.position = position2 + normalized * (hitInfo.distance - 0.1f);
				}
				break;
			}
		}
	}
}
