using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomAcsParentWindow : MonoBehaviour
	{
		public enum AcsSlotNo
		{
			Slot01 = 0,
			Slot02 = 1,
			Slot03 = 2,
			Slot04 = 3,
			Slot05 = 4,
			Slot06 = 5,
			Slot07 = 6,
			Slot08 = 7,
			Slot09 = 8,
			Slot10 = 9,
			Slot11 = 10,
			Slot12 = 11,
			Slot13 = 12,
			Slot14 = 13,
			Slot15 = 14,
			Slot16 = 15,
			Slot17 = 16,
			Slot18 = 17,
			Slot19 = 18,
			Slot20 = 19
		}

		[Serializable]
		public class TypeReactiveProperty : ReactiveProperty<AcsSlotNo>
		{
			public TypeReactiveProperty()
			{
			}

			public TypeReactiveProperty(AcsSlotNo initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private TypeReactiveProperty _slotNo = new TypeReactiveProperty(AcsSlotNo.Slot01);

		[SerializeField]
		private TextMeshProUGUI textTitle;

		[SerializeField]
		private Toggle[] tglParent;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private Toggle tglReference;

		[SerializeField]
		private CvsAccessory[] cvsAccessory;

		private bool updateWin;

		public AcsSlotNo slotNo
		{
			get
			{
				return _slotNo.Value;
			}
			set
			{
				_slotNo.Value = value;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileAccessory accessory
		{
			get
			{
				return chaCtrl.nowCoordinate.accessory;
			}
		}

		private ChaFileAccessory setAccessory
		{
			get
			{
				return chaCtrl.chaFile.coordinate[chaCtrl.chaFile.status.coordinateType].accessory;
			}
		}

		private ChaFileAccessory defAccessory
		{
			get
			{
				return Singleton<CustomBase>.Instance.defChaInfo.coordinate[chaCtrl.chaFile.status.coordinateType].accessory;
			}
		}

		public void UpdateWindow()
		{
			updateWin = true;
			if ((bool)textTitle)
			{
				Singleton<CustomBase>.Instance.FontBind(textTitle);
				string format = "スロット{0:00}の親を選択";
				Singleton<CustomBase>.Instance.TranslateSlotTitle(1).SafeProc(delegate(string text)
				{
					format = text;
				});
				textTitle.text = string.Format(format, (int)(slotNo + 1));
			}
			SelectParent(accessory.parts[(int)slotNo].parentKey);
			updateWin = false;
		}

		public void ChangeSlot(int _no, bool open)
		{
			slotNo = (AcsSlotNo)_no;
			bool isOn = tglReference.isOn;
			tglReference.isOn = false;
			tglReference = cvsAccessory[(int)slotNo].tglAcsParent;
			if (open && isOn)
			{
				tglReference.isOn = true;
			}
		}

		public void CloseWindow()
		{
			tglReference.isOn = false;
		}

		public int SelectParent(string parentKey)
		{
			string[] array = (from key in Enum.GetNames(typeof(ChaAccessoryDefine.AccessoryParentKey))
				where key != "none"
				select key).ToArray();
			int num = Array.IndexOf(array, parentKey);
			if (num != -1)
			{
				Toggle[] array2 = tglParent;
				foreach (Toggle toggle in array2)
				{
					toggle.isOn = false;
				}
				tglParent[num].isOn = true;
			}
			return num;
		}

		public int UpdateCustomUI(int param = 0)
		{
			updateWin = true;
			int result = SelectParent(accessory.parts[(int)slotNo].parentKey);
			updateWin = false;
			return result;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			_slotNo.TakeUntilDestroy(this).Subscribe(delegate
			{
				UpdateWindow();
			});
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					if ((bool)tglReference)
					{
						tglReference.isOn = false;
					}
				});
			}
			tglParent.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!updateWin && isOn)
					{
						cvsAccessory[(int)slotNo].UpdateSelectAccessoryParent(p.index);
					}
				});
			});
		}
	}
}
