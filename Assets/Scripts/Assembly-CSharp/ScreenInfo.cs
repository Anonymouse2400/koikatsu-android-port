using UnityEngine;

public static class ScreenInfo
{
	public static float GetBaseScreenWidth()
	{
		return 1280f;
	}

	public static float GetBaseScreenHeight()
	{
		return 720f;
	}

	public static Vector2 GetScreenSize()
	{
		return new Vector2(Screen.width, Screen.height);
	}

	public static float GetSpriteRate()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		return 1f / (3.6f * (num2 / (720f * (num / 1280f))));
	}

	public static float GetSpriteCorrectY()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		return (num2 - num / 1280f * 720f) * (2f / num2) * 0.5f;
	}

	public static float GetScreenRate()
	{
		float num = Screen.width;
		return num / 1280f;
	}

	public static float GetScreenCorrectY()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		return (num2 - num / 1280f * 720f) * 0.5f;
	}
}
