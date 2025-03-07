using UnityEngine;

namespace IllusionUtility.GetUtility
{
	public static class GameObjectEx
	{
		public static T Get<T>(this GameObject obj) where T : MonoBehaviour
		{
			T component = obj.GetComponent<T>();
			if (!component)
			{
			}
			return component;
		}

		public static T SearchComponent<T>(this GameObject obj, string searchName) where T : MonoBehaviour
		{
			T[] componentsInChildren = obj.GetComponentsInChildren<T>(true);
			T[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				T result = array[i];
				if (searchName == result.name)
				{
					return result;
				}
			}
			return (T)null;
		}
	}
}
