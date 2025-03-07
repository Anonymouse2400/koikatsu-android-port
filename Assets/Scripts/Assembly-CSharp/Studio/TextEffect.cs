using System.Collections.Generic;
using GUITree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	public abstract class TextEffect : UIBehaviour, IMeshModifier
	{
		private Graphic m_Graphic;

		public Graphic graphic
		{
			get
			{
				if (m_Graphic == null)
				{
					m_Graphic = GetComponent<Graphic>();
				}
				return m_Graphic;
			}
		}

		public void ModifyMesh(Mesh mesh)
		{
		}

		public void ModifyMesh(VertexHelper verts)
		{
			List<UIVertex> _stream = ListPool<UIVertex>.Get();
			verts.GetUIVertexStream(_stream);
			Modify(ref _stream);
			verts.Clear();
			verts.AddUIVertexTriangleStream(_stream);
			ListPool<UIVertex>.Release(_stream);
		}

		protected abstract void Modify(ref List<UIVertex> _stream);
	}
}
