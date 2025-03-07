using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class IncorrectUseOfOdinSerializeAttributeExamples1 : MonoBehaviour
	{
		[Serializable]
		public class MyCustomType1
		{
			public int Test;
		}

		public class MyCustomType2
		{
			public int Test;
		}

		[OdinSerialize]
		[ShowInInspector]
		private MyCustomType1 UnitySerializedField1;

		[OdinSerialize]
		[ShowInInspector]
		public MyCustomType2 UnitySerializedField2;

		[OdinSerialize]
		[ShowInInspector]
		public MyCustomType1 UnitySerializedProperty1 { get; private set; }

		[OdinSerialize]
		[ShowInInspector]
		public MyCustomType2 UnitySerializedProperty2 { get; private set; }
	}
}
