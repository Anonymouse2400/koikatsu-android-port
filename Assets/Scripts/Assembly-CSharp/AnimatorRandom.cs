using UnityEngine;

public class AnimatorRandom : StateMachineBehaviour
{
	private int hashRandom = Animator.StringToHash("Random");

	public int randomNum = 2;

	private float timeCount;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timeCount = 0f;
		int value = Random.Range(0, randomNum);
		animator.SetInteger(hashRandom, value);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime - timeCount > 1f)
		{
			timeCount = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			int value = Random.Range(0, randomNum);
			animator.SetInteger(hashRandom, value);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timeCount = 0f;
		int value = Random.Range(0, randomNum);
		animator.SetInteger(hashRandom, value);
	}
}
