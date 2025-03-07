using UnityEngine;

public class NeckLookCalc : MonoBehaviour
{
	public static bool isEnabled = true;

	public Transform rootNode;

	public NeckObject neckObj;

	public Transform controlObj;

	public Vector3 headLookVector = Vector3.forward;

	public Vector3 headUpVector = Vector3.up;

	public NeckTypeState[] neckTypeStates;

	public float angleHRate;

	public float angleVRate;

	private int nowPtnNo;

	private bool initEnd;

	public float sorasiRate = 1f;

	public Quaternion fixAngle = Quaternion.identity;

	private Quaternion fixAngleBackup = Quaternion.identity;

	private float changeTypeTimer;

	public float changeTypeLeapTime = 1f;

	private NECK_LOOK_TYPE lookType;

	public Vector3 backupPos = Vector3.zero;

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
		Quaternion rotation = neckObj.neckTransform.parent.rotation;
		Quaternion quaternion = Quaternion.Inverse(rotation);
		neckObj.referenceLookDir = quaternion * rootNode.rotation * headLookVector.normalized;
		neckObj.referenceUpDir = quaternion * rootNode.rotation * headUpVector.normalized;
		neckObj.angleH = 0f;
		neckObj.angleV = 0f;
		neckObj.dirUp = neckObj.referenceUpDir;
		neckObj.origRotation = default(Quaternion);
		neckObj.origRotation = neckObj.neckTransform.localRotation;
		angleHRate = 0f;
		lookType = NECK_LOOK_TYPE.NO_LOOK;
		initEnd = true;
	}

	public void UpdateCall(int ptnNo)
	{
		if (ptnNo >= neckTypeStates.Length)
		{
			ptnNo = 0;
		}
		if (lookType != neckTypeStates[ptnNo].lookType)
		{
			lookType = neckTypeStates[ptnNo].lookType;
			changeTypeTimer = 0f;
			if (lookType == NECK_LOOK_TYPE.TARGET)
			{
				fixAngleBackup = neckObj.neckTransform.localRotation;
			}
			else
			{
				fixAngleBackup = fixAngle;
			}
		}
	}

	public void SetFixAngle(Quaternion angle)
	{
		fixAngleBackup = (fixAngle = angle);
		if (neckObj != null)
		{
			neckObj.neckTransform.localRotation = angle;
		}
		if (controlObj != null)
		{
			controlObj.localRotation = angle;
		}
	}

	public void NeckUpdateCalc(Vector3 target, int ptnNo)
	{
		backupPos = target;
		if (!initEnd)
		{
			return;
		}
		nowPtnNo = ptnNo;
		if (!isEnabled || Time.deltaTime == 0f)
		{
			return;
		}
		NeckTypeState neckTypeState = neckTypeStates[ptnNo];
		changeTypeTimer += Time.deltaTime;
		if (lookType == NECK_LOOK_TYPE.NO_LOOK)
		{
			fixAngle = neckObj.neckTransform.localRotation;
			float t = Mathf.InverseLerp(0f, changeTypeLeapTime, changeTypeTimer);
			neckObj.neckTransform.localRotation = Quaternion.Lerp(fixAngleBackup, fixAngle, t);
			if (controlObj != null)
			{
				controlObj.localRotation = fixAngle;
				if (controlObj.gameObject.activeSelf)
				{
					controlObj.gameObject.SetActive(false);
				}
			}
			return;
		}
		if (controlObj == null && lookType == NECK_LOOK_TYPE.CONTROL)
		{
			lookType = NECK_LOOK_TYPE.FIX;
		}
		if (controlObj != null)
		{
			if (lookType == NECK_LOOK_TYPE.CONTROL)
			{
				if (!controlObj.gameObject.activeSelf)
				{
					controlObj.gameObject.SetActive(true);
				}
				controlObj.gameObject.SetActive(true);
				fixAngle = controlObj.localRotation;
				float t2 = Mathf.InverseLerp(0f, changeTypeLeapTime, changeTypeTimer);
				neckObj.neckTransform.localRotation = Quaternion.Lerp(fixAngleBackup, fixAngle, t2);
				return;
			}
			if (controlObj.gameObject.activeSelf)
			{
				controlObj.gameObject.SetActive(false);
			}
		}
		if (lookType == NECK_LOOK_TYPE.FIX)
		{
			float t3 = Mathf.InverseLerp(0f, changeTypeLeapTime, changeTypeTimer);
			neckObj.neckTransform.localRotation = Quaternion.Lerp(fixAngleBackup, fixAngle, t3);
			if (controlObj != null)
			{
				controlObj.localRotation = fixAngle;
			}
			return;
		}
		Vector3 vector = target - rootNode.position;
		float num = Vector3.Distance(target, rootNode.position);
		if (num < neckTypeStates[ptnNo].nearDis)
		{
			vector = vector.normalized * neckTypeStates[ptnNo].nearDis;
			target = rootNode.position + vector;
		}
		float num2 = Vector3.Angle(new Vector3(vector.x, rootNode.forward.y, vector.z), rootNode.forward);
		float num3 = Vector3.Angle(new Vector3(rootNode.forward.x, vector.y, vector.z), rootNode.forward);
		bool flag = false;
		if (num2 > neckTypeStates[ptnNo].hAngleLimit || num3 > neckTypeStates[ptnNo].vAngleLimit)
		{
			flag = true;
		}
		if (flag || lookType == NECK_LOOK_TYPE.FORWARD)
		{
			target = rootNode.position + rootNode.forward * neckTypeStates[ptnNo].forntTagDis;
		}
		neckObj.neckTransform.localRotation = neckObj.origRotation;
		Quaternion rotation = neckObj.neckTransform.parent.rotation;
		Quaternion quaternion = Quaternion.Inverse(rotation);
		Vector3 normalized = (target - neckObj.neckTransform.position).normalized;
		Vector3 vector2 = quaternion * normalized;
		float f = AngleAroundAxis(neckObj.referenceLookDir, vector2, neckObj.referenceUpDir);
		Vector3 axis = Vector3.Cross(neckObj.referenceUpDir, vector2);
		Vector3 dirA = vector2 - Vector3.Project(vector2, neckObj.referenceUpDir);
		float f2 = AngleAroundAxis(dirA, vector2, axis);
		float f3 = Mathf.Max(0f, Mathf.Abs(f) - neckTypeState.thresholdAngleDifference) * Mathf.Sign(f);
		float f4 = Mathf.Max(0f, Mathf.Abs(f2) - neckTypeState.thresholdAngleDifference) * Mathf.Sign(f2);
		f = Mathf.Max(Mathf.Abs(f3) * Mathf.Abs(neckTypeState.bendingMultiplier), Mathf.Abs(f) - neckTypeState.maxAngleDifference) * Mathf.Sign(f) * Mathf.Sign(neckTypeState.bendingMultiplier);
		f2 = Mathf.Max(Mathf.Abs(f4) * Mathf.Abs(neckTypeState.bendingMultiplier), Mathf.Abs(f2) - neckTypeState.maxAngleDifference) * Mathf.Sign(f2) * Mathf.Sign(neckTypeState.bendingMultiplier);
		float maxBendingAngle = neckTypeState.maxBendingAngle;
		float minBendingAngle = neckTypeState.minBendingAngle;
		f = Mathf.Clamp(f, minBendingAngle, maxBendingAngle);
		f2 = Mathf.Clamp(f2, neckTypeState.upBendingAngle, neckTypeState.downBendingAngle);
		Vector3 axis2 = Vector3.Cross(neckObj.referenceUpDir, neckObj.referenceLookDir);
		if (lookType == NECK_LOOK_TYPE.AWAY)
		{
			float num4 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f - neckTypeStates[nowPtnNo].maxBendingAngle, 0f - neckTypeStates[nowPtnNo].minBendingAngle, neckObj.angleH));
			float num5 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f - neckTypeStates[nowPtnNo].maxBendingAngle, 0f - neckTypeStates[nowPtnNo].minBendingAngle, f));
			float num6 = num4 - num5;
			f = ((!(Mathf.Abs(num6) < sorasiRate)) ? neckObj.angleH : Mathf.Lerp(t: Mathf.InverseLerp(-1f, 1f, (num6 < 0f) ? ((!(num5 < 0f - sorasiRate)) ? (num5 - sorasiRate) : (num5 + sorasiRate)) : ((!(num6 > 0f)) ? (num5 + sorasiRate) : ((!(num5 > sorasiRate)) ? (num5 + sorasiRate) : (num5 - sorasiRate)))), a: 0f - neckTypeStates[nowPtnNo].maxBendingAngle, b: 0f - neckTypeStates[nowPtnNo].minBendingAngle));
			f2 = 0f - f2;
		}
		neckObj.angleH = Mathf.Lerp(neckObj.angleH, f, Time.deltaTime * neckTypeState.leapSpeed);
		neckObj.angleV = Mathf.Lerp(neckObj.angleV, f2, Time.deltaTime * neckTypeState.leapSpeed);
		vector2 = Quaternion.AngleAxis(neckObj.angleH, neckObj.referenceUpDir) * Quaternion.AngleAxis(neckObj.angleV, axis2) * neckObj.referenceLookDir;
		Vector3 tangent = neckObj.referenceUpDir;
		Vector3.OrthoNormalize(ref vector2, ref tangent);
		Vector3 normal = vector2;
		neckObj.dirUp = Vector3.Slerp(neckObj.dirUp, tangent, Time.deltaTime * 5f);
		Vector3.OrthoNormalize(ref normal, ref neckObj.dirUp);
		Quaternion quaternion2 = rotation * Quaternion.LookRotation(normal, neckObj.dirUp) * Quaternion.Inverse(rotation * Quaternion.LookRotation(neckObj.referenceLookDir, neckObj.referenceUpDir));
		neckObj.neckTransform.rotation = quaternion2 * neckObj.neckTransform.rotation;
		fixAngle = neckObj.neckTransform.localRotation;
		float t5 = Mathf.InverseLerp(0f, changeTypeLeapTime, changeTypeTimer);
		neckObj.neckTransform.localRotation = Quaternion.Lerp(fixAngleBackup, fixAngle, t5);
		if (controlObj != null)
		{
			controlObj.localRotation = fixAngle;
		}
		backupPos = target;
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
		if (neckObj != null)
		{
			angleHRate = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(0f - neckTypeStates[nowPtnNo].maxBendingAngle, 0f - neckTypeStates[nowPtnNo].minBendingAngle, neckObj.angleH));
		}
	}

	private float AngleVRateCalc()
	{
		if (neckObj != null)
		{
			if (neckTypeStates[nowPtnNo].downBendingAngle <= neckTypeStates[nowPtnNo].upBendingAngle)
			{
				if (0f <= neckObj.angleV)
				{
					return 0f - Mathf.InverseLerp(0f, neckTypeStates[nowPtnNo].upBendingAngle, neckObj.angleV);
				}
				return Mathf.InverseLerp(0f, neckTypeStates[nowPtnNo].downBendingAngle, neckObj.angleV);
			}
			if (0f <= neckObj.angleV)
			{
				return 0f - Mathf.InverseLerp(0f, neckTypeStates[nowPtnNo].downBendingAngle, neckObj.angleV);
			}
			return Mathf.InverseLerp(0f, neckTypeStates[nowPtnNo].upBendingAngle, neckObj.angleV);
		}
		return 0f;
	}

	public float GetAngleHRate()
	{
		return angleHRate;
	}

	public float GetAngleVRate()
	{
		return angleVRate;
	}
}
