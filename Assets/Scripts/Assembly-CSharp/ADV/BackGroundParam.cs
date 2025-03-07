using System.Linq;
using Illusion.Component.UI;
using Manager;
using UnityEngine;

namespace ADV
{
	public class BackGroundParam : Illusion.Component.UI.BackGroundParam
	{
		public bool visibleAll
		{
			get
			{
				return base.visible;
			}
			set
			{
				base.visible = value;
				visibleFog = !value;
				visibleMap = !value;
			}
		}

		public bool visibleFog
		{
			set
			{
				Singleton<Game>.Instance.cameraEffector.config.SetFog(value);
			}
		}

		public bool visibleMap
		{
			set
			{
				Scene.Data baseScene = Singleton<Scene>.Instance.baseScene;
				if (baseScene != null)
				{
					GameObject gameObject = Scene.GetRootGameObjects(baseScene.levelName).FirstOrDefault((GameObject p) => p.name == "Map");
					if (gameObject != null)
					{
						gameObject.SetActive(value);
					}
				}
			}
		}
	}
}
