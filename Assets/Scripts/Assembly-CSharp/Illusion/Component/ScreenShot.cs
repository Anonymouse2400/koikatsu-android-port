using System.Collections.Generic;
using Illusion.CustomAttributes;
using Illusion.Game;
using UnityEngine;

namespace Illusion.Component
{
	public class ScreenShot : MonoBehaviour
	{
		[Button("Capture", "キャプチャー", new object[] { "" })]
		public int excuteCapture;

		public int capRate = 1;

		public Texture texCapMark;

		public List<ScreenShotCamera> ssCamList = new List<ScreenShotCamera>();

		public void Capture(string path = null)
		{
			if (path.IsNullOrEmpty())
			{
				path = Illusion.Game.Utils.ScreenShot.Path;
			}
			StartCoroutine(Illusion.Game.Utils.ScreenShot.CaptureGSS(ssCamList, path, texCapMark, capRate));
		}
	}
}
