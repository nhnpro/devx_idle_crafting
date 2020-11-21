using System;

namespace UniRx.Operators
{
	internal class SubscribeOnObservable<T> : OperatorObservableBase<T>
	{
		private readonly UniRx.IObservable<T> source;

		private readonly IScheduler scheduler;

		public SubscribeOnObservable(UniRx.IObservable<T> source, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
			SerialDisposable d = new SerialDisposable();
			d.Disposable = singleAssignmentDisposable;
			singleAssignmentDisposable.Disposable = scheduler.Schedule(delegate
			{
				d.Disposable = new ScheduledDisposable(scheduler, source.Subscribe(observer));
			});
			return d;
		}
	}
}
