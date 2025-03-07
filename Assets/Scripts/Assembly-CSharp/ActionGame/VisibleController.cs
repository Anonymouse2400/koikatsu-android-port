using UnityEngine;

namespace ActionGame
{
	public class VisibleController
	{
		public class Visible
		{
			public bool active;

			public bool enable;

			public Renderer renderer;
		}

		public Visible[] Visibles { get; private set; }

		public VisibleController(Visible[] visibles)
		{
			Visibles = visibles;
		}

		public void Change(bool isVisible)
		{
			for (int i = 0; i < Visibles.Length; i++)
			{
				Visible visible = Visibles[i];
				if (visible.active && visible.renderer != null)
				{
					visible.renderer.enabled = isVisible && visible.enable;
				}
			}
		}
	}
}
