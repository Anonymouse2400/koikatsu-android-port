using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerParameterData : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string name;

		public AnimatorControllerParameterType type;
	}

	public List<Param> list = new List<Param>();
}
