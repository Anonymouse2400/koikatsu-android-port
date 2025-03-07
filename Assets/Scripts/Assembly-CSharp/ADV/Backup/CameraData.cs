using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace ADV.Backup
{
	internal class CameraData
	{
		private class BlurBK
		{
			private bool enabled;

			private int iterations;

			private float blurSpread;

			public BlurBK(Blur blur)
			{
				if (!(blur == null))
				{
					enabled = blur.enabled;
					iterations = blur.iterations;
					blurSpread = blur.blurSpread;
				}
			}

			public void Set(Blur blur)
			{
				if (!(blur == null))
				{
					blur.enabled = enabled;
					blur.iterations = iterations;
					blur.blurSpread = blurSpread;
				}
			}
		}

		private class DOFBK
		{
			private bool enabled;

			private Transform focalTransform;

			private float focalLength;

			private float focalSize;

			private float aperture;

			public DOFBK(DepthOfField dof)
			{
				if (!(dof == null))
				{
					enabled = dof.enabled;
					focalTransform = dof.focalTransform;
					focalLength = dof.focalLength;
					focalSize = dof.focalSize;
					aperture = dof.aperture;
				}
			}

			public void Set(DepthOfField dof)
			{
				if (!(dof == null))
				{
					dof.enabled = enabled;
					dof.focalTransform = focalTransform;
					dof.focalLength = focalLength;
					dof.focalSize = focalSize;
					dof.aperture = aperture;
				}
			}
		}

		private Rect rect;

		private Transform parent;

		private float fov;

		private float far;

		private BlurBK blurBK;

		private DOFBK dofBK;

		public CameraData(Camera cam)
		{
			blurBK = null;
			dofBK = null;
			if (!(cam == null))
			{
				rect = cam.rect;
				parent = cam.transform.parent;
				fov = cam.fieldOfView;
				far = cam.farClipPlane;
				Blur component = cam.GetComponent<Blur>();
				if (component != null)
				{
					blurBK = new BlurBK(component);
				}
				DepthOfField component2 = cam.GetComponent<DepthOfField>();
				if (component2 != null)
				{
					dofBK = new DOFBK(component2);
				}
			}
		}

		public void Load(Camera cam)
		{
			if (!(cam == null))
			{
				cam.rect = rect;
				cam.transform.parent = parent;
				cam.fieldOfView = fov;
				cam.farClipPlane = far;
				blurBK.SafeProc(delegate(BlurBK p)
				{
					p.Set(cam.GetComponent<Blur>());
				});
				dofBK.SafeProc(delegate(DOFBK p)
				{
					p.Set(cam.GetComponent<DepthOfField>());
				});
			}
		}
	}
}
