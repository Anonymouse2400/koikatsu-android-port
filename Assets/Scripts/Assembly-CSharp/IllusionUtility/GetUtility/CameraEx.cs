using System;
using UnityEngine;

namespace IllusionUtility.GetUtility
{
	public static class CameraEx
	{
		public static float GetDistanceWithWidthSize(this Camera camera, float width)
		{
			if (camera == null || width <= 0f)
			{
				return 0f;
			}
			float num = width / camera.aspect;
			return num * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
		}
	}
}
