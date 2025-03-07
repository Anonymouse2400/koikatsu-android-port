using System;
using System.Reflection;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("✫ Script Control/Common")]
	[Description("Will subscribe to a public event of Action type and return true when the event is raised.\n(eg public event System.Action [name])")]
	public class CheckCSharpEvent : ConditionTask
	{
		[SerializeField]
		private Type targetType;

		[SerializeField]
		private string eventName;

		public override Type agentType
		{
			get
			{
				return (targetType == null) ? typeof(Transform) : targetType;
			}
		}

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
			EventInfo eventInfo = agentType.RTGetEvent(eventName);
			if (eventInfo == null)
			{
				return "Event was not found";
			}
			MethodInfo method = GetType().RTGetMethod("Raised");
			Delegate handler = method.RTCreateDelegate(eventInfo.EventHandlerType, this);
			eventInfo.AddEventHandler(base.agent, handler);
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
	[Description("Will subscribe to a public event of Action<T> type and return true when the event is raised.\n(eg public event System.Action<T> [name])")]
	public class CheckCSharpEvent<T> : ConditionTask
	{
		[SerializeField]
		private Type targetType;

		[SerializeField]
		private string eventName;

		[SerializeField]
		private BBParameter<T> saveAs;

		public override Type agentType
		{
			get
			{
				return targetType ?? typeof(Transform);
			}
		}

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
			EventInfo eventInfo = agentType.RTGetEvent(eventName);
			if (eventInfo == null)
			{
				return "Event was not found";
			}
			MethodInfo method = GetType().RTGetMethod("Raised");
			Delegate handler = method.RTCreateDelegate(eventInfo.EventHandlerType, this);
			eventInfo.AddEventHandler(base.agent, handler);
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
