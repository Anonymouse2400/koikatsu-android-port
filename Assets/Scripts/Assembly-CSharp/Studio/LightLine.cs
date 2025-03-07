using System;
using UnityEngine;

namespace Studio
{
	public static class LightLine
	{
		private static Material m_Material = null;

		private static Color m_Color = Color.white;

		private static float backfaceAlphaMultiplier = 0.2f;

		private static Color lineTransparency = new Color(1f, 1f, 1f, 0.75f);

		public static Shader shader { get; set; }

		public static Material material
		{
			get
			{
				if (m_Material == null)
				{
					CreateMaterial();
				}
				return m_Material;
			}
		}

		public static Color color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}

		public static void DrawLine(Light _light)
		{
			switch (_light.type)
			{
			case LightType.Point:
				m_Color = _light.color;
				DrawPointLight(Quaternion.identity, _light.transform.position, _light.range);
				break;
			case LightType.Spot:
				m_Color = _light.color;
				DrawSpotLight(_light.transform.rotation, _light.transform.position, _light.spotAngle, _light.range, 1f, 1f);
				break;
			}
		}

		private static void DrawPointLight(Quaternion _rotation, Vector3 _position, float _radius)
		{
			Vector3[] array = new Vector3[3]
			{
				_rotation * Vector3.right,
				_rotation * Vector3.up,
				_rotation * Vector3.forward
			};
			if (Camera.current.orthographic)
			{
				Vector3 forward = Camera.current.transform.forward;
				DrawWireDisc(_position, forward, _radius);
				for (int i = 0; i < 3; i++)
				{
					Vector3 normalized = Vector3.Cross(array[i], forward).normalized;
					DrawTwoShadedWireDisc(_position, array[i], normalized, 180f, _radius);
				}
				return;
			}
			Vector3 vector = _position - Camera.current.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			float num = _radius * _radius;
			float num2 = num * num / sqrMagnitude;
			float num3 = num2 / num;
			if (num3 < 1f)
			{
				DrawWireDisc(_position - num * vector / sqrMagnitude, vector, Mathf.Sqrt(num - num2));
			}
			for (int j = 0; j < 3; j++)
			{
				if (num3 < 1f)
				{
					float num4 = Vector3.Angle(vector, array[j]);
					num4 = 90f - Mathf.Min(num4, 180f - num4);
					float num5 = Mathf.Tan(num4 * ((float)Math.PI / 180f));
					float num6 = Mathf.Sqrt(num2 + num5 * num5 * num2) / _radius;
					if (num6 < 1f)
					{
						float num7 = Mathf.Asin(num6) * 57.29578f;
						Vector3 normalized2 = Vector3.Cross(array[j], vector).normalized;
						normalized2 = Quaternion.AngleAxis(num7, array[j]) * normalized2;
						DrawTwoShadedWireDisc(_position, array[j], normalized2, (90f - num7) * 2f, _radius);
					}
					else
					{
						DrawTwoShadedWireDisc(_position, array[j], _radius);
					}
				}
				else
				{
					DrawTwoShadedWireDisc(_position, array[j], _radius);
				}
			}
		}

		private static void DrawSpotLight(Quaternion _rotation, Vector3 _position, float _angle, float _range, float _angleScale, float _rangeScale)
		{
			float num = _range * _rangeScale;
			float num2 = num * Mathf.Tan((float)Math.PI / 180f * _angle / 2f) * _angleScale;
			Vector3 vector = _rotation * Vector3.forward;
			Vector3 vector2 = _rotation * Vector3.up;
			Vector3 vector3 = _rotation * Vector3.right;
			DrawLine(_position, _position + vector * num + vector2 * num2);
			DrawLine(_position, _position + vector * num - vector2 * num2);
			DrawLine(_position, _position + vector * num + vector3 * num2);
			DrawLine(_position, _position + vector * num - vector3 * num2);
			DrawWireDisc(_position + num * vector, vector, num2);
		}

		public static void DrawWireDisc(Vector3 _center, Vector3 _normal, float _radius)
		{
			Vector3 from = Vector3.Cross(_normal, Vector3.up);
			if (from.sqrMagnitude < 0.001f)
			{
				from = Vector3.Cross(_normal, Vector3.right);
			}
			DrawWireArc(_center, _normal, from, 360f, _radius);
		}

		public static void DrawWireArc(Vector3 _center, Vector3 _normal, Vector3 _from, float _angle, float _radius)
		{
			Vector3[] array = new Vector3[60];
			SetDiscSectionPoints(array, 60, _center, _normal, _from, _angle, _radius);
			DrawPolyLine(array);
		}

		public static void DrawPolyLine(params Vector3[] _points)
		{
			if (BeginLineDrawing(Matrix4x4.identity))
			{
				for (int i = 1; i < _points.Length; i++)
				{
					GL.Vertex(_points[i]);
					GL.Vertex(_points[i - 1]);
				}
				EndLineDrawing();
			}
		}

		public static void DrawLine(Vector3 p1, Vector3 p2)
		{
			if (BeginLineDrawing(Matrix4x4.identity))
			{
				GL.Vertex(p1);
				GL.Vertex(p2);
				EndLineDrawing();
			}
		}

		private static void DrawTwoShadedWireDisc(Vector3 _position, Vector3 _axis, Vector3 _from, float _degrees, float _radius)
		{
			DrawWireArc(_position, _axis, _from, _degrees, _radius);
			Color color = m_Color;
			Color color2 = color;
			color.a *= backfaceAlphaMultiplier;
			m_Color = color;
			DrawWireArc(_position, _axis, _from, _degrees - 360f, _radius);
			m_Color = color2;
		}

		private static void DrawTwoShadedWireDisc(Vector3 position, Vector3 axis, float radius)
		{
			Color color = m_Color;
			Color color2 = color;
			color.a *= backfaceAlphaMultiplier;
			m_Color = color;
			DrawWireDisc(position, axis, radius);
			m_Color = color2;
		}

		private static void SetDiscSectionPoints(Vector3[] _dest, int _count, Vector3 _center, Vector3 _normal, Vector3 _from, float _angle, float _radius)
		{
			_from.Normalize();
			Quaternion quaternion = Quaternion.AngleAxis(_angle / (float)(_count - 1), _normal);
			Vector3 vector = _from * _radius;
			for (int i = 0; i < _count; i++)
			{
				_dest[i] = _center + vector;
				vector = quaternion * vector;
			}
		}

		private static bool BeginLineDrawing(Matrix4x4 matrix)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return false;
			}
			Color value = m_Color * lineTransparency;
			material.SetPass(0);
			material.SetColor("_Color", value);
			GL.PushMatrix();
			GL.MultMatrix(matrix);
			GL.Begin(1);
			return true;
		}

		private static void EndLineDrawing()
		{
			GL.End();
			GL.PopMatrix();
		}

		private static void CreateMaterial()
		{
			Shader shader = ((!(LightLine.shader == null)) ? LightLine.shader : Shader.Find("Custom/LightLine"));
			if (!(shader == null))
			{
				m_Material = new Material(shader);
			}
		}
	}
}
