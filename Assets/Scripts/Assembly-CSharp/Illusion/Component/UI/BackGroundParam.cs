using Illusion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Component.UI
{
	public class BackGroundParam : MonoBehaviour
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private Image _image;

		private string assetBundleNameChache;

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
		}

		public Camera cam
		{
			get
			{
				return _camera;
			}
		}

		public Image image
		{
			get
			{
				return _image;
			}
		}

		public bool visible
		{
			get
			{
				return _canvas.enabled;
			}
			set
			{
				Canvas obj = _canvas;
				bool flag = value;
				_camera.enabled = flag;
				obj.enabled = flag;
			}
		}

		public void Load(string assetBundleName, string assetName)
		{
			if (!assetBundleName.IsNullOrEmpty())
			{
				assetBundleNameChache = assetBundleName;
			}
			Illusion.Game.Utils.Bundle.LoadSprite(assetBundleNameChache, assetName, _image, false);
		}

		public void Release(bool visible = false)
		{
			_image.sprite = null;
			this.visible = visible;
		}

		public void SetTexture(Texture2D tex2D)
		{
			if (tex2D != null)
			{
				_image.sprite = Sprite.Create(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), Vector2.zero);
			}
			else
			{
				_image.sprite = null;
			}
		}

		[ContextMenu("Setup")]
		private void Setup()
		{
			_canvas = GetComponentInChildren<Canvas>();
			_camera = GetComponentInChildren<Camera>();
			_image = GetComponentInChildren<Image>();
		}
	}
}
