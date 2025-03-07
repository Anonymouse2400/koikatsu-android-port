using RootMotion.FinalIK;
using UnityEngine;

namespace ActionGame
{
	public class HoldingHands : MonoBehaviour
	{
		public bool isTargetMain = true;

		[Range(0f, 1f)]
		public float mainHandTargetWeight = 1f;

		[Range(0f, 100f)]
		public float handMoveSpeed = 5f;

		[Range(0f, 100f)]
		public float mainHandDistance = 2f;

		public bool isFixHandTrans;

		[Range(0f, 100f)]
		public float endHandTrans = 0.75f;

		public bool isUseRotation;

		[Range(0f, 1f)]
		public float crossFade = 0.5f;

		[SerializeField]
		private HandIK myHandIK;

		[SerializeField]
		private Transform handsParent;

		[SerializeField]
		private Transform handsChild;

		private float handPositionTrans;

		private bool isMainRight = true;

		private float weight;

		public HandIK targetHandIK { get; private set; }

		public void Set(HandIK targetHandIK)
		{
			this.targetHandIK = targetHandIK;
			HandIK handIK = myHandIK;
			HoldingHands holdingHandUnion = (targetHandIK.holdingHandUnion = this);
			handIK.holdingHandUnion = holdingHandUnion;
		}

		public void Release()
		{
			base.enabled = false;
			myHandIK.enabled = false;
			myHandIK.holdingHandUnion = null;
			if (targetHandIK != null)
			{
				targetHandIK.enabled = false;
				targetHandIK.holdingHandUnion = null;
				targetHandIK = null;
			}
		}

		private void LateUpdate()
		{
			HandIK handIK = null;
			HandIK handIK2 = null;
			if (isTargetMain)
			{
				handIK = myHandIK;
				handIK2 = targetHandIK;
			}
			else
			{
				handIK = targetHandIK;
				handIK2 = myHandIK;
			}
			Transform transform = handIK.ik.transform;
			Transform transform2 = handIK2.ik.transform;
			Vector3 vector = transform2.position - transform.position;
			float t = Time.deltaTime * handMoveSpeed;
			weight = Mathf.Lerp(weight, Mathf.InverseLerp(mainHandDistance, 1f, vector.magnitude), t);
			if (isFixHandTrans && weight >= 0.1f)
			{
				weight = 1f;
			}
			bool flag = isMainRight;
			isMainRight = Vector3.Dot(transform.right, vector.normalized) > 0f;
			bool flag2 = Vector3.Dot(transform.forward, transform2.forward) < 0f;
			if ((flag != isMainRight || flag2) && !isFixHandTrans)
			{
				weight = 0f;
			}
			handIK.PositionWeight(isMainRight, weight, isUseRotation);
			bool flag3 = (flag2 ? (!isMainRight) : isMainRight);
			handIK2.PositionWeight(!flag3, weight, isUseRotation);
			IKEffector iKEffector = ((!isMainRight) ? handIK.HandLeft : handIK.HandRight);
			IKEffector iKEffector2 = ((!flag3) ? handIK2.HandRight : handIK2.HandLeft);
			handsParent.position = Vector3.Lerp(iKEffector2.bone.position, iKEffector.bone.position, mainHandTargetWeight);
			handsChild.position = Vector3.Lerp(iKEffector.bone.position, iKEffector2.bone.position, weight - handPositionTrans);
			Transform target = iKEffector.target;
			Vector3 position = handsChild.position;
			iKEffector2.target.position = position;
			target.position = position;
			handsParent.rotation = Quaternion.Slerp(iKEffector.bone.rotation, iKEffector2.bone.rotation, crossFade);
			Transform target2 = iKEffector.target;
			Quaternion rotation = handsChild.rotation;
			iKEffector2.target.rotation = rotation;
			target2.rotation = rotation;
		}
	}
}
