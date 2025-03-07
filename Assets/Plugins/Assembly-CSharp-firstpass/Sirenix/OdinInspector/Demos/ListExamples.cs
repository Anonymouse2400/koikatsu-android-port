using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ListExamples : MonoBehaviour
	{
		[Serializable]
		public struct SomeStruct
		{
			public string SomeString;

			public int One;

			public int Two;

			public int Three;
		}

		public List<float> FloatList;

		[Range(0f, 1f)]
		public float[] FloatRangeArray;

		[ReadOnly]
		public int[] ReadOnlyArray1 = new int[3] { 1, 2, 3 };

		[ListDrawerSettings(IsReadOnly = true)]
		public int[] ReadOnlyArray2 = new int[3] { 1, 2, 3 };

		[ListDrawerSettings(NumberOfItemsPerPage = 5)]
		public int[] FiveItemsPerPage;

		[ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "SomeString")]
		public SomeStruct[] IndexLabels;

		[ListDrawerSettings(DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = false)]
		public int[] MoreListSettings = new int[3] { 1, 2, 3 };

		[ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElement", OnEndListElementGUI = "EndDrawListElement")]
		public SomeStruct[] InjectListElementGUI;

		[ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "DrawAddButton")]
		public List<int> CustomButtons;
	}
}
