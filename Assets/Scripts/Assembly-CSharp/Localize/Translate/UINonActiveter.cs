using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	[DefaultExecutionOrder(-1)]
	public class UINonActiveter : BaseLoader, InitializeSolution.IInitializable
	{
		private enum Type
		{
			GameObject = 0,
			Image = 1
		}

		[SerializeField]
		private Type type;

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
			if (initialized)
			{
				return;
			}
			initialized = true;
			if (Manager.isTranslate)
			{
				GameObject gameObject = target;
				if (gameObject == null)
				{
					gameObject = base.gameObject;
				}
				switch (type)
				{
				case Type.Image:
					gameObject.GetComponent<Image>().enabled = false;
					break;
				default:
					gameObject.SetActive(false);
					break;
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
