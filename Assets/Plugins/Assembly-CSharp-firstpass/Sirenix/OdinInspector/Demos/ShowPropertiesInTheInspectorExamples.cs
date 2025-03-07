using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ShowPropertiesInTheInspectorExamples : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private int evenNumber;

		[ShowInInspector]
		public int EvenNumber
		{
			get
			{
				return evenNumber;
			}
			set
			{
				evenNumber = value + value % 2;
			}
		}
	}
}
