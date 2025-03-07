using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class TableMatrixExamples : SerializedMonoBehaviour
	{
		[BoxGroup("Two Dimensional array without the TableMatrix attribute.", true, false, 0)]
		public bool[,] BooleanTable = new bool[15, 6];

		[BoxGroup("ReadOnly table", true, false, 0)]
		[TableMatrix(IsReadOnly = true)]
		public int[,] ReadOnlyTable = new int[5, 5];

		[BoxGroup("Labled table", true, false, 0)]
		[TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
		public GameObject[,] LabledTable = new GameObject[15, 10];

		[BoxGroup("Enum table", true, false, 0)]
		[TableMatrix(HorizontalTitle = "X axis")]
		public InfoMessageType[,] EnumTable = new InfoMessageType[4, 4];

		[BoxGroup("Custom table", true, false, 0)]
		[TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
		public bool[,] CustomCellDrawing = new bool[30, 30];

		[InfoBox("Right-click and drag the column and row labels in order to modify the tables.", InfoMessageType.Info, null)]
		[PropertyOrder(-10)]
		[OnInspectorGUI]
		private void MSG()
		{
		}
	}
}
