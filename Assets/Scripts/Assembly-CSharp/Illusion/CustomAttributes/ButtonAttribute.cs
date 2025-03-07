using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public sealed class ButtonAttribute : PropertyAttribute
	{
		public string Function { get; private set; }

		public string Name { get; private set; }

		public object[] Parameters { get; private set; }

		public ButtonAttribute(string function, string name, params object[] parameters)
		{
			Function = function;
			Name = name;
			Parameters = parameters;
		}
	}
}
