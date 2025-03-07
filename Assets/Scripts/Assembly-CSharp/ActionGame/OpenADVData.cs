using ADV;

namespace ActionGame
{
	public class OpenADVData : OpenData
	{
		private ADV.Data _data;

		public override Data data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value as ADV.Data;
			}
		}

		protected override void Start()
		{
			assetBundleName = string.Empty;
			levelName = "ADV";
			base.Start();
		}
	}
}
