using System;
using System.Collections;
using System.Linq;
using Illusion.Extensions;
using Localize.Translate;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsClothes : MonoBehaviour
	{
		private class TextInitBtnNameSeter : InitializeSolution.IInitializable
		{
			private readonly ChaFileDefine.ClothesKind typeClothes;

			private readonly TMP_Text text;

			private readonly GameObject gameObject;

			public bool initialized { get; private set; }

			public TextInitBtnNameSeter(ChaFileDefine.ClothesKind typeClothes, TMP_Text text, GameObject gameObject)
			{
				this.typeClothes = typeClothes;
				this.text = text;
				this.gameObject = gameObject;
			}

			public void Initialize()
			{
				if (!initialized)
				{
					initialized = true;
					Localize.Translate.Manager.Bind(text, gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CUSTOM_UI).Get(998).Get((int)typeClothes), true);
				}
			}
		}

		public ChaFileDefine.ClothesKind typeClothes;

		[SerializeField]
		private Button btnInitSetting;

		[SerializeField]
		private TextMeshProUGUI textInitBtnName;

		[SerializeField]
		private Toggle tglClothesKind;

		[SerializeField]
		private Image imgClothesKind;

		[SerializeField]
		private TextMeshProUGUI textClothesKind;

		[SerializeField]
		private CustomSelectKind customClothes;

		[SerializeField]
		private CanvasGroup cgClothesWin;

		[SerializeField]
		private Toggle tglSailor01Kind;

		[SerializeField]
		private Image imgSailor01Kind;

		[SerializeField]
		private TextMeshProUGUI textSailor01Kind;

		[SerializeField]
		private CustomSelectKind customSailor01;

		[SerializeField]
		private CanvasGroup cgSailor01Win;

		[SerializeField]
		private Toggle tglSailor02Kind;

		[SerializeField]
		private Image imgSailor02Kind;

		[SerializeField]
		private TextMeshProUGUI textSailor02Kind;

		[SerializeField]
		private CustomSelectKind customSailor02;

		[SerializeField]
		private CanvasGroup cgSailor02Win;

		[SerializeField]
		private Toggle tglSailor03Kind;

		[SerializeField]
		private Image imgSailor03Kind;

		[SerializeField]
		private TextMeshProUGUI textSailor03Kind;

		[SerializeField]
		private CustomSelectKind customSailor03;

		[SerializeField]
		private CanvasGroup cgSailor03Win;

		[SerializeField]
		private Toggle tglJacket01Kind;

		[SerializeField]
		private Image imgJacket01Kind;

		[SerializeField]
		private TextMeshProUGUI textJacket01Kind;

		[SerializeField]
		private CustomSelectKind customJacket01;

		[SerializeField]
		private CanvasGroup cgJacket01Win;

		[SerializeField]
		private Toggle tglJacket02Kind;

		[SerializeField]
		private Image imgJacket02Kind;

		[SerializeField]
		private TextMeshProUGUI textJacket02Kind;

		[SerializeField]
		private CustomSelectKind customJacket02;

		[SerializeField]
		private CanvasGroup cgJacket02Win;

		[SerializeField]
		private Toggle tglJacket03Kind;

		[SerializeField]
		private Image imgJacket03Kind;

		[SerializeField]
		private TextMeshProUGUI textJacket03Kind;

		[SerializeField]
		private CustomSelectKind customJacket03;

		[SerializeField]
		private CanvasGroup cgJacket03Win;

		[SerializeField]
		private GameObject objPtn01Separate;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnMainColor01;

		[SerializeField]
		private Image imgMainColor01;

		[SerializeField]
		private Toggle tglPtn01Kind;

		[SerializeField]
		private Image imgPtn01Kind;

		[SerializeField]
		private TextMeshProUGUI textPtn01Kind;

		[SerializeField]
		private CustomSelectKind customPtn01;

		[SerializeField]
		private CanvasGroup cgPtn01Win;

		[SerializeField]
		private Slider sldPtn01Width;

		[SerializeField]
		private TMP_InputField inpPtn01Width;

		[SerializeField]
		private Button btnPtn01Width;

		[SerializeField]
		private Slider sldPtn01Height;

		[SerializeField]
		private TMP_InputField inpPtn01Height;

		[SerializeField]
		private Button btnPtn01Height;

		[SerializeField]
		private Button btnPtnColor01;

		[SerializeField]
		private Image imgPtnColor01;

		[SerializeField]
		private GameObject objPtn02Separate;

		[SerializeField]
		private Button btnMainColor02;

		[SerializeField]
		private Image imgMainColor02;

		[SerializeField]
		private Toggle tglPtn02Kind;

		[SerializeField]
		private Image imgPtn02Kind;

		[SerializeField]
		private TextMeshProUGUI textPtn02Kind;

		[SerializeField]
		private CustomSelectKind customPtn02;

		[SerializeField]
		private CanvasGroup cgPtn02Win;

		[SerializeField]
		private Slider sldPtn02Width;

		[SerializeField]
		private TMP_InputField inpPtn02Width;

		[SerializeField]
		private Button btnPtn02Width;

		[SerializeField]
		private Slider sldPtn02Height;

		[SerializeField]
		private TMP_InputField inpPtn02Height;

		[SerializeField]
		private Button btnPtn02Height;

		[SerializeField]
		private Button btnPtnColor02;

		[SerializeField]
		private Image imgPtnColor02;

		[SerializeField]
		private GameObject objPtn03Separate;

		[SerializeField]
		private Button btnMainColor03;

		[SerializeField]
		private Image imgMainColor03;

		[SerializeField]
		private Toggle tglPtn03Kind;

		[SerializeField]
		private Image imgPtn03Kind;

		[SerializeField]
		private TextMeshProUGUI textPtn03Kind;

		[SerializeField]
		private CustomSelectKind customPtn03;

		[SerializeField]
		private CanvasGroup cgPtn03Win;

		[SerializeField]
		private Slider sldPtn03Width;

		[SerializeField]
		private TMP_InputField inpPtn03Width;

		[SerializeField]
		private Button btnPtn03Width;

		[SerializeField]
		private Slider sldPtn03Height;

		[SerializeField]
		private TMP_InputField inpPtn03Height;

		[SerializeField]
		private Button btnPtn03Height;

		[SerializeField]
		private Button btnPtnColor03;

		[SerializeField]
		private Image imgPtnColor03;

		[SerializeField]
		private GameObject objPtn04Separate;

		[SerializeField]
		private Button btnMainColor04;

		[SerializeField]
		private Image imgMainColor04;

		[SerializeField]
		private Toggle tglPtn04Kind;

		[SerializeField]
		private Image imgPtn04Kind;

		[SerializeField]
		private TextMeshProUGUI textPtn04Kind;

		[SerializeField]
		private CustomSelectKind customPtn04;

		[SerializeField]
		private CanvasGroup cgPtn04Win;

		[SerializeField]
		private Slider sldPtn04Width;

		[SerializeField]
		private TMP_InputField inpPtn04Width;

		[SerializeField]
		private Button btnPtn04Width;

		[SerializeField]
		private Slider sldPtn04Height;

		[SerializeField]
		private TMP_InputField inpPtn04Height;

		[SerializeField]
		private Button btnPtn04Height;

		[SerializeField]
		private Button btnPtnColor04;

		[SerializeField]
		private Image imgPtnColor04;

		[SerializeField]
		private GameObject objEmblemSeparate;

		[SerializeField]
		private Toggle tglEmblemKind;

		[SerializeField]
		private Image imgEmblemKind;

		[SerializeField]
		private TextMeshProUGUI textEmblemKind;

		[SerializeField]
		private CustomSelectKind customEmblem;

		[SerializeField]
		private CanvasGroup cgEmblemWin;

		[SerializeField]
		private GameObject objOption;

		[SerializeField]
		private Toggle[] tglOption;

		private int clothesType
		{
			get
			{
				return (int)typeClothes;
			}
		}

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

		private ChaFileClothes setClothes
		{
			get
			{
				return chaCtrl.chaFile.coordinate[chaCtrl.chaFile.status.coordinateType].clothes;
			}
		}

		private ChaFileClothes defClothes
		{
			get
			{
				return Singleton<CustomBase>.Instance.defChaInfo.coordinate[chaCtrl.chaFile.status.coordinateType].clothes;
			}
		}

		public void CalculateUI()
		{
			if ((bool)sldPtn01Width)
			{
				sldPtn01Width.value = clothes.parts[clothesType].colorInfo[0].tiling.x;
			}
			if ((bool)sldPtn01Height)
			{
				sldPtn01Height.value = clothes.parts[clothesType].colorInfo[0].tiling.y;
			}
			if ((bool)sldPtn02Width)
			{
				sldPtn02Width.value = clothes.parts[clothesType].colorInfo[1].tiling.x;
			}
			if ((bool)sldPtn02Height)
			{
				sldPtn02Height.value = clothes.parts[clothesType].colorInfo[1].tiling.y;
			}
			if ((bool)sldPtn03Width)
			{
				sldPtn03Width.value = clothes.parts[clothesType].colorInfo[2].tiling.x;
			}
			if ((bool)sldPtn03Height)
			{
				sldPtn03Height.value = clothes.parts[clothesType].colorInfo[2].tiling.y;
			}
			if ((bool)sldPtn04Width)
			{
				sldPtn04Width.value = clothes.parts[clothesType].colorInfo[3].tiling.x;
			}
			if ((bool)sldPtn04Height)
			{
				sldPtn04Height.value = clothes.parts[clothesType].colorInfo[3].tiling.y;
			}
			imgMainColor01.color = clothes.parts[clothesType].colorInfo[0].baseColor;
			imgMainColor02.color = clothes.parts[clothesType].colorInfo[1].baseColor;
			imgMainColor03.color = clothes.parts[clothesType].colorInfo[2].baseColor;
			if ((bool)imgMainColor04)
			{
				imgMainColor04.color = clothes.parts[clothesType].colorInfo[3].baseColor;
			}
			imgPtnColor01.color = clothes.parts[clothesType].colorInfo[0].patternColor;
			imgPtnColor02.color = clothes.parts[clothesType].colorInfo[1].patternColor;
			imgPtnColor03.color = clothes.parts[clothesType].colorInfo[2].patternColor;
			if ((bool)imgPtnColor04)
			{
				imgPtnColor04.color = clothes.parts[clothesType].colorInfo[3].patternColor;
			}
			if (clothesType == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					tglOption[i].isOn = !clothes.hideBraOpt[i];
				}
			}
			else if (clothesType == 3)
			{
				for (int j = 0; j < 2; j++)
				{
					tglOption[j].isOn = !clothes.hideShortsOpt[j];
				}
			}
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customClothes)
			{
				customClothes.UpdateCustomUI();
			}
			if (null != customSailor01)
			{
				customSailor01.UpdateCustomUI();
			}
			if (null != customSailor02)
			{
				customSailor02.UpdateCustomUI();
			}
			if (null != customSailor03)
			{
				customSailor03.UpdateCustomUI();
			}
			if (null != customJacket01)
			{
				customJacket01.UpdateCustomUI();
			}
			if (null != customJacket02)
			{
				customJacket02.UpdateCustomUI();
			}
			if (null != customJacket03)
			{
				customJacket03.UpdateCustomUI();
			}
			if (null != customPtn01)
			{
				customPtn01.UpdateCustomUI();
			}
			if (null != customPtn02)
			{
				customPtn02.UpdateCustomUI();
			}
			if (null != customPtn03)
			{
				customPtn03.UpdateCustomUI();
			}
			if (null != customPtn04)
			{
				customPtn04.UpdateCustomUI();
			}
			if (null != customEmblem)
			{
				customEmblem.UpdateCustomUI();
			}
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTop01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTop02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTop03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTop04))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[3].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTopPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTopPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTopPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosTopPtn04))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[3].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.bot:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBot01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBot02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBot03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBotPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBotPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBotPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.bra:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBra01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBra02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBra03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBraPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBraPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosBraPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.shorts:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosShorts01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosShorts02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosShorts03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosShortsPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosShortsPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosShortsPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.gloves:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosGloves01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosGloves02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosGloves03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosGlovesPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosGlovesPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosGlovesPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.panst:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosPanst01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosPanst02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosPanst03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosPanstPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosPanstPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosPanstPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.socks:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosSocks01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosSocks02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosSocks03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosSocksPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosSocksPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosSocksPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosInnerShoes01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosInnerShoes02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosInnerShoes03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosInnerShoesPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosInnerShoesPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosInnerShoesPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosOuterShoes01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosOuterShoes02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosOuterShoes03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosOuterShoesPtn01))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[0].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosOuterShoesPtn02))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[1].patternColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.CosOuterShoesPtn03))
				{
					cvsColor.SetColor(clothes.parts[clothesType].colorInfo[2].patternColor);
				}
				break;
			}
			ChangeSubPartsVisible();
			ChangeUseColorVisible();
			ChangeEmblemVisible();
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			if ((bool)inpPtn01Width)
			{
				inpPtn01Width.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[0].tiling.x);
			}
			if ((bool)inpPtn01Height)
			{
				inpPtn01Height.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[0].tiling.y);
			}
			if ((bool)inpPtn02Width)
			{
				inpPtn02Width.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[1].tiling.x);
			}
			if ((bool)inpPtn02Height)
			{
				inpPtn02Height.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[1].tiling.y);
			}
			if ((bool)inpPtn03Width)
			{
				inpPtn03Width.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[2].tiling.x);
			}
			if ((bool)inpPtn03Height)
			{
				inpPtn03Height.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[2].tiling.y);
			}
			if ((bool)inpPtn04Width)
			{
				inpPtn04Width.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[3].tiling.x);
			}
			if ((bool)inpPtn04Height)
			{
				inpPtn04Height.text = CustomBase.ConvertTextFromRate(0, 100, clothes.parts[clothesType].colorInfo[3].tiling.y);
			}
		}

		public ChaListDefine.CategoryNo GetCategooryNo()
		{
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				return ChaListDefine.CategoryNo.co_top;
			case ChaFileDefine.ClothesKind.bot:
				return ChaListDefine.CategoryNo.co_bot;
			case ChaFileDefine.ClothesKind.bra:
				return ChaListDefine.CategoryNo.co_bra;
			case ChaFileDefine.ClothesKind.shorts:
				return ChaListDefine.CategoryNo.co_shorts;
			case ChaFileDefine.ClothesKind.gloves:
				return ChaListDefine.CategoryNo.co_gloves;
			case ChaFileDefine.ClothesKind.panst:
				return ChaListDefine.CategoryNo.co_panst;
			case ChaFileDefine.ClothesKind.socks:
				return ChaListDefine.CategoryNo.co_socks;
			case ChaFileDefine.ClothesKind.shoes_inner:
				return ChaListDefine.CategoryNo.co_shoes;
			case ChaFileDefine.ClothesKind.shoes_outer:
				return ChaListDefine.CategoryNo.co_shoes;
			default:
				return ChaListDefine.CategoryNo.cha_sample_f;
			}
		}

		private bool CheckUseColor(int _type, ref bool[] use)
		{
			bool flag = true;
			if (_type == 0)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[_type].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				ChaClothesComponent chaClothesComponent = chaCtrl.cusClothesCmp[_type];
				if (null != chaClothesComponent)
				{
					use[0] = chaClothesComponent.useColorN01;
					use[1] = chaClothesComponent.useColorN02;
					use[2] = chaClothesComponent.useColorN03;
				}
				if (_type == 2 || _type == 3)
				{
					bool flag2 = !(null == chaClothesComponent) && chaClothesComponent.useOpt01;
					bool flag3 = !(null == chaClothesComponent) && chaClothesComponent.useOpt02;
					tglOption[0].gameObject.SetActiveIfDifferent(flag2);
					tglOption[1].gameObject.SetActiveIfDifferent(flag3);
					bool active = flag2 || flag3;
					objOption.SetActiveIfDifferent(active);
				}
			}
			else
			{
				ChaClothesComponent[] cusClothesSubCmp = chaCtrl.cusClothesSubCmp;
				foreach (ChaClothesComponent chaClothesComponent2 in cusClothesSubCmp)
				{
					if (!(null == chaClothesComponent2))
					{
						use[0] |= chaClothesComponent2.useColorN01;
						use[1] |= chaClothesComponent2.useColorN02;
						use[2] |= chaClothesComponent2.useColorN03;
						use[3] |= null != chaClothesComponent2.rendAccessory;
					}
				}
			}
			return true;
		}

		public void ChangeUseColorVisible()
		{
			bool[] use = new bool[4];
			if (!CheckUseColor(clothesType, ref use))
			{
				return;
			}
			if ((bool)objPtn01Separate)
			{
				objPtn01Separate.SetActiveIfDifferent(use[0]);
			}
			if ((bool)btnMainColor01)
			{
				btnMainColor01.transform.parent.gameObject.SetActiveIfDifferent(use[0]);
			}
			if ((bool)tglPtn01Kind)
			{
				tglPtn01Kind.transform.parent.gameObject.SetActiveIfDifferent(use[0]);
			}
			if ((bool)sldPtn01Width)
			{
				sldPtn01Width.transform.parent.gameObject.SetActiveIfDifferent(use[0]);
			}
			if ((bool)sldPtn01Height)
			{
				sldPtn01Height.transform.parent.gameObject.SetActiveIfDifferent(use[0]);
			}
			if ((bool)btnPtnColor01)
			{
				btnPtnColor01.transform.parent.gameObject.SetActiveIfDifferent(use[0]);
			}
			if ((bool)objPtn02Separate)
			{
				objPtn02Separate.SetActiveIfDifferent(use[1]);
			}
			if ((bool)btnMainColor02)
			{
				btnMainColor02.transform.parent.gameObject.SetActiveIfDifferent(use[1]);
			}
			if ((bool)tglPtn02Kind)
			{
				tglPtn02Kind.transform.parent.gameObject.SetActiveIfDifferent(use[1]);
			}
			if ((bool)sldPtn02Width)
			{
				sldPtn02Width.transform.parent.gameObject.SetActiveIfDifferent(use[1]);
			}
			if ((bool)sldPtn02Height)
			{
				sldPtn02Height.transform.parent.gameObject.SetActiveIfDifferent(use[1]);
			}
			if ((bool)btnPtnColor02)
			{
				btnPtnColor02.transform.parent.gameObject.SetActiveIfDifferent(use[1]);
			}
			if ((bool)objPtn03Separate)
			{
				objPtn03Separate.SetActiveIfDifferent(use[2]);
			}
			if ((bool)btnMainColor03)
			{
				btnMainColor03.transform.parent.gameObject.SetActiveIfDifferent(use[2]);
			}
			if ((bool)tglPtn03Kind)
			{
				tglPtn03Kind.transform.parent.gameObject.SetActiveIfDifferent(use[2]);
			}
			if ((bool)sldPtn03Width)
			{
				sldPtn03Width.transform.parent.gameObject.SetActiveIfDifferent(use[2]);
			}
			if ((bool)sldPtn03Height)
			{
				sldPtn03Height.transform.parent.gameObject.SetActiveIfDifferent(use[2]);
			}
			if ((bool)btnPtnColor03)
			{
				btnPtnColor03.transform.parent.gameObject.SetActiveIfDifferent(use[2]);
			}
			if ((bool)objPtn04Separate)
			{
				objPtn04Separate.SetActiveIfDifferent(use[3]);
			}
			if ((bool)btnMainColor04)
			{
				btnMainColor04.transform.parent.gameObject.SetActiveIfDifferent(use[3]);
			}
			if ((bool)tglPtn04Kind)
			{
				tglPtn04Kind.transform.parent.gameObject.SetActiveIfDifferent(use[3]);
			}
			if ((bool)sldPtn04Width)
			{
				sldPtn04Width.transform.parent.gameObject.SetActiveIfDifferent(use[3]);
			}
			if ((bool)sldPtn04Height)
			{
				sldPtn04Height.transform.parent.gameObject.SetActiveIfDifferent(use[3]);
			}
			if ((bool)btnPtnColor04)
			{
				btnPtnColor04.transform.parent.gameObject.SetActiveIfDifferent(use[3]);
			}
			bool flag = true;
			for (int i = 0; i < use.Length; i++)
			{
				if (use[i])
				{
					flag = false;
				}
			}
			if (flag)
			{
				cvsColor.Close();
			}
		}

		public void ChangeEmblemVisible()
		{
			bool active = chaCtrl.IsEmblem(clothesType);
			if ((bool)objEmblemSeparate)
			{
				objEmblemSeparate.SetActiveIfDifferent(active);
			}
			if ((bool)tglEmblemKind)
			{
				tglEmblemKind.transform.parent.gameObject.SetActiveIfDifferent(active);
			}
		}

		public void ChangeSubPartsVisible()
		{
			if (typeClothes != 0)
			{
				return;
			}
			ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
			if (listInfo != null)
			{
				if (listInfo.Kind == 1)
				{
					tglSailor01Kind.transform.parent.gameObject.SetActiveIfDifferent(true);
					tglSailor02Kind.transform.parent.gameObject.SetActiveIfDifferent(true);
					tglSailor03Kind.transform.parent.gameObject.SetActiveIfDifferent(true);
					tglJacket01Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglJacket02Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglJacket03Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				else if (listInfo.Kind == 2)
				{
					tglSailor01Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglSailor02Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglSailor03Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglJacket01Kind.transform.parent.gameObject.SetActiveIfDifferent(true);
					tglJacket02Kind.transform.parent.gameObject.SetActiveIfDifferent(true);
					tglJacket03Kind.transform.parent.gameObject.SetActiveIfDifferent(true);
				}
				else
				{
					tglSailor01Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglSailor02Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglSailor03Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglJacket01Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglJacket02Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
					tglJacket03Kind.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
			}
		}

		public void UpdateSelectClothes(string name, Sprite sp, int index)
		{
			if ((bool)textClothesKind)
			{
				textClothesKind.text = name;
			}
			if ((bool)imgClothesKind)
			{
				imgClothesKind.sprite = sp;
			}
			if (clothes.parts[clothesType].id == index)
			{
				return;
			}
			clothes.parts[clothesType].id = index;
			setClothes.parts[clothesType].id = index;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
			{
				FuncUpdateClothesTop();
				int num = Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length;
				for (int i = 0; i < num; i++)
				{
					setClothes.subPartsId[i] = clothes.subPartsId[i];
				}
				switch (chaCtrl.infoClothes[0].Kind)
				{
				case 1:
					customSailor01.UpdateCustomUI();
					customSailor02.UpdateCustomUI();
					customSailor03.UpdateCustomUI();
					break;
				case 2:
					customJacket01.UpdateCustomUI();
					customJacket02.UpdateCustomUI();
					customJacket03.UpdateCustomUI();
					break;
				}
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesTop);
				Singleton<CustomBase>.Instance.updateCvsCosBot = true;
				break;
			}
			case ChaFileDefine.ClothesKind.bot:
				FuncUpdateClothesBot();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesBot);
				break;
			case ChaFileDefine.ClothesKind.bra:
				FuncUpdateClothesBra();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesBra);
				Singleton<CustomBase>.Instance.updateCvsCosShorts = true;
				break;
			case ChaFileDefine.ClothesKind.shorts:
				FuncUpdateClothesShorts();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesShorts);
				break;
			case ChaFileDefine.ClothesKind.gloves:
				FuncUpdateClothesGloves();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesGloves);
				break;
			case ChaFileDefine.ClothesKind.panst:
				FuncUpdateClothesPanst();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesPanst);
				break;
			case ChaFileDefine.ClothesKind.socks:
				FuncUpdateClothesSocks();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesSocks);
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				FuncUpdateClothesInnerShoes();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesInnerShoes);
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				FuncUpdateClothesOuterShoes();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateClothesOuterShoes);
				break;
			}
			ChangeSubPartsVisible();
			ChangeUseColorVisible();
			ChangeEmblemVisible();
		}

		public bool FuncUpdateClothesTop()
		{
			int id = setClothes.parts[0].id;
			int[] array = new int[3]
			{
				setClothes.subPartsId[0],
				setClothes.subPartsId[1],
				setClothes.subPartsId[2]
			};
			chaCtrl.ChangeClothesTop(id, array[0], array[1], array[2], true);
			return true;
		}

		public bool FuncUpdateClothesBot()
		{
			int id = setClothes.parts[1].id;
			chaCtrl.ChangeClothesBot(id, true);
			return true;
		}

		public bool FuncUpdateClothesBra()
		{
			int id = setClothes.parts[2].id;
			chaCtrl.ChangeClothesBra(id, true);
			return true;
		}

		public bool FuncUpdateClothesShorts()
		{
			int id = setClothes.parts[3].id;
			chaCtrl.ChangeClothesShorts(id, true);
			return true;
		}

		public bool FuncUpdateClothesGloves()
		{
			int id = setClothes.parts[4].id;
			chaCtrl.ChangeClothesGloves(id, true);
			return true;
		}

		public bool FuncUpdateClothesPanst()
		{
			int id = setClothes.parts[5].id;
			chaCtrl.ChangeClothesPanst(id, true);
			return true;
		}

		public bool FuncUpdateClothesSocks()
		{
			int id = setClothes.parts[6].id;
			chaCtrl.ChangeClothesSocks(id, true);
			return true;
		}

		public bool FuncUpdateClothesInnerShoes()
		{
			int id = setClothes.parts[7].id;
			chaCtrl.ChangeClothesShoes(0, id, true);
			return true;
		}

		public bool FuncUpdateClothesOuterShoes()
		{
			int id = setClothes.parts[8].id;
			chaCtrl.ChangeClothesShoes(1, id, true);
			return true;
		}

		public void UpdateSelectSailor01(string name, Sprite sp, int index)
		{
			if ((bool)textSailor01Kind)
			{
				textSailor01Kind.text = name;
			}
			if ((bool)imgSailor01Kind)
			{
				imgSailor01Kind.sprite = sp;
			}
			if (clothes.subPartsId[0] != index)
			{
				clothes.subPartsId[0] = index;
				setClothes.subPartsId[0] = index;
				FuncUpdateSailor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSailor);
				ChangeUseColorVisible();
				ChangeEmblemVisible();
			}
		}

		public void UpdateSelectSailor02(string name, Sprite sp, int index)
		{
			if ((bool)textSailor02Kind)
			{
				textSailor02Kind.text = name;
			}
			if ((bool)imgSailor02Kind)
			{
				imgSailor02Kind.sprite = sp;
			}
			if (clothes.subPartsId[1] != index)
			{
				clothes.subPartsId[1] = index;
				setClothes.subPartsId[1] = index;
				FuncUpdateSailor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSailor);
				ChangeUseColorVisible();
				ChangeEmblemVisible();
			}
		}

		public void UpdateSelectSailor03(string name, Sprite sp, int index)
		{
			if ((bool)textSailor03Kind)
			{
				textSailor03Kind.text = name;
			}
			if ((bool)imgSailor03Kind)
			{
				imgSailor03Kind.sprite = sp;
			}
			if (clothes.subPartsId[2] != index)
			{
				clothes.subPartsId[2] = index;
				setClothes.subPartsId[2] = index;
				FuncUpdateSailor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSailor);
				ChangeUseColorVisible();
				ChangeEmblemVisible();
			}
		}

		public bool FuncUpdateSailor()
		{
			int[] array = new int[3]
			{
				setClothes.subPartsId[0],
				setClothes.subPartsId[1],
				setClothes.subPartsId[2]
			};
			chaCtrl.ChangeClothesTop(1, array[0], array[1], array[2], true);
			return true;
		}

		public void UpdateSelectJacket01(string name, Sprite sp, int index)
		{
			if ((bool)textJacket01Kind)
			{
				textJacket01Kind.text = name;
			}
			if ((bool)imgJacket01Kind)
			{
				imgJacket01Kind.sprite = sp;
			}
			if (clothes.subPartsId[0] != index)
			{
				clothes.subPartsId[0] = index;
				setClothes.subPartsId[0] = index;
				FuncUpdateJacket();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateJacket);
				ChangeUseColorVisible();
				ChangeEmblemVisible();
			}
		}

		public void UpdateSelectJacket02(string name, Sprite sp, int index)
		{
			if ((bool)textJacket02Kind)
			{
				textJacket02Kind.text = name;
			}
			if ((bool)imgJacket02Kind)
			{
				imgJacket02Kind.sprite = sp;
			}
			if (clothes.subPartsId[1] != index)
			{
				clothes.subPartsId[1] = index;
				setClothes.subPartsId[1] = index;
				FuncUpdateJacket();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateJacket);
				ChangeUseColorVisible();
				ChangeEmblemVisible();
			}
		}

		public void UpdateSelectJacket03(string name, Sprite sp, int index)
		{
			if ((bool)textJacket03Kind)
			{
				textJacket03Kind.text = name;
			}
			if ((bool)imgJacket03Kind)
			{
				imgJacket03Kind.sprite = sp;
			}
			if (clothes.subPartsId[2] != index)
			{
				clothes.subPartsId[2] = index;
				setClothes.subPartsId[2] = index;
				FuncUpdateJacket();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateJacket);
				ChangeUseColorVisible();
				ChangeEmblemVisible();
			}
		}

		public bool FuncUpdateJacket()
		{
			int[] array = new int[3]
			{
				setClothes.subPartsId[0],
				setClothes.subPartsId[1],
				setClothes.subPartsId[2]
			};
			chaCtrl.ChangeClothesTop(2, array[0], array[1], array[2], true);
			return true;
		}

		public void UpdateSelectPtn01(string name, Sprite sp, int index)
		{
			if ((bool)textPtn01Kind)
			{
				textPtn01Kind.text = name;
			}
			if ((bool)imgPtn01Kind)
			{
				imgPtn01Kind.sprite = sp;
			}
			if (clothes.parts[clothesType].colorInfo[0].pattern != index)
			{
				clothes.parts[clothesType].colorInfo[0].pattern = index;
				setClothes.parts[clothesType].colorInfo[0].pattern = index;
				FuncUpdatePattern01();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePattern01);
			}
		}

		public bool FuncUpdatePattern01()
		{
			bool flag = false;
			if (typeClothes == ChaFileDefine.ClothesKind.top)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				chaCtrl.ChangeCustomClothes(false, 0, false, true, false, false, false);
				chaCtrl.ChangeCustomClothes(false, 1, false, true, false, false, false);
			}
			else
			{
				chaCtrl.ChangeCustomClothes(true, clothesType, false, true, false, false, false);
			}
			return true;
		}

		public void UpdateSelectPtn02(string name, Sprite sp, int index)
		{
			if ((bool)textPtn02Kind)
			{
				textPtn02Kind.text = name;
			}
			if ((bool)imgPtn02Kind)
			{
				imgPtn02Kind.sprite = sp;
			}
			if (clothes.parts[clothesType].colorInfo[1].pattern != index)
			{
				clothes.parts[clothesType].colorInfo[1].pattern = index;
				setClothes.parts[clothesType].colorInfo[1].pattern = index;
				FuncUpdatePattern02();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePattern02);
			}
		}

		public bool FuncUpdatePattern02()
		{
			bool flag = false;
			if (typeClothes == ChaFileDefine.ClothesKind.top)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				chaCtrl.ChangeCustomClothes(false, 0, false, false, true, false, false);
				chaCtrl.ChangeCustomClothes(false, 1, false, false, true, false, false);
			}
			else
			{
				chaCtrl.ChangeCustomClothes(true, clothesType, false, false, true, false, false);
			}
			return true;
		}

		public void UpdateSelectPtn03(string name, Sprite sp, int index)
		{
			if ((bool)textPtn03Kind)
			{
				textPtn03Kind.text = name;
			}
			if ((bool)imgPtn03Kind)
			{
				imgPtn03Kind.sprite = sp;
			}
			if (clothes.parts[clothesType].colorInfo[2].pattern != index)
			{
				clothes.parts[clothesType].colorInfo[2].pattern = index;
				setClothes.parts[clothesType].colorInfo[2].pattern = index;
				FuncUpdatePattern03();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePattern03);
			}
		}

		public bool FuncUpdatePattern03()
		{
			bool flag = false;
			if (typeClothes == ChaFileDefine.ClothesKind.top)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				chaCtrl.ChangeCustomClothes(false, 0, false, false, false, true, false);
				chaCtrl.ChangeCustomClothes(false, 1, false, false, false, true, false);
			}
			else
			{
				chaCtrl.ChangeCustomClothes(true, clothesType, false, false, false, true, false);
			}
			return true;
		}

		public void UpdateSelectPtn04(string name, Sprite sp, int index)
		{
			if ((bool)textPtn04Kind)
			{
				textPtn04Kind.text = name;
			}
			if ((bool)imgPtn04Kind)
			{
				imgPtn04Kind.sprite = sp;
			}
			if (clothes.parts[clothesType].colorInfo[3].pattern != index)
			{
				clothes.parts[clothesType].colorInfo[3].pattern = index;
				setClothes.parts[clothesType].colorInfo[3].pattern = index;
				FuncUpdatePattern04();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePattern04);
			}
		}

		public bool FuncUpdatePattern04()
		{
			bool flag = false;
			if (typeClothes == ChaFileDefine.ClothesKind.top)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				chaCtrl.ChangeCustomClothes(false, 2, false, false, false, false, true);
			}
			else
			{
				chaCtrl.ChangeCustomClothes(true, clothesType, false, false, false, false, true);
			}
			return true;
		}

		public void UpdateSelectEmblem(string name, Sprite sp, int index)
		{
			if ((bool)textEmblemKind)
			{
				textEmblemKind.text = name;
			}
			if ((bool)imgEmblemKind)
			{
				imgEmblemKind.sprite = sp;
			}
			if (clothes.parts[clothesType].emblemeId != index)
			{
				clothes.parts[clothesType].emblemeId = index;
				setClothes.parts[clothesType].emblemeId = index;
				FuncUpdateEmblem();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateEmblem);
			}
		}

		public bool FuncUpdateEmblem()
		{
			chaCtrl.ChangeCustomEmblem(clothesType);
			return true;
		}

		public void UpdateCosColor01(Color color)
		{
			clothes.parts[clothesType].colorInfo[0].baseColor = color;
			setClothes.parts[clothesType].colorInfo[0].baseColor = color;
			imgMainColor01.color = color;
			FuncUpdateCosColor();
		}

		public bool FuncUpdateCosColor()
		{
			bool flag = false;
			if (typeClothes == ChaFileDefine.ClothesKind.top)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				for (int i = 0; i < clothes.subPartsId.Length; i++)
				{
					chaCtrl.ChangeCustomClothes(false, i, true, false, false, false, false);
				}
			}
			else
			{
				chaCtrl.ChangeCustomClothes(true, clothesType, true, false, false, false, false);
			}
			return true;
		}

		public void UpdateCosColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
		}

		public void UpdateCosColor02(Color color)
		{
			clothes.parts[clothesType].colorInfo[1].baseColor = color;
			setClothes.parts[clothesType].colorInfo[1].baseColor = color;
			imgMainColor02.color = color;
			FuncUpdateCosColor();
		}

		public void UpdateCosColor03(Color color)
		{
			clothes.parts[clothesType].colorInfo[2].baseColor = color;
			setClothes.parts[clothesType].colorInfo[2].baseColor = color;
			imgMainColor03.color = color;
			FuncUpdateCosColor();
		}

		public void UpdateCosColor04(Color color)
		{
			clothes.parts[clothesType].colorInfo[3].baseColor = color;
			setClothes.parts[clothesType].colorInfo[3].baseColor = color;
			imgMainColor04.color = color;
			FuncUpdateCosColor();
		}

		public void UpdatePtnColor01(Color color)
		{
			clothes.parts[clothesType].colorInfo[0].patternColor = color;
			setClothes.parts[clothesType].colorInfo[0].patternColor = color;
			imgPtnColor01.color = color;
			FuncUpdateCosColor();
		}

		public void UpdatePtnColor02(Color color)
		{
			clothes.parts[clothesType].colorInfo[1].patternColor = color;
			setClothes.parts[clothesType].colorInfo[1].patternColor = color;
			imgPtnColor02.color = color;
			FuncUpdateCosColor();
		}

		public void UpdatePtnColor03(Color color)
		{
			clothes.parts[clothesType].colorInfo[2].patternColor = color;
			setClothes.parts[clothesType].colorInfo[2].patternColor = color;
			imgPtnColor03.color = color;
			FuncUpdateCosColor();
		}

		public void UpdatePtnColor04(Color color)
		{
			clothes.parts[clothesType].colorInfo[3].patternColor = color;
			setClothes.parts[clothesType].colorInfo[3].patternColor = color;
			imgPtnColor04.color = color;
			FuncUpdateCosColor();
		}

		public bool FuncUpdateAllPtnAndColor()
		{
			bool flag = false;
			if (typeClothes == ChaFileDefine.ClothesKind.top)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.co_top, clothes.parts[clothesType].id);
				if (listInfo == null)
				{
					return false;
				}
				if (listInfo.Kind == 1 || listInfo.Kind == 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				chaCtrl.ChangeCustomClothes(false, 0, true, true, true, true, false);
				chaCtrl.ChangeCustomClothes(false, 1, true, true, true, true, false);
				chaCtrl.ChangeCustomClothes(false, 2, true, false, false, false, true);
			}
			else
			{
				chaCtrl.ChangeCustomClothes(true, clothesType, true, true, true, true, true);
			}
			return true;
		}

		public void SetDefaultColorWindow(int no)
		{
			CvsColor.ConnectColorKind kind = CvsColor.ConnectColorKind.CosTop01;
			string winCos01Title = string.Empty;
			switch ((ChaFileDefine.ClothesKind)no)
			{
			case ChaFileDefine.ClothesKind.top:
				kind = CvsColor.ConnectColorKind.CosTop01;
				winCos01Title = "トップスの色０１";
				break;
			case ChaFileDefine.ClothesKind.bot:
				kind = CvsColor.ConnectColorKind.CosBot01;
				winCos01Title = "ボトムスの色０１";
				break;
			case ChaFileDefine.ClothesKind.bra:
				kind = CvsColor.ConnectColorKind.CosBra01;
				winCos01Title = "ブラの色０１";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				kind = CvsColor.ConnectColorKind.CosShorts01;
				winCos01Title = "ショーツの色０１";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				kind = CvsColor.ConnectColorKind.CosGloves01;
				winCos01Title = "手袋の色０１";
				break;
			case ChaFileDefine.ClothesKind.panst:
				kind = CvsColor.ConnectColorKind.CosPanst01;
				winCos01Title = "パンストの色０１";
				break;
			case ChaFileDefine.ClothesKind.socks:
				kind = CvsColor.ConnectColorKind.CosSocks01;
				winCos01Title = "靴下の色０１";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				kind = CvsColor.ConnectColorKind.CosInnerShoes01;
				winCos01Title = "内履きの色０１";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				kind = CvsColor.ConnectColorKind.CosOuterShoes01;
				winCos01Title = "外履きの色０１";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(kind).SafeProc(delegate(string text)
			{
				winCos01Title = text;
			});
			cvsColor.Setup(winCos01Title, kind, clothes.parts[no].colorInfo[0].baseColor, UpdateCosColor01, UpdateCosColorHistory, false);
			bool[] use = new bool[4];
			if (CheckUseColor(no, ref use))
			{
				bool flag = true;
				for (int i = 0; i < use.Length; i++)
				{
					if (use[i])
					{
						flag = false;
					}
				}
				if (flag)
				{
					cvsColor.Close();
				}
			}
			else
			{
				cvsColor.Close();
			}
		}

		protected virtual void Awake()
		{
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				textInitBtnName.text = "トップスを初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.bot:
				textInitBtnName.text = "ボトムスを初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.bra:
				textInitBtnName.text = "ブラを初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				textInitBtnName.text = "ショーツを初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				textInitBtnName.text = "手袋を初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.panst:
				textInitBtnName.text = "パンストを初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.socks:
				textInitBtnName.text = "靴下を初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				textInitBtnName.text = "内履きを初期の状態に戻す";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				textInitBtnName.text = "外履きを初期の状態に戻す";
				break;
			}
			Localize.Translate.Manager.initializeSolution.Add(new TextInitBtnNameSeter(typeClothes, textInitBtnName, base.gameObject));
		}

		protected virtual void Start()
		{
			if ((bool)inpPtn01Width)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn01Width);
			}
			if ((bool)inpPtn01Height)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn01Height);
			}
			if ((bool)inpPtn02Width)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn02Width);
			}
			if ((bool)inpPtn02Height)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn02Height);
			}
			if ((bool)inpPtn03Width)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn03Width);
			}
			if ((bool)inpPtn03Height)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn03Height);
			}
			if ((bool)inpPtn04Width)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn04Width);
			}
			if ((bool)inpPtn04Height)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPtn04Height);
			}
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				Singleton<CustomBase>.Instance.actUpdateCvsCosTop += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.bot:
				Singleton<CustomBase>.Instance.actUpdateCvsCosBot += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.bra:
				Singleton<CustomBase>.Instance.actUpdateCvsCosBra += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.shorts:
				Singleton<CustomBase>.Instance.actUpdateCvsCosShorts += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.gloves:
				Singleton<CustomBase>.Instance.actUpdateCvsCosGloves += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.panst:
				Singleton<CustomBase>.Instance.actUpdateCvsCosPanst += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.socks:
				Singleton<CustomBase>.Instance.actUpdateCvsCosSocks += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				Singleton<CustomBase>.Instance.actUpdateCvsCosInnerShoes += UpdateCustomUI;
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				Singleton<CustomBase>.Instance.actUpdateCvsCosOuterShoes += UpdateCustomUI;
				break;
			}
			tglClothesKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgClothesWin)
				{
					bool flag = ((cgClothesWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgClothesWin.Enable(isOn);
						if (isOn)
						{
							if ((bool)tglSailor01Kind)
							{
								tglSailor01Kind.isOn = false;
							}
							if ((bool)tglSailor02Kind)
							{
								tglSailor02Kind.isOn = false;
							}
							if ((bool)tglSailor03Kind)
							{
								tglSailor03Kind.isOn = false;
							}
							if ((bool)tglJacket01Kind)
							{
								tglJacket01Kind.isOn = false;
							}
							if ((bool)tglJacket02Kind)
							{
								tglJacket02Kind.isOn = false;
							}
							if ((bool)tglJacket03Kind)
							{
								tglJacket03Kind.isOn = false;
							}
							tglPtn01Kind.isOn = false;
							tglPtn02Kind.isOn = false;
							tglPtn03Kind.isOn = false;
							if ((bool)tglPtn04Kind)
							{
								tglPtn04Kind.isOn = false;
							}
							if ((bool)tglEmblemKind)
							{
								tglEmblemKind.isOn = false;
							}
						}
					}
				}
			});
			if ((bool)tglSailor01Kind)
			{
				tglSailor01Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgSailor01Win)
					{
						bool flag2 = ((cgSailor01Win.alpha != 0f) ? true : false);
						if (flag2 != isOn)
						{
							cgSailor01Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor02Kind.isOn = false;
								tglSailor03Kind.isOn = false;
								tglJacket01Kind.isOn = false;
								tglJacket02Kind.isOn = false;
								tglJacket03Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglPtn04Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			if ((bool)tglSailor02Kind)
			{
				tglSailor02Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgSailor02Win)
					{
						bool flag3 = ((cgSailor02Win.alpha != 0f) ? true : false);
						if (flag3 != isOn)
						{
							cgSailor02Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor01Kind.isOn = false;
								tglSailor03Kind.isOn = false;
								tglJacket01Kind.isOn = false;
								tglJacket02Kind.isOn = false;
								tglJacket03Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglPtn04Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			if ((bool)tglSailor03Kind)
			{
				tglSailor03Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgSailor03Win)
					{
						bool flag4 = ((cgSailor03Win.alpha != 0f) ? true : false);
						if (flag4 != isOn)
						{
							cgSailor03Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor01Kind.isOn = false;
								tglSailor02Kind.isOn = false;
								tglJacket01Kind.isOn = false;
								tglJacket02Kind.isOn = false;
								tglJacket03Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglPtn04Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			if ((bool)tglJacket01Kind)
			{
				tglJacket01Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgJacket01Win)
					{
						bool flag5 = ((cgJacket01Win.alpha != 0f) ? true : false);
						if (flag5 != isOn)
						{
							cgJacket01Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor01Kind.isOn = false;
								tglSailor02Kind.isOn = false;
								tglSailor03Kind.isOn = false;
								tglJacket02Kind.isOn = false;
								tglJacket03Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglPtn04Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			if ((bool)tglJacket02Kind)
			{
				tglJacket02Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgJacket02Win)
					{
						bool flag6 = ((cgJacket02Win.alpha != 0f) ? true : false);
						if (flag6 != isOn)
						{
							cgJacket02Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor01Kind.isOn = false;
								tglSailor02Kind.isOn = false;
								tglSailor03Kind.isOn = false;
								tglJacket01Kind.isOn = false;
								tglJacket03Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglPtn04Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			if ((bool)tglJacket03Kind)
			{
				tglJacket03Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgJacket03Win)
					{
						bool flag7 = ((cgJacket03Win.alpha != 0f) ? true : false);
						if (flag7 != isOn)
						{
							cgJacket03Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor01Kind.isOn = false;
								tglSailor02Kind.isOn = false;
								tglSailor03Kind.isOn = false;
								tglJacket01Kind.isOn = false;
								tglJacket02Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglPtn04Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			tglPtn01Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPtn01Win)
				{
					bool flag8 = ((cgPtn01Win.alpha != 0f) ? true : false);
					if (flag8 != isOn)
					{
						cgPtn01Win.Enable(isOn);
						if (isOn)
						{
							tglClothesKind.isOn = false;
							if ((bool)tglSailor01Kind)
							{
								tglSailor01Kind.isOn = false;
							}
							if ((bool)tglSailor02Kind)
							{
								tglSailor02Kind.isOn = false;
							}
							if ((bool)tglSailor03Kind)
							{
								tglSailor03Kind.isOn = false;
							}
							if ((bool)tglJacket01Kind)
							{
								tglJacket01Kind.isOn = false;
							}
							if ((bool)tglJacket02Kind)
							{
								tglJacket02Kind.isOn = false;
							}
							if ((bool)tglJacket03Kind)
							{
								tglJacket03Kind.isOn = false;
							}
							tglPtn02Kind.isOn = false;
							tglPtn03Kind.isOn = false;
							if ((bool)tglPtn04Kind)
							{
								tglPtn04Kind.isOn = false;
							}
							if ((bool)tglEmblemKind)
							{
								tglEmblemKind.isOn = false;
							}
						}
					}
				}
			});
			tglPtn02Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPtn02Win)
				{
					bool flag9 = ((cgPtn02Win.alpha != 0f) ? true : false);
					if (flag9 != isOn)
					{
						cgPtn02Win.Enable(isOn);
						if (isOn)
						{
							tglClothesKind.isOn = false;
							if ((bool)tglSailor01Kind)
							{
								tglSailor01Kind.isOn = false;
							}
							if ((bool)tglSailor02Kind)
							{
								tglSailor02Kind.isOn = false;
							}
							if ((bool)tglSailor03Kind)
							{
								tglSailor03Kind.isOn = false;
							}
							if ((bool)tglJacket01Kind)
							{
								tglJacket01Kind.isOn = false;
							}
							if ((bool)tglJacket02Kind)
							{
								tglJacket02Kind.isOn = false;
							}
							if ((bool)tglJacket03Kind)
							{
								tglJacket03Kind.isOn = false;
							}
							tglPtn01Kind.isOn = false;
							tglPtn03Kind.isOn = false;
							if ((bool)tglPtn04Kind)
							{
								tglPtn04Kind.isOn = false;
							}
							if ((bool)tglEmblemKind)
							{
								tglEmblemKind.isOn = false;
							}
						}
					}
				}
			});
			tglPtn03Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPtn03Win)
				{
					bool flag10 = ((cgPtn03Win.alpha != 0f) ? true : false);
					if (flag10 != isOn)
					{
						cgPtn03Win.Enable(isOn);
						if (isOn)
						{
							tglClothesKind.isOn = false;
							if ((bool)tglSailor01Kind)
							{
								tglSailor01Kind.isOn = false;
							}
							if ((bool)tglSailor02Kind)
							{
								tglSailor02Kind.isOn = false;
							}
							if ((bool)tglSailor03Kind)
							{
								tglSailor03Kind.isOn = false;
							}
							if ((bool)tglJacket01Kind)
							{
								tglJacket01Kind.isOn = false;
							}
							if ((bool)tglJacket02Kind)
							{
								tglJacket02Kind.isOn = false;
							}
							if ((bool)tglJacket03Kind)
							{
								tglJacket03Kind.isOn = false;
							}
							tglPtn01Kind.isOn = false;
							tglPtn02Kind.isOn = false;
							if ((bool)tglPtn04Kind)
							{
								tglPtn04Kind.isOn = false;
							}
							if ((bool)tglEmblemKind)
							{
								tglEmblemKind.isOn = false;
							}
						}
					}
				}
			});
			if ((bool)tglPtn04Kind)
			{
				tglPtn04Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgPtn04Win)
					{
						bool flag11 = ((cgPtn04Win.alpha != 0f) ? true : false);
						if (flag11 != isOn)
						{
							cgPtn04Win.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								tglSailor01Kind.isOn = false;
								tglSailor02Kind.isOn = false;
								tglSailor03Kind.isOn = false;
								tglJacket01Kind.isOn = false;
								tglJacket02Kind.isOn = false;
								tglJacket03Kind.isOn = false;
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								tglEmblemKind.isOn = false;
							}
						}
					}
				});
			}
			if ((bool)tglEmblemKind)
			{
				tglEmblemKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if ((bool)cgEmblemWin)
					{
						bool flag12 = ((cgEmblemWin.alpha != 0f) ? true : false);
						if (flag12 != isOn)
						{
							cgEmblemWin.Enable(isOn);
							if (isOn)
							{
								tglClothesKind.isOn = false;
								if ((bool)tglSailor01Kind)
								{
									tglSailor01Kind.isOn = false;
								}
								if ((bool)tglSailor02Kind)
								{
									tglSailor02Kind.isOn = false;
								}
								if ((bool)tglSailor03Kind)
								{
									tglSailor03Kind.isOn = false;
								}
								if ((bool)tglJacket01Kind)
								{
									tglJacket01Kind.isOn = false;
								}
								if ((bool)tglJacket02Kind)
								{
									tglJacket02Kind.isOn = false;
								}
								if ((bool)tglJacket03Kind)
								{
									tglJacket03Kind.isOn = false;
								}
								tglPtn01Kind.isOn = false;
								tglPtn02Kind.isOn = false;
								tglPtn03Kind.isOn = false;
								if ((bool)tglPtn04Kind)
								{
									tglPtn04Kind.isOn = false;
								}
							}
						}
					}
				});
			}
			CvsColor.ConnectColorKind colorCos01Kind = CvsColor.ConnectColorKind.CosTop01;
			string winCos01Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorCos01Kind = CvsColor.ConnectColorKind.CosTop01;
				winCos01Title = "トップスの色０１";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorCos01Kind = CvsColor.ConnectColorKind.CosBot01;
				winCos01Title = "ボトムスの色０１";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorCos01Kind = CvsColor.ConnectColorKind.CosBra01;
				winCos01Title = "ブラの色０１";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorCos01Kind = CvsColor.ConnectColorKind.CosShorts01;
				winCos01Title = "ショーツの色０１";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorCos01Kind = CvsColor.ConnectColorKind.CosGloves01;
				winCos01Title = "手袋の色０１";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorCos01Kind = CvsColor.ConnectColorKind.CosPanst01;
				winCos01Title = "パンストの色０１";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorCos01Kind = CvsColor.ConnectColorKind.CosSocks01;
				winCos01Title = "靴下の色０１";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorCos01Kind = CvsColor.ConnectColorKind.CosInnerShoes01;
				winCos01Title = "内履きの色０１";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorCos01Kind = CvsColor.ConnectColorKind.CosOuterShoes01;
				winCos01Title = "外履きの色０１";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorCos01Kind).SafeProc(delegate(string text)
			{
				winCos01Title = text;
			});
			btnMainColor01.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorCos01Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winCos01Title, colorCos01Kind, clothes.parts[clothesType].colorInfo[0].baseColor, UpdateCosColor01, UpdateCosColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorCos02Kind = CvsColor.ConnectColorKind.CosTop02;
			string winCos02Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorCos02Kind = CvsColor.ConnectColorKind.CosTop02;
				winCos02Title = "トップスの色０２";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorCos02Kind = CvsColor.ConnectColorKind.CosBot02;
				winCos02Title = "ボトムスの色０２";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorCos02Kind = CvsColor.ConnectColorKind.CosBra02;
				winCos02Title = "ブラの色０２";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorCos02Kind = CvsColor.ConnectColorKind.CosShorts02;
				winCos02Title = "ショーツの色０２";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorCos02Kind = CvsColor.ConnectColorKind.CosGloves02;
				winCos02Title = "手袋の色０２";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorCos02Kind = CvsColor.ConnectColorKind.CosPanst02;
				winCos02Title = "パンストの色０２";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorCos02Kind = CvsColor.ConnectColorKind.CosSocks02;
				winCos02Title = "靴下の色０２";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorCos02Kind = CvsColor.ConnectColorKind.CosInnerShoes02;
				winCos02Title = "内履きの色０２";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorCos02Kind = CvsColor.ConnectColorKind.CosOuterShoes02;
				winCos02Title = "外履きの色０２";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorCos02Kind).SafeProc(delegate(string text)
			{
				winCos02Title = text;
			});
			btnMainColor02.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorCos02Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winCos02Title, colorCos02Kind, clothes.parts[clothesType].colorInfo[1].baseColor, UpdateCosColor02, UpdateCosColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorCos03Kind = CvsColor.ConnectColorKind.CosTop03;
			string winCos03Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorCos03Kind = CvsColor.ConnectColorKind.CosTop03;
				winCos03Title = "トップスの色０３";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorCos03Kind = CvsColor.ConnectColorKind.CosBot03;
				winCos03Title = "ボトムスの色０３";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorCos03Kind = CvsColor.ConnectColorKind.CosBra03;
				winCos03Title = "ブラの色０３";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorCos03Kind = CvsColor.ConnectColorKind.CosShorts03;
				winCos03Title = "ショーツの色０３";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorCos03Kind = CvsColor.ConnectColorKind.CosGloves03;
				winCos03Title = "手袋の色０３";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorCos03Kind = CvsColor.ConnectColorKind.CosPanst03;
				winCos03Title = "パンストの色０３";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorCos03Kind = CvsColor.ConnectColorKind.CosSocks03;
				winCos03Title = "靴下の色０３";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorCos03Kind = CvsColor.ConnectColorKind.CosInnerShoes03;
				winCos03Title = "内履きの色０３";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorCos03Kind = CvsColor.ConnectColorKind.CosOuterShoes03;
				winCos03Title = "外履きの色０３";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorCos03Kind).SafeProc(delegate(string text)
			{
				winCos03Title = text;
			});
			btnMainColor03.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorCos03Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winCos03Title, colorCos03Kind, clothes.parts[clothesType].colorInfo[2].baseColor, UpdateCosColor03, UpdateCosColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorCos04Kind = CvsColor.ConnectColorKind.CosTop04;
			string winCos04Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorCos04Kind = CvsColor.ConnectColorKind.CosTop04;
				winCos04Title = "トップスの色０４";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorCos04Kind = CvsColor.ConnectColorKind.CosBot04;
				winCos04Title = "ボトムスの色０４";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorCos04Kind = CvsColor.ConnectColorKind.CosBra04;
				winCos04Title = "ブラの色０４";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorCos04Kind = CvsColor.ConnectColorKind.CosShorts04;
				winCos04Title = "ショーツの色０４";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorCos04Kind = CvsColor.ConnectColorKind.CosGloves04;
				winCos04Title = "手袋の色０４";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorCos04Kind = CvsColor.ConnectColorKind.CosPanst04;
				winCos04Title = "パンストの色０４";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorCos04Kind = CvsColor.ConnectColorKind.CosSocks04;
				winCos04Title = "靴下の色０４";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorCos04Kind = CvsColor.ConnectColorKind.CosInnerShoes04;
				winCos04Title = "内履きの色０４";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorCos04Kind = CvsColor.ConnectColorKind.CosOuterShoes04;
				winCos04Title = "外履きの色０４";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorCos04Kind).SafeProc(delegate(string text)
			{
				winCos04Title = text;
			});
			if ((bool)btnMainColor04)
			{
				btnMainColor04.OnClickAsObservable().Subscribe(delegate
				{
					if (cvsColor.isOpen && cvsColor.connectColorKind == colorCos04Kind)
					{
						cvsColor.Close();
					}
					else
					{
						cvsColor.Setup(winCos04Title, colorCos04Kind, clothes.parts[clothesType].colorInfo[3].baseColor, UpdateCosColor04, UpdateCosColorHistory, false);
					}
				});
			}
			CvsColor.ConnectColorKind colorPtn01Kind = CvsColor.ConnectColorKind.CosTopPtn01;
			string winPtn01Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosTopPtn01;
				winPtn01Title = "トップス柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosBotPtn01;
				winPtn01Title = "ボトムス柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosBraPtn01;
				winPtn01Title = "ブラ柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosShortsPtn01;
				winPtn01Title = "ショーツ柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosGlovesPtn01;
				winPtn01Title = "手袋柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosPanstPtn01;
				winPtn01Title = "パンスト柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosSocksPtn01;
				winPtn01Title = "靴下柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosInnerShoesPtn01;
				winPtn01Title = "内履き柄の色０１";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorPtn01Kind = CvsColor.ConnectColorKind.CosOuterShoesPtn01;
				winPtn01Title = "外履き柄の色０１";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorPtn01Kind).SafeProc(delegate(string text)
			{
				winPtn01Title = text;
			});
			btnPtnColor01.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorPtn01Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winPtn01Title, colorPtn01Kind, clothes.parts[clothesType].colorInfo[0].patternColor, UpdatePtnColor01, UpdateCosColorHistory, true);
				}
			});
			CvsColor.ConnectColorKind colorPtn02Kind = CvsColor.ConnectColorKind.CosTopPtn02;
			string winPtn02Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosTopPtn02;
				winPtn02Title = "トップス柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosBotPtn02;
				winPtn02Title = "ボトムス柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosBraPtn02;
				winPtn02Title = "ブラ柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosShortsPtn02;
				winPtn02Title = "ショーツ柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosGlovesPtn02;
				winPtn02Title = "手袋柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosPanstPtn02;
				winPtn02Title = "パンスト柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosSocksPtn02;
				winPtn02Title = "靴下柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosInnerShoesPtn02;
				winPtn02Title = "内履き柄の色０２";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorPtn02Kind = CvsColor.ConnectColorKind.CosOuterShoesPtn02;
				winPtn02Title = "外履き柄の色０２";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorPtn02Kind).SafeProc(delegate(string text)
			{
				winPtn02Title = text;
			});
			btnPtnColor02.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorPtn02Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winPtn02Title, colorPtn02Kind, clothes.parts[clothesType].colorInfo[1].patternColor, UpdatePtnColor02, UpdateCosColorHistory, true);
				}
			});
			CvsColor.ConnectColorKind colorPtn03Kind = CvsColor.ConnectColorKind.CosTopPtn03;
			string winPtn03Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosTopPtn03;
				winPtn03Title = "トップス柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosBotPtn03;
				winPtn03Title = "ボトムス柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosBraPtn03;
				winPtn03Title = "ブラ柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosShortsPtn03;
				winPtn03Title = "ショーツ柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosGlovesPtn03;
				winPtn03Title = "手袋柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosPanstPtn03;
				winPtn03Title = "パンスト柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosSocksPtn03;
				winPtn03Title = "靴下柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosInnerShoesPtn03;
				winPtn03Title = "内履き柄の色０３";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorPtn03Kind = CvsColor.ConnectColorKind.CosOuterShoesPtn03;
				winPtn03Title = "外履き柄の色０３";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorPtn03Kind).SafeProc(delegate(string text)
			{
				winPtn03Title = text;
			});
			btnPtnColor03.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorPtn03Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winPtn03Title, colorPtn03Kind, clothes.parts[clothesType].colorInfo[2].patternColor, UpdatePtnColor03, UpdateCosColorHistory, true);
				}
			});
			CvsColor.ConnectColorKind colorPtn04Kind = CvsColor.ConnectColorKind.CosTopPtn04;
			string winPtn04Title = string.Empty;
			switch (typeClothes)
			{
			case ChaFileDefine.ClothesKind.top:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosTopPtn04;
				winPtn04Title = "トップス柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.bot:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosBotPtn04;
				winPtn04Title = "ボトムス柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.bra:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosBraPtn04;
				winPtn04Title = "ブラ柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.shorts:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosShortsPtn04;
				winPtn04Title = "ショーツ柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.gloves:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosGlovesPtn04;
				winPtn04Title = "手袋柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.panst:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosPanstPtn04;
				winPtn04Title = "パンスト柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.socks:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosSocksPtn04;
				winPtn04Title = "靴下柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.shoes_inner:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosInnerShoesPtn04;
				winPtn04Title = "内履き柄の色０４";
				break;
			case ChaFileDefine.ClothesKind.shoes_outer:
				colorPtn04Kind = CvsColor.ConnectColorKind.CosOuterShoesPtn04;
				winPtn04Title = "外履き柄の色０４";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorPtn04Kind).SafeProc(delegate(string text)
			{
				winPtn04Title = text;
			});
			if ((bool)btnPtnColor04)
			{
				btnPtnColor04.OnClickAsObservable().Subscribe(delegate
				{
					if (cvsColor.isOpen && cvsColor.connectColorKind == colorPtn04Kind)
					{
						cvsColor.Close();
					}
					else
					{
						cvsColor.Setup(winPtn04Title, colorPtn04Kind, clothes.parts[clothesType].colorInfo[3].patternColor, UpdatePtnColor04, UpdateCosColorHistory, true);
					}
				});
			}
			sldPtn01Width.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				clothes.parts[clothesType].colorInfo[0].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[0].tiling.y);
				setClothes.parts[clothesType].colorInfo[0].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[0].tiling.y);
				FuncUpdateCosColor();
				inpPtn01Width.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPtn01Width.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn01Width.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPtn01Width.value = Mathf.Clamp(sldPtn01Width.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPtn01Width.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn01Width.value);
			});
			inpPtn01Width.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPtn01Width.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			btnPtn01Width.onClick.AsObservable().Subscribe(delegate
			{
				float x = defClothes.parts[clothesType].colorInfo[0].tiling.x;
				clothes.parts[clothesType].colorInfo[0].tiling = new Vector2(x, clothes.parts[clothesType].colorInfo[0].tiling.y);
				setClothes.parts[clothesType].colorInfo[0].tiling = new Vector2(x, clothes.parts[clothesType].colorInfo[0].tiling.y);
				inpPtn01Width.text = CustomBase.ConvertTextFromRate(0, 100, x);
				sldPtn01Width.value = x;
				FuncUpdateCosColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn01Height.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				clothes.parts[clothesType].colorInfo[0].tiling = new Vector2(clothes.parts[clothesType].colorInfo[0].tiling.x, value);
				setClothes.parts[clothesType].colorInfo[0].tiling = new Vector2(clothes.parts[clothesType].colorInfo[0].tiling.x, value);
				FuncUpdateCosColor();
				inpPtn01Height.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPtn01Height.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn01Height.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPtn01Height.value = Mathf.Clamp(sldPtn01Height.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPtn01Height.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn01Height.value);
			});
			inpPtn01Height.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPtn01Height.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			btnPtn01Height.onClick.AsObservable().Subscribe(delegate
			{
				float y = defClothes.parts[clothesType].colorInfo[0].tiling.y;
				clothes.parts[clothesType].colorInfo[0].tiling = new Vector2(clothes.parts[clothesType].colorInfo[0].tiling.x, y);
				setClothes.parts[clothesType].colorInfo[0].tiling = new Vector2(clothes.parts[clothesType].colorInfo[0].tiling.x, y);
				inpPtn01Height.text = CustomBase.ConvertTextFromRate(0, 100, y);
				sldPtn01Height.value = y;
				FuncUpdateCosColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn02Width.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				clothes.parts[clothesType].colorInfo[1].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[1].tiling.y);
				setClothes.parts[clothesType].colorInfo[1].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[1].tiling.y);
				FuncUpdateCosColor();
				inpPtn02Width.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPtn02Width.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn02Width.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPtn02Width.value = Mathf.Clamp(sldPtn02Width.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPtn02Width.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn02Width.value);
			});
			inpPtn02Width.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPtn02Width.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			btnPtn02Width.onClick.AsObservable().Subscribe(delegate
			{
				float x2 = defClothes.parts[clothesType].colorInfo[1].tiling.x;
				clothes.parts[clothesType].colorInfo[1].tiling = new Vector2(x2, clothes.parts[clothesType].colorInfo[1].tiling.y);
				setClothes.parts[clothesType].colorInfo[1].tiling = new Vector2(x2, clothes.parts[clothesType].colorInfo[1].tiling.y);
				inpPtn02Width.text = CustomBase.ConvertTextFromRate(0, 100, x2);
				sldPtn02Width.value = x2;
				FuncUpdateCosColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn02Height.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				clothes.parts[clothesType].colorInfo[1].tiling = new Vector2(clothes.parts[clothesType].colorInfo[1].tiling.x, value);
				setClothes.parts[clothesType].colorInfo[1].tiling = new Vector2(clothes.parts[clothesType].colorInfo[1].tiling.x, value);
				FuncUpdateCosColor();
				inpPtn02Height.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPtn02Height.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn02Height.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPtn02Height.value = Mathf.Clamp(sldPtn02Height.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPtn02Height.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn02Height.value);
			});
			inpPtn02Height.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPtn02Height.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			btnPtn02Height.onClick.AsObservable().Subscribe(delegate
			{
				float y2 = defClothes.parts[clothesType].colorInfo[1].tiling.y;
				clothes.parts[clothesType].colorInfo[1].tiling = new Vector2(clothes.parts[clothesType].colorInfo[1].tiling.x, y2);
				setClothes.parts[clothesType].colorInfo[1].tiling = new Vector2(clothes.parts[clothesType].colorInfo[1].tiling.x, y2);
				inpPtn02Height.text = CustomBase.ConvertTextFromRate(0, 100, y2);
				sldPtn02Height.value = y2;
				FuncUpdateCosColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn03Width.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				clothes.parts[clothesType].colorInfo[2].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[2].tiling.y);
				setClothes.parts[clothesType].colorInfo[2].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[2].tiling.y);
				FuncUpdateCosColor();
				inpPtn03Width.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPtn03Width.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn03Width.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPtn03Width.value = Mathf.Clamp(sldPtn03Width.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPtn03Width.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn03Width.value);
			});
			inpPtn03Width.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPtn03Width.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			btnPtn03Width.onClick.AsObservable().Subscribe(delegate
			{
				float x3 = defClothes.parts[clothesType].colorInfo[2].tiling.x;
				clothes.parts[clothesType].colorInfo[2].tiling = new Vector2(x3, clothes.parts[clothesType].colorInfo[2].tiling.y);
				setClothes.parts[clothesType].colorInfo[2].tiling = new Vector2(x3, clothes.parts[clothesType].colorInfo[2].tiling.y);
				inpPtn03Width.text = CustomBase.ConvertTextFromRate(0, 100, x3);
				sldPtn03Width.value = x3;
				FuncUpdateCosColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn03Height.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				clothes.parts[clothesType].colorInfo[2].tiling = new Vector2(clothes.parts[clothesType].colorInfo[2].tiling.x, value);
				setClothes.parts[clothesType].colorInfo[2].tiling = new Vector2(clothes.parts[clothesType].colorInfo[2].tiling.x, value);
				FuncUpdateCosColor();
				inpPtn03Height.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPtn03Height.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			sldPtn03Height.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPtn03Height.value = Mathf.Clamp(sldPtn03Height.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPtn03Height.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn03Height.value);
			});
			inpPtn03Height.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPtn03Height.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			btnPtn03Height.onClick.AsObservable().Subscribe(delegate
			{
				float y3 = defClothes.parts[clothesType].colorInfo[2].tiling.y;
				clothes.parts[clothesType].colorInfo[2].tiling = new Vector2(clothes.parts[clothesType].colorInfo[2].tiling.x, y3);
				setClothes.parts[clothesType].colorInfo[2].tiling = new Vector2(clothes.parts[clothesType].colorInfo[2].tiling.x, y3);
				inpPtn03Height.text = CustomBase.ConvertTextFromRate(0, 100, y3);
				sldPtn03Height.value = y3;
				FuncUpdateCosColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
			});
			if ((bool)sldPtn04Width)
			{
				sldPtn04Width.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					clothes.parts[clothesType].colorInfo[3].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[3].tiling.y);
					setClothes.parts[clothesType].colorInfo[3].tiling = new Vector2(value, clothes.parts[clothesType].colorInfo[3].tiling.y);
					FuncUpdateCosColor();
					inpPtn04Width.text = CustomBase.ConvertTextFromRate(0, 100, value);
				});
			}
			if ((bool)sldPtn04Width)
			{
				sldPtn04Width.OnPointerUpAsObservable().Subscribe(delegate
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
				});
			}
			if ((bool)sldPtn04Width)
			{
				sldPtn04Width.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldPtn04Width.value = Mathf.Clamp(sldPtn04Width.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
					inpPtn04Width.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn04Width.value);
				});
			}
			if ((bool)inpPtn04Width)
			{
				inpPtn04Width.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					sldPtn04Width.value = CustomBase.ConvertRateFromText(0, 100, value);
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
				});
			}
			if ((bool)btnPtn04Width)
			{
				btnPtn04Width.onClick.AsObservable().Subscribe(delegate
				{
					float x4 = defClothes.parts[clothesType].colorInfo[3].tiling.x;
					clothes.parts[clothesType].colorInfo[3].tiling = new Vector2(x4, clothes.parts[clothesType].colorInfo[3].tiling.y);
					setClothes.parts[clothesType].colorInfo[3].tiling = new Vector2(x4, clothes.parts[clothesType].colorInfo[3].tiling.y);
					inpPtn04Width.text = CustomBase.ConvertTextFromRate(0, 100, x4);
					sldPtn04Width.value = x4;
					FuncUpdateCosColor();
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
				});
			}
			if ((bool)sldPtn04Height)
			{
				sldPtn04Height.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					clothes.parts[clothesType].colorInfo[3].tiling = new Vector2(clothes.parts[clothesType].colorInfo[3].tiling.x, value);
					setClothes.parts[clothesType].colorInfo[3].tiling = new Vector2(clothes.parts[clothesType].colorInfo[3].tiling.x, value);
					FuncUpdateCosColor();
					inpPtn04Height.text = CustomBase.ConvertTextFromRate(0, 100, value);
				});
			}
			if ((bool)sldPtn04Height)
			{
				sldPtn04Height.OnPointerUpAsObservable().Subscribe(delegate
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
				});
			}
			if ((bool)sldPtn04Height)
			{
				sldPtn04Height.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldPtn04Height.value = Mathf.Clamp(sldPtn04Height.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
					inpPtn04Height.text = CustomBase.ConvertTextFromRate(0, 100, sldPtn04Height.value);
				});
			}
			if ((bool)inpPtn04Height)
			{
				inpPtn04Height.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					sldPtn04Height.value = CustomBase.ConvertRateFromText(0, 100, value);
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
				});
			}
			if ((bool)btnPtn04Height)
			{
				btnPtn04Height.onClick.AsObservable().Subscribe(delegate
				{
					float y4 = defClothes.parts[clothesType].colorInfo[3].tiling.y;
					clothes.parts[clothesType].colorInfo[3].tiling = new Vector2(clothes.parts[clothesType].colorInfo[3].tiling.x, y4);
					setClothes.parts[clothesType].colorInfo[3].tiling = new Vector2(clothes.parts[clothesType].colorInfo[3].tiling.x, y4);
					inpPtn04Height.text = CustomBase.ConvertTextFromRate(0, 100, y4);
					sldPtn04Height.value = y4;
					FuncUpdateCosColor();
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCosColor);
				});
			}
			btnInitSetting.OnClickAsObservable().Subscribe(delegate
			{
				for (int i = 0; i < 4; i++)
				{
					Color clothesDefaultColor = chaCtrl.GetClothesDefaultColor(clothesType, i);
					clothes.parts[clothesType].colorInfo[i].baseColor = clothesDefaultColor;
					setClothes.parts[clothesType].colorInfo[i].baseColor = clothesDefaultColor;
					clothes.parts[clothesType].colorInfo[i].patternColor = Color.white;
					setClothes.parts[clothesType].colorInfo[i].patternColor = Color.white;
					clothes.parts[clothesType].colorInfo[i].pattern = 0;
					setClothes.parts[clothesType].colorInfo[i].pattern = 0;
					clothes.parts[clothesType].colorInfo[i].tiling = Vector2.zero;
					setClothes.parts[clothesType].colorInfo[i].tiling = Vector2.zero;
				}
				FuncUpdateAllPtnAndColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllPtnAndColor);
				UpdateCustomUI();
			});
			tglOption.Select((Toggle val, int idx) => new { val, idx }).ToList().ForEach(item =>
			{
				item.val.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (clothesType == 2)
					{
						clothes.hideBraOpt[item.idx] = !isOn;
						setClothes.hideBraOpt[item.idx] = !isOn;
					}
					else if (clothesType == 3)
					{
						clothes.hideShortsOpt[item.idx] = !isOn;
						setClothes.hideShortsOpt[item.idx] = !isOn;
					}
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				});
			});
			StartCoroutine(SetInputText());
		}
	}
}
