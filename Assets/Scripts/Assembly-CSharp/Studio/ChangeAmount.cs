using System;
using System.IO;
using UnityEngine;

namespace Studio
{
	public class ChangeAmount
	{
		protected Vector3 m_Pos = Vector3.zero;

		protected Vector3 m_Rot = Vector3.zero;

		protected Vector3 m_Scale = Vector3.one;

		public Action onChangePos;

		public Action onChangePosAfter;

		public Action onChangeRot;

		public Action<Vector3> onChangeScale;

		public Vector3 pos
		{
			get
			{
				return m_Pos;
			}
			set
			{
				if (Utility.SetStruct(ref m_Pos, value) && onChangePos != null)
				{
					onChangePos();
					if (onChangePosAfter != null)
					{
						onChangePosAfter();
					}
				}
			}
		}

		public Vector3 rot
		{
			get
			{
				return m_Rot;
			}
			set
			{
				if (Utility.SetStruct(ref m_Rot, value) && onChangeRot != null)
				{
					onChangeRot();
				}
			}
		}

		public Vector3 scale
		{
			get
			{
				return m_Scale;
			}
			set
			{
				if (Utility.SetStruct(ref m_Scale, value) && onChangeScale != null)
				{
					onChangeScale(value);
				}
			}
		}

		public ChangeAmount()
		{
			m_Pos = Vector3.zero;
			m_Rot = Vector3.zero;
			m_Scale = Vector3.one;
		}

		public void Save(BinaryWriter _writer)
		{
			_writer.Write(m_Pos.x);
			_writer.Write(m_Pos.y);
			_writer.Write(m_Pos.z);
			_writer.Write(m_Rot.x);
			_writer.Write(m_Rot.y);
			_writer.Write(m_Rot.z);
			_writer.Write(m_Scale.x);
			_writer.Write(m_Scale.y);
			_writer.Write(m_Scale.z);
		}

		public void Load(BinaryReader _reader)
		{
			Vector3 vector = m_Pos;
			vector.Set(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
			m_Pos = vector;
			vector = m_Rot;
			vector.Set(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
			m_Rot = vector;
			vector = m_Scale;
			vector.Set(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
			m_Scale = vector;
		}

		public ChangeAmount Clone()
		{
			ChangeAmount changeAmount = new ChangeAmount();
			changeAmount.pos = new Vector3(m_Pos.x, m_Pos.y, m_Pos.z);
			changeAmount.rot = new Vector3(m_Rot.x, m_Rot.y, m_Rot.z);
			changeAmount.scale = new Vector3(m_Scale.x, m_Scale.y, m_Scale.z);
			return changeAmount;
		}

		public void Copy(ChangeAmount _source, bool _pos = true, bool _rot = true, bool _scale = true)
		{
			if (_pos)
			{
				pos = _source.pos;
			}
			if (_rot)
			{
				rot = _source.rot;
			}
			if (_scale)
			{
				scale = _source.scale;
			}
		}

		public void OnChange()
		{
			if (onChangePos != null)
			{
				onChangePos();
				if (onChangePosAfter != null)
				{
					onChangePosAfter();
				}
			}
			if (onChangeRot != null)
			{
				onChangeRot();
			}
			if (onChangeScale != null)
			{
				onChangeScale(scale);
			}
		}

		public void Reset()
		{
			pos = Vector3.zero;
			rot = Vector3.zero;
			scale = Vector3.one;
		}
	}
}
