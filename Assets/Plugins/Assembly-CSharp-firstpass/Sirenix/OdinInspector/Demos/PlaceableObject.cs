using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class PlaceableObject : MonoBehaviour
	{
		public bool Enabled;

		[EnableIf("Enabled")]
		[Range(0f, 1f)]
		public float SpawnChance = 0.5f;

		[EnableIf("Enabled")]
		[LabelText("Scale")]
		[MinMaxSlider(0f, 2f, false)]
		public Vector2 MinMaxScaleSize = new Vector2(0.5f, 1.2f);
	}
}
