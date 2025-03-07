using System;
using UnityEngine;

public class SerialCapture : MonoBehaviour
{
	public string Name = "1000/FrameRateして整数にならないとダメ";

	public int FrameRate = 25;

	public int SuperSize;

	public int EndCount = -1;

	public bool AutoRecord;

	public KeyCode ExitKey = KeyCode.Q;

	public Vector2 posCapBtn = new Vector2(0f, 0f);

	private string captureDirectory = string.Empty;

	private string serialId = string.Empty;

	private int frameCount = -1;

	private bool recording;

	private void Awake()
	{
		captureDirectory = UserData.Create("SerialCapture");
	}

	private void Start()
	{
		if (AutoRecord)
		{
			StartRecording();
		}
	}

	public bool GetRecording()
	{
		return recording;
	}

	private void StartRecording()
	{
		serialId = DateTime.Now.Minute.ToString("00");
		serialId += DateTime.Now.Second.ToString("00");
		serialId += "_";
		Time.captureFramerate = 1000 / FrameRate;
		frameCount = -1;
		recording = true;
	}

	private void EndRecording()
	{
		Time.captureFramerate = 0;
		recording = false;
	}

	private void Update()
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

	private void OnGUI()
	{
		if (recording)
		{
			if (Input.GetKeyDown(ExitKey))
			{
				EndRecording();
			}
		}
		else if (GUI.Button(new Rect(posCapBtn.x, posCapBtn.y, 200f, 30f), "Start SerialCapture"))
		{
			StartRecording();
		}
	}
}
