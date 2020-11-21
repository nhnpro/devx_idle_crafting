using System;
using System.IO;
using System.Net;
using System.Threading;

namespace UniRx
{
	public static class WebRequestExtensions
	{
		private static UniRx.IObservable<TResult> AbortableDeferredAsyncRequest<TResult>(Func<AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end, WebRequest request)
		{
			return Observable.Create(delegate(UniRx.IObserver<TResult> observer)
			{
				int isCompleted = -1;
				IDisposable subscription = Observable.FromAsyncPattern(begin, delegate(IAsyncResult ar)
				{
					try
					{
						Interlocked.Increment(ref isCompleted);
						return end(ar);
					}
					catch (WebException ex)
					{
						if (ex.Status != WebExceptionStatus.RequestCanceled)
						{
							throw;
						}
						return default(TResult);
					}
				})().Subscribe(observer);
				return Disposable.Create(delegate
				{
					if (Interlocked.Increment(ref isCompleted) == 0)
					{
						subscription.Dispose();
						request.Abort();
					}
				});
			});
		}

		public static UniRx.IObservable<WebResponse> GetResponseAsObservable(this WebRequest request)
		{
			return AbortableDeferredAsyncRequest(request.BeginGetResponse, request.EndGetResponse, request);
		}

		public static UniRx.IObservable<HttpWebResponse> GetResponseAsObservable(this HttpWebRequest request)
		{
			HttpWebRequest httpWebRequest = request;
			return AbortableDeferredAsyncRequest(((WebRequest)httpWebRequest).BeginGetResponse, (IAsyncResult ar) => (HttpWebResponse)request.EndGetResponse(ar), request);
		}

		public static UniRx.IObservable<Stream> GetRequestStreamAsObservable(this WebRequest request)
		{
			return AbortableDeferredAsyncRequest(request.BeginGetRequestStream, request.EndGetRequestStream, request);
		}
	}
}
