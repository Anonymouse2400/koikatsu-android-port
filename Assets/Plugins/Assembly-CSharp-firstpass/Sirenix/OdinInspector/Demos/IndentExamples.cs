using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class IndentExamples : MonoBehaviour
	{
		[Header("Nicely organize your properties.")]
		[Indent(1)]
		public int A;

		[Indent(2)]
		public int B;

		[Indent(3)]
		public int C;

		[Indent(4)]
		public int D;

		[Header("Using the Indent attribute")]
		[Indent(1)]
		public int E;

		[Indent(0)]
		public int F;

		[Indent(-1)]
		public int G;
	}
}
