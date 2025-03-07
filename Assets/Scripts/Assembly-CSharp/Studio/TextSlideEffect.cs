using System.Collections.Generic;
using UnityEngine;

namespace Studio
{
	public class TextSlideEffect : TextEffect
	{
		private float m_SubPos;

		public float subPos
		{
			get
			{
				return m_SubPos;
			}
			set
			{
				m_SubPos = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		protected override void Modify(ref List<UIVertex> _stream)
		{
			int i = 0;
			for (int count = _stream.Count; i < count; i += 6)
			{
				for (int j = 0; j < 6; j++)
				{
					UIVertex value = _stream[i + j];
					Vector3 position = value.position;
					position.x -= subPos;
					value.position = position;
					_stream[i + j] = value;
				}
			}
		}
	}
}
