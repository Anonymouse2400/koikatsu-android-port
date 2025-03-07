using UnityEngine;

namespace Config
{
	public class TextSystem : BaseSystem
	{
		private static Color initWindowColor = new Color(1f, 1f, 1f, 0.5f);

		private static Color initFont0Color = new Color(0.8f, 1f, 1f, 1f);

		private static Color initFont1Color = new Color(1f, 0.8f, 1f, 1f);

		public int FontSpeed = 40;

		public bool ReadSkip = true;

		public bool NextVoiceStop = true;

		public float AutoWaitTime = 3f;

		public bool ChoicesSkip;

		public bool ChoicesAuto;

		public Color WindowColor = initWindowColor;

		public Color Font0Color = initFont0Color;

		public Color Font1Color = initFont1Color;

		public Color Font2Color = Color.white;

		public TextSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Init()
		{
			FontSpeed = 40;
			ReadSkip = true;
			NextVoiceStop = true;
			AutoWaitTime = 3f;
			ChoicesSkip = false;
			ChoicesAuto = false;
			WindowColor = initWindowColor;
			Font0Color = initFont0Color;
			Font1Color = initFont1Color;
			Font2Color = Color.white;
		}
	}
}
