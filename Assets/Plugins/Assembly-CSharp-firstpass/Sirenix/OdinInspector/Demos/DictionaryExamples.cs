using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class DictionaryExamples : SerializedMonoBehaviour
	{
		public class MyCustomType
		{
			public int SomeMember;

			public Quaternion SomeRotation;

			public GameObject SomePrefab;
		}

		[InfoBox("In order to serialize dictionaries, all we need to do is to inherit our class from SerializedMonoBehaviour.", InfoMessageType.Info, null)]
		public Dictionary<int, Material> IntMaterialLookup;

		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
		public Dictionary<MyEnum, MyCustomType> EnumObjectLookup;

		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		public Dictionary<string, List<int>> StringListDictionary;

		public Dictionary<string, string> StringStringDictionary;

		[InfoBox("Odin supports all types for its dictionary values, but for the key you are currently limited.", InfoMessageType.Info, null)]
		[InfoBox("Remember, it is only the Inspector that is lacking support for other key types. Serialization is still going to work as expected.", InfoMessageType.Info, null)]
		public Dictionary<MonoBehaviour, float> NotSupportedDictionary;
	}
}
