using System;
using System.Reflection;

public static class WMG_Reflection
{
	public static bool IsValueType(Type type)
	{
		return type.IsValueType;
	}

	public static PropertyInfo GetProperty(Type type, string name)
	{
		return type.GetProperty(name);
	}

	public static MethodInfo GetMethod(Type type, string name)
	{
		return type.GetMethod(name);
	}

	public static bool IsEnum(Type type)
	{
		return type.IsEnum;
	}

	public static FieldInfo GetField(Type type, string name)
	{
		return type.GetField(name);
	}

	public static Delegate CreateDelegate(Type type, object target, MethodInfo method)
	{
		return Delegate.CreateDelegate(type, target, method);
	}

	public static bool IsAssignableFrom(Type first, Type second)
	{
		return first.IsAssignableFrom(second);
	}
}
