namespace Illusion.Component.Correct
{
	public class IKCorrect : BaseCorrect
	{
		public static string[] FrameNames = new string[13]
		{
			"cf_t_hips", "cf_t_waist_L", "cf_t_waist_R", "cf_t_shoulder_L", "cf_t_shoulder_R", "cf_t_hand_L", "cf_t_elbo_L", "cf_t_hand_R", "cf_t_elbo_R", "cf_t_leg_L",
			"cf_t_knee_L", "cf_t_leg_R", "cf_t_knee_R"
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
