using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public abstract class ChangeUIBase : MonoBehaviour
	{
		public const string onEnable = "OnEnable";

		public const string onDisable = "OnDisable";

		[SerializeField]
		protected Image target;

		[SerializeField]
		protected Animator animator;

		[SerializeField]
		protected Sprite[] sprites;

		private const float WAIT_TIME = 2f;

		protected virtual void Set(int i)
		{
			target.sprite = sprites[i];
			StartCoroutine(ChangeAnimation());
		}

		private IEnumerator ChangeAnimation()
		{
			yield return Play("OnEnable");
			yield return new WaitForSecondsRealtime(2f);
			yield return Play("OnDisable");
		}

		private IEnumerator Play(string state)
		{
			animator.Play(state);
			yield return null;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
	}
}
