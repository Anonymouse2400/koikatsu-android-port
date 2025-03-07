using System;
using Sirenix.Serialization;

namespace Sirenix.OdinInspector.Demos
{
	public class IncorrectUseOfOdinSerializeAttributeExamples2 : SerializedMonoBehaviour
	{
		[Serializable]
		public class MyCustomType
		{
			public int Test;
		}

		[OdinSerialize]
		public MyCustomType UnityAndOdinSerializedField1;

		[OdinSerialize]
		public int UnityAndOdinSerializedField2;
	}
}
