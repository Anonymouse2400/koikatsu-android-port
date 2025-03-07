using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class MyToggleObject
	{
		[HideInInspector]
		public bool Enabled;

		[HideInInspector]
		public string Title;

		public int A;

		public int B;
	}
}
