using UnityEngine;

public class SpRoot : MonoBehaviour
{
	public Camera renderCamera;

	public float baseScreenWidth = 1280f;

	public float baseScreenHeight = 720f;

	public float GetSpriteRate()
	{
		float num = baseScreenHeight / 2f * 0.01f;
		return 1f / (num * ((float)Screen.height / (baseScreenHeight * ((float)Screen.width / baseScreenWidth))));
	}

	public float GetSpriteCorrectY()
	{
		return ((float)Screen.height - (float)Screen.width / baseScreenWidth * baseScreenHeight) * (2f / (float)Screen.height) * 0.5f;
	}
}
