using UnityEngine;

namespace Illusion.Component.Correct
{
	public class BaseData : MonoBehaviour
	{
		public Transform bone;

		[SerializeField]
		private Vector3 _pos;

		[SerializeField]
		private Quaternion _rot;

		public Vector3 pos
		{
			get
			{
				return _pos;
			}
			set
			{
				_pos = value;
			}
		}

		public Vector3 ang
		{
			get
			{
				return _rot.eulerAngles;
			}
			set
			{
				_rot = Quaternion.Euler(value);
			}
		}

		public Quaternion rot
		{
			get
			{
				return _rot;
			}
			set
			{
				_rot = value;
			}
		}

		private void Reset()
		{
			bone = null;
			_pos = Vector3.zero;
			_rot = Quaternion.identity;
		}
	}
}
