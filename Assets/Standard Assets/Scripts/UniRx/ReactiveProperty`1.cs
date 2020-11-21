using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UniRx
{
	[Serializable]
	public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable, IOptimizedObservable<T>, IReadOnlyReactiveProperty<T>, UniRx.IObservable<T>
	{
		private class ReactivePropertyObserver : IObserver<T>
		{
			private readonly ReactiveProperty<T> parent;

			private int isStopped;

			public ReactivePropertyObserver(ReactiveProperty<T> parent)
			{
				this.parent = parent;
			}

			public void OnNext(T value)
			{
				parent.Value = value;
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
					IDisposable sourceConnection = parent.sourceConnection;
					parent.sourceConnection = null;
					sourceConnection?.Dispose();
				}
			}
		}

		private static readonly IEqualityComparer<T> defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();

		[NonSerialized]
		private bool canPublishValueOnSubscribe;

		[NonSerialized]
		private bool isDisposed;

		[SerializeField]
		private T value = default(T);

		[NonSerialized]
		private Subject<T> publisher;

		[NonSerialized]
		private IDisposable sourceConnection;

		[NonSerialized]
		private Exception lastException;

		protected virtual IEqualityComparer<T> EqualityComparer => defaultEqualityComparer;

		public T Value
		{
			get
			{
				return value;
			}
			set
			{
				if (!canPublishValueOnSubscribe)
				{
					canPublishValueOnSubscribe = true;
					SetValue(value);
					if (!isDisposed)
					{
						publisher?.OnNext(this.value);
					}
				}
				else if (!EqualityComparer.Equals(this.value, value))
				{
					SetValue(value);
					if (!isDisposed)
					{
						publisher?.OnNext(this.value);
					}
				}
			}
		}

		public bool HasValue => canPublishValueOnSubscribe;

		public ReactiveProperty()
			: this(default(T))
		{
		}

		public ReactiveProperty(T initialValue)
		{
			SetValue(initialValue);
			canPublishValueOnSubscribe = true;
		}

		public ReactiveProperty(UniRx.IObservable<T> source)
		{
			canPublishValueOnSubscribe = false;
			sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
		}

		public ReactiveProperty(UniRx.IObservable<T> source, T initialValue)
		{
			canPublishValueOnSubscribe = false;
			Value = initialValue;
			sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
		}

		public void UnpublishValue()
		{
			canPublishValueOnSubscribe = false;
		}

		protected virtual void SetValue(T value)
		{
			this.value = value;
		}

		public void SetValueAndForceNotify(T value)
		{
			SetValue(value);
			if (!isDisposed)
			{
				publisher?.OnNext(this.value);
			}
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
