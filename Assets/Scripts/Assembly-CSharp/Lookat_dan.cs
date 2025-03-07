using System;
using System.Collections.Generic;
using IllusionUtility.GetUtility;
using UnityEngine;

public class Lookat_dan : MonoBehaviour
{
	[Serializable]
	public class LookAt
	{
		public enum AxisType
		{
			X = 0,
			Y = 1,
			Z = 2,
			RevX = 3,
			RevY = 4,
			RevZ = 5,
			None = 6
		}

		public enum RotationOrder
		{
			XYZ = 0,
			XZY = 1,
			YXZ = 2,
			YZX = 3,
			ZXY = 4,
			ZYX = 5
		}

		public string lookAtName = string.Empty;

		public string targetName = string.Empty;

		public AxisType targetAxisType = AxisType.Z;

		public string upAxisName = string.Empty;

		public AxisType upAxisType = AxisType.Y;

		public AxisType sourceAxisType = AxisType.Y;

		public AxisType limitAxisType = AxisType.None;

		public RotationOrder rotOrder = RotationOrder.ZXY;

		[Range(-180f, 180f)]
		public float limitMin;

		[Range(-180f, 180f)]
		public float limitMax;

		private Quaternion oldRotation = Quaternion.identity;

		public Transform trfLookAt { get; private set; }

		public Transform trfTarget { get; private set; }

		public Transform trfUpAxis { get; private set; }

		public LookAt()
		{
			trfLookAt = null;
			trfTarget = null;
			trfUpAxis = null;
		}

		public void SetLookAtTransform(Transform trf)
		{
			trfLookAt = trf;
		}

		public void SetTargetTransform(Transform trf)
		{
			trfTarget = trf;
		}

		public void SetUpAxisTransform(Transform trf)
		{
			trfUpAxis = trf;
		}

		public void SetOldRotation(Quaternion q)
		{
			oldRotation = q;
		}

		public void Update()
		{
			if (null == trfTarget || null == trfLookAt)
			{
				return;
			}
			Vector3 upVector = GetUpVector();
			Vector3 vector = Vector3.Normalize(trfTarget.position - trfLookAt.position);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(upVector, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			if (targetAxisType == AxisType.RevX || targetAxisType == AxisType.RevY || targetAxisType == AxisType.RevZ)
			{
				vector = -vector;
				vector2 = -vector2;
			}
			Vector3 xvec = Vector3.zero;
			Vector3 yvec = Vector3.zero;
			Vector3 zvec = Vector3.zero;
			switch (targetAxisType)
			{
			case AxisType.X:
			case AxisType.RevX:
				xvec = vector;
				if (sourceAxisType == AxisType.Y)
				{
					yvec = vector3;
					zvec = -vector2;
				}
				else if (sourceAxisType == AxisType.RevY)
				{
					yvec = -vector3;
					zvec = vector2;
				}
				else if (sourceAxisType == AxisType.Z)
				{
					yvec = vector2;
					zvec = vector3;
				}
				else if (sourceAxisType == AxisType.RevZ)
				{
					yvec = -vector2;
					zvec = -vector3;
				}
				break;
			case AxisType.Y:
			case AxisType.RevY:
				yvec = vector;
				if (sourceAxisType == AxisType.X)
				{
					xvec = vector3;
					zvec = vector2;
				}
				else if (sourceAxisType == AxisType.RevX)
				{
					xvec = -vector3;
					zvec = -vector2;
				}
				else if (sourceAxisType == AxisType.Z)
				{
					xvec = -vector2;
					zvec = vector3;
				}
				else if (sourceAxisType == AxisType.RevZ)
				{
					xvec = vector2;
					zvec = -vector3;
				}
				break;
			case AxisType.Z:
			case AxisType.RevZ:
				zvec = vector;
				if (sourceAxisType == AxisType.X)
				{
					xvec = vector3;
					yvec = -vector2;
				}
				else if (sourceAxisType == AxisType.RevX)
				{
					xvec = -vector3;
					yvec = vector2;
				}
				else if (sourceAxisType == AxisType.Y)
				{
					xvec = vector2;
					yvec = vector3;
				}
				else if (sourceAxisType == AxisType.RevY)
				{
					xvec = -vector2;
					yvec = -vector3;
				}
				break;
			}
			if (limitAxisType == AxisType.None)
			{
				Quaternion q = default(Quaternion);
				if (LookAtQuat(xvec, yvec, zvec, ref q))
				{
					trfLookAt.rotation = q;
				}
				else
				{
					trfLookAt.rotation = oldRotation;
				}
				oldRotation = trfLookAt.rotation;
				return;
			}
			Quaternion q2 = default(Quaternion);
			if (LookAtQuat(xvec, yvec, zvec, ref q2))
			{
				trfLookAt.rotation = q2;
			}
			else
			{
				trfLookAt.rotation = oldRotation;
			}
			ConvertRotation.RotationOrder order = (ConvertRotation.RotationOrder)rotOrder;
			Quaternion localRotation = trfLookAt.localRotation;
			Vector3 vector4 = ConvertRotation.ConvertDegreeFromQuaternion(order, localRotation);
			Quaternion q3 = Quaternion.Slerp(localRotation, Quaternion.identity, 0.5f);
			Vector3 vector5 = ConvertRotation.ConvertDegreeFromQuaternion(order, q3);
			if (limitAxisType == AxisType.X)
			{
				if ((vector4.x < 0f && vector5.x > 0f) || (vector4.x > 0f && vector5.x < 0f))
				{
					vector4.x *= -1f;
				}
				vector4.x = Mathf.Clamp(vector4.x, limitMin, limitMax);
			}
			else if (limitAxisType == AxisType.Y)
			{
				if ((vector4.y < 0f && vector5.y > 0f) || (vector4.y > 0f && vector5.y < 0f))
				{
					vector4.y *= -1f;
				}
				vector4.y = Mathf.Clamp(vector4.y, limitMin, limitMax);
			}
			else if (limitAxisType == AxisType.Z)
			{
				if ((vector4.z < 0f && vector5.z > 0f) || (vector4.z > 0f && vector5.z < 0f))
				{
					vector4.z *= -1f;
				}
				vector4.z = Mathf.Clamp(vector4.z, limitMin, limitMax);
			}
			trfLookAt.localRotation = ConvertRotation.ConvertDegreeToQuaternion(order, vector4.x, vector4.y, vector4.z);
			oldRotation = trfLookAt.rotation;
		}

		private Vector3 GetUpVector()
		{
			Vector3 result = Vector3.up;
			if ((bool)trfUpAxis)
			{
				switch (upAxisType)
				{
				case AxisType.X:
					result = trfUpAxis.right;
					break;
				case AxisType.Y:
					result = trfUpAxis.up;
					break;
				case AxisType.Z:
					result = trfUpAxis.forward;
					break;
				}
			}
			return result;
		}

		private bool LookAtQuat(Vector3 xvec, Vector3 yvec, Vector3 zvec, ref Quaternion q)
		{
			float num = 1f + xvec.x + yvec.y + zvec.z;
			if (num == 0f)
			{
				GlobalMethod.DebugLog("LookAt 計算不可 値0", 1);
				return false;
			}
			float num2 = Mathf.Sqrt(num) / 2f;
			if (float.IsNaN(num2))
			{
				GlobalMethod.DebugLog("LookAt 計算不可 NaN", 1);
				return false;
			}
			float num3 = 4f * num2;
			if (num3 == 0f)
			{
				GlobalMethod.DebugLog("LookAt 計算不可 w=0", 1);
				return false;
			}
			float x = (yvec.z - zvec.y) / num3;
			float y = (zvec.x - xvec.z) / num3;
			float z = (xvec.y - yvec.x) / num3;
			q = new Quaternion(x, y, z, num2);
			return true;
		}
	}

	[Serializable]
	public class ShapeInfo
	{
		public int shape;

		public Vector3 minPos;

		public Vector3 maxPos;
	}

	[Serializable]
	public class MotionLookAtList
	{
		public string strMotion;

		public int numFemale;

		public string strLookAtNull;

		public bool bTopStick;

		public int lookAtCalcKind;

		public List<ShapeInfo> lstShape = new List<ShapeInfo>();

		public void MemberInit()
		{
			strMotion = string.Empty;
			numFemale = 0;
			strLookAtNull = string.Empty;
			bTopStick = false;
			lookAtCalcKind = 0;
			lstShape = new List<ShapeInfo>();
		}
	}

	private ChaControl[] females;

	private ChaControl male;

	public List<MotionLookAtList> lstLookAt = new List<MotionLookAtList>();

	public string nameBaseBone = "cm_J_dan101_00";

	public string nameTop = "cm_J_dan109_00";

	public string nameRefBone = "cm_J_dan100_00";

	public LookAt lookat = new LookAt();

	[Header("デバッグ")]
	public string strPlayMotion = string.Empty;

	public Transform transLookAtNull;

	public bool bTopStick;

	public List<ShapeInfo> lstShape = new List<ShapeInfo>();

	public int numFemale;

	public int lookAtCalcKind;

	private GameObject objDanBase;

	private GameObject objDanTop;

	public void SetFemale(ChaControl[] _females)
	{
		females = _females;
	}

	public void SetMale(ChaControl _male)
	{
		male = _male;
		objDanBase = null;
		objDanTop = null;
		Transform transform = ((!(male != null)) ? null : male.objBodyBone.transform);
		if (transform != null)
		{
			objDanBase = transform.FindLoop(nameBaseBone);
			objDanTop = transform.FindLoop(nameTop);
			GameObject gameObject = transform.FindLoop(nameRefBone);
			lookat.SetUpAxisTransform((!(gameObject != null)) ? null : gameObject.transform);
		}
		lookat.SetLookAtTransform((!(objDanBase != null)) ? null : objDanBase.transform);
	}

	public bool LoadList(string _pathAssetFolder, string _pathFile)
	{
		Release();
		if (_pathFile == string.Empty)
		{
			return false;
		}
		string text = GlobalMethod.LoadAllListText(_pathAssetFolder, _pathFile);
		if (text == string.Empty)
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			string motion = data[i, num++];
			MotionLookAtList motionLookAtList = lstLookAt.Find((MotionLookAtList l) => l.strMotion == motion);
			if (motionLookAtList == null)
			{
				lstLookAt.Add(new MotionLookAtList());
				motionLookAtList = lstLookAt[lstLookAt.Count - 1];
			}
			else
			{
				motionLookAtList.MemberInit();
			}
			motionLookAtList.strMotion = motion;
			int.TryParse(data[i, num++], out motionLookAtList.numFemale);
			motionLookAtList.strLookAtNull = data[i, num++];
			motionLookAtList.bTopStick = data[i, num++] == "1";
			motionLookAtList.lookAtCalcKind = GlobalMethod.GetIntTryParse(data[i, num++]);
			for (int j = num; j < length2; j += 7)
			{
				ShapeInfo shapeInfo = new ShapeInfo();
				int num2 = 0;
				int result = 0;
				if (!int.TryParse(data[i, j + num2++], out result))
				{
					break;
				}
				shapeInfo.shape = result;
				shapeInfo.minPos = new Vector3(float.Parse(data[i, j + num2++]), float.Parse(data[i, j + num2++]), float.Parse(data[i, j + num2++]));
				shapeInfo.maxPos = new Vector3(float.Parse(data[i, j + num2++]), float.Parse(data[i, j + num2++]), float.Parse(data[i, j + num2++]));
				motionLookAtList.lstShape.Add(shapeInfo);
			}
		}
		if (lstLookAt.Count != 0)
		{
			SetInfo(lstLookAt[0]);
		}
		return true;
	}

	public void Release()
	{
		lstLookAt.Clear();
		lookat.SetTargetTransform(null);
		strPlayMotion = string.Empty;
		transLookAtNull = null;
		bTopStick = false;
	}

	private void LateUpdate()
	{
		if (male == null || females == null || male.objBodyBone == null || females[0].objBodyBone == null)
		{
			return;
		}
		SetLookAt();
		if (lstShape != null && transLookAtNull != null)
		{
			Vector3 position = transLookAtNull.position;
			for (int i = 0; i < lstShape.Count; i++)
			{
				float shapeBodyValue = females[numFemale].GetShapeBodyValue(lstShape[i].shape);
				Vector3 direction = ((!(shapeBodyValue >= 0.5f)) ? Vector3.Lerp(lstShape[i].minPos, Vector3.zero, Mathf.InverseLerp(0f, 0.5f, shapeBodyValue)) : Vector3.Lerp(Vector3.zero, lstShape[i].maxPos, Mathf.InverseLerp(0.5f, 1f, shapeBodyValue)));
				direction = transLookAtNull.TransformDirection(direction);
				position += direction;
			}
			transLookAtNull.position = position;
		}
		LookAtProc(transLookAtNull, bTopStick);
	}

	private bool SetLookAt()
	{
		AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
		if (animatorStateInfo.IsName(strPlayMotion))
		{
			return true;
		}
		foreach (MotionLookAtList item in lstLookAt)
		{
			if (!animatorStateInfo.IsName(item.strMotion))
			{
				continue;
			}
			SetInfo(item);
			break;
		}
		return true;
	}

	private bool SetInfo(MotionLookAtList _list)
	{
		if (_list == null)
		{
			return false;
		}
		if (females[_list.numFemale].objBodyBone == null)
		{
			transLookAtNull = null;
			lstShape = null;
			return false;
		}
		strPlayMotion = _list.strMotion;
		numFemale = _list.numFemale;
		if (_list.strLookAtNull == string.Empty)
		{
			transLookAtNull = null;
			lstShape = null;
		}
		else
		{
			GameObject gameObject = females[_list.numFemale].objBodyBone.transform.FindLoop(_list.strLookAtNull);
			transLookAtNull = ((!(gameObject != null)) ? null : gameObject.transform);
			lstShape = _list.lstShape;
		}
		bTopStick = _list.bTopStick;
		lookat.SetTargetTransform(transLookAtNull);
		lookat.SetOldRotation((!(objDanBase != null)) ? Quaternion.identity : objDanBase.transform.rotation);
		lookAtCalcKind = _list.lookAtCalcKind;
		return true;
	}

	private bool LookAtProc(Transform _transLook, bool _bPositon)
	{
		if (objDanBase == null)
		{
			return false;
		}
		if (lookAtCalcKind == 0)
		{
			lookat.Update();
		}
		else
		{
			objDanBase.transform.LookAt(_transLook);
		}
		if (_bPositon && objDanTop != null && _transLook != null)
		{
			objDanTop.transform.position = _transLook.position;
		}
		return true;
	}
}
