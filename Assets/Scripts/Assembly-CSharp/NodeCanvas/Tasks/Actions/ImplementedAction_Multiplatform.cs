using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Calls a function that has signature of 'public Status NAME()' or 'public Status NAME(T)'. You should return Status.Success, Failure or Running within that function.")]
	[Name("Implemented Action (mp)")]
	[Category("✫ Script Control/Multiplatform")]
	public class ImplementedAction_Multiplatform : ActionTask
	{
		[SerializeField]
		private SerializedMethodInfo method;

		[SerializeField]
		private List<BBObjectParameter> parameters = new List<BBObjectParameter>();

		private Status actionStatus = Status.Resting;

		private MethodInfo targetMethod
		{
			get
			{
				return (method == null) ? null : method.Get();
			}
		}

		public override Type agentType
		{
			get
			{
				return (targetMethod == null) ? typeof(Transform) : targetMethod.RTReflectedType();
			}
		}

		protected override string info
		{
			get
			{
				if (method == null)
				{
					return "No Action Selected";
				}
				if (targetMethod == null)
				{
					return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString());
				}
				return string.Format("[ {0}.{1}({2}) ]", base.agentInfo, targetMethod.Name, (parameters.Count != 1) ? string.Empty : parameters[0].ToString());
			}
		}

		public override void OnValidate(ITaskSystem ownerSystem)
		{
			if (method != null && method.HasChanged())
			{
				SetMethod(method.Get());
			}
			if (method != null && method.Get() == null)
			{
				Error(string.Format("Missing Method '{0}'", method.GetMethodString()));
			}
		}

		protected override string OnInit()
		{
			if (method == null)
			{
				return "No method selected";
			}
			if (targetMethod == null)
			{
				return string.Format("Missing method '{0}'", method.GetMethodString());
			}
			return null;
		}

		protected override void OnExecute()
		{
			Forward();
		}

		protected override void OnUpdate()
		{
			Forward();
		}

		private void Forward()
		{
			object[] array = parameters.Select((BBObjectParameter p) => p.value).ToArray();
			actionStatus = (Status)targetMethod.Invoke(base.agent, array);
			if (actionStatus == Status.Success)
			{
				EndAction(true);
			}
			else if (actionStatus == Status.Failure)
			{
				EndAction(false);
			}
		}

		protected override void OnStop()
		{
			actionStatus = Status.Resting;
		}

		private void SetMethod(MethodInfo method)
		{
			if (method != null)
			{
				this.method = new SerializedMethodInfo(method);
				parameters.Clear();
				ParameterInfo[] array = method.GetParameters();
				foreach (ParameterInfo parameterInfo in array)
				{
					BBObjectParameter bBObjectParameter = new BBObjectParameter(parameterInfo.ParameterType);
					bBObjectParameter.bb = base.blackboard;
					BBObjectParameter item = bBObjectParameter;
					parameters.Add(item);
				}
			}
		}
	}
}
