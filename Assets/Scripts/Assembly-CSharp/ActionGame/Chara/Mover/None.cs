using System.Collections;

namespace ActionGame.Chara.Mover
{
	public class None : State
	{
		public None(Base move)
			: base(move)
		{
		}

		public override IEnumerator Initialize()
		{
			base.initialized = true;
			yield break;
		}

		public override void Release()
		{
		}

		public override void Update()
		{
		}
	}
}
