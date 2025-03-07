using System;
using System.Linq;
using Manager;
using UnityEngine;

public class BaseCameraControl_Ver2 : MonoBehaviour
{
	public delegate bool NoCtrlFunc();

	[Serializable]
	public struct CameraData
	{
		public Vector3 Pos;

		public Vector3 Dir;

		public Vector3 Rot;

		public float Fov;

		public void Copy(ResetData copy)
		{
			Pos = copy.Pos;
			Dir = copy.Dir;
			Rot = copy.Rot;
			Fov = copy.Fov;
		}

		public void Copy(CameraData copy)
		{
			Pos = copy.Pos;
			Dir = copy.Dir;
			Rot = copy.Rot;
			Fov = copy.Fov;
		}
	}

	public struct ResetData
	{
		public Vector3 Pos;

		public Vector3 Dir;

		public Vector3 Rot;

		public Quaternion RotQ;

		public float Fov;

		public void Copy(CameraData copy, Quaternion rot)
		{
			Pos = copy.Pos;
			Dir = copy.Dir;
			Rot = copy.Rot;
			RotQ = rot;
			Fov = copy.Fov;
		}
	}

	public enum Config
	{
		MoveXZ = 0,
		Rotation = 1,
		Translation = 2,
		MoveXY = 3
	}

	public Transform transBase;

	public Transform targetObj;

	public float xRotSpeed = 5f;

	public float yRotSpeed = 5f;

	public float zoomSpeed = 5f;

	public float moveSpeed = 0.05f;

	public float noneTargetDir = 5f;

	public float rateSpeedMin = 0.5f;

	public float rateSpeedMax = 2f;

	public bool isLimitPos;

	public float limitPos = 2f;

	public bool isLimitDir;

	public float limitDir = 10f;

	public float limitFov = 40f;

	public float limitRotX = 360f;

	public float limitRotY = 360f;

	public bool isLlimitRot;

	public NoCtrlFunc NoCtrlCondition;

	public NoCtrlFunc ZoomCondition;

	public NoCtrlFunc KeyCondition;

	public readonly int CONFIG_SIZE = Enum.GetNames(typeof(Config)).Length;

	[SerializeField]
	protected CameraData CamDat;

	protected Config cameraType = Config.Rotation;

	protected bool[] isDrags;

	protected ResetData CamReset;

	protected bool isInit;

	private const float INIT_FOV = 23f;

	protected CapsuleCollider viewCollider;

	protected float rateAddSpeed = 1f;

	public Camera thisCmaera { get; protected set; }

	public bool isControlNow { get; protected set; }

	public Config CameraType
	{
		get
		{
			return cameraType;
		}
		set
		{
			cameraType = value;
		}
	}

	public float CameraInitFov
	{
		get
		{
			return CamReset.Fov;
		}
		set
		{
			CamReset.Fov = value;
			CamDat.Fov = value;
			if (thisCmaera != null)
			{
				thisCmaera.fieldOfView = value;
			}
		}
	}

	public Vector3 TargetPos
	{
		get
		{
			return CamDat.Pos;
		}
		set
		{
			CamDat.Pos = value;
		}
	}

	public Vector3 CameraAngle
	{
		get
		{
			return CamDat.Rot;
		}
		set
		{
			base.transform.rotation = Quaternion.Euler(value);
			CamDat.Rot = value;
		}
	}

	public Vector3 Rot
	{
		set
		{
			CamDat.Rot = value;
		}
	}

	public Vector3 CameraDir
	{
		get
		{
			return CamDat.Dir;
		}
		set
		{
			CamDat.Dir = value;
		}
	}

	public float CameraFov
	{
		get
		{
			return CamDat.Fov;
		}
		set
		{
			CamDat.Fov = value;
			if (thisCmaera != null)
			{
				thisCmaera.fieldOfView = value;
			}
		}
	}

	public BaseCameraControl_Ver2()
	{
		CamDat.Fov = 23f;
		CamReset.Fov = 23f;
	}

	public void Reset(int mode)
	{
		int num = 0;
		if (mode == num++)
		{
			CamDat.Copy(CamReset);
			if (thisCmaera != null)
			{
				thisCmaera.fieldOfView = CamDat.Fov;
			}
		}
		else if (mode == num++)
		{
			CamDat.Pos = CamReset.Pos;
		}
		else if (mode == num++)
		{
			base.transform.rotation = CamReset.RotQ;
		}
		else if (mode == num++)
		{
			CamDat.Dir = CamReset.Dir;
		}
	}

	protected bool InputTouchProc()
	{
		if (Input.touchCount < 1)
		{
			return false;
		}
		float num = 10f * Time.deltaTime;
		if (Input.touchCount == 3)
		{
			Reset(0);
		}
		else if (Input.touchCount == 1)
		{
			Touch touch = Input.touches.First();
			TouchPhase phase = touch.phase;
			if (phase != 0 && phase == TouchPhase.Moved)
			{
				float num2 = 0.1f;
				float num3 = 0.01f;
				Vector3 zero = Vector3.zero;
				if (cameraType == Config.Rotation)
				{
					zero.y += touch.deltaPosition.x * xRotSpeed * num * num2;
					zero.x -= touch.deltaPosition.y * yRotSpeed * num * num2;
					zero += base.transform.rotation.eulerAngles;
					base.transform.rotation = Quaternion.Euler(zero);
				}
				else if (cameraType == Config.Translation)
				{
					CamDat.Dir.z -= touch.deltaPosition.x * xRotSpeed * num * num3;
					CamDat.Pos.y += touch.deltaPosition.y * yRotSpeed * num * num3;
				}
				else if (cameraType == Config.MoveXY)
				{
					zero.x = touch.deltaPosition.x * xRotSpeed * num * num3;
					zero.y = touch.deltaPosition.y * yRotSpeed * num * num3;
					CamDat.Pos += base.transform.TransformDirection(zero);
				}
				else if (cameraType == Config.MoveXZ)
				{
					zero.x = touch.deltaPosition.x * xRotSpeed * num * num3;
					zero.z = touch.deltaPosition.y * yRotSpeed * num * num3;
					CamDat.Pos += base.transform.TransformDirection(zero);
				}
			}
		}
		return true;
	}

	protected bool InputMouseWheelZoomProc()
	{
		bool result = false;
		float num = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		if (num != 0f)
		{
			CamDat.Dir.z += num;
			CamDat.Dir.z = Mathf.Min(0f, CamDat.Dir.z);
			result = true;
		}
		return result;
	}

	protected virtual bool InputMouseProc()
	{
		bool result = false;
		bool[] array = new bool[CONFIG_SIZE];
		array[1] = Input.GetMouseButton(0);
		array[2] = Input.GetMouseButton(1);
		array[3] = Input.GetMouseButton(2);
		array[0] = Input.GetMouseButton(0) && Input.GetMouseButton(1);
		for (int i = 0; i < CONFIG_SIZE; i++)
		{
			if (array[i])
			{
				isDrags[i] = true;
			}
		}
		for (int j = 0; j < CONFIG_SIZE; j++)
		{
			if (isDrags[j] && !array[j])
			{
				isDrags[j] = false;
			}
		}
		float axis = Input.GetAxis("Mouse X");
		float axis2 = Input.GetAxis("Mouse Y");
		for (int k = 0; k < CONFIG_SIZE; k++)
		{
			if (!isDrags[k])
			{
				continue;
			}
			Vector3 zero = Vector3.zero;
			switch (k)
			{
			case 0:
				zero.x = axis * moveSpeed * rateAddSpeed;
				zero.z = axis2 * moveSpeed * rateAddSpeed;
				if (transBase != null)
				{
					CamDat.Pos += transBase.InverseTransformDirection(base.transform.TransformDirection(zero));
				}
				else
				{
					CamDat.Pos += base.transform.TransformDirection(zero);
				}
				break;
			case 1:
				zero.y += axis * xRotSpeed * rateAddSpeed;
				zero.x -= axis2 * yRotSpeed * rateAddSpeed;
				CamDat.Rot.y = (CamDat.Rot.y + zero.y) % 360f;
				CamDat.Rot.x = (CamDat.Rot.x + zero.x) % 360f;
				break;
			case 2:
				CamDat.Pos.y += axis2 * moveSpeed * rateAddSpeed;
				CamDat.Dir.z -= axis * moveSpeed * rateAddSpeed;
				CamDat.Dir.z = Mathf.Min(0f, CamDat.Dir.z);
				break;
			case 3:
				zero.x = axis * moveSpeed * rateAddSpeed;
				zero.y = axis2 * moveSpeed * rateAddSpeed;
				if (transBase != null)
				{
					CamDat.Pos += transBase.InverseTransformDirection(base.transform.TransformDirection(zero));
				}
				else
				{
					CamDat.Pos += base.transform.TransformDirection(zero);
				}
				break;
			}
			result = true;
			break;
		}
		return result;
	}

	protected bool InputKeyProc()
	{
		bool flag = false;
		if (Input.GetKeyDown(KeyCode.R))
		{
			Reset(0);
		}
		else if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			CamDat.Rot.x = CamReset.Rot.x;
			CamDat.Rot.y = CamReset.Rot.y;
		}
		else if (Input.GetKeyDown(KeyCode.Slash))
		{
			CamDat.Rot.z = 0f;
		}
		else if (Input.GetKeyDown(KeyCode.Semicolon))
		{
			CamDat.Fov = CamReset.Fov;
			if (thisCmaera != null)
			{
				thisCmaera.fieldOfView = CamDat.Fov;
			}
		}
		float deltaTime = Time.deltaTime;
		if (Input.GetKey(KeyCode.Home))
		{
			flag = true;
			CamDat.Dir.z += deltaTime;
			CamDat.Dir.z = Mathf.Min(0f, CamDat.Dir.z);
		}
		else if (Input.GetKey(KeyCode.End))
		{
			flag = true;
			CamDat.Dir.z -= deltaTime;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			flag = true;
			if (transBase != null)
			{
				CamDat.Pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(deltaTime, 0f, 0f)));
			}
			else
			{
				CamDat.Pos += base.transform.TransformDirection(new Vector3(deltaTime, 0f, 0f));
			}
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			flag = true;
			if (transBase != null)
			{
				CamDat.Pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(0f - deltaTime, 0f, 0f)));
			}
			else
			{
				CamDat.Pos += base.transform.TransformDirection(new Vector3(0f - deltaTime, 0f, 0f));
			}
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			flag = true;
			if (transBase != null)
			{
				CamDat.Pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(0f, 0f, deltaTime)));
			}
			else
			{
				CamDat.Pos += base.transform.TransformDirection(new Vector3(0f, 0f, deltaTime));
			}
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			flag = true;
			if (transBase != null)
			{
				CamDat.Pos += transBase.InverseTransformDirection(base.transform.TransformDirection(new Vector3(0f, 0f, 0f - deltaTime)));
			}
			else
			{
				CamDat.Pos += base.transform.TransformDirection(new Vector3(0f, 0f, 0f - deltaTime));
			}
		}
		if (Input.GetKey(KeyCode.PageUp))
		{
			flag = true;
			CamDat.Pos.y += deltaTime;
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			flag = true;
			CamDat.Pos.y -= deltaTime;
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
			CamDat.Rot.y = (CamDat.Rot.y + zero.y) % 360f;
			CamDat.Rot.x = (CamDat.Rot.x + zero.x) % 360f;
			CamDat.Rot.z = (CamDat.Rot.z + zero.z) % 360f;
		}
		float deltaTime2 = Time.deltaTime;
		if (Input.GetKey(KeyCode.Equals))
		{
			flag = true;
			CamDat.Fov = Mathf.Max(CamDat.Fov - deltaTime2 * 15f, 1f);
			if (thisCmaera != null)
			{
				thisCmaera.fieldOfView = CamDat.Fov;
			}
		}
		else if (Input.GetKey(KeyCode.RightBracket))
		{
			flag = true;
			CamDat.Fov = Mathf.Min(CamDat.Fov + deltaTime2 * 15f, limitFov);
			if (thisCmaera != null)
			{
				thisCmaera.fieldOfView = CamDat.Fov;
			}
		}
		return flag;
	}

	protected void Start()
	{
		SetCtrlSpeed();
		thisCmaera = GetComponent<Camera>();
		if (thisCmaera != null)
		{
			thisCmaera.fieldOfView = CamReset.Fov;
		}
		ZoomCondition = () => false;
		isControlNow = false;
		isDrags = new bool[CONFIG_SIZE];
		for (int i = 0; i < isDrags.Length; i++)
		{
			isDrags[i] = false;
		}
		if (!isInit)
		{
			if (!targetObj)
			{
				Vector3 vector = base.transform.TransformDirection(Vector3.forward);
				CamDat.Pos = base.transform.position + vector * noneTargetDir;
			}
			TargetSet(targetObj, true);
		}
	}

	protected void LateUpdate()
	{
		isControlNow = false;
		SetCtrlSpeed();
		if (!isControlNow)
		{
			NoCtrlFunc zoomCondition = ZoomCondition;
			bool flag = true;
			if (zoomCondition != null)
			{
				flag = zoomCondition();
			}
			isControlNow |= flag && InputMouseWheelZoomProc();
		}
		if (!isControlNow)
		{
			NoCtrlFunc noCtrlCondition = NoCtrlCondition;
			bool flag2 = false;
			if (noCtrlCondition != null)
			{
				flag2 = noCtrlCondition();
			}
			if (!flag2)
			{
				if (InputTouchProc())
				{
					isControlNow = true;
				}
				else if (InputMouseProc())
				{
					isControlNow = true;
				}
			}
		}
		if (!isControlNow)
		{
			NoCtrlFunc keyCondition = KeyCondition;
			bool flag3 = true;
			if (keyCondition != null)
			{
				flag3 = keyCondition();
			}
			isControlNow |= flag3 && InputKeyProc();
		}
		CameraUpdate();
	}

	protected void CameraUpdate()
	{
		if (isLimitDir)
		{
			CamDat.Dir.z = Mathf.Clamp(CamDat.Dir.z, 0f - limitDir, 0f);
		}
		if (isLimitPos)
		{
			CamDat.Pos = Vector3.ClampMagnitude(CamDat.Pos, limitPos);
		}
		if (isLlimitRot)
		{
			CamDat.Rot.x = Mathf.Clamp(CamDat.Rot.x, 0f - limitRotX, limitRotX);
			CamDat.Rot.y = Mathf.Clamp(CamDat.Rot.y, 0f - limitRotY, limitRotY);
		}
		if (transBase != null)
		{
			base.transform.rotation = transBase.rotation * Quaternion.Euler(CamDat.Rot);
			base.transform.position = base.transform.rotation * CamDat.Dir + transBase.TransformPoint(CamDat.Pos);
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(CamDat.Rot);
			base.transform.position = base.transform.rotation * CamDat.Dir + CamDat.Pos;
		}
		viewCollider.height = CamDat.Dir.z;
		viewCollider.center = -Vector3.forward * CamDat.Dir.z * 0.5f;
	}

	public void TargetSet(Transform target, bool isReset)
	{
		if ((bool)target)
		{
			targetObj = target;
		}
		if ((bool)targetObj)
		{
			CamDat.Pos = targetObj.position;
		}
		Transform transform = base.transform;
		CamDat.Dir = Vector3.zero;
		CamDat.Dir.z = 0f - Vector3.Distance(CamDat.Pos, transform.position);
		transform.LookAt(CamDat.Pos);
		CamDat.Rot = base.transform.rotation.eulerAngles;
		if (isReset)
		{
			CamReset.Copy(CamDat, base.transform.rotation);
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
			CamDat.Pos = target.position;
		}
		if ((bool)target)
		{
			if (dir != float.MinValue)
			{
				CamDat.Dir = Vector3.zero;
				CamDat.Dir.z = 0f - dir;
			}
			Transform transform = base.transform;
			transform.position = target.position;
			transform.rotation.eulerAngles.Set(CamDat.Rot.x, CamDat.Rot.y, CamDat.Rot.z);
			transform.position += transform.forward * CamDat.Dir.z;
			transform.LookAt(CamDat.Pos);
			CamDat.Rot = base.transform.rotation.eulerAngles;
			if (isReset)
			{
				CamReset.Copy(CamDat, base.transform.rotation);
			}
		}
	}

	public void SetCamera(BaseCameraControl_Ver2 src)
	{
		base.transform.position = src.transform.position;
		base.transform.rotation = src.transform.rotation;
		CamDat = src.CamDat;
		CamDat.Pos = -(base.transform.rotation * CamDat.Dir - base.transform.position);
		CamReset.Copy(CamDat, base.transform.rotation);
		if (thisCmaera != null && src.thisCmaera != null)
		{
			thisCmaera.CopyFrom(src.thisCmaera);
		}
	}

	public void SetCamera(Vector3 pos, Vector3 angle, Quaternion rot, Vector3 dir)
	{
		base.transform.localPosition = pos;
		base.transform.localRotation = rot;
		CamDat.Rot = angle;
		CamDat.Dir = dir;
		CamDat.Pos = -(base.transform.localRotation * CamDat.Dir - base.transform.localPosition);
		CamReset.Copy(CamDat, base.transform.rotation);
	}

	public void CopyCamera(BaseCameraControl_Ver2 dest)
	{
		dest.transform.position = base.transform.position;
		dest.transform.rotation = base.transform.rotation;
		dest.CamDat = CamDat;
		dest.CamDat.Pos = -(dest.transform.rotation * dest.CamDat.Dir - dest.transform.position);
	}

	public void CopyInstance(BaseCameraControl_Ver2 src)
	{
		isInit = true;
		targetObj = src.targetObj;
		xRotSpeed = src.xRotSpeed;
		yRotSpeed = src.yRotSpeed;
		zoomSpeed = src.zoomSpeed;
		moveSpeed = src.moveSpeed;
		noneTargetDir = src.noneTargetDir;
		NoCtrlCondition = src.NoCtrlCondition;
		ZoomCondition = src.ZoomCondition;
		KeyCondition = src.KeyCondition;
		if (thisCmaera != null && src.thisCmaera != null)
		{
			thisCmaera.CopyFrom(src.thisCmaera);
		}
	}

	public CameraData GetCameraData()
	{
		return CamDat;
	}

	public void SetCameraData(CameraData _data)
	{
		CamDat = _data;
		if (thisCmaera != null)
		{
			thisCmaera.fieldOfView = CamDat.Fov;
		}
		CameraUpdate();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = ((!(CamDat.Dir.z > 0f)) ? Color.blue : Color.red);
		Gizmos.DrawRay(direction: (!(transBase != null)) ? (CamDat.Pos - base.transform.position) : (transBase.TransformPoint(CamDat.Pos) - base.transform.position), from: base.transform.position);
	}

	public bool SetWorldBase(Transform _trans)
	{
		if (transBase == null)
		{
			return false;
		}
		transBase.transform.position = _trans.position;
		transBase.transform.rotation = _trans.rotation;
		return true;
	}

	public bool SetWorldBase(Vector3 _pos, Vector3 _rot)
	{
		if (transBase == null)
		{
			return false;
		}
		transBase.transform.position = _pos;
		transBase.transform.rotation = Quaternion.Euler(_rot);
		return true;
	}

	public bool SetWorldBase(Vector3 _pos, Quaternion _rot)
	{
		if (transBase == null)
		{
			return false;
		}
		transBase.transform.position = _pos;
		transBase.transform.rotation = _rot;
		return true;
	}

	public bool GetWorldBase(ref Vector3 _pos, ref Quaternion _rot)
	{
		if (transBase == null)
		{
			return false;
		}
		_pos = transBase.transform.position;
		_rot = transBase.transform.rotation;
		return true;
	}

	public bool SetLocalBase(Vector3 _pos, Vector3 _rot)
	{
		if (transBase == null)
		{
			return false;
		}
		transBase.transform.localPosition = _pos;
		transBase.transform.localRotation = Quaternion.Euler(_rot);
		return true;
	}

	public bool SetCtrlSpeed()
	{
		if (Singleton<Manager.Config>.IsInstance())
		{
		}
		return true;
	}
}
