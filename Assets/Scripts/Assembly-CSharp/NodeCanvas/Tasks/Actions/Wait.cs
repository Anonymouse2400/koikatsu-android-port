using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Utility")]
	public class Wait : ActionTask
	{
		public BBParameter<float> waitTime = new BBParameter<float>
		{
			value = 1f
		};

		public CompactStatus finishStatus = CompactStatus.Success;

		protected override string info
		{
			get
			{
				return string.Concat("Wait ", waitTime, " sec.");
			}
		}

		protected override void OnUpdate()
		{
			if (base.elapsedTime >= waitTime.value)
			{
				EndAction(finishStatus == CompactStatus.Success);
			}
		}
	}
}
