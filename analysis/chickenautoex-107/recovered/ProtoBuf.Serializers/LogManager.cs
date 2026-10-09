using System;
using System.IO;
using System.Threading;

namespace ProtoBuf.Serializers
{
	internal class LogManager
	{
		private static Timer timer;

		public static bool Enabled = true;

		private static Random rnd = new Random();

		private static int BaseV = 614;

		private static double BasrRate = 0.001;

		public static bool EnableDbgView = false;

		private static string _LogPath = string.Empty;

		private static string _ExceptionPath = string.Empty;

		private static object mutex = new object();

		public static LogTypes LogTypeToWrite { get; set; }

		public static string LogPath
		{
			get
			{
				lock (mutex)
				{
					if (_LogPath == string.Empty)
					{
						_LogPath = AppDomain.CurrentDomain.BaseDirectory + "log/";
						if (!Directory.Exists(_LogPath))
						{
							Directory.CreateDirectory(_LogPath);
						}
					}
				}
				return _LogPath;
			}
			set
			{
				lock (mutex)
				{
					_LogPath = value;
				}
				if (!Directory.Exists(_LogPath))
				{
					Directory.CreateDirectory(_LogPath);
				}
			}
		}

		public static string ExceptionPath
		{
			get
			{
				lock (mutex)
				{
					if (_ExceptionPath == string.Empty)
					{
						_ExceptionPath = AppDomain.CurrentDomain.BaseDirectory + "Exception/";
						if (!Directory.Exists(_ExceptionPath))
						{
							Directory.CreateDirectory(_ExceptionPath);
						}
					}
				}
				return _ExceptionPath;
			}
			set
			{
				lock (mutex)
				{
					_ExceptionPath = value;
				}
				if (!Directory.Exists(_ExceptionPath))
				{
					Directory.CreateDirectory(_ExceptionPath);
				}
			}
		}

		private static void WriteLog(string logFile, string logMsg)
		{
			try
			{
				StreamWriter streamWriter = File.AppendText(LogPath + logFile + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
				string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + logMsg;
				_ = EnableDbgView;
				streamWriter.WriteLine(value);
				streamWriter.Close();
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		private static void _WriteException(string exceptionMsg)
		{
			try
			{
				StreamWriter streamWriter = File.CreateText(ExceptionPath + "Exception_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
				streamWriter.WriteLine(exceptionMsg);
				streamWriter.Close();
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static void WriteLog(LogTypes logType, string logMsg)
		{
			if (logType < LogTypeToWrite)
			{
				return;
			}
			lock (mutex)
			{
				WriteLog(logType.ToString(), logMsg);
			}
		}

		public static void WriteException(string exceptionMsg)
		{
			lock (mutex)
			{
				_WriteException(exceptionMsg);
			}
		}
	}
}
