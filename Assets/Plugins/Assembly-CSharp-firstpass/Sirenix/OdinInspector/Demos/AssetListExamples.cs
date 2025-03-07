using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class AssetListExamples : MonoBehaviour
	{
		[InfoBox("The AssetList attribute work on both lists of UnityEngine.Object types and UnityEngine.Object types, but have different behaviour.", InfoMessageType.Info, null)]
		[AssetList]
		[InlineEditor(InlineEditorModes.LargePreview)]
		public GameObject Prefab;

		[AssetList]
		public List<PlaceableObject> PlaceableObjects;

		[FoldoutGroup("Filtered AssetLists examples", false, 100)]
		[AssetList(Path = "Plugins/Sirenix/")]
		[InlineEditor(InlineEditorModes.LargePreview)]
		public Object Object;

		[AssetList(AutoPopulate = true)]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<PlaceableObject> PlaceableObjectsAutoPopulated;

		[AssetList(LayerNames = "MyLayerName")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public GameObject[] AllPrefabsWithLayerName;

		[AssetList(AssetNamePrefix = "Rock")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<GameObject> PrefabsStartingWithRock;

		[AssetList(Path = "/Plugins/Sirenix/")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<GameObject> AllPrefabsLocatedInFolder;

		[AssetList(Tags = "MyTagA, MyTabB", Path = "/Plugins/Sirenix/")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<GameObject> GameObjectsWithTag;

		[AssetList(Path = "/Plugins/Sirenix/")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<Material> AllMaterialsInSirenix;

		[AssetList(Path = "/Plugins/Sirenix/")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<ScriptableObject> AllScriptableObjects;

		[InfoBox("Use a method as a custom filter for the asset list.", InfoMessageType.Info, null)]
		[AssetList(CustomFilterMethod = "HasRigidbodyComponent")]
		[FoldoutGroup("Filtered AssetLists examples", 0)]
		public List<GameObject> MyRigidbodyPrefabs;

		private bool HasRigidbodyComponent(GameObject obj)
		{
			return obj.GetComponent<Rigidbody>() != null;
		}
	}
}
