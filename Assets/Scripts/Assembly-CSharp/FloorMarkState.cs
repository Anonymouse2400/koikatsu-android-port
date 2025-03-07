using UnityEngine;

public class FloorMarkState : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (Singleton<FloorMark>.IsInstance())
		{
			Singleton<FloorMark>.Instance.nowTag = animatorStateInfo.tagHash;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (Singleton<FloorMark>.IsInstance())
		{
			Singleton<FloorMark>.Instance.nowTag = 0;
		}
	}
}
