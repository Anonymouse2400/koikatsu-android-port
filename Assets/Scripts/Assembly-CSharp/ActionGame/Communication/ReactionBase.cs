using System.Diagnostics;

namespace ActionGame.Communication
{
	public class ReactionBase
	{
		public string assetbundle = string.Empty;

		public string file = string.Empty;

		public string pose = string.Empty;

		public string facialExpression = string.Empty;

		public int eyesLook = 1;

		public int neckLook;

		public ReactionBase(Info.GenericInfo _info)
		{
			if (_info != null)
			{
				assetbundle = _info.talk[0].assetbundle;
				file = _info.talk[0].file;
				pose = _info.talk[0].pose;
				facialExpression = _info.talk[0].facialExpression;
				eyesLook = _info.talk[0].eyeLook;
				neckLook = _info.talk[0].neckLook;
			}
		}

		[Conditional("OUTPUT_LOG")]
		private void Log(string _str)
		{
		}
	}
}
