using UnityEngine;

public class NeckLookCalcVer2 : MonoBehaviour
{
	public bool isEnabled = true;

	public Transform transformAim;

	public Transform boneCalcAngle;

	public NeckObjectVer2[] aBones;

	public NeckTypeStateVer2[] neckTypeStates;

	[Tooltip("表示するパターン番号")]
	public int ptnDraw;

	public float drawLineLength = 1f;

	public float changeTypeLeapTime = 1f;

	public AnimationCurve changeTypeLerpCurve = new AnimationCurve();

	[Tooltip("無条件でこれだけ回る")]
	public Vector2 nowAngle;

	[Range(0f, 1f)]
	public float calcLerp = 1f;

	public bool skipCalc;

	private int nowPtnNo;

	private bool initEnd;

	private Vector3 backupPos = Vector3.zero;

	private float changeTypeTimer;

	private NECK_LOOK_TYPE_VER2 lookType;

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

	private void OnDrawGizmos()
	{
		if ((bool)boneCalcAngle)
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
			if (neckTypeStates.Length > ptnDraw)
			{
				NeckTypeStateVer2 neckTypeStateVer = neckTypeStates[ptnDraw];
				Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
				Vector3 vector = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, neckTypeStateVer.hAngleLimit, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector);
				Vector3 vector2 = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, 0f - neckTypeStateVer.hAngleLimit, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector2);
				Vector3 vector3 = boneCalcAngle.TransformDirection(Quaternion.Euler(neckTypeStateVer.vAngleLimit, 0f, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector3);
				Vector3 vector4 = boneCalcAngle.TransformDirection(Quaternion.Euler(0f - neckTypeStateVer.vAngleLimit, 0f, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector4);
				Gizmos.DrawLine(vector, vector4);
				Gizmos.DrawLine(vector4, vector2);
				Gizmos.DrawLine(vector2, vector3);
				Gizmos.DrawLine(vector3, vector);
				if (neckTypeStateVer.limitBreakCorrectionValue != 0f)
				{
					Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
					vector = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, neckTypeStateVer.hAngleLimit + neckTypeStateVer.limitBreakCorrectionValue, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
					Gizmos.DrawLine(boneCalcAngle.position, vector);
					vector2 = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, 0f - neckTypeStateVer.hAngleLimit - neckTypeStateVer.limitBreakCorrectionValue, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
					Gizmos.DrawLine(boneCalcAngle.position, vector2);
					vector3 = boneCalcAngle.TransformDirection(Quaternion.Euler(neckTypeStateVer.vAngleLimit + neckTypeStateVer.limitBreakCorrectionValue, 0f, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
					Gizmos.DrawLine(boneCalcAngle.position, vector3);
					vector4 = boneCalcAngle.TransformDirection(Quaternion.Euler(0f - neckTypeStateVer.vAngleLimit - neckTypeStateVer.limitBreakCorrectionValue, 0f, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
					Gizmos.DrawLine(boneCalcAngle.position, vector4);
					Gizmos.DrawLine(vector, vector4);
					Gizmos.DrawLine(vector4, vector2);
					Gizmos.DrawLine(vector2, vector3);
					Gizmos.DrawLine(vector3, vector);
				}
				Gizmos.color = new Color(1f, 0f, 1f, 0.8f);
				float num = 0f;
				for (int i = 0; i < neckTypeStateVer.aParam.Length; i++)
				{
					num += neckTypeStateVer.aParam[i].maxBendingAngle;
				}
				vector = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, num, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector);
				vector2 = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, num - neckTypeStateVer.limitAway, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector2);
				Gizmos.DrawLine(vector, vector2);
				num = 0f;
				for (int j = 0; j < neckTypeStateVer.aParam.Length; j++)
				{
					num += neckTypeStateVer.aParam[j].minBendingAngle;
				}
				vector = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, num, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector);
				vector2 = boneCalcAngle.TransformDirection(Quaternion.Euler(0f, num + neckTypeStateVer.limitAway, 0f) * Vector3.forward * drawLineLength) + boneCalcAngle.position;
				Gizmos.DrawLine(boneCalcAngle.position, vector2);
				Gizmos.DrawLine(vector, vector2);
			}
		}
		Gizmos.color = Color.white;
	}

	public void Init()
	{
		NeckObjectVer2[] array = aBones;
		foreach (NeckObjectVer2 neckObjectVer in array)
		{
			if (neckObjectVer.referenceCalc == null)
			{
				neckObjectVer.referenceCalc = base.transform;
			}
			neckObjectVer.angleH = 0f;
			neckObjectVer.angleV = 0f;
			neckObjectVer.angleHRate = 0f;
			neckObjectVer.angleVRate = 0f;
		}
		lookType = NECK_LOOK_TYPE_VER2.ANIMATION;
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
			for (int i = 0; i < aBones.Length; i++)
			{
				aBones[i].fixAngleBackup = aBones[i].fixAngle;
			}
			if (lookType == NECK_LOOK_TYPE_VER2.FORWARD)
			{
				for (int j = 0; j < aBones.Length; j++)
				{
					aBones[j].angleH = 0f;
					aBones[j].angleV = 0f;
				}
			}
		}
		if (lookType == NECK_LOOK_TYPE_VER2.TARGET)
		{
			for (int k = 0; k < aBones.Length; k++)
			{
				aBones[k].backupLocalRotaionByTarget = aBones[k].neckBone.localRotation;
				aBones[k].neckBone.localRotation = aBones[k].fixAngle;
			}
		}
	}

	public void NeckUpdateCalc(Vector3 target, int ptnNo, bool _isUseBackUpPos = false)
	{
		if (!initEnd)
		{
			return;
		}
		nowPtnNo = ptnNo;
		if (!isEnabled || (!skipCalc && Time.deltaTime == 0f))
		{
			return;
		}
		NeckTypeStateVer2 neckTypeStateVer = neckTypeStates[nowPtnNo];
		if (!_isUseBackUpPos)
		{
			backupPos = target;
		}
		if (neckTypeStateVer.aParam.Length != aBones.Length)
		{
			return;
		}
		if (skipCalc)
		{
			changeTypeTimer = changeTypeLeapTime;
		}
		changeTypeTimer = Mathf.Clamp(changeTypeTimer + Time.deltaTime, 0f, changeTypeLeapTime);
		float num = Mathf.InverseLerp(0f, changeTypeLeapTime, changeTypeTimer);
		if (changeTypeLerpCurve != null)
		{
			num = changeTypeLerpCurve.Evaluate(num);
		}
		if (neckTypeStateVer.lookType == NECK_LOOK_TYPE_VER2.ANIMATION)
		{
			Vector2 _Angle = GetAngleToTarget(target, aBones[aBones.Length - 1], 1f);
			for (int num2 = aBones.Length - 1; num2 > -1; num2--)
			{
				MaxRotateToAngle(aBones[num2], neckTypeStateVer, num2, ref _Angle);
			}
			for (int i = 0; i < aBones.Length; i++)
			{
				aBones[i].fixAngle = aBones[i].neckBone.localRotation;
				aBones[i].neckBone.localRotation = Quaternion.Slerp(aBones[i].fixAngleBackup, aBones[i].fixAngle, num);
			}
			return;
		}
		if (neckTypeStateVer.lookType == NECK_LOOK_TYPE_VER2.FORWARD)
		{
			for (int j = 0; j < aBones.Length; j++)
			{
				aBones[j].fixAngle = Quaternion.identity;
				Quaternion b = Quaternion.Slerp(aBones[j].neckBone.localRotation, aBones[j].fixAngle, calcLerp);
				aBones[j].neckBone.localRotation = Quaternion.Slerp(aBones[j].fixAngleBackup, b, num);
			}
			return;
		}
		if (neckTypeStateVer.lookType == NECK_LOOK_TYPE_VER2.FIX)
		{
			for (int k = 0; k < aBones.Length; k++)
			{
				Quaternion b2 = Quaternion.Slerp(aBones[k].neckBone.localRotation, aBones[k].fixAngle, calcLerp);
				aBones[k].neckBone.localRotation = Quaternion.Slerp(aBones[k].fixAngleBackup, b2, num);
			}
			return;
		}
		Vector3 dirB = target - boneCalcAngle.position;
		float f = AngleAroundAxis(boneCalcAngle.forward, dirB, boneCalcAngle.up);
		float f2 = AngleAroundAxis(boneCalcAngle.forward, dirB, boneCalcAngle.right);
		bool flag = false;
		float num3 = ((!neckTypeStateVer.isLimitBreakBackup) ? neckTypeStates[ptnNo].limitBreakCorrectionValue : 0f);
		if (Mathf.Abs(f) > neckTypeStates[ptnNo].hAngleLimit + num3 || Mathf.Abs(f2) > neckTypeStates[ptnNo].vAngleLimit + num3)
		{
			flag = true;
		}
		neckTypeStateVer.isLimitBreakBackup = flag;
		if (flag)
		{
			nowAngle = Vector2.zero;
		}
		else
		{
			if (_isUseBackUpPos)
			{
				target = backupPos;
			}
			nowAngle = GetAngleToTarget(target, aBones[aBones.Length - 1], 1f);
			if (neckTypeStateVer.lookType == NECK_LOOK_TYPE_VER2.TARGET)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(transformAim.position, boneCalcAngle.rotation, transformAim.lossyScale);
				Vector3 vector = matrix4x.inverse.MultiplyPoint3x4(target);
				Vector3 vector2 = matrix4x.inverse.MultiplyPoint3x4(boneCalcAngle.position);
				if (vector.z < 0f && vector.y < 0f)
				{
					if (vector2.x < 0f)
					{
						if (vector2.x < vector.x && vector.x < 0f)
						{
							target = transformAim.position;
						}
					}
					else if (vector2.x > vector.x && vector.x > 0f)
					{
						target = transformAim.position;
					}
				}
				if ((transformAim.position - target).magnitude == 0f)
				{
					for (int l = 0; l < aBones.Length; l++)
					{
						CalcNeckBone(aBones[l]);
						aBones[l].neckBone.localRotation = Quaternion.Slerp(aBones[l].fixAngleBackup, aBones[l].fixAngle, num);
					}
					return;
				}
			}
			else if (neckTypeStateVer.lookType == NECK_LOOK_TYPE_VER2.AWAY)
			{
				float num4 = 0f;
				float num5 = 0f;
				for (int m = 0; m < aBones.Length; m++)
				{
					num4 += aBones[m].angleH;
				}
				if (nowAngle.y <= num4)
				{
					for (int n = 0; n < aBones.Length; n++)
					{
						num5 += neckTypeStateVer.aParam[n].maxBendingAngle;
					}
					if (nowAngle.y <= num5 - neckTypeStateVer.limitAway || nowAngle.y < 0f)
					{
						nowAngle.y = num5;
					}
					else
					{
						nowAngle.y = 0f;
						for (int num6 = 0; num6 < aBones.Length; num6++)
						{
							nowAngle.y += neckTypeStateVer.aParam[num6].minBendingAngle;
						}
					}
				}
				else if (nowAngle.y > num4)
				{
					for (int num7 = 0; num7 < aBones.Length; num7++)
					{
						num5 += neckTypeStateVer.aParam[num7].minBendingAngle;
					}
					if (nowAngle.y >= num5 + neckTypeStateVer.limitAway || nowAngle.y > 0f)
					{
						nowAngle.y = num5;
					}
					else
					{
						nowAngle.y = 0f;
						for (int num8 = 0; num8 < aBones.Length; num8++)
						{
							nowAngle.y += neckTypeStateVer.aParam[num8].maxBendingAngle;
						}
					}
				}
				nowAngle.x = 0f - nowAngle.x;
			}
		}
		Vector2 _Angle2 = nowAngle;
		for (int num9 = aBones.Length - 1; num9 > -1; num9--)
		{
			RotateToAngle(aBones[num9], neckTypeStateVer, num9, ref _Angle2);
		}
		for (int num10 = 0; num10 < aBones.Length; num10++)
		{
			Quaternion b3 = Quaternion.Slerp(aBones[num10].backupLocalRotaionByTarget, aBones[num10].fixAngle, calcLerp);
			aBones[num10].neckBone.localRotation = Quaternion.Slerp(aBones[num10].fixAngleBackup, b3, num);
		}
	}

	private Vector2 GetAngleToTarget(Vector3 _targetPosition, NeckObjectVer2 _bone, float _weight)
	{
		Quaternion quaternion = Quaternion.FromToRotation(transformAim.rotation * Vector3.forward, _targetPosition - transformAim.position);
		Quaternion quaternion2 = quaternion * _bone.neckBone.rotation;
		float num = AngleAroundAxis(boneCalcAngle.forward, quaternion2 * Vector3.forward, boneCalcAngle.up);
		quaternion = Quaternion.AngleAxis(num, boneCalcAngle.up) * boneCalcAngle.rotation;
		Vector3 axis = Vector3.Cross(boneCalcAngle.up, quaternion * Vector3.forward);
		float x = AngleAroundAxis(quaternion * Vector3.forward, quaternion2 * Vector3.forward, axis);
		return new Vector2(x, num);
	}

	private void RotateToAngle(NeckObjectVer2 _bone, NeckTypeStateVer2 _param, int _boneNum, ref Vector2 _Angle)
	{
		float num = Mathf.Clamp(_Angle.y, _param.aParam[_boneNum].minBendingAngle, _param.aParam[_boneNum].maxBendingAngle);
		float num2 = Mathf.Clamp(_Angle.x, _param.aParam[_boneNum].upBendingAngle, _param.aParam[_boneNum].downBendingAngle);
		_Angle -= new Vector2(num2, num);
		float t = Mathf.Clamp01(Time.deltaTime * _param.leapSpeed);
		if (skipCalc)
		{
			t = 1f;
		}
		_bone.angleH = Mathf.Lerp(_bone.angleH, num, t);
		_bone.angleV = Mathf.Lerp(_bone.angleV, num2, t);
		CalcNeckBone(_bone);
	}

	private void CalcNeckBone(NeckObjectVer2 _bone)
	{
		Quaternion rotation = Quaternion.AngleAxis(_bone.angleH, _bone.referenceCalc.up) * Quaternion.AngleAxis(_bone.angleV, _bone.referenceCalc.right) * _bone.referenceCalc.rotation;
		_bone.neckBone.rotation = rotation;
		_bone.fixAngle = _bone.neckBone.localRotation;
	}

	private void MaxRotateToAngle(NeckObjectVer2 _bone, NeckTypeStateVer2 _param, int _boneNum, ref Vector2 _Angle)
	{
		float num = Mathf.Clamp(_Angle.y, _param.aParam[_boneNum].minBendingAngle, _param.aParam[_boneNum].maxBendingAngle);
		float num2 = Mathf.Clamp(_Angle.x, _param.aParam[_boneNum].upBendingAngle, _param.aParam[_boneNum].downBendingAngle);
		_Angle -= new Vector2(num2, num);
		_bone.angleH = num;
		_bone.angleV = num2;
	}

	private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
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
}
