using Illusion.Extensions;
using UnityEngine;

namespace ADV.Backup
{
	internal class FadeData
	{
		private Color color;

		private float time;

		private Texture2D texture;

		public FadeData(SimpleFade fade)
		{
			color = fade._Color;
			time = fade._Time;
			texture = fade._Texture;
		}

		public void Load(SimpleFade fade)
		{
			fade._Color = fade._Color.Get(color, false);
			fade._Time = time;
			fade._Texture = texture;
		}
	}
}
