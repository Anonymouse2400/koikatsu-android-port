using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public sealed class ButtonRegulateAttribute : PropertyAttribute
	{
		public string Function { get; private set; }

		public string Name { get; private set; }

		public bool PlayingRegulate { get; private set; }

		public object[] Parameters { get; private set; }

		public ButtonRegulateAttribute(string function, string name, bool playingRegulate, params object[] parameters)
		{
			Function = function;
			Name = name;
			PlayingRegulate = playingRegulate;
			Parameters = parameters;
		}
	}
}
