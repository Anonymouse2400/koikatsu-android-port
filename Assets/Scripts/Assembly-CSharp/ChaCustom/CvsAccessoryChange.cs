using System.Linq;
using MessagePack;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsAccessoryChange : MonoBehaviour
	{
		[SerializeField]
		private CustomAcsChangeSlot cmpAcsChangeSlot;

		[SerializeField]
		private Toggle[] tglSrcKind;

		[SerializeField]
		private Toggle[] tglDstKind;

		[SerializeField]
		private TextMeshProUGUI[] textSrc;

		[SerializeField]
		private TextMeshProUGUI[] textDst;

		[SerializeField]
		private Toggle tglReverse;

		[SerializeField]
		private Button btnSlotCopy;

		[SerializeField]
		private Button[] btnCopyValue;

		[SerializeField]
		private Button[] btnCopyRevValueLR;

		[SerializeField]
		private Button[] btnCopyRevValueUD;

		private int selSrc;

		private int selDst;

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

		public void CalculateUI()
		{
			for (int i = 0; i < 20; i++)
			{
				ListInfoBase listInfo = chaCtrl.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)accessory.parts[i].type, accessory.parts[i].id);
				if (listInfo == null)
				{
					textDst[i].text = "なし";
					textSrc[i].text = "なし";
				}
				else
				{
					textDst[i].text = listInfo.Name;
					textSrc[i].text = listInfo.Name;
				}
			}
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		private void CopyAcs()
		{
			byte[] bytes = MessagePackSerializer.Serialize(accessory.parts[selSrc]);
			accessory.parts[selDst] = MessagePackSerializer.Deserialize<ChaFileAccessory.PartsInfo>(bytes);
			if (tglReverse.isOn)
			{
				string reverseParent = ChaAccessoryDefine.GetReverseParent(accessory.parts[selDst].parentKey);
				if (string.Empty != reverseParent)
				{
					accessory.parts[selDst].parentKey = reverseParent;
				}
			}
			chaCtrl.AssignCoordinate((ChaFileDefine.CoordinateType)chaCtrl.fileStatus.coordinateType);
			chaCtrl.Reload(false, true, true, true);
			CalculateUI();
			cmpAcsChangeSlot.UpdateSlotNames();
			Singleton<CustomBase>.Instance.SetUpdateCvsAccessory(selDst, true);
			Singleton<CustomHistory>.Instance.Add5(chaCtrl, chaCtrl.Reload, false, true, true, true);
		}

		private void CopyAcsCorrect(int correctNo)
		{
			for (int i = 0; i < 3; i++)
			{
				accessory.parts[selDst].addMove[correctNo, i] = (setAccessory.parts[selDst].addMove[correctNo, i] = accessory.parts[selSrc].addMove[correctNo, i]);
			}
			chaCtrl.UpdateAccessoryMoveFromInfo(selDst);
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateAccessoryMoveAllFromInfo);
		}

		private void CopyAcsCorrectRevLR(int correctNo)
		{
			for (int i = 0; i < 3; i++)
			{
				Vector3 vector = accessory.parts[selSrc].addMove[correctNo, i];
				if (i == 1)
				{
					vector.y += 180f;
					if (vector.y >= 360f)
					{
						vector.y -= 360f;
					}
				}
				accessory.parts[selDst].addMove[correctNo, i] = (setAccessory.parts[selDst].addMove[correctNo, i] = vector);
			}
			chaCtrl.UpdateAccessoryMoveFromInfo(selDst);
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateAccessoryMoveAllFromInfo);
		}

		private void CopyAcsCorrectRevUD(int correctNo)
		{
			for (int i = 0; i < 3; i++)
			{
				Vector3 vector = accessory.parts[selSrc].addMove[correctNo, i];
				if (i == 1)
				{
					vector.x += 180f;
					if (vector.x >= 360f)
					{
						vector.x -= 360f;
					}
				}
				accessory.parts[selDst].addMove[correctNo, i] = (setAccessory.parts[selDst].addMove[correctNo, i] = vector);
			}
			chaCtrl.UpdateAccessoryMoveFromInfo(selDst);
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateAccessoryMoveAllFromInfo);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.actUpdateCvsAccessoryChange += UpdateCustomUI;
			tglSrcKind.Select((Toggle p, int index) => new
			{
				tgl = p,
				index = index
			}).ToList().ForEach(p =>
			{
				(from isOn in p.tgl.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					selSrc = p.index;
				});
			});
			tglDstKind.Select((Toggle p, int index) => new
			{
				tgl = p,
				index = index
			}).ToList().ForEach(p =>
			{
				(from isOn in p.tgl.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					selDst = p.index;
				});
			});
			btnSlotCopy.OnClickAsObservable().Subscribe(delegate
			{
				CopyAcs();
			});
			btnCopyValue.Select((Button p, int index) => new
			{
				btn = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					CopyAcsCorrect(p.index);
				});
			});
			btnCopyRevValueLR.Select((Button p, int index) => new
			{
				btn = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					CopyAcsCorrectRevLR(p.index);
				});
			});
			btnCopyRevValueUD.Select((Button p, int index) => new
			{
				btn = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					CopyAcsCorrectRevUD(p.index);
				});
			});
			Observable.EveryUpdate().Subscribe(delegate
			{
				btnSlotCopy.interactable = selDst != selSrc;
			}).AddTo(this);
		}
	}
}
