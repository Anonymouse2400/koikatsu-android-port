using ActionGame.Chara;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Illusion/ActionGame/NPC/Map")]
	[Description("マップ移動検索")]
	public class MapSearch : ActionTask
	{
		public BBParameter<int> targetMapNo = new BBParameter<int>
		{
			value = -1
		};

		protected override void OnExecute()
		{
			if (targetMapNo.value != -1)
			{
				base.agent.GetComponent<NPC>().AI.MapRouteFind(targetMapNo.value);
			}
			EndAction();
		}
	}
}
