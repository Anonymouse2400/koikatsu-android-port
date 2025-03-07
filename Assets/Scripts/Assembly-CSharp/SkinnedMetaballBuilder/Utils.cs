using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkinnedMetaballBuilder
{
	public static class Utils
	{
		public static void DestroyChildren(Transform parent)
		{
			int childCount = parent.childCount;
			GameObject[] array = new GameObject[childCount];
			for (int i = 0; i < childCount; i++)
			{
				array[i] = parent.GetChild(i).gameObject;
			}
			parent.DetachChildren();
			for (int j = 0; j < childCount; j++)
			{
				UnityEngine.Object.Destroy(array[j]);
			}
		}

		public static T StringToEnumType<T>(string value, T defaultValue)
		{
			try
			{
				if (string.IsNullOrEmpty(value))
				{
					return defaultValue;
				}
				return (T)Enum.Parse(typeof(T), value);
			}
			catch (ArgumentException ex)
			{
				throw new UnityException(ex.Message + Environment.NewLine + "failed to parse string [" + value + "] -> enum type [" + typeof(T).ToString() + "]");
			}
		}

		public static List<T> GetComponentsRecursive<T>(Transform t) where T : Component
		{
			List<T> list = new List<T>();
			T component = t.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			int i = 0;
			for (int childCount = t.childCount; i < childCount; i++)
			{
				list.AddRange(GetComponentsRecursive<T>(t.GetChild(i)));
			}
			return list;
		}

		public static T FindComponentInParents<T>(Transform t) where T : Component
		{
			T component = t.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			if (t.parent != null)
			{
				return FindComponentInParents<T>(t.parent);
			}
			return (T)null;
		}

		public static void ConvertMeshIntoWireFrame(Mesh mesh)
		{
			if (mesh.GetTopology(0) == MeshTopology.Triangles)
			{
				int[] indices = mesh.GetIndices(0);
				int[] array = new int[indices.Length * 2];
				for (int i = 0; i < indices.Length / 3; i++)
				{
					int num = indices[i * 3];
					int num2 = indices[i * 3 + 1];
					int num3 = indices[i * 3 + 2];
					array[i * 6] = num;
					array[i * 6 + 1] = num2;
					array[i * 6 + 2] = num2;
					array[i * 6 + 3] = num3;
					array[i * 6 + 4] = num3;
					array[i * 6 + 5] = num;
				}
				mesh.SetIndices(array, MeshTopology.Lines, 0);
			}
		}
	}
}
