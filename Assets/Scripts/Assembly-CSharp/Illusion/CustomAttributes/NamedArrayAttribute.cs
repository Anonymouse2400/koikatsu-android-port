using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	public class NamedArrayAttribute : PropertyAttribute
	{
		public readonly string[] names;

		public NamedArrayAttribute(params string[] names)
		{
			this.names = names;
		}

		public NamedArrayAttribute(Type enumType)
		{
			names = Enum.GetNames(enumType);
		}
	}
}
