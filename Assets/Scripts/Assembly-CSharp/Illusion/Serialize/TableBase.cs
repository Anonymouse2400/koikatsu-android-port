using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Illusion.Serialize
{
	[Serializable]
	public class TableBase<TKey, TValue, Type> where Type : KeyValuePair<TKey, TValue>, new()
	{
		[SerializeField]
		private List<Type> list = new List<Type>();

		public Dictionary<TKey, TValue> Table
		{
			get
			{
				return list.ToDictionary((Type v) => v.Key, (Type v) => v.Value);
			}
			set
			{
				list = value.Select((System.Collections.Generic.KeyValuePair<TKey, TValue> v) => new Type
				{
					Key = v.Key,
					Value = v.Value
				}).ToList();
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return Get(key);
			}
		}

		public TValue Get(TKey key)
		{
			Type val = list.Find((Type type) => type.Key.Equals(key));
			return (val == null) ? default(TValue) : val.Value;
		}

		public void Set(Type type)
		{
			Remove(type.Key);
			Add(type);
		}

		public void Set(TKey key, TValue value)
		{
			Set(new Type
			{
				Key = key,
				Value = value
			});
		}

		public bool Add(Type type)
		{
			bool flag = !list.Any((Type p) => p.Key.Equals(type.Key));
			if (flag)
			{
				list.Add(type);
			}
			return flag;
		}

		public bool Add(TKey key, TValue value)
		{
			return Add(new Type
			{
				Key = key,
				Value = value
			});
		}

		public bool Add(TKey key)
		{
			return Add(new Type
			{
				Key = key
			});
		}

		public int Remove(TKey key)
		{
			return list.RemoveAll((Type p) => p.Key.Equals(key));
		}

		public int Remove(TValue value)
		{
			return list.RemoveAll((Type p) => p.Value.Equals(value));
		}

		public void Clear()
		{
			list.Clear();
		}

		public IEnumerator<Type> GetEnumerator()
		{
			foreach (Type item in list)
			{
				yield return item;
			}
		}
	}
}
