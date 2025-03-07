using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ToggleExamples : MonoBehaviour
	{
		[ToggleGroup("MyToggle", 0, null)]
		public bool MyToggle;

		[ToggleGroup("MyToggle", 0, null)]
		public float A;

		[ToggleGroup("MyToggle", 0, null)]
		[HideLabel]
		[Multiline]
		public string B;

		[ToggleGroup("EnableGroupOne", "$GroupOneTitle")]
		public bool EnableGroupOne;

		[ToggleGroup("EnableGroupOne", 0, null)]
		public string GroupOneTitle = "One";

		[ToggleGroup("EnableGroupOne", 0, null)]
		public float GroupOneA;

		[ToggleGroup("EnableGroupOne", 0, null)]
		public float GroupOneB;

		[Toggle("Enabled")]
		public MyToggleObject Three = new MyToggleObject();

		[Toggle("Enabled")]
		public MyToggleObject Four = new MyToggleA();

		[Toggle("Enabled")]
		public MyToggleObject Five = new MyToggleB();

		public MyToggleC[] ToggleList;
	}
}
