namespace Studio
{
	public class PatternSelectInfo
	{
		public int index = -1;

		public string name = string.Empty;

		public string assetBundle = string.Empty;

		public string assetName = string.Empty;

		public int category;

		public bool disable;

		public bool disvisible;

		public PatternSelectInfoComponent sic;

		public bool activeSelf
		{
			get
			{
				return sic.gameObject.activeSelf;
			}
		}

		public bool interactable
		{
			get
			{
				return sic.tgl.interactable;
			}
		}

		public bool isOn
		{
			get
			{
				return sic.tgl.isOn;
			}
		}
	}
}
