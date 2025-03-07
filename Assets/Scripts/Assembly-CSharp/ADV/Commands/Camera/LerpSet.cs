using UnityEngine;

namespace ADV.Commands.Camera
{
	public class LerpSet : Base
	{
		protected Vector3 initPos;

		protected Vector3 initAng;

		protected Vector3 endPos;

		protected Vector3 endAng;

		protected float endDir;

		protected float initDir;

		protected float startTime;

		protected bool isSuccess;

		protected float time;

		protected float timer;

		protected Vector3 calcPos;

		protected Vector3 calcAng;

		protected float calcDir;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Time", "X,Y,Z", "Pitch,Yaw,Roll", "Dir" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		protected void Init(TextScenario scenario)
		{
			timer = 0f;
			time = 0f;
			calcPos = new Vector3(float.NaN, float.NaN, float.NaN);
			calcAng = new Vector3(float.NaN, float.NaN, float.NaN);
			calcDir = float.NaN;
			BaseCameraControl baseCamCtrl = scenario.BaseCamCtrl;
			if (!(baseCamCtrl == null))
			{
				baseCamCtrl.enabled = true;
				endPos = (initPos = baseCamCtrl.transform.position);
				endAng = (initAng = baseCamCtrl.CameraAngle);
				endDir = (initDir = baseCamCtrl.CameraDir.z);
			}
		}

		protected virtual void Analytics(string[] args, TextScenario scenario)
		{
			int num = 0;
			float.TryParse(args[num++], out time);
			string text = args[num++];
			string text2 = args[num++];
			string s = args[num++];
			float result = 0f;
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length && i < 3; i++)
			{
				if (float.TryParse(array[i], out result))
				{
					calcPos[i] = result;
				}
			}
			string[] array2 = text2.Split(',');
			for (int j = 0; j < array2.Length && j < 3; j++)
			{
				if (float.TryParse(array2[j], out result))
				{
					calcAng[j] = result;
				}
			}
			if (float.TryParse(s, out result))
			{
				calcDir = result;
			}
		}

		public virtual void Calc()
		{
			for (int i = 0; i < 3; i++)
			{
				if (!float.IsNaN(calcPos[i]))
				{
					endPos[i] = calcPos[i];
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (!float.IsNaN(calcAng[j]))
				{
					endAng[j] = calcAng[j];
				}
			}
			if (!float.IsNaN(calcDir))
			{
				endDir = calcDir;
			}
		}

		public override void Do()
		{
			base.Do();
			Init(base.scenario);
			Analytics(args, base.scenario);
			Calc();
		}

		public override bool Process()
		{
			base.Process();
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl == null)
			{
				return true;
			}
			timer = Mathf.Min(timer + Time.deltaTime, time);
			Vector3 position = baseCamCtrl.transform.position;
			Vector3 cameraAngle = baseCamCtrl.CameraAngle;
			float t = Mathf.InverseLerp(0f, time, timer);
			for (int i = 0; i < 3; i++)
			{
				position[i] = Mathf.Lerp(initPos[i], endPos[i], t);
				cameraAngle[i] = Mathf.Lerp(initAng[i], endAng[i], t);
			}
			float z = Mathf.Lerp(initDir, endDir, t);
			baseCamCtrl.SetCamera(position, cameraAngle, Quaternion.Euler(cameraAngle), new Vector3(0f, 0f, z));
			return timer >= time;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (!(baseCamCtrl == null))
			{
				baseCamCtrl.SetCamera(endPos, endAng, Quaternion.Euler(endAng), new Vector3(0f, 0f, endDir));
			}
		}
	}
}
