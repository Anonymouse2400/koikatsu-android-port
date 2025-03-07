using UnityEngine;

namespace ADV.Commands.Camera
{
	public class LerpNullMove : CommandBase
	{
		private float time;

		private float timer;

		private Transform nowT;

		private Vector3 startPos;

		private Vector3 endPos;

		private Vector3 startAngle;

		private Vector3 endAngle;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Time", "Start", "End" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					"0",
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			nowT = base.scenario.AdvCamera.transform;
			int num = 0;
			time = float.Parse(args[num++]);
			string key = args[num++];
			string text = args[num++];
			Transform value;
			base.scenario.commandController.NullDic.TryGetValue(key, out value);
			Transform value2 = null;
			if (!text.IsNullOrEmpty())
			{
				base.scenario.commandController.NullDic.TryGetValue(text, out value2);
			}
			if (value2 == null)
			{
				startPos = nowT.position;
				startAngle = nowT.eulerAngles;
				endPos = value.position;
				endAngle = value.eulerAngles;
			}
			else
			{
				startPos = value.position;
				startAngle = value.eulerAngles;
				endPos = value2.position;
				endAngle = value2.eulerAngles;
			}
		}

		public override bool Process()
		{
			timer = Mathf.Min(timer + Time.deltaTime, time);
			SetPosAng(NowLerp());
			return timer >= time;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (!processEnd)
			{
				timer = time;
				SetPosAng(NowLerp());
			}
		}

		private void SetPosAng(float t)
		{
			nowT.position = CommandBase.LerpV3(startPos, endPos, t);
			nowT.eulerAngles = CommandBase.LerpAngleV3(startAngle, endAngle, t);
		}

		private float NowLerp()
		{
			return Mathf.InverseLerp(0f, time, timer);
		}
	}
}
