namespace ActionGame.Communication
{
	public class PassingInfo
	{
		public SaveData.Player player;

		public SaveData.Heroine heroine;

		public NecessaryInfo info;

		public BaseMap map;

		public int mapNo
		{
			get
			{
				return (info == null) ? (-1) : info.mapNo;
			}
		}

		public bool exposure
		{
			get
			{
				return info == null || info.exposure;
			}
		}

		public string[] placeNames
		{
			get
			{
				return (info == null) ? new string[0] : info.placeNames;
			}
		}

		public Cycle.Type cycleType
		{
			get
			{
				return (info == null) ? Cycle.Type.LunchTime : info.cycleType;
			}
		}

		public bool isOtherPeople
		{
			get
			{
				return info == null || info.isOtherPeople;
			}
		}

		public bool isChase
		{
			get
			{
				return info != null && info.isChase;
			}
		}

		public bool isChasePossible
		{
			get
			{
				return info != null && info.isChasePossible;
			}
		}

		public bool isHPossible
		{
			get
			{
				return info != null && info.isHPossible;
			}
		}

		public bool isNotice
		{
			get
			{
				return info == null || info.isNotice;
			}
		}

		public int state
		{
			get
			{
				return (info != null) ? info.state : 0;
			}
		}

		public bool isAttack
		{
			get
			{
				return info != null && info.isAttack;
			}
		}

		public bool femaleOnly
		{
			get
			{
				return info != null && info.femaleOnly;
			}
		}
	}
}
