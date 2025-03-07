using System;
using System.Collections.Generic;
using System.Linq;

namespace Illusion.Elements.Reference
{
	public class AutoIndexer<T>
	{
		protected T initializeValue;

		protected Func<T> initializeValueFunc;

		protected Dictionary<string, T> dic = new Dictionary<string, T>();

		public T this[int index]
		{
			get
			{
				return this[index.ToString()];
			}
			set
			{
				this[index.ToString()] = value;
			}
		}

		public virtual T this[string key]
		{
			get
			{
				T value;
				if (!dic.TryGetValue(key, out value))
				{
					T val;
					if (initializeValueFunc == null)
					{
						val = initializeValue;
						dic[key] = val;
						return val;
					}
					val = initializeValueFunc();
					dic[key] = val;
					return val;
				}
				return value;
			}
			set
			{
				dic[key] = value;
			}
		}

		public AutoIndexer()
		{
			initializeValue = default(T);
		}

		public AutoIndexer(T initializeValue)
		{
			this.initializeValue = initializeValue;
		}

		public AutoIndexer(Func<T> initializeValueFunc)
		{
			this.initializeValueFunc = initializeValueFunc;
		}

		public void Clear()
		{
			dic.Clear();
		}

		public Dictionary<string, T> ToStringDictionary()
		{
			return dic;
		}

		public Dictionary<int, T> ToIntDictionary()
		{
			return dic.ToDictionary((KeyValuePair<string, T> v) => int.Parse(v.Key), (KeyValuePair<string, T> v) => v.Value);
		}
	}
}
