using System;
using System.Collections.Generic;
using MessagePack.Formatters;
using UnityEngine;

namespace MessagePack.Unity
{
	internal static class UnityResolveryResolverGetFormatterHelper
	{
		private static readonly Dictionary<Type, object> formatterMap = new Dictionary<Type, object>
		{
			{
				typeof(Vector2),
				new Vector2Formatter()
			},
			{
				typeof(Vector3),
				new Vector3Formatter()
			},
			{
				typeof(Vector4),
				new Vector4Formatter()
			},
			{
				typeof(Quaternion),
				new QuaternionFormatter()
			},
			{
				typeof(Color),
				new ColorFormatter()
			},
			{
				typeof(Bounds),
				new BoundsFormatter()
			},
			{
				typeof(Rect),
				new RectFormatter()
			},
			{
				typeof(Vector2?),
				new StaticNullableFormatter<Vector2>(new Vector2Formatter())
			},
			{
				typeof(Vector3?),
				new StaticNullableFormatter<Vector3>(new Vector3Formatter())
			},
			{
				typeof(Vector4?),
				new StaticNullableFormatter<Vector4>(new Vector4Formatter())
			},
			{
				typeof(Quaternion?),
				new StaticNullableFormatter<Quaternion>(new QuaternionFormatter())
			},
			{
				typeof(Color?),
				new StaticNullableFormatter<Color>(new ColorFormatter())
			},
			{
				typeof(Bounds?),
				new StaticNullableFormatter<Bounds>(new BoundsFormatter())
			},
			{
				typeof(Rect?),
				new StaticNullableFormatter<Rect>(new RectFormatter())
			}
		};

		internal static object GetFormatter(Type t)
		{
			object value;
			if (formatterMap.TryGetValue(t, out value))
			{
				return value;
			}
			return null;
		}
	}
}
