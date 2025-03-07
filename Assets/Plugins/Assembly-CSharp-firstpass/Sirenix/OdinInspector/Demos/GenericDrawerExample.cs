using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class GenericDrawerExample : SerializedMonoBehaviour
	{
		[InfoBox("This examples demonstates how a custom drawer can defined to be generic.\nThis allows a single drawer implementation, to deal with a wide array of values.", InfoMessageType.Info, null)]
		[OdinSerialize]
		public MyGenericClass<int, int> A;

		[OdinSerialize]
		public MyGenericClass<Vector3, Quaternion> B;

		[OdinSerialize]
		public MyGenericClass<int, GameObject> C;

		[OdinSerialize]
		public MyGenericClass<string, List<string>> D;

		[OdinSerialize]
		public MyGenericClass<string, string> E;

		public List<MyClass> F;
	}
}
