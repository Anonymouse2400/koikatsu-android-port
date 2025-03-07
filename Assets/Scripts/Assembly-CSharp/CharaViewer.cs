using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CharaFiles;
using Illusion.Extensions;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharaViewer : MonoBehaviour
{
	[SerializeField]
	private ChaFileListCtrl[] fileListCtrl;

	[SerializeField]
	private Toggle[] tglSex;

	[SerializeField]
	private TMP_Dropdown[] ddCoordinate;

	[SerializeField]
	private Transform trfContent;

	[SerializeField]
	private GameObject objInfoBase;

	[SerializeField]
	private Toggle tglSpecial;

	[SerializeField]
	private Button btnLoad;

	[SerializeField]
	private Button btnOutput;

	private ChaControl maleCtrl;

	private ChaControl femaleCtrl;

	private List<string> lstUseData = new List<string>();

	private void CreateCharaList()
	{
		FolderAssist folderAssist = new FolderAssist();
		string[] searchPattern = new string[1] { "*.png" };
		string[] array = new string[2] { "chara/male/", "chara/female/" };
		for (int i = 0; i < 2; i++)
		{
			string folder = UserData.Path + array[i];
			folderAssist.CreateFolderInfoEx(folder, searchPattern);
			fileListCtrl[i].ClearList();
			int fileCount = folderAssist.GetFileCount();
			int num = 0;
			for (int j = 0; j < fileCount; j++)
			{
				ChaFileControl chaFileControl = new ChaFileControl();
				if (!chaFileControl.LoadCharaFile(folderAssist.lstFile[j].FullPath))
				{
					int lastErrorCode = chaFileControl.GetLastErrorCode();
				}
				else if (chaFileControl.parameter.sex == i)
				{
					fileListCtrl[i].AddList(new ChaFileInfo(chaFileControl, folderAssist.lstFile[j])
					{
						index = num
					});
					num++;
				}
			}
		}
		fileListCtrl[0].Create(OnChangeChaFileListSelect);
		fileListCtrl[1].Create(OnChangeChaFileListSelect);
	}

	public void AddInfoObject(string title, int distributionNo, string name)
	{
		Color color = Color.white;
		string text = "範囲外";
		int num = 1;
		switch (distributionNo)
		{
		case -1:
			color = new Color(1f, 0.2f, 0.2f);
			break;
		case 1:
			text = "ソフマップ";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 2:
			text = "予約特典";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 3:
			text = "オフィシャル早期";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 4:
			text = "オフィシャル予約";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 5:
			text = "数量限定";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 6:
			text = "和セット";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 8:
			text = "メカ";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 9:
			text = "ファンタジー";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 10:
			text = "？？？";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 11:
			text = "髪詰め合わせ";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		case 12:
			text = "和セット";
			color = new Color(0.2f, 1f, 0.2f);
			break;
		default:
		{
			string text2 = "abdata";
			if (distributionNo != 0)
			{
				text2 = string.Format("add{0:00}", distributionNo);
			}
			text = text2;
			num = 0;
			break;
		}
		}
		GameObject gameObject = Object.Instantiate(objInfoBase);
		UI_Parameter component = gameObject.GetComponent<UI_Parameter>();
		component.index = num;
		Transform transform = gameObject.transform.Find("title");
		Text component2 = transform.GetComponent<Text>();
		component2.text = title;
		component2.color = color;
		Transform transform2 = gameObject.transform.Find("distribution");
		Text component3 = transform2.GetComponent<Text>();
		component3.text = text;
		component3.color = color;
		Transform transform3 = gameObject.transform.Find("name");
		Text component4 = transform3.GetComponent<Text>();
		component4.text = name;
		component4.color = color;
		gameObject.transform.SetParent(trfContent);
		if (tglSpecial.isOn && num == 0)
		{
			gameObject.SetActiveIfDifferent(false);
		}
		else
		{
			gameObject.SetActiveIfDifferent(true);
		}
	}

	public void OnChangeChaFileListSelect(ChaFileInfo chafileInfo)
	{
		if (null == trfContent || null == objInfoBase)
		{
			return;
		}
		for (int num = trfContent.childCount - 1; num >= 0; num--)
		{
			Transform child = trfContent.GetChild(num);
			Object.Destroy(child.gameObject);
		}
		ChaFileControl chaFileControl = new ChaFileControl();
		chaFileControl.skipRangeCheck = true;
		chaFileControl.LoadCharaFile(chafileInfo.FullPath);
		chaFileControl.skipRangeCheck = false;
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		ChaFileFace face = chaFileControl.custom.face;
		ChaFileBody body = chaFileControl.custom.body;
		ChaFileHair hair = chaFileControl.custom.hair;
		ChaFileCoordinate[] coordinate = chaFileControl.coordinate;
		ListInfoBase lib = null;
		lstUseData.Clear();
		Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_head);
		if (!categoryInfo.TryGetValue(face.headId, out lib))
		{
			AddInfoObject("【頭の種類】", -1, face.headId.ToString());
		}
		else
		{
			AddInfoObject("【頭の種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
			}
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MatData)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MatData));
			}
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex));
			}
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_face_detail);
		if (!categoryInfo.TryGetValue(face.detailId, out lib))
		{
			AddInfoObject("【彫りの種類】", -1, face.detailId.ToString());
		}
		else
		{
			AddInfoObject("【彫りの種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.NormallMapDetail)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.NormallMapDetail));
			}
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.LineMask)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.LineMask));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyebrow);
		if (!categoryInfo.TryGetValue(face.eyebrowId, out lib))
		{
			AddInfoObject("【眉毛の種類】", -1, face.eyebrowId.ToString());
		}
		else
		{
			AddInfoObject("【眉毛の種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyebrowTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyebrowTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_nose);
		if (!categoryInfo.TryGetValue(face.noseId, out lib))
		{
			AddInfoObject("【鼻の種類】", -1, face.noseId.ToString());
		}
		else
		{
			AddInfoObject("【鼻の種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.NoseTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.NoseTex));
			}
		}
		for (int i = 0; i < 2; i++)
		{
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye);
			if (!categoryInfo.TryGetValue(face.pupil[i].id, out lib))
			{
				AddInfoObject("【瞳の種類】", -1, face.pupil[i].id.ToString());
			}
			else
			{
				AddInfoObject("【瞳の種類】", lib.Distribution, lib.Name);
				if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyeTex)))
				{
					lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyeTex));
				}
			}
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_gradation);
			if (!categoryInfo.TryGetValue(face.pupil[i].gradMaskId, out lib))
			{
				AddInfoObject("【瞳のグラデマスク種類】", -1, face.pupil[i].gradMaskId.ToString());
				continue;
			}
			AddInfoObject("【瞳のグラデマスク種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_hi_up);
		if (!categoryInfo.TryGetValue(face.hlUpId, out lib))
		{
			AddInfoObject("【ハイライト上種類】", -1, face.hlUpId.ToString());
		}
		else
		{
			AddInfoObject("【ハイライト上種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyeHiUpTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyeHiUpTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_hi_down);
		if (!categoryInfo.TryGetValue(face.hlDownId, out lib))
		{
			AddInfoObject("【ハイライト下種類】", -1, face.hlDownId.ToString());
		}
		else
		{
			AddInfoObject("【ハイライト下種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyeHiDownTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyeHiDownTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eye_white);
		if (!categoryInfo.TryGetValue(face.whiteId, out lib))
		{
			AddInfoObject("【白目種類】", -1, face.whiteId.ToString());
		}
		else
		{
			AddInfoObject("【白目種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyeWhiteTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyeWhiteTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeline_up);
		if (!categoryInfo.TryGetValue(face.eyelineUpId, out lib))
		{
			AddInfoObject("【アイライン上種類】", -1, face.eyelineUpId.ToString());
		}
		else
		{
			AddInfoObject("【アイライン上種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyelineUpTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyelineUpTex));
			}
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyelineShadowTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyelineShadowTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeline_down);
		if (!categoryInfo.TryGetValue(face.eyelineDownId, out lib))
		{
			AddInfoObject("【アイライン下種類】", -1, face.eyelineDownId.ToString());
		}
		else
		{
			AddInfoObject("【アイライン下種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyelineDownTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyelineDownTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_mole);
		if (!categoryInfo.TryGetValue(face.moleId, out lib))
		{
			AddInfoObject("【ホクロの種類】", -1, face.moleId.ToString());
		}
		else
		{
			AddInfoObject("【ホクロの種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MoleTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MoleTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_lipline);
		if (!categoryInfo.TryGetValue(face.lipLineId, out lib))
		{
			AddInfoObject("【リップラインの種類】", -1, face.lipLineId.ToString());
		}
		else
		{
			AddInfoObject("【リップラインの種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.LiplineTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.LiplineTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_eyeshadow);
		if (!categoryInfo.TryGetValue(face.baseMakeup.eyeshadowId, out lib))
		{
			AddInfoObject("【アイシャドウ種類】", -1, face.baseMakeup.eyeshadowId.ToString());
		}
		else
		{
			AddInfoObject("【アイシャドウ種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.EyeshadowTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.EyeshadowTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_cheek);
		if (!categoryInfo.TryGetValue(face.baseMakeup.cheekId, out lib))
		{
			AddInfoObject("【チークの種類】", -1, face.baseMakeup.cheekId.ToString());
		}
		else
		{
			AddInfoObject("【チークの種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.CheekTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.CheekTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_lip);
		if (!categoryInfo.TryGetValue(face.baseMakeup.lipId, out lib))
		{
			AddInfoObject("【リップの種類】", -1, face.baseMakeup.lipId.ToString());
		}
		else
		{
			AddInfoObject("【リップの種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.LipTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.LipTex));
			}
		}
		for (int j = 0; j < 2; j++)
		{
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_face_paint);
			if (!categoryInfo.TryGetValue(face.baseMakeup.paintId[j], out lib))
			{
				AddInfoObject("【ペイント種類】", -1, face.baseMakeup.paintId[j].ToString());
				continue;
			}
			AddInfoObject("【ペイント種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.PaintTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.PaintTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_body_detail);
		if (!categoryInfo.TryGetValue(body.detailId, out lib))
		{
			AddInfoObject("【筋肉表現】", -1, body.detailId.ToString());
		}
		else
		{
			AddInfoObject("【筋肉表現】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.NormallMapDetail)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.NormallMapDetail));
			}
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.LineMask)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.LineMask));
			}
		}
		for (int k = 0; k < 2; k++)
		{
			categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_body_paint);
			if (!categoryInfo.TryGetValue(body.paintId[k], out lib))
			{
				AddInfoObject("【筋肉表現】", -1, body.paintId[k].ToString());
				continue;
			}
			AddInfoObject("【筋肉表現】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.PaintTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.PaintTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_sunburn);
		if (!categoryInfo.TryGetValue(body.sunburnId, out lib))
		{
			AddInfoObject("【日焼けの種類】", -1, body.sunburnId.ToString());
		}
		else
		{
			AddInfoObject("【日焼けの種類】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.SunburnTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.SunburnTex));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_b);
		if (!categoryInfo.TryGetValue(hair.parts[0].id, out lib))
		{
			AddInfoObject("【後ろ髪】", -1, hair.parts[0].id.ToString());
		}
		else
		{
			AddInfoObject("【後ろ髪】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_f);
		if (!categoryInfo.TryGetValue(hair.parts[1].id, out lib))
		{
			AddInfoObject("【前髪】", -1, hair.parts[1].id.ToString());
		}
		else
		{
			AddInfoObject("【前髪】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_s);
		if (!categoryInfo.TryGetValue(hair.parts[2].id, out lib))
		{
			AddInfoObject("【横髪】", -1, hair.parts[2].id.ToString());
		}
		else
		{
			AddInfoObject("【横髪】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.bo_hair_o);
		if (!categoryInfo.TryGetValue(hair.parts[3].id, out lib))
		{
			AddInfoObject("【エクステ】", -1, hair.parts[3].id.ToString());
		}
		else
		{
			AddInfoObject("【エクステ】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
			}
		}
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_hairgloss);
		if (!categoryInfo.TryGetValue(hair.glossId, out lib))
		{
			AddInfoObject("【グロスID】", -1, hair.glossId.ToString());
		}
		else
		{
			AddInfoObject("【グロスID】", lib.Distribution, lib.Name);
			if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex)))
			{
				lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex));
			}
		}
		string[] array = new string[9] { "トップス", "ボトムス", "ブラ", "ショーツ", "手袋", "パンスト", "靴下", "内履き", "外履き" };
		ChaListDefine.CategoryNo[] array2 = new ChaListDefine.CategoryNo[9]
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
		int[] array3 = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		string[,] array4 = new string[2, 3]
		{
			{ "セーラーA", "セーラーB", "セーラーC" },
			{ "ジャケットA", "ジャケットB", "ジャケットC" }
		};
		ChaListDefine.CategoryNo[,] array5 = new ChaListDefine.CategoryNo[2, 3]
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
		int[] array6 = new int[3] { 0, 1, 2 };
		for (int l = 0; l < coordinate.Length; l++)
		{
			ChaFileClothes clothes = coordinate[l].clothes;
			for (int m = 0; m < array3.Length; m++)
			{
				categoryInfo = chaListCtrl.GetCategoryInfo(array2[m]);
				if (!categoryInfo.TryGetValue(clothes.parts[array3[m]].id, out lib))
				{
					AddInfoObject(string.Format("【{0}】", array[m]), -1, clothes.parts[array3[m]].id.ToString());
				}
				else
				{
					AddInfoObject(string.Format("【{0}】", array[m]), lib.Distribution, lib.Name);
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.NormalData)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.NormalData));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.OverBodyMask)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.OverBodyMask));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.OverBraMask)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.OverBraMask));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex02)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex02));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.ColorMask02Tex)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.ColorMask02Tex));
					}
				}
				categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_emblem);
				if (!categoryInfo.TryGetValue(clothes.parts[array3[m]].emblemeId, out lib))
				{
					AddInfoObject("【エンブレム】", -1, clothes.parts[array3[m]].emblemeId.ToString());
					continue;
				}
				AddInfoObject("【エンブレム】", lib.Distribution, lib.Name);
				if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex)))
				{
					lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex));
				}
			}
			ListInfoBase listInfo = chaListCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[0].id);
			if (listInfo.Kind == 1 || listInfo.Kind == 2)
			{
				int num2 = ((listInfo.Kind != 1) ? 1 : 0);
				for (int n = 0; n < array6.Length; n++)
				{
					categoryInfo = chaListCtrl.GetCategoryInfo(array5[num2, n]);
					if (!categoryInfo.TryGetValue(clothes.subPartsId[array6[n]], out lib))
					{
						AddInfoObject(string.Format("【{0}】", array4[num2, n]), -1, clothes.subPartsId[array6[n]].ToString());
						continue;
					}
					AddInfoObject(string.Format("【{0}】", array4[num2, n]), lib.Distribution, lib.Name);
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.NormalData)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.NormalData));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.OverBodyMask)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.OverBodyMask));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.OverBraMask)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.OverBraMask));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.ColorMaskTex));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainTex02)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainTex02));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.ColorMask02Tex)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.ColorMask02Tex));
					}
					if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.OverInnerMask)))
					{
						lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.OverInnerMask));
					}
				}
			}
			ChaFileAccessory accessory = coordinate[l].accessory;
			for (int num3 = 0; num3 < accessory.parts.Length; num3++)
			{
				int type = accessory.parts[num3].type;
				int id = accessory.parts[num3].id;
				categoryInfo = chaListCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)type);
				if (categoryInfo == null)
				{
					continue;
				}
				if (!categoryInfo.TryGetValue(accessory.parts[num3].id, out lib))
				{
					AddInfoObject(string.Format("【{0}】", ChaAccessoryDefine.GetAccessoryTypeName((ChaListDefine.CategoryNo)type)), -1, accessory.parts[num3].id.ToString());
					continue;
				}
				AddInfoObject(string.Format("【{0}】", ChaAccessoryDefine.GetAccessoryTypeName((ChaListDefine.CategoryNo)type)), lib.Distribution, lib.Name);
				if (!lstUseData.Exists((string buf) => buf == lib.GetInfo(ChaListDefine.KeyType.MainData)))
				{
					lstUseData.Add(lib.GetInfo(ChaListDefine.KeyType.MainData));
				}
			}
		}
	}

	private void OutputText()
	{
		string path = Application.dataPath + "/LoadDataInfo.txt";
		using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
			{
				foreach (string lstUseDatum in lstUseData)
				{
					streamWriter.Write(lstUseDatum);
					streamWriter.Write("\n");
				}
			}
		}
	}

	private void Start()
	{
		CreateCharaList();
		maleCtrl = Singleton<Character>.Instance.CreateChara(0, null, 0);
		femaleCtrl = Singleton<Character>.Instance.CreateChara(1, null, 0);
		if (tglSex != null)
		{
			tglSex.Select((Toggle p, int index) => new
			{
				toggle = p,
				index = index
			}).ToList().ForEach(p =>
			{
				(from isOn in p.toggle.onValueChanged.AsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					fileListCtrl[p.index].gameObject.SetActiveIfDifferent(true);
					fileListCtrl[1 - p.index].gameObject.SetActiveIfDifferent(false);
					maleCtrl.SetActiveTop(p.index == 0);
					femaleCtrl.SetActiveTop(p.index == 1);
				});
			});
		}
		tglSpecial.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
		{
			UI_Parameter[] componentsInChildren = trfContent.GetComponentsInChildren<UI_Parameter>(true);
			UI_Parameter[] array = componentsInChildren;
			foreach (UI_Parameter uI_Parameter in array)
			{
				if (uI_Parameter.index == 0 && isOn)
				{
					uI_Parameter.gameObject.SetActiveIfDifferent(false);
				}
				else
				{
					uI_Parameter.gameObject.SetActiveIfDifferent(true);
				}
			}
		});
		btnLoad.OnClickAsObservable().Subscribe(delegate
		{
			LoadChara(true);
		});
		ddCoordinate.Select((TMP_Dropdown p, int index) => new
		{
			dd = p,
			index = index
		}).ToList().ForEach(p =>
		{
			p.dd.onValueChanged.AsObservable().Subscribe(delegate
			{
				LoadChara(false);
			});
		});
		btnOutput.OnClickAsObservable().Subscribe(delegate
		{
			OutputText();
		});
	}

	public void LoadChara(bool loadFile)
	{
		ChaControl chaControl = null;
		int num = ((!tglSex[0].isOn) ? 1 : 0);
		chaControl = ((num != 0) ? femaleCtrl : maleCtrl);
		if (!chaControl.loadEnd)
		{
			loadFile = true;
		}
		if (loadFile)
		{
			ChaFileInfoComponent selectTopItem = fileListCtrl[num].GetSelectTopItem();
			if (null == selectTopItem)
			{
				return;
			}
			chaControl.chaFile.LoadCharaFile(selectTopItem.info.FullPath);
		}
		chaControl.ChangeCoordinateType((ChaFileDefine.CoordinateType)ddCoordinate[num].value);
		if (chaControl.loadEnd)
		{
			chaControl.Reload();
		}
		else
		{
			chaControl.Load();
		}
		chaControl.LoadAnimation("custom/custompose.unity3d", "custom_pose", string.Empty);
	}

	private void Update()
	{
	}
}
