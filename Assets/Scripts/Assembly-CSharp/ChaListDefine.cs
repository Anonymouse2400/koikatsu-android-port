using System;

public static class ChaListDefine
{
	public enum CategoryNo
	{
		cha_sample_f = 0,
		cha_sample_m = 1,
		cha_eyeset = 2,
		bodypaint_layout = 3,
		cha_default_cos_f = 4,
		cha_default_cos_m = 5,
		cha_sample_voice = 6,
		facepaint_layout = 7,
		mole_layout = 8,
		bo_head = 100,
		bo_hair_b = 101,
		bo_hair_f = 102,
		bo_hair_s = 103,
		bo_hair_o = 104,
		co_top = 105,
		co_bot = 106,
		co_bra = 107,
		co_shorts = 108,
		co_gloves = 109,
		co_panst = 110,
		co_socks = 111,
		co_shoes = 112,
		ao_none = 120,
		ao_hair = 121,
		ao_head = 122,
		ao_face = 123,
		ao_neck = 124,
		ao_body = 125,
		ao_waist = 126,
		ao_leg = 127,
		ao_arm = 128,
		ao_hand = 129,
		ao_kokan = 130,
		cpo_sailor_a = 200,
		cpo_sailor_b = 201,
		cpo_sailor_c = 202,
		cpo_jacket_a = 210,
		cpo_jacket_b = 211,
		cpo_jacket_c = 212,
		mt_face_detail = 400,
		mt_eyeshadow = 401,
		mt_cheek = 402,
		mt_lip = 403,
		mt_lipline = 404,
		mt_face_paint = 405,
		mt_eyebrow = 406,
		mt_eye_white = 407,
		mt_eye = 408,
		mt_eye_gradation = 409,
		mt_eye_hi_up = 410,
		mt_eye_hi_down = 411,
		mt_eyeline_up = 412,
		mt_eyeline_down = 413,
		mt_nose = 414,
		mt_mole = 415,
		mt_body_detail = 420,
		mt_body_paint = 421,
		mt_sunburn = 422,
		mt_nip = 423,
		mt_underhair = 424,
		mt_pattern = 430,
		mt_emblem = 431,
		mt_ramp = 432,
		mt_hairgloss = 433
	}

	public enum KeyType
	{
		Category = 0,
		DistributionNo = 1,
		AngleSpeed = 2,
		CheekTex = 3,
		ColorMask02AB = 4,
		ColorMask02Tex = 5,
		ColorMaskAB = 6,
		ColorMaskTex = 7,
		Coordinate = 8,
		EpsTex = 9,
		EpsTexAB = 10,
		EyeBase = 11,
		EyebrowTex = 12,
		EyeHiDownTex = 13,
		EyeHitomi = 14,
		EyeHiUpTex = 15,
		EyelineDownTex = 16,
		EyelineShadowTex = 17,
		EyelineUpTex = 18,
		EyeObjNo = 19,
		EyeshadowTex = 20,
		EyesPtn = 21,
		EyeTex = 22,
		EyeWhiteTex = 23,
		HideHair = 24,
		ID = 25,
		Kind = 26,
		LineMask = 27,
		LiplineTex = 28,
		LipTex = 29,
		MainAB = 30,
		MainData = 31,
		MainManifest = 32,
		MainTex = 33,
		MainTex02 = 34,
		MainTex02AB = 35,
		MainTexAB = 36,
		MatAB = 37,
		MatData = 38,
		MatManifest = 39,
		MoleTex = 40,
		Name = 41,
		NipTex = 42,
		NormalData = 43,
		NormallMapDetail = 44,
		NoseTex = 45,
		NotBra = 46,
		OverBodyMask = 47,
		OverBodyMaskAB = 48,
		OverBraMask = 49,
		OverBraMaskAB = 50,
		OverInnerMask = 51,
		OverInnerMaskAB = 52,
		PaintTex = 53,
		Parent = 54,
		Possess = 55,
		SetHair = 56,
		Sex = 57,
		ShapeAnime = 58,
		SizeSpeed = 59,
		SizeWidth = 60,
		StateType = 61,
		SunburnTex = 62,
		TileAnim = 63,
		UnderhairTex = 64,
		Yurayura = 65,
		KokanHide = 66,
		ThumbAB = 67,
		ThumbTex = 68,
		PosX = 69,
		PosY = 70,
		Rot = 71,
		Scale = 72,
		CenterX = 73,
		MoveX = 74,
		CenterY = 75,
		MoveY = 76,
		CenterScale = 77,
		AddScale = 78,
		DefCoorde01 = 79,
		DefCoorde02 = 80,
		DefCoorde03 = 81,
		DefCoorde04 = 82,
		DefCoorde05 = 83,
		DefCoorde06 = 84,
		DefCoorde07 = 85,
		Data01 = 86,
		Data02 = 87,
		Data03 = 88,
		Eyebrow01 = 89,
		Eyebrow02 = 90,
		Eyebrow03 = 91,
		Eye01 = 92,
		Eye02 = 93,
		Eye03 = 94,
		Mouth01 = 95,
		Mouth02 = 96,
		Mouth03 = 97,
		EyeHiLight01 = 98,
		EyeHiLight02 = 99,
		EyeHiLight03 = 100
	}

	public static readonly Version CheckItemVersion = new Version("0.0.0");

	public static string GetCategoryName(int no)
	{
		switch (no)
		{
		case 0:
			return "キャラサンプル女";
		case 1:
			return "キャラサンプル男";
		case 2:
			return "目のパターン設定";
		case 3:
			return "ボディーペイントの配置設定";
		case 4:
			return "女キャラ服装デフォルト";
		case 5:
			return "男キャラ服装デフォルト";
		case 6:
			return "サンプル音声";
		case 7:
			return "フェイスペイントの配置設定";
		case 8:
			return "ホクロの配置設定";
		case 100:
			return "頭";
		case 101:
			return "後ろ髪";
		case 102:
			return "前髪";
		case 103:
			return "横髪";
		case 104:
			return "付け毛";
		case 105:
			return "服上";
		case 106:
			return "服下";
		case 107:
			return "ブラ・水着(上)";
		case 108:
			return "ショーツ・水着(下)";
		case 109:
			return "手袋";
		case 110:
			return "パンスト";
		case 111:
			return "靴下";
		case 112:
			return "靴";
		case 120:
			return "アクセサリ(なし)";
		case 121:
			return "アクセサリ髪";
		case 122:
			return "アクセサリ頭";
		case 123:
			return "アクセサリ顔";
		case 124:
			return "アクセサリ首";
		case 125:
			return "アクセサリ胴";
		case 126:
			return "アクセサリ腰";
		case 127:
			return "アクセサリ脚";
		case 128:
			return "アクセサリ腕";
		case 129:
			return "アクセサリ手";
		case 130:
			return "アクセサリ股間周り";
		case 200:
			return "セーラーパーツ（胴）";
		case 201:
			return "セーラーパーツ（襟）";
		case 202:
			return "セーラーパーツ（装飾）";
		case 210:
			return "ジャケットパーツ（インナー）";
		case 211:
			return "ジャケットパーツ（アウター）";
		case 212:
			return "ジャケットパーツ（装飾）";
		case 400:
			return "顔のシワ";
		case 401:
			return "アイシャドウテクスチャ";
		case 402:
			return "チークテクスチャ";
		case 403:
			return "唇テクスチャ";
		case 404:
			return "唇ラインテクスチャ";
		case 405:
			return "顔ペイントテクスチャ";
		case 406:
			return "眉毛テクスチャ";
		case 407:
			return "瞳白目作成テクスチャ";
		case 408:
			return "瞳テクスチャ";
		case 409:
			return "瞳グラデーションマスクテクスチャ";
		case 410:
			return "瞳ハイライト上テクスチャ";
		case 411:
			return "瞳ハイライト下テクスチャ";
		case 412:
			return "アイライン上テクスチャ";
		case 413:
			return "アイライン下テクスチャ";
		case 414:
			return "鼻テクスチャ";
		case 415:
			return "ホクロテクスチャ";
		case 420:
			return "体の肉感";
		case 421:
			return "体ペイントテクスチャ";
		case 422:
			return "日焼けテクスチャ";
		case 423:
			return "乳首テクスチャ";
		case 424:
			return "ヘアテクスチャ";
		case 430:
			return "パターンテクスチャ";
		case 431:
			return "エンブレムテクスチャ";
		case 432:
			return "ランプテクスチャ";
		case 433:
			return "髪の毛ツヤテクスチャ";
		default:
			return string.Empty;
		}
	}
}
