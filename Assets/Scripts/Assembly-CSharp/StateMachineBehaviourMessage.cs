using UnityEngine;
//using UnityEngine.Animations;
using UnityEngine.Experimental.Director;
//using UnityEngine.Experimental.Director;

public class StateMachineBehaviourMessage : StateMachineBehaviour
{
	private static void Output(string eventName, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Output("OnStateEnter", animator, stateInfo, layerIndex);
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
	{
		Output("OnStateEnter(controller)", animator, stateInfo, layerIndex);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Output("OnStateExit", animator, stateInfo, layerIndex);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
	{
		Output("OnStateExit(controller)", animator, stateInfo, layerIndex);
	}

	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Output("OnStateIK", animator, stateInfo, layerIndex);
	}

	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
	{
		Output("OnStateIK(controller)", animator, stateInfo, layerIndex);
	}

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
	}

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
	{
	}

	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
	}

	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
	{
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Output("OnStateMove", animator, stateInfo, layerIndex);
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
	{
		Output("OnStateMove(controller)", animator, stateInfo, layerIndex);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Output("OnStateUpdate", animator, stateInfo, layerIndex);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
	{
		Output("OnStateUpdate(controller)", animator, stateInfo, layerIndex);
	}
}
