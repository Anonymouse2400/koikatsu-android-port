using System;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomAcsMoveWindow : MonoBehaviour
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

		private readonly float[] movePosValue = new float[3] { 0.1f, 1f, 10f };

		private readonly float[] moveRotValue = new float[3] { 1f, 5f, 10f };

		private readonly float[] moveSclValue = new float[3] { 0.01f, 0.1f, 1f };

		[SerializeField]
		private TypeReactiveProperty _slotNo = new TypeReactiveProperty(AcsSlotNo.Slot01);

		public int correctNo;

		[SerializeField]
		private TextMeshProUGUI textTitle;

		[SerializeField]
		private Toggle[] tglPosRate;

		[SerializeField]
		private Button[] btnPos;

		[SerializeField]
		private TMP_InputField[] inpPos;

		[SerializeField]
		private Button[] btnPosReset;

		[SerializeField]
		private Toggle[] tglRotRate;

		[SerializeField]
		private Button[] btnRot;

		[SerializeField]
		private TMP_InputField[] inpRot;

		[SerializeField]
		private Button[] btnRotReset;

		[SerializeField]
		private Toggle[] tglSclRate;

		[SerializeField]
		private Button[] btnScl;

		[SerializeField]
		private TMP_InputField[] inpScl;

		[SerializeField]
		private Button[] btnSclReset;

		[SerializeField]
		private Button btnCopy;

		[SerializeField]
		private Button btnPaste;

		[SerializeField]
		private Button btnAllReset;

		[SerializeField]
		private Button btnClose;

		[SerializeField]
		private Toggle tglReference;

		[SerializeField]
		private CvsAccessory[] cvsAccessory;

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

		public int nSlotNo
		{
			get
			{
				return (int)_slotNo.Value;
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
			if ((bool)textTitle)
			{
				Singleton<CustomBase>.Instance.FontBind(textTitle);
				string format = "スロット{0:00}の調整{1:00}";
				Singleton<CustomBase>.Instance.TranslateSlotTitle(2).SafeProc(delegate(string text)
				{
					format = text;
				});
				textTitle.text = string.Format(format, nSlotNo + 1, correctNo + 1);
			}
			int num = Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[correctNo];
			int num2 = Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[correctNo];
			int num3 = Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[correctNo];
			for (int i = 0; i < 3; i++)
			{
				tglPosRate[i].isOn = ((i == num) ? true : false);
				tglRotRate[i].isOn = ((i == num2) ? true : false);
				tglSclRate[i].isOn = ((i == num3) ? true : false);
			}
			UpdateCustomUI();
		}

		public void ChangeSlot(int _no, bool open)
		{
			slotNo = (AcsSlotNo)_no;
			bool isOn = tglReference.isOn;
			tglReference.isOn = false;
			if (correctNo == 0)
			{
				tglReference = cvsAccessory[nSlotNo].tglAcsMove01;
			}
			else
			{
				tglReference = cvsAccessory[nSlotNo].tglAcsMove02;
			}
			if (open && isOn)
			{
				tglReference.isOn = true;
			}
		}

		public void CloseWindow()
		{
			tglReference.isOn = false;
		}

		public void UpdateCustomUI(int param = 0)
		{
			for (int i = 0; i < 3; i++)
			{
				inpPos[i].text = accessory.parts[nSlotNo].addMove[correctNo, 0][i].ToString();
				inpRot[i].text = accessory.parts[nSlotNo].addMove[correctNo, 1][i].ToString();
				inpScl[i].text = accessory.parts[nSlotNo].addMove[correctNo, 2][i].ToString();
			}
		}

		public void UpdateDragValue(int type, int xyz, float move)
		{
			switch (type)
			{
			case 0:
			{
				float val3 = move * movePosValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[correctNo]];
				cvsAccessory[nSlotNo].FuncUpdateAcsPosAdd(correctNo, xyz, true, val3);
				inpPos[xyz].text = accessory.parts[nSlotNo].addMove[correctNo, 0][xyz].ToString();
				break;
			}
			case 1:
			{
				float val2 = move * moveRotValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[correctNo]];
				cvsAccessory[nSlotNo].FuncUpdateAcsRotAdd(correctNo, xyz, true, val2);
				inpRot[xyz].text = accessory.parts[nSlotNo].addMove[correctNo, 1][xyz].ToString();
				break;
			}
			case 2:
			{
				float val = move * moveSclValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[correctNo]];
				cvsAccessory[nSlotNo].FuncUpdateAcsSclAdd(correctNo, xyz, true, val);
				inpScl[xyz].text = accessory.parts[nSlotNo].addMove[correctNo, 2][xyz].ToString();
				break;
			}
			}
			cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
		}

		public void UpdateHistory()
		{
			cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			for (int i = 0; i < 3; i++)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPos[i]);
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpRot[i]);
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpScl[i]);
			}
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
			tglPosRate.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				(from isOn in p.toggle.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[correctNo] = p.index;
				});
			});
			tglRotRate.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				(from isOn in p.toggle.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[correctNo] = p.index;
				});
			});
			tglSclRate.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				(from isOn in p.toggle.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[correctNo] = p.index;
				});
			});
			float downTimeCnt = 0f;
			float loopTimeCnt = 0f;
			bool change = false;
			btnPos.Select((Button p, int idx) => new
			{
				btn = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					if (!change)
					{
						int num = p.index / 2;
						int num2 = ((p.index % 2 != 0) ? 1 : (-1));
						if (num == 0)
						{
							num2 *= -1;
						}
						float val = (float)num2 * movePosValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[correctNo]];
						cvsAccessory[nSlotNo].FuncUpdateAcsPosAdd(correctNo, num, true, val);
						cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
						inpPos[num].text = accessory.parts[nSlotNo].addMove[correctNo, 0][num].ToString();
						cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
					}
				});
				p.btn.UpdateAsObservable().SkipUntil(p.btn.OnPointerDownAsObservable().Do(delegate
				{
					downTimeCnt = 0f;
					loopTimeCnt = 0f;
					change = false;
				})).TakeUntil(p.btn.OnPointerUpAsObservable().Do(delegate
				{
					if (change)
					{
						cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					}
				}))
					.RepeatUntilDestroy(this)
					.Subscribe(delegate
					{
						int num3 = p.index / 2;
						int num4 = ((p.index % 2 != 0) ? 1 : (-1));
						if (num3 == 0)
						{
							num4 *= -1;
						}
						float num5 = (float)num4 * movePosValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[correctNo]];
						float num6 = 0f;
						downTimeCnt += Time.deltaTime;
						if (downTimeCnt > 0.3f)
						{
							for (loopTimeCnt += Time.deltaTime; loopTimeCnt > 0.05f; loopTimeCnt -= 0.05f)
							{
								num6 += num5;
							}
							if (num6 != 0f)
							{
								cvsAccessory[nSlotNo].FuncUpdateAcsPosAdd(correctNo, num3, true, num6);
								inpPos[num3].text = accessory.parts[nSlotNo].addMove[correctNo, 0][num3].ToString();
								change = true;
								cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
							}
						}
					})
					.AddTo(this);
			});
			inpPos.Select((TMP_InputField p, int idx) => new
			{
				inp = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.inp.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					int xyz = p.index % 3;
					float val2 = CustomBase.ConvertValueFromTextLimit(-100f, 100f, 1, value);
					p.inp.text = val2.ToString();
					cvsAccessory[nSlotNo].FuncUpdateAcsPosAdd(correctNo, xyz, false, val2);
					cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
				});
			});
			btnPosReset.Select((Button p, int idx) => new
			{
				btn = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					inpPos[p.index].text = "0";
					cvsAccessory[nSlotNo].FuncUpdateAcsPosAdd(correctNo, p.index, false, 0f);
					cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
				});
			});
			btnRot.Select((Button p, int idx) => new
			{
				btn = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					if (!change)
					{
						int num7 = p.index / 2;
						int num8 = ((p.index % 2 != 0) ? 1 : (-1));
						float val3 = (float)num8 * moveRotValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[correctNo]];
						cvsAccessory[nSlotNo].FuncUpdateAcsRotAdd(correctNo, num7, true, val3);
						cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
						inpRot[num7].text = accessory.parts[nSlotNo].addMove[correctNo, 1][num7].ToString();
						cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
					}
				});
				p.btn.UpdateAsObservable().SkipUntil(p.btn.OnPointerDownAsObservable().Do(delegate
				{
					downTimeCnt = 0f;
					loopTimeCnt = 0f;
					change = false;
				})).TakeUntil(p.btn.OnPointerUpAsObservable().Do(delegate
				{
					if (change)
					{
						cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					}
				}))
					.RepeatUntilDestroy(this)
					.Subscribe(delegate
					{
						int num9 = p.index / 2;
						int num10 = ((p.index % 2 != 0) ? 1 : (-1));
						float num11 = (float)num10 * moveRotValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[correctNo]];
						float num12 = 0f;
						downTimeCnt += Time.deltaTime;
						if (downTimeCnt > 0.3f)
						{
							for (loopTimeCnt += Time.deltaTime; loopTimeCnt > 0.05f; loopTimeCnt -= 0.05f)
							{
								num12 += num11;
							}
							if (num12 != 0f)
							{
								cvsAccessory[nSlotNo].FuncUpdateAcsRotAdd(correctNo, num9, true, num12);
								inpRot[num9].text = accessory.parts[nSlotNo].addMove[correctNo, 1][num9].ToString();
								change = true;
								cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
							}
						}
					})
					.AddTo(this);
			});
			inpRot.Select((TMP_InputField p, int idx) => new
			{
				inp = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.inp.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					int xyz2 = p.index % 3;
					float val4 = CustomBase.ConvertValueFromTextLimit(0f, 360f, 0, value);
					p.inp.text = val4.ToString();
					cvsAccessory[nSlotNo].FuncUpdateAcsRotAdd(correctNo, xyz2, false, val4);
					cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
				});
			});
			btnRotReset.Select((Button p, int idx) => new
			{
				btn = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					inpRot[p.index].text = "0";
					cvsAccessory[nSlotNo].FuncUpdateAcsRotAdd(correctNo, p.index, false, 0f);
					cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
				});
			});
			btnScl.Select((Button p, int idx) => new
			{
				btn = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					if (!change)
					{
						int num13 = p.index / 2;
						int num14 = ((p.index % 2 != 0) ? 1 : (-1));
						float val5 = (float)num14 * moveSclValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[correctNo]];
						cvsAccessory[nSlotNo].FuncUpdateAcsSclAdd(correctNo, num13, true, val5);
						cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
						inpScl[num13].text = accessory.parts[nSlotNo].addMove[correctNo, 2][num13].ToString();
					}
				});
				p.btn.UpdateAsObservable().SkipUntil(p.btn.OnPointerDownAsObservable().Do(delegate
				{
					downTimeCnt = 0f;
					loopTimeCnt = 0f;
					change = false;
				})).TakeUntil(p.btn.OnPointerUpAsObservable().Do(delegate
				{
					if (change)
					{
						cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
					}
				}))
					.RepeatUntilDestroy(this)
					.Subscribe(delegate
					{
						int num15 = p.index / 2;
						int num16 = ((p.index % 2 != 0) ? 1 : (-1));
						float num17 = (float)num16 * moveSclValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[correctNo]];
						float num18 = 0f;
						downTimeCnt += Time.deltaTime;
						if (downTimeCnt > 0.3f)
						{
							for (loopTimeCnt += Time.deltaTime; loopTimeCnt > 0.05f; loopTimeCnt -= 0.05f)
							{
								num18 += num17;
							}
							if (num18 != 0f)
							{
								cvsAccessory[nSlotNo].FuncUpdateAcsSclAdd(correctNo, num15, true, num18);
								inpScl[num15].text = accessory.parts[nSlotNo].addMove[correctNo, 2][num15].ToString();
								change = true;
							}
						}
					})
					.AddTo(this);
			});
			inpScl.Select((TMP_InputField p, int idx) => new
			{
				inp = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.inp.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					int xyz3 = p.index % 3;
					float val6 = CustomBase.ConvertValueFromTextLimit(0.01f, 100f, 2, value);
					p.inp.text = val6.ToString();
					cvsAccessory[nSlotNo].FuncUpdateAcsSclAdd(correctNo, xyz3, false, val6);
					cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
				});
			});
			btnSclReset.Select((Button p, int idx) => new
			{
				btn = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.btn.OnClickAsObservable().Subscribe(delegate
				{
					inpScl[p.index].text = "1";
					cvsAccessory[nSlotNo].FuncUpdateAcsSclAdd(correctNo, p.index, false, 1f);
					cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
				});
			});
			btnCopy.OnClickAsObservable().Subscribe(delegate
			{
				Singleton<CustomBase>.Instance.vecAcsClipBord[0] = accessory.parts[nSlotNo].addMove[correctNo, 0];
				Singleton<CustomBase>.Instance.vecAcsClipBord[1] = accessory.parts[nSlotNo].addMove[correctNo, 1];
				Singleton<CustomBase>.Instance.vecAcsClipBord[2] = accessory.parts[nSlotNo].addMove[correctNo, 2];
			});
			btnPaste.OnClickAsObservable().Subscribe(delegate
			{
				cvsAccessory[nSlotNo].FuncUpdateAcsMovePaste(correctNo, Singleton<CustomBase>.Instance.vecAcsClipBord);
				cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
				inpPos[0].text = accessory.parts[nSlotNo].addMove[correctNo, 0].x.ToString();
				inpPos[1].text = accessory.parts[nSlotNo].addMove[correctNo, 0].y.ToString();
				inpPos[2].text = accessory.parts[nSlotNo].addMove[correctNo, 0].z.ToString();
				inpRot[0].text = accessory.parts[nSlotNo].addMove[correctNo, 1].x.ToString();
				inpRot[1].text = accessory.parts[nSlotNo].addMove[correctNo, 1].y.ToString();
				inpRot[2].text = accessory.parts[nSlotNo].addMove[correctNo, 1].z.ToString();
				inpScl[0].text = accessory.parts[nSlotNo].addMove[correctNo, 2].x.ToString();
				inpScl[1].text = accessory.parts[nSlotNo].addMove[correctNo, 2].y.ToString();
				inpScl[2].text = accessory.parts[nSlotNo].addMove[correctNo, 2].z.ToString();
				cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
			});
			btnAllReset.OnClickAsObservable().Subscribe(delegate
			{
				for (int j = 0; j < 3; j++)
				{
					inpPos[j].text = "0";
					inpRot[j].text = "0";
					inpScl[j].text = "1";
				}
				cvsAccessory[nSlotNo].FuncUpdateAcsAllReset(correctNo);
				cvsAccessory[nSlotNo].UpdateAcsMoveHistory();
				cvsAccessory[nSlotNo].SetControllerTransform(correctNo);
			});
		}
	}
}
