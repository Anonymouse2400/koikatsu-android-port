using ADV.Commands.Sound.Base;
using Manager;

namespace ADV.Commands.Sound.ENV
{
	public class Play : ADV.Commands.Sound.Base.Play
	{
		public Play()
			: base(Manager.Sound.Type.ENV)
		{
		}
	}
}
