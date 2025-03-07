using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class SceneAndAssetsOnlyExamples : MonoBehaviour
	{
		[Title("Assets only", null, TitleAlignments.Left, true, true)]
		[AssetsOnly]
		public List<GameObject> OnlyPrefabs;

		[AssetsOnly]
		public GameObject SomePrefab;

		[AssetsOnly]
		public Material MaterialAsset;

		[AssetsOnly]
		public MeshRenderer SomeMeshRendererOnPrefab;

		[Title("Scene Objects only", null, TitleAlignments.Left, true, true)]
		[SceneObjectsOnly]
		public List<GameObject> OnlySceneObjects;

		[SceneObjectsOnly]
		public GameObject SomeSceneObject;

		[SceneObjectsOnly]
		public MeshRenderer SomeMeshRenderer;
	}
}
