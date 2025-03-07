using System;
using System.Collections;
using ActionGame.Chara;
using Illusion.Component.UI.ToolTip;
using UnityEngine;

namespace ActionGame
{
	public class SpeakInfoPos : InfoPos
	{
		[SerializeField]
		private Base chara;

		private Canvas _canvas;

		private Canvas canvas
		{
			get
			{
				return this.GetComponentCache(ref _canvas);
			}
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			yield return new WaitUntil(() => chara.initialized);
			base.target = chara.Head;
			base.enabled = true;
		}

		protected override void Update()
		{
			base.Update();
			if (!(Camera.main == null))
			{
				Plane[] array = GeometryUtility.CalculateFrustumPlanes(Camera.main);
				Array.Resize(ref array, 4);
				canvas.enabled = GeometryUtility.TestPlanesAABB(array, chara.baseCollider.bounds);
			}
		}
	}
}
