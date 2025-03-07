using System.IO;
using Config;
using Manager;
using UnityEngine;

public class CameraControl : BaseCameraControl
{
	public SmartTouch smartTouch;

	public PinchInOut pinchInOut;

	private Transform targetTex;

	public bool disableShortcut;

	public bool isOutsideTargetTex { get; set; }

	public void SetCenterSC()
	{
		EtceteraSystem etcData = Manager.Config.EtcData;
		etcData.Look = !etcData.Look;
		Singleton<Manager.Config>.Instance.Save();
	}

	public void ChangeDepthOfFieldSetting()
	{
		if (!(null == Singleton<Manager.Config>.Instance))
		{
		}
	}

	public void UpdateDepthOfFieldSetting()
	{
		if (!disableShortcut)
		{
		}
	}

	protected override void Start()
	{
		base.Start();
		targetTex = base.transform.Find("CameraTarget");
		if ((bool)targetTex)
		{
			targetTex.localScale = Vector3.one * 0.01f;
		}
		isOutsideTargetTex = true;
	}

	protected new void LateUpdate()
	{
		UpdateDepthOfFieldSetting();
		if (!Singleton<Scene>.Instance.sceneFade.IsFadeNow && !Singleton<Scene>.Instance.IsOverlap)
		{
			base.LateUpdate();
			EtceteraSystem etcData = Manager.Config.EtcData;
			if ((bool)targetTex)
			{
				targetTex.position = CamDat.Pos;
				Vector3 position = base.transform.position;
				position.y = CamDat.Pos.y;
				targetTex.transform.LookAt(position);
				targetTex.Rotate(new Vector3(90f, 0f, 0f));
				targetTex.GetComponent<Renderer>().enabled = base.isControlNow & etcData.Look & isOutsideTargetTex;
			}
			if (!disableShortcut && Input.GetKeyDown(KeyCode.Alpha6))
			{
				SetCenterSC();
			}
		}
	}

	protected new bool InputTouchProc()
	{
		if (base.InputTouchProc())
		{
			float deltaTime = Time.deltaTime;
			if ((bool)pinchInOut)
			{
				float rate = pinchInOut.Rate;
				if (pinchInOut.NowState == PinchInOut.State.ScalUp)
				{
					CamDat.Dir.z += rate * deltaTime * zoomSpeed;
				}
				else if (pinchInOut.NowState == PinchInOut.State.ScalDown)
				{
					CamDat.Dir.z -= rate * deltaTime * zoomSpeed;
				}
			}
			return true;
		}
		return false;
	}

	private void Save(BinaryWriter Writer)
	{
		Writer.Write(CamDat.Pos.x);
		Writer.Write(CamDat.Pos.y);
		Writer.Write(CamDat.Pos.z);
		Writer.Write(CamDat.Dir.x);
		Writer.Write(CamDat.Dir.y);
		Writer.Write(CamDat.Dir.z);
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		Writer.Write(eulerAngles.x);
		Writer.Write(eulerAngles.y);
		Writer.Write(eulerAngles.z);
	}

	private void Load(BinaryReader Reader)
	{
		CamDat.Pos.x = Reader.ReadSingle();
		CamDat.Pos.y = Reader.ReadSingle();
		CamDat.Pos.z = Reader.ReadSingle();
		CamDat.Dir.x = Reader.ReadSingle();
		CamDat.Dir.y = Reader.ReadSingle();
		CamDat.Dir.z = Reader.ReadSingle();
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		eulerAngles.x = Reader.ReadSingle();
		eulerAngles.y = Reader.ReadSingle();
		eulerAngles.z = Reader.ReadSingle();
		base.transform.rotation = Quaternion.Euler(eulerAngles);
	}
}
