using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class RandomPlacementFunction : ObjectPlacementFunction
	{
		private static ValueDropdownList<bool> areaTypes = new ValueDropdownList<bool>
		{
			{ "Circle", true },
			{ "Square", false }
		};

		[LabelText("Area")]
		[ValueDropdown("areaTypes")]
		public bool isCircle = true;

		public override Vector3 GetPosition(float t)
		{
			if (isCircle)
			{
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				return new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
			}
			return new Vector3(UnityEngine.Random.value * 2f - 1f, 0f, UnityEngine.Random.value * 2f - 1f);
		}
	}
}
