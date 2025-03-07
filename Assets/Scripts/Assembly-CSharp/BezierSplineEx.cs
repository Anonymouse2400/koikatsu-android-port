using System.Collections.Generic;
using UnityEngine;

public class BezierSplineEx : MonoBehaviour
{
	[SerializeField]
	private List<Vector3> points;

	[SerializeField]
	private List<BezierControlPointMode> modes;

	[SerializeField]
	private bool loop;

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
			if (value)
			{
				modes[modes.Count - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount
	{
		get
		{
			return points.Count;
		}
	}

	public int CurveCount
	{
		get
		{
			return (points.Count - 1) / 3;
		}
	}

	public Vector3 GetControlPoint(int index)
	{
		return points[index];
	}

	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 vector = point - points[index];
			if (loop)
			{
				if (index == 0)
				{
					points[1] += vector;
					points[points.Count - 2] += vector;
					points[points.Count - 1] = point;
				}
				else if (index == points.Count - 1)
				{
					points[0] = point;
					points[1] += vector;
					points[index - 1] += vector;
				}
				else
				{
					points[index - 1] += vector;
					points[index + 1] += vector;
				}
			}
			else
			{
				if (index > 0)
				{
					points[index - 1] += vector;
				}
				if (index + 1 < points.Count)
				{
					points[index + 1] += vector;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		modes[num] = mode;
		if (loop)
		{
			if (num == 0)
			{
				modes[modes.Count - 1] = mode;
			}
			else if (num == modes.Count - 1)
			{
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!loop && (num == 0 || num == modes.Count - 1)))
		{
			return;
		}
		int num2 = num * 3;
		int num3;
		int num4;
		if (index <= num2)
		{
			num3 = num2 - 1;
			if (num3 < 0)
			{
				num3 = points.Count - 2;
			}
			num4 = num2 + 1;
			if (num4 >= points.Count)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= points.Count)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = points.Count - 2;
			}
		}
		Vector3 vector = points[num2];
		Vector3 vector2 = vector - points[num3];
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			vector2 = vector2.normalized * Vector3.Distance(vector, points[num4]);
		}
		points[num4] = vector + vector2;
	}

	public Vector3 GetPoint(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = points.Count - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetPoint(points[num], points[num + 1], points[num + 2], points[num + 3], t));
	}

	public Vector3 GetVelocity(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = points.Count - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(points[num], points[num + 1], points[num + 2], points[num + 3], t)) - base.transform.position;
	}

	public Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}

	public void AddCurve()
	{
		InsertBackCurve(points.Count - 1);
	}

	public void InsertFrontCurve(int index)
	{
		int num = (index + 1) / 3;
		int num2 = num * 3;
		bool flag = num2 == points.Count - 1;
		Vector3 vector = points[num2];
		int index2 = num2;
		points.InsertRange(index2, new List<Vector3>
		{
			new Vector3(vector.x + 1f, vector.y, vector.z),
			new Vector3(vector.x + 2f, vector.y, vector.z),
			new Vector3(vector.x + 3f, vector.y, vector.z)
		});
		modes.Insert(num, modes[num]);
		for (int i = 0; i < points.Count; i += 3)
		{
			EnforceMode(i);
		}
		if (flag && loop)
		{
			points[points.Count - 1] = points[0];
			modes[modes.Count - 1] = modes[0];
			EnforceMode(0);
		}
	}

	public void InsertBackCurve(int index)
	{
		int num = (index + 1) / 3;
		int num2 = num * 3;
		bool flag = num2 == points.Count - 1;
		Vector3 vector = points[num2];
		int index2 = num2 + 1;
		points.InsertRange(index2, new List<Vector3>
		{
			new Vector3(vector.x + 1f, vector.y, vector.z),
			new Vector3(vector.x + 2f, vector.y, vector.z),
			new Vector3(vector.x + 3f, vector.y, vector.z)
		});
		modes.Insert(num + 1, modes[num]);
		for (int i = 0; i < points.Count; i += 3)
		{
			EnforceMode(i);
		}
		if (flag && loop)
		{
			points[points.Count - 1] = points[0];
			modes[modes.Count - 1] = modes[0];
			EnforceMode(0);
		}
	}

	public void RemoveCurve(ref int index)
	{
		if (points.Count <= 4)
		{
			return;
		}
		int num = (index + 1) / 3;
		int num2 = num * 3;
		if (num2 == 0)
		{
			for (int i = 0; i < 3; i++)
			{
				points.RemoveAt(0);
			}
		}
		else if (num2 == points.Count - 1)
		{
			for (int j = 0; j < 3; j++)
			{
				points.RemoveAt(points.Count - 1);
			}
			num2 = points.Count - 1;
		}
		else
		{
			for (int k = 0; k < 3; k++)
			{
				points.RemoveAt(num2 - 1);
			}
		}
		modes.RemoveAt(num);
		EnforceMode(num2);
		if (num2 == points.Count - 1 && loop)
		{
			points[points.Count - 1] = points[0];
			modes[modes.Count - 1] = modes[0];
			EnforceMode(0);
		}
		index = num2;
	}

	public void Reset()
	{
		points = new List<Vector3>
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new List<BezierControlPointMode>
		{
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}
}
