using System;

namespace Illusion.Serialize
{
	[Serializable]
	public class KeyValuePair<TKey, TValue>
	{
		public TKey Key;

		public TValue Value;
	}
}
