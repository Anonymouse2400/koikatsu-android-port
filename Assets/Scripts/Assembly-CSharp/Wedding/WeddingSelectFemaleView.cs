using TMPro;
using UnityEngine;

namespace Wedding
{
	public class WeddingSelectFemaleView : WeddingSelectBaseView
	{
		[SerializeField]
		private TMP_Dropdown _dropdownPersonality;

		public TMP_Dropdown dropdownPersonality
		{
			get
			{
				return _dropdownPersonality;
			}
		}
	}
}
