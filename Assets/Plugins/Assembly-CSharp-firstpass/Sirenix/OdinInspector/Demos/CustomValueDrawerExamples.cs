using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CustomValueDrawerExamples : MonoBehaviour
	{
		public float From = 2f;

		public float To = 7f;

		[CustomValueDrawer("MyStaticCustomDrawerStatic")]
		public float CustomDrawerStatic;

		[CustomValueDrawer("MyStaticCustomDrawerInstance")]
		public float CustomDrawerInstance;

		[CustomValueDrawer("MyStaticCustomDrawerArray")]
		public float[] CustomDrawerArray;
	}
}
