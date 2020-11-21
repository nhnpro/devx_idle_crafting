using System;

namespace UniRx.Operators
{
	internal class NeverObservable<T> : OperatorObservableBase<T>
	{
		public NeverObservable()
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return Disposable.Empty;
		}
	}
}
