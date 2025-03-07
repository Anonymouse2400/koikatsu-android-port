using UnityEngine;

public class SpotLaser : MonoBehaviour
{
	public float offset;

	private Animator anim;

	private GameObject child;

	private float beam;

	private void Start()
	{
		anim = GetComponent<Animator>();
		anim.SetFloat("offset", offset);
		child = base.gameObject.transform.Find("spotBeam_gp/spotBeam_active").gameObject;
	}

	private void Update()
	{
		if (child.activeSelf)
		{
			beam = 1f;
		}
		else
		{
			beam = 0f;
		}
		anim.SetFloat("beam", beam);
	}
}
