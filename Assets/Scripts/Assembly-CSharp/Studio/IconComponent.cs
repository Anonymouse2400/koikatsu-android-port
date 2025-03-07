using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Studio
{
	public class IconComponent : MonoBehaviour
	{
		[SerializeField]
		private Renderer renderer;

		private Transform transTarget;

		private Transform transRender;

		public bool active
		{
			get
			{
				return renderer.enabled;
			}
			set
			{
				renderer.enabled = value;
			}
		}

		private void Awake()
		{
			transRender = renderer.transform;
			transTarget = Camera.main.transform;
			(from _ in this.UpdateAsObservable()
				where renderer.enabled
				select _).Subscribe(delegate
			{
				transRender.LookAt(transTarget.position);
			});
		}
	}
}
