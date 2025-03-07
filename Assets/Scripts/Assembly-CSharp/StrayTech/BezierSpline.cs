using System;
using UnityEngine;

namespace StrayTech
{
	public class BezierSpline : MonoBehaviour
	{
		public enum BezierControlPointMode
		{
			Free = 0,
			Aligned = 1,
			Mirrored = 2
		}

		[SerializeField]
		private Vector3[] _points;

		[SerializeField]
		private BezierControlPointMode[] _modes;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		private int _interpolationAccuracy = 5;

		private float _length;

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
				if (value)
				{
					_modes[_modes.Length - 1] = _modes[0];
					SetControlPoint(0, _points[0]);
				}
			}
		}

		public float Length
		{
			get
			{
				return _length;
			}
		}

		public int CurveCount
		{
			get
			{
				return (_points.Length - 1) / 3;
			}
		}

		public int ControlPointCount
		{
			get
			{
				return _points.Length;
			}
		}

		private void Awake()
		{
			CalculateSplineLength();
		}

		public Vector3 GetControlPoint(int index)
		{
			return _points[index];
		}

		public void SetControlPoint(int index, Vector3 point)
		{
			if (index % 3 == 0)
			{
				Vector3 vector = point - _points[index];
				if (_loop)
				{
					if (index == 0)
					{
						_points[1] += vector;
						_points[_points.Length - 2] += vector;
						_points[_points.Length - 1] = point;
					}
					else if (index == _points.Length - 1)
					{
						_points[0] = point;
						_points[1] += vector;
						_points[index - 1] += vector;
					}
					else
					{
						_points[index - 1] += vector;
						_points[index + 1] += vector;
					}
				}
				else
				{
					if (index > 0)
					{
						_points[index - 1] += vector;
					}
					if (index + 1 < _points.Length)
					{
						_points[index + 1] += vector;
					}
				}
			}
			_points[index] = point;
			EnforceMode(index);
		}

		public BezierControlPointMode GetControlPointMode(int index)
		{
			return _modes[(index + 1) / 3];
		}

		public void SetControlPointMode(int index, BezierControlPointMode mode)
		{
			int num = (index + 1) / 3;
			_modes[num] = mode;
			if (_loop)
			{
				if (num == 0)
				{
					_modes[_modes.Length - 1] = mode;
				}
				else if (num == _modes.Length - 1)
				{
					_modes[0] = mode;
				}
			}
			EnforceMode(index);
		}

		private void EnforceMode(int index)
		{
			int num = (index + 1) / 3;
			BezierControlPointMode bezierControlPointMode = _modes[num];
			if (bezierControlPointMode == BezierControlPointMode.Free || (!_loop && (num == 0 || num == _modes.Length - 1)))
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
					num3 = _points.Length - 2;
				}
				num4 = num2 + 1;
				if (num4 >= _points.Length)
				{
					num4 = 1;
				}
			}
			else
			{
				num3 = num2 + 1;
				if (num3 >= _points.Length)
				{
					num3 = 1;
				}
				num4 = num2 - 1;
				if (num4 < 0)
				{
					num4 = _points.Length - 2;
				}
			}
			Vector3 vector = _points[num2];
			Vector3 vector2 = vector - _points[num3];
			if (bezierControlPointMode == BezierControlPointMode.Aligned)
			{
				vector2 = vector2.normalized * Vector3.Distance(vector, _points[num4]);
			}
			_points[num4] = vector + vector2;
		}

		public Vector3 GetPosition(float t)
		{
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = _points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			return base.transform.TransformPoint(Bezier.GetPoint(_points[num], _points[num + 1], _points[num + 2], _points[num + 3], t));
		}

		public Vector3 GetVelocity(float t)
		{
			int num;
			if (t >= 1f)
			{
				t = 1f;
				num = _points.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * (float)CurveCount;
				num = (int)t;
				t -= (float)num;
				num *= 3;
			}
			return base.transform.TransformPoint(Bezier.GetFirstDerivative(_points[num], _points[num + 1], _points[num + 2], _points[num + 3], t)) - base.transform.position;
		}

		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}

		public void AddCurve()
		{
			Vector3 vector = _points[_points.Length - 1];
			Array.Resize(ref _points, _points.Length + 3);
			vector.x += 1f;
			_points[_points.Length - 3] = vector;
			vector.x += 1f;
			_points[_points.Length - 2] = vector;
			vector.x += 1f;
			_points[_points.Length - 1] = vector;
			Array.Resize(ref _modes, _modes.Length + 1);
			_modes[_modes.Length - 1] = _modes[_modes.Length - 2];
			EnforceMode(_points.Length - 4);
			if (_loop)
			{
				_points[_points.Length - 1] = _points[0];
				_modes[_modes.Length - 1] = _modes[0];
				EnforceMode(0);
			}
		}

		public float GetClosestPointParam(Vector3 point, int iterations, float start = 0f, float end = 1f, float step = 0.01f)
		{
			iterations = Mathf.Clamp(iterations, 0, 5);
			float closestPointParamOnSegmentIntern = GetClosestPointParamOnSegmentIntern(point, start, end, step);
			for (int i = 0; i < iterations; i++)
			{
				float num = Mathf.Pow(10f, 0f - ((float)i + 2f));
				start = Mathf.Clamp01(closestPointParamOnSegmentIntern - num);
				end = Mathf.Clamp01(closestPointParamOnSegmentIntern + num);
				step = num * 0.1f;
				closestPointParamOnSegmentIntern = GetClosestPointParamOnSegmentIntern(point, start, end, step);
			}
			return closestPointParamOnSegmentIntern;
		}

		private float GetClosestPointParamOnSegmentIntern(Vector3 point, float start, float end, float step)
		{
			float num = float.PositiveInfinity;
			float result = 0f;
			for (float num2 = start; num2 <= end; num2 += step)
			{
				float sqrMagnitude = (point - GetPosition(num2)).sqrMagnitude;
				if (num > sqrMagnitude)
				{
					num = sqrMagnitude;
					result = num2;
				}
			}
			return result;
		}

		private void CalculateSplineLength()
		{
			float num = 1f / (float)_interpolationAccuracy;
			float num2 = 0f;
			Vector3 position = GetPosition(0f);
			float x = position.x;
			float y = position.y;
			float z = position.z;
			for (float num3 = num; (double)num3 < 1.0 + (double)num * 0.5; num3 += num)
			{
				position = GetPosition(num3);
				float num4 = x - position.x;
				float num5 = y - position.y;
				float num6 = z - position.z;
				num2 += Mathf.Sqrt(num4 * num4 + num5 * num5 + num6 * num6);
				x = position.x;
				y = position.y;
				z = position.z;
			}
			_length = num2;
		}

		public void Reset()
		{
			_points = new Vector3[4]
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(2f, 0f, 0f),
				new Vector3(3f, 0f, 0f),
				new Vector3(4f, 0f, 0f)
			};
			_modes = new BezierControlPointMode[2];
		}
	}
}
