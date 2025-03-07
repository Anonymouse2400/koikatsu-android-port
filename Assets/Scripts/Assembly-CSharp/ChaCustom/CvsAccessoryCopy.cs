using MessagePack;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsAccessoryCopy : MonoBehaviour
	{
		[SerializeField]
		private TMP_Dropdown[] ddCoordeType;

		[SerializeField]
		private Toggle[] tglKind;

		[SerializeField]
		private TextMeshProUGUI[] textDst;

		[SerializeField]
		private TextMeshProUGUI[] textSrc;

		[SerializeField]
		private Button btnAllOn;

		[SerializeField]
		private Button btnAllOff;

		[SerializeField]
		private Button btnCopy;

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
			ChaFileAccessory chaFileAccessory = chaCtrl.chaFile.coordinate[ddCoordeType[0].value].accessory;
			for (int i = 0; i < 20; i++)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)chaFileAccessory.parts[i].type, chaFileAccessory.parts[i].id);
				if (listInfo == null)
				{
					textDst[i].text = "なし";
				}
				else
				{
					textDst[i].text = listInfo.Name;
				}
			}
		}

		private void ChangeSrcDD()
		{
			ChaFileAccessory chaFileAccessory = chaCtrl.chaFile.coordinate[ddCoordeType[1].value].accessory;
			for (int i = 0; i < 20; i++)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)chaFileAccessory.parts[i].type, chaFileAccessory.parts[i].id);
				if (listInfo == null)
				{
					textSrc[i].text = "なし";
				}
				else
				{
					textSrc[i].text = listInfo.Name;
				}
			}
		}

		private void CopyAcs()
		{
			ChaFileAccessory chaFileAccessory = chaCtrl.chaFile.coordinate[ddCoordeType[0].value].accessory;
			ChaFileAccessory chaFileAccessory2 = chaCtrl.chaFile.coordinate[ddCoordeType[1].value].accessory;
			for (int i = 0; i < 20; i++)
			{
				if (tglKind[i].isOn)
				{
					byte[] bytes = MessagePackSerializer.Serialize(chaFileAccessory2.parts[i]);
					chaFileAccessory.parts[i] = MessagePackSerializer.Deserialize<ChaFileAccessory.PartsInfo>(bytes);
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
			Singleton<CustomBase>.Instance.actUpdateCvsAccessoryCopy += UpdateCustomUI;
			ddCoordeType[0].onValueChanged.AddListener(delegate
			{
				ChangeDstDD();
			});
			ddCoordeType[1].onValueChanged.AddListener(delegate
			{
				ChangeSrcDD();
			});
			btnAllOn.OnClickAsObservable().Subscribe(delegate
			{
				Toggle[] array = tglKind;
				foreach (Toggle toggle in array)
				{
					toggle.isOn = true;
				}
			});
			btnAllOff.OnClickAsObservable().Subscribe(delegate
			{
				Toggle[] array2 = tglKind;
				foreach (Toggle toggle2 in array2)
				{
					toggle2.isOn = false;
				}
			});
			btnCopy.OnClickAsObservable().Subscribe(delegate
			{
				CopyAcs();
			});
			Observable.EveryUpdate().Subscribe(delegate
			{
				btnCopy.interactable = ddCoordeType[0].value != ddCoordeType[1].value;
			}).AddTo(this);
		}
	}
}
