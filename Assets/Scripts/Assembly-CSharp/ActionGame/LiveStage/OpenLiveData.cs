using System;

namespace ActionGame.LiveStage
{
	public class OpenLiveData : OpenData
	{
		[Serializable]
		public new class Data : OpenData.Data
		{
			public int coordinatetype;

			public SaveData.Heroine heroine;

			public string clothFullPath = string.Empty;
		}

		private Data _data;

		public override OpenData.Data data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value as Data;
			}
		}

		protected override void Start()
		{
			assetBundleName = string.Empty;
			levelName = "LiveStage";
			base.Start();
		}
	}
}
