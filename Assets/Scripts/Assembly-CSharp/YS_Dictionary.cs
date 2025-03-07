using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class YS_Dictionary<TKey, TValue, TPair> where TPair : YS_KeyAndValue<TKey, TValue>, new()
{
	[SerializeField]
	protected List<TPair> list;

	protected Dictionary<TKey, TValue> table;

	public int Length
	{
		get
		{
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}
	}

	public YS_Dictionary()
	{
		list = new List<TPair>();
	}

	public Dictionary<TKey, TValue> GetTable()
	{
		if (table == null)
		{
			table = ConvertListToDictionary(list);
		}
		return table;
	}

	public TValue GetValue(TKey key)
	{
		if (GetTable().Keys.Contains(key))
		{
			return GetTable()[key];
		}
		return default(TValue);
	}

	public void SetValue(TKey key, TValue value)
	{
		if (GetTable().Keys.Contains(key))
		{
			table[key] = value;
		}
		else
		{
			table.Add(key, value);
		}
	}

	public void Reset()
	{
		table = new Dictionary<TKey, TValue>();
		list = new List<TPair>();
	}

	public void Apply()
	{
		list = ConvertDictionaryToList(table);
	}

	private static Dictionary<TKey, TValue> ConvertListToDictionary(List<TPair> list)
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		foreach (TPair item in list)
		{
			dictionary.Add(item.Key, item.Value);
		}
		return dictionary;
	}

	private static List<TPair> ConvertDictionaryToList(Dictionary<TKey, TValue> table)
	{
		List<TPair> list = new List<TPair>();
		if (table != null)
		{
			foreach (KeyValuePair<TKey, TValue> item in table)
			{
				list.Add(new TPair
				{
					Key = item.Key,
					Value = item.Value
				});
			}
		}
		return list;
	}
}
