using RootMotion.FinalIK;
using UnityEngine;

namespace Studio
{
	public class Preparation : MonoBehaviour
	{
		[SerializeField]
		private FullBodyBipedIK _fullBodyIK;

		[SerializeField]
		private Transform _lookAtTarget;

		[SerializeField]
		private HandAnimeCtrl[] _handAnimeCtrl;

		[SerializeField]
		private CharAnimeCtrl _charAnimeCtrl;

		[SerializeField]
		private IKCtrl _IKCtrl;

		[SerializeField]
		private PVCopy _pvCopy;

		public FullBodyBipedIK fullBodyIK
		{
			get
			{
				return _fullBodyIK;
			}
		}

		public Transform lookAtTarget
		{
			get
			{
				return _lookAtTarget;
			}
		}

		public HandAnimeCtrl[] handAnimeCtrl
		{
			get
			{
				return _handAnimeCtrl;
			}
		}

		public CharAnimeCtrl charAnimeCtrl
		{
			get
			{
				return _charAnimeCtrl;
			}
		}

		public IKCtrl IKCtrl
		{
			get
			{
				return _IKCtrl;
			}
		}

		public PVCopy pvCopy
		{
			get
			{
				return _pvCopy;
			}
		}
	}
}
