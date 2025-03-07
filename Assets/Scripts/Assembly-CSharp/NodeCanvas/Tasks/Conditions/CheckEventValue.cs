using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
	[EventReceiver(new string[] { "OnCustomEvent" })]
	[Description("Check if an event is received and it's value is equal to specified value, then return true for one frame")]
	[Category("✫ Utility")]
	public class CheckEventValue<T> : ConditionTask<GraphOwner>
	{
		[RequiredField]
		public BBParameter<string> eventName;

		public BBParameter<T> value;

		protected override string info
		{
			get
			{
				return string.Format("Event [{0}].value == {1}", eventName, value);
			}
		}

		protected override bool OnCheck()
		{
			return false;
		}

		public void OnCustomEvent(EventData receivedEvent)
		{
			if (receivedEvent is EventData<T> && base.isActive && receivedEvent.name.ToUpper() == eventName.value.ToUpper())
			{
				T val = ((EventData<T>)receivedEvent).value;
				if (val != null && val.Equals(value.value))
				{
					YieldReturn(true);
				}
			}
		}
	}
}
