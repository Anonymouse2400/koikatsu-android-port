using System;
using System.Reflection;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[Description("Will subscribe to a public event of Action type and return true when the event is raised.\n(eg public static event System.Action [name])")]
	[Category("✫ Script Control/Common")]
	public class CheckStaticCSharpEvent : ConditionTask
	{
		[SerializeField]
		private Type targetType;

		[SerializeField]
		private string eventName;

		protected override string info
		{
			get
			{
				if (string.IsNullOrEmpty(eventName))
				{
					return "No Event Selected";
				}
				return string.Format("'{0}' Raised", eventName);
			}
		}

		protected override string OnInit()
		{
			if (eventName == null)
			{
				return "No Event Selected";
			}
			EventInfo eventInfo = targetType.RTGetEvent(eventName);
			if (eventInfo == null)
			{
				return "Event was not found";
			}
			Action handler = delegate
			{
				Raised();
			};
			eventInfo.AddEventHandler(null, handler);
			return null;
		}

		public void Raised()
		{
			YieldReturn(true);
		}

		protected override bool OnCheck()
		{
			return false;
		}
	}
	[Category("✫ Script Control/Common")]
	[Description("Will subscribe to a public event of Action type and return true when the event is raised.\n(eg public static event System.Action<T> [name])")]
	public class CheckStaticCSharpEvent<T> : ConditionTask
	{
		[SerializeField]
		private Type targetType;

		[SerializeField]
		private string eventName;

		[BlackboardOnly]
		[SerializeField]
		private BBParameter<T> saveAs;

		protected override string info
		{
			get
			{
				if (string.IsNullOrEmpty(eventName))
				{
					return "No Event Selected";
				}
				return string.Format("'{0}' Raised", eventName);
			}
		}

		protected override string OnInit()
		{
			if (eventName == null)
			{
				return "No Event Selected";
			}
			EventInfo eventInfo = targetType.RTGetEvent(eventName);
			if (eventInfo == null)
			{
				return "Event was not found";
			}
			Action<T> handler = delegate(T v)
			{
				Raised(v);
			};
			eventInfo.AddEventHandler(null, handler);
			return null;
		}

		public void Raised(T eventValue)
		{
			saveAs.value = eventValue;
			YieldReturn(true);
		}

		protected override bool OnCheck()
		{
			return false;
		}
	}
}
