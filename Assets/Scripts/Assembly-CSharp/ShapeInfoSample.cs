using System;
using System.Collections.Generic;
using IllusionUtility.SetUtility;
using UnityEngine;

public class ShapeInfoSample : ShapeInfoBase
{
	public enum DstBoneName
	{
		cf_J_Root = 0,
		cf_J_Root_s = 1,
		cf_J_Spine01_s = 2,
		cf_J_Spine02_s = 3,
		cf_J_Spine03_s = 4,
		cf_J_Spine02_ss = 5
	}

	public enum SrcBoneName
	{
		cf_J_Root_height = 0,
		cf_J_Root_1 = 1,
		cf_J_Spine01_s_sx = 2,
		cf_J_Spine01_s_sz = 3,
		cf_J_Spine02_s_sx = 4,
		cf_J_Spine02_s_sz = 5,
		cf_J_Spine03_s_tx = 6,
		cf_J_Spine03_s_ty = 7,
		cf_J_Spine03_s_rz = 8,
		cf_J_Spine02_ss_ty = 9,
		cf_J_Spine02_ss_rx = 10
	}

	private bool initEnd;

	public override void InitShapeInfo(string assetBundleAnmKey, string assetBundleCategory, string resAnmKeyInfoPath, string resCateInfoPath, Transform trfObj)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		DstBoneName[] array = (DstBoneName[])Enum.GetValues(typeof(DstBoneName));
		DstBoneName[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			DstBoneName value = array2[i];
			dictionary[value.ToString()] = (int)value;
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		SrcBoneName[] array3 = (SrcBoneName[])Enum.GetValues(typeof(SrcBoneName));
		SrcBoneName[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			SrcBoneName value2 = array4[j];
			dictionary2[value2.ToString()] = (int)value2;
		}
		InitShapeInfoBase(assetBundleAnmKey, assetBundleCategory, resAnmKeyInfoPath, resCateInfoPath, trfObj, dictionary, dictionary2);
		initEnd = true;
	}

	public override void Update()
	{
		if (initEnd)
		{
			dictDst[0].trfBone.SetLocalScale(dictSrc[0].vctScl.x, dictSrc[0].vctScl.y, dictSrc[0].vctScl.z);
			dictDst[1].trfBone.SetLocalScale(dictSrc[1].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z);
			dictDst[2].trfBone.SetLocalScale(dictSrc[1].vctScl.x * dictSrc[2].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z * dictSrc[3].vctScl.z);
			dictDst[3].trfBone.SetLocalScale(dictSrc[1].vctScl.x * dictSrc[4].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z * dictSrc[5].vctScl.z);
			dictDst[4].trfBone.SetLocalPositionX(dictSrc[6].vctPos.x);
			dictDst[4].trfBone.SetLocalPositionY(dictSrc[7].vctPos.y);
			dictDst[4].trfBone.SetLocalRotation(0f, 0f, dictSrc[8].vctRot.z);
			dictDst[4].trfBone.SetLocalScale(dictSrc[1].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z);
			dictDst[5].trfBone.SetLocalPositionY(dictSrc[9].vctPos.y);
			dictDst[5].trfBone.SetLocalRotation(dictSrc[10].vctRot.x, 0f, 0f);
			dictDst[5].trfBone.SetLocalScale(1f, dictSrc[10].vctScl.y, 1f);
		}
	}

	public override void UpdateAlways()
	{
	}
}
