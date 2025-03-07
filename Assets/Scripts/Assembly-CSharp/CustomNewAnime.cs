using System;
using UnityEngine;

public class CustomNewAnime : MonoBehaviour
{
	private float deg;

	[SerializeField]
	private float baseScl = 0.8f;

	[SerializeField]
	private float rate = 180f;

	[SerializeField]
	private float add = 0.03f;

	private void Update()
	{
		deg += Time.deltaTime * rate;
		if (deg >= 360f)
		{
			deg -= 360f;
		}
		float num = baseScl + Mathf.Sin(deg * ((float)Math.PI / 180f)) * add;
		base.transform.localScale = new Vector3(num, num, 1f);
	}
}
