using UnityEngine;

public class KamifubukiLauncher : MonoBehaviour
{
	public GameObject particle;

	public Vector3[] position;

	private Animator cut;

	private float timer;

	private int id;

	private void Start()
	{
		cut = GetComponentInParent<Animator>();
		timer = 0f;
	}

	private void Update()
	{
		AnimatorStateInfo currentAnimatorStateInfo = cut.GetCurrentAnimatorStateInfo(0);
		timer += Time.deltaTime;
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("main") && timer > 2f)
		{
			id = 0;
			bomb();
			timer = 0f;
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("center") && timer > 2f)
		{
			id = 4;
			bomb();
			timer = 0f;
		}
	}

	private void bomb()
	{
		for (int i = id; i < id + 4; i++)
		{
			Object.Instantiate(particle, position[i], Quaternion.identity);
		}
	}
}
