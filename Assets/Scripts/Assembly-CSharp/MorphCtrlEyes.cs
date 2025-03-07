using System;

[Serializable]
public class MorphCtrlEyes : MorphFaceBase
{
	public void CalcBlend(float blinkRate)
	{
		if (0f <= blinkRate)
		{
			openRate = blinkRate;
		}
		CalculateBlendVertex();
	}
}
