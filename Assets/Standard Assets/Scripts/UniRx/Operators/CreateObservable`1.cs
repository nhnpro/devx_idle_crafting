using System;

namespace UniRx.Operators
{
	internal class CreateObservable<T> : OperatorObservableBase<T>
	{
		private class Create : OperatorObserverBase<T, T>
		{
			public Create(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					observer.OnError(error);
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnCompleted()
			{
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly Func<IObserver<T>, IDisposable> subscribe;

		public CreateObservable(Func<IObserver<T>, IDisposable> subscribe)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.subscribe = subscribe;
		}

		public CreateObservable(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
			: base(isRequiredSubscribeOnCurrentThread)
		{
			this.subscribe = subscribe;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new Create(observer, cancel);
			return subscribe(observer) ?? Disposable.Empty;
		}
	}
	internal class CreateObservable<T, TState> : OperatorObservableBase<T>
	{
		private class Create : OperatorObserverBase<T, T>
		{
			public Create(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					observer.OnError(error);
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnCompleted()
			{
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}

		private readonly TState state;

		private readonly Func<TState, IObserver<T>, IDisposable> subscribe;

		public CreateObservable(TState state, Func<TState, IObserver<T>, IDisposable> subscribe)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.state = state;
			this.subscribe = subscribe;
		}

		public CreateObservable(TState state, Func<TState, IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
			: base(isRequiredSubscribeOnCurrentThread)
		{
			this.state = state;
			this.subscribe = subscribe;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			observer = new Create(observer, cancel);
			return subscribe(state, observer) ?? Disposable.Empty;
		}
	}
}
