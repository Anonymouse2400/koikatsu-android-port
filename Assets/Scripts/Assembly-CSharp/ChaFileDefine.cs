using System;
using Localize.Translate;
using UnityEngine;

public static class ChaFileDefine
{
	public enum BodyShapeIdx
	{
		Height = 0,
		HeadSize = 1,
		NeckW = 2,
		NeckZ = 3,
		BustSize = 4,
		BustY = 5,
		BustRotX = 6,
		BustX = 7,
		BustRotY = 8,
		BustSharp = 9,
		BustForm = 10,
		AreolaBulge = 11,
		NipWeight = 12,
		NipStand = 13,
		BodyShoulderW = 14,
		BodyShoulderZ = 15,
		BodyUpW = 16,
		BodyUpZ = 17,
		BodyLowW = 18,
		BodyLowZ = 19,
		WaistY = 20,
		Belly = 21,
		WaistUpW = 22,
		WaistUpZ = 23,
		WaistLowW = 24,
		WaistLowZ = 25,
		Hip = 26,
		HipRotX = 27,
		ThighUpW = 28,
		ThighUpZ = 29,
		ThighLowW = 30,
		ThighLowZ = 31,
		KneeLowW = 32,
		KneeLowZ = 33,
		Calf = 34,
		AnkleW = 35,
		AnkleZ = 36,
		ShoulderW = 37,
		ShoulderZ = 38,
		ArmUpW = 39,
		ArmUpZ = 40,
		ElbowW = 41,
		ElbowZ = 42,
		ArmLow = 43
	}

	public enum FaceShapeIdx
	{
		FaceBaseW = 0,
		FaceUpZ = 1,
		FaceUpY = 2,
		FaceUpSize = 3,
		FaceLowZ = 4,
		FaceLowW = 5,
		ChinLowY = 6,
		ChinLowZ = 7,
		ChinY = 8,
		ChinW = 9,
		ChinZ = 10,
		ChinTipY = 11,
		ChinTipZ = 12,
		ChinTipW = 13,
		CheekBoneW = 14,
		CheekBoneZ = 15,
		CheekW = 16,
		CheekZ = 17,
		CheekY = 18,
		EyebrowY = 19,
		EyebrowX = 20,
		EyebrowRotZ = 21,
		EyebrowInForm = 22,
		EyebrowOutForm = 23,
		EyelidsUpForm1 = 24,
		EyelidsUpForm2 = 25,
		EyelidsUpForm3 = 26,
		EyelidsLowForm1 = 27,
		EyelidsLowForm2 = 28,
		EyelidsLowForm3 = 29,
		EyeY = 30,
		EyeX = 31,
		EyeZ = 32,
		EyeTilt = 33,
		EyeH = 34,
		EyeW = 35,
		EyeInX = 36,
		EyeOutY = 37,
		NoseTipH = 38,
		NoseY = 39,
		NoseBridgeH = 40,
		MouthY = 41,
		MouthW = 42,
		MouthZ = 43,
		MouthUpForm = 44,
		MouthLowForm = 45,
		MouthCornerForm = 46,
		EarSize = 47,
		EarRotY = 48,
		EarRotZ = 49,
		EarUpForm = 50,
		EarLowForm = 51
	}

	public enum HairKind
	{
		back = 0,
		front = 1,
		side = 2,
		option = 3
	}

	public enum ClothesKind
	{
		top = 0,
		bot = 1,
		bra = 2,
		shorts = 3,
		gloves = 4,
		panst = 5,
		socks = 6,
		shoes_inner = 7,
		shoes_outer = 8
	}

	public enum ClothesSubKind
	{
		partsA = 0,
		partsB = 1,
		partsC = 2
	}

	public enum CoordinateType
	{
		School01 = 0,
		School02 = 1,
		Gym = 2,
		Swim = 3,
		Club = 4,
		Plain = 5,
		Pajamas = 6
	}

	public enum SiruParts
	{
		SiruKao = 0,
		SiruFrontUp = 1,
		SiruFrontDown = 2,
		SiruBackUp = 3,
		SiruBackDown = 4
	}

	public static readonly Version ChaFileVersion = new Version("0.0.0");

	public static readonly Version ChaFileCustomVersion = new Version("0.0.0");

	public static readonly Version ChaFileFaceVersion = new Version("0.0.2");

	public static readonly Version ChaFileBodyVersion = new Version("0.0.2");

	public static readonly Version ChaFileHairVersion = new Version("0.0.4");

	public static readonly Version ChaFileCoordinateVersion = new Version("0.0.0");

	public static readonly Version ChaFileClothesVersion = new Version("0.0.1");

	public static readonly Version ChaFileAccessoryVersion = new Version("0.0.2");

	public static readonly Version ChaFileMakeupVersion = new Version("0.0.0");

	public static readonly Version ChaFileParameterVersion = new Version("0.0.5");

	public static readonly Version ChaFileStatusVersion = new Version("0.0.0");

	public const int AccessoryCategoryTypeNone = 120;

	public const int AccessoryColorNum = 4;

	public const int AccessoryCorrectNum = 2;

	public const int AccessorySlotNum = 20;

	public const int CustomPaintNum = 2;

	public const float VoicePitchMin = 0.94f;

	public const float VoicePitchMax = 1.06f;

	public const int ProductNo = 100;

	public const string CharaFileMark = "【KoiKatuCharaS】";

	private const string CharaFileMarkSteam = "【KoiKatuCharaS】";

	public const string ClothesFileMark = "【KoiKatuClothes】";

	public const string CharaFileFemaleDir = "chara/female/";

	public const string CharaFileMaleDir = "chara/male/";

	public const string CoordinateFileDir = "coordinate/";

	public const int LoadError_Tag = -1;

	public const int LoadError_Version = -2;

	public const int LoadError_ProductNo = -3;

	public const int LoadError_EndOfStream = -4;

	public const int LoadError_OnlyPNG = -5;

	public const int LoadError_FileNotExist = -6;

	public const int LoadError_ETC = -999;

	public const int DefHeadID = 0;

	public const int DefHairBackID = 0;

	public const int DefHairFrontID = 1;

	public const int DefHairSideID = 0;

	public const int DefHairOptionID = 0;

	public const int DefClothesTopID = 0;

	public const int DefClothesBotID = 0;

	public const int DefClothesBraID = 0;

	public const int DefClothesShortsID = 0;

	public const int DefClothesGlovesID = 0;

	public const int DefClothesPanstID = 0;

	public const int DefClothesSocksID = 0;

	public const int DefClothesShoes_InnerID = 0;

	public const int DefClothesShoes_OuterID = 0;

	public const int DefClothesSailorPA_ID = 0;

	public const int DefClothesSailorPB_ID = 0;

	public const int DefClothesSailorPC_ID = 1;

	public const int DefClothesJacketPA_ID = 0;

	public const int DefClothesJacketPB_ID = 1;

	public const int DefClothesJacketPC_ID = 1;

	public static readonly string[] cf_bodyshapename = new string[44]
	{
		"身長", "頭サイズ", "首周り幅", "首周り奥", "胸サイズ", "胸上下位置", "胸の左右開き", "胸の左右位置", "胸上下角度", "胸の尖り",
		"胸形状", "乳輪の膨らみ", "乳首太さ", "乳首立ち", "胴体肩周り幅", "胴体肩周り奥", "胴体上幅", "胴体上奥", "胴体下幅", "胴体下奥",
		"ウエスト位置", "腹部", "腰上幅", "腰上奥", "腰下幅", "腰下奥", "尻", "尻角度", "太もも上幅", "太もも上奥",
		"太もも下幅", "太もも下奥", "膝下幅", "膝下奥", "ふくらはぎ", "足首幅", "足首奥", "肩幅", "肩奥", "上腕幅",
		"上腕奥", "肘周り幅", "肘周り奥", "前腕"
	};

	public static readonly int[] cf_BustShapeMaskID = new int[9] { 5, 6, 7, 8, 9, 10, 11, 12, 13 };

	public static readonly int[] cf_ShapeMaskBust = new int[6] { 0, 1, 2, 3, 4, 5 };

	public static readonly int[] cf_ShapeMaskNip = new int[2] { 6, 7 };

	public static readonly int cf_ShapeMaskNipStand = 8;

	public static readonly int[] cf_CategoryWaist = new int[6] { 18, 19, 22, 23, 24, 25 };

	public static readonly string[] cf_headshapename = new string[52]
	{
		"顔全体横幅", "顔上部前後", "顔上部上下", "顔上部サイズ", "顔下部前後", "顔下部横幅", "顎下部上下", "顎下部奥行", "顎上下", "顎幅",
		"顎前後", "顎先上下", "顎先前後", "顎先幅", "頬骨幅", "頬骨前後", "頬幅", "頬前後", "頬上下", "眉毛上下",
		"眉毛横位置", "眉毛角度Z軸", "眉毛内側形状", "眉毛外側形状", "上まぶた形状１", "上まぶた形状２", "上まぶた形状３", "下まぶた形状１", "下まぶた形状２", "下まぶた形状３",
		"目上下", "目横位置", "目前後", "目の角度", "目の縦幅", "目の横幅", "目頭左右位置", "目尻上下位置", "鼻先高さ", "鼻上下",
		"鼻筋高さ", "口上下", "口横幅", "口前後", "口形状上", "口形状下", "口形状口角", "耳サイズ", "耳角度Y軸", "耳角度Z軸",
		"耳上部形状", "耳下部形状"
	};

	public static readonly int[] cf_MouthShapeMaskID = new int[7] { 4, 41, 42, 43, 44, 45, 46 };

	public static readonly float[] cf_MouthShapeDefault = new float[7] { 0.5f, 0.5f, 0f, 0.5f, 0f, 0f, 0.5f };

	public static readonly float[] cf_faceInitValue = new float[52]
	{
		0.5f, 0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0f, 0.5f, 0.5f,
		0.5f, 0.5f, 0.5f, 0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
		0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
		0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
		0.5f, 0f, 0f, 0.5f, 0f, 0f, 0.5f, 0.5f, 0.5f, 0.5f,
		0f, 0f
	};

	public const float cf_ShapeLowMin = 0.2f;

	public const float cf_ShapeLowMax = 0.8f;

	public static readonly Color[] defClothesSubColor = new Color[4]
	{
		new Color(0.882f, 0.882f, 0.882f),
		new Color(0.376f, 0.431f, 0.529f),
		new Color(0.882f, 0.882f, 0.882f),
		new Color(0.784f, 0.352f, 0.352f)
	};

	public static string GetBloodTypeStr(int bloodType)
	{
		string[] array = Localize.Translate.Manager.OtherData.Get(3).Values.ToArray("BloodType");
		if (array.Length == 0)
		{
			array = new string[4] { "Ａ", "Ｂ", "Ｏ", "ＡＢ" };
		}
		string text = array.SafeGet(bloodType);
		string text2 = Localize.Translate.Manager.OtherData.Get(3).SafeGetText(2);
		if (text2 != null)
		{
			return (text == null) ? Localize.Translate.Manager.UnknownText : string.Format(text2, text);
		}
		return (text == null) ? Localize.Translate.Manager.UnknownText : (text + "型");
	}
}
