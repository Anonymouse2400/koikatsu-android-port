using System;
using UnityEngine;

public class ExpressionHoney : MonoBehaviour
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

		public void Calculate()
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
				trfLookAt.rotation = LookAtQuat(xvec, yvec, zvec);
				return;
			}
			trfLookAt.rotation = LookAtQuat(xvec, yvec, zvec);
			ConvertRotation.RotationOrder order = (ConvertRotation.RotationOrder)rotOrder;
			Quaternion localRotation = trfLookAt.localRotation;
			Vector3 vector4 = ConvertRotation.ConvertDegreeFromQuaternion(order, localRotation);
			Quaternion q = Quaternion.Slerp(localRotation, Quaternion.identity, 0.5f);
			Vector3 vector5 = ConvertRotation.ConvertDegreeFromQuaternion(order, q);
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

		private Quaternion LookAtQuat(Vector3 xvec, Vector3 yvec, Vector3 zvec)
		{
			float num = 1f + xvec.x + yvec.y + zvec.z;
			if (num == 0f)
			{
				return Quaternion.identity;
			}
			float num2 = Mathf.Sqrt(num) / 2f;
			if (float.IsNaN(num2))
			{
				return Quaternion.identity;
			}
			float num3 = 4f * num2;
			if (num3 == 0f)
			{
				return Quaternion.identity;
			}
			float x = (yvec.z - zvec.y) / num3;
			float y = (zvec.x - xvec.z) / num3;
			float z = (xvec.y - yvec.x) / num3;
			return new Quaternion(x, y, z, num2);
		}
	}

	[Serializable]
	public class Correct
	{
		public enum CalcType
		{
			Euler = 0,
			Quaternion = 1
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

		public string correctName = string.Empty;

		public string referenceName = string.Empty;

		public CalcType calcType;

		public RotationOrder rotOrder = RotationOrder.ZXY;

		[Range(0f, 1f)]
		public float charmRate;

		public bool useRX;

		[Range(-1f, 1f)]
		public float valRXMin;

		[Range(-1f, 1f)]
		public float valRXMax;

		public bool useRY;

		[Range(-1f, 1f)]
		public float valRYMin;

		[Range(-1f, 1f)]
		public float valRYMax;

		public bool useRZ;

		[Range(-1f, 1f)]
		public float valRZMin;

		[Range(-1f, 1f)]
		public float valRZMax;

		public Transform trfCorrect { get; private set; }

		public Transform trfReference { get; private set; }

		public Correct()
		{
			trfCorrect = null;
			trfReference = null;
		}

		public void SetCorrectTransform(Transform trf)
		{
			trfCorrect = trf;
		}

		public void SetReferenceTransform(Transform trf)
		{
			trfReference = trf;
		}

		public void Calculate()
		{
			if (null == trfCorrect || null == trfReference)
			{
				return;
			}
			if (calcType == CalcType.Euler)
			{
				ConvertRotation.RotationOrder order = (ConvertRotation.RotationOrder)rotOrder;
				Vector3 vector = ConvertRotation.ConvertDegreeFromQuaternion(order, trfCorrect.localRotation);
				Vector3 vector2 = ConvertRotation.ConvertDegreeFromQuaternion(order, trfReference.localRotation);
				float num = 1f;
				Quaternion identity = Quaternion.identity;
				Vector3 vector3 = Vector3.zero;
				if (charmRate != 0f)
				{
					identity = Quaternion.Slerp(trfReference.localRotation, Quaternion.identity, charmRate);
					vector3 = ConvertRotation.ConvertDegreeFromQuaternion(order, identity);
				}
				if (useRX)
				{
					num = Mathf.InverseLerp(0f, 90f, Mathf.Clamp(Mathf.Abs(vector2.x), 0f, 90f));
					num = Mathf.Lerp(valRXMin, valRXMax, num);
					vector.x = vector2.x * num;
					if (charmRate != 0f && ((vector2.x < 0f && vector3.x > 0f) || (vector2.x > 0f && vector3.x < 0f)))
					{
						vector.x *= -1f;
					}
				}
				if (useRY)
				{
					num = Mathf.InverseLerp(0f, 90f, Mathf.Clamp(Mathf.Abs(vector2.y), 0f, 90f));
					num = Mathf.Lerp(valRYMin, valRYMax, num);
					vector.y = vector2.y * num;
					if (charmRate != 0f && ((vector2.y < 0f && vector3.y > 0f) || (vector2.y > 0f && vector3.y < 0f)))
					{
						vector.y *= -1f;
					}
				}
				if (useRZ)
				{
					num = Mathf.InverseLerp(0f, 90f, Mathf.Clamp(Mathf.Abs(vector2.z), 0f, 90f));
					num = Mathf.Lerp(valRZMin, valRZMax, num);
					vector.z = vector2.z * num;
					if (charmRate != 0f && ((vector2.z < 0f && vector3.z > 0f) || (vector2.z > 0f && vector3.z < 0f)))
					{
						vector.z *= -1f;
					}
				}
				trfCorrect.localRotation = ConvertRotation.ConvertDegreeToQuaternion(order, vector.x, vector.y, vector.z);
			}
			else if (calcType == CalcType.Quaternion)
			{
				Quaternion localRotation = trfCorrect.localRotation;
				if (useRX)
				{
					localRotation.x = trfReference.localRotation.x * (valRXMin + valRXMax) * 0.5f;
				}
				if (useRY)
				{
					localRotation.y = trfReference.localRotation.y * (valRYMin + valRYMax) * 0.5f;
				}
				if (useRZ)
				{
					localRotation.z = trfReference.localRotation.z * (valRZMin + valRZMax) * 0.5f;
				}
				trfCorrect.localRotation = localRotation;
			}
		}
	}
}
