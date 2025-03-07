namespace ActionGame.Communication
{
	public class ChangeValueSelectInfo : ChangeValueAbstractInfo
	{
		private int[] aFavor;

		private int[] aLewdness;

		private int[] aAnger;

		public int this[int _idx, int _kind]
		{
			get
			{
				int result;
				switch (_kind)
				{
				case 0:
					result = aFavor[_idx];
					break;
				case 1:
					result = aLewdness[_idx];
					break;
				default:
					result = aAnger[_idx];
					break;
				}
				return result;
			}
			set
			{
				switch (_kind)
				{
				case 0:
					aFavor[_idx] = value;
					break;
				case 1:
					aLewdness[_idx] = value;
					break;
				default:
					aAnger[_idx] = value;
					break;
				}
			}
		}

		public int select { get; set; }

		public int success { get; set; }

		public bool isSuccess
		{
			get
			{
				return success == select;
			}
		}

		public override int favor
		{
			get
			{
				return aFavor.SafeGet(select);
			}
		}

		public override int lewdness
		{
			get
			{
				return aLewdness.SafeGet(select);
			}
		}

		public override int anger
		{
			get
			{
				return aAnger.SafeGet(select);
			}
		}

		public void Init(int _num)
		{
			aFavor = new int[_num];
			aLewdness = new int[_num];
			aAnger = new int[_num];
		}
	}
}
