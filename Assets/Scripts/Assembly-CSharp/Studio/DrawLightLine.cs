using System.Collections.Generic;
using UnityEngine;

namespace Studio
{
	public class DrawLightLine : MonoBehaviour
	{
		[SerializeField]
		private Shader m_Shader;

		private Dictionary<Light, bool> dicLight = new Dictionary<Light, bool>();

		public void Add(Light _light)
		{
			dicLight.Add(_light, true);
		}

		public void Remove(Light _light)
		{
			dicLight.Remove(_light);
		}

		public void Clear()
		{
			dicLight.Clear();
		}

		public void SetEnable(Light _light, bool _value)
		{
			if (dicLight.ContainsKey(_light))
			{
				dicLight[_light] = _value;
			}
		}

		private void Start()
		{
			LightLine.shader = m_Shader;
		}

		public void OnPostRender()
		{
			if (dicLight.Count <= 0)
			{
				return;
			}
			foreach (KeyValuePair<Light, bool> item in dicLight)
			{
				if (item.Value)
				{
					LightLine.DrawLine(item.Key);
				}
			}
		}
	}
}
