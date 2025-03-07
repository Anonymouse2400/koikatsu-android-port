using UnityEngine;

namespace ADV
{
	public abstract class SceneParameter
	{
		private static ADVScene _advScene;

		public static ADVScene advScene
		{
			get
			{
				return _advScene;
			}
			set
			{
				_advScene = value;
			}
		}

		public MonoBehaviour mono { get; private set; }

		public SceneParameter(MonoBehaviour mono)
		{
			this.mono = mono;
		}

		public static bool IsNowScene(MonoBehaviour mono)
		{
			return mono != null && mono == _advScene.nowScene;
		}

		public abstract void Init(Data advData);

		public abstract void Release();

		public abstract void WaitEndProc();
	}
}
