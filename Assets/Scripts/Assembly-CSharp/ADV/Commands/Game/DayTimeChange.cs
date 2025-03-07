using Illusion.Extensions;

namespace ADV.Commands.Game
{
	public class DayTimeChange : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "TimeZoon" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "昼" };
			}
		}

		public override void Do()
		{
			base.Do();
			string value = args[0];
			int type = new string[3] { "昼", "夕方", "夜" }.Check(value);
			SunLightInfo sunLightInfo = base.scenario.advScene.Map.sunLightInfo;
			sunLightInfo.Set((SunLightInfo.Info.Type)type, base.scenario.AdvCamera);
			base.scenario.advScene.Map.UpdateCameraFog();
		}
	}
}
