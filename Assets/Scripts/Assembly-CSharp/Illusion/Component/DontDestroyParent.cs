using UnityEngine;
using UnityEngine.SceneManagement;

namespace Illusion.Component
{
	public class DontDestroyParent : MonoBehaviour
	{
		private static DontDestroyParent instance;

		public static DontDestroyParent Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Object.FindObjectOfType<DontDestroyParent>();
					if (instance == null)
					{
						GameObject gameObject = new GameObject("DontDestroyParent");
						instance = gameObject.AddComponent<DontDestroyParent>();
					}
					instance.gameObject.hideFlags = HideFlags.NotEditable;
				}
				return instance;
			}
		}

		public static void Register(GameObject obj)
		{
			obj.transform.parent = Instance.transform;
		}

		public static void Register(MonoBehaviour component)
		{
			Register(component.gameObject);
		}

		private void Awake()
		{
			SceneManager.sceneLoaded += delegate
			{
				Check();
			};
			CheckInstance();
		}

		private void CheckInstance()
		{
			if (Instance == this)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void Check()
		{
			Object.Destroy(base.gameObject);
			instance = null;
		}
	}
}
