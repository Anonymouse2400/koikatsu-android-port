using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

public static class ChaRandom
{
	public static void RandomFace(ChaControl cha, bool shape, bool eyes, bool makeup, bool etc)
	{
		RandomFace(cha.chaFile, shape, eyes, makeup, etc);
	}

	public static void RandomFace(ChaFileControl file, bool shape, bool eyes, bool makeup, bool etc)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileFace face = file.custom.face;
		ChaFileBody body = file.custom.body;
		Dictionary<int, ListInfoBase> dictionary = null;
		if (shape)
		{
			float num = Random.Range(-0.2f, 0.2f);
			for (int i = 0; i < face.shapeValueFace.Length; i++)
			{
				face.shapeValueFace[i] = Mathf.Clamp(ChaFileDefine.cf_faceInitValue[i] + num, 0f, 1f);
			}
		}
		if (eyes)
		{
			bool flag = true;
			if (GetRandomIndex(5, 95) == 0)
			{
				flag = false;
			}
			for (int j = 0; j < 2; j++)
			{
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye);
				face.pupil[j].id = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
				face.pupil[j].baseColor = new Color(Random.value, Random.value, Random.value);
				face.pupil[j].subColor = face.pupil[j].baseColor;
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_gradation);
				face.pupil[j].gradMaskId = 0;
				face.pupil[j].gradBlend = 0f;
			}
			if (flag)
			{
				face.pupil[1].Copy(face.pupil[0]);
			}
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_hi_up);
			face.hlUpId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			face.hlUpColor = Color.white;
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_hi_down);
			face.hlDownId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			face.hlDownColor = Color.white;
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_white);
			face.whiteId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			face.whiteBaseColor = Color.white;
			face.whiteSubColor = Color.white;
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeline_up);
			face.eyelineUpId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeline_down);
			face.eyelineDownId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			float H;
			float S;
			float V;
			Color.RGBToHSV(face.pupil[0].baseColor, out H, out S, out V);
			V = Mathf.Clamp(V - 0.3f, 0f, 1f);
			face.eyelineColor = Color.HSVToRGB(H, S, V);
		}
		if (etc)
		{
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_head);
			face.headId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_face_detail);
			face.detailId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			face.detailPower = 0.5f;
			face.lipGlossPower = 0.3f;
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyebrow);
			face.eyebrowId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			face.eyebrowColor = file.custom.hair.parts[0].baseColor;
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_nose);
			face.noseId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_mole);
			face.moleId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			face.moleColor = Color.black;
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_lipline);
			face.lipLineId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			Color skinMainColor = body.skinMainColor;
			float H2;
			float S2;
			float V2;
			Color.RGBToHSV(skinMainColor, out H2, out S2, out V2);
			face.lipLineColor = Color.HSVToRGB(H2, S2, Mathf.Max(V2 - 0.3f, 0f));
			face.lipGlossPower = 0.3f;
			face.doubleTooth = GetRandomIndex(5, 95) == 0;
		}
		if (makeup)
		{
			RandomMakeup(face.baseMakeup);
		}
	}

	public static void RandomBody(ChaControl cha, bool shape, bool etc)
	{
		RandomBody(cha.chaFile, shape, etc);
	}

	public static void RandomBody(ChaFileControl file, bool shape, bool etc)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileBody body = file.custom.body;
		Dictionary<int, ListInfoBase> dictionary = null;
		if (shape)
		{
			float num = Random.Range(-0.3f, 0.3f);
			for (int i = 0; i < body.shapeValueBody.Length; i++)
			{
				body.shapeValueBody[i] = Mathf.Clamp(0.5f + num, 0f, 1f);
			}
			body.shapeValueBody[4] = Random.value;
			body.bustSoftness = Random.value;
			body.bustWeight = Random.value;
		}
		if (!etc)
		{
			return;
		}
		dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_body_detail);
		body.detailId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
		body.detailPower = 0.5f;
		float H = 0.06f;
		float S = Random.Range(0.13f, 0.39f);
		float V = Random.Range(0.66f, 0.98f);
		Color skinMainColor = Color.HSVToRGB(H, S, V);
		body.skinMainColor = skinMainColor;
		Color.RGBToHSV(body.skinMainColor, out H, out S, out V);
		S = Mathf.Min(1f, S + 0.1f);
		V = Mathf.Max(0f, V - 0.1f);
		skinMainColor = Color.HSVToRGB(H, S, V);
		skinMainColor.r = Mathf.Min(1f, skinMainColor.r + 0.1f);
		body.skinSubColor = skinMainColor;
		body.skinGlossPower = 0.3f;
		for (int j = 0; j < 2; j++)
		{
			if (GetRandomIndex(1, 99) == 0)
			{
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_body_paint);
				body.paintId[j] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
				body.paintColor[j] = new Color(Random.value, Random.value, Random.value);
			}
		}
		dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_sunburn);
		body.sunburnId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
		Color.RGBToHSV(body.skinMainColor, out H, out S, out V);
		S = Mathf.Max(0f, S - 0.1f);
		body.sunburnColor = Color.HSVToRGB(H, S, V);
		dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_nip);
		body.nipId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
		Color.RGBToHSV(body.skinMainColor, out H, out S, out V);
		S = Mathf.Min(1f, S + 0.1f);
		body.nipColor = Color.HSVToRGB(H, S, V);
		body.areolaSize = Random.value;
		dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_underhair);
		body.underhairId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
		body.underhairColor = file.custom.hair.parts[0].baseColor;
		body.nailColor = Color.white;
		body.nailGlossPower = 0.3f;
		body.drawAddLine = GetRandomIndex(50, 50) == 0;
	}

	public static void RandomHair(ChaControl cha, bool type, bool color, bool etc)
	{
		RandomHair(cha.chaFile, type, color, etc);
	}

	public static void RandomHair(ChaFileControl file, bool type, bool color, bool etc)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileHair hair = file.custom.hair;
		Dictionary<int, ListInfoBase> dictionary = null;
		int[] array = new int[4] { 0, 1, 2, 3 };
		if (type)
		{
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_b);
			hair.parts[array[0]].id = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_f);
			hair.parts[array[1]].id = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			if (GetRandomIndex(20, 80) == 0)
			{
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_s);
				hair.parts[array[2]].id = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
			}
			else
			{
				hair.parts[array[2]].id = 0;
			}
		}
		if (color)
		{
			Color color2 = new Color(Random.value, Random.value, Random.value);
			float H;
			float S;
			float V;
			Color.RGBToHSV(color2, out H, out S, out V);
			Color startColor = Color.HSVToRGB(H, S, Mathf.Max(V - 0.3f, 0f));
			Color endColor = Color.HSVToRGB(H, S, Mathf.Min(V + 0.15f, 1f));
			hair.parts[array[0]].baseColor = color2;
			hair.parts[array[0]].startColor = startColor;
			hair.parts[array[0]].endColor = endColor;
			hair.parts[array[1]].baseColor = color2;
			hair.parts[array[1]].startColor = startColor;
			hair.parts[array[1]].endColor = endColor;
			hair.parts[array[2]].baseColor = color2;
			hair.parts[array[2]].startColor = startColor;
			hair.parts[array[2]].endColor = endColor;
			file.custom.face.eyebrowColor = hair.parts[0].baseColor;
			file.custom.body.underhairColor = hair.parts[0].baseColor;
		}
		if (!etc)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				hair.parts[array[i]].acsColor[j] = new Color(Random.value, Random.value, Random.value);
			}
		}
		dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_hairgloss);
		hair.glossId = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
	}

	public static void RandomClothes(ChaControl cha, bool type, bool color, bool ptn)
	{
		RandomClothes(cha.nowCoordinate.clothes, type, color, ptn);
	}

	public static void RandomClothes(ChaFileClothes clothes, bool type, bool color, bool ptn)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		Dictionary<int, ListInfoBase> dictionary = null;
		List<int> list = null;
		ChaFileClothes.PartsInfo partsInfo = null;
		Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_emblem);
		Dictionary<int, ListInfoBase> categoryInfo2 = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_pattern);
		if (type)
		{
			if (GetRandomIndex(10, 90) == 0)
			{
				clothes.parts[0].id = 0;
				clothes.parts[1].id = 0;
				partsInfo = clothes.parts[2];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_bra);
				list = (from x in dictionary
					where 2 == x.Value.Kind
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				partsInfo = clothes.parts[3];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_shorts);
				list = (from x in dictionary
					where 2 == x.Value.Kind
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				clothes.parts[4].id = 0;
				clothes.parts[5].id = 0;
				clothes.parts[6].id = 0;
				clothes.parts[7].id = 0;
				clothes.parts[8].id = 0;
			}
			else
			{
				partsInfo = clothes.parts[0];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				switch (GetRandomIndex(40, 40, 20))
				{
				case 0:
					partsInfo.id = 1;
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cpo_sailor_a);
					clothes.subPartsId[0] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cpo_sailor_b);
					clothes.subPartsId[1] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cpo_sailor_c);
					clothes.subPartsId[2] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
					break;
				case 1:
					partsInfo.id = 2;
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cpo_jacket_a);
					clothes.subPartsId[0] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cpo_jacket_b);
					clothes.subPartsId[1] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cpo_jacket_c);
					clothes.subPartsId[2] = dictionary.Keys.ElementAt(Random.Range(0, dictionary.Keys.Count));
					break;
				default:
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
					list = (from x in dictionary
						where 2 < x.Key
						select x.Key).ToList();
					partsInfo.id = list[Random.Range(0, list.Count)];
					break;
				}
				partsInfo = clothes.parts[1];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_bot);
				list = (from x in dictionary
					where 0 != x.Key
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				partsInfo = clothes.parts[2];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_bra);
				list = (from x in dictionary
					where x.Key != 0 && 2 != x.Value.Kind
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				partsInfo = clothes.parts[3];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_shorts);
				list = (from x in dictionary
					where x.Key != 0 && 2 != x.Value.Kind
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				partsInfo = clothes.parts[4];
				if (GetRandomIndex(5, 95) == 1)
				{
					partsInfo.id = 0;
				}
				else
				{
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_gloves);
					list = (from x in dictionary
						where 0 != x.Key
						select x.Key).ToList();
					partsInfo.id = list[Random.Range(0, list.Count)];
					partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				}
				partsInfo = clothes.parts[5];
				if (GetRandomIndex(5, 95) == 1)
				{
					partsInfo.id = 0;
				}
				else
				{
					dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_panst);
					list = (from x in dictionary
						where 0 != x.Key
						select x.Key).ToList();
					partsInfo.id = list[Random.Range(0, list.Count)];
					partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				}
				partsInfo = clothes.parts[6];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_socks);
				list = (from x in dictionary
					where 0 != x.Key
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				partsInfo = clothes.parts[7];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_shoes);
				list = (from x in dictionary
					where 0 != x.Key
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				partsInfo = clothes.parts[8];
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_shoes);
				list = (from x in dictionary
					where 0 != x.Key
					select x.Key).ToList();
				partsInfo.id = list[Random.Range(0, list.Count)];
				partsInfo.emblemeId = categoryInfo.Keys.ElementAt(Random.Range(0, categoryInfo.Keys.Count));
				for (int i = 0; i < clothes.hideBraOpt.Length; i++)
				{
					clothes.hideBraOpt[i] = true;
				}
				for (int j = 0; j < clothes.hideShortsOpt.Length; j++)
				{
					clothes.hideShortsOpt[j] = true;
				}
			}
		}
		if (color)
		{
			for (int k = 0; k < clothes.parts.Length; k++)
			{
				for (int l = 0; l < clothes.parts[k].colorInfo.Length; l++)
				{
					clothes.parts[k].colorInfo[l].baseColor = new Color(Random.value, Random.value, Random.value);
					clothes.parts[k].colorInfo[l].patternColor = new Color(Random.value, Random.value, Random.value);
				}
			}
		}
		if (!ptn)
		{
			return;
		}
		for (int m = 0; m < clothes.parts.Length; m++)
		{
			for (int n = 0; n < clothes.parts[m].colorInfo.Length; n++)
			{
				clothes.parts[m].colorInfo[n].pattern = categoryInfo2.Keys.ElementAt(Random.Range(0, categoryInfo2.Keys.Count));
			}
		}
	}

	public static void RandomAccessory(ChaControl cha, bool slot, bool color)
	{
		RandomAccessory(cha.nowCoordinate.accessory, slot, color);
	}

	public static void RandomAccessory(ChaFileAccessory acs, bool slot, bool color)
	{
	}

	public static void RandomMakeup(ChaControl cha)
	{
		RandomMakeup(cha.nowCoordinate.makeup);
	}

	public static void RandomMakeup(ChaFileMakeup makeup)
	{
	}

	public static void RandomName(ChaControl cha, bool last, bool first, bool nick)
	{
		RandomName(cha.chaFile, last, first, nick);
	}

	public static void RandomName(ChaFileControl file, bool last, bool first, bool nick)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileParameter parameter = file.parameter;
		Character.RandamNameInfo randamNameInfo = Singleton<Character>.Instance.randamNameInfo;
		if (last && first)
		{
			int randomIndex = GetRandomIndex(95, 5);
			if (randomIndex == 0)
			{
				parameter.lastname = randamNameInfo.lstLastNameH[Random.Range(0, randamNameInfo.lstLastNameH.Count)];
			}
			else
			{
				parameter.lastname = randamNameInfo.lstLastNameK[Random.Range(0, randamNameInfo.lstLastNameK.Count)];
			}
			int num = 0;
			if (randomIndex == 0)
			{
				if (GetRandomIndex(97, 3) == 1)
				{
					num = 1;
				}
			}
			else
			{
				num = 1;
			}
			if (num == 0)
			{
				if (parameter.sex == 0)
				{
					parameter.firstname = randamNameInfo.lstMaleFirstNameH[Random.Range(0, randamNameInfo.lstMaleFirstNameH.Count)];
				}
				else
				{
					parameter.firstname = randamNameInfo.lstFemaleFirstNameH[Random.Range(0, randamNameInfo.lstFemaleFirstNameH.Count)];
				}
			}
			else if (parameter.sex == 0)
			{
				parameter.firstname = randamNameInfo.lstMaleFirstNameK[Random.Range(0, randamNameInfo.lstMaleFirstNameK.Count)];
			}
			else
			{
				parameter.firstname = randamNameInfo.lstFemaleFirstNameK[Random.Range(0, randamNameInfo.lstFemaleFirstNameK.Count)];
			}
			return;
		}
		if (last)
		{
			if (GetRandomIndex(95, 5) == 0)
			{
				parameter.lastname = randamNameInfo.lstLastNameH[Random.Range(0, randamNameInfo.lstLastNameH.Count)];
			}
			else
			{
				parameter.lastname = randamNameInfo.lstLastNameK[Random.Range(0, randamNameInfo.lstLastNameK.Count)];
			}
		}
		if (!first)
		{
			return;
		}
		if (GetRandomIndex(95, 5) == 0)
		{
			if (parameter.sex == 0)
			{
				parameter.firstname = randamNameInfo.lstMaleFirstNameH[Random.Range(0, randamNameInfo.lstMaleFirstNameH.Count)];
			}
			else
			{
				parameter.firstname = randamNameInfo.lstFemaleFirstNameH[Random.Range(0, randamNameInfo.lstFemaleFirstNameH.Count)];
			}
		}
		else if (parameter.sex == 0)
		{
			parameter.firstname = randamNameInfo.lstMaleFirstNameK[Random.Range(0, randamNameInfo.lstMaleFirstNameK.Count)];
		}
		else
		{
			parameter.firstname = randamNameInfo.lstFemaleFirstNameK[Random.Range(0, randamNameInfo.lstFemaleFirstNameK.Count)];
		}
	}

	public static void RandomParameter(ChaControl cha)
	{
		RandomParameter(cha.chaFile);
	}

	public static void RandomParameter(ChaFileControl file)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileParameter parameter = file.parameter;
		VoiceInfo.Param[] source = Singleton<Voice>.Instance.voiceInfoDic.Values.Where((VoiceInfo.Param x) => 0 <= x.No).ToArray();
		int[] array = source.Select((VoiceInfo.Param x) => x.No).ToArray();
		parameter.personality = (byte)array[Random.Range(0, array.Length)];
		parameter.bloodType = (byte)Random.Range(0, 5);
		parameter.birthMonth = (byte)Random.Range(1, 13);
		int[] array2 = new int[12]
		{
			31, 29, 31, 30, 31, 30, 31, 31, 30, 31,
			30, 31
		};
		parameter.birthDay = (byte)Random.Range(1, array2[parameter.birthMonth - 1] + 1);
		List<int> list = Game.ClubInfos.Values.Select((ClubInfo.Param x) => x.ID).ToList();
		parameter.clubActivities = (byte)list[Random.Range(0, list.Count)];
		parameter.weakPoint = Random.Range(-1, 6);
		parameter.awnser.animal = GetRandomBool();
		parameter.awnser.eat = GetRandomBool();
		parameter.awnser.cook = GetRandomBool();
		parameter.awnser.exercise = GetRandomBool();
		parameter.awnser.study = GetRandomBool();
		parameter.awnser.fashionable = GetRandomBool();
		parameter.awnser.blackCoffee = GetRandomBool();
		parameter.awnser.spicy = GetRandomBool();
		parameter.awnser.sweet = GetRandomBool();
		parameter.denial.kiss = GetRandomBool();
		parameter.denial.aibu = GetRandomBool();
		parameter.denial.anal = GetRandomBool();
		parameter.denial.massage = GetRandomBool();
		parameter.denial.notCondom = GetRandomBool();
		parameter.attribute.hinnyo = GetRandomBool();
		parameter.attribute.harapeko = GetRandomBool();
		parameter.attribute.donkan = GetRandomBool();
		parameter.attribute.choroi = GetRandomBool();
		parameter.attribute.bitch = GetRandomBool();
		parameter.attribute.mutturi = GetRandomBool();
		parameter.attribute.dokusyo = GetRandomBool();
		parameter.attribute.ongaku = GetRandomBool();
		parameter.attribute.kappatu = GetRandomBool();
		parameter.attribute.ukemi = GetRandomBool();
		parameter.attribute.friendly = GetRandomBool();
		parameter.attribute.kireizuki = GetRandomBool();
		parameter.attribute.taida = GetRandomBool();
		parameter.attribute.sinsyutu = GetRandomBool();
		parameter.attribute.hitori = GetRandomBool();
		parameter.attribute.undo = GetRandomBool();
		parameter.attribute.majime = GetRandomBool();
		parameter.attribute.likeGirls = GetRandomBool();
	}

	public static bool GetRandomBool()
	{
		return (Random.Range(0, 2) != 0) ? true : false;
	}

	public static int GetRandomIndex(params int[] weightTable)
	{
		int num = weightTable.Sum();
		int num2 = Random.Range(1, num + 1);
		int result = -1;
		for (int i = 0; i < weightTable.Length; i++)
		{
			if (weightTable[i] >= num2)
			{
				result = i;
				break;
			}
			num2 -= weightTable[i];
		}
		return result;
	}
}
