using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	public class GuideInput : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		public delegate void OnVisible(bool _value);

		[SerializeField]
		protected TMP_InputField[] inputPos;

		[SerializeField]
		protected TMP_InputField[] inputRot;

		[SerializeField]
		protected TMP_InputField[] inputScale;

		[SerializeField]
		private Button[] buttonInit;

		[Space]
		[SerializeField]
		private Canvas canvasParent;

		private HashSet<GuideObject> hashSelectObject = new HashSet<GuideObject>();

		private BoolReactiveProperty _outsideVisible = new BoolReactiveProperty(true);

		private BoolReactiveProperty _visible = new BoolReactiveProperty(true);

		public OnVisible onVisible;

		private int _selectIndex = -1;

		public GuideObject guideObject
		{
			set
			{
				if (SetGuideObject(value))
				{
					UpdateUI();
				}
			}
		}

		public GuideObject deselectObject
		{
			set
			{
				if (DeselectGuideObject(value))
				{
					UpdateUI();
				}
			}
		}

		public bool outsideVisible
		{
			get
			{
				return _outsideVisible.Value;
			}
			set
			{
				_outsideVisible.Value = value;
			}
		}

		public bool visible
		{
			get
			{
				return _visible.Value;
			}
			set
			{
				_visible.Value = value;
			}
		}

		public int selectIndex
		{
			get
			{
				return _selectIndex;
			}
			set
			{
				_selectIndex = ((_selectIndex != value) ? value : (-1));
			}
		}

		private TMP_InputField[] arrayInput { get; set; }

		public void Stop()
		{
			hashSelectObject.Clear();
			visible = false;
		}

		public void UpdateUI()
		{
			if (hashSelectObject.Count != 0)
			{
				SetInputText();
			}
			else
			{
				visible = false;
			}
		}

		public void OnEndEditPos(int _target)
		{
			if (hashSelectObject.Count == 0)
			{
				return;
			}
			float num = InputToFloat(inputPos[_target]);
			List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
			foreach (GuideObject item in hashSelectObject)
			{
				if (item.enablePos)
				{
					Vector3 pos = item.changeAmount.pos;
					if (pos[_target] != num)
					{
						pos[_target] = num;
						Vector3 pos2 = item.changeAmount.pos;
						item.changeAmount.pos = pos;
						list.Add(new GuideCommand.EqualsInfo
						{
							dicKey = item.dicKey,
							oldValue = pos2,
							newValue = pos
						});
					}
				}
			}
			if (!list.IsNullOrEmpty())
			{
				Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(list.ToArray()));
			}
			SetInputTextPos();
		}

		public void OnEndEditRot(int _target)
		{
			if (hashSelectObject.Count == 0)
			{
				return;
			}
			float num = InputToFloat(inputRot[_target]) % 360f;
			List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
			foreach (GuideObject item in hashSelectObject)
			{
				if (item.enableRot)
				{
					Vector3 rot = item.changeAmount.rot;
					if (rot[_target] != num)
					{
						rot[_target] = num;
						Vector3 rot2 = item.changeAmount.rot;
						item.changeAmount.rot = rot;
						list.Add(new GuideCommand.EqualsInfo
						{
							dicKey = item.dicKey,
							oldValue = rot2,
							newValue = rot
						});
					}
				}
			}
			if (!list.IsNullOrEmpty())
			{
				Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.RotationEqualsCommand(list.ToArray()));
			}
			SetInputTextRot();
		}

		public void OnEndEditScale(int _target)
		{
			if (hashSelectObject.Count == 0)
			{
				return;
			}
			float num = Mathf.Max(InputToFloat(inputScale[_target]), 0.01f);
			List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
			foreach (GuideObject item in hashSelectObject)
			{
				if (item.enableScale)
				{
					Vector3 scale = item.changeAmount.scale;
					if (scale[_target] != num)
					{
						scale[_target] = num;
						Vector3 scale2 = item.changeAmount.scale;
						item.changeAmount.scale = scale;
						list.Add(new GuideCommand.EqualsInfo
						{
							dicKey = item.dicKey,
							oldValue = scale2,
							newValue = scale
						});
					}
				}
			}
			if (!list.IsNullOrEmpty())
			{
				Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.ScaleEqualsCommand(list.ToArray()));
			}
			SetInputTextScale(Vector3.zero);
		}

		public void SetInputText()
		{
			visible = true;
			bool flag = hashSelectObject.Any((GuideObject v) => !v.enablePos);
			bool flag2 = hashSelectObject.Any((GuideObject v) => !v.enableRot);
			bool flag3 = hashSelectObject.Any((GuideObject v) => !v.enableScale);
			SetInputTextPos();
			for (int i = 0; i < 3; i++)
			{
				inputPos[i].interactable = !flag;
			}
			SetInputTextRot();
			for (int j = 0; j < 3; j++)
			{
				inputRot[j].interactable = !flag2;
			}
			SetInputTextScale(Vector3.zero);
			for (int k = 0; k < 3; k++)
			{
				inputScale[k].interactable = !flag3;
			}
		}

		public void AddSelectMultiple(GuideObject _object)
		{
			if (!hashSelectObject.Contains(_object))
			{
				AddObject(_object);
				SetInputText();
			}
		}

		private bool SetGuideObject(GuideObject _object)
		{
			bool flag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			bool key = Input.GetKey(KeyCode.X);
			if (flag && !key)
			{
				if (hashSelectObject.Contains(_object))
				{
					return false;
				}
				AddObject(_object);
			}
			else
			{
				foreach (GuideObject item in hashSelectObject)
				{
					ChangeAmount changeAmount = item.changeAmount;
					changeAmount.onChangePos = (Action)Delegate.Remove(changeAmount.onChangePos, new Action(SetInputTextPos));
					changeAmount.onChangeRot = (Action)Delegate.Remove(changeAmount.onChangeRot, new Action(SetInputTextRot));
					changeAmount.onChangeScale = (Action<Vector3>)Delegate.Remove(changeAmount.onChangeScale, new Action<Vector3>(SetInputTextScale));
				}
				hashSelectObject.Clear();
				AddObject(_object);
			}
			return true;
		}

		private bool DeselectGuideObject(GuideObject _object)
		{
			if (_object == null)
			{
				return false;
			}
			if (!hashSelectObject.Contains(_object))
			{
				return false;
			}
			ChangeAmount changeAmount = _object.changeAmount;
			changeAmount.onChangePos = (Action)Delegate.Remove(changeAmount.onChangePos, new Action(SetInputTextPos));
			changeAmount.onChangeRot = (Action)Delegate.Remove(changeAmount.onChangeRot, new Action(SetInputTextRot));
			changeAmount.onChangeScale = (Action<Vector3>)Delegate.Remove(changeAmount.onChangeScale, new Action<Vector3>(SetInputTextScale));
			hashSelectObject.Remove(_object);
			return true;
		}

		private void AddObject(GuideObject _object)
		{
			if (!(_object == null))
			{
				ChangeAmount changeAmount = _object.changeAmount;
				changeAmount.onChangePos = (Action)Delegate.Combine(changeAmount.onChangePos, new Action(SetInputTextPos));
				changeAmount.onChangeRot = (Action)Delegate.Combine(changeAmount.onChangeRot, new Action(SetInputTextRot));
				changeAmount.onChangeScale = (Action<Vector3>)Delegate.Combine(changeAmount.onChangeScale, new Action<Vector3>(SetInputTextScale));
				hashSelectObject.Add(_object);
			}
		}

		private void SetInputTextPos()
		{
			GuideObject guideObject = hashSelectObject.ElementAtOrDefault(0);
			Vector3 vector = ((!guideObject) ? Vector3.zero : guideObject.changeAmount.pos);
			bool[] array = new bool[3] { true, true, true };
			foreach (GuideObject item in hashSelectObject)
			{
				Vector3 pos = item.changeAmount.pos;
				for (int i = 0; i < 3; i++)
				{
					array[i] = vector[i] == pos[i];
				}
			}
			for (int j = 0; j < 3; j++)
			{
				inputPos[j].text = ((!array[j]) ? "-" : vector[j].ToString("0.000"));
			}
		}

		private void SetInputTextRot()
		{
			GuideObject guideObject = hashSelectObject.ElementAtOrDefault(0);
			Vector3 vector = ((!guideObject) ? Vector3.zero : guideObject.changeAmount.rot);
			bool[] array = new bool[3] { true, true, true };
			foreach (GuideObject item in hashSelectObject)
			{
				Vector3 rot = item.changeAmount.rot;
				for (int i = 0; i < 3; i++)
				{
					array[i] = vector[i] == rot[i];
				}
			}
			for (int j = 0; j < 3; j++)
			{
				inputRot[j].text = ((!array[j]) ? "-" : vector[j].ToString("0.000"));
			}
		}

		private void SetInputTextScale(Vector3 _value)
		{
			GuideObject guideObject = hashSelectObject.ElementAtOrDefault(0);
			Vector3 vector = ((!guideObject) ? Vector3.zero : guideObject.changeAmount.scale);
			bool[] array = new bool[3] { true, true, true };
			foreach (GuideObject item in hashSelectObject)
			{
				Vector3 scale = item.changeAmount.scale;
				for (int i = 0; i < 3; i++)
				{
					array[i] = vector[i] == scale[i];
				}
			}
			for (int j = 0; j < 3; j++)
			{
				inputScale[j].text = ((!array[j]) ? "-" : vector[j].ToString("0.000"));
			}
		}

		private float InputToFloat(TMP_InputField _input)
		{
			float result = 0f;
			return (!float.TryParse(_input.text, out result)) ? 0f : result;
		}

		private bool Vector3Equals(ref Vector3 _a, ref Vector3 _b)
		{
			return _a.x == _b.x && _a.y == _b.y && _a.z == _b.z;
		}

		private void SetVisible()
		{
			bool active = outsideVisible & visible;
			if ((bool)canvasParent)
			{
				canvasParent.enabled = active;
			}
			else
			{
				base.gameObject.SetActive(active);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			SortCanvas.select = canvasParent;
		}

		private void ChangeTarget()
		{
			if (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift) | Input.GetKeyDown(KeyCode.LeftShift) | Input.GetKeyDown(KeyCode.RightShift))
			{
				selectIndex--;
				if (selectIndex < 0)
				{
					selectIndex = arrayInput.Length - 1;
				}
			}
			else
			{
				selectIndex = (selectIndex + 1) % arrayInput.Length;
			}
			if ((bool)arrayInput[selectIndex])
			{
				arrayInput[selectIndex].Select();
			}
		}

		private void OnClickInitPos()
		{
			if (hashSelectObject.Count == 0)
			{
				return;
			}
			List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
			foreach (GuideObject item in hashSelectObject)
			{
				if (item.enablePos)
				{
					Vector3 pos = item.changeAmount.pos;
					item.changeAmount.pos = Vector3.zero;
					list.Add(new GuideCommand.EqualsInfo
					{
						dicKey = item.dicKey,
						oldValue = pos,
						newValue = Vector3.zero
					});
				}
			}
			if (!list.IsNullOrEmpty())
			{
				Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(list.ToArray()));
			}
			SetInputTextPos();
		}

		private void OnClickInitRot()
		{
			if (hashSelectObject.Count == 0)
			{
				return;
			}
			List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
			foreach (GuideObject item in hashSelectObject)
			{
				if (item.enableRot)
				{
					Vector3 rot = item.changeAmount.rot;
					item.changeAmount.rot = Vector3.zero;
					list.Add(new GuideCommand.EqualsInfo
					{
						dicKey = item.dicKey,
						oldValue = rot,
						newValue = Vector3.zero
					});
				}
			}
			if (!list.IsNullOrEmpty())
			{
				Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.RotationEqualsCommand(list.ToArray()));
			}
			SetInputTextRot();
		}

		private void OnClickInitScale()
		{
			if (hashSelectObject.Count == 0)
			{
				return;
			}
			List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
			foreach (GuideObject item in hashSelectObject)
			{
				if (item.enableScale)
				{
					Vector3 scale = item.changeAmount.scale;
					item.changeAmount.scale = Vector3.one;
					list.Add(new GuideCommand.EqualsInfo
					{
						dicKey = item.dicKey,
						oldValue = scale,
						newValue = Vector3.one
					});
				}
			}
			if (!list.IsNullOrEmpty())
			{
				Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.ScaleEqualsCommand(list.ToArray()));
			}
			SetInputTextScale(Vector3.zero);
		}

		private void Awake()
		{
			_outsideVisible.Subscribe(delegate
			{
				SetVisible();
			});
			_visible.Subscribe(delegate(bool _b)
			{
				SetVisible();
				if (onVisible != null)
				{
					onVisible(_b);
				}
			});
			buttonInit[0].onClick.AddListener(OnClickInitPos);
			buttonInit[1].onClick.AddListener(OnClickInitRot);
			buttonInit[2].onClick.AddListener(OnClickInitScale);
			visible = false;
			List<TMP_InputField> list = new List<TMP_InputField>();
			list.AddRange(inputPos);
			list.AddRange(inputRot);
			list.AddRange(inputScale);
			arrayInput = list.ToArray();
			selectIndex = -1;
		}

		private void Start()
		{
			(from _ in this.UpdateAsObservable()
				where selectIndex != -1
				where Input.GetKeyDown(KeyCode.Tab)
				select _).Subscribe(delegate
			{
				ChangeTarget();
			});
		}
	}
}
