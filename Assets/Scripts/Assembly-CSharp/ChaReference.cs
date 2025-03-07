using System.Collections.Generic;
using UnityEngine;

public class ChaReference : BaseLoader
{
	public enum RefObjKey
	{
		HeadParent = 0,
		HairParent = 1,
		a_n_hair_pony = 2,
		a_n_hair_twin_L = 3,
		a_n_hair_twin_R = 4,
		a_n_hair_pin = 5,
		a_n_hair_pin_R = 6,
		a_n_headtop = 7,
		a_n_headflont = 8,
		a_n_head = 9,
		a_n_headside = 10,
		a_n_megane = 11,
		a_n_earrings_L = 12,
		a_n_earrings_R = 13,
		a_n_nose = 14,
		a_n_mouth = 15,
		a_n_neck = 16,
		a_n_bust_f = 17,
		a_n_bust = 18,
		a_n_nip_L = 19,
		a_n_nip_R = 20,
		a_n_back = 21,
		a_n_back_L = 22,
		a_n_back_R = 23,
		a_n_waist = 24,
		a_n_waist_f = 25,
		a_n_waist_b = 26,
		a_n_waist_L = 27,
		a_n_waist_R = 28,
		a_n_leg_L = 29,
		a_n_leg_R = 30,
		a_n_knee_L = 31,
		a_n_knee_R = 32,
		a_n_ankle_L = 33,
		a_n_ankle_R = 34,
		a_n_heel_L = 35,
		a_n_heel_R = 36,
		a_n_shoulder_L = 37,
		a_n_shoulder_R = 38,
		a_n_elbo_L = 39,
		a_n_elbo_R = 40,
		a_n_arm_L = 41,
		a_n_arm_R = 42,
		a_n_wrist_L = 43,
		a_n_wrist_R = 44,
		a_n_hand_L = 45,
		a_n_hand_R = 46,
		a_n_ind_L = 47,
		a_n_ind_R = 48,
		a_n_mid_L = 49,
		a_n_mid_R = 50,
		a_n_ring_L = 51,
		a_n_ring_R = 52,
		a_n_dan = 53,
		a_n_kokan = 54,
		a_n_ana = 55,
		k_f_handL_00 = 56,
		k_f_handR_00 = 57,
		k_f_shoulderL_00 = 58,
		k_f_shoulderR_00 = 59,
		ObjEyeline = 60,
		ObjEyelineLow = 61,
		ObjEyebrow = 62,
		ObjNoseline = 63,
		ObjEyeL = 64,
		ObjEyeR = 65,
		ObjEyeWL = 66,
		ObjEyeWR = 67,
		ObjFace = 68,
		ObjDoubleTooth = 69,
		ObjBody = 70,
		ObjNip = 71,
		N_FaceSpecial = 72,
		CORRECT_ARM_L = 73,
		CORRECT_ARM_R = 74,
		CORRECT_HAND_L = 75,
		CORRECT_HAND_R = 76,
		S_ANA = 77,
		S_TongueF = 78,
		S_TongueB = 79,
		S_Son = 80,
		S_SimpleTop = 81,
		S_SimpleBody = 82,
		S_SimpleTongue = 83,
		S_MNPA = 84,
		S_MNPB = 85,
		S_MOZ_ALL = 86,
		S_GOMU = 87,
		S_CTOP_T_DEF = 88,
		S_CTOP_T_NUGE = 89,
		S_CTOP_B_DEF = 90,
		S_CTOP_B_NUGE = 91,
		S_CBOT_T_DEF = 92,
		S_CBOT_T_NUGE = 93,
		S_CBOT_B_DEF = 94,
		S_CBOT_B_NUGE = 95,
		S_UWT_T_DEF = 96,
		S_UWT_T_NUGE = 97,
		S_UWT_B_DEF = 98,
		S_UWT_B_NUGE = 99,
		S_UWB_T_DEF = 100,
		S_UWB_T_NUGE = 101,
		S_UWB_B_DEF = 102,
		S_UWB_B_NUGE = 103,
		S_UWB_B_NUGE2 = 104,
		S_PANST_DEF = 105,
		S_PANST_NUGE = 106,
		S_TPARTS_00_DEF = 107,
		S_TPARTS_00_NUGE = 108,
		S_TPARTS_01_DEF = 109,
		S_TPARTS_01_NUGE = 110,
		S_TPARTS_02_DEF = 111,
		S_TPARTS_02_NUGE = 112,
		ObjBraDef = 113,
		ObjBraNuge = 114,
		ObjInnerDef = 115,
		ObjInnerNuge = 116,
		S_TEARS_01 = 117,
		S_TEARS_02 = 118,
		S_TEARS_03 = 119,
		N_EyeBase = 120,
		N_Hitomi = 121,
		N_Gag00 = 122,
		N_Gag01 = 123,
		N_Gag02 = 124,
		DB_SKIRT_TOP = 125,
		DB_SKIRT_TOPA = 126,
		DB_SKIRT_TOPB = 127,
		DB_SKIRT_BOT = 128,
		F_ADJUSTWIDTHSCALE = 129,
		A_ROOTBONE = 130,
		BUSTUP_TARGET = 131,
		NECK_LOOK_TARGET = 132
	}

	public const ulong FbxTypeBodyBone = 1uL;

	public const ulong FbxTypeHeadBone = 2uL;

	public const ulong FbxTypeBody = 3uL;

	public const ulong FbxTypeHead = 4uL;

	public const ulong FbxTypeCTop = 5uL;

	public const ulong FbxTypeCBot = 6uL;

	public const ulong FbxTypeBra = 7uL;

	public const ulong FbxTypeShorts = 8uL;

	public const ulong FbxTypeGloves = 9uL;

	public const ulong FbxTypePanst = 10uL;

	public const ulong FbxTypeSocks = 11uL;

	public const ulong FbxTypeShoes = 12uL;

	public const ulong FbxTypeCTopA = 13uL;

	public const ulong FbxTypeCTopB = 14uL;

	public const ulong FbxTypeCTopC = 15uL;

	private Dictionary<RefObjKey, GameObject> dictRefObj = new Dictionary<RefObjKey, GameObject>();

	public void Log_ReferenceObjectNull()
	{
		if (dictRefObj == null)
		{
			return;
		}
		foreach (KeyValuePair<RefObjKey, GameObject> item in dictRefObj)
		{
			if (null == item.Value)
			{
				string text = "There is no " + item.Key.ToString() + ".";
			}
		}
	}

	public void CreateReferenceInfo(ulong flags, GameObject objRef)
	{
		ReleaseRefObject(flags);
		if (null == objRef)
		{
			return;
		}
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(objRef.transform);
		if ((long)flags >= 1L && (long)flags <= 15L)
		{
			switch (flags - 1)
			{
			case 1uL:
				dictRefObj[RefObjKey.a_n_hair_pony] = findAssist.GetObjectFromName("a_n_hair_pony");
				dictRefObj[RefObjKey.a_n_hair_twin_L] = findAssist.GetObjectFromName("a_n_hair_twin_L");
				dictRefObj[RefObjKey.a_n_hair_twin_R] = findAssist.GetObjectFromName("a_n_hair_twin_R");
				dictRefObj[RefObjKey.a_n_hair_pin] = findAssist.GetObjectFromName("a_n_hair_pin");
				dictRefObj[RefObjKey.a_n_hair_pin_R] = findAssist.GetObjectFromName("a_n_hair_pin_R");
				dictRefObj[RefObjKey.a_n_headtop] = findAssist.GetObjectFromName("a_n_headtop");
				dictRefObj[RefObjKey.a_n_headside] = findAssist.GetObjectFromName("a_n_headside");
				dictRefObj[RefObjKey.a_n_head] = findAssist.GetObjectFromName("a_n_head");
				dictRefObj[RefObjKey.a_n_headflont] = findAssist.GetObjectFromName("a_n_headflont");
				dictRefObj[RefObjKey.a_n_megane] = findAssist.GetObjectFromName("a_n_megane");
				dictRefObj[RefObjKey.a_n_earrings_L] = findAssist.GetObjectFromName("a_n_earrings_L");
				dictRefObj[RefObjKey.a_n_earrings_R] = findAssist.GetObjectFromName("a_n_earrings_R");
				dictRefObj[RefObjKey.a_n_nose] = findAssist.GetObjectFromName("a_n_nose");
				dictRefObj[RefObjKey.a_n_mouth] = findAssist.GetObjectFromName("a_n_mouth");
				dictRefObj[RefObjKey.F_ADJUSTWIDTHSCALE] = findAssist.GetObjectFromName("cf_J_MouthBase_rx");
				dictRefObj[RefObjKey.HairParent] = findAssist.GetObjectFromName("cf_J_FaceUp_ty");
				break;
			case 3uL:
				dictRefObj[RefObjKey.ObjFace] = findAssist.GetObjectFromName("cf_O_face");
				dictRefObj[RefObjKey.ObjEyeline] = findAssist.GetObjectFromName("cf_O_eyeline");
				dictRefObj[RefObjKey.ObjEyelineLow] = findAssist.GetObjectFromName("cf_O_eyeline_low");
				dictRefObj[RefObjKey.ObjEyebrow] = findAssist.GetObjectFromName("cf_O_mayuge");
				dictRefObj[RefObjKey.ObjNoseline] = findAssist.GetObjectFromName("cf_O_noseline");
				dictRefObj[RefObjKey.ObjEyeL] = findAssist.GetObjectFromName("cf_Ohitomi_L02");
				dictRefObj[RefObjKey.ObjEyeR] = findAssist.GetObjectFromName("cf_Ohitomi_R02");
				dictRefObj[RefObjKey.ObjEyeWL] = findAssist.GetObjectFromName("cf_Ohitomi_L");
				dictRefObj[RefObjKey.ObjEyeWR] = findAssist.GetObjectFromName("cf_Ohitomi_R");
				dictRefObj[RefObjKey.ObjDoubleTooth] = findAssist.GetObjectFromName("cf_O_canine");
				dictRefObj[RefObjKey.S_TongueF] = findAssist.GetObjectFromName("o_tang");
				dictRefObj[RefObjKey.S_TEARS_01] = findAssist.GetObjectFromName("cf_O_namida_S");
				dictRefObj[RefObjKey.S_TEARS_02] = findAssist.GetObjectFromName("cf_O_namida_M");
				dictRefObj[RefObjKey.S_TEARS_03] = findAssist.GetObjectFromName("cf_O_namida_L");
				dictRefObj[RefObjKey.N_EyeBase] = findAssist.GetObjectFromName("n_eyebase");
				dictRefObj[RefObjKey.N_Hitomi] = findAssist.GetObjectFromName("n_hitomi");
				dictRefObj[RefObjKey.N_Gag00] = findAssist.GetObjectFromName("cf_O_gag_eye_00");
				dictRefObj[RefObjKey.N_Gag01] = findAssist.GetObjectFromName("cf_O_gag_eye_01");
				dictRefObj[RefObjKey.N_Gag02] = findAssist.GetObjectFromName("cf_O_gag_eye_02");
				dictRefObj[RefObjKey.N_FaceSpecial] = findAssist.GetObjectFromName("n_special");
				break;
			case 0uL:
				dictRefObj[RefObjKey.a_n_neck] = findAssist.GetObjectFromName("a_n_neck");
				dictRefObj[RefObjKey.a_n_bust_f] = findAssist.GetObjectFromName("a_n_bust_f");
				dictRefObj[RefObjKey.a_n_bust] = findAssist.GetObjectFromName("a_n_bust");
				dictRefObj[RefObjKey.a_n_nip_L] = findAssist.GetObjectFromName("a_n_nip_L");
				dictRefObj[RefObjKey.a_n_nip_R] = findAssist.GetObjectFromName("a_n_nip_R");
				dictRefObj[RefObjKey.a_n_back] = findAssist.GetObjectFromName("a_n_back");
				dictRefObj[RefObjKey.a_n_back_L] = findAssist.GetObjectFromName("a_n_back_L");
				dictRefObj[RefObjKey.a_n_back_R] = findAssist.GetObjectFromName("a_n_back_R");
				dictRefObj[RefObjKey.a_n_waist] = findAssist.GetObjectFromName("a_n_waist");
				dictRefObj[RefObjKey.a_n_waist_f] = findAssist.GetObjectFromName("a_n_waist_f");
				dictRefObj[RefObjKey.a_n_waist_b] = findAssist.GetObjectFromName("a_n_waist_b");
				dictRefObj[RefObjKey.a_n_waist_L] = findAssist.GetObjectFromName("a_n_waist_L");
				dictRefObj[RefObjKey.a_n_waist_R] = findAssist.GetObjectFromName("a_n_waist_R");
				dictRefObj[RefObjKey.a_n_leg_L] = findAssist.GetObjectFromName("a_n_leg_L");
				dictRefObj[RefObjKey.a_n_leg_R] = findAssist.GetObjectFromName("a_n_leg_R");
				dictRefObj[RefObjKey.a_n_knee_L] = findAssist.GetObjectFromName("a_n_knee_L");
				dictRefObj[RefObjKey.a_n_knee_R] = findAssist.GetObjectFromName("a_n_knee_R");
				dictRefObj[RefObjKey.a_n_ankle_L] = findAssist.GetObjectFromName("a_n_ankle_L");
				dictRefObj[RefObjKey.a_n_ankle_R] = findAssist.GetObjectFromName("a_n_ankle_R");
				dictRefObj[RefObjKey.a_n_heel_L] = findAssist.GetObjectFromName("a_n_heel_L");
				dictRefObj[RefObjKey.a_n_heel_R] = findAssist.GetObjectFromName("a_n_heel_R");
				dictRefObj[RefObjKey.a_n_shoulder_L] = findAssist.GetObjectFromName("a_n_shoulder_L");
				dictRefObj[RefObjKey.a_n_shoulder_R] = findAssist.GetObjectFromName("a_n_shoulder_R");
				dictRefObj[RefObjKey.a_n_elbo_L] = findAssist.GetObjectFromName("a_n_elbo_L");
				dictRefObj[RefObjKey.a_n_elbo_R] = findAssist.GetObjectFromName("a_n_elbo_R");
				dictRefObj[RefObjKey.a_n_arm_L] = findAssist.GetObjectFromName("a_n_arm_L");
				dictRefObj[RefObjKey.a_n_arm_R] = findAssist.GetObjectFromName("a_n_arm_R");
				dictRefObj[RefObjKey.a_n_wrist_L] = findAssist.GetObjectFromName("a_n_wrist_L");
				dictRefObj[RefObjKey.a_n_wrist_R] = findAssist.GetObjectFromName("a_n_wrist_R");
				dictRefObj[RefObjKey.a_n_hand_L] = findAssist.GetObjectFromName("a_n_hand_L");
				dictRefObj[RefObjKey.a_n_hand_R] = findAssist.GetObjectFromName("a_n_hand_R");
				dictRefObj[RefObjKey.a_n_ind_L] = findAssist.GetObjectFromName("a_n_ind_L");
				dictRefObj[RefObjKey.a_n_ind_R] = findAssist.GetObjectFromName("a_n_ind_R");
				dictRefObj[RefObjKey.a_n_mid_L] = findAssist.GetObjectFromName("a_n_mid_L");
				dictRefObj[RefObjKey.a_n_mid_R] = findAssist.GetObjectFromName("a_n_mid_R");
				dictRefObj[RefObjKey.a_n_ring_L] = findAssist.GetObjectFromName("a_n_ring_L");
				dictRefObj[RefObjKey.a_n_ring_R] = findAssist.GetObjectFromName("a_n_ring_R");
				dictRefObj[RefObjKey.a_n_dan] = findAssist.GetObjectFromName("a_n_dan");
				dictRefObj[RefObjKey.a_n_kokan] = findAssist.GetObjectFromName("a_n_kokan");
				dictRefObj[RefObjKey.a_n_ana] = findAssist.GetObjectFromName("a_n_ana");
				dictRefObj[RefObjKey.k_f_handL_00] = findAssist.GetObjectFromName("k_f_handL_00");
				dictRefObj[RefObjKey.k_f_handR_00] = findAssist.GetObjectFromName("k_f_handR_00");
				dictRefObj[RefObjKey.k_f_shoulderL_00] = findAssist.GetObjectFromName("k_f_shoulderL_00");
				dictRefObj[RefObjKey.k_f_shoulderR_00] = findAssist.GetObjectFromName("k_f_shoulderR_00");
				dictRefObj[RefObjKey.A_ROOTBONE] = findAssist.GetObjectFromName("cf_j_hips");
				dictRefObj[RefObjKey.HeadParent] = findAssist.GetObjectFromName("cf_s_head");
				dictRefObj[RefObjKey.BUSTUP_TARGET] = findAssist.GetObjectFromName("cf_j_spine03");
				dictRefObj[RefObjKey.NECK_LOOK_TARGET] = findAssist.GetObjectFromName("cf_s_spine03");
				dictRefObj[RefObjKey.CORRECT_ARM_L] = findAssist.GetObjectFromName("cf_j_forearm01_L");
				dictRefObj[RefObjKey.CORRECT_ARM_R] = findAssist.GetObjectFromName("cf_j_forearm01_R");
				dictRefObj[RefObjKey.CORRECT_HAND_L] = findAssist.GetObjectFromName("cf_j_hand_L");
				dictRefObj[RefObjKey.CORRECT_HAND_R] = findAssist.GetObjectFromName("cf_j_hand_R");
				dictRefObj[RefObjKey.S_ANA] = findAssist.GetObjectFromName("cf_s_ana");
				break;
			case 2uL:
				dictRefObj[RefObjKey.ObjBody] = findAssist.GetObjectFromName("o_body_a");
				dictRefObj[RefObjKey.ObjNip] = findAssist.GetObjectFromName("o_nip");
				dictRefObj[RefObjKey.S_TongueB] = findAssist.GetObjectFromName("o_tang");
				dictRefObj[RefObjKey.S_Son] = findAssist.GetObjectFromName("n_dankon");
				dictRefObj[RefObjKey.S_MNPA] = findAssist.GetObjectFromName("o_mnpa");
				dictRefObj[RefObjKey.S_MNPB] = findAssist.GetObjectFromName("o_mnpb");
				dictRefObj[RefObjKey.S_MOZ_ALL] = findAssist.GetObjectFromName("n_mnpb");
				dictRefObj[RefObjKey.S_GOMU] = findAssist.GetObjectFromName("o_gomu");
				dictRefObj[RefObjKey.S_SimpleTop] = findAssist.GetObjectFromName("n_silhouetteTop");
				dictRefObj[RefObjKey.S_SimpleBody] = findAssist.GetObjectFromName("n_body_silhouette");
				dictRefObj[RefObjKey.S_SimpleTongue] = findAssist.GetObjectFromName("n_tang_silhouette");
				break;
			case 4uL:
				dictRefObj[RefObjKey.S_CTOP_T_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_CTOP_T_NUGE] = findAssist.GetObjectFromName("n_top_b");
				dictRefObj[RefObjKey.S_CTOP_B_DEF] = findAssist.GetObjectFromName("n_bot_a");
				dictRefObj[RefObjKey.S_CTOP_B_NUGE] = findAssist.GetObjectFromName("n_bot_b");
				dictRefObj[RefObjKey.DB_SKIRT_TOP] = findAssist.GetObjectFromName("db_skirt");
				break;
			case 12uL:
				dictRefObj[RefObjKey.S_TPARTS_00_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_TPARTS_00_NUGE] = findAssist.GetObjectFromName("n_top_b");
				dictRefObj[RefObjKey.ObjInnerDef] = findAssist.GetObjectFromName("o_inner_a");
				dictRefObj[RefObjKey.ObjInnerNuge] = findAssist.GetObjectFromName("o_inner_b");
				dictRefObj[RefObjKey.DB_SKIRT_TOPA] = findAssist.GetObjectFromName("db_skirt");
				break;
			case 13uL:
				dictRefObj[RefObjKey.S_TPARTS_01_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_TPARTS_01_NUGE] = findAssist.GetObjectFromName("n_top_b");
				dictRefObj[RefObjKey.DB_SKIRT_TOPB] = findAssist.GetObjectFromName("db_skirt");
				break;
			case 14uL:
				dictRefObj[RefObjKey.S_TPARTS_02_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_TPARTS_02_NUGE] = findAssist.GetObjectFromName("n_top_b");
				break;
			case 5uL:
				dictRefObj[RefObjKey.S_CBOT_T_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_CBOT_T_NUGE] = findAssist.GetObjectFromName("n_top_b");
				dictRefObj[RefObjKey.S_CBOT_B_DEF] = findAssist.GetObjectFromName("n_bot_a");
				dictRefObj[RefObjKey.S_CBOT_B_NUGE] = findAssist.GetObjectFromName("n_bot_b");
				dictRefObj[RefObjKey.DB_SKIRT_BOT] = findAssist.GetObjectFromName("db_skirt");
				break;
			case 6uL:
				dictRefObj[RefObjKey.S_UWT_T_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_UWT_T_NUGE] = findAssist.GetObjectFromName("n_top_b");
				dictRefObj[RefObjKey.S_UWT_B_DEF] = findAssist.GetObjectFromName("n_bot_a");
				dictRefObj[RefObjKey.S_UWT_B_NUGE] = findAssist.GetObjectFromName("n_bot_b");
				dictRefObj[RefObjKey.ObjBraDef] = findAssist.GetObjectFromName("o_bra_a");
				dictRefObj[RefObjKey.ObjBraNuge] = findAssist.GetObjectFromName("o_bra_b");
				break;
			case 7uL:
				dictRefObj[RefObjKey.S_UWB_T_DEF] = findAssist.GetObjectFromName("n_top_a");
				dictRefObj[RefObjKey.S_UWB_T_NUGE] = findAssist.GetObjectFromName("n_top_b");
				dictRefObj[RefObjKey.S_UWB_B_DEF] = findAssist.GetObjectFromName("n_bot_a");
				dictRefObj[RefObjKey.S_UWB_B_NUGE] = findAssist.GetObjectFromName("n_bot_b");
				dictRefObj[RefObjKey.S_UWB_B_NUGE2] = findAssist.GetObjectFromName("n_bot_c");
				break;
			case 9uL:
				dictRefObj[RefObjKey.S_PANST_DEF] = findAssist.GetObjectFromName("n_panst_a");
				dictRefObj[RefObjKey.S_PANST_NUGE] = findAssist.GetObjectFromName("n_panst_b");
				break;
			case 8uL:
			case 10uL:
			case 11uL:
				break;
			}
		}
	}

	public void ReleaseRefObject(ulong flags)
	{
		if ((long)flags >= 1L && (long)flags <= 15L)
		{
			switch (flags - 1)
			{
			case 1uL:
				dictRefObj.Remove(RefObjKey.a_n_hair_pony);
				dictRefObj.Remove(RefObjKey.a_n_hair_twin_L);
				dictRefObj.Remove(RefObjKey.a_n_hair_twin_R);
				dictRefObj.Remove(RefObjKey.a_n_hair_pin);
				dictRefObj.Remove(RefObjKey.a_n_hair_pin_R);
				dictRefObj.Remove(RefObjKey.a_n_headtop);
				dictRefObj.Remove(RefObjKey.a_n_headside);
				dictRefObj.Remove(RefObjKey.a_n_head);
				dictRefObj.Remove(RefObjKey.a_n_headflont);
				dictRefObj.Remove(RefObjKey.a_n_megane);
				dictRefObj.Remove(RefObjKey.a_n_earrings_L);
				dictRefObj.Remove(RefObjKey.a_n_earrings_R);
				dictRefObj.Remove(RefObjKey.a_n_nose);
				dictRefObj.Remove(RefObjKey.a_n_mouth);
				dictRefObj.Remove(RefObjKey.HairParent);
				dictRefObj.Remove(RefObjKey.F_ADJUSTWIDTHSCALE);
				break;
			case 3uL:
				dictRefObj.Remove(RefObjKey.ObjFace);
				dictRefObj.Remove(RefObjKey.ObjEyeline);
				dictRefObj.Remove(RefObjKey.ObjEyelineLow);
				dictRefObj.Remove(RefObjKey.ObjEyebrow);
				dictRefObj.Remove(RefObjKey.ObjNoseline);
				dictRefObj.Remove(RefObjKey.ObjEyeL);
				dictRefObj.Remove(RefObjKey.ObjEyeR);
				dictRefObj.Remove(RefObjKey.ObjEyeWL);
				dictRefObj.Remove(RefObjKey.ObjEyeWR);
				dictRefObj.Remove(RefObjKey.ObjDoubleTooth);
				dictRefObj.Remove(RefObjKey.S_TongueF);
				dictRefObj.Remove(RefObjKey.S_TEARS_01);
				dictRefObj.Remove(RefObjKey.S_TEARS_02);
				dictRefObj.Remove(RefObjKey.S_TEARS_03);
				dictRefObj.Remove(RefObjKey.N_EyeBase);
				dictRefObj.Remove(RefObjKey.N_Hitomi);
				dictRefObj.Remove(RefObjKey.N_Gag00);
				dictRefObj.Remove(RefObjKey.N_Gag01);
				dictRefObj.Remove(RefObjKey.N_Gag02);
				dictRefObj.Remove(RefObjKey.N_FaceSpecial);
				break;
			case 0uL:
				dictRefObj.Remove(RefObjKey.a_n_neck);
				dictRefObj.Remove(RefObjKey.a_n_bust_f);
				dictRefObj.Remove(RefObjKey.a_n_bust);
				dictRefObj.Remove(RefObjKey.a_n_nip_L);
				dictRefObj.Remove(RefObjKey.a_n_nip_R);
				dictRefObj.Remove(RefObjKey.a_n_back);
				dictRefObj.Remove(RefObjKey.a_n_back_L);
				dictRefObj.Remove(RefObjKey.a_n_back_R);
				dictRefObj.Remove(RefObjKey.a_n_waist);
				dictRefObj.Remove(RefObjKey.a_n_waist_f);
				dictRefObj.Remove(RefObjKey.a_n_waist_b);
				dictRefObj.Remove(RefObjKey.a_n_waist_L);
				dictRefObj.Remove(RefObjKey.a_n_waist_R);
				dictRefObj.Remove(RefObjKey.a_n_leg_L);
				dictRefObj.Remove(RefObjKey.a_n_leg_R);
				dictRefObj.Remove(RefObjKey.a_n_knee_L);
				dictRefObj.Remove(RefObjKey.a_n_knee_R);
				dictRefObj.Remove(RefObjKey.a_n_ankle_L);
				dictRefObj.Remove(RefObjKey.a_n_ankle_R);
				dictRefObj.Remove(RefObjKey.a_n_heel_L);
				dictRefObj.Remove(RefObjKey.a_n_heel_R);
				dictRefObj.Remove(RefObjKey.a_n_shoulder_L);
				dictRefObj.Remove(RefObjKey.a_n_shoulder_R);
				dictRefObj.Remove(RefObjKey.a_n_elbo_L);
				dictRefObj.Remove(RefObjKey.a_n_elbo_R);
				dictRefObj.Remove(RefObjKey.a_n_arm_L);
				dictRefObj.Remove(RefObjKey.a_n_arm_R);
				dictRefObj.Remove(RefObjKey.a_n_wrist_L);
				dictRefObj.Remove(RefObjKey.a_n_wrist_R);
				dictRefObj.Remove(RefObjKey.a_n_hand_L);
				dictRefObj.Remove(RefObjKey.a_n_hand_R);
				dictRefObj.Remove(RefObjKey.a_n_ind_L);
				dictRefObj.Remove(RefObjKey.a_n_ind_R);
				dictRefObj.Remove(RefObjKey.a_n_mid_L);
				dictRefObj.Remove(RefObjKey.a_n_mid_R);
				dictRefObj.Remove(RefObjKey.a_n_ring_L);
				dictRefObj.Remove(RefObjKey.a_n_ring_R);
				dictRefObj.Remove(RefObjKey.a_n_dan);
				dictRefObj.Remove(RefObjKey.a_n_kokan);
				dictRefObj.Remove(RefObjKey.a_n_ana);
				dictRefObj.Remove(RefObjKey.k_f_handL_00);
				dictRefObj.Remove(RefObjKey.k_f_handR_00);
				dictRefObj.Remove(RefObjKey.k_f_shoulderL_00);
				dictRefObj.Remove(RefObjKey.k_f_shoulderR_00);
				dictRefObj.Remove(RefObjKey.A_ROOTBONE);
				dictRefObj.Remove(RefObjKey.HeadParent);
				dictRefObj.Remove(RefObjKey.BUSTUP_TARGET);
				dictRefObj.Remove(RefObjKey.NECK_LOOK_TARGET);
				dictRefObj.Remove(RefObjKey.CORRECT_ARM_L);
				dictRefObj.Remove(RefObjKey.CORRECT_ARM_R);
				dictRefObj.Remove(RefObjKey.CORRECT_HAND_L);
				dictRefObj.Remove(RefObjKey.CORRECT_HAND_R);
				dictRefObj.Remove(RefObjKey.S_ANA);
				break;
			case 2uL:
				dictRefObj.Remove(RefObjKey.ObjBody);
				dictRefObj.Remove(RefObjKey.ObjNip);
				dictRefObj.Remove(RefObjKey.S_TongueB);
				dictRefObj.Remove(RefObjKey.S_Son);
				dictRefObj.Remove(RefObjKey.S_MNPA);
				dictRefObj.Remove(RefObjKey.S_MNPB);
				dictRefObj.Remove(RefObjKey.S_MOZ_ALL);
				dictRefObj.Remove(RefObjKey.S_GOMU);
				dictRefObj.Remove(RefObjKey.S_SimpleTop);
				dictRefObj.Remove(RefObjKey.S_SimpleBody);
				dictRefObj.Remove(RefObjKey.S_SimpleTongue);
				break;
			case 4uL:
				dictRefObj.Remove(RefObjKey.S_CTOP_T_DEF);
				dictRefObj.Remove(RefObjKey.S_CTOP_T_NUGE);
				dictRefObj.Remove(RefObjKey.S_CTOP_B_DEF);
				dictRefObj.Remove(RefObjKey.S_CTOP_B_NUGE);
				dictRefObj.Remove(RefObjKey.DB_SKIRT_TOP);
				break;
			case 12uL:
				dictRefObj.Remove(RefObjKey.S_TPARTS_00_DEF);
				dictRefObj.Remove(RefObjKey.S_TPARTS_00_NUGE);
				dictRefObj.Remove(RefObjKey.ObjInnerDef);
				dictRefObj.Remove(RefObjKey.ObjInnerNuge);
				dictRefObj.Remove(RefObjKey.DB_SKIRT_TOPA);
				break;
			case 13uL:
				dictRefObj.Remove(RefObjKey.S_TPARTS_01_DEF);
				dictRefObj.Remove(RefObjKey.S_TPARTS_01_NUGE);
				dictRefObj.Remove(RefObjKey.DB_SKIRT_TOPB);
				break;
			case 14uL:
				dictRefObj.Remove(RefObjKey.S_TPARTS_02_DEF);
				dictRefObj.Remove(RefObjKey.S_TPARTS_02_NUGE);
				break;
			case 5uL:
				dictRefObj.Remove(RefObjKey.S_CBOT_T_DEF);
				dictRefObj.Remove(RefObjKey.S_CBOT_T_NUGE);
				dictRefObj.Remove(RefObjKey.S_CBOT_B_DEF);
				dictRefObj.Remove(RefObjKey.S_CBOT_B_NUGE);
				dictRefObj.Remove(RefObjKey.DB_SKIRT_BOT);
				break;
			case 6uL:
				dictRefObj.Remove(RefObjKey.S_UWT_T_DEF);
				dictRefObj.Remove(RefObjKey.S_UWT_T_NUGE);
				dictRefObj.Remove(RefObjKey.S_UWT_B_DEF);
				dictRefObj.Remove(RefObjKey.S_UWT_B_NUGE);
				dictRefObj.Remove(RefObjKey.ObjBraDef);
				dictRefObj.Remove(RefObjKey.ObjBraNuge);
				break;
			case 7uL:
				dictRefObj.Remove(RefObjKey.S_UWB_T_DEF);
				dictRefObj.Remove(RefObjKey.S_UWB_T_NUGE);
				dictRefObj.Remove(RefObjKey.S_UWB_B_DEF);
				dictRefObj.Remove(RefObjKey.S_UWB_B_NUGE);
				dictRefObj.Remove(RefObjKey.S_UWB_B_NUGE2);
				break;
			case 9uL:
				dictRefObj.Remove(RefObjKey.S_PANST_DEF);
				dictRefObj.Remove(RefObjKey.S_PANST_NUGE);
				break;
			case 8uL:
			case 10uL:
			case 11uL:
				break;
			}
		}
	}

	public void ReleaseRefAll()
	{
		dictRefObj.Clear();
	}

	public GameObject GetReferenceInfo(RefObjKey key)
	{
		GameObject value = null;
		dictRefObj.TryGetValue(key, out value);
		return value;
	}
}
