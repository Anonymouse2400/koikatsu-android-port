using ADV.Commands.Sound.Base;
using Manager;

namespace ADV.Commands.Sound.SE2D
{
	public class Play : ADV.Commands.Sound.Base.Play
	{
		public Play()
			: base(Manager.Sound.Type.GameSE2D)
		{
		}
	}
}
