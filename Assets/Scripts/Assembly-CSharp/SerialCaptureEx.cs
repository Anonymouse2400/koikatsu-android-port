using System;
using UnityEngine;

public class SerialCaptureEx
{
	public string Name = "1000/FrameRateして整数にならないとダメ";

	public int FrameRate = 25;

	public int SuperSize;

	public int EndCount = -1;

	private string captureDirectory = string.Empty;

	private string serialId = string.Empty;

	private int frameCount = -1;

	private bool recording;

	public SerialCaptureEx()
	{
		captureDirectory = UserData.Create("SerialCapture");
	}

	public void StartRecording()
	{
		serialId = DateTime.Now.Minute.ToString("00");
		serialId += DateTime.Now.Second.ToString("00");
		serialId += "_";
		Time.captureFramerate = 1000 / FrameRate;
		frameCount = -1;
		recording = true;
	}

	public void EndRecording()
	{
		Time.captureFramerate = 0;
		recording = false;
	}

	public void Update()
	{
		if (recording)
		{
			if (0 < frameCount)
			{
				string filename = captureDirectory + "/" + serialId + frameCount.ToString("0000") + ".png";
                Application.CaptureScreenshot(filename, SuperSize);
			}
			frameCount++;
			if (0 < frameCount && frameCount % FrameRate == 0)
			{
				int num = frameCount / FrameRate;
				int num2 = num / 60;
				int num3 = num % 60;
			}
			if (EndCount != -1 && frameCount > EndCount)
			{
				EndRecording();
			}
		}
	}
}
