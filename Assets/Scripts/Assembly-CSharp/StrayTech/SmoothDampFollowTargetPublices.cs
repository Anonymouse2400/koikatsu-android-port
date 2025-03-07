using System.Collections;
using Manager;
using UnityEngine;

namespace StrayTech
{
	public class SmoothDampFollowTargetPublices : MonoBehaviour
	{
		[SerializeField]
		private Transform _followTarget;

		[SerializeField]
		private Vector3 _targetOffset = Vector3.zero;

		[SerializeField]
		private float _smoothTime = 0.25f;

		[SerializeField]
		private bool _useFixedUpdate;

		private Vector3[] _vector3s = new Vector3[4];

		private Transform cachedTransform;

		public Transform followTarget
		{
			get
			{
				return _followTarget;
			}
			set
			{
				_followTarget = value;
			}
		}

		public Vector3 targetOffset
		{
			get
			{
				return offset;
			}
		}

		private Vector3 offset
		{
			get
			{
				Vector3 result = _targetOffset;
				result.y += Manager.Config.ActData.TPSOffsetY;
				return result;
			}
		}

		public void Sync()
		{
			if (!(_followTarget == null))
			{
				cachedTransform.position = _followTarget.position + _followTarget.rotation * offset;
				cachedTransform.rotation = _followTarget.rotation;
			}
		}

		private void Awake()
		{
			cachedTransform = base.transform;
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			yield return new WaitWhile(() => _followTarget == null);
			Sync();
			base.enabled = true;
		}

		private void Update()
		{
			if (!_useFixedUpdate)
			{
				DoUpdate(Time.deltaTime);
			}
		}

		private void FixedUpdate()
		{
			if (_useFixedUpdate)
			{
				DoUpdate(Time.fixedDeltaTime);
			}
		}

		private void DoUpdate(float deltaTime)
		{
			if (!(_followTarget == null) && !Mathf.Approximately(deltaTime, 0f))
			{
				_vector3s[0] = (_vector3s[1] = cachedTransform.position);
				_vector3s[1] = _followTarget.position + _followTarget.rotation * offset;
				_vector3s[3].x = Mathf.SmoothDamp(_vector3s[0].x, _vector3s[1].x, ref _vector3s[2].x, _smoothTime * deltaTime);
				_vector3s[3].y = Mathf.SmoothDamp(_vector3s[0].y, _vector3s[1].y, ref _vector3s[2].y, _smoothTime * deltaTime);
				_vector3s[3].z = Mathf.SmoothDamp(_vector3s[0].z, _vector3s[1].z, ref _vector3s[2].z, _smoothTime * deltaTime);
				cachedTransform.position = _vector3s[3];
				cachedTransform.rotation = _followTarget.rotation;
			}
		}
	}
}
