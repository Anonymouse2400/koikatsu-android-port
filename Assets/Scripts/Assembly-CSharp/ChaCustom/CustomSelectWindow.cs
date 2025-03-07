using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomSelectWindow : MonoBehaviour
	{
		public enum SelectWindowType
		{
			FaceDetail = 0,
			Eyebrow = 1,
			EyelineUp = 2,
			EyelineLow = 3,
			EyeWhite = 4,
			EyeHiUp = 5,
			EyeHiLow = 6,
			Pupil = 7,
			PupilGrad = 8,
			Nose = 9,
			LipLine = 10,
			Mole = 11,
			EyeShadow = 12,
			Cheek = 13,
			Lip = 14,
			FacePaint01 = 15,
			FacePaint02 = 16,
			BodyDetail = 17,
			Nip = 18,
			UnderHair = 19,
			SunBurn = 20,
			BodyPaint01 = 21,
			BodyPaint02 = 22,
			BodyPaint01Layout = 23,
			BodyPaint02Layout = 24,
			HairFront = 25,
			HairBack = 26,
			HairSide = 27,
			HairOption = 28,
			CosTop = 29,
			CosSailorA = 30,
			CosSailorB = 31,
			CosSailorC = 32,
			CosJacketA = 33,
			CosJacketB = 34,
			CosJacketC = 35,
			CosPattern = 36,
			Emblem = 37,
			CosBot = 38,
			CosBra = 39,
			CosShorts = 40,
			CosGloves = 41,
			CosPanst = 42,
			CosSocks = 43,
			CosShoes = 44,
			AcsHair = 45,
			AcsHead = 46,
			AcsFace = 47,
			AcsNeck = 48,
			AcsBody = 49,
			AcsWaist = 50,
			AcsLeg = 51,
			AcsArm = 52,
			AcsHand = 53,
			AcsKokan = 54,
			HairGloss = 55
		}

		[Serializable]
		public class TypeReactiveProperty : ReactiveProperty<SelectWindowType>
		{
			public TypeReactiveProperty()
			{
			}

			public TypeReactiveProperty(SelectWindowType initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private TypeReactiveProperty _swType = new TypeReactiveProperty(SelectWindowType.FaceDetail);

		[SerializeField]
		private TextMeshProUGUI textTitle;

		[SerializeField]
		private Button btnClose;

		public Toggle tglReference;

		[SerializeField]
		private GameObject objTglCategory;

		[SerializeField]
		private GameObject objTglTemp;

		[HideInInspector]
		public Toggle[] tglCategory;

		public SelectWindowType swType
		{
			get
			{
				return _swType.Value;
			}
			set
			{
				_swType.Value = value;
			}
		}

		public void Awake()
		{
		}

		public IEnumerator Start()
		{
			yield return new WaitUntil(() => null != Singleton<CustomBase>.Instance.lstSelectList);
			_swType.TakeUntilDestroy(this).Subscribe(delegate
			{
				UpdateWindow();
			});
			if (!btnClose)
			{
				yield break;
			}
			btnClose.OnClickAsObservable().Subscribe(delegate
			{
				if ((bool)tglReference)
				{
					tglReference.isOn = false;
				}
			});
		}

		public void UpdateWindow()
		{
			List<ExcelData.Param> lstSelectList = Singleton<CustomBase>.Instance.lstSelectList;
			ExcelData.Param param = lstSelectList.FirstOrDefault((ExcelData.Param x) => swType.ToString() == x.list[1]);
			if (param == null)
			{
				return;
			}
			if (null != textTitle)
			{
				textTitle.text = param.list[2];
			}
			if ((bool)objTglCategory)
			{
				objTglCategory.SetActiveIfDifferent("1" == param.list[3]);
			}
			List<Toggle> list = new List<Toggle>();
			int num = param.list.Count - 4;
			for (int i = 0; i < num && !("0" == param.list[4 + i]); i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objTglTemp, objTglCategory.transform, false);
				if (null == gameObject)
				{
					break;
				}
				Transform transform = gameObject.transform.Find("textCate");
				if (null == transform)
				{
					break;
				}
				TextMeshProUGUI component = transform.GetComponent<TextMeshProUGUI>();
				if ((bool)component)
				{
					component.text = param.list[4 + i];
				}
				Toggle component2 = gameObject.GetComponent<Toggle>();
				gameObject.SetActiveIfDifferent(true);
				list.Add(component2);
			}
			if (list.Count != 0)
			{
				tglCategory = list.ToArray();
			}
		}
	}
}
