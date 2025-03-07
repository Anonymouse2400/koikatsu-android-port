using System.Collections.Generic;
using System.Linq;
using Localize.Translate;
using UnityEngine;

public static class LocalizeExtensions
{
	public static Dictionary<int, Dictionary<int, Data.Param>> LoadTranslater(this GameObject gameObject, Localize.Translate.Manager.SCENE_ID sceneID)
	{
		return Localize.Translate.Manager.LoadScene((int)sceneID, gameObject);
	}

	public static Dictionary<int, Dictionary<int, Data.Param>> LoadTranslater(this GameObject gameObject, int sceneID)
	{
		return Localize.Translate.Manager.LoadScene(sceneID, gameObject);
	}

	public static IEnumerable<Data.Param> FindTags(this IEnumerable<Data.Param> self, string tag, bool isDesc = false)
	{
		IEnumerable<Data.Param> source = self.Where((Data.Param p) => p.tag == tag);
		return isDesc ? source.OrderByDescending((Data.Param p) => p.ID) : source.OrderBy((Data.Param p) => p.ID);
	}

	public static Data.Param FindTag(this IEnumerable<Data.Param> self, string tag, bool isDesc = false)
	{
		IEnumerable<Data.Param> source = self.Where((Data.Param p) => p.tag == tag);
		return (isDesc ? source.OrderByDescending((Data.Param p) => p.ID) : source.OrderBy((Data.Param p) => p.ID)).FirstOrDefault();
	}

	public static string FindTagText(this IEnumerable<Data.Param> self, string tag, bool isDesc = false, bool isEmpty = false)
	{
		Data.Param param = self.FindTag(tag, isDesc);
		if (param == null)
		{
			return null;
		}
		if (isEmpty)
		{
			return param.text;
		}
		return (!(param.text == string.Empty)) ? param.text : null;
	}

	public static string[] ToArray(this IEnumerable<Data.Param> self, string tag, bool isDesc = false)
	{
		return (from p in self.FindTags(tag, isDesc)
			select p.text).ToArray();
	}

	public static Dictionary<int, Data.Param> Get(this Dictionary<int, Dictionary<int, Data.Param>> self, int key)
	{
		return self.SafeGet(key) ?? new Dictionary<int, Data.Param>();
	}

	public static Dictionary<int, Data.Param> SafeGet(this Dictionary<int, Dictionary<int, Data.Param>> self, int key)
	{
		Dictionary<int, Dictionary<int, Data.Param>> dictionary = self ?? new Dictionary<int, Dictionary<int, Data.Param>>();
		Dictionary<int, Data.Param> value;
		dictionary.TryGetValue(key, out value);
		return value;
	}

	public static string SafeGetText(this Dictionary<int, Data.Param> self, int key)
	{
		Data.Param param = self.SafeGet(key);
		if (param == null)
		{
			return null;
		}
		return (!param.text.IsNullOrEmpty()) ? param.text : null;
	}

	public static Data.Param Get(this Dictionary<int, Data.Param> self, int key)
	{
		return self.SafeGet(key) ?? new Data.Param();
	}

	public static Data.Param SafeGet(this Dictionary<int, Data.Param> self, int key)
	{
		Dictionary<int, Data.Param> dictionary = self ?? new Dictionary<int, Data.Param>();
		Data.Param value;
		dictionary.TryGetValue(key, out value);
		return value;
	}
}
