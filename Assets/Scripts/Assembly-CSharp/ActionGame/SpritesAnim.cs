using System.Linq;
using UnityEngine;

namespace ActionGame
{
	public class SpritesAnim : MonoBehaviour
	{
		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private string[] _names;

		private int[] playParams;

		public Animator animator
		{
			get
			{
				return _animator;
			}
		}

		public string[] names
		{
			get
			{
				return _names;
			}
		}

		public bool isEnable
		{
			get
			{
				return animator.enabled;
			}
			set
			{
				animator.enabled = value;
			}
		}

		public void Play(int index)
		{
			if (animator.gameObject.activeInHierarchy)
			{
				animator.Play(playParams[index]);
			}
		}

		private void Awake()
		{
			if (names.Any())
			{
				playParams = names.Select(Animator.StringToHash).ToArray();
			}
		}
	}
}
