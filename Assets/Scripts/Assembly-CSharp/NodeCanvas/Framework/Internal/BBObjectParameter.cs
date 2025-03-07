using System;
using ParadoxNotion;
using UnityEngine;

namespace NodeCanvas.Framework.Internal
{
	[Serializable]
	public class BBObjectParameter : BBParameter<object>
	{
		[SerializeField]
		private Type _type;

		public override Type varType
		{
			get
			{
				return _type;
			}
		}

		public BBObjectParameter()
		{
			SetType(typeof(object));
		}

		public BBObjectParameter(Type t)
		{
			SetType(t);
		}

		public void SetType(Type t)
		{
			if (t == null)
			{
				t = typeof(object);
			}
			if (t != _type)
			{
				_value = ((!t.RTIsValueType()) ? null : Activator.CreateInstance(t));
			}
			_type = t;
		}
	}
}
