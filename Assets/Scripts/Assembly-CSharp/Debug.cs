using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	private const string DEBUG_MODE = "GAME_DEBUG";

	[Conditional("GAME_DEBUG")]
	public static void Break()
	{
		if (IsEnable())
		{
			UnityEngine.Debug.Break();
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void Log(object message)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.Log(message);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void Log(object message, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.Log(message, context);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogFormat(string format, params object[] args)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogFormat(format, args);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogFormat(context, format, args);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogWarning(object message)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogWarning(message, context);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogWarningFormat(string format, params object[] args)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogWarningFormat(format, args);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogWarningFormat(context, format, args);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogError(object message)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogError(message);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogError(object message, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogError(message, context);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogErrorFormat(string format, params object[] args)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogErrorFormat(format, args);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogErrorFormat(context, format, args);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogException(Exception exception)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.LogException(exception, context);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void Assert(bool condition)
	{
		if (!IsEnable())
		{
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void Assert(bool condition, object message)
	{
		if (!IsEnable())
		{
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
		if (!IsEnable())
		{
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end, color);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end, color, duration);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
		}
	}

	[Conditional("GAME_DEBUG")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		if (IsEnable())
		{
			UnityEngine.Debug.DrawRay(start, dir, color);
		}
	}

	private static bool IsEnable()
	{
		return UnityEngine.Debug.isDebugBuild;
	}
}
