using Illusion.CustomAttributes;
using UnityEngine;

public class HSpriteAutoDisable : MonoBehaviour
{
	[Label("消えるまでの時間")]
	public float timeDisable = 10f;

	private float time;

	private bool isFade;

	private void Update()
	{
		if (isFade)
		{
			time += Time.deltaTime;
			if (!(timeDisable > time))
			{
				base.gameObject.SetActive(false);
				isFade = false;
			}
		}
	}

	public void FadeStart()
	{
		time = 0f;
		isFade = true;
		base.gameObject.SetActive(true);
	}
}
