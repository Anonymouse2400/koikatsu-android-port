using System;
using System.Text;
using UnityEngine;

public static class ScreenShot
{
	public static void Capture(string path = "", int superSize = 0)
	{
		if (string.Empty == path)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			string value = UserData.Create("cap");
			stringBuilder.Append(value);
			stringBuilder.Append(DateTime.Now.Year.ToString("0000"));
			stringBuilder.Append(DateTime.Now.Month.ToString("00"));
			stringBuilder.Append(DateTime.Now.Day.ToString("00"));
			stringBuilder.Append(DateTime.Now.Hour.ToString("00"));
			stringBuilder.Append(DateTime.Now.Minute.ToString("00"));
			stringBuilder.Append(DateTime.Now.Second.ToString("00"));
			stringBuilder.Append(DateTime.Now.Millisecond.ToString("000"));
			stringBuilder.Append(".png");
			path = stringBuilder.ToString();
		}
        Application.CaptureScreenshot(path, superSize);
	}
}
