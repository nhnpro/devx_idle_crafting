using System;
using System.Collections.Generic;
using System.Threading;

namespace UniRx
{
	public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable, IOptimizedObservable<T>, UniRx.IObservable<T>
	{
		private class ReadOnlyReactivePropertyObserver : IObserver<T>
		{
			private readonly ReadOnlyReactiveProperty<T> parent;

			private int isStopped;

			public ReadOnlyReactivePropertyObserver(ReadOnlyReactiveProperty<T> parent)
			{
				this.parent = parent;
			}

			public void OnNext(T value)
			{
				if (parent.distinctUntilChanged && parent.canPublishValueOnSubscribe)
				{
					if (!parent.EqualityComparer.Equals(parent.value, value))
					{
						parent.value = value;
						parent.publisher?.OnNext(value);
					}
				}
				else
				{
					parent.value = value;
					parent.canPublishValueOnSubscribe = true;
					parent.publisher?.OnNext(value);
				}
			}

			public void OnError(Exception error)
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					parent.lastException = error;
					parent.publisher?.OnError(error);
					parent.Dispose();
				}
			}

			public void OnCompleted()
			{
				if (Interlocked.Increment(ref isStopped) == 1)
				{
					parent.isSourceCompleted = true;
					IDisposable sourceConnection = parent.sourceConnection;
					parent.sourceConnection = null;
					sourceConnection?.Dispose();
					Subject<T> publisher = parent.publisher;
					parent.publisher = null;
					if (publisher != null)
					{
						try
						{
							publisher.OnCompleted();
						}
						finally
						{
							publisher.Dispose();
						}
					}
				}
			}
		}

		private static readonly IEqualityComparer<T> defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();

		private readonly bool distinctUntilChanged = true;

		private bool canPublishValueOnSubscribe;

		private bool isDisposed;

		private Exception lastException;

		private T value = default(T);

		private Subject<T> publisher;

		private IDisposable sourceConnection;

		private bool isSourceCompleted;

		public T Value => value;

		public bool HasValue => canPublishValueOnSubscribe;

		protected virtual IEqualityComparer<T> EqualityComparer => defaultEqualityComparer;

		public ReadOnlyReactiveProperty(UniRx.IObservable<T> source)
		{
			sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
		}

		public ReadOnlyReactiveProperty(UniRx.IObservable<T> source, bool distinctUntilChanged)
		{
			this.distinctUntilChanged = distinctUntilChanged;
			sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
		}

		public ReadOnlyReactiveProperty(UniRx.IObservable<T> source, T initialValue)
		{
			value = initialValue;
			canPublishValueOnSubscribe = true;
			sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
		}

		public ReadOnlyReactiveProperty(UniRx.IObservable<T> source, T initialValue, bool distinctUntilChanged)
		{
			this.distinctUntilChanged = distinctUntilChanged;
			value = initialValue;
			canPublishValueOnSubscribe = true;
			sourceConnection = source.Subscribe(new ReadOnlyReactivePropertyObserver(this));
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			if (lastException != null)
			{
				observer.OnError(lastException);
				return Disposable.Empty;
			}
			if (isDisposed)
			{
				observer.OnCompleted();
				return Disposable.Empty;
			}
			if (isSourceCompleted)
			{
				if (canPublishValueOnSubscribe)
				{
					observer.OnNext(value);
					observer.OnCompleted();
					return Disposable.Empty;
				}
				observer.OnCompleted();
				return Disposable.Empty;
			}
			if (publisher == null)
			{
				publisher = new Subject<T>();
			}
			Subject<T> subject = publisher;
			if (subject != null)
			{
				IDisposable result = subject.Subscribe(observer);
				if (canPublishValueOnSubscribe)
				{
					observer.OnNext(value);
				}
				return result;
			}
			observer.OnCompleted();
			return Disposable.Empty;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
				IDisposable disposable = sourceConnection;
				if (disposable != null)
				{
					disposable.Dispose();
					sourceConnection = null;
				}
				Subject<T> subject = publisher;
				if (subject != null)
				{
					try
					{
						subject.OnCompleted();
					}
					finally
					{
						subject.Dispose();
						publisher = null;
					}
				}
			}
		}

		public override string ToString()
		{
			return (value != null) ? value.ToString() : "(null)";
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}
}
