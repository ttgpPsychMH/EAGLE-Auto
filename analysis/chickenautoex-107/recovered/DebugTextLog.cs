using System;
using ClientTools.Log;

public static class DebugTextLog
{
	public static IDebugTextLog DebugLog;

	public static void LogException(Exception exception)
	{
		if (DebugLog != null)
		{
			DebugLog.LogException(exception);
		}
	}

	public static void LogError(params object[] messages)
	{
		if (DebugLog != null)
		{
			DebugLog.LogError(messages);
		}
	}

	public static void Log(params object[] messages)
	{
		if (DebugLog != null)
		{
			DebugLog.Log(messages);
		}
	}

	public static void LogStackMsg(string message)
	{
		if (DebugLog != null)
		{
			DebugLog.LogStackMsg(message);
		}
	}
}
