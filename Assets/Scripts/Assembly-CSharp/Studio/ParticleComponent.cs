using Illusion.CustomAttributes;
using UnityEngine;

namespace Studio
{
	public class ParticleComponent : MonoBehaviour
	{
		[Header("カラー１反映対象")]
		public ParticleSystem[] particleColor1;

		[Space]
		public Color defColor01 = Color.white;

		[Button("SetColor", "初期色を設定", new object[] { })]
		public int setcolor;

		public bool[] useColor
		{
			get
			{
				return new bool[3]
				{
					!particleColor1.IsNullOrEmpty(),
					false,
					false
				};
			}
		}

		public bool check
		{
			get
			{
				return !particleColor1.IsNullOrEmpty();
			}
		}

		public void UpdateColor(OIItemInfo _info)
		{
			ParticleSystem[] array = particleColor1;
			foreach (ParticleSystem particleSystem in array)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				main.startColor = _info.color[0];
			}
		}

		public void SetColor()
		{
			if (!particleColor1.IsNullOrEmpty())
			{
				defColor01 = particleColor1[0].main.startColor.color;
			}
		}
	}
}
