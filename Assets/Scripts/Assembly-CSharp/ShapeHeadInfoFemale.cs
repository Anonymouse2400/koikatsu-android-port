using System;
using System.Collections.Generic;
using IllusionUtility.SetUtility;
using Manager;
using UnityEngine;

public class ShapeHeadInfoFemale : ShapeInfoBase
{
	public enum DstBoneName
	{
		cf_J_FaceBase = 0,
		cf_J_FaceUp_tz = 1,
		cf_J_NoseBridge_ty = 2,
		cf_J_FaceUp_ty = 3,
		cf_J_FaceLow_tz = 4,
		cf_J_FaceLow_sx = 5,
		cf_J_CheekUp2_L = 6,
		cf_J_CheekUp2_R = 7,
		cf_J_ChinLow = 8,
		cf_J_CheekLow_s_L = 9,
		cf_J_CheekLow_s_R = 10,
		cf_J_Chin_Base = 11,
		cf_J_ChinTip_Base = 12,
		cf_J_Nose_tip = 13,
		cf_J_NoseBase_rx = 14,
		cf_J_NoseBridge_rx = 15,
		cf_J_CheekUpBase = 16,
		cf_J_CheekUp_s_L = 17,
		cf_J_CheekUp_s_R = 18,
		cf_J_Eye_tx_L = 19,
		cf_J_Eye_tx_R = 20,
		cf_J_megane_rx_ear = 21,
		cf_J_Eye_rz_L = 22,
		cf_J_Eye_rz_R = 23,
		cf_J_Eye_tz = 24,
		cf_J_Eye01_s_L = 25,
		cf_J_Eye01_s_R = 26,
		cf_J_Eye05_s_L = 27,
		cf_J_Eye05_s_R = 28,
		cf_J_Eye02_s_L = 29,
		cf_J_Eye02_s_R = 30,
		cf_J_Eye03_s_L = 31,
		cf_J_Eye03_s_R = 32,
		cf_J_Eye04_s_L = 33,
		cf_J_Eye04_s_R = 34,
		cf_J_Eye08_s_L = 35,
		cf_J_Eye08_s_R = 36,
		cf_J_Eye07_s_L = 37,
		cf_J_Eye07_s_R = 38,
		cf_J_Eye06_s_L = 39,
		cf_J_Eye06_s_R = 40,
		cf_J_Mayu_L = 41,
		cf_J_Mayu_R = 42,
		cf_J_MayuMid_s_L = 43,
		cf_J_MayuMid_s_R = 44,
		cf_J_MayuTip_s_L = 45,
		cf_J_MayuTip_s_R = 46,
		cf_J_EarBase_ry_L = 47,
		cf_J_EarBase_ry_R = 48,
		cf_J_EarUp_L = 49,
		cf_J_EarUp_R = 50,
		cf_J_EarLow_L = 51,
		cf_J_EarLow_R = 52,
		cf_J_MouthBase_ty = 53,
		cf_J_Mouth_L = 54,
		cf_J_Mouth_R = 55,
		cf_J_MouthBase_rx = 56,
		cf_J_Mouthup = 57,
		cf_J_MouthLow = 58
	}

	public enum SrcBoneName
	{
		cf_J_FaceBase = 0,
		cf_J_FaceUp_tz = 1,
		cf_J_NoseBridge_ty = 2,
		cf_J_FaceUp_ty = 3,
		cf_J_FaceLow_tz = 4,
		cf_J_FaceLow_sx = 5,
		cf_J_CheekUp2_L = 6,
		cf_J_CheekUp2_R = 7,
		cf_J_ChinLow = 8,
		cf_J_CheekLow_s_L = 9,
		cf_J_CheekLow_s_R = 10,
		cf_J_Chin_Base = 11,
		cf_J_ChinTip_Base = 12,
		cf_J_Nose_tip = 13,
		cf_J_NoseBase_rx = 14,
		cf_J_NoseBridge_rx = 15,
		cf_J_CheekUpBase = 16,
		cf_J_CheekUp_s_L = 17,
		cf_J_CheekUp_s_R = 18,
		cf_J_Eye_tx_L = 19,
		cf_J_Eye_tx_R = 20,
		cf_J_megane_rx_ear = 21,
		cf_J_Eye_rz_L = 22,
		cf_J_Eye_rz_R = 23,
		cf_J_Eye_tz = 24,
		cf_J_Eye01_s_L = 25,
		cf_J_Eye01_s_R = 26,
		cf_J_Eye05_s_L = 27,
		cf_J_Eye05_s_R = 28,
		cf_J_Eye02_s_L = 29,
		cf_J_Eye02_s_R = 30,
		cf_J_Eye03_s_L = 31,
		cf_J_Eye03_s_R = 32,
		cf_J_Eye04_s_L = 33,
		cf_J_Eye04_s_R = 34,
		cf_J_Eye08_s_L = 35,
		cf_J_Eye08_s_R = 36,
		cf_J_Eye07_s_L = 37,
		cf_J_Eye07_s_R = 38,
		cf_J_Eye06_s_L = 39,
		cf_J_Eye06_s_R = 40,
		cf_J_Mayu_L = 41,
		cf_J_Mayu_R = 42,
		cf_J_MayuMid_s_L = 43,
		cf_J_MayuMid_s_R = 44,
		cf_J_MayuTip_s_L = 45,
		cf_J_MayuTip_s_R = 46,
		cf_J_EarBase_ry_L = 47,
		cf_J_EarBase_ry_R = 48,
		cf_J_EarUp_L = 49,
		cf_J_EarUp_R = 50,
		cf_J_EarLow_L = 51,
		cf_J_EarLow_R = 52,
		cf_J_MouthBase_ty = 53,
		cf_J_Mouth_L = 54,
		cf_J_Mouth_R = 55,
		cf_J_MouthBase_rx = 56,
		cf_J_Mouthup = 57,
		cf_J_MouthLow = 58
	}

	public override void InitShapeInfo(string assetBundleAnmKey, string assetBundleCategory, string anmKeyInfoName, string cateInfoName, Transform trfObj)
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
		InitShapeInfoBase(assetBundleAnmKey, assetBundleCategory, anmKeyInfoName, cateInfoName, trfObj, dictionary, dictionary2, Singleton<Character>.Instance.AddLoadAssetBundle);
		base.InitEnd = true;
	}

	public override void Update()
	{
		if (base.InitEnd && dictSrc.Count != 0)
		{
			BoneInfo value = null;
			if (dictDst.TryGetValue(0, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[0].vctScl.x, 1f, 1f);
			}
			if (dictDst.TryGetValue(1, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[1].vctPos.z);
			}
			if (dictDst.TryGetValue(2, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[2].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[2].vctPos.z);
			}
			if (dictDst.TryGetValue(3, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[3].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[3].vctScl.x, dictSrc[3].vctScl.y, dictSrc[3].vctScl.z);
			}
			if (dictDst.TryGetValue(4, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[4].vctPos.z);
			}
			if (dictDst.TryGetValue(5, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[5].vctScl.x, 1f, 1f);
			}
			if (dictDst.TryGetValue(6, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[6].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[6].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[6].vctScl.x, 1f, 1f);
			}
			if (dictDst.TryGetValue(7, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[7].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[7].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[7].vctScl.x, 1f, 1f);
			}
			if (dictDst.TryGetValue(8, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[8].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[8].vctScl.x, dictSrc[8].vctScl.y, dictSrc[8].vctScl.z);
			}
			if (dictDst.TryGetValue(9, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[9].vctPos.z);
				value.trfBone.SetLocalScale(1f, 1f, dictSrc[9].vctScl.z);
			}
			if (dictDst.TryGetValue(10, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[10].vctPos.z);
				value.trfBone.SetLocalScale(1f, 1f, dictSrc[10].vctScl.z);
			}
			if (dictDst.TryGetValue(11, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[11].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[11].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[11].vctScl.x, 1f, 1f);
			}
			if (dictDst.TryGetValue(12, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[12].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[12].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[12].vctScl.x, 1f, 1f);
			}
			if (dictDst.TryGetValue(13, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[13].vctPos.z);
			}
			if (dictDst.TryGetValue(14, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[14].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[14].vctPos.z);
			}
			if (dictDst.TryGetValue(15, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[15].vctPos.z);
			}
			if (dictDst.TryGetValue(16, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[16].vctPos.y);
			}
			if (dictDst.TryGetValue(17, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[17].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[17].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[17].vctPos.z);
			}
			if (dictDst.TryGetValue(18, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[18].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[18].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[18].vctPos.z);
			}
			if (dictDst.TryGetValue(19, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[19].vctPos.y);
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[19].vctRot.z);
			}
			if (dictDst.TryGetValue(20, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[20].vctPos.y);
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[20].vctRot.z);
			}
			if (dictDst.TryGetValue(21, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[21].vctPos.y);
				value.trfBone.SetLocalRotation(dictSrc[21].vctRot.x, 0f, 0f);
			}
			if (dictDst.TryGetValue(22, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[22].vctPos.x);
				value.trfBone.SetLocalScale(dictSrc[22].vctScl.x, dictSrc[22].vctScl.y, 1f);
			}
			if (dictDst.TryGetValue(23, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[23].vctPos.x);
				value.trfBone.SetLocalScale(dictSrc[23].vctScl.x, dictSrc[23].vctScl.y, 1f);
			}
			if (dictDst.TryGetValue(24, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[24].vctPos.z);
			}
			if (dictDst.TryGetValue(25, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[25].vctPos.x);
			}
			if (dictDst.TryGetValue(26, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[26].vctPos.x);
			}
			if (dictDst.TryGetValue(27, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[27].vctPos.y);
			}
			if (dictDst.TryGetValue(28, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[28].vctPos.y);
			}
			if (dictDst.TryGetValue(29, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[29].vctPos.y);
			}
			if (dictDst.TryGetValue(30, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[30].vctPos.y);
			}
			if (dictDst.TryGetValue(31, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[31].vctPos.y);
			}
			if (dictDst.TryGetValue(32, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[32].vctPos.y);
			}
			if (dictDst.TryGetValue(33, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[33].vctPos.y);
			}
			if (dictDst.TryGetValue(34, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[34].vctPos.y);
			}
			if (dictDst.TryGetValue(35, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[35].vctPos.y);
			}
			if (dictDst.TryGetValue(36, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[36].vctPos.y);
			}
			if (dictDst.TryGetValue(37, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[37].vctPos.y);
			}
			if (dictDst.TryGetValue(38, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[38].vctPos.y);
			}
			if (dictDst.TryGetValue(39, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[39].vctPos.y);
			}
			if (dictDst.TryGetValue(40, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[40].vctPos.y);
			}
			if (dictDst.TryGetValue(41, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[41].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[41].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[41].vctPos.z);
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[41].vctRot.z);
			}
			if (dictDst.TryGetValue(42, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[42].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[42].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[42].vctPos.z);
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[42].vctRot.z);
			}
			if (dictDst.TryGetValue(43, out value))
			{
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[43].vctRot.z);
			}
			if (dictDst.TryGetValue(44, out value))
			{
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[44].vctRot.z);
			}
			if (dictDst.TryGetValue(45, out value))
			{
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[45].vctRot.z);
			}
			if (dictDst.TryGetValue(46, out value))
			{
				value.trfBone.SetLocalRotation(0f, 0f, dictSrc[46].vctRot.z);
			}
			if (dictDst.TryGetValue(47, out value))
			{
				value.trfBone.SetLocalRotation(0f, dictSrc[47].vctRot.y, dictSrc[47].vctRot.z);
				value.trfBone.SetLocalScale(dictSrc[47].vctScl.x, dictSrc[47].vctScl.y, dictSrc[47].vctScl.z);
			}
			if (dictDst.TryGetValue(48, out value))
			{
				value.trfBone.SetLocalRotation(0f, dictSrc[48].vctRot.y, dictSrc[48].vctRot.z);
				value.trfBone.SetLocalScale(dictSrc[48].vctScl.x, dictSrc[48].vctScl.y, dictSrc[48].vctScl.z);
			}
			if (dictDst.TryGetValue(49, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[49].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[49].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[49].vctScl.x, dictSrc[49].vctScl.y, dictSrc[49].vctScl.z);
			}
			if (dictDst.TryGetValue(50, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[50].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[50].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[50].vctScl.x, dictSrc[50].vctScl.y, dictSrc[50].vctScl.z);
			}
			if (dictDst.TryGetValue(51, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[51].vctPos.y);
			}
			if (dictDst.TryGetValue(52, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[52].vctPos.y);
			}
			if (dictDst.TryGetValue(53, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[53].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[53].vctPos.z);
			}
			if (dictDst.TryGetValue(54, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[54].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[54].vctPos.y);
			}
			if (dictDst.TryGetValue(55, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[55].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[55].vctPos.y);
			}
			if (dictDst.TryGetValue(56, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[56].vctPos.z);
			}
			if (dictDst.TryGetValue(57, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[57].vctPos.z);
			}
			if (dictDst.TryGetValue(58, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[58].vctPos.z);
			}
		}
	}

	public override void UpdateAlways()
	{
	}
}
