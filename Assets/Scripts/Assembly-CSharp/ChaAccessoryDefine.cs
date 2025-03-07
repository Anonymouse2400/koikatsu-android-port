using System;
using System.Collections.Generic;

public static class ChaAccessoryDefine
{
	public enum AccessoryType
	{
		None = 0,
		Hair = 1,
		Head = 2,
		Face = 3,
		Neck = 4,
		Body = 5,
		Waist = 6,
		Leg = 7,
		Arm = 8,
		Hand = 9,
		Kokan = 10
	}

	public enum AccessoryParentKey
	{
		none = 0,
		a_n_hair_pony = 1,
		a_n_hair_twin_L = 2,
		a_n_hair_twin_R = 3,
		a_n_hair_pin = 4,
		a_n_hair_pin_R = 5,
		a_n_headtop = 6,
		a_n_headflont = 7,
		a_n_head = 8,
		a_n_headside = 9,
		a_n_earrings_L = 10,
		a_n_earrings_R = 11,
		a_n_megane = 12,
		a_n_nose = 13,
		a_n_mouth = 14,
		a_n_neck = 15,
		a_n_bust_f = 16,
		a_n_bust = 17,
		a_n_nip_L = 18,
		a_n_nip_R = 19,
		a_n_back = 20,
		a_n_back_L = 21,
		a_n_back_R = 22,
		a_n_waist = 23,
		a_n_waist_f = 24,
		a_n_waist_b = 25,
		a_n_waist_L = 26,
		a_n_waist_R = 27,
		a_n_leg_L = 28,
		a_n_knee_L = 29,
		a_n_ankle_L = 30,
		a_n_heel_L = 31,
		a_n_leg_R = 32,
		a_n_knee_R = 33,
		a_n_ankle_R = 34,
		a_n_heel_R = 35,
		a_n_shoulder_L = 36,
		a_n_elbo_L = 37,
		a_n_arm_L = 38,
		a_n_wrist_L = 39,
		a_n_shoulder_R = 40,
		a_n_elbo_R = 41,
		a_n_arm_R = 42,
		a_n_wrist_R = 43,
		a_n_hand_L = 44,
		a_n_ind_L = 45,
		a_n_mid_L = 46,
		a_n_ring_L = 47,
		a_n_hand_R = 48,
		a_n_ind_R = 49,
		a_n_mid_R = 50,
		a_n_ring_R = 51,
		a_n_dan = 52,
		a_n_kokan = 53,
		a_n_ana = 54
	}

	public static readonly string[] AccessoryTypeName;

	public static readonly string[] AccessoryParentName;

	public static Dictionary<int, string> dictAccessoryType { get; private set; }

	public static Dictionary<int, string> dictAccessoryParent { get; private set; }

	static ChaAccessoryDefine()
	{
		AccessoryTypeName = new string[11]
		{
			"なし", "髪", "頭", "顔", "首", "胴", "腰", "脚", "腕", "手",
			"股間周り"
		};
		AccessoryParentName = new string[55]
		{
			"未設定", "ポニー", "ツイン左", "ツイン右", "ヘアピン左", "ヘアピン右", "帽子", "額", "頭上", "頭中心",
			"イヤリング左", "イヤリング右", "眼鏡", "鼻", "口", "首", "胸上", "胸上中央", "左乳首", "右乳首",
			"背中中央", "背中左", "背中右", "腰", "腰前", "腰後ろ", "腰左", "腰右", "左太もも", "左ひざ",
			"左足首", "かかと左", "右太もも", "右ひざ", "右足首", "かかと右", "左肩", "左肘", "左上腕", "左手首",
			"右肩", "右肘", "右上腕", "右手首", "左手", "左人差指", "左中指", "左薬指", "右手", "右人差指",
			"右中指", "右薬指", "男根根本", "女性器", "尻穴"
		};
		dictAccessoryType = new Dictionary<int, string>();
		int length = Enum.GetValues(typeof(AccessoryType)).Length;
		int num = AccessoryTypeName.Length;
		if (length == num)
		{
			for (int i = 0; i < length; i++)
			{
				dictAccessoryType[i] = AccessoryTypeName[i];
			}
		}
		dictAccessoryParent = new Dictionary<int, string>();
		length = Enum.GetValues(typeof(AccessoryParentKey)).Length;
		num = AccessoryParentName.Length;
		if (length == num)
		{
			for (int j = 0; j < length; j++)
			{
				dictAccessoryParent[j] = AccessoryParentName[j];
			}
		}
	}

	public static string GetAccessoryTypeName(ChaListDefine.CategoryNo cate)
	{
		switch (cate)
		{
		case ChaListDefine.CategoryNo.ao_none:
			return AccessoryTypeName[0];
		case ChaListDefine.CategoryNo.ao_hair:
			return AccessoryTypeName[1];
		case ChaListDefine.CategoryNo.ao_head:
			return AccessoryTypeName[2];
		case ChaListDefine.CategoryNo.ao_face:
			return AccessoryTypeName[3];
		case ChaListDefine.CategoryNo.ao_neck:
			return AccessoryTypeName[4];
		case ChaListDefine.CategoryNo.ao_body:
			return AccessoryTypeName[5];
		case ChaListDefine.CategoryNo.ao_waist:
			return AccessoryTypeName[6];
		case ChaListDefine.CategoryNo.ao_leg:
			return AccessoryTypeName[7];
		case ChaListDefine.CategoryNo.ao_arm:
			return AccessoryTypeName[8];
		case ChaListDefine.CategoryNo.ao_hand:
			return AccessoryTypeName[9];
		case ChaListDefine.CategoryNo.ao_kokan:
			return AccessoryTypeName[10];
		default:
			return "不明";
		}
	}

	public static AccessoryParentKey GetReverseParent(AccessoryParentKey key)
	{
		switch (key)
		{
		case AccessoryParentKey.a_n_hair_twin_L:
			return AccessoryParentKey.a_n_hair_twin_R;
		case AccessoryParentKey.a_n_hair_pin:
			return AccessoryParentKey.a_n_hair_pin_R;
		case AccessoryParentKey.a_n_earrings_L:
			return AccessoryParentKey.a_n_earrings_R;
		case AccessoryParentKey.a_n_nip_L:
			return AccessoryParentKey.a_n_nip_R;
		case AccessoryParentKey.a_n_back_L:
			return AccessoryParentKey.a_n_back_R;
		case AccessoryParentKey.a_n_waist_L:
			return AccessoryParentKey.a_n_waist_R;
		case AccessoryParentKey.a_n_leg_L:
			return AccessoryParentKey.a_n_leg_R;
		case AccessoryParentKey.a_n_knee_L:
			return AccessoryParentKey.a_n_knee_R;
		case AccessoryParentKey.a_n_ankle_L:
			return AccessoryParentKey.a_n_ankle_R;
		case AccessoryParentKey.a_n_heel_L:
			return AccessoryParentKey.a_n_heel_R;
		case AccessoryParentKey.a_n_shoulder_L:
			return AccessoryParentKey.a_n_shoulder_R;
		case AccessoryParentKey.a_n_elbo_L:
			return AccessoryParentKey.a_n_elbo_R;
		case AccessoryParentKey.a_n_arm_L:
			return AccessoryParentKey.a_n_arm_R;
		case AccessoryParentKey.a_n_wrist_L:
			return AccessoryParentKey.a_n_wrist_R;
		case AccessoryParentKey.a_n_hand_L:
			return AccessoryParentKey.a_n_hand_R;
		case AccessoryParentKey.a_n_ind_L:
			return AccessoryParentKey.a_n_ind_R;
		case AccessoryParentKey.a_n_mid_L:
			return AccessoryParentKey.a_n_mid_R;
		case AccessoryParentKey.a_n_ring_L:
			return AccessoryParentKey.a_n_ring_R;
		case AccessoryParentKey.a_n_hair_twin_R:
			return AccessoryParentKey.a_n_hair_twin_L;
		case AccessoryParentKey.a_n_hair_pin_R:
			return AccessoryParentKey.a_n_hair_pin;
		case AccessoryParentKey.a_n_earrings_R:
			return AccessoryParentKey.a_n_earrings_L;
		case AccessoryParentKey.a_n_nip_R:
			return AccessoryParentKey.a_n_nip_L;
		case AccessoryParentKey.a_n_back_R:
			return AccessoryParentKey.a_n_back_L;
		case AccessoryParentKey.a_n_waist_R:
			return AccessoryParentKey.a_n_waist_L;
		case AccessoryParentKey.a_n_leg_R:
			return AccessoryParentKey.a_n_leg_L;
		case AccessoryParentKey.a_n_knee_R:
			return AccessoryParentKey.a_n_knee_L;
		case AccessoryParentKey.a_n_ankle_R:
			return AccessoryParentKey.a_n_ankle_L;
		case AccessoryParentKey.a_n_heel_R:
			return AccessoryParentKey.a_n_heel_L;
		case AccessoryParentKey.a_n_shoulder_R:
			return AccessoryParentKey.a_n_shoulder_L;
		case AccessoryParentKey.a_n_elbo_R:
			return AccessoryParentKey.a_n_elbo_L;
		case AccessoryParentKey.a_n_arm_R:
			return AccessoryParentKey.a_n_arm_L;
		case AccessoryParentKey.a_n_wrist_R:
			return AccessoryParentKey.a_n_wrist_L;
		case AccessoryParentKey.a_n_hand_R:
			return AccessoryParentKey.a_n_hand_L;
		case AccessoryParentKey.a_n_ind_R:
			return AccessoryParentKey.a_n_ind_L;
		case AccessoryParentKey.a_n_mid_R:
			return AccessoryParentKey.a_n_mid_L;
		case AccessoryParentKey.a_n_ring_R:
			return AccessoryParentKey.a_n_ring_L;
		default:
			return AccessoryParentKey.none;
		}
	}

	public static string GetReverseParent(string key)
	{
		switch (key)
		{
		case "a_n_hair_twin_L":
			return "a_n_hair_twin_R";
		case "a_n_hair_pin":
			return "a_n_hair_pin_R";
		case "a_n_earrings_L":
			return "a_n_earrings_R";
		case "a_n_nip_L":
			return "a_n_nip_R";
		case "a_n_back_L":
			return "a_n_back_R";
		case "a_n_waist_L":
			return "a_n_waist_R";
		case "a_n_leg_L":
			return "a_n_leg_R";
		case "a_n_knee_L":
			return "a_n_knee_R";
		case "a_n_ankle_L":
			return "a_n_ankle_R";
		case "a_n_heel_L":
			return "a_n_heel_R";
		case "a_n_shoulder_L":
			return "a_n_shoulder_R";
		case "a_n_elbo_L":
			return "a_n_elbo_R";
		case "a_n_arm_L":
			return "a_n_arm_R";
		case "a_n_wrist_L":
			return "a_n_wrist_R";
		case "a_n_hand_L":
			return "a_n_hand_R";
		case "a_n_ind_L":
			return "a_n_ind_R";
		case "a_n_mid_L":
			return "a_n_mid_R";
		case "a_n_ring_L":
			return "a_n_ring_R";
		case "a_n_hair_twin_R":
			return "a_n_hair_twin_L";
		case "a_n_hair_pin_R":
			return "a_n_hair_pin";
		case "a_n_earrings_R":
			return "a_n_earrings_L";
		case "a_n_nip_R":
			return "a_n_nip_L";
		case "a_n_back_R":
			return "a_n_back_L";
		case "a_n_waist_R":
			return "a_n_waist_L";
		case "a_n_leg_R":
			return "a_n_leg_L";
		case "a_n_knee_R":
			return "a_n_knee_L";
		case "a_n_ankle_R":
			return "a_n_ankle_L";
		case "a_n_heel_R":
			return "a_n_heel_L";
		case "a_n_shoulder_R":
			return "a_n_shoulder_L";
		case "a_n_elbo_R":
			return "a_n_elbo_L";
		case "a_n_arm_R":
			return "a_n_arm_L";
		case "a_n_wrist_R":
			return "a_n_wrist_L";
		case "a_n_hand_R":
			return "a_n_hand_L";
		case "a_n_ind_R":
			return "a_n_ind_L";
		case "a_n_mid_R":
			return "a_n_mid_L";
		case "a_n_ring_R":
			return "a_n_ring_L";
		default:
			return string.Empty;
		}
	}

	public static bool CheckPartsOfHead(string parentKey)
	{
		object obj = Enum.Parse(typeof(AccessoryParentKey), parentKey);
		if (MathfEx.RangeEqualOn(1, (int)obj, 14))
		{
			return true;
		}
		return false;
	}
}
