using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SceneAssist
{
	public class PointerEnterAction : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		public List<UnityAction> listAction = new List<UnityAction>();

		public void OnPointerEnter(PointerEventData eventData)
		{
			foreach (UnityAction item in listAction)
			{
				item();
			}
		}
	}
}
