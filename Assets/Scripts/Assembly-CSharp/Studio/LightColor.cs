using UnityEngine;

namespace Studio
{
	public class LightColor : MonoBehaviour
	{
		[SerializeField]
		private Renderer renderer;

		private Material material;

		public Color color
		{
			set
			{
				if ((bool)material)
				{
					material.color = value;
				}
			}
		}

		public virtual void Awake()
		{
			material = renderer.material;
		}
	}
}
