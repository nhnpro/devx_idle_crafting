using System;

namespace UniRx.Operators
{
	internal class DelaySubscriptionObservable<T> : OperatorObservableBase<T>
	{
		private readonly UniRx.IObservable<T> source;

		private readonly IScheduler scheduler;

		private readonly TimeSpan? dueTimeT;

		private readonly DateTimeOffset? dueTimeD;

		public DelaySubscriptionObservable(UniRx.IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
			dueTimeT = dueTime;
		}

		public DelaySubscriptionObservable(UniRx.IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
			dueTimeD = dueTime;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (dueTimeT.HasValue)
			{
				MultipleAssignmentDisposable d = new MultipleAssignmentDisposable();
				TimeSpan dueTime = Scheduler.Normalize(dueTimeT.Value);
				d.Disposable = scheduler.Schedule(dueTime, delegate
				{
					d.Disposable = source.Subscribe(observer);
				});
				return d;
			}
			MultipleAssignmentDisposable d2 = new MultipleAssignmentDisposable();
			d2.Disposable = scheduler.Schedule(dueTimeD.Value, (Action)delegate
			{
				d2.Disposable = source.Subscribe(observer);
			});
			return d2;
		}
	}
}
