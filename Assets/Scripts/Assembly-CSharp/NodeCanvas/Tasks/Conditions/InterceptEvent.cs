using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NodeCanvas.Tasks.Conditions
{
	[Description("Returns true when the selected event is triggered on the selected agent.\nYou can use this for both GUI and 3D objects.\nPlease make sure that Unity Event Systems are setup correctly")]
	[Category("UGUI")]
	public class InterceptEvent : ConditionTask<Transform>
	{
		public EventTriggerType eventType;

		protected override string info
		{
			get
			{
				return string.Format("{0} on {1}", eventType.ToString(), base.agentInfo);
			}
		}

		protected override string OnInit()
		{
			RegisterEvent("On" + eventType);
			return null;
		}

		protected override bool OnCheck()
		{
			return false;
		}

		private void OnPointerEnter(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnPointerExit(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnPointerDown(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnPointerUp(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnPointerClick(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnDrag(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnDrop(BaseEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnScroll(PointerEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnUpdateSelected(BaseEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnSelect(BaseEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnDeselect(BaseEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnMove(AxisEventData eventData)
		{
			YieldReturn(true);
		}

		private void OnSubmit(BaseEventData eventData)
		{
			YieldReturn(true);
		}
	}
}
