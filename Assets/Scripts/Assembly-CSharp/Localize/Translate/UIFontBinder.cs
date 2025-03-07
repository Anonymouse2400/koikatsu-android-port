using UnityEngine;

namespace Localize.Translate
{
	[DefaultExecutionOrder(-1)]
	public class UIFontBinder : BaseLoader, InitializeSolution.IInitializable
	{
		[SerializeField]
		private GameObject target;

		private bool initialized;

		bool InitializeSolution.IInitializable.initialized
		{
			get
			{
				return initialized;
			}
		}

		void InitializeSolution.IInitializable.Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				GameObject gameObject = target;
				if (gameObject == null)
				{
					gameObject = base.gameObject;
				}
				if (Manager.isTranslate)
				{
					Manager.BindFont(gameObject);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			Manager.initializeSolution.Add(this);
		}
	}
}
