using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StrayTech
{
	public static class GameObjectExtension
	{
		public static void CopyLocalTransform(this GameObject self, GameObject other)
		{
			if (!(other == null))
			{
				self.transform.CopyLocalTransform(other.transform);
			}
		}

		public static void CopyLocalTransform(this Transform self, Transform other)
		{
			if (!(other == null))
			{
				self.localPosition = other.localPosition;
				self.localRotation = other.localRotation;
				self.localScale = other.localScale;
			}
		}

		public static void ResetLocalTransform(this Transform self)
		{
			self.localPosition = Vector3.zero;
			self.localRotation = Quaternion.identity;
			self.localScale = Vector3.one;
		}

		public static void NiceifyGameobjectName(this GameObject current, string prefix = "", string suffix = "", List<string> substringsToRemove = null)
		{
			if (string.IsNullOrEmpty(current.name))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(current.name);
			if (substringsToRemove != null)
			{
				foreach (string item in substringsToRemove)
				{
					if (!string.IsNullOrEmpty(item))
					{
						stringBuilder.Replace(item, string.Empty);
					}
				}
			}
			stringBuilder.Insert(0, prefix);
			stringBuilder.Append(suffix);
			current.name = stringBuilder.ToString();
		}

		public static T GetInterface<T>(this GameObject toSearch) where T : class
		{
			if (!typeof(T).IsInterface)
			{
				return (T)null;
			}
			MonoBehaviour[] components = toSearch.GetComponents<MonoBehaviour>();
			MonoBehaviour[] array = components;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if (monoBehaviour is T)
				{
					return monoBehaviour as T;
				}
			}
			return (T)null;
		}

		public static T[] GetInterfaces<T>(this GameObject toSearch) where T : class
		{
			if (!typeof(T).IsInterface)
			{
				return null;
			}
			List<T> list = new List<T>();
			MonoBehaviour[] components = toSearch.GetComponents<MonoBehaviour>();
			MonoBehaviour[] array = components;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if (monoBehaviour is T)
				{
					list.Add(monoBehaviour as T);
				}
			}
			return list.ToArray();
		}

		public static T[] GetInterfacesInChildren<T>(this GameObject toSearch, bool includeInactive, bool searchSelf = false) where T : class
		{
			List<T> list = new List<T>();
			if (toSearch == null)
			{
				return list.ToArray();
			}
			if (!typeof(T).IsInterface)
			{
				return list.ToArray();
			}
			MonoBehaviour[] componentsInChildren = toSearch.GetComponentsInChildren<MonoBehaviour>(includeInactive);
			MonoBehaviour[] array = componentsInChildren;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if ((searchSelf || !(monoBehaviour == toSearch)) && monoBehaviour is T)
				{
					list.Add(monoBehaviour as T);
				}
			}
			return list.ToArray();
		}

		public static T GetInterfaceInChildren<T>(this GameObject toSearch, bool includeInactive) where T : class
		{
			if (!typeof(T).IsInterface)
			{
				return (T)null;
			}
			MonoBehaviour[] componentsInChildren = toSearch.GetComponentsInChildren<MonoBehaviour>(includeInactive);
			MonoBehaviour[] array = componentsInChildren;
			foreach (MonoBehaviour monoBehaviour in array)
			{
				if (monoBehaviour is T)
				{
					return monoBehaviour as T;
				}
			}
			return (T)null;
		}

		public static TMonoBehaviour AddOrGetComponent<TMonoBehaviour>(this GameObject source) where TMonoBehaviour : Component
		{
			TMonoBehaviour val = source.GetComponent<TMonoBehaviour>();
			if (val == null)
			{
				val = source.AddComponent<TMonoBehaviour>();
			}
			return val;
		}

		public static T GetComponentUpwards<T>(this GameObject toSearch, bool searchSelfFirst = false) where T : Component
		{
			if (!searchSelfFirst && toSearch.transform.parent == null)
			{
				return (T)null;
			}
			Transform transform = ((!searchSelfFirst) ? toSearch.transform.parent : toSearch.transform);
			T val = (T)null;
			while (transform != null)
			{
				val = transform.GetComponent<T>();
				if (val != null)
				{
					return val;
				}
				transform = transform.parent;
			}
			return val;
		}

		public static GameObject GetFirstChild(this GameObject toSearch)
		{
			Transform[] componentsInChildren = toSearch.GetComponentsInChildren<Transform>(true);
			if (componentsInChildren.Length < 2)
			{
				return null;
			}
			return componentsInChildren[1].gameObject;
		}

		public static GameObject CreateChild(this GameObject parent, string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parent.transform;
			gameObject.transform.ResetLocalTransform();
			return gameObject;
		}

		public static GameObject FindChildDeep(this Transform parent, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (transform.name == name)
				{
					return transform.gameObject;
				}
			}
			return null;
		}

		public static T GetComponentInChildren<T>(this GameObject component, bool includeInactive) where T : Component
		{
			T[] componentsInChildren = component.GetComponentsInChildren<T>(includeInactive);
			if (componentsInChildren.Length == 0)
			{
				return (T)null;
			}
			return componentsInChildren[0];
		}

		public static T GetComponentInChildren<T>(this Component component, bool includeInactive) where T : Component
		{
			return component.gameObject.GetComponentInChildren<T>(includeInactive);
		}

		public static GameObject FindOrCreate(string name)
		{
			GameObject gameObject = GameObject.Find("/" + name);
			if (gameObject == null)
			{
				gameObject = new GameObject(name);
			}
			return gameObject;
		}

		public static void LookAtXZ(this Transform self, Transform target)
		{
			if (!(self == null) && !(target == null))
			{
				self.LookAtXZ(target.position);
			}
		}

		public static void LookAtXZ(this Transform self, Vector3 worldTarget)
		{
			Vector3 vector = new Vector3(worldTarget.x, 0f, worldTarget.z);
			Vector3 vector2 = new Vector3(self.position.x, 0f, self.position.z);
			Quaternion rotation = Quaternion.LookRotation(vector - vector2);
			self.transform.rotation = rotation;
		}

		public static Transform AddOrGetChild(this Transform parent, string childName)
		{
			if (string.IsNullOrEmpty(childName))
			{
				return null;
			}
			Transform transform = null;
			transform = parent.transform.Find(childName);
			if (transform == null)
			{
				transform = new GameObject(childName).transform;
				transform.parent = parent.transform;
				transform.ResetLocalTransform();
			}
			return transform;
		}

		public static GameObject FindChild(this GameObject self, string childName)
		{
			if (string.IsNullOrEmpty(childName))
			{
				return null;
			}
			Transform transform = self.transform.Find(childName);
			if (transform == null)
			{
				return null;
			}
			return transform.gameObject;
		}

		public static IEnumerable<T> FindAllInterfaces<T>() where T : class
		{
			if (!typeof(T).IsInterface)
			{
				yield return (T)null;
			}
			MonoBehaviour[] allMonos = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
			MonoBehaviour[] array = allMonos;
			foreach (MonoBehaviour mono in array)
			{
				if (mono is T)
				{
					yield return mono as T;
				}
			}
		}

		public static bool IsNullOrInvalid(this IValidates self)
		{
			return self == null || !self.IsValid();
		}

		public static IEnumerable<TInterface> ScrapeInterfaces<TInterface>(this IEnumerable<GameObject> self) where TInterface : class
		{
			if (!typeof(TInterface).IsInterface)
			{
				yield return (TInterface)null;
			}
			if (self == null)
			{
				yield return (TInterface)null;
			}
			foreach (GameObject item in self)
			{
				if (!(item == null))
				{
					TInterface foundInterface = item.GetInterface<TInterface>();
					if (foundInterface != null)
					{
						yield return foundInterface;
					}
				}
			}
		}

		public static string FullPath(this GameObject self)
		{
			if (self == null)
			{
				return string.Empty;
			}
			Stack<string> stack = new Stack<string>();
			Transform transform = self.transform;
			while (transform != null)
			{
				stack.Push(transform.gameObject.name);
				transform = transform.parent;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (stack.Count > 0)
			{
				stringBuilder.Append(stack.Pop());
				stringBuilder.Append('.');
			}
			return stringBuilder.ToString();
		}

		public static string FullPath(this Component self)
		{
			if (self == null)
			{
				return string.Empty;
			}
			return string.Format("{0}{1}", self.gameObject.FullPath(), self.GetType().Name);
		}

		public static List<GameObject> GetAllChildren(this GameObject self)
		{
			List<GameObject> list = new List<GameObject>();
			if (self == null)
			{
				return list;
			}
			int childCount = self.transform.childCount;
			for (int i = 0; i <= childCount - 1; i++)
			{
				list.Add(self.transform.GetChild(i).gameObject);
			}
			return list;
		}

		public static void GetAllChildren(this GameObject self, ref List<GameObject> toPopulate)
		{
			if (!(self == null))
			{
				int childCount = self.transform.childCount;
				if (toPopulate == null)
				{
					toPopulate = new List<GameObject>(childCount);
				}
				else
				{
					toPopulate.Clear();
				}
				for (int i = 0; i <= childCount - 1; i++)
				{
					toPopulate.Add(self.transform.GetChild(i).gameObject);
				}
			}
		}

		public static IEnumerable<T> GetComponentsInChildren<T>(this Component self, Predicate<T> filter, bool includeInactive = false) where T : Component
		{
			if (self == null || filter == null)
			{
				yield break;
			}
			T[] componentsInChildren = self.GetComponentsInChildren<T>(includeInactive);
			foreach (T foundChild in componentsInChildren)
			{
				if (filter(foundChild))
				{
					yield return foundChild;
				}
			}
		}

		public static T GetComponentInChildren<T>(this Component self, Predicate<T> filter) where T : Component
		{
			if (self == null)
			{
				return (T)null;
			}
			if (filter == null)
			{
				return (T)null;
			}
			T[] componentsInChildren = self.GetComponentsInChildren<T>();
			foreach (T val in componentsInChildren)
			{
				if (filter(val))
				{
					return val;
				}
			}
			return (T)null;
		}

		public static T GetComponentFromLoadedResource<T>(string resourcePath) where T : Component
		{
			if (string.IsNullOrEmpty(resourcePath))
			{
				return (T)null;
			}
			GameObject gameObject = Resources.Load<GameObject>(resourcePath);
			if (gameObject == null)
			{
				return (T)null;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			if (gameObject2 == null)
			{
				UnityEngine.Object.Destroy(gameObject2);
				return (T)null;
			}
			T component = gameObject2.GetComponent<T>();
			if (component == null)
			{
				UnityEngine.Object.Destroy(gameObject2);
				return (T)null;
			}
			return component;
		}
	}
}
