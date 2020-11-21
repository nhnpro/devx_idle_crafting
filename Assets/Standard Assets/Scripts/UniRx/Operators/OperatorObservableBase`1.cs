using System;

namespace UniRx.Operators
{
	public abstract class OperatorObservableBase<T> : UniRx.IObservable<T>, IOptimizedObservable<T>
	{
		private readonly bool isRequiredSubscribeOnCurrentThread;

		public OperatorObservableBase(bool isRequiredSubscribeOnCurrentThread)
		{
			this.isRequiredSubscribeOnCurrentThread = isRequiredSubscribeOnCurrentThread;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return isRequiredSubscribeOnCurrentThread;
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			SingleAssignmentDisposable subscription = new SingleAssignmentDisposable();
			if (isRequiredSubscribeOnCurrentThread && Scheduler.IsCurrentThreadSchedulerScheduleRequired)
			{
				Scheduler.CurrentThread.Schedule(delegate
				{
					subscription.Disposable = SubscribeCore(observer, subscription);
				});
			}
			else
			{
				subscription.Disposable = SubscribeCore(observer, subscription);
			}
			return subscription;
		}

		protected abstract IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel);
	}
}
