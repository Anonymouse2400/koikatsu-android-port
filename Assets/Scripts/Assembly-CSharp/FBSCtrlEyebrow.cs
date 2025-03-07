using System;

[Serializable]
public class FBSCtrlEyebrow : FBSBase
{
	public bool SyncBlink = true;

	public void CalcBlend(float blinkRate)
	{
		if (0f <= blinkRate && SyncBlink)
		{
			openRate = blinkRate;
		}
		CalculateBlendShape();
	}
}
