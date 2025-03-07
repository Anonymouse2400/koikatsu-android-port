namespace ActionGame.Communication
{
	public class ResultInfo
	{
		public ResultEnum result;

		public bool isFirst;

		public ResultInfo()
		{
			result = ResultEnum.None;
			isFirst = false;
		}
	}
}
