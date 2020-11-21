using System;
using System.Collections;
using UnityEngine;

namespace UniRx
{
	public static class AsyncOperationExtensions
	{
		public static UniRx.IObservable<AsyncOperation> AsObservable(this AsyncOperation asyncOperation, IProgress<float> progress = null)
		{
			return Observable.FromCoroutine((UniRx.IObserver<AsyncOperation> observer, CancellationToken cancellation) => AsObservableCore(asyncOperation, observer, progress, cancellation));
		}

		public static UniRx.IObservable<T> AsAsyncOperationObservable<T>(this T asyncOperation, IProgress<float> progress = null) where T : AsyncOperation
		{
			return Observable.FromCoroutine((UniRx.IObserver<T> observer, CancellationToken cancellation) => AsObservableCore(asyncOperation, observer, progress, cancellation));
		}

		private static IEnumerator AsObservableCore<T>(T asyncOperation, IObserver<T> observer, IProgress<float> reportProgress, CancellationToken cancel) where T : AsyncOperation
		{
			if (reportProgress != null)
			{
				while (!asyncOperation.isDone && !cancel.IsCancellationRequested)
				{
					try
					{
						reportProgress.Report(asyncOperation.progress);
					}
					catch (Exception error)
					{
						observer.OnError(error);
						yield break;
					}
					yield return null;
				}
			}
			else if (!asyncOperation.isDone)
			{
				yield return asyncOperation;
			}
			if (!cancel.IsCancellationRequested)
			{
				if (reportProgress != null)
				{
					try
					{
						reportProgress.Report(asyncOperation.progress);
					}
					catch (Exception error2)
					{
						observer.OnError(error2);
						yield break;
					}
				}
				observer.OnNext(asyncOperation);
				observer.OnCompleted();
			}
		}
	}
}
