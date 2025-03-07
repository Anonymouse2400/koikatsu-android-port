using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class SerializeAnything : SerializedMonoBehaviour
	{
		public MyGeneric<float> MyGenericFloat;

		public MyGeneric<GameObject[]> MyGenericGameObjects;

		public Dictionary<string, ISomeInterface> MyDictionary;

		public Vector3? NullableVector3;
	}
}
