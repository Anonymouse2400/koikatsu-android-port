using UnityEngine;

public class BustSoft
{
	private ChaControl chaCtrl;

	private ChaInfo info;

	private float[] bustDamping = new float[3] { 0.2f, 0.1f, 0.1f };

	private float[] bustElasticity = new float[3] { 0.2f, 0.15f, 0.05f };

	private float[] bustStiffness = new float[3] { 1f, 0.1f, 0.01f };

	public BustSoft(ChaControl _ctrl)
	{
		chaCtrl = _ctrl;
		info = chaCtrl;
	}

	public void Change(float soft, params int[] changePtn)
	{
		if (!(null == chaCtrl) && !(null == info))
		{
			info.fileBody.bustSoftness = soft;
			ReCalc(changePtn);
		}
	}

	public void ReCalc(params int[] changePtn)
	{
		if (null == chaCtrl || null == info || changePtn.Length == 0)
		{
			return;
		}
		float value = info.fileBody.bustSoftness * info.fileBody.shapeValueBody[4] + 0.01f;
		value = Mathf.Clamp(value, 0f, 1f);
		float stiffness = TreeLerp(bustStiffness, value);
		float elasticity = TreeLerp(bustElasticity, value);
		float damping = TreeLerp(bustDamping, value);
		DynamicBone_Ver02[] array = new DynamicBone_Ver02[2]
		{
			chaCtrl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL),
			chaCtrl.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR)
		};
		foreach (int ptn in changePtn)
		{
			DynamicBone_Ver02[] array2 = array;
			foreach (DynamicBone_Ver02 dynamicBone_Ver in array2)
			{
				if (dynamicBone_Ver != null)
				{
					dynamicBone_Ver.setSoftParams(ptn, -1, damping, elasticity, stiffness);
				}
			}
		}
	}

	private float TreeLerp(float[] vals, float rate)
	{
		if (rate < 0.5f)
		{
			return Mathf.Lerp(vals[0], vals[1], rate * 2f);
		}
		return Mathf.Lerp(vals[1], vals[2], (rate - 0.5f) * 2f);
	}
}
