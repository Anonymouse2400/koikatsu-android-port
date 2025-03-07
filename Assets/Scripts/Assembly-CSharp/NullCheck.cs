using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class NullCheck
{
	public static bool IsDefault<T>(this T value) where T : struct
	{
		return value.Equals(default(T));
	}

	public static bool IsNull<T, TU>(this KeyValuePair<T, TU> pair)
	{
		return pair.Equals(default(KeyValuePair<T, TU>));
	}

	public static T GetCache<T>(this object _, ref T ret, Func<T> get)
	{
		return (ret == null) ? (ret = get()) : ret;
	}

	public static T GetCacheObject<T>(this object _, ref T ret, Func<T> get) where T : UnityEngine.Object
	{
		return (!(ret != null)) ? (ret = get()) : ret;
	}

	public static T GetComponentCache<T>(this Component component, ref T ret) where T : Component
	{
		return component.GetCacheObject(ref ret, () => component.GetComponent<T>());
	}

	public static T GetComponentCache<T>(this GameObject gameObject, ref T ret) where T : Component
	{
		return gameObject.GetCacheObject(ref ret, () => gameObject.GetComponent<T>());
	}

	public static T GetOrAddComponent<T>(this Component component) where T : Component
	{
		return (!(component == null)) ? component.gameObject.GetOrAddComponent<T>() : ((T)null);
	}

	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		if (gameObject == null)
		{
			return (T)null;
		}
		T val = gameObject.GetComponent<T>();
		if (val == null)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}

	public static bool IsNullOrWhiteSpace(this string self)
	{
		return self == null || self.Trim() == string.Empty;
	}

	public static bool IsNullOrEmpty(this string self)
	{
		return string.IsNullOrEmpty(self);
	}

	public static bool IsNullOrEmpty(this string[] args, int index)
	{
		bool ret = false;
		args.SafeGet(index).SafeProc(delegate(string s)
		{
			ret = !s.IsNullOrEmpty();
		});
		return !ret;
	}

	public static bool IsNullOrEmpty(this List<string> args, int index)
	{
		bool ret = false;
		args.SafeGet(index).SafeProc(delegate(string s)
		{
			ret = !s.IsNullOrEmpty();
		});
		return !ret;
	}

	public static bool IsNullOrEmpty<T>(this IList<T> self)
	{
		return self == null || self.Count == 0;
	}

	public static bool IsNullOrEmpty<T>(this List<T> self)
	{
		return self == null || self.Count == 0;
	}

	public static bool IsNullOrEmpty(this MulticastDelegate self)
	{
		return (object)self == null || self.GetInvocationList() == null || self.GetInvocationList().Length == 0;
	}

	public static bool IsNullOrEmpty(this UnityEvent self)
	{
		return self == null || self.GetPersistentEventCount() == 0;
	}

	public static bool IsNullOrEmpty(this UnityEvent self, int target)
	{
		if (self.IsNullOrEmpty())
		{
			return true;
		}
		return self.GetPersistentTarget(target) == null || self.GetPersistentMethodName(target).IsNullOrEmpty();
	}

	public static T SafeGet<T>(this T[] array, int index)
	{
		if (array == null)
		{
			return default(T);
		}
		return ((uint)index >= array.Length) ? default(T) : array[index];
	}

	public static bool SafeProc<T>(this T[] array, int index, Action<T> act)
	{
		return array.SafeGet(index).SafeProc(act);
	}

	public static T SafeGet<T>(this List<T> list, int index)
	{
		if (list == null)
		{
			return default(T);
		}
		return ((uint)index >= list.Count) ? default(T) : list[index];
	}

	public static bool SafeProc<T>(this List<T> list, int index, Action<T> act)
	{
		return list.SafeGet(index).SafeProc(act);
	}

	public static bool SafeProc(this string[] args, int index, Action<string> act)
	{
		if (args.IsNullOrEmpty(index))
		{
			return false;
		}
		act.Call(args[index]);
		return true;
	}

	public static bool SafeProc(this List<string> args, int index, Action<string> act)
	{
		if (args.IsNullOrEmpty(index))
		{
			return false;
		}
		act.Call(args[index]);
		return true;
	}

	public static bool SafeProc<T>(this T self, Action<T> act)
	{
		bool flag = self != null;
		if (flag)
		{
			act.Call(self);
		}
		return flag;
	}

	public static bool SafeProcObject<T>(this T self, Action<T> act) where T : UnityEngine.Object
	{
		bool flag = self != null;
		if (flag)
		{
			act.Call(self);
		}
		return flag;
	}

	public static void Call(this Action action)
	{
		if (action != null)
		{
			action();
		}
	}

	public static void Call<T>(this Action<T> action, T arg)
	{
		if (action != null)
		{
			action(arg);
		}
	}

	public static void Call<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		if (action != null)
		{
			action(arg1, arg2);
		}
	}

	public static void Call<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		if (action != null)
		{
			action(arg1, arg2, arg3);
		}
	}

	public static TResult Call<TResult>(this Func<TResult> func, TResult result = default(TResult))
	{
		return (func == null) ? result : func();
	}

	public static TResult Call<T, TResult>(this Func<T, TResult> func, T arg, TResult result = default(TResult))
	{
		return (func == null) ? result : func(arg);
	}

	public static TResult Call<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, TResult result = default(TResult))
	{
		return (func == null) ? result : func(arg1, arg2);
	}

	public static TResult Call<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, TResult result = default(TResult))
	{
		return (func == null) ? result : func(arg1, arg2, arg3);
	}

	public static bool Proc<T>(this T self, Func<T, bool> conditional, Action<T> act)
	{
		bool flag = conditional(self);
		if (flag)
		{
			act.Call(self);
		}
		return flag;
	}

	public static bool Proc<T>(this T self, Func<T, bool> conditional, Action<T> actTrue, Action<T> actFalse)
	{
		bool flag = conditional(self);
		self.Proc((T _) => true, (!flag) ? actFalse : actTrue);
		return flag;
	}
}
