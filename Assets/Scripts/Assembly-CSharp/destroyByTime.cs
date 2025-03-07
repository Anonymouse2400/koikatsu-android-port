using UnityEngine;

public class destroyByTime : MonoBehaviour
{
	public float lifeTime;

	private void Start()
	{
		Object.Destroy(base.gameObject, lifeTime);
	}
}
