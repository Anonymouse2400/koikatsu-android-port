using UniRx;
using UnityEngine;

namespace ActionGame
{
	public class NightMenuScene : BaseLoader
	{
		[SerializeField]
		private NightMainMenu mainMenu;

		public Subject<Unit> onLoadSubject
		{
			get
			{
				return mainMenu.onLoadSubject;
			}
		}
	}
}
