using System.Collections.Generic;
using System.Linq;
using Manager;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomSelectKind : MonoBehaviour
	{
		public enum SelectKindType
		{
			FaceDetail = 0,
			Eyebrow = 1,
			EyelineUp = 2,
			EyelineDown = 3,
			EyeWGrade = 4,
			EyeHLUp = 5,
			EyeHLDown = 6,
			Pupil = 7,
			PupilGrade = 8,
			Nose = 9,
			Lipline = 10,
			Mole = 11,
			Eyeshadow = 12,
			Cheek = 13,
			Lip = 14,
			FacePaint01 = 15,
			FacePaint02 = 16,
			BodyDetail = 17,
			Nip = 18,
			Underhair = 19,
			Sunburn = 20,
			BodyPaint01 = 21,
			BodyPaint02 = 22,
			BodyPaint01Layout = 23,
			BodyPaint02Layout = 24,
			HairBack = 25,
			HairFront = 26,
			HairSide = 27,
			HairExtension = 28,
			CosTop = 29,
			CosSailor01 = 30,
			CosSailor02 = 31,
			CosSailor03 = 32,
			CosJacket01 = 33,
			CosJacket02 = 34,
			CosJacket03 = 35,
			CosTopPtn01 = 36,
			CosTopPtn02 = 37,
			CosTopPtn03 = 38,
			CosTopPtn04 = 39,
			CosTopEmblem = 40,
			CosBot = 41,
			CosBotPtn01 = 42,
			CosBotPtn02 = 43,
			CosBotPtn03 = 44,
			CosBotPtn04 = 45,
			CosBotEmblem = 46,
			CosBra = 47,
			CosBraPtn01 = 48,
			CosBraPtn02 = 49,
			CosBraPtn03 = 50,
			CosBraPtn04 = 51,
			CosBraEmblem = 52,
			CosShorts = 53,
			CosShortsPtn01 = 54,
			CosShortsPtn02 = 55,
			CosShortsPtn03 = 56,
			CosShortsPtn04 = 57,
			CosShortsEmblem = 58,
			CosGloves = 59,
			CosGlovesPtn01 = 60,
			CosGlovesPtn02 = 61,
			CosGlovesPtn03 = 62,
			CosGlovesPtn04 = 63,
			CosGlovesEmblem = 64,
			CosPanst = 65,
			CosPanstPtn01 = 66,
			CosPanstPtn02 = 67,
			CosPanstPtn03 = 68,
			CosPanstPtn04 = 69,
			CosPanstEmblem = 70,
			CosSocks = 71,
			CosSocksPtn01 = 72,
			CosSocksPtn02 = 73,
			CosSocksPtn03 = 74,
			CosSocksPtn04 = 75,
			CosSocksEmblem = 76,
			CosInnerShoes = 77,
			CosInnerShoesPtn01 = 78,
			CosInnerShoesPtn02 = 79,
			CosInnerShoesPtn03 = 80,
			CosInnerShoesPtn04 = 81,
			CosInnerShoesEmblem = 82,
			CosOuterShoes = 83,
			CosOuterShoesPtn01 = 84,
			CosOuterShoesPtn02 = 85,
			CosOuterShoesPtn03 = 86,
			CosOuterShoesPtn04 = 87,
			CosOuterShoesEmblem = 88,
			HairGloss = 89
		}

		[SerializeField]
		private SelectKindType type;

		[SerializeField]
		private CustomSelectListCtrl listCtrl;

		[SerializeField]
		private CvsFaceAll cvsFaceAll;

		[SerializeField]
		private CvsEyebrow cvsEyebrow;

		[SerializeField]
		private CvsEye01 cvsEye01;

		[SerializeField]
		private CvsEye02 cvsEye02;

		[SerializeField]
		private CvsNose cvsNose;

		[SerializeField]
		private CvsMouth cvsMouth;

		[SerializeField]
		private CvsMole cvsMole;

		[SerializeField]
		private CvsMakeup cvsMakeup;

		[SerializeField]
		private CvsBodyAll cvsBodyAll;

		[SerializeField]
		private CvsBreast cvsBreast;

		[SerializeField]
		private CvsUnderhair cvsUnderhair;

		[SerializeField]
		private CvsSunburn cvsSunburn;

		[SerializeField]
		private CvsBodyPaint cvsBodyPaint;

		[SerializeField]
		private CvsHair cvsHair;

		[SerializeField]
		private CvsHairEtc cvsHairEtc;

		[SerializeField]
		private CvsClothes cvsClothes;

		private CanvasGroup parentCG;

		private CanvasGroup myCG;

		private bool parentInteractable;

		private bool myInteractable;

		private bool InitEnd;

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileFace face
		{
			get
			{
				return chaCtrl.chaFile.custom.face;
			}
		}

		private ChaFileBody body
		{
			get
			{
				return chaCtrl.chaFile.custom.body;
			}
		}

		private ChaFileHair hair
		{
			get
			{
				return chaCtrl.chaFile.custom.hair;
			}
		}

		private ChaFileClothes clothes
		{
			get
			{
				return chaCtrl.nowCoordinate.clothes;
			}
		}

		private void Awake()
		{
			myCG = base.gameObject.GetComponent<CanvasGroup>();
			parentCG = base.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			Observable.EveryUpdate().Subscribe(delegate
			{
				bool flag = false;
				if ((bool)myCG)
				{
					if (myCG.interactable && !myInteractable)
					{
						flag = true;
					}
					myInteractable = myCG.interactable;
				}
				if ((bool)parentCG)
				{
					if (parentCG.interactable && !parentInteractable)
					{
						flag = true;
					}
					parentInteractable = parentCG.interactable;
				}
				if (flag)
				{
					if (!InitEnd)
					{
						Initialize();
					}
					if ((bool)listCtrl)
					{
						listCtrl.UpdateStateNew();
					}
				}
			}).AddTo(this);
		}

		private void Initialize()
		{
			ChaListDefine.CategoryNo[] array = new ChaListDefine.CategoryNo[90]
			{
				ChaListDefine.CategoryNo.mt_face_detail,
				ChaListDefine.CategoryNo.mt_eyebrow,
				ChaListDefine.CategoryNo.mt_eyeline_up,
				ChaListDefine.CategoryNo.mt_eyeline_down,
				ChaListDefine.CategoryNo.mt_eye_white,
				ChaListDefine.CategoryNo.mt_eye_hi_up,
				ChaListDefine.CategoryNo.mt_eye_hi_down,
				ChaListDefine.CategoryNo.mt_eye,
				ChaListDefine.CategoryNo.mt_eye_gradation,
				ChaListDefine.CategoryNo.mt_nose,
				ChaListDefine.CategoryNo.mt_lipline,
				ChaListDefine.CategoryNo.mt_mole,
				ChaListDefine.CategoryNo.mt_eyeshadow,
				ChaListDefine.CategoryNo.mt_cheek,
				ChaListDefine.CategoryNo.mt_lip,
				ChaListDefine.CategoryNo.mt_face_paint,
				ChaListDefine.CategoryNo.mt_face_paint,
				ChaListDefine.CategoryNo.mt_body_detail,
				ChaListDefine.CategoryNo.mt_nip,
				ChaListDefine.CategoryNo.mt_underhair,
				ChaListDefine.CategoryNo.mt_sunburn,
				ChaListDefine.CategoryNo.mt_body_paint,
				ChaListDefine.CategoryNo.mt_body_paint,
				ChaListDefine.CategoryNo.bodypaint_layout,
				ChaListDefine.CategoryNo.bodypaint_layout,
				ChaListDefine.CategoryNo.bo_hair_b,
				ChaListDefine.CategoryNo.bo_hair_f,
				ChaListDefine.CategoryNo.bo_hair_s,
				ChaListDefine.CategoryNo.bo_hair_o,
				ChaListDefine.CategoryNo.co_top,
				ChaListDefine.CategoryNo.cpo_sailor_a,
				ChaListDefine.CategoryNo.cpo_sailor_b,
				ChaListDefine.CategoryNo.cpo_sailor_c,
				ChaListDefine.CategoryNo.cpo_jacket_a,
				ChaListDefine.CategoryNo.cpo_jacket_b,
				ChaListDefine.CategoryNo.cpo_jacket_c,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_bot,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_bra,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_shorts,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_gloves,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_panst,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_socks,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_shoes,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.co_shoes,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_pattern,
				ChaListDefine.CategoryNo.mt_emblem,
				ChaListDefine.CategoryNo.mt_hairgloss
			};
			ChaListDefine.CategoryNo cn = array[(int)type];
			ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
			Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(cn);
			List<ListInfoBase> list = categoryInfo.Values.Where(delegate(ListInfoBase val)
			{
				if (cn == ChaListDefine.CategoryNo.co_top || cn == ChaListDefine.CategoryNo.co_bra)
				{
					int infoInt = val.GetInfoInt(ChaListDefine.KeyType.Sex);
					if (chaCtrl.fileParam.sex == 0 && infoInt == 3)
					{
						return false;
					}
					if (chaCtrl.fileParam.sex == 1 && infoInt == 2)
					{
						return false;
					}
				}
				return true;
			}).ToList();
			list.ForEach(delegate(ListInfoBase info)
			{
				listCtrl.AddList(info.Category, info.Id, info.Name, info.GetInfo(ChaListDefine.KeyType.ThumbAB), info.GetInfo(ChaListDefine.KeyType.ThumbTex));
			});
			listCtrl.Create(OnSelect);
			Singleton<CustomBase>.Instance.updateCustomUI = true;
			InitEnd = true;
		}

		public void UpdateCustomUI(int param = 0)
		{
			if (InitEnd)
			{
				int[] array = new int[90]
				{
					face.detailId,
					face.eyebrowId,
					face.eyelineUpId,
					face.eyelineDownId,
					face.whiteId,
					face.hlUpId,
					face.hlDownId,
					face.pupil[param].id,
					face.pupil[param].gradMaskId,
					face.noseId,
					face.lipLineId,
					face.moleId,
					face.baseMakeup.eyeshadowId,
					face.baseMakeup.cheekId,
					face.baseMakeup.lipId,
					face.baseMakeup.paintId[0],
					face.baseMakeup.paintId[1],
					body.detailId,
					body.nipId,
					body.underhairId,
					body.sunburnId,
					body.paintId[0],
					body.paintId[1],
					body.paintLayoutId[0],
					body.paintLayoutId[1],
					hair.parts[0].id,
					hair.parts[1].id,
					hair.parts[2].id,
					hair.parts[3].id,
					clothes.parts[0].id,
					clothes.subPartsId[0],
					clothes.subPartsId[1],
					clothes.subPartsId[2],
					clothes.subPartsId[0],
					clothes.subPartsId[1],
					clothes.subPartsId[2],
					clothes.parts[0].colorInfo[0].pattern,
					clothes.parts[0].colorInfo[1].pattern,
					clothes.parts[0].colorInfo[2].pattern,
					clothes.parts[0].colorInfo[3].pattern,
					clothes.parts[0].emblemeId,
					clothes.parts[1].id,
					clothes.parts[1].colorInfo[0].pattern,
					clothes.parts[1].colorInfo[1].pattern,
					clothes.parts[1].colorInfo[2].pattern,
					clothes.parts[1].colorInfo[3].pattern,
					clothes.parts[1].emblemeId,
					clothes.parts[2].id,
					clothes.parts[2].colorInfo[0].pattern,
					clothes.parts[2].colorInfo[1].pattern,
					clothes.parts[2].colorInfo[2].pattern,
					clothes.parts[2].colorInfo[3].pattern,
					clothes.parts[2].emblemeId,
					clothes.parts[3].id,
					clothes.parts[3].colorInfo[0].pattern,
					clothes.parts[3].colorInfo[1].pattern,
					clothes.parts[3].colorInfo[2].pattern,
					clothes.parts[3].colorInfo[3].pattern,
					clothes.parts[3].emblemeId,
					clothes.parts[4].id,
					clothes.parts[4].colorInfo[0].pattern,
					clothes.parts[4].colorInfo[1].pattern,
					clothes.parts[4].colorInfo[2].pattern,
					clothes.parts[4].colorInfo[3].pattern,
					clothes.parts[4].emblemeId,
					clothes.parts[5].id,
					clothes.parts[5].colorInfo[0].pattern,
					clothes.parts[5].colorInfo[1].pattern,
					clothes.parts[5].colorInfo[2].pattern,
					clothes.parts[5].colorInfo[3].pattern,
					clothes.parts[5].emblemeId,
					clothes.parts[6].id,
					clothes.parts[6].colorInfo[0].pattern,
					clothes.parts[6].colorInfo[1].pattern,
					clothes.parts[6].colorInfo[2].pattern,
					clothes.parts[6].colorInfo[3].pattern,
					clothes.parts[6].emblemeId,
					clothes.parts[7].id,
					clothes.parts[7].colorInfo[0].pattern,
					clothes.parts[7].colorInfo[1].pattern,
					clothes.parts[7].colorInfo[2].pattern,
					clothes.parts[7].colorInfo[3].pattern,
					clothes.parts[7].emblemeId,
					clothes.parts[8].id,
					clothes.parts[8].colorInfo[0].pattern,
					clothes.parts[8].colorInfo[1].pattern,
					clothes.parts[8].colorInfo[2].pattern,
					clothes.parts[8].colorInfo[3].pattern,
					clothes.parts[8].emblemeId,
					hair.glossId
				};
				listCtrl.SelectItem(array[(int)type]);
			}
		}

		public void OnSelect(int index)
		{
			if (!InitEnd)
			{
				return;
			}
			CustomSelectInfo selectInfoFromIndex = listCtrl.GetSelectInfoFromIndex(index);
			if (selectInfoFromIndex != null)
			{
				switch (type)
				{
				case SelectKindType.FaceDetail:
					cvsFaceAll.UpdateSelectDetailKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Eyebrow:
					cvsEyebrow.UpdateSelectEyebrowKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.EyelineUp:
					cvsEye01.UpdateSelectEyelineUpKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.EyelineDown:
					cvsEye01.UpdateSelectEyelineDownKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.EyeWGrade:
					cvsEye02.UpdateSelectEyeWGradeKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.EyeHLUp:
					cvsEye02.UpdateSelectEyeHLUpKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.EyeHLDown:
					cvsEye02.UpdateSelectEyeHLDownKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Pupil:
					cvsEye02.UpdateSelectPupilKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.PupilGrade:
					cvsEye02.UpdateSelectPupilGradeKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Nose:
					cvsNose.UpdateSelectNoseKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Lipline:
					cvsMouth.UpdateSelectLiplineKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Mole:
					cvsMole.UpdateSelectMoleKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Eyeshadow:
					cvsMakeup.UpdateSelectEyeshadowKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Cheek:
					cvsMakeup.UpdateSelectCheekKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Lip:
					cvsMakeup.UpdateSelectLipKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.FacePaint01:
					cvsMakeup.UpdateSelectPaint01Kind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.FacePaint02:
					cvsMakeup.UpdateSelectPaint02Kind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.BodyDetail:
					cvsBodyAll.UpdateSelectDetailKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Nip:
					cvsBreast.UpdateSelectNipKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Underhair:
					cvsUnderhair.UpdateSelectUnderhairKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.Sunburn:
					cvsSunburn.UpdateSelectSunburnKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.BodyPaint01:
					cvsBodyPaint.UpdateSelectPaint01Kind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.BodyPaint02:
					cvsBodyPaint.UpdateSelectPaint02Kind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.BodyPaint01Layout:
					cvsBodyPaint.UpdateSelectPaint01Layout(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.BodyPaint02Layout:
					cvsBodyPaint.UpdateSelectPaint02Layout(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.HairBack:
					cvsHair.UpdateSelectHair(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.HairFront:
					cvsHair.UpdateSelectHair(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.HairSide:
					cvsHair.UpdateSelectHair(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.HairExtension:
					cvsHair.UpdateSelectHair(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosTop:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSailor01:
					cvsClothes.UpdateSelectSailor01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSailor02:
					cvsClothes.UpdateSelectSailor02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSailor03:
					cvsClothes.UpdateSelectSailor03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosJacket01:
					cvsClothes.UpdateSelectJacket01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosJacket02:
					cvsClothes.UpdateSelectJacket02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosJacket03:
					cvsClothes.UpdateSelectJacket03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosTopPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosTopPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosTopPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosTopPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosTopEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBot:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBotPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBotPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBotPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBotPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBotEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBra:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBraPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBraPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBraPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBraPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosBraEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosShorts:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosShortsPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosShortsPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosShortsPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosShortsPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosShortsEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosGloves:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosGlovesPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosGlovesPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosGlovesPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosGlovesPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosGlovesEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosPanst:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosPanstPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosPanstPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosPanstPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosPanstPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosPanstEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSocks:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSocksPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSocksPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSocksPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSocksPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosSocksEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosInnerShoes:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosInnerShoesPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosInnerShoesPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosInnerShoesPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosInnerShoesPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosInnerShoesEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosOuterShoes:
					cvsClothes.UpdateSelectClothes(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosOuterShoesPtn01:
					cvsClothes.UpdateSelectPtn01(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosOuterShoesPtn02:
					cvsClothes.UpdateSelectPtn02(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosOuterShoesPtn03:
					cvsClothes.UpdateSelectPtn03(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosOuterShoesPtn04:
					cvsClothes.UpdateSelectPtn04(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.CosOuterShoesEmblem:
					cvsClothes.UpdateSelectEmblem(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				case SelectKindType.HairGloss:
					cvsHairEtc.UpdateSelectGloss(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
					break;
				}
			}
		}
	}
}
