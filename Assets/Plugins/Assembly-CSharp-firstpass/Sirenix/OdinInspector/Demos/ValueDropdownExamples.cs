using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ValueDropdownExamples : MonoBehaviour
	{
		public GameObject[] AvailablePrefabs;

		[ValueDropdown("AvailablePrefabs")]
		public GameObject ActivePrefab;

		[ValueDropdown("GetLayers")]
		public string Layer;

		[ValueDropdown("textureSizes")]
		public int TextureSize;

		[ValueDropdown("friendlyTextureSizes")]
		public int FriendlyTextureSize;

		private static int[] textureSizes = new int[3] { 256, 512, 1024 };

		private static ValueDropdownList<int> friendlyTextureSizes = new ValueDropdownList<int>
		{
			{ "Small", 256 },
			{ "Medium", 512 },
			{ "Large", 1024 }
		};

		private static List<string> GetLayers()
		{
			return (from s in Enumerable.Range(0, 32).Select(LayerMask.LayerToName)
				where s.Length > 0
				select s).ToList();
		}
	}
}
