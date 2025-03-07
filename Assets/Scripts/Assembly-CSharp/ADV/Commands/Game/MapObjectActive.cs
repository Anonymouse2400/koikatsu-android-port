using ActionGame.MapObject;

namespace ADV.Commands.Game
{
	public class MapObjectActive : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Name", "isActive" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			bool active = bool.Parse(args[num++]);
			if (text.IsNullOrEmpty())
			{
				base.scenario.advScene.Map.mapObjectGroup.gameObject.SetActive(active);
				return;
			}
			Kind kind = null;
			Kind[] mapObjects = base.scenario.advScene.Map.mapObjects;
			foreach (Kind kind2 in mapObjects)
			{
				if (kind2.name == text)
				{
					kind = kind2;
					break;
				}
			}
			kind.gameObject.SetActive(active);
		}
	}
}
