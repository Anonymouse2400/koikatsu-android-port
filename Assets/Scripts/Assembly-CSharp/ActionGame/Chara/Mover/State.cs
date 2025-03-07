using System;
using System.Collections;

namespace ActionGame.Chara.Mover
{
	[Serializable]
	public abstract class State
	{
		public bool initialized { get; protected set; }

		public State(Base mover)
		{
		}

		public abstract IEnumerator Initialize();

		public abstract void Release();

		public abstract void Update();
	}
}
