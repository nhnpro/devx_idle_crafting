using System;
using UniRx.InternalUtil;

namespace UniRx
{
	public sealed class BehaviorSubject<T> : ISubject<T>, IDisposable, IOptimizedObservable<T>, ISubject<T, T>, IObserver<T>, UniRx.IObservable<T>
	{
		private class Subscription : IDisposable
		{
			private readonly object gate = new object();

			private BehaviorSubject<T> parent;

			private IObserver<T> unsubscribeTarget;

			public Subscription(BehaviorSubject<T> parent, IObserver<T> unsubscribeTarget)
			{
				this.parent = parent;
				this.unsubscribeTarget = unsubscribeTarget;
			}

			public void Dispose()
			{
				lock (gate)
				{
					if (parent != null)
					{
						lock (parent.observerLock)
						{
							ListObserver<T> listObserver = parent.outObserver as ListObserver<T>;
							if (listObserver != null)
							{
								parent.outObserver = listObserver.Remove(unsubscribeTarget);
							}
							else
							{
								parent.outObserver = EmptyObserver<T>.Instance;
							}
							unsubscribeTarget = null;
							parent = null;
						}
					}
				}
			}
		}

		private object observerLock = new object();

		private bool isStopped;

		private bool isDisposed;

		private T lastValue;

		private Exception lastError;

		private IObserver<T> outObserver = EmptyObserver<T>.Instance;

		public T Value
		{
			get
			{
				ThrowIfDisposed();
				if (lastError != null)
				{
					throw lastError;
				}
				return lastValue;
			}
		}

		public bool HasObservers => !(outObserver is EmptyObserver<T>) && !isStopped && !isDisposed;

		public BehaviorSubject(T defaultValue)
		{
			lastValue = defaultValue;
		}

		public void OnCompleted()
		{
			UniRx.IObserver<T> observer;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (isStopped)
				{
					return;
				}
				observer = outObserver;
				outObserver = EmptyObserver<T>.Instance;
				isStopped = true;
			}
			observer.OnCompleted();
		}

		public void OnError(Exception error)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}
			UniRx.IObserver<T> observer;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (isStopped)
				{
					return;
				}
				observer = outObserver;
				outObserver = EmptyObserver<T>.Instance;
				isStopped = true;
				lastError = error;
			}
			observer.OnError(error);
		}

		public void OnNext(T value)
		{
			UniRx.IObserver<T> observer;
			lock (observerLock)
			{
				if (isStopped)
				{
					return;
				}
				lastValue = value;
				observer = outObserver;
			}
			observer.OnNext(value);
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}
			Exception ex = null;
			T value = default(T);
			Subscription subscription = null;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (!isStopped)
				{
					ListObserver<T> listObserver = outObserver as ListObserver<T>;
					if (listObserver != null)
					{
						outObserver = listObserver.Add(observer);
					}
					else
					{
						UniRx.IObserver<T> observer2 = outObserver;
						if (observer2 is EmptyObserver<T>)
						{
							outObserver = observer;
						}
						else
						{
							outObserver = new ListObserver<T>(new ImmutableList<IObserver<T>>(new IObserver<T>[2]
							{
								observer2,
								observer
							}));
						}
					}
					value = lastValue;
					subscription = new Subscription(this, observer);
				}
				else
				{
					ex = lastError;
				}
			}
			if (subscription != null)
			{
				observer.OnNext(value);
				return subscription;
			}
			if (ex != null)
			{
				observer.OnError(ex);
			}
			else
			{
				observer.OnCompleted();
			}
			return Disposable.Empty;
		}

		public void Dispose()
		{
			lock (observerLock)
			{
				isDisposed = true;
				outObserver = DisposedObserver<T>.Instance;
				lastError = null;
				lastValue = default(T);
			}
		}

		private void ThrowIfDisposed()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException(string.Empty);
			}
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}
}
