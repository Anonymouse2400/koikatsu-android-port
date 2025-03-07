using System.Collections.Generic;

namespace ActionGame
{
	public static class ListPool<T>
	{
		private static readonly ObjectPool<List<T>> _listPool = new ObjectPool<List<T>>(null, delegate(List<T> x)
		{
			x.Clear();
		});

		public static List<T> Get()
		{
			return _listPool.Get();
		}

		public static void Release(List<T> toRelease)
		{
			_listPool.Release(toRelease);
		}
	}
}
