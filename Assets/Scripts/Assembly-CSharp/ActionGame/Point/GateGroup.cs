using System.Collections.Generic;
using UnityEngine;

namespace ActionGame.Point
{
	public class GateGroup : MonoBehaviour
	{
		[SerializeField]
		private List<Gate> _gateList = new List<Gate>();

		public List<Gate> gateList
		{
			get
			{
				return _gateList;
			}
		}

		[ContextMenu("Setup")]
		private void Setup()
		{
			_gateList.Clear();
			_gateList.AddRange(Object.FindObjectsOfType<Gate>());
			_gateList.Sort((Gate a, Gate b) => a.ID.CompareTo(b.ID));
		}
	}
}
