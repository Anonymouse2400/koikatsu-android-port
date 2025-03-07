using UnityEngine;

namespace Studio
{
	public class GuideViewCtrl : MonoBehaviour
	{
		[SerializeField]
		private Camera camera;

		[SerializeField]
		private LayerMask layerMask = -1;

		private LayerMask layerMaskDefault = -1;

		[SerializeField]
		private DrawLightLine drawLightLine;

		private bool isDefault = true;

		public void OnClick()
		{
			isDefault = !isDefault;
			camera.cullingMask = ((!isDefault) ? layerMask : layerMaskDefault);
			drawLightLine.enabled = isDefault;
		}

		private void Awake()
		{
			camera.enabled = true;
			layerMaskDefault = camera.cullingMask;
			drawLightLine.enabled = true;
			isDefault = true;
		}
	}
}
