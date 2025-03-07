using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace StrayTech
{
	public static class ReflectionExtension
	{
		public static IList<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider toScan, bool inherit = false) where TAttribute : Attribute
		{
			List<TAttribute> list = new List<TAttribute>();
			object[] customAttributes = toScan.GetCustomAttributes(typeof(TAttribute), inherit);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Attribute attribute = (Attribute)customAttributes[i];
				TAttribute val = attribute as TAttribute;
				if (val != null)
				{
					list.Add(val);
				}
			}
			return list;
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider toScan, bool inherit = false) where TAttribute : Attribute
		{
			IList<TAttribute> customAttributes = toScan.GetCustomAttributes<TAttribute>(inherit);
			if (customAttributes.Count == 0)
			{
				return (TAttribute)null;
			}
			if (customAttributes.Count > 1)
			{
			}
			return customAttributes[0];
		}

		public static IList<TAttribute> GetFieldAttributes<TAttribute>(this FieldInfo field, bool inherit = false) where TAttribute : Attribute
		{
			List<TAttribute> list = new List<TAttribute>();
			Attribute[] customAttributes = Attribute.GetCustomAttributes(field, typeof(TAttribute), inherit);
			foreach (Attribute attribute in customAttributes)
			{
				TAttribute val = attribute as TAttribute;
				if (val != null)
				{
					list.Add(val);
				}
			}
			return list;
		}

		public static TAttribute GetFieldAttribute<TAttribute>(this FieldInfo field, bool inherit = false) where TAttribute : Attribute
		{
			IList<TAttribute> fieldAttributes = field.GetFieldAttributes<TAttribute>(inherit);
			if (fieldAttributes.Count == 0)
			{
				return (TAttribute)null;
			}
			if (fieldAttributes.Count > 1)
			{
			}
			return fieldAttributes[0];
		}

		public static IEnumerable<FieldInfo> GetAllFields(this Type type)
		{
			if (type == null)
			{
				return Enumerable.Empty<FieldInfo>();
			}
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return type.GetFields(bindingAttr).Concat(type.BaseType.GetAllFields());
		}

		public static void CopyFromOther(this UnityEngine.Object source, UnityEngine.Object target)
		{
			if (source == null || target == null)
			{
				throw new UnityException("source and/or target cannot be null!");
			}
			if (source.GetType() != target.GetType())
			{
				throw new UnityException("The source and target components must be of the same (most-derived) type!");
			}
			FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(source, fieldInfo.GetValue(target));
			}
		}

		public static void CopyFromOther(this UnityEngine.Object source, UnityEngine.Object target, HashSet<Type> typesToSkip)
		{
			if (source == null || target == null || typesToSkip == null)
			{
				throw new UnityException("source and/or target ( or types to skip) cannot be null!");
			}
			if (source.GetType() != target.GetType())
			{
				throw new UnityException("The source and target components must be of the same (most-derived) type!");
			}
			FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!typesToSkip.Contains(fieldInfo.FieldType))
				{
					fieldInfo.SetValue(source, fieldInfo.GetValue(target));
				}
			}
		}
	}
}
