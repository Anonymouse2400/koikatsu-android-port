using System;
using System.IO;
using UnityEngine;

public class EyeLookCalc : MonoBehaviour
{
	public static bool isEnabled = true;

	public bool correct = true;

	public Transform rootNode;

	[SerializeField]
	private Transform trfCenter;

	[SerializeField]
	private float closeEyeLength;

	[SerializeField]
	private float centerEyeLength;

	private GameObject objFrontCorrect;

	private GameObject objCalcTC_Front;

	private GameObject objCalcTC;

	private GameObject[] objCalcTLR;

	public EyeObject[] eyeObjs;

	public Vector3 headLookVector = Vector3.forward;

	public Vector3 headUpVector = Vector3.up;

	public EyeTypeState[] eyeTypeStates;

	public float[] angleHRate = new float[2];

	public float angleVRate;

	public float sorasiRate = 1f;

	private int nowPtnNo;

	private bool initEnd;

	public GameObject targetObj;

	[Range(0.5f, 5f)]
	public float targetObjMaxDir = 0.5f;

	private Vector3 targetPos = Vector3.zero;

	public Quaternion[] fixAngle = new Quaternion[2];

	public Vector3 TargetPos
	{
		get
		{
			return targetPos;
		}
		set
		{
			targetPos = value;
		}
	}

	private void Awake()
	{
		if (!initEnd)
		{
			Init();
		}
	}

	private void Start()
	{
		if (!initEnd)
		{
			Init();
		}
	}

	public void Init()
	{
		if (rootNode == null)
		{
			rootNode = base.transform;
		}
		if (null != trfCenter)
		{
			objFrontCorrect = new GameObject("n_cam_front");
			objFrontCorrect.transform.SetParent(rootNode, false);
			objFrontCorrect.transform.localEulerAngles = new Vector3(5f, 0f, 0f);
			objCalcTC = new GameObject("n_cam_tc");
			objCalcTC.transform.SetParent(trfCenter, false);
			objCalcTC_Front = new GameObject("n_cam_tc_f");
			objCalcTC_Front.transform.SetParent(objCalcTC.transform, false);
			objCalcTLR = new GameObject[2];
			string[] array = new string[2] { "n_cam_tl", "n_cam_tr" };
			for (int i = 0; i < 2; i++)
			{
				objCalcTLR[i] = new GameObject(array[i]);
				objCalcTLR[i].transform.SetParent(objCalcTC_Front.transform, false);
			}
		}
		EyeObject[] array2 = eyeObjs;
		foreach (EyeObject eyeObject in array2)
		{
			Quaternion rotation = eyeObject.eyeTransform.parent.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			eyeObject.referenceLookDir = quaternion * rootNode.rotation * headLookVector.normalized;
			eyeObject.referenceUpDir = quaternion * rootNode.rotation * headUpVector.normalized;
			eyeObject.angleH = 0f;
			eyeObject.angleV = 0f;
			eyeObject.dirUp = eyeObject.referenceUpDir;
			eyeObject.origRotation = default(Quaternion);
			eyeObject.origRotation = eyeObject.eyeTransform.localRotation;
		}
		angleHRate = new float[2];
		initEnd = true;
	}

	public void EyeUpdateCalc(Vector3 target, int ptnNo)
	{
		if (!initEnd)
		{
			if (targetObj != null && targetObj.activeSelf)
			{
				targetObj.SetActive(false);
			}
			return;
		}
		nowPtnNo = ptnNo;
		if (!isEnabled)
		{
			if (targetObj != null && targetObj.activeSelf)
			{
				targetObj.SetActive(false);
			}
			return;
		}
		if (Time.deltaTime == 0f)
		{
			if (targetObj != null && targetObj.activeSelf)
			{
				targetObj.SetActive(false);
			}
			return;
		}
		Vector3[] array = new Vector3[2] { target, target };
		EyeTypeState eyeTypeState = new EyeTypeState();
		eyeTypeState = eyeTypeStates[ptnNo];
		EYE_LOOK_TYPE eYE_LOOK_TYPE = EYE_LOOK_TYPE.NO_LOOK;
		eYE_LOOK_TYPE = eyeTypeStates[ptnNo].lookType;
		if (eYE_LOOK_TYPE == EYE_LOOK_TYPE.NO_LOOK)
		{
			eyeObjs[0].eyeTransform.localRotation = fixAngle[0];
			eyeObjs[1].eyeTransform.localRotation = fixAngle[1];
			if (targetObj != null && targetObj.activeSelf)
			{
				targetObj.SetActive(false);
			}
			AngleHRateCalc();
			angleVRate = AngleVRateCalc();
			return;
		}
		Vector3 vector = target - rootNode.position;
		float num = Vector3.Distance(target, rootNode.position);
		if (eYE_LOOK_TYPE != EYE_LOOK_TYPE.TARGET && num < eyeTypeStates[ptnNo].nearDis)
		{
			vector = vector.normalized * eyeTypeStates[ptnNo].nearDis;
			target = rootNode.position + vector;
		}
		float num2 = Vector3.Angle(new Vector3(vector.x, rootNode.forward.y, vector.z), rootNode.forward);
		float num3 = Vector3.Angle(new Vector3(rootNode.forward.x, vector.y, vector.z), rootNode.forward);
		if (num2 > eyeTypeStates[ptnNo].hAngleLimit || num3 > eyeTypeStates[ptnNo].vAngleLimit)
		{
			eYE_LOOK_TYPE = EYE_LOOK_TYPE.FORWARD;
		}
		if (eYE_LOOK_TYPE == EYE_LOOK_TYPE.FORWARD)
		{
			target = objFrontCorrect.transform.position + objFrontCorrect.transform.forward * eyeTypeStates[ptnNo].forntTagDis;
		}
		if (eYE_LOOK_TYPE == EYE_LOOK_TYPE.CONTROL)
		{
			if (targetObj != null)
			{
				if (!targetObj.activeSelf)
				{
					targetObj.SetActive(true);
				}
				target = Vector3.MoveTowards(rootNode.transform.position, targetObj.transform.position, eyeTypeStates[ptnNo].forntTagDis);
				targetObj.transform.position = Vector3.MoveTowards(rootNode.transform.position, target, targetObjMaxDir);
			}
		}
		else if (targetObj != null)
		{
			targetObj.transform.position = Vector3.MoveTowards(rootNode.transform.position, target, targetObjMaxDir);
			if (targetObj.activeSelf)
			{
				targetObj.SetActive(false);
			}
		}
		if (correct && (eYE_LOOK_TYPE == EYE_LOOK_TYPE.TARGET || eYE_LOOK_TYPE == EYE_LOOK_TYPE.FORWARD) && !(null == objCalcTC_Front) && !(null == objCalcTC) && objCalcTLR != null && !(null == objCalcTLR[0]) && !(null == objCalcTLR[1]))
		{
			Vector3 position = target;
			Vector3 position2 = trfCenter.InverseTransformPoint(position);
			if (position2.z < 0.5f)
			{
				position2.z = 0.5f;
			}
			position = trfCenter.TransformPoint(position2);
			float num4 = centerEyeLength;
			objCalcTLR[0].transform.localPosition = new Vector3(0f - num4, 0f, 0f);
			objCalcTLR[1].transform.localPosition = new Vector3(num4, 0f, 0f);
			objCalcTC_Front.transform.position = position;
			Vector3 normalized = (position - trfCenter.position).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.up, normalized).normalized;
			Vector3 normalized3 = Vector3.Cross(normalized2, Vector3.up).normalized;
			objCalcTC_Front.transform.rotation = Quaternion.LookRotation(normalized3, Vector3.up);
			array[0] = objCalcTLR[0].transform.position;
			array[1] = objCalcTLR[1].transform.position;
		}
		float num5 = -1f;
		for (int i = 0; i < eyeObjs.Length; i++)
		{
			EyeObject eyeObject = eyeObjs[i];
			eyeObject.eyeTransform.localRotation = eyeObject.origRotation;
			Quaternion rotation = eyeObject.eyeTransform.parent.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Vector3 zero = Vector3.zero;
			zero = ((eYE_LOOK_TYPE != EYE_LOOK_TYPE.TARGET && eYE_LOOK_TYPE != EYE_LOOK_TYPE.FORWARD) ? (target - eyeObject.eyeTransform.position).normalized : (array[i] - eyeObject.eyeTransform.position).normalized);
			Vector3 vector2 = quaternion * zero;
			float f = AngleAroundAxis(eyeObject.referenceLookDir, vector2, eyeObject.referenceUpDir);
			Vector3 axis = Vector3.Cross(eyeObject.referenceUpDir, vector2);
			Vector3 dirA = vector2 - Vector3.Project(vector2, eyeObject.referenceUpDir);
			float f2 = AngleAroundAxis(dirA, vector2, axis);
			float f3 = Mathf.Max(0f, Mathf.Abs(f) - eyeTypeState.thresholdAngleDifference) * Mathf.Sign(f);
			float f4 = Mathf.Max(0f, Mathf.Abs(f2) - eyeTypeState.thresholdAngleDifference) * Mathf.Sign(f2);
			f = Mathf.Max(Mathf.Abs(f3) * Mathf.Abs(eyeTypeState.bendingMultiplier), Mathf.Abs(f) - eyeTypeState.maxAngleDifference) * Mathf.Sign(f) * Mathf.Sign(eyeTypeState.bendingMultiplier);
			f2 = Mathf.Max(Mathf.Abs(f4) * Mathf.Abs(eyeTypeState.bendingMultiplier), Mathf.Abs(f2) - eyeTypeState.maxAngleDifference) * Mathf.Sign(f2) * Mathf.Sign(eyeTypeState.bendingMultiplier);
			float max = eyeTypeState.maxBendingAngle;
			float min = eyeTypeState.minBendingAngle;
			if (eyeObject.eyeLR == EYE_LR.EYE_R)
			{
				max = 0f - eyeTypeState.minBendingAngle;
				min = 0f - eyeTypeState.maxBendingAngle;
			}
			f = Mathf.Clamp(f, min, max);
			f2 = Mathf.Clamp(f2, eyeTypeState.upBendingAngle, eyeTypeState.downBendingAngle);
			Vector3 axis2 = Vector3.Cross(eyeObject.referenceUpDir, eyeObject.referenceLookDir);
			if (eYE_LOOK_TYPE == EYE_LOOK_TYPE.AWAY)
			{
				if (num5 == -1f)
				{
					float num6 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f - eyeTypeStates[nowPtnNo].maxBendingAngle, 0f - eyeTypeStates[nowPtnNo].minBendingAngle, eyeObject.angleH));
					float num7 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f - eyeTypeStates[nowPtnNo].maxBendingAngle, 0f - eyeTypeStates[nowPtnNo].minBendingAngle, f));
					float num8 = num6 - num7;
					if (Mathf.Abs(num8) < sorasiRate)
					{
						num6 = ((num8 < 0f) ? ((!(num7 < 0f - sorasiRate)) ? (num7 - sorasiRate) : (num7 + sorasiRate)) : ((!(num8 > 0f)) ? (num7 + sorasiRate) : ((!(num7 > sorasiRate)) ? (num7 + sorasiRate) : (num7 - sorasiRate))));
						num5 = Mathf.InverseLerp(-1f, 1f, num6);
						f = Mathf.Lerp(0f - eyeTypeStates[nowPtnNo].maxBendingAngle, 0f - eyeTypeStates[nowPtnNo].minBendingAngle, num5);
					}
					else
					{
						num5 = Mathf.InverseLerp(-1f, 1f, num6);
						f = eyeObject.angleH;
					}
				}
				else
				{
					f = Mathf.Lerp(0f - eyeTypeStates[nowPtnNo].maxBendingAngle, 0f - eyeTypeStates[nowPtnNo].minBendingAngle, num5);
				}
				f2 = 0f - f2;
			}
			float t = Time.deltaTime * eyeTypeState.leapSpeed;
			eyeObject.angleH = Mathf.Lerp(eyeObject.angleH, f, t);
			eyeObject.angleV = Mathf.Lerp(eyeObject.angleV, f2, t);
			vector2 = Quaternion.AngleAxis(eyeObject.angleH, eyeObject.referenceUpDir) * Quaternion.AngleAxis(eyeObject.angleV, axis2) * eyeObject.referenceLookDir;
			Vector3 tangent = eyeObject.referenceUpDir;
			Vector3.OrthoNormalize(ref vector2, ref tangent);
			Vector3 normal = vector2;
			eyeObject.dirUp = Vector3.Slerp(eyeObject.dirUp, tangent, Time.deltaTime * 5f);
			Vector3.OrthoNormalize(ref normal, ref eyeObject.dirUp);
			Quaternion quaternion2 = rotation * Quaternion.LookRotation(normal, eyeObject.dirUp) * Quaternion.Inverse(rotation * Quaternion.LookRotation(eyeObject.referenceLookDir, eyeObject.referenceUpDir));
			eyeObject.eyeTransform.rotation = quaternion2 * eyeObject.eyeTransform.rotation;
		}
		targetPos = target;
		fixAngle[0] = eyeObjs[0].eyeTransform.localRotation;
		fixAngle[1] = eyeObjs[1].eyeTransform.localRotation;
		AngleHRateCalc();
		angleVRate = AngleVRateCalc();
	}

	public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		float num = Vector3.Angle(dirA, dirB);
		return num * (float)((!(Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f)) ? 1 : (-1));
	}

	public void setEnable(bool setFlag)
	{
		isEnabled = setFlag;
	}

	private void AngleHRateCalc()
	{
		for (int i = 0; i < 2; i++)
		{
			if (eyeObjs[i] != null)
			{
				if (eyeObjs[i].eyeLR == EYE_LR.EYE_R)
				{
					angleHRate[i] = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f - eyeTypeStates[nowPtnNo].maxBendingAngle, 0f - eyeTypeStates[nowPtnNo].minBendingAngle, eyeObjs[i].angleH));
				}
				else
				{
					angleHRate[i] = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(eyeTypeStates[nowPtnNo].minBendingAngle, eyeTypeStates[nowPtnNo].maxBendingAngle, eyeObjs[i].angleH));
				}
			}
		}
	}

	private float AngleVRateCalc()
	{
		if (eyeObjs[0] != null)
		{
			if (eyeTypeStates[nowPtnNo].downBendingAngle <= eyeTypeStates[nowPtnNo].upBendingAngle)
			{
				if (0f <= eyeObjs[0].angleV)
				{
					return 0f - Mathf.InverseLerp(0f, eyeTypeStates[nowPtnNo].upBendingAngle, eyeObjs[0].angleV);
				}
				return Mathf.InverseLerp(0f, eyeTypeStates[nowPtnNo].downBendingAngle, eyeObjs[0].angleV);
			}
			if (0f <= eyeObjs[0].angleV)
			{
				return 0f - Mathf.InverseLerp(0f, eyeTypeStates[nowPtnNo].downBendingAngle, eyeObjs[0].angleV);
			}
			return Mathf.InverseLerp(0f, eyeTypeStates[nowPtnNo].upBendingAngle, eyeObjs[0].angleV);
		}
		return 0f;
	}

	public float GetAngleHRate(EYE_LR eyeLR)
	{
		if (eyeLR == EYE_LR.EYE_L)
		{
			return angleHRate[0];
		}
		return angleHRate[1];
	}

	public float GetAngleVRate()
	{
		return angleVRate;
	}

	public void SaveAngle(BinaryWriter writer)
	{
		fixAngle[0] = eyeObjs[0].eyeTransform.localRotation;
		fixAngle[1] = eyeObjs[1].eyeTransform.localRotation;
		writer.Write(fixAngle[0].x);
		writer.Write(fixAngle[0].y);
		writer.Write(fixAngle[0].z);
		writer.Write(fixAngle[0].w);
		writer.Write(fixAngle[1].x);
		writer.Write(fixAngle[1].y);
		writer.Write(fixAngle[1].z);
		writer.Write(fixAngle[1].w);
		writer.Write(eyeObjs[0].angleH);
		writer.Write(eyeObjs[1].angleH);
		writer.Write(eyeObjs[0].angleV);
		writer.Write(eyeObjs[1].angleV);
	}

	public void LoadAngle(BinaryReader reader, Version version)
	{
		fixAngle[0] = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		fixAngle[1] = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		eyeObjs[0].eyeTransform.localRotation = fixAngle[0];
		eyeObjs[1].eyeTransform.localRotation = fixAngle[1];
		if (version.CompareTo(new Version(0, 0, 8)) >= 0)
		{
			eyeObjs[0].angleH = reader.ReadSingle();
			eyeObjs[1].angleH = reader.ReadSingle();
			eyeObjs[0].angleV = reader.ReadSingle();
			eyeObjs[1].angleV = reader.ReadSingle();
		}
	}
}
