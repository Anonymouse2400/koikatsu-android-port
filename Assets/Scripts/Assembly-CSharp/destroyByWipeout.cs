using UnityEngine;

public class destroyByWipeout : MonoBehaviour
{
	private GameObject childParticle;

	private ParticleSystem ps;

	private ParticleSystem.Particle[] p;

	private void Start()
	{
		childParticle = base.transform.Find("fxConfetti").gameObject;
		ps = childParticle.GetComponent<ParticleSystem>();
		p = new ParticleSystem.Particle[ps.main.maxParticles];
	}

	private void LateUpdate()
	{
		if (ps.GetParticles(p) <= 0)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
