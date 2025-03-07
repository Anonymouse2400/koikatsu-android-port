using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
	[Description("Simply use to debug return true or false by inverting the condition if needed")]
	[Category("✫ Utility")]
	public class DebugCondition : ConditionTask
	{
		protected override bool OnCheck()
		{
			return false;
		}
	}
}
