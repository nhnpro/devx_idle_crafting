using System;
using System.Collections;

namespace UniRx.Operators
{
	internal class FromMicroCoroutineObservable<T> : OperatorObservableBase<T>
	{
		private class FromMicroCoroutine : OperatorObserverBase<T, T>
		{
			public FromMicroCoroutine(UniRx.IObserver<T> observer, IDisposable cancel)
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

		private readonly FrameCountType frameCountType;

		public FromMicroCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine, FrameCountType frameCountType)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.coroutine = coroutine;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			FromMicroCoroutine arg = new FromMicroCoroutine(observer, cancel);
			BooleanDisposable booleanDisposable = new BooleanDisposable();
			CancellationToken arg2 = new CancellationToken(booleanDisposable);
			switch (frameCountType)
			{
			case FrameCountType.Update:
				MainThreadDispatcher.StartUpdateMicroCoroutine(coroutine(arg, arg2));
				break;
			case FrameCountType.FixedUpdate:
				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(coroutine(arg, arg2));
				break;
			case FrameCountType.EndOfFrame:
				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(coroutine(arg, arg2));
				break;
			default:
				throw new ArgumentException("Invalid FrameCountType:" + frameCountType);
			}
			return booleanDisposable;
		}
	}
}
