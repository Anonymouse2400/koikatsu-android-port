using System;
using System.Collections.Generic;
using UnityEngine;

public class AibuEnableState : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string AnimationName;

		public bool Mouth;

		public bool Bust_L;

		public bool Bust_R;

		public bool Kokan;

		public bool Anal;

		public bool Hip_L;

		public bool Hip_R;

		public int MotionState;
	}

	public List<Param> param = new List<Param>();
}
