using System;
using System.Collections;

namespace UniRx.Operators
{
	internal class FromCoroutineObservable<T> : OperatorObservableBase<T>
	{
		private class FromCoroutine : OperatorObserverBase<T, T>
		{
			public FromCoroutine(UniRx.IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
			}

			public override void OnNext(T value)
			{
				try
				{
					observer.OnNext(value);
				}
				catch
				{
					Dispose();
					throw;
				}
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

		private readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;

		public FromCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.coroutine = coroutine;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			FromCoroutine arg = new FromCoroutine(observer, cancel);
			BooleanDisposable booleanDisposable = new BooleanDisposable();
			CancellationToken arg2 = new CancellationToken(booleanDisposable);
			MainThreadDispatcher.SendStartCoroutine(coroutine(arg, arg2));
			return booleanDisposable;
		}
	}
}
