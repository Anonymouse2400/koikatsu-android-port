using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Utility")]
	[Description("Send a Graph Event to multiple gameobjects which should have a GraphOwner component attached.")]
	public class SendEventToObjects : ActionTask
	{
		[RequiredField]
		public BBParameter<List<GameObject>> targetObjects;

		[RequiredField]
		public BBParameter<string> eventName;

		protected override string info
		{
			get
			{
				return string.Format("Send Event [{0}] to {1}", eventName, targetObjects);
			}
		}

		protected override void OnExecute()
		{
			foreach (GameObject item in targetObjects.value)
			{
				if (item != null)
				{
					GraphOwner component = item.GetComponent<GraphOwner>();
					if (component != null)
					{
						component.SendEvent(eventName.value);
					}
				}
			}
			EndAction();
		}
	}
	[Description("Send a Graph Event to multiple gameobjects which should have a GraphOwner component attached.")]
	[Category("✫ Utility")]
	public class SendEventToObjects<T> : ActionTask
	{
		[RequiredField]
		public BBParameter<List<GameObject>> targetObjects;

		[RequiredField]
		public BBParameter<string> eventName;

		public BBParameter<T> eventValue;

		protected override string info
		{
			get
			{
				return string.Format("Send Event [{0}]({1}) to {2}", eventName, eventValue, targetObjects);
			}
		}

		protected override void OnExecute()
		{
			foreach (GameObject item in targetObjects.value)
			{
				GraphOwner component = item.GetComponent<GraphOwner>();
				if (component != null)
				{
					component.SendEvent(eventName.value, eventValue.value);
				}
			}
			EndAction();
		}
	}
}
