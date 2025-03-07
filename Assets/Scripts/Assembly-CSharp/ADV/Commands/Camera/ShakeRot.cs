using DG.Tweening;
using UnityEngine;

namespace ADV.Commands.Camera
{
	public class ShakeRot : Base
	{
		private float duration = 0.1f;

		private float strength = 1f;

		private int vibrato = 10;

		private float randomness = 90f;

		private bool fadeOut = true;

		private Vector3 strengthVec = Vector3.zero;

		private Tween tween;

		private bool isVec;

		private bool isOnComplete;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "duration", "strength", "vibrato", "randomness", "fadeOut" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					"1",
					"1",
					"10",
					"90",
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			isVec = false;
			isOnComplete = false;
			int num = 0;
			duration = float.Parse(args[num++]);
			string[] argToSplit = GetArgToSplit(num++);
			if (argToSplit.Length != 0)
			{
				if (argToSplit.Length == 1)
				{
					strength = float.Parse(argToSplit[0]);
				}
				else
				{
					isVec = true;
					for (int i = 0; i < argToSplit.Length && i < 3; i++)
					{
						float result;
						if (float.TryParse(argToSplit[i], out result))
						{
							strengthVec[i] = result;
						}
					}
				}
			}
			args.SafeProc(num++, delegate(string s)
			{
				vibrato = int.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				randomness = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				fadeOut = bool.Parse(s);
			});
			Transform transform = base.scenario.AdvCamera.transform;
			if (!isVec)
			{
				tween = transform.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut);
			}
			else
			{
				tween = transform.DOShakeRotation(duration, strengthVec, vibrato, randomness, fadeOut);
			}
			tween.OnComplete(delegate
			{
				isOnComplete = true;
			});
		}

		public override bool Process()
		{
			base.Process();
			return isOnComplete;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (!processEnd)
			{
				tween.Complete();
				tween = null;
			}
		}
	}
}
