using UnityEngine;

namespace IllusionUtility.SetUtility
{
	public static class TransformCopy
	{
		public static void CopyPosRotScl(this Transform dst, Transform src)
		{
			dst.localScale = src.localScale;
			dst.position = src.position;
			dst.rotation = src.rotation;
		}
	}
}
