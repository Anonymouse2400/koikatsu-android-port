using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class PrefabRelatedAttributesExamples : MonoBehaviour
	{
		[HideInPrefabAssets]
		public GameObject HiddenInPrefabAssets;

		[HideInPrefabInstances]
		public GameObject HiddenInPrefabInstances;

		[HideInPrefabs]
		public GameObject HiddenInPrefabs;

		[HideInNonPrefabs]
		public GameObject HiddenInNonPrefabs;

		[DisableInPrefabAssets]
		public GameObject DisabledInPrefabAssets;

		[DisableInPrefabInstances]
		public GameObject DisabledInPrefabInstances;

		[DisableInPrefabs]
		public GameObject DisabledInPrefabs;

		[DisableInNonPrefabs]
		public GameObject DisabledInNonPrefabs;
	}
}
