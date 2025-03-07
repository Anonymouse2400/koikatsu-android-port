using System;
using Illusion.Extensions;
using MessagePack;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsClothesCopy : MonoBehaviour
	{
		[SerializeField]
		private TMP_Dropdown[] ddCoordeType;

		[SerializeField]
		private Toggle[] tglKind;

		[SerializeField]
		private Toggle[] tglSubKind;

		[SerializeField]
		private GameObject[] objTglSub;

		[SerializeField]
		private TextMeshProUGUI[] textDst;

		[SerializeField]
		private TextMeshProUGUI[] textDstSub;

		[SerializeField]
		private TextMeshProUGUI[] textSrc;

		[SerializeField]
		private TextMeshProUGUI[] textSrcSub;

		[SerializeField]
		private Button btnAllOn;

		[SerializeField]
		private Button btnAllOff;

		[SerializeField]
		private Button btnCopy;

		[SerializeField]
		private GameObject objKindBra;

		private ChaListDefine.CategoryNo[] cateNo = new ChaListDefine.CategoryNo[9]
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

		private ChaListDefine.CategoryNo[] cateSailorNo = new ChaListDefine.CategoryNo[3]
		{
			ChaListDefine.CategoryNo.cpo_sailor_a,
			ChaListDefine.CategoryNo.cpo_sailor_b,
			ChaListDefine.CategoryNo.cpo_sailor_c
		};

		private ChaListDefine.CategoryNo[] cateJacketNo = new ChaListDefine.CategoryNo[3]
		{
			ChaListDefine.CategoryNo.cpo_jacket_a,
			ChaListDefine.CategoryNo.cpo_jacket_b,
			ChaListDefine.CategoryNo.cpo_jacket_c
		};

		private int[] defClothesID = new int[9];

		private int[] defClothesSailorID = new int[3] { 0, 0, 1 };

		private int[] defClothesJacketID = new int[3] { 0, 1, 1 };

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileClothes clothes
		{
			get
			{
				return chaCtrl.nowCoordinate.clothes;
			}
		}

		public void CalculateUI()
		{
			ChangeDstDD();
			ChangeSrcDD();
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		private void ChangeDstDD()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
			ChaFileClothes chaFileClothes = chaCtrl.chaFile.coordinate[ddCoordeType[0].value].clothes;
			for (int i = 0; i < num; i++)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], chaFileClothes.parts[i].id);
				if (listInfo == null)
				{
					listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], defClothesID[i]);
					textDst[i].text = listInfo.Name;
				}
				else
				{
					textDst[i].text = listInfo.Name;
				}
				if (i != 0)
				{
					continue;
				}
				for (int j = 0; j < 3; j++)
				{
					textDstSub[j].text = string.Empty;
				}
				if (listInfo.Kind == 1)
				{
					for (int k = 0; k < 3; k++)
					{
						listInfo = chaCtrl.lstCtrl.GetListInfo(cateSailorNo[k], chaFileClothes.subPartsId[k]);
						if (listInfo == null)
						{
							listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], defClothesSailorID[i]);
							textDstSub[i].text = listInfo.Name;
						}
						else
						{
							textDstSub[k].text = listInfo.Name;
						}
					}
				}
				else
				{
					if (listInfo.Kind != 2)
					{
						continue;
					}
					for (int l = 0; l < 3; l++)
					{
						listInfo = chaCtrl.lstCtrl.GetListInfo(cateJacketNo[l], chaFileClothes.subPartsId[l]);
						if (listInfo == null)
						{
							listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], defClothesJacketID[i]);
							textDstSub[i].text = listInfo.Name;
						}
						else
						{
							textDstSub[l].text = listInfo.Name;
						}
					}
				}
			}
			ChangeHideSubParts();
		}

		private void ChangeSrcDD()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
			ChaFileClothes chaFileClothes = chaCtrl.chaFile.coordinate[ddCoordeType[1].value].clothes;
			for (int i = 0; i < num; i++)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], chaFileClothes.parts[i].id);
				if (listInfo == null)
				{
					listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], defClothesID[i]);
					textSrc[i].text = listInfo.Name;
				}
				else
				{
					textSrc[i].text = listInfo.Name;
				}
				if (i != 0)
				{
					continue;
				}
				for (int j = 0; j < 3; j++)
				{
					textSrcSub[j].text = string.Empty;
				}
				if (listInfo.Kind == 1)
				{
					for (int k = 0; k < 3; k++)
					{
						listInfo = chaCtrl.lstCtrl.GetListInfo(cateSailorNo[k], chaFileClothes.subPartsId[k]);
						if (listInfo == null)
						{
							listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], defClothesSailorID[i]);
							textSrcSub[i].text = listInfo.Name;
						}
						else
						{
							textSrcSub[k].text = listInfo.Name;
						}
					}
				}
				else
				{
					if (listInfo.Kind != 2)
					{
						continue;
					}
					for (int l = 0; l < 3; l++)
					{
						listInfo = chaCtrl.lstCtrl.GetListInfo(cateJacketNo[l], chaFileClothes.subPartsId[l]);
						if (listInfo == null)
						{
							listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[i], defClothesSailorID[i]);
							textSrcSub[i].text = listInfo.Name;
						}
						else
						{
							textSrcSub[l].text = listInfo.Name;
						}
					}
				}
			}
			ChangeHideSubParts();
		}

		private void ChangeHideSubParts()
		{
			ChaFileClothes chaFileClothes = chaCtrl.chaFile.coordinate[ddCoordeType[0].value].clothes;
			ChaFileClothes chaFileClothes2 = chaCtrl.chaFile.coordinate[ddCoordeType[1].value].clothes;
			ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[0], chaFileClothes.parts[0].id);
			if (listInfo == null)
			{
				listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[0], defClothesID[0]);
			}
			ListInfoBase listInfo2 = chaCtrl.lstCtrl.GetListInfo(cateNo[0], chaFileClothes2.parts[0].id);
			if (listInfo2 == null)
			{
				listInfo2 = chaCtrl.lstCtrl.GetListInfo(cateNo[0], defClothesID[0]);
			}
			bool active = false;
			if (listInfo.Kind == 1 || listInfo.Kind == 2 || listInfo2.Kind == 1 || listInfo2.Kind == 2)
			{
				active = true;
			}
			for (int i = 0; i < 3; i++)
			{
				tglSubKind[i].transform.parent.gameObject.SetActiveIfDifferent(active);
			}
			bool flag = false;
			if ((listInfo2.Kind == 1 && listInfo.Kind == 1) || (listInfo2.Kind == 2 && listInfo.Kind == 2))
			{
				flag = true;
			}
			for (int j = 0; j < 3; j++)
			{
				tglSubKind[j].interactable = flag;
				objTglSub[j].SetActiveIfDifferent(flag);
			}
		}

		private void CopyClothes()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
			ChaFileClothes chaFileClothes = chaCtrl.chaFile.coordinate[ddCoordeType[0].value].clothes;
			ChaFileClothes chaFileClothes2 = chaCtrl.chaFile.coordinate[ddCoordeType[1].value].clothes;
			ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[0], chaFileClothes.parts[0].id);
			if (listInfo == null)
			{
				listInfo = chaCtrl.lstCtrl.GetListInfo(cateNo[0], defClothesID[0]);
			}
			ListInfoBase listInfo2 = chaCtrl.lstCtrl.GetListInfo(cateNo[0], chaFileClothes2.parts[0].id);
			if (listInfo2 == null)
			{
				listInfo2 = chaCtrl.lstCtrl.GetListInfo(cateNo[0], defClothesID[0]);
			}
			for (int i = 0; i < num; i++)
			{
				if (!tglKind[i].isOn)
				{
					continue;
				}
				byte[] bytes = MessagePackSerializer.Serialize(chaFileClothes2.parts[i]);
				chaFileClothes.parts[i] = MessagePackSerializer.Deserialize<ChaFileClothes.PartsInfo>(bytes);
				switch (i)
				{
				case 0:
					if ((listInfo2.Kind == 1 && listInfo.Kind == 1) || (listInfo2.Kind == 2 && listInfo.Kind == 2))
					{
						for (int j = 0; j < chaFileClothes.subPartsId.Length; j++)
						{
							chaFileClothes.subPartsId[j] = chaFileClothes2.subPartsId[j];
						}
					}
					else if (listInfo2.Kind == 1 || listInfo2.Kind == 2)
					{
						for (int k = 0; k < chaFileClothes.subPartsId.Length; k++)
						{
							chaFileClothes.subPartsId[k] = chaFileClothes2.subPartsId[k];
						}
					}
					else
					{
						for (int l = 0; l < chaFileClothes.subPartsId.Length; l++)
						{
							chaFileClothes.subPartsId[l] = 0;
						}
					}
					break;
				case 2:
					chaFileClothes.hideBraOpt[0] = chaFileClothes2.hideBraOpt[0];
					chaFileClothes.hideBraOpt[1] = chaFileClothes2.hideBraOpt[1];
					break;
				case 3:
					chaFileClothes.hideShortsOpt[0] = chaFileClothes2.hideShortsOpt[0];
					chaFileClothes.hideShortsOpt[1] = chaFileClothes2.hideShortsOpt[1];
					break;
				}
			}
			chaCtrl.ChangeCoordinateType();
			chaCtrl.Reload(false, true, true, true);
			CalculateUI();
			Singleton<CustomBase>.Instance.updateCustomUI = true;
			Singleton<CustomHistory>.Instance.Add5(chaCtrl, chaCtrl.Reload, false, true, true, true);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			if (Singleton<CustomBase>.Instance.chaCtrl.sex == 0 && (bool)objKindBra)
			{
				objKindBra.SetActiveIfDifferent(Singleton<CustomBase>.Instance.IsMaleCoordinateBra());
			}
			Singleton<CustomBase>.Instance.actUpdateCvsClothesCopy += UpdateCustomUI;
			ddCoordeType[0].onValueChanged.AddListener(delegate
			{
				ChangeDstDD();
			});
			ddCoordeType[1].onValueChanged.AddListener(delegate
			{
				ChangeSrcDD();
			});
			ChangeHideSubParts();
			btnAllOn.OnClickAsObservable().Subscribe(delegate
			{
				Toggle[] array = tglKind;
				foreach (Toggle toggle in array)
				{
					toggle.isOn = true;
				}
				Toggle[] array2 = tglSubKind;
				foreach (Toggle toggle2 in array2)
				{
					toggle2.isOn = true;
				}
			});
			btnAllOff.OnClickAsObservable().Subscribe(delegate
			{
				Toggle[] array3 = tglKind;
				foreach (Toggle toggle3 in array3)
				{
					toggle3.isOn = false;
				}
				Toggle[] array4 = tglSubKind;
				foreach (Toggle toggle4 in array4)
				{
					toggle4.isOn = false;
				}
			});
			btnCopy.OnClickAsObservable().Subscribe(delegate
			{
				CopyClothes();
			});
			Observable.EveryUpdate().Subscribe(delegate
			{
				btnCopy.interactable = ddCoordeType[0].value != ddCoordeType[1].value;
			}).AddTo(this);
		}
	}
}
