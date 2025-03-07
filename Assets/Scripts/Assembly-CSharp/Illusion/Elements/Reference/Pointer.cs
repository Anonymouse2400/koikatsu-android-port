using System;

namespace Illusion.Elements.Reference
{
	public class Pointer<T>
	{
		private Func<T> get;

		private Action<T> set;

		public T value
		{
			get
			{
				return get.Call();
			}
			set
			{
				set.Call(value);
			}
		}

		public Pointer(Func<T> get, Action<T> set = null)
		{
			this.get = get;
			this.set = set;
		}
	}
}
