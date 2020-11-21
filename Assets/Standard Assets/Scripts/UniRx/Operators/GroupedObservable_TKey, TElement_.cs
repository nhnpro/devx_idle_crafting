using System;

namespace UniRx.Operators
{
	internal class GroupedObservable<TKey, TElement> : IGroupedObservable<TKey, TElement>, UniRx.IObservable<TElement>
	{
		private readonly TKey key;

		private readonly UniRx.IObservable<TElement> subject;

		private readonly RefCountDisposable refCount;

		public TKey Key => key;

		public GroupedObservable(TKey key, ISubject<TElement> subject, RefCountDisposable refCount)
		{
			this.key = key;
			this.subject = subject;
			this.refCount = refCount;
		}

		public IDisposable Subscribe(UniRx.IObserver<TElement> observer)
		{
			IDisposable disposable = refCount.GetDisposable();
			IDisposable disposable2 = subject.Subscribe(observer);
			return StableCompositeDisposable.Create(disposable, disposable2);
		}
	}
}
