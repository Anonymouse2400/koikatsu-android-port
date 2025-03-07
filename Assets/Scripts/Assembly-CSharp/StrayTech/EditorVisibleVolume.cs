using UnityEngine;

namespace StrayTech
{
	[RequireComponent(typeof(BoxCollider))]
	public class EditorVisibleVolume : MonoBehaviour
	{
		[SerializeField]
		private Color _volumeColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private bool _shouldRender = true;

		[SerializeField]
		private bool _shouldRenderOnlyWhenSelected;

		private BoxCollider _collider;

		public Color VolumeColor
		{
			get
			{
				return _volumeColor;
			}
			set
			{
				_volumeColor = value;
			}
		}
	}
}
