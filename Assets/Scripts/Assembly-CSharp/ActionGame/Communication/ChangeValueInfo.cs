namespace ActionGame.Communication
{
	public class ChangeValueInfo : ChangeValueAbstractInfo
	{
		private int[] array = new int[3];

		public int this[int _idx]
		{
			get
			{
				return array[_idx];
			}
			set
			{
				array[_idx] = value;
			}
		}

		public override int favor
		{
			get
			{
				return array[0];
			}
		}

		public override int lewdness
		{
			get
			{
				return array[1];
			}
		}

		public override int anger
		{
			get
			{
				return array[2];
			}
		}

		public ChangeValueInfo(int _favor = 0, int _lewdness = 0, int _anger = 0)
		{
			array[0] = _favor;
			array[1] = _lewdness;
			array[2] = _anger;
		}
	}
}
