using System;

namespace UniRx.Diagnostics
{
	public static class LogEntryExtensions
	{
		public static IDisposable LogToUnityDebug(this UniRx.IObservable<LogEntry> source)
		{
			return source.Subscribe(new UnityDebugSink());
		}
	}
}
