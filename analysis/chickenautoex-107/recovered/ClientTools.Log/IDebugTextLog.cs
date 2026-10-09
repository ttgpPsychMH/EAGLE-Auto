using System;

namespace ClientTools.Log
{
	public interface IDebugTextLog
	{
		void Log(params object[] messages);

		void LogWarning(params object[] messages);

		void LogError(params object[] messages);

		void LogException(Exception exception);

		void LogStackMsg(string message);
	}
}
