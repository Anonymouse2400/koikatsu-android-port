using System;
using System.Collections.Generic;
using IllusionUtility.SetUtility;
using Manager;
using UnityEngine;

public class ShapeBodyInfoFemale : ShapeInfoBase
{
	public enum DstName
	{
		cf_n_height = 0,
		cf_s_hand_L = 1,
		cf_s_hand_R = 2,
		cf_s_head = 3,
		cf_s_neck = 4,
		cf_s_spine03 = 5,
		cf_s_shoulder02_L = 6,
		cf_s_shoulder02_R = 7,
		cf_s_arm01_L = 8,
		cf_s_arm01_R = 9,
		cf_s_arm02_L = 10,
		cf_s_arm02_R = 11,
		cf_s_arm03_L = 12,
		cf_s_arm03_R = 13,
		cf_s_forearm01_L = 14,
		cf_s_forearm01_R = 15,
		cf_s_forearm02_L = 16,
		cf_s_forearm02_R = 17,
		cf_s_wrist_L = 18,
		cf_s_wrist_R = 19,
		cf_s_spine02 = 20,
		cf_s_spine01 = 21,
		cf_s_waist01 = 22,
		cf_s_waist02 = 23,
		cf_s_siri_L = 24,
		cf_s_siri_R = 25,
		cf_s_thigh01_L = 26,
		cf_s_thigh01_R = 27,
		cf_s_thigh02_L = 28,
		cf_s_thigh02_R = 29,
		cf_s_thigh03_L = 30,
		cf_s_thigh03_R = 31,
		cf_s_leg01_L = 32,
		cf_s_leg01_R = 33,
		cf_s_leg02_L = 34,
		cf_s_leg02_R = 35,
		cf_s_leg03_L = 36,
		cf_s_leg03_R = 37,
		cf_d_kokan = 38,
		cf_s_bust00_L = 39,
		cf_d_bust01_L = 40,
		cf_d_bust02_L = 41,
		cf_d_bust03_L = 42,
		cf_s_bust01_L = 43,
		cf_s_bust02_L = 44,
		cf_s_bust03_L = 45,
		cf_hit_bust02_L = 46,
		cf_d_bnip01_L = 47,
		cf_s_bnip01_L = 48,
		cf_s_bnip025_L = 49,
		cf_s_bnip015_L = 50,
		cf_s_bnip02_L = 51,
		cf_s_bnipacc_L = 52,
		cf_s_bust00_R = 53,
		cf_d_bust01_R = 54,
		cf_d_bust02_R = 55,
		cf_d_bust03_R = 56,
		cf_s_bust01_R = 57,
		cf_s_bust02_R = 58,
		cf_s_bust03_R = 59,
		cf_hit_bust02_R = 60,
		cf_d_bnip01_R = 61,
		cf_s_bnip01_R = 62,
		cf_s_bnip025_R = 63,
		cf_s_bnip015_R = 64,
		cf_s_bnip02_R = 65,
		cf_s_bnipacc_R = 66,
		cf_hit_siri_L = 67,
		cf_hit_siri_R = 68,
		cf_hit_waist_L = 69,
		cf_hit_berry = 70,
		cf_hit_spine02_L = 71,
		cf_hit_shoulder_L = 72,
		cf_hit_shoulder_R = 73,
		cf_hit_arm_L = 74,
		cf_hit_arm_R = 75,
		cf_hit_spine01 = 76,
		cf_d_sk_top = 77,
		cf_d_sk_00_00 = 78,
		cf_d_sk_01_00 = 79,
		cf_d_sk_02_00 = 80,
		cf_d_sk_03_00 = 81,
		cf_d_sk_04_00 = 82,
		cf_d_sk_05_00 = 83,
		cf_d_sk_06_00 = 84,
		cf_d_sk_07_00 = 85
	}

	public enum SrcName
	{
		cf_a_height = 0,
		cf_a_height_aid = 1,
		cf_a_head = 2,
		cf_a_neck = 3,
		cf_a_spine03 = 4,
		cf_a_shoulder = 5,
		cf_a_shoulder_L_aid03 = 6,
		cf_a_shoulder_R_aid03 = 7,
		cf_a_arm_L_aid03 = 8,
		cf_a_arm_R_aid03 = 9,
		cf_a_arm02 = 10,
		cf_a_arm03_blend01 = 11,
		cf_a_arm03_blend02 = 12,
		cf_a_farm01 = 13,
		cf_a_farm02_blend01 = 14,
		cf_a_farm02_blend03 = 15,
		cf_a_farm03 = 16,
		cf_a_spine02 = 17,
		cf_a_spine02_aid_berry = 18,
		cf_a_spine01 = 19,
		cf_a_berry = 20,
		cf_a_waist01 = 21,
		cf_a_waist02 = 22,
		cf_a_siri = 23,
		cf_a_thigh01_L = 24,
		cf_a_thigh01_L_aid = 25,
		cf_a_thigh01_R = 26,
		cf_a_thigh01_R_aid = 27,
		cf_a_thigh02_L_blend01 = 28,
		cf_a_thigh02_L_blend03 = 29,
		cf_a_thigh02_R_blend01 = 30,
		cf_a_thigh02_R_blend03 = 31,
		cf_a_thigh03_L = 32,
		cf_a_thigh03_R = 33,
		cf_a_leg01_L = 34,
		cf_a_leg01_R = 35,
		cf_a_leg02_L = 36,
		cf_a_leg02_R = 37,
		cf_a_leg03 = 38,
		cf_a_dan = 39,
		cf_a_bust_ty = 40,
		cf_a_bust00_aid03_sz = 41,
		cf_a_bust00_aid02_sz = 42,
		cf_a_bust_L_ry = 43,
		cf_a_bust_rx = 44,
		cf_a_bust01_size = 45,
		cf_a_bust_L_tx = 46,
		cf_a_bust02_size = 47,
		cf_a_bust_tz = 48,
		cf_a_bust03_size = 49,
		cf_a_bust01_shape1 = 50,
		cf_a_bust02_shape1 = 51,
		cf_a_bust03_shape1 = 52,
		cf_a_hit_bust_shape1 = 53,
		cf_a_hit_bust_shape2 = 54,
		cf_a_bnip01 = 55,
		cf_a_bnip01_size = 56,
		cf_a_d_bnip01_size = 57,
		cf_a_bnip02_size = 58,
		cf_a_bnip015_size = 59,
		cf_a_bnip02 = 60,
		cf_a_bnipacc_stand = 61,
		cf_a_bnipacc_size = 62,
		cf_a_bust_R_ry = 63,
		cf_a_bust_R_tx = 64,
		cf_a_hit_siri_shape1 = 65,
		cf_a_hit_siri_shape2 = 66,
		cf_a_hit_siri_shape3 = 67,
		cf_a_hit_siri_shape4 = 68,
		cf_a_hit_siri_shape5 = 69,
		cf_a_hit_siri_shape6 = 70,
		cf_a_hit_waist_shape1 = 71,
		cf_a_hit_waist_shape2 = 72,
		cf_a_hit_waist_shape3 = 73,
		cf_a_hit_spinety_shape = 74,
		cf_a_hit_waist_shape4 = 75,
		cf_a_hit_waist_shape5 = 76,
		cf_a_hit_berry_shape = 77,
		cf_a_hit_berry_shape2 = 78,
		cf_a_hit_berry_shape3 = 79,
		cf_a_hit_spine02_shape1 = 80,
		cf_a_hit_spine02_shape2 = 81,
		cf_a_hit_spine02_shape3 = 82,
		cf_a_hit_shoulder_shape1 = 83,
		cf_a_hit_shoulder_shape2 = 84,
		cf_a_hit_shoulder_shape3 = 85,
		cf_a_hit_arm_shape2 = 86,
		cf_a_hit_arm_shape3 = 87,
		cf_a_hit_arm_shape4 = 88,
		cf_a_hit_spine01_shape1 = 89,
		cf_a_hit_spine01_shape2 = 90,
		cf_a_sk_00_00 = 91,
		cf_a_sk_00_01 = 92,
		cf_a_sk_berry = 93,
		cf_a_sk_thigh01_sz = 94,
		cf_a_sk_01_00 = 95,
		cf_a_sk_01_01 = 96,
		cf_a_sk_thigh01_sx = 97,
		cf_a_sk_02_00 = 98,
		cf_a_sk_02_01 = 99,
		cf_a_sk_siri = 100,
		cf_a_sk_03_00 = 101,
		cf_a_sk_03_01 = 102,
		cf_a_sk_04_00 = 103,
		cf_a_sk_04_01 = 104,
		cf_a_sk_05_00 = 105,
		cf_a_sk_05_01 = 106,
		cf_a_sk_06_00 = 107,
		cf_a_sk_06_01 = 108,
		cf_a_sk_07_00 = 109,
		cf_a_sk_07_01 = 110
	}

	public const int UPDATE_MASK_BUST_L = 1;

	public const int UPDATE_MASK_BUST_R = 2;

	public const int UPDATE_MASK_ETC = 4;

	public const int UPDATE_MASK_ALL = 7;

	public int updateMask = 7;

	public override void InitShapeInfo(string assetBundleAnmKey, string assetBundleCategory, string anmKeyInfoName, string cateInfoName, Transform trfObj)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		DstName[] array = (DstName[])Enum.GetValues(typeof(DstName));
		DstName[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			DstName value = array2[i];
			dictionary[value.ToString()] = (int)value;
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		SrcName[] array3 = (SrcName[])Enum.GetValues(typeof(SrcName));
		SrcName[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			SrcName value2 = array4[j];
			dictionary2[value2.ToString()] = (int)value2;
		}
		InitShapeInfoBase(assetBundleAnmKey, assetBundleCategory, anmKeyInfoName, cateInfoName, trfObj, dictionary, dictionary2, Singleton<Character>.Instance.AddLoadAssetBundle);
		base.InitEnd = true;
	}

	public override void Update()
	{
		if (!base.InitEnd || dictSrc.Count == 0)
		{
			return;
		}
		BoneInfo value = null;
		if ((updateMask & 4) != 0)
		{
			if (dictDst.TryGetValue(0, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[0].vctScl.x, dictSrc[0].vctScl.y, dictSrc[0].vctScl.z);
			}
			if (dictDst.TryGetValue(1, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[1].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(2, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[1].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(3, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[2].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[2].vctScl.x * dictSrc[1].vctScl.x, dictSrc[2].vctScl.y * dictSrc[1].vctScl.y, dictSrc[2].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(4, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[3].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[3].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[3].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(5, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[4].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[4].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[4].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(6, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[6].vctPos.x + dictSrc[5].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[5].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[5].vctScl.x, dictSrc[5].vctScl.y * dictSrc[1].vctScl.y, dictSrc[5].vctScl.z * dictSrc[6].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(7, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[7].vctPos.x - dictSrc[5].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[5].vctPos.y);
				value.trfBone.SetLocalScale(dictSrc[5].vctScl.x, dictSrc[5].vctScl.y * dictSrc[1].vctScl.y, dictSrc[5].vctScl.z * dictSrc[7].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(8, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[8].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[5].vctPos.y);
				value.trfBone.SetLocalScale(1f, dictSrc[5].vctScl.y * dictSrc[1].vctScl.y, dictSrc[5].vctScl.z * dictSrc[8].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(9, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[9].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[5].vctPos.y);
				value.trfBone.SetLocalScale(1f, dictSrc[5].vctScl.y * dictSrc[1].vctScl.y, dictSrc[5].vctScl.z * dictSrc[9].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(10, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[10].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[10].vctPos.z);
				value.trfBone.SetLocalScale(1f, dictSrc[10].vctScl.y * dictSrc[1].vctScl.y, dictSrc[10].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(11, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[10].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[10].vctPos.z);
				value.trfBone.SetLocalScale(1f, dictSrc[10].vctScl.y * dictSrc[1].vctScl.y, dictSrc[10].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(12, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[11].vctScl.y * dictSrc[12].vctScl.y * dictSrc[1].vctScl.y, dictSrc[11].vctScl.z * dictSrc[12].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(13, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[11].vctScl.y * dictSrc[12].vctScl.y * dictSrc[1].vctScl.y, dictSrc[11].vctScl.z * dictSrc[12].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(14, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[13].vctScl.y * dictSrc[1].vctScl.y, dictSrc[13].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(15, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[13].vctScl.y * dictSrc[1].vctScl.y, dictSrc[13].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(16, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[14].vctScl.y * dictSrc[15].vctScl.y * dictSrc[1].vctScl.y, dictSrc[14].vctScl.z * dictSrc[15].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(17, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[14].vctScl.y * dictSrc[15].vctScl.y * dictSrc[1].vctScl.y, dictSrc[14].vctScl.z * dictSrc[15].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(18, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[16].vctScl.y * dictSrc[1].vctScl.y, dictSrc[16].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(19, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[16].vctScl.y * dictSrc[1].vctScl.y, dictSrc[16].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(20, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[17].vctPos.z + dictSrc[18].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[17].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[17].vctScl.z * dictSrc[1].vctScl.z * dictSrc[18].vctScl.z);
			}
			if (dictDst.TryGetValue(21, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[19].vctPos.y + dictSrc[20].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[19].vctPos.z + dictSrc[20].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[20].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[19].vctScl.x * dictSrc[1].vctScl.x * dictSrc[20].vctScl.x, dictSrc[20].vctScl.y, dictSrc[19].vctScl.z * dictSrc[1].vctScl.z * dictSrc[20].vctScl.z);
			}
			if (dictDst.TryGetValue(22, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[20].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[21].vctPos.z + dictSrc[20].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[20].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[21].vctScl.x * dictSrc[1].vctScl.x * dictSrc[20].vctScl.x, dictSrc[20].vctScl.y, dictSrc[21].vctScl.z * dictSrc[1].vctScl.z * dictSrc[20].vctScl.z);
			}
			if (dictDst.TryGetValue(23, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[22].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[22].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[22].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(24, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[23].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[23].vctPos.z + dictSrc[22].vctPos.z * 0.3f);
				value.trfBone.SetLocalRotation(dictSrc[23].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[23].vctScl.x + (-1f + dictSrc[22].vctScl.x) * 0.5f, dictSrc[23].vctScl.y, dictSrc[23].vctScl.z + (-1f + dictSrc[22].vctScl.z) * 0.5f + (-1f + dictSrc[21].vctScl.z) * 0.5f);
			}
			if (dictDst.TryGetValue(25, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[23].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[23].vctPos.z + dictSrc[22].vctPos.z * 0.3f);
				value.trfBone.SetLocalRotation(dictSrc[23].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[23].vctScl.x + (-1f + dictSrc[22].vctScl.x) * 0.5f, dictSrc[23].vctScl.y, dictSrc[23].vctScl.z + (-1f + dictSrc[22].vctScl.z) * 0.5f + (-1f + dictSrc[21].vctScl.z) * 0.5f);
			}
			if (dictDst.TryGetValue(26, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[24].vctPos.x + dictSrc[25].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[24].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[24].vctRot.x, 0f, dictSrc[24].vctRot.z);
				value.trfBone.SetLocalScale(dictSrc[24].vctScl.x * dictSrc[25].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[24].vctScl.z * dictSrc[25].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(27, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[26].vctPos.x + dictSrc[27].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[26].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[26].vctRot.x, 0f, dictSrc[26].vctRot.z);
				value.trfBone.SetLocalScale(dictSrc[26].vctScl.x * dictSrc[27].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[26].vctScl.z * dictSrc[27].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(28, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[28].vctPos.x + dictSrc[29].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[28].vctPos.z + dictSrc[29].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[28].vctScl.x * dictSrc[29].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[28].vctScl.z * dictSrc[29].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(29, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[30].vctPos.x + dictSrc[31].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[30].vctPos.z + dictSrc[31].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[30].vctScl.x * dictSrc[31].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[30].vctScl.z * dictSrc[31].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(30, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[32].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[32].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[32].vctRot.x, 0f, dictSrc[32].vctRot.z);
				value.trfBone.SetLocalScale(dictSrc[32].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[32].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(31, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[33].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[33].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[33].vctRot.x, 0f, dictSrc[33].vctRot.z);
				value.trfBone.SetLocalScale(dictSrc[33].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[33].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(32, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[34].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[34].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[34].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[34].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(33, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[35].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[35].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[35].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[35].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(34, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[36].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[36].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[36].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[36].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(35, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[37].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[37].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[37].vctScl.x * dictSrc[1].vctScl.x, 1f, dictSrc[37].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(36, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[38].vctScl.x, 1f, dictSrc[38].vctScl.z);
			}
			if (dictDst.TryGetValue(37, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[38].vctScl.x, 1f, dictSrc[38].vctScl.z);
			}
			if (dictDst.TryGetValue(67, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[65].vctPos.x + dictSrc[66].vctPos.x + dictSrc[67].vctPos.x + dictSrc[68].vctPos.x + dictSrc[69].vctPos.x + dictSrc[70].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[67].vctPos.y + dictSrc[68].vctPos.y + dictSrc[70].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[65].vctPos.z + dictSrc[67].vctPos.z + dictSrc[70].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[65].vctScl.x * dictSrc[67].vctScl.x * dictSrc[68].vctScl.x * dictSrc[1].vctScl.x * dictSrc[70].vctScl.x, dictSrc[65].vctScl.y * dictSrc[67].vctScl.y * dictSrc[68].vctScl.y * dictSrc[1].vctScl.y * dictSrc[70].vctScl.x, dictSrc[65].vctScl.z * dictSrc[67].vctScl.z * dictSrc[68].vctScl.z * dictSrc[1].vctScl.z * dictSrc[70].vctScl.x);
			}
			if (dictDst.TryGetValue(68, out value))
			{
				value.trfBone.SetLocalPositionX(0f - dictSrc[65].vctPos.x - dictSrc[66].vctPos.x - dictSrc[67].vctPos.x - dictSrc[68].vctPos.x - dictSrc[69].vctPos.x - dictSrc[70].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[67].vctPos.y + dictSrc[68].vctPos.y + dictSrc[70].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[65].vctPos.z + dictSrc[67].vctPos.z + dictSrc[70].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[65].vctScl.x * dictSrc[67].vctScl.x * dictSrc[68].vctScl.x * dictSrc[1].vctScl.x * dictSrc[70].vctScl.x, dictSrc[65].vctScl.y * dictSrc[67].vctScl.y * dictSrc[68].vctScl.y * dictSrc[1].vctScl.y * dictSrc[70].vctScl.x, dictSrc[65].vctScl.z * dictSrc[67].vctScl.z * dictSrc[68].vctScl.z * dictSrc[1].vctScl.z * dictSrc[70].vctScl.x);
			}
			if (dictDst.TryGetValue(69, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[71].vctPos.y + dictSrc[72].vctPos.y + dictSrc[73].vctPos.y + dictSrc[74].vctPos.y + dictSrc[75].vctPos.y + dictSrc[76].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[72].vctPos.z + dictSrc[73].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[71].vctScl.x * dictSrc[72].vctScl.x * dictSrc[1].vctScl.x * dictSrc[73].vctScl.x * dictSrc[76].vctScl.x * dictSrc[75].vctScl.x, dictSrc[71].vctScl.x * dictSrc[72].vctScl.x * dictSrc[1].vctScl.y * dictSrc[73].vctScl.y * dictSrc[76].vctScl.y * dictSrc[75].vctScl.y, dictSrc[71].vctScl.x * dictSrc[72].vctScl.x * dictSrc[1].vctScl.z * dictSrc[73].vctScl.z * dictSrc[76].vctScl.z * dictSrc[75].vctScl.z);
			}
			if (dictDst.TryGetValue(70, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[77].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[77].vctPos.y + dictSrc[74].vctPos.y * 2f + dictSrc[78].vctPos.y + dictSrc[79].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[77].vctPos.z + dictSrc[78].vctPos.z + dictSrc[79].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[77].vctScl.x * dictSrc[78].vctScl.x * dictSrc[1].vctScl.x * dictSrc[79].vctScl.x, dictSrc[77].vctScl.y * dictSrc[78].vctScl.y * dictSrc[1].vctScl.y * dictSrc[79].vctScl.y, dictSrc[77].vctScl.z * dictSrc[78].vctScl.z * dictSrc[1].vctScl.z * dictSrc[79].vctScl.z);
			}
			if (dictDst.TryGetValue(71, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[80].vctPos.y + dictSrc[81].vctPos.y + dictSrc[82].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[82].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[81].vctScl.x * dictSrc[82].vctScl.x * dictSrc[1].vctScl.x, dictSrc[81].vctScl.y * dictSrc[82].vctScl.y * dictSrc[1].vctScl.y, dictSrc[81].vctScl.z * dictSrc[82].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(72, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[83].vctPos.x + dictSrc[84].vctPos.x + dictSrc[85].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[83].vctPos.y + dictSrc[84].vctPos.y + dictSrc[85].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[83].vctPos.z + dictSrc[84].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[83].vctScl.x * dictSrc[84].vctScl.x * dictSrc[85].vctScl.x * dictSrc[1].vctScl.x, dictSrc[83].vctScl.y * dictSrc[84].vctScl.y * dictSrc[85].vctScl.y * dictSrc[1].vctScl.y, dictSrc[83].vctScl.z * dictSrc[84].vctScl.z * dictSrc[85].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(73, out value))
			{
				value.trfBone.SetLocalPositionX(0f - dictSrc[83].vctPos.x - dictSrc[84].vctPos.x - dictSrc[85].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[83].vctPos.y + dictSrc[84].vctPos.y + dictSrc[85].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[83].vctPos.z + dictSrc[84].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[83].vctScl.x * dictSrc[84].vctScl.x * dictSrc[85].vctScl.x * dictSrc[1].vctScl.x, dictSrc[83].vctScl.y * dictSrc[84].vctScl.y * dictSrc[85].vctScl.y * dictSrc[1].vctScl.y, dictSrc[83].vctScl.z * dictSrc[84].vctScl.z * dictSrc[85].vctScl.z * dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(74, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[86].vctScl.y * dictSrc[87].vctScl.y * dictSrc[88].vctScl.y * dictSrc[1].vctScl.x, dictSrc[86].vctScl.z * dictSrc[87].vctScl.z * dictSrc[88].vctScl.z * dictSrc[1].vctScl.x);
			}
			if (dictDst.TryGetValue(75, out value))
			{
				value.trfBone.SetLocalScale(1f, dictSrc[86].vctScl.y * dictSrc[87].vctScl.y * dictSrc[88].vctScl.y * dictSrc[1].vctScl.x, dictSrc[86].vctScl.z * dictSrc[87].vctScl.z * dictSrc[88].vctScl.z * dictSrc[1].vctScl.x);
			}
			if (dictDst.TryGetValue(76, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[89].vctPos.z + dictSrc[90].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[89].vctScl.x * dictSrc[90].vctScl.x * dictSrc[1].vctScl.x, dictSrc[89].vctScl.y * dictSrc[90].vctScl.y * dictSrc[1].vctScl.y, dictSrc[89].vctScl.z * dictSrc[90].vctScl.z * dictSrc[1].vctScl.z);
			}
			float num = ((!(180f < dictSrc[94].vctRot.x)) ? dictSrc[94].vctRot.x : (dictSrc[94].vctRot.x - 360f));
			float num2 = ((!(180f < dictSrc[97].vctRot.x)) ? dictSrc[97].vctRot.x : (dictSrc[97].vctRot.x - 360f));
			if (dictDst.TryGetValue(77, out value))
			{
				value.trfBone.SetLocalScale(dictSrc[1].vctScl.x, dictSrc[1].vctScl.y, dictSrc[1].vctScl.z);
			}
			if (dictDst.TryGetValue(78, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[91].vctPos.x + dictSrc[92].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[91].vctPos.z + dictSrc[92].vctPos.z + dictSrc[93].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[91].vctRot.x + dictSrc[92].vctRot.x + dictSrc[92].vctRot.z + num, dictSrc[91].vctRot.y, dictSrc[91].vctRot.z);
			}
			if (dictDst.TryGetValue(79, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[95].vctPos.x + dictSrc[96].vctPos.x - dictSrc[93].vctPos.x * 0.5f);
				value.trfBone.SetLocalPositionZ(dictSrc[95].vctPos.z + dictSrc[96].vctPos.z + dictSrc[93].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[95].vctRot.x + dictSrc[96].vctRot.x + dictSrc[96].vctRot.z + num * 0.6f + num2 * 0.6f, dictSrc[95].vctRot.y, dictSrc[95].vctRot.z);
			}
			if (dictDst.TryGetValue(80, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[98].vctPos.x + dictSrc[99].vctPos.x + dictSrc[100].vctPos.x * 0.5f - dictSrc[93].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[98].vctPos.z + dictSrc[99].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[98].vctRot.x + dictSrc[99].vctRot.x + dictSrc[99].vctRot.z + num2, dictSrc[98].vctRot.y, dictSrc[98].vctRot.z);
			}
			if (dictDst.TryGetValue(81, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[101].vctPos.x + dictSrc[102].vctPos.x + dictSrc[100].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[101].vctPos.z + dictSrc[102].vctPos.z + dictSrc[100].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[101].vctRot.x + dictSrc[102].vctRot.x + dictSrc[102].vctRot.z + num * 0.6f + num2 * 0.6f + dictSrc[100].vctRot.x, dictSrc[101].vctRot.y, dictSrc[101].vctRot.z);
			}
			if (dictDst.TryGetValue(82, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[103].vctPos.x + dictSrc[104].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[103].vctPos.z + dictSrc[104].vctPos.z + dictSrc[100].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[103].vctRot.x + dictSrc[104].vctRot.x + dictSrc[104].vctRot.z + num + dictSrc[100].vctRot.x, dictSrc[103].vctRot.y, dictSrc[103].vctRot.z);
			}
			if (dictDst.TryGetValue(83, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[105].vctPos.x + dictSrc[106].vctPos.x - dictSrc[100].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[105].vctPos.z + dictSrc[106].vctPos.z + dictSrc[100].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[105].vctRot.x + dictSrc[106].vctRot.x + dictSrc[106].vctRot.z + num * 0.6f + num2 * 0.6f + dictSrc[100].vctRot.x, dictSrc[105].vctRot.y, dictSrc[105].vctRot.z);
			}
			if (dictDst.TryGetValue(84, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[107].vctPos.x + dictSrc[108].vctPos.x - dictSrc[100].vctPos.x * 0.5f + dictSrc[93].vctPos.x);
				value.trfBone.SetLocalPositionZ(dictSrc[107].vctPos.z + dictSrc[108].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[107].vctRot.x + dictSrc[108].vctRot.x + dictSrc[108].vctRot.z + num2, dictSrc[107].vctRot.y, dictSrc[107].vctRot.z);
			}
			if (dictDst.TryGetValue(85, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[109].vctPos.x + dictSrc[110].vctPos.x + dictSrc[93].vctPos.x * 0.5f);
				value.trfBone.SetLocalPositionZ(dictSrc[109].vctPos.z + dictSrc[110].vctPos.z + dictSrc[93].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[109].vctRot.x + dictSrc[110].vctRot.x + dictSrc[110].vctRot.z + num * 0.6f + num2 * 0.6f, dictSrc[109].vctRot.y, dictSrc[109].vctRot.z);
			}
		}
		if ((updateMask & 1) != 0)
		{
			if (dictDst.TryGetValue(39, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[40].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[41].vctPos.z + dictSrc[42].vctPos.z + dictSrc[43].vctPos.z + dictSrc[40].vctPos.z + dictSrc[44].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[44].vctRot.x, 0f, 0f);
			}
			if (dictDst.TryGetValue(40, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[46].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[45].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[45].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[45].vctRot.x, dictSrc[43].vctRot.y + dictSrc[45].vctRot.y, 0f);
				value.trfBone.SetLocalScale(dictSrc[45].vctScl.x, dictSrc[45].vctScl.y, dictSrc[45].vctScl.z);
			}
			if (dictDst.TryGetValue(41, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[47].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[47].vctPos.z + dictSrc[48].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[47].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[47].vctScl.x, dictSrc[47].vctScl.y, dictSrc[47].vctScl.z);
			}
			if (dictDst.TryGetValue(42, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[49].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[49].vctPos.z + dictSrc[48].vctPos.z / 2f);
				value.trfBone.SetLocalRotation(dictSrc[49].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[49].vctScl.x, dictSrc[49].vctScl.y, dictSrc[49].vctScl.z);
			}
			if (dictDst.TryGetValue(43, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[50].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[50].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[50].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[50].vctRot.x, dictSrc[50].vctRot.y, 0f);
				value.trfBone.SetLocalScale(dictSrc[50].vctScl.x, dictSrc[50].vctScl.y, dictSrc[48].vctScl.z);
			}
			if (dictDst.TryGetValue(44, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[51].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[51].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[51].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[51].vctRot.x, dictSrc[51].vctRot.y, 0f);
				value.trfBone.SetLocalScale(dictSrc[51].vctScl.x * dictSrc[48].vctScl.x, dictSrc[51].vctScl.y * dictSrc[48].vctScl.y, dictSrc[51].vctScl.z);
			}
			if (dictDst.TryGetValue(45, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[52].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[52].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[52].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[52].vctScl.x * dictSrc[48].vctScl.x, dictSrc[52].vctScl.y * dictSrc[48].vctScl.y, 1f);
			}
			if (dictDst.TryGetValue(46, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[53].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[53].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[53].vctPos.z + dictSrc[54].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[53].vctScl.x * dictSrc[54].vctScl.x, dictSrc[53].vctScl.y * dictSrc[54].vctScl.x, dictSrc[53].vctScl.z * dictSrc[54].vctScl.x);
			}
			if (dictDst.TryGetValue(47, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[55].vctPos.z + dictSrc[56].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[57].vctScl.x, dictSrc[57].vctScl.x, dictSrc[57].vctScl.z);
			}
			if (dictDst.TryGetValue(48, out value))
			{
				value.trfBone.SetLocalPositionZ((0f - (dictSrc[55].vctPos.z - 0.01f)) * 1.2f);
				value.trfBone.SetLocalScale(dictSrc[55].vctScl.x * dictSrc[56].vctScl.x, dictSrc[55].vctScl.y * dictSrc[56].vctScl.y, dictSrc[55].vctScl.z * dictSrc[56].vctScl.z);
			}
			if (dictDst.TryGetValue(49, out value))
			{
				value.trfBone.SetLocalScale(1f / dictSrc[55].vctScl.x * (dictSrc[58].vctScl.x * 1.2f), 1f / dictSrc[55].vctScl.y * (dictSrc[58].vctScl.y * 1.2f), 1f / dictSrc[55].vctScl.z);
			}
			if (dictDst.TryGetValue(50, out value))
			{
				value.trfBone.SetLocalPositionZ(0.0025f + dictSrc[59].vctPos.z - (dictSrc[55].vctPos.z - 0.01f) * 1f);
				value.trfBone.SetLocalScale(0.1f + dictSrc[59].vctScl.y * dictSrc[59].vctScl.x, 0.1f + dictSrc[59].vctScl.y * dictSrc[59].vctScl.x, 0.1f + dictSrc[59].vctScl.z * dictSrc[59].vctScl.x);
			}
			if (dictDst.TryGetValue(51, out value))
			{
				value.trfBone.SetLocalPositionZ(0.004f + dictSrc[60].vctPos.z + dictSrc[55].vctPos.x + dictSrc[58].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[60].vctScl.x * dictSrc[58].vctScl.x, dictSrc[60].vctScl.y * dictSrc[58].vctScl.y, dictSrc[60].vctScl.z * dictSrc[58].vctScl.z);
			}
			if (dictDst.TryGetValue(52, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[61].vctPos.z);
				value.trfBone.SetLocalScale(1f * dictSrc[61].vctScl.x / dictSrc[60].vctScl.x / dictSrc[58].vctScl.x * dictSrc[62].vctScl.x, 1f * dictSrc[61].vctScl.y / dictSrc[60].vctScl.y / dictSrc[58].vctScl.y * dictSrc[62].vctScl.y, 1f * dictSrc[61].vctScl.z / dictSrc[60].vctScl.z / dictSrc[58].vctScl.z * dictSrc[62].vctScl.z);
			}
		}
		if ((updateMask & 2) != 0)
		{
			if (dictDst.TryGetValue(53, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[40].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[41].vctPos.z + dictSrc[42].vctPos.z + dictSrc[43].vctPos.z + dictSrc[40].vctPos.z + dictSrc[44].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[44].vctRot.x, 0f, 0f);
			}
			if (dictDst.TryGetValue(54, out value))
			{
				value.trfBone.SetLocalPositionX(dictSrc[64].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[45].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[45].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[45].vctRot.x, dictSrc[63].vctRot.y - dictSrc[45].vctRot.y, 0f);
				value.trfBone.SetLocalScale(dictSrc[45].vctScl.x, dictSrc[45].vctScl.y, dictSrc[45].vctScl.z);
			}
			if (dictDst.TryGetValue(55, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[47].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[47].vctPos.z + dictSrc[48].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[47].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[47].vctScl.x, dictSrc[47].vctScl.y, dictSrc[47].vctScl.z);
			}
			if (dictDst.TryGetValue(56, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[49].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[49].vctPos.z + dictSrc[48].vctPos.z / 2f);
				value.trfBone.SetLocalRotation(dictSrc[49].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[49].vctScl.x, dictSrc[49].vctScl.y, dictSrc[49].vctScl.z);
			}
			if (dictDst.TryGetValue(57, out value))
			{
				value.trfBone.SetLocalPositionX(0f - dictSrc[50].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[50].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[50].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[50].vctRot.x, 0f - dictSrc[50].vctRot.y, 0f);
				value.trfBone.SetLocalScale(dictSrc[50].vctScl.x, dictSrc[50].vctScl.y, dictSrc[48].vctScl.z);
			}
			if (dictDst.TryGetValue(58, out value))
			{
				value.trfBone.SetLocalPositionX(0f - dictSrc[51].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[51].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[51].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[51].vctRot.x, 0f - dictSrc[51].vctRot.y, 0f);
				value.trfBone.SetLocalScale(dictSrc[51].vctScl.x * dictSrc[48].vctScl.x, dictSrc[51].vctScl.y * dictSrc[48].vctScl.y, dictSrc[51].vctScl.z);
			}
			if (dictDst.TryGetValue(59, out value))
			{
				value.trfBone.SetLocalPositionY(dictSrc[52].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[52].vctPos.z);
				value.trfBone.SetLocalRotation(dictSrc[52].vctRot.x, 0f, 0f);
				value.trfBone.SetLocalScale(dictSrc[52].vctScl.x * dictSrc[48].vctScl.x, dictSrc[52].vctScl.y * dictSrc[48].vctScl.y, 1f);
			}
			if (dictDst.TryGetValue(60, out value))
			{
				value.trfBone.SetLocalPositionX(0f - dictSrc[53].vctPos.x);
				value.trfBone.SetLocalPositionY(dictSrc[53].vctPos.y);
				value.trfBone.SetLocalPositionZ(dictSrc[53].vctPos.z + dictSrc[54].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[53].vctScl.x * dictSrc[54].vctScl.x, dictSrc[53].vctScl.y * dictSrc[54].vctScl.x, dictSrc[53].vctScl.z * dictSrc[54].vctScl.x);
			}
			if (dictDst.TryGetValue(61, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[55].vctPos.z + dictSrc[56].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[57].vctScl.x, dictSrc[57].vctScl.x, dictSrc[57].vctScl.z);
			}
			if (dictDst.TryGetValue(62, out value))
			{
				value.trfBone.SetLocalPositionZ((0f - (dictSrc[55].vctPos.z - 0.01f)) * 1.2f);
				value.trfBone.SetLocalScale(dictSrc[55].vctScl.x * dictSrc[56].vctScl.x, dictSrc[55].vctScl.y * dictSrc[56].vctScl.y, dictSrc[55].vctScl.z * dictSrc[56].vctScl.z);
			}
			if (dictDst.TryGetValue(63, out value))
			{
				value.trfBone.SetLocalScale(1f / dictSrc[55].vctScl.x * (dictSrc[58].vctScl.x * 1.2f), 1f / dictSrc[55].vctScl.y * (dictSrc[58].vctScl.y * 1.2f), 1f / dictSrc[55].vctScl.z);
			}
			if (dictDst.TryGetValue(64, out value))
			{
				value.trfBone.SetLocalPositionZ(0.0025f + dictSrc[59].vctPos.z - (dictSrc[55].vctPos.z - 0.01f) * 1f);
				value.trfBone.SetLocalScale(0.1f + dictSrc[59].vctScl.y * dictSrc[59].vctScl.x, 0.1f + dictSrc[59].vctScl.y * dictSrc[59].vctScl.x, 0.1f + dictSrc[59].vctScl.z * dictSrc[59].vctScl.x);
			}
			if (dictDst.TryGetValue(65, out value))
			{
				value.trfBone.SetLocalPositionZ(0.004f + dictSrc[60].vctPos.z + dictSrc[55].vctPos.x + dictSrc[58].vctPos.z);
				value.trfBone.SetLocalScale(dictSrc[60].vctScl.x * dictSrc[58].vctScl.x, dictSrc[60].vctScl.y * dictSrc[58].vctScl.y, dictSrc[60].vctScl.z * dictSrc[58].vctScl.z);
			}
			if (dictDst.TryGetValue(66, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[61].vctPos.z);
				value.trfBone.SetLocalScale(1f * dictSrc[61].vctScl.x / dictSrc[60].vctScl.x / dictSrc[58].vctScl.x * dictSrc[62].vctScl.x, 1f * dictSrc[61].vctScl.y / dictSrc[60].vctScl.y / dictSrc[58].vctScl.y * dictSrc[62].vctScl.y, 1f * dictSrc[61].vctScl.z / dictSrc[60].vctScl.z / dictSrc[58].vctScl.z * dictSrc[62].vctScl.z);
			}
		}
	}

	public override void UpdateAlways()
	{
		if (base.InitEnd && dictSrc.Count != 0)
		{
			BoneInfo value = null;
			if (dictDst.TryGetValue(38, out value))
			{
				value.trfBone.SetLocalPositionZ(dictSrc[39].vctPos.z);
			}
		}
	}
}
