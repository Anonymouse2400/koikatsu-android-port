using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class CameraControl : MonoBehaviour
	{
		public delegate bool NoCtrlFunc();

		public class CameraData
		{
			private const int ver = 2;

			public Vector3 pos = Vector3.zero;

			public Vector3 rotate = Vector3.zero;

			public Vector3 distance = Vector3.zero;

			public float parse = 23f;

			public Quaternion rotation
			{
				get
				{
					return Quaternion.Euler(rotate);
				}
			}

			public CameraData()
			{
			}

			public CameraData(CameraData _src)
			{
				Copy(_src);
			}

			public void Set(Vector3 _pos, Vector3 _rotate, Vector3 _distance, float _parse)
			{
				pos = _pos;
				rotate = _rotate;
				distance = _distance;
				parse = _parse;
			}

			public void Save(BinaryWriter _writer)
			{
				_writer.Write(2);
				_writer.Write(pos.x);
				_writer.Write(pos.y);
				_writer.Write(pos.z);
				_writer.Write(rotate.x);
				_writer.Write(rotate.y);
				_writer.Write(rotate.z);
				_writer.Write(distance.x);
				_writer.Write(distance.y);
				_writer.Write(distance.z);
				_writer.Write(parse);
			}

			public void Load(BinaryReader _reader)
			{
				int num = _reader.ReadInt32();
				pos.x = _reader.ReadSingle();
				pos.y = _reader.ReadSingle();
				pos.z = _reader.ReadSingle();
				rotate.x = _reader.ReadSingle();
				rotate.y = _reader.ReadSingle();
				rotate.z = _reader.ReadSingle();
				if (num == 1)
				{
					_reader.ReadSingle();
				}
				else
				{
					distance.x = _reader.ReadSingle();
					distance.y = _reader.ReadSingle();
					distance.z = _reader.ReadSingle();
				}
				parse = _reader.ReadSingle();
			}

			public void Copy(CameraData _src)
			{
				pos = _src.pos;
				rotate = _src.rotate;
				distance = _src.distance;
				parse = _src.parse;
			}
		}

		public enum Config
		{
			MoveXZ = 0,
			Rotation = 1,
			Translation = 2,
			MoveXY = 3
		}

		public class VisibleObject
		{
			public string nameCollider;

			public float delay;

			public bool isVisible = true;

			public List<MeshRenderer> listRender = new List<MeshRenderer>();
		}

		private int m_MapLayer = -1;

		public Transform transBase;

		public Transform targetObj;

		public float xRotSpeed = 5f;

		public float yRotSpeed = 5f;

		public float zoomSpeed = 5f;

		public float moveSpeed = 0.05f;

		public float noneTargetDir = 5f;

		public bool isLimitPos;

		public float limitPos = 2f;

		public bool isLimitDir;

		public float limitDir = 10f;

		public float limitFov = 40f;

		[SerializeField]
		private Camera m_SubCamera;

		public NoCtrlFunc noCtrlCondition;

		public NoCtrlFunc zoomCondition;

		public NoCtrlFunc keyCondition;

		public readonly int CONFIG_SIZE = Enum.GetNames(typeof(Config)).Length;

		[SerializeField]
		protected CameraData cameraData = new CameraData();

		protected CameraData cameraReset = new CameraData();

		protected bool isInit;

		private const float INIT_FOV = 23f;

		protected CapsuleCollider viewCollider;

		protected float rateAddSpeed = 1f;

		private bool dragging;

		private bool m_ConfigVanish = true;

		[SerializeField]
		private Transform m_TargetTex;

		[SerializeField]
		private Renderer m_TargetRender;

		[SerializeField]
		private GameObject objRoot;

		private List<VisibleObject> lstMapVanish = new List<VisibleObject>();

		private List<Collider> listCollider = new List<Collider>();

		public bool isFlashVisible;

		private int mapLayer
		{
			get
			{
				if (m_MapLayer == -1)
				{
					m_MapLayer = LayerMask.GetMask("Map", "MapNoShadow");
				}
				return m_MapLayer;
			}
		}

		public Camera mainCmaera { get; protected set; }

		public Camera subCamera
		{
			get
			{
				return m_SubCamera;
			}
		}

		public bool isControlNow { get; protected set; }

		public bool isOutsideTargetTex { get; set; }

		public bool isCursorLock { get; set; }

		public bool isConfigTargetTex { get; set; }

		public bool isConfigVanish
		{
			get
			{
				return m_ConfigVanish;
			}
			set
			{
				if (Utility.SetStruct(ref m_ConfigVanish, value))
				{
					VisibleFroceVanish(true);
				}
			}
		}

		public Transform targetTex
		{
			get
			{
				return m_TargetTex;
			}
		}

		public bool active
		{
			get
			{
				return objRoot.activeSelf;
			}
			set
			{
				objRoot.SetActive(value);
			}
		}

		public Vector3 targetPos
		{
			get
			{
				return cameraData.pos;
			}
			set
			{
				cameraData.pos = value;
			}
		}

		public Vector3 cameraAngle
		{
			get
			{
				return cameraData.rotate;
			}
			set
			{
				base.transform.rotation = Quaternion.Euler(value);
				cameraData.rotate = value;
			}
		}

		public float fieldOfView
		{
			get
			{
				return cameraData.parse;
			}
			set
			{
				cameraData.parse = value;
				if (mainCmaera != null)
				{
					mainCmaera.fieldOfView = value;
				}
				if (subCamera != null)
				{
					subCamera.fieldOfView = value;
				}
			}
		}

		public CameraControl()
		{
			cameraData.parse = 23f;
			cameraReset.parse = 23f;
		}

		public CameraData Export()
		{
			return new CameraData(cameraData);
		}

		public CameraData ExportResetData()
		{
			return new CameraData(cameraReset);
		}

		public void Import(CameraData _src)
		{
			if (_src != null)
			{
				cameraData.Copy(_src);
				fieldOfView = cameraData.parse;
			}
		}

		public bool LoadVanish(string _assetbundle, string _file, GameObject _objMap)
		{
			lstMapVanish.Clear();
			if (_objMap == null)
			{
				return false;
			}
			if (_assetbundle.IsNullOrEmpty() || _file.IsNullOrEmpty())
			{
				return false;
			}
			string text = GlobalMethod.LoadAllListText(_assetbundle, _file);
			string[,] data;
			GlobalMethod.GetListString(text, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				VisibleObject visibleObject = new VisibleObject();
				visibleObject.nameCollider = data[i, 0];
				for (int j = 1; j < length2; j++)
				{
					string text2 = data[i, j];
					if (text2 == string.Empty)
					{
						break;
					}
					GameObject gameObject = _objMap.transform.FindLoop(text2);
					if (!(gameObject == null))
					{
						MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>(true);
						visibleObject.listRender.AddRange(componentsInChildren);
					}
				}
				lstMapVanish.Add(visibleObject);
			}
			return true;
		}

		public void CloerListCollider()
		{
			listCollider.Clear();
			lstMapVanish.Clear();
		}

		public void VisibleFroceVanish(bool _visible)
		{
			foreach (VisibleObject item in lstMapVanish)
			{
				foreach (MeshRenderer item2 in item.listRender)
				{
					if ((bool)item2)
					{
						item2.enabled = _visible;
					}
				}
				item.isVisible = _visible;
				item.delay = ((!_visible) ? 0f : 0.3f);
			}
		}

		private void VisibleFroceVanish(VisibleObject _obj, bool _visible)
		{
			if (_obj == null || _obj.listRender == null)
			{
				return;
			}
			foreach (MeshRenderer item in _obj.listRender)
			{
				item.enabled = _visible;
			}
			_obj.delay = ((!_visible) ? 0f : 0.3f);
			_obj.isVisible = _visible;
		}

		private void VanishProc()
		{
			if (!isConfigVanish)
			{
				return;
			}
			int count = lstMapVanish.Count;
			int i;
			for (i = 0; i < count; i++)
			{
				Collider collider = listCollider.Find((Collider x) => lstMapVanish[i].nameCollider == x.name);
				if (collider == null)
				{
					VanishDelayVisible(lstMapVanish[i]);
				}
				else if (lstMapVanish[i].isVisible)
				{
					VisibleFroceVanish(lstMapVanish[i], false);
				}
			}
		}

		private void VanishDelayVisible(VisibleObject _visible)
		{
			if (_visible.isVisible)
			{
				return;
			}
			if (!isFlashVisible)
			{
				_visible.delay += Time.deltaTime;
				if (_visible.delay >= 0.3f)
				{
					VisibleFroceVanish(_visible, true);
				}
			}
			else
			{
				VisibleFroceVanish(_visible, true);
			}
		}

		public void Reset(int _mode)
		{
			switch (_mode)
			{
			case 0:
				cameraData.Copy(cameraReset);
				fieldOfView = cameraData.parse;
				break;
			case 1:
				cameraData.pos = cameraReset.pos;
				break;
			case 2:
				base.transform.rotation = cameraReset.rotation;
				break;
			case 3:
				cameraData.distance = cameraReset.distance;
				break;
			}
		}

		protected virtual bool InputMouseWheelZoomProc()
		{
			float num = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
			if (num != 0f)
			{
				cameraData.distance.z += num;
				cameraData.distance.z = Mathf.Min(0f, cameraData.distance.z);
				return true;
			}
			return false;
		}

		protected virtual bool InputMouseProc()
		{
			bool result = false;
			float axis = Input.GetAxis("Mouse X");
			float axis2 = Input.GetAxis("Mouse Y");
			if ((!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()) && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
			{
				dragging = true;
			}
			else if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
			{
				dragging = false;
			}
			if (!dragging)
			{
				return false;
			}
			if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
			{
				Vector3 zero = Vector3.zero;
				zero.x = axis * moveSpeed * rateAddSpeed;
				zero.z = axis2 * moveSpeed * rateAddSpeed;
				if (transBase != null)
				{
					cameraData.pos += transBase.InverseTransformDirection(base.transform.TransformDirection(zero));
				}
				else
				{
					cameraData.pos += base.transform.TransformDirection(zero);
				}
				result = true;
			}
			else if (Input.GetMouseButton(0))
			{
				Vector3 zero2 = Vector3.zero;
				zero2.y += axis * xRotSpeed * rateAddSpeed;
				zero2.x -= axis2 * yRotSpeed * rateAddSpeed;
				cameraData.rotate.y = (cameraData.rotate.y + zero2.y) % 360f;
				cameraData.rotate.x = (cameraData.rotate.x + zero2.x) % 360f;
				result = true;
			}
			else if (Input.GetMouseButton(1))
			{
				cameraData.pos.y += axis2 * moveSpeed * rateAddSpeed;
				cameraData.distance.z -= axis * moveSpeed * rateAddSpeed;
				cameraData.distance.z = Mathf.Min(0f, cameraData.distance.z);
				result = true;
			}
			else if (Input.GetMouseButton(2))
			{
				Vector3 zero3 = Vector3.zero;
				zero3.x = axis * moveSpeed * rateAddSpeed;
				zero3.y = axis2 * moveSpeed * rateAddSpeed;
				if (transBase != null)
				{
					cameraData.pos += transBase.InverseTransformDirection(base.transform.TransformDirection(zero3));
				}
				else
				{
					cameraData.pos += base.transform.TransformDirection(zero3);
				}
				result = true;
			}
			return result;
		}

		protected virtual bool InputKeyProc()
		{
			bool flag = false;
			if (Input.GetKeyDown(KeyCode.A))
			{
				Reset(0);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				cameraData.rotate.x = cameraReset.rotate.x;
				cameraData.rotate.y = cameraReset.rotate.y;
			}
			else if (Input.GetKeyDown(KeyCode.Slash))
			{
				cameraData.rotate.z = 0f;
			}
			else if (Input.GetKeyDown(KeyCode.Semicolon))
			{
				fieldOfView = cameraReset.parse;
			}
			float deltaTime = Time.deltaTime;
			if (Input.GetKey(KeyCode.Home))
			{
				flag = true;
				cameraData.distance.z += deltaTime;
				cameraData.distance.z = Mathf.Min(0f, cameraData.distance.z);
			}
			else if (Input.GetKey(KeyCode.End))
			{
				flag = true;
				cameraData.distance.z -= deltaTime;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				flag = true;
				if (transBase != null)
				{
					cameraData.pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(deltaTime, 0f, 0f)));
				}
				else
				{
					cameraData.pos += base.transform.TransformDirection(new Vector3(deltaTime, 0f, 0f));
				}
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				flag = true;
				if (transBase != null)
				{
					cameraData.pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(0f - deltaTime, 0f, 0f)));
				}
				else
				{
					cameraData.pos += base.transform.TransformDirection(new Vector3(0f - deltaTime, 0f, 0f));
				}
			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				flag = true;
				if (transBase != null)
				{
					cameraData.pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(0f, 0f, deltaTime)));
				}
				else
				{
					cameraData.pos += base.transform.TransformDirection(new Vector3(0f, 0f, deltaTime));
				}
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				flag = true;
				if (transBase != null)
				{
					cameraData.pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(0f, 0f, 0f - deltaTime)));
				}
				else
				{
					cameraData.pos += base.transform.TransformDirection(new Vector3(0f, 0f, 0f - deltaTime));
				}
			}
			if (Input.GetKey(KeyCode.PageUp))
			{
				flag = true;
				cameraData.pos.y += deltaTime;
			}
			else if (Input.GetKey(KeyCode.PageDown))
			{
				flag = true;
				cameraData.pos.y -= deltaTime;
			}
			float num = 10f * Time.deltaTime;
			Vector3 zero = Vector3.zero;
			if (Input.GetKey(KeyCode.Period))
			{
				flag = true;
				zero.z += num;
			}
			else if (Input.GetKey(KeyCode.Backslash))
			{
				flag = true;
				zero.z -= num;
			}
			if (Input.GetKey(KeyCode.Keypad2))
			{
				flag = true;
				zero.x -= num * yRotSpeed;
			}
			else if (Input.GetKey(KeyCode.Keypad8))
			{
				flag = true;
				zero.x += num * yRotSpeed;
			}
			if (Input.GetKey(KeyCode.Keypad4))
			{
				flag = true;
				zero.y += num * xRotSpeed;
			}
			else if (Input.GetKey(KeyCode.Keypad6))
			{
				flag = true;
				zero.y -= num * xRotSpeed;
			}
			if (flag)
			{
				cameraData.rotate.y = (cameraData.rotate.y + zero.y) % 360f;
				cameraData.rotate.x = (cameraData.rotate.x + zero.x) % 360f;
				cameraData.rotate.z = (cameraData.rotate.z + zero.z) % 360f;
			}
			float deltaTime2 = Time.deltaTime;
			if (Input.GetKey(KeyCode.Equals))
			{
				flag = true;
				fieldOfView = Mathf.Max(cameraData.parse - deltaTime2 * 15f, 1f);
			}
			else if (Input.GetKey(KeyCode.RightBracket))
			{
				flag = true;
				fieldOfView = Mathf.Min(cameraData.parse + deltaTime2 * 15f, limitFov);
			}
			return flag;
		}

		protected virtual void CameraUpdate()
		{
			if (isLimitDir)
			{
				cameraData.distance.z = Mathf.Clamp(cameraData.distance.z, 0f - limitDir, 0f);
			}
			if (isLimitPos)
			{
				cameraData.pos = Vector3.ClampMagnitude(cameraData.pos, limitPos);
			}
			if (transBase != null)
			{
				base.transform.rotation = transBase.rotation * Quaternion.Euler(cameraData.rotate);
				base.transform.position = base.transform.rotation * cameraData.distance + transBase.TransformPoint(cameraData.pos);
			}
			else
			{
				base.transform.rotation = Quaternion.Euler(cameraData.rotate);
				base.transform.position = base.transform.rotation * cameraData.distance + cameraData.pos;
			}
			viewCollider.height = cameraData.distance.z;
			viewCollider.center = -Vector3.forward * cameraData.distance.z * 0.5f;
		}

		public void TargetSet(Transform target, bool isReset)
		{
			if ((bool)target)
			{
				targetObj = target;
			}
			if ((bool)targetObj)
			{
				cameraData.pos = targetObj.position;
			}
			Transform transform = base.transform;
			cameraData.distance = Vector3.zero;
			cameraData.distance.z = 0f - Vector3.Distance(cameraData.pos, transform.position);
			transform.LookAt(cameraData.pos);
			cameraData.rotate = base.transform.rotation.eulerAngles;
			if (isReset)
			{
				cameraReset.Copy(cameraData);
			}
		}

		public void FrontTarget(Transform target, bool isReset, float dir = float.MinValue)
		{
			if ((bool)target)
			{
				targetObj = target;
			}
			if ((bool)targetObj)
			{
				target = targetObj;
				cameraData.pos = target.position;
			}
			if ((bool)target)
			{
				if (dir != float.MinValue)
				{
					cameraData.distance = Vector3.zero;
					cameraData.distance.z = 0f - dir;
				}
				Transform transform = base.transform;
				transform.position = target.position;
				transform.rotation.eulerAngles.Set(cameraData.rotate.x, cameraData.rotate.y, cameraData.rotate.z);
				transform.position += transform.forward * cameraData.distance.z;
				transform.LookAt(cameraData.pos);
				cameraData.rotate = base.transform.rotation.eulerAngles;
				if (isReset)
				{
					cameraReset.Copy(cameraData);
				}
			}
		}

		public void SetCamera(Vector3 pos, Vector3 angle, Quaternion rot, Vector3 dir)
		{
			base.transform.localPosition = pos;
			base.transform.localRotation = rot;
			cameraData.rotate = angle;
			cameraData.distance = dir;
			cameraData.pos = -(base.transform.localRotation * cameraData.distance - base.transform.localPosition);
			cameraReset.Copy(cameraData);
		}

		public void SetBase(Transform _trans)
		{
			if (!(transBase == null))
			{
				transBase.transform.position = _trans.position;
				transBase.transform.rotation = _trans.rotation;
			}
		}

		public void ReflectOption()
		{
			rateAddSpeed = Studio.optionSystem.cameraSpeed;
			xRotSpeed = Studio.optionSystem.cameraSpeedX;
			yRotSpeed = Studio.optionSystem.cameraSpeedY;
			List<string> list = new List<string>();
			if (Singleton<Studio>.Instance.workInfo.visibleAxis)
			{
				if (Studio.optionSystem.selectedState == 0)
				{
					list.Add("Studio/Col");
				}
				list.Add("Studio/Select");
			}
			list.Add("Studio/Route");
			m_SubCamera.cullingMask = LayerMask.GetMask(list.ToArray());
		}

		private void Awake()
		{
			m_MapLayer = -1;
			mainCmaera = GetComponent<Camera>();
			fieldOfView = cameraReset.parse;
			zoomCondition = () => false;
			isControlNow = false;
			if (!targetObj)
			{
				Vector3 vector = base.transform.TransformDirection(Vector3.forward);
				cameraData.pos = base.transform.position + vector * noneTargetDir;
			}
			TargetSet(targetObj, true);
			isOutsideTargetTex = true;
			isConfigTargetTex = true;
			isCursorLock = true;
		}

		private IEnumerator Start()
		{
			if (m_TargetTex == null)
			{
				m_TargetTex = base.transform.Find("CameraTarget");
			}
			if ((bool)m_TargetTex)
			{
				m_TargetTex.localScale = Vector3.one * 0.01f;
				if (m_TargetRender == null)
				{
					m_TargetRender = m_TargetTex.GetComponent<Renderer>();
				}
			}
			viewCollider = base.gameObject.AddComponent<CapsuleCollider>();
			viewCollider.radius = 0.05f;
			viewCollider.isTrigger = true;
			viewCollider.direction = 2;
			Rigidbody rigid = base.gameObject.AddComponent<Rigidbody>();
			rigid.useGravity = false;
			rigid.isKinematic = true;
			ReflectOption();
			yield return new WaitWhile(() => !Manager.Config.initialized);
			m_ConfigVanish = Manager.Config.EtcData.Shield;
			listCollider.Clear();
			isInit = true;
		}

		private void LateUpdate()
		{
			if (Singleton<Scene>.Instance.AddSceneName != string.Empty || Singleton<Scene>.Instance.IsNowLoadingFade || (!isControlNow && Input.GetKey(KeyCode.B)))
			{
				return;
			}
			isControlNow = false;
			xRotSpeed = Studio.optionSystem.cameraSpeedX;
			yRotSpeed = Studio.optionSystem.cameraSpeedY;
			if (!isControlNow)
			{
				isControlNow |= (zoomCondition == null || zoomCondition()) && InputMouseWheelZoomProc();
			}
			if (!isControlNow && (noCtrlCondition == null || !noCtrlCondition()) && InputMouseProc())
			{
				isControlNow = true;
			}
			if (!isControlNow)
			{
				isControlNow |= (keyCondition == null || keyCondition()) && InputKeyProc();
			}
			CameraUpdate();
			if ((bool)targetTex)
			{
				if (transBase != null)
				{
					targetTex.position = transBase.TransformPoint(cameraData.pos);
				}
				else
				{
					targetTex.position = cameraData.pos;
				}
				Vector3 position = base.transform.position;
				position.y = targetTex.position.y;
				targetTex.transform.LookAt(position);
				targetTex.Rotate(90f, 0f, 0f);
				if ((bool)m_TargetRender)
				{
					m_TargetRender.enabled = isControlNow & isOutsideTargetTex & isConfigTargetTex;
				}
				if (Singleton<GameCursor>.IsInstance() && isCursorLock)
				{
					Singleton<GameCursor>.Instance.SetCursorLock(isControlNow & isOutsideTargetTex);
				}
			}
			VanishProc();
		}

		protected void OnTriggerEnter(Collider other)
		{
			if (!(other == null) && (mapLayer & (1 << other.gameObject.layer)) != 0)
			{
				Collider collider = listCollider.Find((Collider x) => other.name == x.name);
				if (collider == null)
				{
					listCollider.Add(other);
				}
			}
		}

		protected void OnTriggerStay(Collider other)
		{
			if (!(other == null) && (mapLayer & (1 << other.gameObject.layer)) != 0)
			{
				Collider collider = listCollider.Find((Collider x) => other.name == x.name);
				if (collider == null)
				{
					listCollider.Add(other);
				}
			}
		}

		protected void OnTriggerExit(Collider other)
		{
			listCollider.Clear();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = ((!(cameraData.distance.z > 0f)) ? Color.blue : Color.red);
			Gizmos.DrawRay(direction: (!(transBase != null)) ? (cameraData.pos - base.transform.position) : (transBase.TransformPoint(cameraData.pos) - base.transform.position), from: base.transform.position);
		}
	}
}
