using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Script Control/Standalone Only")]
	[Description("Execute a static function of up to 6 parameters and optionaly save the return value")]
	public class ExecuteStaticFunction : ActionTask, ISubParametersContainer
	{
		[SerializeField]
		protected ReflectedWrapper functionWrapper;

		private MethodInfo targetMethod
		{
			get
			{
				return (functionWrapper == null) ? null : functionWrapper.GetMethod();
			}
		}

		protected override string info
		{
			get
			{
				if (functionWrapper == null)
				{
					return "No Method Selected";
				}
				if (targetMethod == null)
				{
					return string.Format("<color=#ff6457>* {0} *</color>", functionWrapper.GetMethodString());
				}
				BBParameter[] variables = functionWrapper.GetVariables();
				string text = string.Empty;
				string text2 = string.Empty;
				if (targetMethod.ReturnType == typeof(void))
				{
					for (int i = 0; i < variables.Length; i++)
					{
						text2 = text2 + ((i == 0) ? string.Empty : ", ") + variables[i].ToString();
					}
				}
				else
				{
					text = ((!variables[0].isNone) ? string.Concat(variables[0], " = ") : string.Empty);
					for (int j = 1; j < variables.Length; j++)
					{
						text2 = text2 + ((j == 1) ? string.Empty : ", ") + variables[j].ToString();
					}
				}
				return string.Format("{0}{1}.{2} ({3})", text, targetMethod.DeclaringType.FriendlyName(), targetMethod.Name, text2);
			}
		}

		BBParameter[] ISubParametersContainer.GetIncludeParseParameters()
		{
			return (functionWrapper == null) ? null : functionWrapper.GetVariables();
		}

		public override void OnValidate(ITaskSystem ownerSystem)
		{
			if (functionWrapper != null && functionWrapper.HasChanged())
			{
				SetMethod(functionWrapper.GetMethod());
			}
			if (functionWrapper != null && targetMethod == null)
			{
				Error(string.Format("Missing Method '{0}'", functionWrapper.GetMethodString()));
			}
		}

		protected override string OnInit()
		{
			if (functionWrapper == null)
			{
				return "No Method selected";
			}
			if (targetMethod == null)
			{
				return string.Format("Missing Method '{0}'", functionWrapper.GetMethodString());
			}
			try
			{
				functionWrapper.Init(null);
				return null;
			}
			catch
			{
				return "ExecuteFunction Error";
			}
		}

		protected override void OnExecute()
		{
			if (targetMethod == null)
			{
				EndAction(false);
				return;
			}
			if (functionWrapper is ReflectedActionWrapper)
			{
				(functionWrapper as ReflectedActionWrapper).Call();
			}
			else
			{
				(functionWrapper as ReflectedFunctionWrapper).Call();
			}
			EndAction();
		}

		private void SetMethod(MethodInfo method)
		{
			if (method != null)
			{
				functionWrapper = ReflectedWrapper.Create(method, base.blackboard);
			}
		}
	}
}
