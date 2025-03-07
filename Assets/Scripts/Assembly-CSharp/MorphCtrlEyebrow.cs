using System;

[Serializable]
public class MorphCtrlEyebrow : MorphFaceBase
{
	public bool SyncBlink = true;

	public void CalcBlend(float blinkRate)
	{
		if (0f <= blinkRate && SyncBlink)
		{
			openRate = blinkRate;
		}
		CalculateBlendVertex();
	}
}
