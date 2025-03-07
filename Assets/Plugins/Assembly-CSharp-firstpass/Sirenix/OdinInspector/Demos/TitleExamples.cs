using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class TitleExamples : MonoBehaviour
	{
		[Title("Titles and Headers", null, TitleAlignments.Left, true, true)]
		[InfoBox("The Title attribute has the same purpose as Unity's Header attribute,but it also supports properties, and methods.\n\nTitle also offers more features such as subtitles, options for horizontal underline, bold text and text alignment.\n\nBoth attributes, with Odin, supports either static strings, or refering to members strings by adding a $ in front.", InfoMessageType.Info, null)]
		public string MyTitle = "My Dynamic Title";

		public string MySubtitle = "My Dynamic Subtitle";

		[Title("Static title", null, TitleAlignments.Left, true, true)]
		public int C;

		public int D;

		[Title("Static title", "Static subtitle", TitleAlignments.Left, true, true)]
		public int E;

		public int F;

		[Title("$MyTitle", "$MySubtitle", TitleAlignments.Left, true, true)]
		public int G;

		public int H;

		[Title("Non bold title", "$MySubtitle", TitleAlignments.Left, true, false)]
		public int I;

		public int J;

		[Title("Non bold title", "With no line seperator", TitleAlignments.Left, false, false)]
		public int K;

		public int L;

		[Title("$MyTitle", "$MySubtitle", TitleAlignments.Right, true, true)]
		public int M;

		public int N;

		[Title("$MyTitle", "$MySubtitle", TitleAlignments.Centered, true, true)]
		public int O;

		public int P;

		[Title("$Combined", null, TitleAlignments.Centered, true, true)]
		public int Q;

		public int R;

		[ShowInInspector]
		[Title("Title on a Property", null, TitleAlignments.Left, true, true)]
		public int S { get; set; }

		public string Combined
		{
			get
			{
				return MyTitle + " - " + MySubtitle;
			}
		}

		[Title("Title on a Method", null, TitleAlignments.Left, true, true)]
		[Button(ButtonSizes.Small)]
		public void DoNothing()
		{
		}
	}
}
