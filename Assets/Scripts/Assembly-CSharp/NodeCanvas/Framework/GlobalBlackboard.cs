using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeCanvas.Framework
{
	[ExecuteInEditMode]
	public class GlobalBlackboard : Blackboard
	{
		public static List<GlobalBlackboard> allGlobals = new List<GlobalBlackboard>();

		public bool dontDestroy = true;

		public new string name
		{
			get
			{
				return base.name;
			}
			set
			{
				if (base.name != value)
				{
					base.name = value;
					if (IsUnique())
					{
					}
				}
			}
		}

		public static GlobalBlackboard Create()
		{
			GlobalBlackboard globalBlackboard = new GameObject("@GlobalBlackboard").AddComponent<GlobalBlackboard>();
			globalBlackboard.name = "Global";
			return globalBlackboard;
		}

		public static GlobalBlackboard Find(string name)
		{
			if (!Application.isPlaying)
			{
				return (from b in Object.FindObjectsOfType<GlobalBlackboard>()
					where b.name == name
					select b).FirstOrDefault();
			}
			return allGlobals.Find((GlobalBlackboard b) => b.name == name);
		}

		private void OnEnable()
		{
			if (!allGlobals.Contains(this))
			{
				allGlobals.Add(this);
			}
			if (Application.isPlaying)
			{
				if (IsUnique())
				{
					if (dontDestroy)
					{
						Object.DontDestroyOnLoad(base.gameObject);
					}
				}
				else
				{
					Object.DestroyImmediate(base.gameObject);
				}
			}
			if (!Application.isPlaying && IsUnique())
			{
			}
		}

		private void OnDestroy()
		{
			allGlobals.Remove(this);
		}

		private bool IsUnique()
		{
			return allGlobals.Find((GlobalBlackboard b) => b.name == name && b != this) == null;
		}
	}
}
