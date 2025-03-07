using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Component.UI
{
	public class UIIgnoreHit : Button, ICanvasRaycastFilter
	{
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}
}
