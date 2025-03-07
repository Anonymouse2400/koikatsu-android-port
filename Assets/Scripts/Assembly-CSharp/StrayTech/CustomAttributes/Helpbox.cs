using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class Helpbox : PropertyAttribute, IValidates
	{
		public enum Type
		{
			Info = 0,
			Warning = 1,
			Error = 2
		}

		public readonly Type MessageType;

		public readonly string Message;

		public Helpbox(string message, Type displayType = Type.Info)
		{
			MessageType = displayType;
			Message = message;
		}

		public bool IsValid()
		{
			if (string.IsNullOrEmpty(Message))
			{
				return false;
			}
			return true;
		}
	}
}
