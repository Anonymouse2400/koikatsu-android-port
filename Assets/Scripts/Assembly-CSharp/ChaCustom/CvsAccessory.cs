using System;
using System.Collections.Generic;
using System.Linq;
using CustomUtility;
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
	[DefaultExecutionOrder(13000)]
	public class CvsAccessory : MonoBehaviour
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

		private int[] defaultAcsID = new int[11]
		{
			0, 0, 0, 0, 0, 0, 6, 0, 6, 0,
			0
		};

		[SerializeField]
		private TypeReactiveProperty _slotNo = new TypeReactiveProperty(AcsSlotNo.Slot01);

		[SerializeField]
		private CanvasGroup cgSlot;

		[SerializeField]
		private CvsDrawCtrl cmpDrawCtrl;

		[SerializeField]
		private TextMeshProUGUI textSlotName;

		[SerializeField]
		private CustomAcsChangeSlot cmpChgSlotCtrl;

		[SerializeField]
		private Toggle tglTakeOverParent;

		[SerializeField]
		private Toggle tglTakeOverColor;

		[SerializeField]
		private TMP_Dropdown ddAcsType;

		public Toggle tglAcsKind;

		[SerializeField]
		private Image imgAcsKind;

		[SerializeField]
		private TextMeshProUGUI textAcsKind;

		[SerializeField]
		private CustomAcsSelectKind[] customAccessory;

		[SerializeField]
		private CanvasGroup[] cgAccessoryWin;

		public Toggle tglAcsParent;

		[SerializeField]
		private TextMeshProUGUI textAcsParent;

		[SerializeField]
		private CustomAcsParentWindow cusAcsParentWin;

		[SerializeField]
		private CanvasGroup cgAcsParent;

		[SerializeField]
		private Button btnInitParent;

		[SerializeField]
		private Button btnReverseParent;

		[SerializeField]
		private GameObject separateColor;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnAcsColor01;

		[SerializeField]
		private Image imgAcsColor01;

		[SerializeField]
		private Button btnAcsColor02;

		[SerializeField]
		private Image imgAcsColor02;

		[SerializeField]
		private Button btnAcsColor03;

		[SerializeField]
		private Image imgAcsColor03;

		[SerializeField]
		private Button btnAcsColor04;

		[SerializeField]
		private Image imgAcsColor04;

		[SerializeField]
		private Button btnInitColor;

		[SerializeField]
		private GameObject separateCorrect;

		public Toggle tglAcsMove01;

		[SerializeField]
		private CustomAcsMoveWindow cusAcsMove01Win;

		[SerializeField]
		private CanvasGroup cgAcsMove01;

		[SerializeField]
		private GameObject objControllerTop01;

		[SerializeField]
		private Toggle tglDrawController01;

		[SerializeField]
		private Toggle[] tglControllerType01;

		[SerializeField]
		private Slider sldControllerSpeed01;

		[SerializeField]
		private Slider sldControllerScale01;

		public Toggle tglAcsMove02;

		[SerializeField]
		private CustomAcsMoveWindow cusAcsMove02Win;

		[SerializeField]
		private CanvasGroup cgAcsMove02;

		[SerializeField]
		private GameObject objControllerTop02;

		[SerializeField]
		private Toggle tglDrawController02;

		[SerializeField]
		private Toggle[] tglControllerType02;

		[SerializeField]
		private Slider sldControllerSpeed02;

		[SerializeField]
		private Slider sldControllerScale02;

		[SerializeField]
		private GameObject separateGroup;

		[SerializeField]
		private Toggle[] tglAcsGroup;

		[SerializeField]
		private GameObject objExplanation;

		private bool[] isDrag = new bool[2];

		private int[,] colorKind = new int[20, 4]
		{
			{ 124, 125, 126, 127 },
			{ 128, 129, 130, 131 },
			{ 132, 133, 134, 135 },
			{ 136, 137, 138, 139 },
			{ 140, 141, 142, 143 },
			{ 144, 145, 146, 147 },
			{ 148, 149, 150, 151 },
			{ 152, 153, 154, 155 },
			{ 156, 157, 158, 159 },
			{ 160, 161, 162, 163 },
			{ 164, 165, 166, 167 },
			{ 168, 169, 170, 171 },
			{ 172, 173, 174, 175 },
			{ 176, 177, 178, 179 },
			{ 180, 181, 182, 183 },
			{ 184, 185, 186, 187 },
			{ 188, 189, 190, 191 },
			{ 192, 193, 194, 195 },
			{ 196, 197, 198, 199 },
			{ 200, 201, 202, 203 }
		};

		private Dictionary<int, Data.Param> _acsParentLT;

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

		private int nSlotNo
		{
			get
			{
				return (int)slotNo;
			}
		}

		public bool isController01Active
		{
			get
			{
				return (bool)objControllerTop01 && objControllerTop01.activeSelf;
			}
		}

		public bool isController02Active
		{
			get
			{
				return (bool)objControllerTop02 && objControllerTop02.activeSelf;
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

		private CustomGuideObject[] cmpGuid
		{
			get
			{
				return Singleton<CustomBase>.Instance.customCtrl.cmpGuid;
			}
		}

		private Dictionary<int, Data.Param> acsParentLT
		{
			get
			{
				return this.GetCache(ref _acsParentLT, delegate
				{
					IEnumerable<Data.Param> second = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CUSTOM_UI).Get(80).Values.FindTags("AcsParent");
					Data.Param param = new Data.Param();
					return new Data.Param[1] { param }.Concat(second).Select((Data.Param d, int i) => new { d, i }).ToDictionary(v => v.i, v => v.d);
				});
			}
		}

		public void CalculateUI()
		{
			tglTakeOverParent.isOn = Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent;
			tglTakeOverColor.isOn = Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor;
			imgAcsColor01.color = accessory.parts[nSlotNo].color[0];
			imgAcsColor02.color = accessory.parts[nSlotNo].color[1];
			imgAcsColor03.color = accessory.parts[nSlotNo].color[2];
			imgAcsColor04.color = accessory.parts[nSlotNo].color[3];
			for (int i = 0; i < tglAcsGroup.Length; i++)
			{
				tglAcsGroup[i].isOn = i == accessory.parts[nSlotNo].hideCategory;
			}
			for (int j = 0; j < 2; j++)
			{
				UpdateDrawControllerState(j);
			}
		}

		public virtual void UpdateCustomUI()
		{
			int selectIndex = cmpChgSlotCtrl.GetSelectIndex();
			if (selectIndex >= 20)
			{
				return;
			}
			CalculateUI();
			cmpDrawCtrl.UpdateAccessoryDraw();
			int num = accessory.parts[selectIndex].type - 120;
			ddAcsType.value = num;
			UpdateAccessoryKindInfo();
			UpdateAccessoryParentInfo();
			UpdateAccessoryMoveInfo();
			ChangeSettingVisible(0 != num);
			bool flag = false;
			for (int i = 0; i < colorKind.GetLength(0); i++)
			{
				for (int j = 0; j < colorKind.GetLength(1); j++)
				{
					if (cvsColor.connectColorKind == (CvsColor.ConnectColorKind)colorKind[i, j])
					{
						cvsColor.SetColor(accessory.parts[i].color[j]);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}

		public void UpdateSlotName()
		{
			if (null == textSlotName)
			{
				return;
			}
			Singleton<CustomBase>.Instance.FontBind(textSlotName);
			if (accessory.parts[nSlotNo].type == 120)
			{
				string format = "スロット{0:00}";
				Singleton<CustomBase>.Instance.TranslateSlotTitle(0).SafeProc(delegate(string text)
				{
					format = text;
				});
				textSlotName.text = string.Format(format, nSlotNo + 1);
			}
			else
			{
				chaCtrl.infoAccessory.SafeProc(nSlotNo, delegate(ListInfoBase info)
				{
					textSlotName.text = info.Name;
				});
			}
		}

		public void UpdateSelectAccessoryType(int index)
		{
			if (accessory.parts[nSlotNo].type - 120 != index)
			{
				accessory.parts[nSlotNo].type = 120 + index;
				setAccessory.parts[nSlotNo].type = accessory.parts[nSlotNo].type;
				accessory.parts[nSlotNo].id = defaultAcsID[index];
				setAccessory.parts[nSlotNo].id = defaultAcsID[index];
				if (!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent)
				{
					accessory.parts[nSlotNo].parentKey = string.Empty;
				}
				FuncUpdateAccessory(false, false);
				setAccessory.parts[nSlotNo].parentKey = accessory.parts[nSlotNo].parentKey;
				for (int i = 0; i < 2; i++)
				{
					setAccessory.parts[nSlotNo].addMove[i, 0] = accessory.parts[nSlotNo].addMove[i, 0];
					setAccessory.parts[nSlotNo].addMove[i, 1] = accessory.parts[nSlotNo].addMove[i, 1];
					setAccessory.parts[nSlotNo].addMove[i, 2] = accessory.parts[nSlotNo].addMove[i, 2];
				}
				UpdateAccessoryKindInfo();
				UpdateAccessoryParentInfo();
				UpdateAccessoryMoveInfo();
				if (!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor)
				{
					SetDefaultColor();
				}
				Singleton<CustomHistory>.Instance.Add3(chaCtrl, FuncUpdateAccessory, false, true);
				UpdateSlotName();
			}
		}

		public void UpdateSelectAccessoryKind(string name, Sprite sp, int index)
		{
			if ((bool)textAcsKind)
			{
				textAcsKind.text = name;
			}
			if ((bool)imgAcsKind)
			{
				imgAcsKind.sprite = sp;
			}
			if (accessory.parts[nSlotNo].id != index)
			{
				accessory.parts[nSlotNo].id = index;
				setAccessory.parts[nSlotNo].id = index;
				Dictionary<int, ListInfoBase> dictionary = null;
				dictionary = chaCtrl.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)accessory.parts[nSlotNo].type);
				ListInfoBase value = null;
				dictionary.TryGetValue(accessory.parts[nSlotNo].id, out value);
				bool flag = value != null && 1 == value.GetInfoInt(ChaListDefine.KeyType.HideHair);
				if (!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent || flag)
				{
					accessory.parts[nSlotNo].parentKey = string.Empty;
				}
				FuncUpdateAccessory(!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor, false);
				setAccessory.parts[nSlotNo].parentKey = accessory.parts[nSlotNo].parentKey;
				UpdateAccessoryParentInfo();
				Singleton<CustomHistory>.Instance.Add3(chaCtrl, FuncUpdateAccessory, false, true);
			}
			ChangeUseColorVisible();
			ChangeParentAndMoveSettingVisible();
			UpdateSlotName();
		}

		public void UpdateSelectAccessoryParent(int index)
		{
			if ((bool)textAcsParent)
			{
				int key2 = index + 1;
				string value;
				ChaAccessoryDefine.dictAccessoryParent.TryGetValue(key2, out value);
				textAcsParent.text = value ?? string.Empty;
				Localize.Translate.Manager.Bind(textAcsParent, acsParentLT.Get(key2), true);
			}
			string[] array = (from key in Enum.GetNames(typeof(ChaAccessoryDefine.AccessoryParentKey))
				where key != "none"
				select key).ToArray();
			string text = array[index];
			if (accessory.parts[nSlotNo].parentKey != text)
			{
				accessory.parts[nSlotNo].parentKey = text;
				setAccessory.parts[nSlotNo].parentKey = text;
				FuncUpdateAcsParent(false);
				Singleton<CustomHistory>.Instance.Add2(chaCtrl, FuncUpdateAcsParent, true);
			}
		}

		public bool FuncUpdateAccessory(bool setDefaultColor, bool history)
		{
			if (history)
			{
				for (int i = 0; i < 20; i++)
				{
					chaCtrl.ChangeAccessory(i, accessory.parts[i].type, accessory.parts[i].id, accessory.parts[i].parentKey, true);
				}
			}
			else
			{
				chaCtrl.ChangeAccessory(nSlotNo, accessory.parts[nSlotNo].type, accessory.parts[nSlotNo].id, accessory.parts[nSlotNo].parentKey, true);
			}
			if (setDefaultColor)
			{
				SetDefaultColor();
			}
			return true;
		}

		public void SetDefaultColor()
		{
			Image[] array = new Image[4] { imgAcsColor01, imgAcsColor02, imgAcsColor03, imgAcsColor04 };
			for (int i = 0; i < 4; i++)
			{
				Color color = Color.white;
				chaCtrl.GetAccessoryDefaultColor(ref color, nSlotNo, i);
				accessory.parts[nSlotNo].color[i] = color;
				setAccessory.parts[nSlotNo].color[i] = color;
				array[i].color = color;
			}
			FuncUpdateAcsColor(false);
		}

		public void UpdateAccessoryKindInfo()
		{
			int num = ddAcsType.value - 1;
			if (0 <= num)
			{
				if (tglAcsKind.isOn)
				{
					for (int i = 0; i < cgAccessoryWin.Length; i++)
					{
						cgAccessoryWin[i].Enable(num == i);
					}
				}
				if (null != customAccessory[num])
				{
					customAccessory[num].UpdateCustomUI();
				}
			}
			else
			{
				tglAcsKind.isOn = false;
			}
		}

		public void UpdateAccessoryParentInfo()
		{
			int num = ddAcsType.value - 1;
			if (0 <= num)
			{
				if (tglAcsParent.isOn)
				{
					cgAcsParent.Enable(true);
				}
				if (null != cusAcsParentWin)
				{
					int key = cusAcsParentWin.UpdateCustomUI() + 1;
					if ((bool)textAcsParent)
					{
						string value;
						ChaAccessoryDefine.dictAccessoryParent.TryGetValue(key, out value);
						textAcsParent.text = value ?? string.Empty;
						Localize.Translate.Manager.Bind(textAcsParent, acsParentLT.Get(key), true);
					}
				}
			}
			else
			{
				tglAcsParent.isOn = false;
			}
		}

		public void UpdateAccessoryMoveInfo()
		{
			int num = ddAcsType.value - 1;
			if (0 <= num)
			{
				if (tglAcsMove01.isOn)
				{
					cgAcsMove01.Enable(true);
				}
				if (null != cusAcsMove01Win)
				{
					cusAcsMove01Win.UpdateCustomUI();
				}
				if (tglAcsMove02.isOn)
				{
					cgAcsMove02.Enable(true);
				}
				if (null != cusAcsMove02Win)
				{
					cusAcsMove02Win.UpdateCustomUI();
				}
			}
			else
			{
				tglAcsMove01.isOn = false;
				tglAcsMove02.isOn = false;
			}
		}

		public bool FuncUpdateAcsParent(bool history)
		{
			if (history)
			{
				for (int i = 0; i < 20; i++)
				{
					chaCtrl.ChangeAccessoryParent(i, accessory.parts[i].parentKey);
				}
				return true;
			}
			return chaCtrl.ChangeAccessoryParent(nSlotNo, accessory.parts[nSlotNo].parentKey);
		}

		public void UpdateAcsColor01(Color color)
		{
			accessory.parts[nSlotNo].color[0] = color;
			setAccessory.parts[nSlotNo].color[0] = color;
			imgAcsColor01.color = color;
			FuncUpdateAcsColor(false);
		}

		public void UpdateAcsColor02(Color color)
		{
			accessory.parts[nSlotNo].color[1] = color;
			setAccessory.parts[nSlotNo].color[1] = color;
			imgAcsColor02.color = color;
			FuncUpdateAcsColor(false);
		}

		public void UpdateAcsColor03(Color color)
		{
			accessory.parts[nSlotNo].color[2] = color;
			setAccessory.parts[nSlotNo].color[2] = color;
			imgAcsColor03.color = color;
			FuncUpdateAcsColor(false);
		}

		public void UpdateAcsColor04(Color color)
		{
			accessory.parts[nSlotNo].color[3] = color;
			setAccessory.parts[nSlotNo].color[3] = color;
			imgAcsColor04.color = color;
			FuncUpdateAcsColor(false);
		}

		public bool FuncUpdateAcsColor(bool history)
		{
			if (history)
			{
				for (int i = 0; i < 20; i++)
				{
					chaCtrl.ChangeAccessoryColor(i);
				}
			}
			else
			{
				chaCtrl.ChangeAccessoryColor(nSlotNo);
			}
			return true;
		}

		public void UpdateAcsColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add2(chaCtrl, FuncUpdateAcsColor, true);
		}

		public void FuncUpdateAcsPosAdd(int correctNo, int xyz, bool add, float val)
		{
			int[] array = new int[3] { 1, 2, 4 };
			chaCtrl.SetAccessoryPos(nSlotNo, correctNo, val, add, array[xyz]);
			setAccessory.parts[nSlotNo].addMove[correctNo, 0] = accessory.parts[nSlotNo].addMove[correctNo, 0];
		}

		public void FuncUpdateAcsRotAdd(int correctNo, int xyz, bool add, float val)
		{
			int[] array = new int[3] { 1, 2, 4 };
			chaCtrl.SetAccessoryRot(nSlotNo, correctNo, val, add, array[xyz]);
			setAccessory.parts[nSlotNo].addMove[correctNo, 1] = accessory.parts[nSlotNo].addMove[correctNo, 1];
		}

		public void FuncUpdateAcsSclAdd(int correctNo, int xyz, bool add, float val)
		{
			int[] array = new int[3] { 1, 2, 4 };
			chaCtrl.SetAccessoryScl(nSlotNo, correctNo, val, add, array[xyz]);
			setAccessory.parts[nSlotNo].addMove[correctNo, 2] = accessory.parts[nSlotNo].addMove[correctNo, 2];
		}

		public void FuncUpdateAcsMovePaste(int correctNo, Vector3[] value)
		{
			chaCtrl.SetAccessoryPos(nSlotNo, correctNo, value[0].x, false, 1);
			chaCtrl.SetAccessoryPos(nSlotNo, correctNo, value[0].y, false, 2);
			chaCtrl.SetAccessoryPos(nSlotNo, correctNo, value[0].z, false, 4);
			setAccessory.parts[nSlotNo].addMove[correctNo, 0] = accessory.parts[nSlotNo].addMove[correctNo, 0];
			chaCtrl.SetAccessoryRot(nSlotNo, correctNo, value[1].x, false, 1);
			chaCtrl.SetAccessoryRot(nSlotNo, correctNo, value[1].y, false, 2);
			chaCtrl.SetAccessoryRot(nSlotNo, correctNo, value[1].z, false, 4);
			setAccessory.parts[nSlotNo].addMove[correctNo, 1] = accessory.parts[nSlotNo].addMove[correctNo, 1];
			chaCtrl.SetAccessoryScl(nSlotNo, correctNo, value[2].x, false, 1);
			chaCtrl.SetAccessoryScl(nSlotNo, correctNo, value[2].y, false, 2);
			chaCtrl.SetAccessoryScl(nSlotNo, correctNo, value[2].z, false, 4);
			setAccessory.parts[nSlotNo].addMove[correctNo, 2] = accessory.parts[nSlotNo].addMove[correctNo, 2];
		}

		public void FuncUpdateAcsAllReset(int correctNo)
		{
			chaCtrl.ResetAccessoryMove(nSlotNo, correctNo);
			setAccessory.parts[nSlotNo].addMove[correctNo, 0] = accessory.parts[nSlotNo].addMove[correctNo, 0];
			setAccessory.parts[nSlotNo].addMove[correctNo, 1] = accessory.parts[nSlotNo].addMove[correctNo, 1];
			setAccessory.parts[nSlotNo].addMove[correctNo, 2] = accessory.parts[nSlotNo].addMove[correctNo, 2];
		}

		public void UpdateAcsMoveHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateAccessoryMoveAllFromInfo);
		}

		public void ChangeSettingVisible(bool visible)
		{
			tglAcsKind.transform.parent.gameObject.SetActiveIfDifferent(visible);
			ChangeParentAndMoveSettingVisible();
			ChangeUseColorVisible();
			btnInitColor.transform.parent.gameObject.SetActiveIfDifferent(visible);
			separateGroup.SetActiveIfDifferent(visible);
			tglAcsGroup[0].transform.parent.gameObject.SetActiveIfDifferent(visible);
			objExplanation.SetActiveIfDifferent(visible);
		}

		public void ChangeParentAndMoveSettingVisible()
		{
			bool flag = true;
			if (ddAcsType.value == 0)
			{
				flag = false;
			}
			Dictionary<int, ListInfoBase> dictionary = null;
			dictionary = chaCtrl.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)accessory.parts[nSlotNo].type);
			ListInfoBase value = null;
			dictionary.TryGetValue(accessory.parts[nSlotNo].id, out value);
			if (value != null && (value.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1 || "null" == value.GetInfo(ChaListDefine.KeyType.Parent)))
			{
				flag = false;
			}
			tglAcsParent.transform.parent.gameObject.SetActiveIfDifferent(flag);
			btnInitParent.transform.parent.gameObject.SetActiveIfDifferent(flag);
			btnReverseParent.transform.parent.gameObject.SetActiveIfDifferent(flag);
			separateCorrect.SetActiveIfDifferent(flag);
			tglAcsMove01.transform.parent.gameObject.SetActiveIfDifferent(flag);
			tglAcsMove02.transform.parent.gameObject.SetActiveIfDifferent(!(null == chaCtrl.objAcsMove[nSlotNo, 1]) && flag);
			if ((bool)objControllerTop01)
			{
				objControllerTop01.SetActiveIfDifferent(flag);
			}
			if ((bool)objControllerTop02)
			{
				objControllerTop02.SetActiveIfDifferent(!(null == chaCtrl.objAcsMove[nSlotNo, 1]) && flag);
			}
		}

		public void ChangeUseColorVisible()
		{
			bool[] array = new bool[4];
			bool active = false;
			if (ddAcsType.value != 0 && null != chaCtrl.cusAcsCmp[nSlotNo])
			{
				if (chaCtrl.cusAcsCmp[nSlotNo].useColor01)
				{
					array[0] = true;
					active = true;
				}
				if (chaCtrl.cusAcsCmp[nSlotNo].useColor02)
				{
					array[1] = true;
					active = true;
				}
				if (chaCtrl.cusAcsCmp[nSlotNo].useColor03)
				{
					array[2] = true;
					active = true;
				}
				if (chaCtrl.cusAcsCmp[nSlotNo].rendAlpha != null && 0 < chaCtrl.cusAcsCmp[nSlotNo].rendAlpha.Length)
				{
					array[3] = true;
					active = true;
				}
			}
			separateColor.SetActiveIfDifferent(active);
			btnAcsColor01.transform.parent.gameObject.SetActiveIfDifferent(array[0]);
			btnAcsColor02.transform.parent.gameObject.SetActiveIfDifferent(array[1]);
			btnAcsColor03.transform.parent.gameObject.SetActiveIfDifferent(array[2]);
			btnAcsColor04.transform.parent.gameObject.SetActiveIfDifferent(array[3]);
		}

		public void SetDefaultColorWindow(int no)
		{
			string title = string.Format("スロット{0:00} カラー①", no + 1);
			Singleton<CustomBase>.Instance.TranslateSlotTitleWithColor(0).SafeProc(delegate(string text)
			{
				title = string.Format(text, no + 1);
			});
			cvsColor.Setup(title, (CvsColor.ConnectColorKind)colorKind[no, 0], accessory.parts[no].color[0], UpdateAcsColor01, UpdateAcsColorHistory, false);
		}

		public void SetControllerTransform(int guidNo)
		{
			GameObject gameObject = chaCtrl.objAcsMove[nSlotNo, guidNo];
			if (!(null == gameObject))
			{
				cmpGuid[guidNo].amount.position = gameObject.transform.position;
				cmpGuid[guidNo].amount.rotation = gameObject.transform.eulerAngles;
			}
		}

		public void SetAccessoryTransform(int guidNo, bool updateInfo)
		{
			GameObject gameObject = chaCtrl.objAcsMove[nSlotNo, guidNo];
			if (null == gameObject || tglControllerType01 == null || tglControllerType01.Length == 0 || tglControllerType02 == null || tglControllerType02.Length == 0)
			{
				return;
			}
			Toggle[] array = new Toggle[2]
			{
				(guidNo != 0) ? tglControllerType02[0] : tglControllerType01[0],
				(guidNo != 0) ? tglControllerType02[1] : tglControllerType01[1]
			};
			if (array[0].isOn)
			{
				gameObject.transform.position = cmpGuid[guidNo].amount.position;
				if (updateInfo)
				{
					Vector3 localPosition = gameObject.transform.localPosition;
					localPosition.x = Mathf.Clamp(localPosition.x * 100f, -100f, 100f);
					localPosition.y = Mathf.Clamp(localPosition.y * 100f, -100f, 100f);
					localPosition.z = Mathf.Clamp(localPosition.z * 100f, -100f, 100f);
					chaCtrl.SetAccessoryPos(nSlotNo, guidNo, localPosition.x, false, 1);
					chaCtrl.SetAccessoryPos(nSlotNo, guidNo, localPosition.y, false, 2);
					chaCtrl.SetAccessoryPos(nSlotNo, guidNo, localPosition.z, false, 4);
					setAccessory.parts[nSlotNo].addMove[guidNo, 0] = accessory.parts[nSlotNo].addMove[guidNo, 0];
					chaCtrl.UpdateAccessoryMoveFromInfo(nSlotNo);
					cmpGuid[guidNo].amount.position = gameObject.transform.position;
				}
			}
			else
			{
				gameObject.transform.eulerAngles = cmpGuid[guidNo].amount.rotation;
				if (updateInfo)
				{
					Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
					chaCtrl.SetAccessoryRot(nSlotNo, guidNo, localEulerAngles.x, false, 1);
					chaCtrl.SetAccessoryRot(nSlotNo, guidNo, localEulerAngles.y, false, 2);
					chaCtrl.SetAccessoryRot(nSlotNo, guidNo, localEulerAngles.z, false, 4);
					setAccessory.parts[nSlotNo].addMove[guidNo, 1] = accessory.parts[nSlotNo].addMove[guidNo, 1];
					chaCtrl.UpdateAccessoryMoveFromInfo(nSlotNo);
					cmpGuid[guidNo].amount.rotation = gameObject.transform.eulerAngles;
				}
			}
			UpdateCustomUI();
		}

		public void UpdateDrawControllerState(int guidNo)
		{
			if (tglControllerType01 == null || tglControllerType01.Length == 0 || tglControllerType02 == null || tglControllerType02.Length == 0)
			{
				return;
			}
			Toggle toggle = ((guidNo != 0) ? tglDrawController02 : tglDrawController01);
			Toggle[] array = new Toggle[2]
			{
				(guidNo != 0) ? tglControllerType02[0] : tglControllerType01[0],
				(guidNo != 0) ? tglControllerType02[1] : tglControllerType01[1]
			};
			Slider slider = ((guidNo != 0) ? sldControllerSpeed02 : sldControllerSpeed01);
			Slider slider2 = ((guidNo != 0) ? sldControllerScale02 : sldControllerScale01);
			toggle.isOn = Singleton<CustomBase>.Instance.customSettingSave.drawController[guidNo];
			if (Singleton<CustomBase>.Instance.customSettingSave.controllerType[guidNo] == 0)
			{
				if ((bool)array[0])
				{
					array[0].isOn = true;
				}
				if ((bool)array[1])
				{
					array[1].isOn = false;
				}
			}
			else
			{
				if ((bool)array[0])
				{
					array[0].isOn = false;
				}
				if ((bool)array[1])
				{
					array[1].isOn = true;
				}
			}
			slider.value = Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[guidNo];
			slider2.value = Singleton<CustomBase>.Instance.customSettingSave.controllerScale[guidNo];
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
            // Initialize variables properly before use
            int num = nSlotNo;
            Action[] actUpdateCvsAccessory = Singleton<CustomBase>.Instance.actUpdateCvsAccessory; // Assign reference

            // Ensure actUpdateCvsAccessory is not null and num is within bounds
            if (actUpdateCvsAccessory != null && num >= 0 && num < actUpdateCvsAccessory.Length)
            {
                actUpdateCvsAccessory[num] = (Action)Delegate.Combine(actUpdateCvsAccessory[num], new Action(UpdateCustomUI));
            }

            // Repeat the process for num2
            int num2 = nSlotNo;
            actUpdateCvsAccessory = Singleton<CustomBase>.Instance.actUpdateAcsSlotName; // Assign reference

            if (actUpdateCvsAccessory != null && num2 >= 0 && num2 < actUpdateCvsAccessory.Length)
            {
                actUpdateCvsAccessory[num2] = (Action)Delegate.Combine(actUpdateCvsAccessory[num2], new Action(UpdateSlotName));
            }

            tglTakeOverParent.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent = isOn;
			});
			tglTakeOverColor.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor = isOn;
			});
			ddAcsType.onValueChanged.AddListener(delegate(int idx)
			{
				UpdateSelectAccessoryType(idx);
				bool visible = idx != 0;
				ChangeSettingVisible(visible);
			});
			tglAcsKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				int num3 = ddAcsType.value - 1;
				if (0 <= num3)
				{
					if ((bool)cgAccessoryWin[num3])
					{
						bool flag = ((cgAccessoryWin[num3].alpha != 0f) ? true : false);
						if (flag != isOn)
						{
							cgAccessoryWin[num3].Enable(isOn);
							if (isOn)
							{
								tglAcsParent.isOn = false;
								tglAcsMove01.isOn = false;
								tglAcsMove02.isOn = false;
								for (int i = 0; i < cgAccessoryWin.Length; i++)
								{
									if (i != num3)
									{
										cgAccessoryWin[i].Enable(false);
									}
								}
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < cgAccessoryWin.Length; j++)
					{
						cgAccessoryWin[j].Enable(false);
					}
				}
			});
			tglAcsParent.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgAcsParent)
				{
					bool flag2 = ((cgAcsParent.alpha != 0f) ? true : false);
					if (flag2 != isOn)
					{
						cgAcsParent.Enable(isOn);
						if (isOn)
						{
							tglAcsKind.isOn = false;
							tglAcsMove01.isOn = false;
							tglAcsMove02.isOn = false;
						}
					}
				}
			});
			btnInitParent.onClick.AsObservable().Subscribe(delegate
			{
				string accessoryDefaultParentStr = chaCtrl.GetAccessoryDefaultParentStr(nSlotNo);
				accessory.parts[nSlotNo].parentKey = accessoryDefaultParentStr;
				setAccessory.parts[nSlotNo].parentKey = accessoryDefaultParentStr;
				FuncUpdateAcsParent(false);
				Singleton<CustomHistory>.Instance.Add2(chaCtrl, FuncUpdateAcsParent, true);
				UpdateCustomUI();
			});
			btnReverseParent.onClick.AsObservable().Subscribe(delegate
			{
				string reverseParent = ChaAccessoryDefine.GetReverseParent(accessory.parts[nSlotNo].parentKey);
				if (string.Empty != reverseParent)
				{
					accessory.parts[nSlotNo].parentKey = reverseParent;
					setAccessory.parts[nSlotNo].parentKey = reverseParent;
					FuncUpdateAcsParent(false);
					Singleton<CustomHistory>.Instance.Add2(chaCtrl, FuncUpdateAcsParent, true);
					UpdateCustomUI();
				}
			});
			btnAcsColor01.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == (CvsColor.ConnectColorKind)colorKind[nSlotNo, 0])
				{
					cvsColor.Close();
				}
				else
				{
					string title = string.Format("スロット{0:00} カラー①", nSlotNo + 1);
					Singleton<CustomBase>.Instance.TranslateSlotTitleWithColor(0).SafeProc(delegate(string text)
					{
						title = string.Format(text, nSlotNo + 1);
					});
					cvsColor.Setup(title, (CvsColor.ConnectColorKind)colorKind[nSlotNo, 0], accessory.parts[nSlotNo].color[0], UpdateAcsColor01, UpdateAcsColorHistory, false);
				}
			});
			btnAcsColor02.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == (CvsColor.ConnectColorKind)colorKind[nSlotNo, 1])
				{
					cvsColor.Close();
				}
				else
				{
					string title2 = string.Format("スロット{0:00} カラー②", nSlotNo + 1);
					Singleton<CustomBase>.Instance.TranslateSlotTitleWithColor(1).SafeProc(delegate(string text)
					{
						title2 = string.Format(text, nSlotNo + 1);
					});
					cvsColor.Setup(title2, (CvsColor.ConnectColorKind)colorKind[nSlotNo, 1], accessory.parts[nSlotNo].color[1], UpdateAcsColor02, UpdateAcsColorHistory, false);
				}
			});
			btnAcsColor03.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == (CvsColor.ConnectColorKind)colorKind[nSlotNo, 2])
				{
					cvsColor.Close();
				}
				else
				{
					string title3 = string.Format("スロット{0:00} カラー③", nSlotNo + 1);
					Singleton<CustomBase>.Instance.TranslateSlotTitleWithColor(2).SafeProc(delegate(string text)
					{
						title3 = string.Format(text, nSlotNo + 1);
					});
					cvsColor.Setup(title3, (CvsColor.ConnectColorKind)colorKind[nSlotNo, 2], accessory.parts[nSlotNo].color[2], UpdateAcsColor03, UpdateAcsColorHistory, false);
				}
			});
			btnAcsColor04.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == (CvsColor.ConnectColorKind)colorKind[nSlotNo, 3])
				{
					cvsColor.Close();
				}
				else
				{
					string title4 = string.Format("スロット{0:00} カラー④", nSlotNo + 1);
					Singleton<CustomBase>.Instance.TranslateSlotTitleWithColor(3).SafeProc(delegate(string text)
					{
						title4 = string.Format(text, nSlotNo + 1);
					});
					cvsColor.Setup(title4, (CvsColor.ConnectColorKind)colorKind[nSlotNo, 3], accessory.parts[nSlotNo].color[3], UpdateAcsColor04, UpdateAcsColorHistory, true);
				}
			});
			btnInitColor.onClick.AsObservable().Subscribe(delegate
			{
				SetDefaultColor();
				UpdateAcsColorHistory();
				UpdateCustomUI();
			});
			tglAcsMove01.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgAcsMove01)
				{
					bool flag3 = ((cgAcsMove01.alpha != 0f) ? true : false);
					if (flag3 != isOn)
					{
						cgAcsMove01.Enable(isOn);
						if (isOn)
						{
							tglAcsKind.isOn = false;
							tglAcsParent.isOn = false;
							tglAcsMove02.isOn = false;
						}
					}
				}
			});
			tglAcsMove02.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgAcsMove02)
				{
					bool flag4 = ((cgAcsMove02.alpha != 0f) ? true : false);
					if (flag4 != isOn)
					{
						cgAcsMove02.Enable(isOn);
						if (isOn)
						{
							tglAcsKind.isOn = false;
							tglAcsParent.isOn = false;
							tglAcsMove01.isOn = false;
						}
					}
				}
			});
			(from item in tglAcsGroup.Select((Toggle tgl, int idx) => new { tgl, idx })
				where item.tgl != null
				select item).ToList().ForEach(item =>
			{
				(from isOn in item.tgl.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					if (!Singleton<CustomBase>.Instance.GetUpdateCvsAccessory(nSlotNo) && item.idx != accessory.parts[nSlotNo].hideCategory)
					{
						accessory.parts[nSlotNo].hideCategory = item.idx;
						setAccessory.parts[nSlotNo].hideCategory = item.idx;
						cmpDrawCtrl.UpdateAccessoryDraw();
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
					}
				});
			});
			if ((bool)tglDrawController01)
			{
				tglDrawController01.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI)
					{
						Singleton<CustomBase>.Instance.customSettingSave.drawController[0] = isOn;
					}
				});
			}
			if (tglControllerType01 != null)
			{
				tglControllerType01.Select((Toggle p, int idx) => new
				{
					toggle = p,
					index = (byte)idx
				}).ToList().ForEach(p =>
				{
					p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
					{
						if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
						{
							Singleton<CustomBase>.Instance.customSettingSave.controllerType[0] = p.index;
							cmpGuid[0].SetMode(p.index);
						}
					});
				});
			}
			if ((bool)sldControllerSpeed01)
			{
				sldControllerSpeed01.onValueChanged.AsObservable().Subscribe(delegate(float val)
				{
					Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[0] = val;
					cmpGuid[0].speedMove = val;
				});
				sldControllerSpeed01.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldControllerSpeed01.value = Mathf.Clamp(sldControllerSpeed01.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.01f, 1f);
				});
			}
			if ((bool)sldControllerScale01)
			{
				sldControllerScale01.onValueChanged.AsObservable().Subscribe(delegate(float val)
				{
					Singleton<CustomBase>.Instance.customSettingSave.controllerScale[0] = val;
					cmpGuid[0].scaleAxis = val;
					cmpGuid[0].UpdateScale();
				});
				sldControllerScale01.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldControllerScale01.value = Mathf.Clamp(sldControllerScale01.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.3f, 3f);
				});
			}
			if ((bool)tglDrawController02)
			{
				tglDrawController02.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI)
					{
						Singleton<CustomBase>.Instance.customSettingSave.drawController[1] = isOn;
					}
				});
			}
			if (tglControllerType02 != null)
			{
				tglControllerType02.Select((Toggle p, int idx) => new
				{
					toggle = p,
					index = (byte)idx
				}).ToList().ForEach(p =>
				{
					p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
					{
						if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
						{
							Singleton<CustomBase>.Instance.customSettingSave.controllerType[1] = p.index;
							cmpGuid[1].SetMode(p.index);
						}
					});
				});
			}
			if ((bool)sldControllerSpeed02)
			{
				sldControllerSpeed02.onValueChanged.AsObservable().Subscribe(delegate(float val)
				{
					Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[1] = val;
					cmpGuid[1].speedMove = val;
				});
				sldControllerSpeed02.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldControllerSpeed02.value = Mathf.Clamp(sldControllerSpeed02.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.01f, 1f);
				});
			}
			if ((bool)sldControllerScale02)
			{
				sldControllerScale02.onValueChanged.AsObservable().Subscribe(delegate(float val)
				{
					Singleton<CustomBase>.Instance.customSettingSave.controllerScale[1] = val;
					cmpGuid[1].scaleAxis = val;
					cmpGuid[1].UpdateScale();
				});
				sldControllerScale02.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldControllerScale02.value = Mathf.Clamp(sldControllerScale02.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.3f, 3f);
				});
			}
		}

		private void LateUpdate()
		{
			if (cgSlot.alpha != 1f)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (!(null == cmpGuid[i]))
				{
					if (cmpGuid[i].isDrag)
					{
						SetAccessoryTransform(i, false);
					}
					else if (isDrag[i])
					{
						SetAccessoryTransform(i, true);
					}
					else
					{
						SetControllerTransform(i);
					}
					isDrag[i] = cmpGuid[i].isDrag;
				}
			}
		}
	}
}
