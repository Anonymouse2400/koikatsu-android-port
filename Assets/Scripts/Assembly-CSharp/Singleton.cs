using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (!instance)
			{
				instance = Object.FindObjectOfType<T>();
			}
			return instance;
		}
	}

	public static bool IsInstance()
	{
		return instance != null;
	}

	protected virtual void Awake()
	{
		CheckInstance();
	}

	protected bool CheckInstance()
	{
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(this);
		return false;
	}
}
