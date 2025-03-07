using UnityEngine;

namespace ActionGame
{
	public class Sprites : MonoBehaviour
	{
		[SerializeField]
		protected Sprite[] _data;

		public Sprite this[int index]
		{
			get
			{
				return _data[index];
			}
		}
	}
}
