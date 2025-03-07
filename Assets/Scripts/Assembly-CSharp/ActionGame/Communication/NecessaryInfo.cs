namespace ActionGame.Communication
{
	public class NecessaryInfo
	{
		public int mapNo { get; private set; }

		public string[] placeNames { get; private set; }

		public Cycle.Type cycleType { get; private set; }

		public bool isOtherPeople { get; private set; }

		public bool isChase { get; private set; }

		public bool isChasePossible { get; private set; }

		public bool isHPossible { get; private set; }

		public bool isNotice { get; private set; }

		public int state { get; private set; }

		public bool isAttack { get; private set; }

		public bool isHAttack { get; private set; }

		public bool exposure { get; private set; }

		public bool femaleOnly { get; private set; }

		public bool introSkip { get; private set; }

		public NecessaryInfo(int _mapNo, Cycle.Type _cycleType, bool _isOtherPeople, bool _isChase, bool _isChasePossible, bool _isHPossible, bool _isNotice, int _state, bool _isAttack, bool _isHAttack, bool _exposure, bool _femaleOnly, bool _introSkip, string[] _placeNames)
		{
			mapNo = _mapNo;
			placeNames = _placeNames;
			cycleType = _cycleType;
			isOtherPeople = _isOtherPeople;
			isChase = _isChase;
			isChasePossible = _isChasePossible;
			isHPossible = _isHPossible;
			isNotice = _isNotice;
			state = _state;
			isAttack = _isAttack;
			isHAttack = _isHAttack;
			exposure = _exposure;
			femaleOnly = _femaleOnly;
			introSkip = _introSkip;
		}
	}
}
