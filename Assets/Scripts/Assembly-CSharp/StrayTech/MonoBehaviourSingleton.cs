using UnityEngine;

namespace StrayTech
{
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					T val = Object.FindObjectOfType<T>();
					if (val != null)
					{
						_instance = val;
					}
				}
				return _instance;
			}
		}

		protected virtual void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Object.Destroy(this);
			}
			else
			{
				_instance = GetComponent<T>();
			}
		}

		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = (T)null;
			}
		}

		protected virtual void OnApplicationQuit()
		{
			if (!(_instance != this))
			{
			}
		}
	}
}
