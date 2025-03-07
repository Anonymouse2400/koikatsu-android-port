using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	[AddComponentMenu("Studio/GUI/Input Field", 1000)]
	public class StudioInputField : InputField
	{
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			base.OnSubmit(eventData);
		}

		protected override void Start()
		{
			base.Start();
		}
	}
}
