using UnityEngine;

public class MenuStateMachineBehaviour : StateMachineBehaviour
{
	public GameObject obj;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if ((bool)obj)
		{
			obj.SetActive(false);
		}
	}
}
