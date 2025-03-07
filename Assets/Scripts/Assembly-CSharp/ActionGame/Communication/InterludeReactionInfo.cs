namespace ActionGame.Communication
{
	public class InterludeReactionInfo : ReactionBase
	{
		public bool isInterlude
		{
			get
			{
				return !assetbundle.IsNullOrEmpty() & !file.IsNullOrEmpty();
			}
		}

		public InterludeReactionInfo(Info.GenericInfo _info)
			: base(_info)
		{
		}
	}
}
