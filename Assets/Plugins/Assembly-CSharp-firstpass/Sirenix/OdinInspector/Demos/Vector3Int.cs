using System;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	[InlineProperty(LabelWidth = 15)]
	public struct Vector3Int
	{
		[HorizontalGroup(0f, 0, 0, 0)]
		public int X;

		[HorizontalGroup(0f, 0, 0, 0)]
		public int Y;

		[HorizontalGroup(0f, 0, 0, 0)]
		public int Z;
	}
}
