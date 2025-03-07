using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseCameraControl : MonoBehaviour
{
	public delegate bool NoCtrlFunc();

	protected struct CameraData
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
	}

	protected struct ResetData
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

	public Transform targetObj;

	public float xRotSpeed = 5f;

	public float yRotSpeed = 5f;

	public float zoomSpeed = 0.1f;

	public float moveSpeed = 0.1f;

	public float noneTargetDir = 5f;

	public NoCtrlFunc NoCtrlCondition;

	public NoCtrlFunc ZoomCondition;

	public NoCtrlFunc KeyCondition;

	public bool EnableResetKey = true;

	public readonly int CONFIG_SIZE = Enum.GetNames(typeof(Config)).Length;

	protected CameraData CamDat;

	protected Config cameraType = Config.Rotation;

	protected bool[] isDrags;

	private ResetData CamReset;

	public bool isInit;

	private const float INIT_FOV = 23f;

	private Camera _nowCamera;

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
			nowCamera.SafeProcObject(delegate(Camera cam)
			{
				cam.fieldOfView = value;
			});
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
			nowCamera.SafeProcObject(delegate(Camera cam)
			{
				cam.fieldOfView = value;
			});
		}
	}

	private Camera nowCamera
	{
		get
		{
			return this.GetComponentCache(ref _nowCamera);
		}
	}

	public void Reset(int mode)
	{
		int num = 0;
		if (mode == num++)
		{
			CamDat.Copy(CamReset);
			base.transform.rotation = CamReset.RotQ;
			nowCamera.SafeProcObject(delegate(Camera cam)
			{
				cam.fieldOfView = CamDat.Fov;
			});
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
		bool flag = false;
		bool[] array = new bool[CONFIG_SIZE];
		array[1] = Input.GetMouseButton(0);
		array[2] = Input.GetMouseButton(1);
		array[3] = false;
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
			if (isDrags[k])
			{
				Vector3 zero = Vector3.zero;
				switch (k)
				{
				case 0:
					zero.x = axis * moveSpeed;
					zero.z = axis2 * moveSpeed;
					CamDat.Pos += base.transform.TransformDirection(zero);
					break;
				case 1:
					zero.y += axis * xRotSpeed;
					zero.x -= axis2 * yRotSpeed;
					CamDat.Rot.y = (CamDat.Rot.y + zero.y) % 360f;
					CamDat.Rot.x = (CamDat.Rot.x + zero.x) % 360f;
					base.transform.rotation = Quaternion.Euler(CamDat.Rot);
					break;
				case 2:
					CamDat.Pos.y += axis2 * moveSpeed;
					CamDat.Dir.z -= axis * moveSpeed;
					CamDat.Dir.z = Mathf.Min(0f, CamDat.Dir.z);
					break;
				case 3:
					zero.x = axis * moveSpeed;
					zero.y = axis2 * moveSpeed;
					CamDat.Pos += base.transform.TransformDirection(zero);
					break;
				}
				flag = true;
				break;
			}
		}
		if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject() && Singleton<GameCursor>.IsInstance())
		{
			if (flag)
			{
				Singleton<GameCursor>.Instance.SetCursorLock(true);
			}
			else
			{
				Singleton<GameCursor>.Instance.UnLockCursor();
			}
		}
		return flag;
	}

	protected bool InputKeyProc()
	{
		bool flag = false;
		if (EnableResetKey && Input.GetKeyDown(KeyCode.R))
		{
			Reset(0);
		}
		else if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			CamDat.Rot.x = CamReset.Rot.x;
			CamDat.Rot.y = CamReset.Rot.y;
			base.transform.rotation = Quaternion.Euler(CamDat.Rot);
		}
		else if (Input.GetKeyDown(KeyCode.Slash))
		{
			CamDat.Rot.z = 0f;
			base.transform.rotation = Quaternion.Euler(CamDat.Rot);
		}
		else if (Input.GetKeyDown(KeyCode.Semicolon))
		{
			CamDat.Fov = CamReset.Fov;
			nowCamera.SafeProcObject(delegate(Camera cam)
			{
				cam.fieldOfView = CamDat.Fov;
			});
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
			CamDat.Pos += base.transform.TransformDirection(new Vector3(deltaTime, 0f, 0f));
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			flag = true;
			CamDat.Pos += base.transform.TransformDirection(new Vector3(0f - deltaTime, 0f, 0f));
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			flag = true;
			CamDat.Pos += base.transform.TransformDirection(new Vector3(0f, 0f, deltaTime));
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			flag = true;
			CamDat.Pos += base.transform.TransformDirection(new Vector3(0f, 0f, 0f - deltaTime));
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
			zero.x -= num;
		}
		else if (Input.GetKey(KeyCode.Keypad8))
		{
			flag = true;
			zero.x += num * xRotSpeed;
		}
		if (Input.GetKey(KeyCode.Keypad4))
		{
			flag = true;
			zero.y += num * yRotSpeed;
		}
		else if (Input.GetKey(KeyCode.Keypad6))
		{
			flag = true;
			zero.y -= num * yRotSpeed;
		}
		if (flag)
		{
			CamDat.Rot.y = (CamDat.Rot.y + zero.y) % 360f;
			CamDat.Rot.x = (CamDat.Rot.x + zero.x) % 360f;
			CamDat.Rot.z = (CamDat.Rot.z + zero.z) % 360f;
			base.transform.rotation = Quaternion.Euler(CamDat.Rot);
		}
		float deltaTime2 = Time.deltaTime;
		if (Input.GetKey(KeyCode.Equals))
		{
			flag = true;
			CamDat.Fov = Mathf.Max(CamDat.Fov - deltaTime2 * 15f, 1f);
			nowCamera.SafeProcObject(delegate(Camera cam)
			{
				cam.fieldOfView = CamDat.Fov;
			});
		}
		else if (Input.GetKey(KeyCode.RightBracket))
		{
			flag = true;
			CamDat.Fov = Mathf.Min(CamDat.Fov + deltaTime2 * 15f, 100f);
			nowCamera.SafeProcObject(delegate(Camera cam)
			{
				cam.fieldOfView = CamDat.Fov;
			});
		}
		return flag;
	}

	protected virtual void Awake()
	{
		CamDat.Fov = 23f;
		CamReset.Fov = 23f;
	}

	protected virtual void Start()
	{
		nowCamera.SafeProcObject(delegate(Camera cam)
		{
			cam.fieldOfView = CamReset.Fov;
		});
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
			isInit = true;
		}
	}

	protected void LateUpdate()
	{
		isControlNow = false;
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
		base.transform.position = base.transform.rotation * CamDat.Dir + CamDat.Pos;
	}

	public void ForceCalculate()
	{
		base.transform.position = base.transform.rotation * CamDat.Dir + CamDat.Pos;
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
			transform.SetPositionAndRotation(target.position, Quaternion.Euler(CamDat.Rot.x, CamDat.Rot.y, CamDat.Rot.z));
			transform.position += transform.forward * CamDat.Dir.z;
			transform.LookAt(CamDat.Pos);
			CamDat.Rot = base.transform.rotation.eulerAngles;
			if (isReset)
			{
				CamReset.Copy(CamDat, base.transform.rotation);
			}
		}
	}

	public void SetCamera(BaseCameraControl src)
	{
		base.transform.SetPositionAndRotation(src.transform.position, src.transform.rotation);
		CamDat = src.CamDat;
		CamDat.Pos = -(base.transform.rotation * CamDat.Dir - base.transform.position);
		CamReset.Copy(CamDat, base.transform.rotation);
		nowCamera.SafeProcObject(delegate(Camera cam)
		{
			cam.CopyFrom(src.GetComponent<Camera>());
		});
	}

	public void SetCamera(Vector3 pos, Vector3 angle, Quaternion rot, Vector3 dir)
	{
		base.transform.SetPositionAndRotation(pos, rot);
		CamDat.Rot = angle;
		CamDat.Dir = dir;
		CamDat.Pos = -(base.transform.rotation * CamDat.Dir - base.transform.position);
		CamReset.Copy(CamDat, base.transform.rotation);
	}

	public void SetCamera(Vector3 targPos, Vector3 camAngle, Vector3 camDir, float fov)
	{
		CameraAngle = camAngle;
		CameraDir = camDir;
		TargetPos = targPos;
		CameraFov = fov;
		base.transform.position = base.transform.rotation * camDir + targPos;
		CamReset.Copy(CamDat, base.transform.rotation);
	}

	public void CopyCamera(BaseCameraControl dest)
	{
		dest.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		dest.CamDat = CamDat;
		dest.CamDat.Pos = -(dest.transform.rotation * dest.CamDat.Dir - dest.transform.position);
	}

	public void CopyInstance(BaseCameraControl src)
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
		nowCamera.SafeProcObject(delegate(Camera cam)
		{
			cam.CopyFrom(src.GetComponent<Camera>());
		});
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = ((!(CamDat.Dir.z > 0f)) ? Color.blue : Color.red);
		Vector3 direction = CamDat.Pos - base.transform.position;
		Gizmos.DrawRay(base.transform.position, direction);
	}
}
