namespace Illusion.Component.Correct
{
	public class FrameCorrect : BaseCorrect
	{
		public static string[] FrameNames = new string[24]
		{
			"cf_j_hips", "cf_j_spine01", "cf_j_spine02", "cf_j_spine03", "cf_j_neck", "cf_j_head", "cf_j_waist01", "cf_j_waist02", "cf_j_thumb01_L", "cf_j_index01_L",
			"cf_j_middle01_L", "cf_j_ring01_L", "cf_j_little01_L", "cf_j_thumb01_R", "cf_j_index01_R", "cf_j_middle01_R", "cf_j_ring01_R", "cf_j_little01_R", "cf_j_bust01_L", "cf_j_bust02_L",
			"cf_j_bust03_L", "cf_j_bust01_R", "cf_j_bust02_R", "cf_j_bust03_R"
		};

		public override string[] GetFrameNames
		{
			get
			{
				return FrameNames;
			}
		}
	}
}
