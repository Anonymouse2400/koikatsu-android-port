using System;
using System.Collections.Generic;
using System.IO;
using Manager;
using MessagePack;
using UnityEngine;

public class ChaFileControl : ChaFile
{
	private static bool saveBytesMode;

	public bool skipRangeCheck;

	public void ChangeSaveBytesMode(bool bytes)
	{
		saveBytesMode = bytes;
	}

	public ChaFilePreview GetPreviewInfo()
	{
		ChaFilePreview chaFilePreview = new ChaFilePreview();
		chaFilePreview.sex = parameter.sex;
		chaFilePreview.lastname = parameter.lastname;
		chaFilePreview.firstname = parameter.firstname;
		chaFilePreview.nickname = parameter.nickname;
		chaFilePreview.personality = parameter.personality;
		chaFilePreview.bloodType = parameter.bloodType;
		chaFilePreview.birthMonth = parameter.birthMonth;
		chaFilePreview.birthDay = parameter.birthDay;
		chaFilePreview.clubActivities = parameter.clubActivities;
		chaFilePreview.voicePitch = parameter.voicePitch;
		return chaFilePreview;
	}

	public static bool CheckDataRangeFace(ChaFile chafile, List<string> checkInfo = null)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileFace face = chafile.custom.face;
		bool result = false;
		for (int i = 0; i < face.shapeValueFace.Length; i++)
		{
			if (!MathfEx.RangeEqualOn(0f, face.shapeValueFace[i], 1f))
			{
				if (checkInfo != null)
				{
					checkInfo.Add(string.Format("【{0}】値:{1}", ChaFileDefine.cf_headshapename[i], face.shapeValueFace[i]));
				}
				result = true;
				face.shapeValueFace[i] = Mathf.Clamp(face.shapeValueFace[i], 0f, 1f);
			}
		}
		Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_head);
		if (!categoryInfo.ContainsKey(face.headId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【頭の種類】値:{0}", face.headId));
			}
			result = true;
			face.headId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_face_detail);
		if (!categoryInfo.ContainsKey(face.detailId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【彫りの種類】値:{0}", face.detailId));
			}
			result = true;
			face.detailId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyebrow);
		if (!categoryInfo.ContainsKey(face.eyebrowId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【眉毛の種類】値:{0}", face.eyebrowId));
			}
			result = true;
			face.eyebrowId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_nose);
		if (!categoryInfo.ContainsKey(face.noseId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【鼻の種類】値:{0}", face.noseId));
			}
			result = true;
			face.noseId = 0;
		}
		for (int j = 0; j < 2; j++)
		{
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye);
			if (!categoryInfo.ContainsKey(face.pupil[j].id))
			{
				if (checkInfo != null)
				{
					checkInfo.Add(string.Format("【瞳の種類】値:{0}", face.pupil[j].id));
				}
				result = true;
				face.pupil[j].id = 0;
			}
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_gradation);
			if (!categoryInfo.ContainsKey(face.pupil[j].gradMaskId))
			{
				if (checkInfo != null)
				{
					checkInfo.Add(string.Format("【瞳のグラデマスク種類】値:{0}", face.pupil[j].gradMaskId));
				}
				result = true;
				face.pupil[j].gradMaskId = 0;
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_hi_up);
		if (!categoryInfo.ContainsKey(face.hlUpId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【ハイライト上種類】値:{0}", face.hlUpId));
			}
			result = true;
			face.hlUpId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_hi_down);
		if (!categoryInfo.ContainsKey(face.hlDownId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【ハイライト下種類】値:{0}", face.hlDownId));
			}
			result = true;
			face.hlDownId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_white);
		if (!categoryInfo.ContainsKey(face.whiteId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【白目種類】値:{0}", face.whiteId));
			}
			result = true;
			face.whiteId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeline_up);
		if (!categoryInfo.ContainsKey(face.eyelineUpId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【アイライン上種類】値:{0}", face.eyelineUpId));
			}
			result = true;
			face.eyelineUpId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeline_down);
		if (!categoryInfo.ContainsKey(face.eyelineDownId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【アイライン下種類】値:{0}", face.eyelineDownId));
			}
			result = true;
			face.eyelineDownId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_mole);
		if (!categoryInfo.ContainsKey(face.moleId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【ホクロの種類】値:{0}", face.moleId));
			}
			result = true;
			face.moleId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_lipline);
		if (!categoryInfo.ContainsKey(face.lipLineId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【リップラインの種類】値:{0}", face.lipLineId));
			}
			result = true;
			face.lipLineId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeshadow);
		if (!categoryInfo.ContainsKey(face.baseMakeup.eyeshadowId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【アイシャドウ種類】値:{0}", face.baseMakeup.eyeshadowId));
			}
			result = true;
			face.baseMakeup.eyeshadowId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_cheek);
		if (!categoryInfo.ContainsKey(face.baseMakeup.cheekId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【チークの種類】値:{0}", face.baseMakeup.cheekId));
			}
			result = true;
			face.baseMakeup.cheekId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_lip);
		if (!categoryInfo.ContainsKey(face.baseMakeup.lipId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【リップの種類】値:{0}", face.baseMakeup.lipId));
			}
			result = true;
			face.baseMakeup.lipId = 0;
		}
		for (int k = 0; k < 2; k++)
		{
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_face_paint);
			if (!categoryInfo.ContainsKey(face.baseMakeup.paintId[k]))
			{
				if (checkInfo != null)
				{
					checkInfo.Add(string.Format("【ペイント種類】値:{0}", face.baseMakeup.paintId[k]));
				}
				result = true;
				face.baseMakeup.paintId[k] = 0;
			}
		}
		return result;
	}

	public static bool CheckDataRangeBody(ChaFile chafile, List<string> checkInfo = null)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileBody body = chafile.custom.body;
		bool result = false;
		for (int i = 0; i < body.shapeValueBody.Length; i++)
		{
			if (!MathfEx.RangeEqualOn(0f, body.shapeValueBody[i], 1f))
			{
				if (checkInfo != null)
				{
					checkInfo.Add(string.Format("【{0}】値:{1}", ChaFileDefine.cf_bodyshapename[i], body.shapeValueBody[i]));
				}
				result = true;
				body.shapeValueBody[i] = Mathf.Clamp(body.shapeValueBody[i], 0f, 1f);
			}
		}
		if (!MathfEx.RangeEqualOn(0f, body.areolaSize, 1f))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【乳輪のサイズ】値:{0}", body.areolaSize));
			}
			result = true;
			body.areolaSize = Mathf.Clamp(body.areolaSize, 0f, 1f);
		}
		if (!MathfEx.RangeEqualOn(0f, body.bustSoftness, 1f))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【胸の柔らかさ】値:{0}", body.bustSoftness));
			}
			result = true;
			body.bustSoftness = Mathf.Clamp(body.bustSoftness, 0f, 1f);
		}
		if (!MathfEx.RangeEqualOn(0f, body.bustWeight, 1f))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【胸の重さ】値:{0}", body.bustWeight));
			}
			result = true;
			body.bustWeight = Mathf.Clamp(body.bustWeight, 0f, 1f);
		}
		Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_body_detail);
		if (!categoryInfo.ContainsKey(body.detailId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【筋肉表現】値:{0}", body.detailId));
			}
			result = true;
			body.detailId = 0;
		}
		for (int j = 0; j < 2; j++)
		{
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_body_paint);
			if (!categoryInfo.ContainsKey(body.paintId[j]))
			{
				if (checkInfo != null)
				{
					checkInfo.Add(string.Format("【ペイントの種類】値:{0}", body.paintId[j]));
				}
				result = true;
				body.paintId[j] = 0;
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_sunburn);
		if (!categoryInfo.ContainsKey(body.sunburnId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【日焼けの種類】値:{0}", body.sunburnId));
			}
			result = true;
			body.sunburnId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_nip);
		if (!categoryInfo.ContainsKey(body.nipId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【乳首の種類】値:{0}", body.nipId));
			}
			result = true;
			body.nipId = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_underhair);
		if (!categoryInfo.ContainsKey(body.underhairId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【アンダーヘア種類】値:{0}", body.underhairId));
			}
			result = true;
			body.underhairId = 0;
		}
		return result;
	}

	public static bool CheckDataRangeHair(ChaFile chafile, List<string> checkInfo = null)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileHair hair = chafile.custom.hair;
		bool result = false;
		Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_b);
		if (!categoryInfo.ContainsKey(hair.parts[0].id))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【後ろ髪】値:{0}", hair.parts[0].id));
			}
			result = true;
			hair.parts[0].id = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_f);
		if (!categoryInfo.ContainsKey(hair.parts[1].id))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【前髪】値:{0}", hair.parts[1].id));
			}
			result = true;
			hair.parts[1].id = 1;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_s);
		if (!categoryInfo.ContainsKey(hair.parts[2].id))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【横髪】値:{0}", hair.parts[2].id));
			}
			result = true;
			hair.parts[2].id = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_o);
		if (!categoryInfo.ContainsKey(hair.parts[3].id))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【エクステ】値:{0}", hair.parts[3].id));
			}
			result = true;
			hair.parts[3].id = 0;
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_hairgloss);
		if (!categoryInfo.ContainsKey(hair.glossId))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【グロスID】値:{0}", hair.glossId));
			}
			result = true;
			hair.glossId = 0;
		}
		return result;
	}

	public static bool CheckDataRangeCoordinate(ChaFile chafile, List<string> checkInfo = null)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileCoordinate[] array = chafile.coordinate;
		bool result = false;
		string[] array2 = new string[9] { "トップス", "ボトムス", "ブラ", "ショーツ", "手袋", "パンスト", "靴下", "内履き", "外履き" };
		ChaListDefine.CategoryNo[] array3 = new ChaListDefine.CategoryNo[9]
		{
			ChaListDefine.CategoryNo.co_top,
			ChaListDefine.CategoryNo.co_bot,
			ChaListDefine.CategoryNo.co_bra,
			ChaListDefine.CategoryNo.co_shorts,
			ChaListDefine.CategoryNo.co_gloves,
			ChaListDefine.CategoryNo.co_panst,
			ChaListDefine.CategoryNo.co_socks,
			ChaListDefine.CategoryNo.co_shoes,
			ChaListDefine.CategoryNo.co_shoes
		};
		int[] array4 = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		int[] array5 = new int[9];
		string[,] array6 = new string[2, 3]
		{
			{ "セーラーA", "セーラーB", "セーラーC" },
			{ "ジャケットA", "ジャケットB", "ジャケットC" }
		};
		ChaListDefine.CategoryNo[,] array7 = new ChaListDefine.CategoryNo[2, 3]
		{
			{
				ChaListDefine.CategoryNo.cpo_sailor_a,
				ChaListDefine.CategoryNo.cpo_sailor_b,
				ChaListDefine.CategoryNo.cpo_sailor_c
			},
			{
				ChaListDefine.CategoryNo.cpo_jacket_a,
				ChaListDefine.CategoryNo.cpo_jacket_b,
				ChaListDefine.CategoryNo.cpo_jacket_c
			}
		};
		int[] array8 = new int[3] { 0, 1, 2 };
		int[,] array9 = new int[2, 3]
		{
			{ 0, 0, 1 },
			{ 0, 1, 1 }
		};
		Dictionary<int, ListInfoBase> dictionary = null;
		for (int i = 0; i < array.Length; i++)
		{
			ChaFileClothes clothes = array[i].clothes;
			for (int j = 0; j < array4.Length; j++)
			{
				dictionary = chaListCtrl.GetCategoryInfo(array3[j]);
				if (!dictionary.ContainsKey(clothes.parts[array4[j]].id))
				{
					if (checkInfo != null)
					{
						checkInfo.Add(string.Format("【{0}】値:{1}", array2[j], clothes.parts[array4[j]].id));
					}
					result = true;
					clothes.parts[array4[j]].id = array5[j];
				}
				dictionary = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_emblem);
				if (!dictionary.ContainsKey(clothes.parts[array4[j]].emblemeId))
				{
					if (checkInfo != null)
					{
						checkInfo.Add(string.Format("【エンブレム】値:{0}", clothes.parts[array4[j]].emblemeId));
					}
					result = true;
					clothes.parts[array4[j]].emblemeId = 0;
				}
			}
			ListInfoBase listInfo = chaListCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[0].id);
			if (listInfo.Kind == 1 || listInfo.Kind == 2)
			{
				int num = ((listInfo.Kind != 1) ? 1 : 0);
				for (int k = 0; k < array8.Length; k++)
				{
					dictionary = chaListCtrl.GetCategoryInfo(array7[num, k]);
					if (!dictionary.ContainsKey(clothes.subPartsId[array8[k]]))
					{
						if (checkInfo != null)
						{
							checkInfo.Add(string.Format("【{0}】値:{1}", array6[num, k], clothes.subPartsId[array8[k]]));
						}
						result = true;
						clothes.subPartsId[array8[k]] = array9[num, k];
					}
				}
			}
			ChaFileAccessory accessory = array[i].accessory;
			for (int l = 0; l < accessory.parts.Length; l++)
			{
				int type = accessory.parts[l].type;
				int id = accessory.parts[l].id;
				dictionary = chaListCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)type);
				if (dictionary != null && !dictionary.ContainsKey(accessory.parts[l].id))
				{
					if (checkInfo != null)
					{
						checkInfo.Add(string.Format("【{0}】値:{1}", ChaAccessoryDefine.GetAccessoryTypeName((ChaListDefine.CategoryNo)type), accessory.parts[l].id));
					}
					result = true;
					accessory.parts[l].MemberInit();
				}
			}
		}
		return result;
	}

	public static bool CheckDataRangeParameter(ChaFile chafile, List<string> checkInfo = null)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileParameter chaFileParameter = chafile.parameter;
		bool result = false;
		if (chaFileParameter.sex == 0)
		{
			return false;
		}
		if (!Singleton<Voice>.Instance.voiceInfoDic.ContainsKey(chaFileParameter.personality))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【性格】値:{0}", chaFileParameter.personality));
			}
			result = true;
			chaFileParameter.personality = 0;
		}
		if (!Game.ClubInfos.ContainsKey(chaFileParameter.clubActivities))
		{
			if (checkInfo != null)
			{
				checkInfo.Add(string.Format("【部活】値:{0}", chaFileParameter.clubActivities));
			}
			result = true;
			chaFileParameter.clubActivities = 0;
		}
		return result;
	}

	public static bool CheckDataRange(ChaFile chafile, bool chk_custom, bool chk_clothes, bool chk_parameter, List<string> checkInfo = null)
	{
		bool flag = false;
		if (chk_custom)
		{
			flag |= CheckDataRangeFace(chafile, checkInfo);
			flag |= CheckDataRangeBody(chafile, checkInfo);
			flag |= CheckDataRangeHair(chafile, checkInfo);
		}
		if (chk_clothes)
		{
			flag |= CheckDataRangeCoordinate(chafile, checkInfo);
		}
		if (chk_parameter)
		{
			flag |= CheckDataRangeParameter(chafile, checkInfo);
		}
		if (flag)
		{
		}
		return flag;
	}

	public bool SaveCharaFile(string filename, byte sex = byte.MaxValue, bool newFile = false)
	{
		string path = ConvertCharaFilePath(filename, sex, newFile);
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		base.charaFileName = Path.GetFileName(path);
		using (FileStream st = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			return SaveCharaFile(st, true);
		}
	}

	public bool SaveCharaFile(Stream st, bool savePng)
	{
		using (BinaryWriter bw = new BinaryWriter(st))
		{
			return SaveCharaFile(bw, savePng);
		}
	}

	public bool SaveCharaFile(BinaryWriter bw, bool savePng)
	{
		return SaveFile(bw, savePng);
	}

	public bool LoadCharaFile(string filename, byte sex = byte.MaxValue, bool noLoadPng = false, bool noLoadStatus = true)
	{
		base.charaFileName = Path.GetFileName(filename);
		string path = ConvertCharaFilePath(filename, sex);
		using (FileStream st = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			return LoadCharaFile(st, noLoadPng, noLoadStatus);
		}
	}

	public bool LoadCharaFile(Stream st, bool noLoadPng = false, bool noLoadStatus = true)
	{
		using (BinaryReader br = new BinaryReader(st))
		{
			return LoadCharaFile(br, noLoadPng, noLoadStatus);
		}
	}

	public bool LoadCharaFile(BinaryReader br, bool noLoadPng = false, bool noLoadStatus = true)
	{
		bool result = LoadFile(br, noLoadPng, noLoadStatus);
		if (!skipRangeCheck)
		{
			CheckDataRange(this, true, true, true);
		}
		return result;
	}

	public bool LoadFileLimited(string filename, byte sex = byte.MaxValue, bool face = true, bool body = true, bool hair = true, bool parameter = true, bool coordinate = true)
	{
		string path = ConvertCharaFilePath(filename, sex);
		ChaFileControl chaFileControl = new ChaFileControl();
		if (chaFileControl.LoadFile(path))
		{
			if (!skipRangeCheck)
			{
				CheckDataRange(chaFileControl, true, true, true);
			}
			byte[] array = null;
			if (face)
			{
				array = MessagePackSerializer.Serialize(chaFileControl.custom.face);
				custom.face = MessagePackSerializer.Deserialize<ChaFileFace>(array);
			}
			if (body)
			{
				array = MessagePackSerializer.Serialize(chaFileControl.custom.body);
				custom.body = MessagePackSerializer.Deserialize<ChaFileBody>(array);
			}
			if (hair)
			{
				array = MessagePackSerializer.Serialize(chaFileControl.custom.hair);
				custom.hair = MessagePackSerializer.Deserialize<ChaFileHair>(array);
			}
			if (parameter)
			{
				base.parameter.Copy(chaFileControl.parameter);
			}
			if (coordinate)
			{
				CopyCoordinate(chaFileControl.coordinate);
			}
		}
		return false;
	}

	public bool LoadFileLimited(string assetBundleName, string assetName, bool face = true, bool body = true, bool hair = true, bool parameter = true, bool coordinate = true)
	{
		ChaFileControl chaFileControl = new ChaFileControl();
		TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
		if (null == textAsset)
		{
			return false;
		}
		if (chaFileControl.LoadFromTextAsset(textAsset, true))
		{
			byte[] array = null;
			if (face)
			{
				array = MessagePackSerializer.Serialize(chaFileControl.custom.face);
				custom.face = MessagePackSerializer.Deserialize<ChaFileFace>(array);
			}
			if (body)
			{
				array = MessagePackSerializer.Serialize(chaFileControl.custom.body);
				custom.body = MessagePackSerializer.Deserialize<ChaFileBody>(array);
			}
			if (hair)
			{
				array = MessagePackSerializer.Serialize(chaFileControl.custom.hair);
				custom.hair = MessagePackSerializer.Deserialize<ChaFileHair>(array);
			}
			if (parameter)
			{
				base.parameter.Copy(chaFileControl.parameter);
			}
			if (coordinate)
			{
				CopyCoordinate(chaFileControl.coordinate);
			}
		}
		return false;
	}

	public bool LoadFromAssetBundle(string assetBundleName, string assetName, bool noSetPNG = false, bool noLoadStatus = true)
	{
		TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
		if (null == textAsset)
		{
			return false;
		}
		bool result = LoadFromTextAsset(textAsset, noSetPNG, noLoadStatus);
		AssetBundleManager.UnloadAssetBundle(assetBundleName, true, null, true);
		return result;
	}

	public bool LoadFromTextAsset(TextAsset ta, bool noSetPNG = false, bool noLoadStatus = true)
	{
		if (null == ta)
		{
			return false;
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			memoryStream.Write(ta.bytes, 0, ta.bytes.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return LoadFile(memoryStream, noSetPNG, noLoadStatus);
		}
	}

	public bool LoadFromBytes(byte[] bytes, bool noSetPNG = false, bool noLoadStatus = true)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			memoryStream.Write(bytes, 0, bytes.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			bool result = LoadFile(memoryStream, noSetPNG, noLoadStatus);
			if (!skipRangeCheck)
			{
				CheckDataRange(this, true, true, true);
			}
			return result;
		}
	}

	public string ConvertCharaFilePath(string path, byte _sex, bool newFile = false)
	{
		byte b = ((_sex != byte.MaxValue) ? _sex : parameter.sex);
		string text = string.Empty;
		string text2 = string.Empty;
		if (path != string.Empty)
		{
			text = Path.GetDirectoryName(path);
			text2 = Path.GetFileName(path);
		}
		text = ((!(text == string.Empty)) ? (text + "/") : (UserData.Path + ((b != 0) ? "chara/female/" : "chara/male/")));
		if (text2 == string.Empty)
		{
			text2 = ((!newFile && !(base.charaFileName == string.Empty)) ? base.charaFileName : ((b != 0) ? ("KoiKatu_F_" + DateTime.Now.ToString("yyyyMMddHHmmssfff")) : ("KoiKatu_M_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"))));
		}
		if (saveBytesMode)
		{
			return text + Path.GetFileNameWithoutExtension(text2) + ".bytes";
		}
		return text + Path.GetFileNameWithoutExtension(text2) + ".png";
	}

	public static string ConvertCoordinateFilePath(string path)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (path != string.Empty)
		{
			text = Path.GetDirectoryName(path);
			text2 = Path.GetFileName(path);
		}
		text = ((!(text == string.Empty)) ? (text + "/") : (UserData.Path + "coordinate/"));
		if (text2 == string.Empty)
		{
			text2 = "KKCoorde_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
		}
		if (saveBytesMode)
		{
			return text + Path.GetFileNameWithoutExtension(text2) + ".bytes";
		}
		return text + Path.GetFileNameWithoutExtension(text2) + ".png";
	}
}
