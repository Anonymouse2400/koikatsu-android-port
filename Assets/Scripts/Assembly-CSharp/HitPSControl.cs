using UnityEngine;

public class HitPSControl : MonoBehaviour
{
	private void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		Object.Destroy(base.gameObject, component.main.duration);
	}

	private void Update()
	{
	}
}
