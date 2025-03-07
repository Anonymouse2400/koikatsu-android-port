using System;
using System.Collections.Generic;
using UnityEngine;

namespace IllusionUtility.GetUtility
{
	public static class TransformFindEx
	{
		public static GameObject FindLoop(this Transform transform, string name)
		{
			if (string.Compare(name, transform.gameObject.name) == 0)
			{
				return transform.gameObject;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject gameObject = transform.GetChild(i).FindLoop(name);
				if (null != gameObject)
				{
					return gameObject;
				}
			}
			return null;
		}

		public static GameObject FindReverseLoop(this Transform transform, string name)
		{
			if (string.Compare(name, transform.gameObject.name) == 0)
			{
				return transform.gameObject;
			}
			if ((bool)transform.parent)
			{
				return transform.parent.FindReverseLoop(name);
			}
			return null;
		}

		public static void FindLoopPrefix(this Transform transform, List<GameObject> list, string name)
		{
			if (string.Compare(name, 0, transform.gameObject.name, 0, name.Length) == 0)
			{
				list.Add(transform.gameObject);
			}
			foreach (Transform item in transform)
			{
				item.FindLoopPrefix(list, name);
			}
		}

		public static void FindLoopTag(this Transform transform, List<GameObject> list, string tag)
		{
			if (transform.gameObject.CompareTag(tag))
			{
				list.Add(transform.gameObject);
			}
			foreach (Transform item in transform)
			{
				item.FindLoopTag(list, tag);
			}
		}

		public static void FindLoopAll(this Transform transform, List<GameObject> list)
		{
			list.Add(transform.gameObject);
			foreach (Transform item in transform)
			{
				item.FindLoopAll(list);
			}
		}

		public static GameObject FindTop(this Transform transform)
		{
			return (!(null == transform.parent)) ? transform.parent.FindTop() : transform.gameObject;
		}

		public static GameObject[] FindRootObject(this Transform transform)
		{
			return Array.FindAll(UnityEngine.Object.FindObjectsOfType<GameObject>(), (GameObject item) => item.transform.parent == null);
		}
	}
}
