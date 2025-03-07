using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Data_Generators : IWMG_Data_Generators
{
	public List<Vector2> GenLinear(int numPoints, float minX, float maxX, float a, float b)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX)
		{
			return list;
		}
		float num = (maxX - minX) / (float)(numPoints - 1);
		for (int i = 0; i < numPoints; i++)
		{
			float num2 = (float)i * num + minX;
			list.Add(new Vector2(num2, a * num2 + b));
		}
		return list;
	}

	public List<Vector2> GenQuadratic(int numPoints, float minX, float maxX, float a, float b, float c)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX)
		{
			return list;
		}
		float num = (maxX - minX) / (float)(numPoints - 1);
		for (int i = 0; i < numPoints; i++)
		{
			float num2 = (float)i * num + minX;
			list.Add(new Vector2(num2, a * num2 * num2 + b * num2 + c));
		}
		return list;
	}

	public List<Vector2> GenExponential(int numPoints, float minX, float maxX, float a, float b, float c)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX || b <= 0f)
		{
			return list;
		}
		float num = (maxX - minX) / (float)(numPoints - 1);
		for (int i = 0; i < numPoints; i++)
		{
			float num2 = (float)i * num + minX;
			list.Add(new Vector2(num2, a * Mathf.Pow(b, num2) + c));
		}
		return list;
	}

	public List<Vector2> GenLogarithmic(int numPoints, float minX, float maxX, float a, float b, float c)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX || b <= 0f || b == 1f)
		{
			return list;
		}
		float num = (maxX - minX) / (float)(numPoints - 1);
		for (int i = 0; i < numPoints; i++)
		{
			float num2 = (float)i * num + minX;
			if (!(num2 <= 0f))
			{
				list.Add(new Vector2(num2, a * Mathf.Log(num2, b) + c));
			}
		}
		return list;
	}

	public List<Vector2> GenCircular(int numPoints, float a, float b, float c)
	{
		return GenCircular2(numPoints, a, b, c, 0f);
	}

	public List<Vector2> GenCircular2(int numPoints, float a, float b, float c, float degreeOffset)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints < 2)
		{
			return list;
		}
		float num = 360f / (float)numPoints;
		for (int i = 0; i < numPoints; i++)
		{
			float num2 = (float)i * num + degreeOffset;
			float num3 = c * Mathf.Cos((float)Math.PI / 180f * num2);
			float num4 = c * Mathf.Sin((float)Math.PI / 180f * num2);
			list.Add(new Vector2(num3 + a, num4 + b));
		}
		return list;
	}

	public List<Vector2> GenRadar(List<float> data, float a, float b, float degreeOffset)
	{
		List<Vector2> list = new List<Vector2>();
		if (data.Count < 2)
		{
			return list;
		}
		float num = 360f / (float)data.Count;
		for (int i = 0; i < data.Count; i++)
		{
			float num2 = (float)i * num + degreeOffset;
			float num3 = data[i] * Mathf.Cos((float)Math.PI / 180f * num2);
			float num4 = data[i] * Mathf.Sin((float)Math.PI / 180f * num2);
			list.Add(new Vector2(num3 + a, num4 + b));
		}
		return list;
	}

	public List<Vector2> GenRandomXY(int numPoints, float minX, float maxX, float minY, float maxY)
	{
		List<Vector2> list = new List<Vector2>();
		if (maxY <= minY || maxX <= minX)
		{
			return list;
		}
		for (int i = 0; i < numPoints; i++)
		{
			list.Add(new Vector2(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY)));
		}
		return list;
	}

	public List<Vector2> GenRandomY(int numPoints, float minX, float maxX, float minY, float maxY)
	{
		List<Vector2> list = new List<Vector2>();
		if (maxY <= minY || maxX <= minX)
		{
			return list;
		}
		float num = (maxX - minX) / (float)(numPoints - 1);
		for (int i = 0; i < numPoints; i++)
		{
			list.Add(new Vector2((float)i * num + minX, UnityEngine.Random.Range(minY, maxY)));
		}
		return list;
	}

	public List<float> GenRandomList(int numPoints, float min, float max)
	{
		List<float> list = new List<float>();
		if (max <= min)
		{
			return list;
		}
		for (int i = 0; i < numPoints; i++)
		{
			list.Add(UnityEngine.Random.Range(min, max));
		}
		return list;
	}
}
