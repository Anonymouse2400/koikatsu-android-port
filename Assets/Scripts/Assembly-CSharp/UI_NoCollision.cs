using UnityEngine;

public class UI_NoCollision : MonoBehaviour, ICanvasRaycastFilter
{
	public bool IsRaycastLocationValid(Vector2 sp, Camera cam)
	{
		return false;
	}
}
