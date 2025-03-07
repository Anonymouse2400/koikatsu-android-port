using System;
using System.Collections.Generic;
using UnityEngine;

public class ClassSchedule : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string Week;

		public string Lesson1;

		public string Lesson2;
	}

	public List<Param> param = new List<Param>();
}
