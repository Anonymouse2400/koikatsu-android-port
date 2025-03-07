using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Component.UI.ColorPicker;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class HSetting : BaseSetting
	{
		[Serializable]
		private class SampleImage
		{
			public Image image;

			public PickerSlider picker;
		}

		[Header("マップの表示")]
		[SerializeField]
		private Toggle visibleMapToggle;

		[Header("男の単色表示")]
		[SerializeField]
		private Toggle simpleBodyToggle;

		[SerializeField]
		[Header("男根表示")]
		private Toggle visibleSonToggle;

		[SerializeField]
		[Header("体表示")]
		private Toggle visibleBodyToggle;

		[SerializeField]
		[Header("男の衣装")]
		private Toggle maleClothesToggle;

		[Header("男のアクセサリーメイン")]
		[SerializeField]
		private Toggle maleAccessoriesMainToggle;

		[SerializeField]
		[Header("男のアクセサリーサブ")]
		private Toggle maleAccessoriesSubToggle;

		[SerializeField]
		[Header("男の靴")]
		private Toggle maleShoesToggle;

		[SerializeField]
		[Header("女の視線")]
		private Toggle femaleEyesCameraToggle;

		[SerializeField]
		[Header("女の首の向き")]
		private Toggle femaleNeckCameraToggle;

		[SerializeField]
		[Header("2人目の女の視線")]
		private Toggle femaleEyesCameraToggle1;

		[Header("2人目の女の首の向き")]
		[SerializeField]
		private Toggle femaleNeckCameraToggle1;

		[SerializeField]
		[Header("Hシーン中のカメラ初期化判断")]
		private Toggle HInitCameraToggle;

		[SerializeField]
		[Header("遮蔽物")]
		private Toggle shieldToggle;

		[Header("汁物の表現[0=>無し, 1=>3D詳細, 2=>3D簡易]")]
		[SerializeField]
		private Toggle[] semenTypeToggles;

		[SerializeField]
		[Header("背景色")]
		private SampleImage backColor;

		[SerializeField]
		[Header("男の単色変更")]
		private SampleImage silhouetteColor;

		[Header("乳首立ち")]
		[SerializeField]
		private Slider nipSlider;

		public override void Init()
		{
			EtceteraSystem data = Manager.Config.EtcData;
			LinkToggle(visibleMapToggle, delegate(bool isOn)
			{
				data.Map = isOn;
			});
			LinkToggle(maleClothesToggle, delegate(bool isOn)
			{
				data.IsMaleClothes = isOn;
			});
			LinkToggle(maleAccessoriesMainToggle, delegate(bool isOn)
			{
				data.IsMaleAccessoriesMain = isOn;
			});
			LinkToggle(maleAccessoriesSubToggle, delegate(bool isOn)
			{
				data.IsMaleAccessoriesSub = isOn;
			});
			LinkToggle(maleShoesToggle, delegate(bool isOn)
			{
				data.IsMaleShoes = isOn;
			});
			LinkToggle(simpleBodyToggle, delegate(bool isOn)
			{
				data.SimpleBody = isOn;
			});
			LinkToggle(visibleSonToggle, delegate(bool isOn)
			{
				data.VisibleSon = isOn;
			});
			LinkToggle(visibleBodyToggle, delegate(bool isOn)
			{
				data.VisibleBody = isOn;
			});
			LinkToggle(femaleEyesCameraToggle, delegate(bool isOn)
			{
				data.FemaleEyesCamera = isOn;
			});
			LinkToggle(femaleNeckCameraToggle, delegate(bool isOn)
			{
				data.FemaleNeckCamera = isOn;
			});
			if ((bool)femaleEyesCameraToggle1)
			{
				LinkToggle(femaleEyesCameraToggle1, delegate(bool isOn)
				{
					data.FemaleEyesCamera1 = isOn;
				});
			}
			if ((bool)femaleNeckCameraToggle1)
			{
				LinkToggle(femaleNeckCameraToggle1, delegate(bool isOn)
				{
					data.FemaleNeckCamera1 = isOn;
				});
			}
			LinkToggle(HInitCameraToggle, delegate(bool isOn)
			{
				data.HInitCamera = isOn;
			});
			LinkToggle(shieldToggle, delegate(bool isOn)
			{
				data.Shield = isOn;
			});
			ReadOnlyReactiveProperty<int> source = (from list in semenTypeToggles.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).ToReadOnlyReactiveProperty();
			source.Subscribe(delegate(int i)
			{
				data.SemenType = i;
			});
			source.Skip(1).Subscribe(delegate
			{
				EnterSE();
			});
			backColor.picker.updateColorAction += delegate(Color color)
			{
				data.BackColor = color;
				backColor.image.color = color;
			};
			silhouetteColor.picker.updateColorAction += delegate(Color color)
			{
				data.SilhouetteColor = color;
				silhouetteColor.image.color = color;
			};
			LinkSlider(nipSlider, delegate(float value)
			{
				data.nipMax = value;
			});
		}

		protected override void ValueToUI()
		{
			EtceteraSystem etcData = Manager.Config.EtcData;
			visibleMapToggle.isOn = etcData.Map;
			maleClothesToggle.isOn = etcData.IsMaleClothes;
			maleAccessoriesMainToggle.isOn = etcData.IsMaleAccessoriesMain;
			maleAccessoriesSubToggle.isOn = etcData.IsMaleAccessoriesSub;
			maleShoesToggle.isOn = etcData.IsMaleShoes;
			simpleBodyToggle.isOn = etcData.SimpleBody;
			visibleSonToggle.isOn = etcData.VisibleSon;
			visibleBodyToggle.isOn = etcData.VisibleBody;
			femaleEyesCameraToggle.isOn = etcData.FemaleEyesCamera;
			femaleNeckCameraToggle.isOn = etcData.FemaleNeckCamera;
			if ((bool)femaleEyesCameraToggle1)
			{
				femaleEyesCameraToggle1.isOn = etcData.FemaleEyesCamera1;
			}
			if ((bool)femaleNeckCameraToggle1)
			{
				femaleNeckCameraToggle1.isOn = etcData.FemaleNeckCamera1;
			}
			HInitCameraToggle.isOn = etcData.HInitCamera;
			shieldToggle.isOn = etcData.Shield;
			foreach (var item in semenTypeToggles.Select((Toggle tgl, int index) => new { tgl, index }))
			{
				item.tgl.isOn = item.index == etcData.SemenType;
			}
			backColor.picker.SetColor(etcData.BackColor);
			silhouetteColor.picker.SetColor(etcData.SilhouetteColor);
			nipSlider.value = etcData.nipMax;
		}
	}
}
