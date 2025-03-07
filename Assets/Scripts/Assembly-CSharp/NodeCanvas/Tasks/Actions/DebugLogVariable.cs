using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Logs the value of a variable in the console")]
	[Category("✫ Utility")]
	public class DebugLogVariable : ActionTask
	{
		[BlackboardOnly]
		public BBParameter<object> log;

		public BBParameter<string> prefix;

		public float secondsToRun = 1f;

		public CompactStatus finishStatus = CompactStatus.Success;

		protected override string info
		{
			get
			{
				return string.Concat("Log '", log, "'", (!(secondsToRun > 0f)) ? string.Empty : (" for " + secondsToRun + " sec."));
			}
		}

		protected override void OnExecute()
		{
		}

		protected override void OnUpdate()
		{
			if (base.elapsedTime >= secondsToRun)
			{
				EndAction(finishStatus == CompactStatus.Success);
			}
		}
	}
}
