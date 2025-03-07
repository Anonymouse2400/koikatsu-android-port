using System;
using UnityEngine;

[Serializable]
public class MatAnmSimpleInfo
{
	public YS_Timer yst;

	public MeshRenderer mr;

	public int materialNo;

	public float time;

	public bool change_RGB = true;

	public bool change_A = true;

	public Gradient Value;

	private float cnt;

	public void Update(int colorPropertyId)
	{
		if (null == mr)
		{
			return;
		}
		float num = 0f;
		if (null == yst)
		{
			cnt += Time.deltaTime;
			while (time < cnt)
			{
				cnt -= time;
			}
			num = cnt / time;
		}
		else
		{
			num = yst.rate;
		}
		Color color = mr.material.GetColor(colorPropertyId);
		if (change_RGB)
		{
			color.r = Value.Evaluate(num).r;
			color.g = Value.Evaluate(num).g;
			color.b = Value.Evaluate(num).b;
		}
		if (change_A)
		{
			color.a = Value.Evaluate(num).a;
		}
		mr.material.SetColor(colorPropertyId, color);
	}
}
