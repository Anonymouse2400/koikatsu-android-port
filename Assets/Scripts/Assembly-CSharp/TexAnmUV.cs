using UnityEngine;

public class TexAnmUV : MonoBehaviour
{
	public YS_Timer yst;

	public TexAnmUVParam ScrollU;

	public TexAnmUVParam ScrollV;

	private int passTimeU;

	private int passTimeV;

	private Renderer rendererData;

	private void Start()
	{
		rendererData = GetComponent<Renderer>();
		if (null == rendererData)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		float num = 0f;
		float num2 = 0f;
		if (ScrollU.Usage)
		{
			if (null == yst)
			{
				passTimeU += (int)(Time.deltaTime * 1000f);
				while (passTimeU >= ScrollU.ChangeTime)
				{
					passTimeU -= ScrollU.ChangeTime;
				}
				num = Mathf.InverseLerp(0f, ScrollU.ChangeTime, passTimeU);
			}
			else
			{
				num = yst.rate;
			}
			if (ScrollU.Plus)
			{
				num = 1f - num;
			}
			num += ScrollU.correct;
			if (num > 1f)
			{
				num -= 1f;
			}
		}
		if (ScrollV.Usage)
		{
			if (null == yst)
			{
				passTimeV += (int)(Time.deltaTime * 1000f);
				while (passTimeV >= ScrollV.ChangeTime)
				{
					passTimeV -= ScrollV.ChangeTime;
				}
				num2 = Mathf.InverseLerp(0f, ScrollV.ChangeTime, passTimeV);
			}
			else
			{
				num2 = yst.rate;
			}
			if (ScrollV.Plus)
			{
				num2 = 1f - num2;
			}
			num2 += ScrollV.correct;
			if (num2 > 1f)
			{
				num2 -= 1f;
			}
		}
		Vector2 value = new Vector2(num, 1f - num2);
		rendererData.material.SetTextureOffset("_MainTex", value);
	}
}
