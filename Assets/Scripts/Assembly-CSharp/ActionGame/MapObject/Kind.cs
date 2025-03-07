using Illusion;
using UnityEngine;

namespace ActionGame.MapObject
{
	public class Kind : MonoBehaviour
	{
		[SerializeField]
		[Header("Hカテゴリー")]
		private int[] _categoryes;

		[SerializeField]
		[Header("対")]
		private Transform[] _targets;

		[Header("グループ")]
		[SerializeField]
		private Transform[] _groups;

		[Header("オフセット位置")]
		[SerializeField]
		private Vector3 _offsetPos;

		[Header("オフセット回転")]
		[SerializeField]
		private Vector3 _offsetAngle;

		private Vector3 backupPos;

		private Vector3 backupAngle;

		public int category
		{
			get
			{
				return _categoryes.SafeGet(0);
			}
		}

		public int[] categoryes
		{
			get
			{
				return _categoryes;
			}
		}

		public Transform[] targets
		{
			get
			{
				return _targets;
			}
		}

		public Transform[] groups
		{
			get
			{
				return _groups;
			}
		}

		public Vector3 offsetPos
		{
			get
			{
				return _offsetPos;
			}
		}

		public Vector3 offsetAngle
		{
			get
			{
				return _offsetAngle;
			}
		}

		public void MoveOffset()
		{
			ResetPosition();
			base.transform.Translate(_offsetPos);
			base.transform.Rotate(_offsetAngle);
		}

		public void ResetPosition()
		{
			base.transform.position = backupPos;
			base.transform.eulerAngles = backupAngle;
		}

		private void Awake()
		{
			backupPos = base.transform.position;
			backupAngle = base.transform.eulerAngles;
		}

		private void OnDrawGizmos()
		{
			if (_offsetPos.magnitude > 1E-05f || _offsetAngle.magnitude > 1E-05f)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Vector3 vector = base.transform.position + _offsetPos;
				float num = 0.25f;
				Gizmos.DrawSphere(vector, num);
				Utils.Gizmos.Axis(vector, Quaternion.Euler(base.transform.eulerAngles + _offsetAngle), num);
			}
		}
	}
}
