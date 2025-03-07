using UnityEngine;

public class Twinkle : MonoBehaviour
{
	public GameObject cf_j_hand_R;

	public GameObject cf_j_index04_R;

	public GameObject handPos;

	public GameObject fingerPos;

	public float rate;

	public ParticleSystem[] handParticles;

	public ParticleSystem fingerParticle;

	private ParticleSystem.EmissionModule emObj;

	private void Update()
	{
		handPos.transform.position = (cf_j_hand_R.transform.position + cf_j_index04_R.transform.position) / 2f;
		fingerPos.transform.position = cf_j_index04_R.transform.position;
	}

	private void hand(int t)
	{
		for (int i = 0; i < handParticles.Length; i++)
		{
			emObj = handParticles[i].emission;
			emObj.rateOverDistance = new ParticleSystem.MinMaxCurve(rate);
			Invoke("hand_off", (float)t / 30f);
		}
	}

	private void finger(int t)
	{
		emObj = fingerParticle.emission;
		emObj.rateOverDistance = new ParticleSystem.MinMaxCurve(rate);
		Invoke("finger_off", (float)t / 30f);
	}

	private void hand_off()
	{
		for (int i = 0; i < handParticles.Length; i++)
		{
			emObj = handParticles[i].emission;
			emObj.rateOverDistance = new ParticleSystem.MinMaxCurve(0f);
		}
	}

	private void finger_off()
	{
		emObj = fingerParticle.emission;
		emObj.rateOverDistance = new ParticleSystem.MinMaxCurve(0f);
	}
}
