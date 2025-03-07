using System;
using Manager;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class Expression
	{
		public int personality;

		public string name;

		public virtual void Setting(ChaControl chaCtrl, int personality, string name)
		{
			Manager.Game.Expression expression = Singleton<Manager.Game>.Instance.GetExpression(personality, name);
			if (expression != null)
			{
				expression.Change(chaCtrl);
			}
		}

		public virtual void Setting(ChaControl chaCtrl)
		{
			Setting(chaCtrl, personality, name);
		}
	}
}
