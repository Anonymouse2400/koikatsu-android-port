using System;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Script Control/Common")]
	[Description("Get a variable of a script and save it to the blackboard")]
	public class GetField : ActionTask
	{
		[SerializeField]
		protected Type targetType;

		[SerializeField]
		protected string fieldName;

		[BlackboardOnly]
		[SerializeField]
		protected BBObjectParameter saveAs;

		private FieldInfo field;

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
				if (string.IsNullOrEmpty(fieldName))
				{
					return "No Field Selected";
				}
				return string.Format("{0} = {1}.{2}", saveAs.ToString(), base.agentInfo, fieldName);
			}
		}

		protected override string OnInit()
		{
			field = agentType.RTGetField(fieldName);
			if (field == null)
			{
				return "Missing Field: " + fieldName;
			}
			return null;
		}

		protected override void OnExecute()
		{
			saveAs.value = field.GetValue(base.agent);
			EndAction();
		}
	}
}
