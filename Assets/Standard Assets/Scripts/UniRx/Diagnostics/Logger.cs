using System;
using UnityEngine;

namespace UniRx.Diagnostics
{
	public class Logger
	{
		private static bool isInitialized;

		private static bool isDebugBuild;

		protected readonly Action<LogEntry> logPublisher;

		public string Name
		{
			get;
			private set;
		}

		public Logger(string loggerName)
		{
			Name = loggerName;
			logPublisher = ObservableLogger.RegisterLogger(this);
		}

		public virtual void Debug(object message, UnityEngine.Object context = null)
		{
			if (!isInitialized)
			{
				isInitialized = true;
				isDebugBuild = UnityEngine.Debug.isDebugBuild;
			}
			if (isDebugBuild)
			{
				logPublisher(new LogEntry(message: (message == null) ? string.Empty : message.ToString(), loggerName: Name, logType: LogType.Log, timestamp: DateTime.Now, context: context));
			}
		}

		public virtual void DebugFormat(string format, params object[] args)
		{
			if (!isInitialized)
			{
				isInitialized = true;
				isDebugBuild = UnityEngine.Debug.isDebugBuild;
			}
			if (isDebugBuild)
			{
				logPublisher(new LogEntry(message: (format == null) ? string.Empty : string.Format(format, args), loggerName: Name, logType: LogType.Log, timestamp: DateTime.Now));
			}
		}

		public virtual void Log(object message, UnityEngine.Object context = null)
		{
			logPublisher(new LogEntry(message: (message == null) ? string.Empty : message.ToString(), loggerName: Name, logType: LogType.Log, timestamp: DateTime.Now, context: context));
		}

		public virtual void LogFormat(string format, params object[] args)
		{
			logPublisher(new LogEntry(message: (format == null) ? string.Empty : string.Format(format, args), loggerName: Name, logType: LogType.Log, timestamp: DateTime.Now));
		}

		public virtual void Warning(object message, UnityEngine.Object context = null)
		{
			logPublisher(new LogEntry(message: (message == null) ? string.Empty : message.ToString(), loggerName: Name, logType: LogType.Warning, timestamp: DateTime.Now, context: context));
		}

		public virtual void WarningFormat(string format, params object[] args)
		{
			logPublisher(new LogEntry(message: (format == null) ? string.Empty : string.Format(format, args), loggerName: Name, logType: LogType.Warning, timestamp: DateTime.Now));
		}

		public virtual void Error(object message, UnityEngine.Object context = null)
		{
			logPublisher(new LogEntry(message: (message == null) ? string.Empty : message.ToString(), loggerName: Name, logType: LogType.Error, timestamp: DateTime.Now, context: context));
		}

		public virtual void ErrorFormat(string format, params object[] args)
		{
			logPublisher(new LogEntry(message: (format == null) ? string.Empty : string.Format(format, args), loggerName: Name, logType: LogType.Error, timestamp: DateTime.Now));
		}

		public virtual void Exception(Exception exception, UnityEngine.Object context = null)
		{
			logPublisher(new LogEntry(message: (exception == null) ? string.Empty : exception.ToString(), loggerName: Name, logType: LogType.Exception, timestamp: DateTime.Now, context: context, exception: exception));
		}

		public virtual void Raw(LogEntry logEntry)
		{
			logPublisher(logEntry);
		}
	}
}
