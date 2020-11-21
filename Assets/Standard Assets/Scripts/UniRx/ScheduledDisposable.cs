using System;
using System.Threading;

namespace UniRx
{
	public sealed class ScheduledDisposable : ICancelable, IDisposable
	{
		private readonly IScheduler scheduler;

		private volatile IDisposable disposable;

		private int isDisposed;

		public IScheduler Scheduler => scheduler;

		public IDisposable Disposable => disposable;

		public bool IsDisposed => isDisposed != 0;

		public ScheduledDisposable(IScheduler scheduler, IDisposable disposable)
		{
			this.scheduler = scheduler;
			this.disposable = disposable;
		}

		public void Dispose()
		{
			Scheduler.Schedule(DisposeInner);
		}

		private void DisposeInner()
		{
			if (Interlocked.Increment(ref isDisposed) == 1)
			{
				disposable.Dispose();
			}
		}
	}
}
