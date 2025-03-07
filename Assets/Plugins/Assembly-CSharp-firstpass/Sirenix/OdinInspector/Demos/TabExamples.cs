using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class TabExamples : MonoBehaviour
	{
		[Serializable]
		public class MyTabObject
		{
			public int A;

			public int B;

			public int C;
		}

		[TabGroup("Tab A", false, 0)]
		public int One;

		[TabGroup("Tab A", false, 0)]
		public int Two;

		[TabGroup("Tab A", false, 0)]
		public int Three;

		[TabGroup("Tab B", false, 0)]
		public string MyString;

		[TabGroup("Tab B", false, 0)]
		public float MyFloat;

		[TabGroup("Tab C", false, 0)]
		[HideLabel]
		public MyTabObject TabC;

		[TabGroup("New Group", "Tab A", false, 0)]
		public int A;

		[TabGroup("New Group", "Tab A", false, 0)]
		public int B;

		[TabGroup("New Group", "Tab A", false, 0)]
		public int C;

		[TabGroup("New Group", "Tab B", false, 0)]
		public string D;

		[TabGroup("New Group", "Tab B", false, 0)]
		public float E;

		[TabGroup("New Group", "Tab C", false, 0)]
		[HideLabel]
		public MyTabObject F;
	}
}
