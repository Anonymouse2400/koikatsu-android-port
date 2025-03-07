using System;
using System.Collections.Generic;
using UnityEngine;

namespace Illusion.Extensions
{
	public static class TransformExtensions
	{
		public static List<Transform> Children(this Transform self)
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < self.childCount; i++)
			{
				list.Add(self.GetChild(i));
			}
			return list;
		}

		public static void ChildrenAction(this Transform self, Action<Transform> act)
		{
			for (int i = 0; i < self.childCount; i++)
			{
				act(self.GetChild(i));
			}
		}
	}
}
