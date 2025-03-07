using Illusion;
using Illusion.Extensions;
using Manager;

namespace ADV.Commands.Effect
{
	public class FadeWait : CommandBase
	{
		private enum Type
		{
			Scene = 0,
			Filter = 1
		}

		private Type type;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Type" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { Type.Scene.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			type = Utils.Enum<Type>.Cast(args[0].Check(true, Utils.Enum<Type>.Names));
		}

		public override bool Process()
		{
			base.Process();
			switch (type)
			{
			case Type.Scene:
				return Singleton<Scene>.Instance.sceneFade.IsEnd;
			case Type.Filter:
				return base.scenario.advScene.AdvFade.IsEnd;
			default:
				return true;
			}
		}

		public override void Result(bool processEnd)
		{
		}
	}
}
