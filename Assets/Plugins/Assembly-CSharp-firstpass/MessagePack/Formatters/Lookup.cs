using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MessagePack.Formatters
{
	internal class Lookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable, IEnumerable<IGrouping<TKey, TElement>>
	{
		private readonly Dictionary<TKey, IGrouping<TKey, TElement>> groupings;

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				return groupings[key];
			}
		}

		public int Count
		{
			get
			{
				return groupings.Count;
			}
		}

		public Lookup(Dictionary<TKey, IGrouping<TKey, TElement>> groupings)
		{
			this.groupings = groupings;
		}

		public bool Contains(TKey key)
		{
			return groupings.ContainsKey(key);
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			return groupings.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return groupings.Values.GetEnumerator();
		}
	}
}
