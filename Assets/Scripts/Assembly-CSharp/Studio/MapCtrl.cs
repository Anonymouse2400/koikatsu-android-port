using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MapCtrl : Singleton<MapCtrl>
	{
		[SerializeField]
		private TMP_InputField[] inputPos;

		[SerializeField]
		private TMP_InputField[] inputRot;

		[SerializeField]
		private MapDragButton[] mapDragButton;

		[SerializeField]
		private Button[] buttonSunLight;

		[SerializeField]
		private Toggle toggleOption;

		private Vector3 oldValue = Vector3.zero;

		private Transform transMap;

		public bool active
		{
			set
			{
				base.gameObject.SetActive(value);
				if (value)
				{
					UpdateUI();
				}
			}
		}

		public void UpdateUI()
		{
			SetInputTextPos();
			SetInputTextRot();
			int sunLightType = Singleton<Studio>.Instance.sceneInfo.sunLightType;
			for (int i = 0; i < buttonSunLight.Length; i++)
			{
				buttonSunLight[i].image.color = ((i != sunLightType) ? Color.white : Color.green);
			}
			toggleOption.isOn = Singleton<Studio>.Instance.sceneInfo.mapOption;
		}

		public void Reflect()
		{
			GameObject mapRoot = Singleton<Map>.Instance.mapRoot;
			if (!(mapRoot == null))
			{
				Transform transform = mapRoot.transform;
				transform.localPosition = Singleton<Studio>.Instance.sceneInfo.caMap.pos;
				transform.localRotation = Quaternion.Euler(Singleton<Studio>.Instance.sceneInfo.caMap.rot);
				UpdateUI();
			}
		}

		public void OnEndEditPos(int _target)
		{
			float num = InputToFloat(inputPos[_target]);
			Vector3 pos = Singleton<Studio>.Instance.sceneInfo.caMap.pos;
			if (pos[_target] != num)
			{
				Vector3 vector = pos;
				pos[_target] = num;
				Singleton<Studio>.Instance.sceneInfo.caMap.pos = pos;
				Singleton<UndoRedoManager>.Instance.Push(new MapCommand.MoveEqualsCommand(new MapCommand.EqualsInfo
				{
					newValue = pos,
					oldValue = vector
				}));
			}
			SetInputTextPos();
		}

		public void OnEndEditRot(int _target)
		{
			float num = InputToFloat(inputRot[_target]) % 360f;
			Vector3 rot = Singleton<Studio>.Instance.sceneInfo.caMap.rot;
			if (rot[_target] != num)
			{
				Vector3 vector = rot;
				rot[_target] = num;
				Singleton<Studio>.Instance.sceneInfo.caMap.rot = rot;
				Singleton<UndoRedoManager>.Instance.Push(new MapCommand.RotationEqualsCommand(new MapCommand.EqualsInfo
				{
					newValue = rot,
					oldValue = vector
				}));
			}
			SetInputTextRot();
		}

		private float InputToFloat(TMP_InputField _input)
		{
			float result = 0f;
			return (!float.TryParse(_input.text, out result)) ? 0f : result;
		}

		private void SetInputTextPos()
		{
			Vector3 pos = Singleton<Studio>.Instance.sceneInfo.caMap.pos;
			for (int i = 0; i < 3; i++)
			{
				inputPos[i].text = pos[i].ToString("0.000");
			}
		}

		private void SetInputTextRot()
		{
			Vector3 rot = Singleton<Studio>.Instance.sceneInfo.caMap.rot;
			for (int i = 0; i < 3; i++)
			{
				inputRot[i].text = rot[i].ToString("0.000");
			}
		}

		private void OnBeginDragTrans()
		{
			oldValue = Singleton<Studio>.Instance.sceneInfo.caMap.pos;
			transMap = Singleton<Map>.Instance.mapRoot.transform;
		}

		private void OnEndDragTrans()
		{
			Singleton<UndoRedoManager>.Instance.Push(new MapCommand.MoveEqualsCommand(new MapCommand.EqualsInfo
			{
				newValue = Singleton<Studio>.Instance.sceneInfo.caMap.pos,
				oldValue = oldValue
			}));
			transMap = null;
		}

		private void OnDragTransXZ()
		{
			Vector3 direction = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));
			Singleton<Studio>.Instance.sceneInfo.caMap.pos += transMap.TransformDirection(direction);
		}

		private void OnDragTransY()
		{
			Vector3 direction = new Vector3(0f, Input.GetAxis("Mouse Y"), 0f);
			Singleton<Studio>.Instance.sceneInfo.caMap.pos += transMap.TransformDirection(direction);
		}

		private void OnBeginDragRot()
		{
			oldValue = Singleton<Studio>.Instance.sceneInfo.caMap.rot;
		}

		private void OnEndDragRot()
		{
			Singleton<UndoRedoManager>.Instance.Push(new MapCommand.RotationEqualsCommand(new MapCommand.EqualsInfo
			{
				newValue = Singleton<Studio>.Instance.sceneInfo.caMap.rot,
				oldValue = oldValue
			}));
		}

		private void OnDragRotX()
		{
			Vector3 rot = Singleton<Studio>.Instance.sceneInfo.caMap.rot;
			rot.x = (rot.x + Input.GetAxis("Mouse Y")) % 360f;
			Singleton<Studio>.Instance.sceneInfo.caMap.rot = rot;
		}

		private void OnDragRotY()
		{
			Vector3 rot = Singleton<Studio>.Instance.sceneInfo.caMap.rot;
			rot.y = (rot.y + Input.GetAxis("Mouse X")) % 360f;
			Singleton<Studio>.Instance.sceneInfo.caMap.rot = rot;
		}

		private void OnDragRotZ()
		{
			Vector3 rot = Singleton<Studio>.Instance.sceneInfo.caMap.rot;
			rot.z = (rot.z + Input.GetAxis("Mouse X")) % 360f;
			Singleton<Studio>.Instance.sceneInfo.caMap.rot = rot;
		}

		private void OnClickSunLightType(int _type)
		{
			Singleton<Map>.Instance.sunType = (SunLightInfo.Info.Type)_type;
			for (int i = 0; i < buttonSunLight.Length; i++)
			{
				buttonSunLight[i].image.color = ((i != _type) ? Color.white : Color.green);
			}
		}

		private void OnValueChangedOption(bool _value)
		{
			Singleton<Map>.Instance.visibleOption = _value;
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				MapDragButton obj = mapDragButton[0];
				obj.onBeginDragFunc = (Action)Delegate.Combine(obj.onBeginDragFunc, new Action(OnBeginDragTrans));
				MapDragButton obj2 = mapDragButton[0];
				obj2.onDragFunc = (Action)Delegate.Combine(obj2.onDragFunc, new Action(OnDragTransXZ));
				MapDragButton obj3 = mapDragButton[0];
				obj3.onEndDragFunc = (Action)Delegate.Combine(obj3.onEndDragFunc, new Action(OnEndDragTrans));
				MapDragButton obj4 = mapDragButton[1];
				obj4.onBeginDragFunc = (Action)Delegate.Combine(obj4.onBeginDragFunc, new Action(OnBeginDragTrans));
				MapDragButton obj5 = mapDragButton[1];
				obj5.onDragFunc = (Action)Delegate.Combine(obj5.onDragFunc, new Action(OnDragTransY));
				MapDragButton obj6 = mapDragButton[1];
				obj6.onEndDragFunc = (Action)Delegate.Combine(obj6.onEndDragFunc, new Action(OnEndDragTrans));
				for (int i = 0; i < 3; i++)
				{
					MapDragButton obj7 = mapDragButton[2 + i];
					obj7.onBeginDragFunc = (Action)Delegate.Combine(obj7.onBeginDragFunc, new Action(OnBeginDragRot));
					MapDragButton obj8 = mapDragButton[2 + i];
					obj8.onEndDragFunc = (Action)Delegate.Combine(obj8.onEndDragFunc, new Action(OnEndDragRot));
				}
				MapDragButton obj9 = mapDragButton[2];
				obj9.onDragFunc = (Action)Delegate.Combine(obj9.onDragFunc, new Action(OnDragRotX));
				MapDragButton obj10 = mapDragButton[3];
				obj10.onDragFunc = (Action)Delegate.Combine(obj10.onDragFunc, new Action(OnDragRotY));
				MapDragButton obj11 = mapDragButton[4];
				obj11.onDragFunc = (Action)Delegate.Combine(obj11.onDragFunc, new Action(OnDragRotZ));
				buttonSunLight[0].onClick.AddListener(delegate
				{
					OnClickSunLightType(0);
				});
				buttonSunLight[1].onClick.AddListener(delegate
				{
					OnClickSunLightType(1);
				});
				buttonSunLight[2].onClick.AddListener(delegate
				{
					OnClickSunLightType(2);
				});
				toggleOption.onValueChanged.AddListener(delegate(bool b)
				{
					OnValueChangedOption(b);
				});
			}
		}
	}
}
