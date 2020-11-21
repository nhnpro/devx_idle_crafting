using System;

namespace UniRx
{
	public static class SubjectExtensions
	{
		private class AnonymousSubject<T, U> : ISubject<T, U>, IObserver<T>, UniRx.IObservable<U>
		{
			private readonly IObserver<T> observer;

			private readonly UniRx.IObservable<U> observable;

			public AnonymousSubject(UniRx.IObserver<T> observer, UniRx.IObservable<U> observable)
			{
				this.observer = observer;
				this.observable = observable;
			}

			public void OnCompleted()
			{
				observer.OnCompleted();
			}

			public void OnError(Exception error)
			{
				if (error == null)
				{
					throw new ArgumentNullException("error");
				}
				observer.OnError(error);
			}

			public void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public IDisposable Subscribe(UniRx.IObserver<U> observer)
			{
				if (observer == null)
				{
					throw new ArgumentNullException("observer");
				}
				return observable.Subscribe(observer);
			}
		}

		private class AnonymousSubject<T> : AnonymousSubject<T, T>, ISubject<T>, ISubject<T, T>, IObserver<T>, UniRx.IObservable<T>
		{
			public AnonymousSubject(UniRx.IObserver<T> observer, UniRx.IObservable<T> observable)
				: base(observer, observable)
			{
			}
		}

		public static ISubject<T> Synchronize<T>(this ISubject<T> subject)
		{
			return new AnonymousSubject<T>(ObserverExtensions.Synchronize(subject), subject);
		}

		public static ISubject<T> Synchronize<T>(this ISubject<T> subject, object gate)
		{
			return new AnonymousSubject<T>(ObserverExtensions.Synchronize(subject, gate), subject);
		}
	}
}
