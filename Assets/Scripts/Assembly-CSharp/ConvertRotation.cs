using UnityEngine;

public class ConvertRotation
{
	public enum RotationOrder
	{
		xyz = 0,
		xzy = 1,
		yxz = 2,
		yzx = 3,
		zxy = 4,
		zyx = 5
	}

	public static void CheckNaN(ref float val, float correct = 0f)
	{
		if (float.IsNaN(val))
		{
			val = correct;
		}
	}

	public static Quaternion ConvertDegreeToQuaternion(RotationOrder order, float x, float y, float z)
	{
		Quaternion quaternion = Quaternion.AngleAxis(x, Vector3.right);
		Quaternion quaternion2 = Quaternion.AngleAxis(y, Vector3.up);
		Quaternion quaternion3 = Quaternion.AngleAxis(z, Vector3.forward);
		switch (order)
		{
		case RotationOrder.xyz:
			return quaternion * quaternion2 * quaternion3;
		case RotationOrder.xzy:
			return quaternion * quaternion3 * quaternion2;
		case RotationOrder.yxz:
			return quaternion2 * quaternion * quaternion3;
		case RotationOrder.yzx:
			return quaternion2 * quaternion3 * quaternion;
		case RotationOrder.zxy:
			return quaternion3 * quaternion * quaternion2;
		case RotationOrder.zyx:
			return quaternion3 * quaternion2 * quaternion;
		default:
			return Quaternion.identity;
		}
	}

	public static Vector3 ConvertDegreeFromQuaternion(RotationOrder order, Quaternion q)
	{
		Vector3 vector = ConvertRadianFromQuaternion(order, q);
		return new Vector3(vector.x * 57.29578f, vector.y * 57.29578f, vector.z * 57.29578f);
	}

	public static Vector3 ConvertRadianFromQuaternion(RotationOrder order, Quaternion q)
	{
		Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
		return ConvertRadianFromMatrix(order, m);
	}

	public static Vector3 ConvertRadianFromMatrix(RotationOrder order, Matrix4x4 m)
	{
		switch (order)
		{
		case RotationOrder.xyz:
		{
			float num = m.m02;
			float y = Mathf.Asin(Mathf.Clamp(num, -1f, 1f));
			float val2;
			float val;
			if (num > 0.9999f)
			{
				val2 = 0f;
				val = Mathf.Atan2(m.m21, m.m11);
				CheckNaN(ref val);
			}
			else
			{
				val = Mathf.Atan2(0f - m.m12, m.m22);
				CheckNaN(ref val);
				val2 = Mathf.Atan2(0f - m.m01, m.m00);
				CheckNaN(ref val2);
			}
			return new Vector3(val, y, val2);
		}
		case RotationOrder.xzy:
		{
			float num = 0f - m.m01;
			float val2 = Mathf.Asin(Mathf.Clamp(num, -1f, 1f));
			float y;
			float val;
			if (num > 0.9999f)
			{
				y = 0f;
				val = Mathf.Atan2(0f - m.m12, m.m22);
				CheckNaN(ref val);
			}
			else
			{
				val = Mathf.Atan2(m.m21, m.m11);
				CheckNaN(ref val);
				y = Mathf.Atan2(m.m02, m.m00);
				CheckNaN(ref y);
			}
			return new Vector3(val, y, val2);
		}
		case RotationOrder.yxz:
		{
			float num = 0f - m.m12;
			float val = Mathf.Asin(Mathf.Clamp(num, -1f, 1f));
			float val2;
			float y;
			if (num > 0.9999f)
			{
				val2 = 0f;
				y = Mathf.Atan2(0f - m.m20, m.m00);
				CheckNaN(ref y);
			}
			else
			{
				y = Mathf.Atan2(m.m02, m.m22);
				CheckNaN(ref y);
				val2 = Mathf.Atan2(m.m10, m.m11);
				CheckNaN(ref val2);
			}
			return new Vector3(val, y, val2);
		}
		case RotationOrder.yzx:
		{
			float num = m.m10;
			float val2 = Mathf.Asin(Mathf.Clamp(num, -1f, 1f));
			float val;
			float y;
			if (num > 0.9999f)
			{
				val = 0f;
				y = Mathf.Atan2(m.m02, m.m22);
				CheckNaN(ref y);
			}
			else
			{
				val = Mathf.Atan2(0f - m.m12, m.m11);
				CheckNaN(ref val);
				y = Mathf.Atan2(0f - m.m20, m.m00);
				CheckNaN(ref y);
			}
			return new Vector3(val, y, val2);
		}
		case RotationOrder.zxy:
		{
			float num = m.m21;
			float val = Mathf.Asin(Mathf.Clamp(num, -1f, 1f));
			float y;
			float val2;
			if (num > 0.9999f)
			{
				y = 0f;
				val2 = Mathf.Atan2(m.m10, m.m00);
				CheckNaN(ref val2);
			}
			else
			{
				y = Mathf.Atan2(0f - m.m20, m.m22);
				CheckNaN(ref y);
				val2 = Mathf.Atan2(0f - m.m01, m.m11);
				CheckNaN(ref val2);
			}
			return new Vector3(val, y, val2);
		}
		case RotationOrder.zyx:
		{
			float num = 0f - m.m20;
			float y = Mathf.Asin(Mathf.Clamp(num, -1f, 1f));
			float val;
			float val2;
			if (num > 0.9999f)
			{
				val = 0f;
				val2 = Mathf.Atan2(0f - m.m01, m.m11);
				CheckNaN(ref val2);
			}
			else
			{
				val = Mathf.Atan2(m.m21, m.m22);
				CheckNaN(ref val);
				val2 = Mathf.Atan2(m.m10, m.m00);
				CheckNaN(ref val2);
			}
			return new Vector3(val, y, val2);
		}
		default:
			return Vector3.zero;
		}
	}
}
