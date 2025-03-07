using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Illusion.Extensions
{
	public static class GameObjectExtensions
	{
		public static List<GameObject> Children(this GameObject self)
		{
			List<GameObject> list = new List<GameObject>();
			Transform transform = self.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				list.Add(transform.GetChild(i).gameObject);
			}
			return list;
		}

		public static void ChildrenAction(this GameObject self, Action<GameObject> act)
		{
			Transform transform = self.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				act(transform.GetChild(i).gameObject);
			}
		}

		public static GameObject[] CreateChild(this GameObject self, string pathName, bool worldPositionStays = true)
		{
			GameObject[] array = (from s in pathName.Split('/')
				select new GameObject(s)).ToArray();
			array.Select((GameObject go) => go.transform).Aggregate(delegate(Transform parent, Transform child)
			{
				child.SetParent(parent);
				return child;
			});
			array[0].transform.SetParent(self.transform, worldPositionStays);
			return array;
		}

		public static bool SetActiveIfDifferent(this GameObject self, bool active)
		{
			if (self.activeSelf == active)
			{
				return false;
			}
			self.SetActive(active);
			return true;
		}
	}
}
