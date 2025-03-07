using UnityEngine;

public class electricLight : MonoBehaviour
{
	private Material mat;

	private Animator cut;

	private void Start()
	{
		cut = GetComponentInParent<Animator>();
		mat = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		AnimatorStateInfo currentAnimatorStateInfo = cut.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("fast"))
		{
			mat.SetFloat("_FullLighting", 0f);
			mat.SetFloat("_TextureChange", 0f);
			mat.SetFloat("_ScrollSpeed", 4f);
			mat.SetFloat("_ColorPhaseSpeed", 5f);
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("slow"))
		{
			mat.SetFloat("_FullLighting", 0f);
			mat.SetFloat("_TextureChange", 1f);
			mat.SetFloat("_ScrollSpeed", 1f);
			mat.SetFloat("_ColorPhaseSpeed", 1f);
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("stop"))
		{
			mat.SetFloat("_FullLighting", 1f);
			mat.SetFloat("_ColorPhaseSpeed", 2f);
		}
	}
}
