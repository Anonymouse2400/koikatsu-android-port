using UnityEngine;

public class YS_Timer : MonoBehaviour
{
	public float time = 1f;

	public float rate;

	private float cnt;

	private void Start()
	{
	}

	private void Update()
	{
		cnt += Time.deltaTime;
		while (cnt > time)
		{
			cnt -= time;
		}
		rate = cnt / time;
	}
}
