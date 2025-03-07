using System;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public struct Vector2Int
	{
		[HorizontalGroup(0f, 0, 0, 0)]
		public int X;

		[HorizontalGroup(0f, 0, 0, 0)]
		public int Y;
	}
}
