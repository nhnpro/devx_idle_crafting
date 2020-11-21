using System;

namespace UniRx.Diagnostics
{
	public class ObservableLogger : UniRx.IObservable<LogEntry>
	{
		private static readonly Subject<LogEntry> logPublisher = new Subject<LogEntry>();

		public static readonly ObservableLogger Listener = new ObservableLogger();

		private ObservableLogger()
		{
		}

		public static Action<LogEntry> RegisterLogger(Logger logger)
		{
			if (logger.Name == null)
			{
				throw new ArgumentNullException("logger.Name is null");
			}
			return logPublisher.OnNext;
		}

		public IDisposable Subscribe(UniRx.IObserver<LogEntry> observer)
		{
			return logPublisher.Subscribe(observer);
		}
	}
}
