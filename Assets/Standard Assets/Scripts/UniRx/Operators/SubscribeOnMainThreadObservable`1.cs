using System;

namespace UniRx.Operators
{
	internal class SubscribeOnMainThreadObservable<T> : OperatorObservableBase<T>
	{
		private readonly UniRx.IObservable<T> source;

		private readonly UniRx.IObservable<long> subscribeTrigger;

		public SubscribeOnMainThreadObservable(UniRx.IObservable<T> source, UniRx.IObservable<long> subscribeTrigger)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.subscribeTrigger = subscribeTrigger;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
			SerialDisposable serialDisposable = new SerialDisposable();
			serialDisposable.Disposable = singleAssignmentDisposable;
			singleAssignmentDisposable.Disposable = subscribeTrigger.SubscribeWithState3(observer, serialDisposable, source, delegate(long _, IObserver<T> o, SerialDisposable disp, UniRx.IObservable<T> s)
			{
				disp.Disposable = s.Subscribe(o);
			});
			return serialDisposable;
		}
	}
}
