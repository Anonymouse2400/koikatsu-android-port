using System;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalModeling : ImplicitSurface
{
	[Serializable]
	public class Brush
	{
		public enum Shape
		{
			sphere = 0,
			box = 1
		}

		public float fadeRadius = 0.1f;

		public float powerScale = 1f;

		public Matrix4x4 invTransform;

		public float sphereRadius = 0.5f;

		public Vector3 boxExtents = Vector3.one * 0.5f;

		public Shape shape;

		public Brush()
		{
		}

		public Brush(Shape shape_, Matrix4x4 invTransformMtx_, float fadeRadius_, float powerScale_, float sphereRadius_, Vector3 boxExtents_)
		{
			shape = shape_;
			fadeRadius = fadeRadius_;
			powerScale = powerScale_;
			invTransform = invTransformMtx_;
			sphereRadius = sphereRadius_;
			boxExtents = boxExtents_;
		}

		public void Draw(IncrementalModeling model)
		{
			switch (shape)
			{
			case Shape.sphere:
				DrawSphere(model);
				break;
			case Shape.box:
				DrawBox(model);
				break;
			}
		}

		private void DrawSphere(IncrementalModeling model)
		{
			int num = model._countX * model._countY * model._countZ;
			for (int i = 0; i < num; i++)
			{
				float magnitude = invTransform.MultiplyPoint(model._positionMap[i]).magnitude;
				if (magnitude < sphereRadius)
				{
					float num2 = 1f;
					if (fadeRadius > 0f)
					{
						num2 = Mathf.Clamp01((sphereRadius - magnitude) / fadeRadius);
					}
					model._powerMap[i] = Mathf.Clamp01(model._powerMap[i] + powerScale * num2);
					model._powerMap[i] *= model._powerMapMask[i];
				}
			}
		}

		private void DrawBox(IncrementalModeling model)
		{
			int num = model._countX * model._countY * model._countZ;
			for (int i = 0; i < num; i++)
			{
				float num2 = 1f;
				Vector3 vector = invTransform.MultiplyPoint(model._positionMap[i]);
				for (int j = 0; j < 3; j++)
				{
					float num3 = Mathf.Abs(vector[j]);
					float num4 = boxExtents[j];
					if (num3 < num4)
					{
						if (fadeRadius > 0f)
						{
							num2 *= Mathf.Clamp01((num4 - num3) / fadeRadius);
						}
						continue;
					}
					num2 = 0f;
					break;
				}
				if (num2 > 0f)
				{
					model._powerMap[i] = Mathf.Clamp01(model._powerMap[i] + powerScale * num2);
					model._powerMap[i] *= model._powerMapMask[i];
				}
			}
		}
	}

	public bool bSaveBrushHistory = true;

	[SerializeField]
	private List<Brush> _brushHistory = new List<Brush>();

	protected override void InitializePowerMap()
	{
		foreach (Brush item in _brushHistory)
		{
			item.Draw(this);
		}
	}

	[ContextMenu("Rebuild")]
	public void Rebuild()
	{
		ResetMaps();
		foreach (Brush item in _brushHistory)
		{
			item.Draw(this);
		}
		CreateMesh();
	}

	[ContextMenu("ClearHistory")]
	public void ClearHistory()
	{
		_brushHistory.Clear();
	}

	public void AddSphere(Transform brushTransform, float radius, float powerScale, float fadeRadius)
	{
		Matrix4x4 invTransformMtx_ = brushTransform.worldToLocalMatrix * base.transform.localToWorldMatrix;
		Brush brush = new Brush(Brush.Shape.sphere, invTransformMtx_, fadeRadius, powerScale, radius, Vector3.one);
		brush.Draw(this);
		if (bSaveBrushHistory)
		{
			_brushHistory.Add(brush);
		}
		CreateMesh();
	}

	public void AddBox(Transform brushTransform, Vector3 extents, float powerScale, float fadeRadius)
	{
		Matrix4x4 invTransformMtx_ = brushTransform.worldToLocalMatrix * base.transform.localToWorldMatrix;
		Brush brush = new Brush(Brush.Shape.box, invTransformMtx_, fadeRadius, powerScale, 1f, extents);
		brush.Draw(this);
		if (bSaveBrushHistory)
		{
			_brushHistory.Add(brush);
		}
		CreateMesh();
	}
}
