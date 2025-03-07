using UnityEngine;

namespace ActionGame.MapObject
{
	public class KindGroup : MonoBehaviour
	{
		[SerializeField]
		private Kind[] _kinds;

		public Kind[] kinds
		{
			get
			{
				return _kinds;
			}
		}
	}
}
