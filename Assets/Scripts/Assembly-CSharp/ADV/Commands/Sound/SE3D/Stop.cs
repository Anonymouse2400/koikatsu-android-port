using ADV.Commands.Sound.Base;
using Manager;

namespace ADV.Commands.Sound.SE3D
{
	public class Stop : ADV.Commands.Sound.Base.Stop
	{
		public Stop()
			: base(Manager.Sound.Type.GameSE3D)
		{
		}
	}
}
