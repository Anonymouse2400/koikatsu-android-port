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
	[Description("Set a property on a script")]
	[Name("Set Property (mp)")]
	[Category("✫ Script Control/Multiplatform")]
	public class SetProperty_Multiplatform : ActionTask
	{
		[SerializeField]
		protected SerializedMethodInfo method;

		[SerializeField]
		protected BBObjectParameter parameter;

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
				return string.Format("{0}.{1} = {2}", base.agentInfo, targetMethod.Name, parameter.ToString());
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
				return "No property selected";
			}
			if (targetMethod == null)
			{
				return string.Format("Missing property '{0}'", method.GetMethodString());
			}
			return null;
		}

		protected override void OnExecute()
		{
			targetMethod.Invoke(base.agent, new object[1] { parameter.value });
			EndAction();
		}

		private void SetMethod(MethodInfo method)
		{
			if (method != null)
			{
				this.method = new SerializedMethodInfo(method);
				parameter.SetType(method.GetParameters()[0].ParameterType);
			}
		}
	}
}
