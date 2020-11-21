using System;
using System.Diagnostics;
using UnityEngine;

public class Debug
{
	public class ProfilePair
	{
		public string Name;

		public float Time;
	}

	private static float sm_prevLogSince;

	private static AndroidJavaClass sm_androidLog = new AndroidJavaClass("android.util.Log");

	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition, string format, params object[] args)
	{
	}

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition)
	{
	}

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition, UnityEngine.Object context)
	{
	}

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition, object message)
	{
	}

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition, string message)
	{
	}

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
	}

	[Conditional("FP_DEVELOP")]
	public static void Assert(bool condition, string message, UnityEngine.Object context)
	{
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	public static void ClearDeveloperConsole()
	{
		UnityEngine.Debug.ClearDeveloperConsole();
	}

	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}

	public static void LogError(object msg)
	{
		UnityEngine.Debug.LogError(msg);
	}

	public static void LogError(object msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(msg, context);
	}

	public static void LogException(Exception e)
	{
		UnityEngine.Debug.LogException(e);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		LogInternal(string.Format(format, args), null);
	}

	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		LogInternal(string.Format(format, args), context);
	}

	public static void LogWarning(object msg)
	{
		UnityEngine.Debug.LogWarning(msg);
	}

	public static void LogWarning(object msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(msg, context);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(format, args);
	}

	[Conditional("FP_DEVELOP")]
	public static void Log(object msg)
	{
		LogInternal(msg, null);
	}

	[Conditional("FP_DEVELOP")]
	public static void Log(object msg, UnityEngine.Object context)
	{
		LogInternal(msg, context);
	}

	[Conditional("FP_DEVELOP")]
	public static void LogFormat(string format, params object[] args)
	{
		LogInternal(string.Format(format, args), null);
	}

	[Conditional("FP_DEVELOP")]
	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		LogInternal(string.Format(format, args), context);
	}

	private static void LogInternal(object msg, UnityEngine.Object context)
	{
		if (context == null)
		{
			UnityEngine.Debug.Log(msg, context);
		}
		else
		{
			UnityEngine.Debug.Log(msg, context);
		}
	}

	[Conditional("FP_DEVELOP")]
	public static void LogBlue(object msg)
	{
		LogInternal("<color=#00ffffff>" + msg + "</color>", null);
	}

	[Conditional("FP_DEVELOP")]
	public static void LogRed(object msg)
	{
		LogInternal("<color=#D64B4BFF>" + msg + "</color>", null);
	}

	[Conditional("FP_DEVELOP")]
	public static void LogGreen(object msg)
	{
		LogInternal("<color=green>" + msg + "</color>", null);
	}

	[Conditional("FP_DEVELOP")]
	public static void LogYellow(object msg)
	{
		LogInternal("<color=yellow>" + msg + "</color>", null);
	}

	[Conditional("FP_DEVELOP")]
	public static void LogSinceStartup(string msg)
	{
	}
}
