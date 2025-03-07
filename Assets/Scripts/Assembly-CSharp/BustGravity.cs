using UnityEngine;

public class BustGravity
{
	private ChaControl chaCtrl;

	private ChaInfo info;

	private float[] range = new float[2] { 0f, -0.005f };

	public BustGravity(ChaControl _ctrl)
	{
		chaCtrl = _ctrl;
		info = chaCtrl;
	}

	public void Change(float gravity, params int[] changePtn)
	{
		if (!(null == chaCtrl) && !(null == info))
		{
			info.fileBody.bustWeight = gravity;
			ReCalc(changePtn);
		}
	}

	public void ReCalc(params int[] changePtn)
	{
		if (null == chaCtrl || null == info || changePtn.Length == 0)
		{
			return;
		}
		float num = info.fileBody.shapeValueBody[4] * info.fileBody.bustSoftness * 0.5f;
		float y = Mathf.Lerp(range[0], range[1], info.fileBody.bustWeight) * num;
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
					dynamicBone_Ver.setGravity(ptn, new Vector3(0f, y, 0f));
				}
			}
		}
	}
}
