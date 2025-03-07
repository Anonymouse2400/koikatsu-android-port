namespace Config
{
	public class DebugSystem : BaseSystem
	{
		public bool FPS;

		public DebugSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Init()
		{
			FPS = false;
		}
	}
}
