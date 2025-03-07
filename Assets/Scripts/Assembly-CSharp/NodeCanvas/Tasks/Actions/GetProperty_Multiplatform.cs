using System;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Get a property of a script and save it to the blackboard")]
	[Category("✫ Script Control/Multiplatform")]
	[Name("Get Property (mp)")]
	public class GetProperty_Multiplatform : ActionTask
	{
		[SerializeField]
		protected SerializedMethodInfo method;

		[SerializeField]
		[BlackboardOnly]
		protected BBObjectParameter returnValue;

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
					return "No Property Selected";
				}
				if (targetMethod == null)
				{
					return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString());
				}
				return string.Format("{0} = {1}.{2}", returnValue.ToString(), base.agentInfo, targetMethod.Name);
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
				Error(string.Format("Missing Property '{0}'", method.GetMethodString()));
			}
		}

		protected override string OnInit()
		{
			if (method == null)
			{
				return "No Property selected";
			}
			if (targetMethod == null)
			{
				return string.Format("Missing Property '{0}'", method.GetMethodString());
			}
			return null;
		}

		protected override void OnExecute()
		{
			returnValue.value = targetMethod.Invoke(base.agent, null);
			EndAction();
		}

		private void SetMethod(MethodInfo method)
		{
			if (method != null)
			{
				this.method = new SerializedMethodInfo(method);
				returnValue.SetType(method.ReturnType);
			}
		}
	}
}
