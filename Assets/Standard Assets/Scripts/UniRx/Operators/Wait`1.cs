using System;
using System.Threading;

namespace UniRx.Operators
{
	internal class Wait<T> : IObserver<T>
	{
		private static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

		private readonly UniRx.IObservable<T> source;

		private readonly TimeSpan timeout;

		private ManualResetEvent semaphore;

		private bool seenValue;

		private T value = default(T);

		private Exception ex;

		public Wait(UniRx.IObservable<T> source, TimeSpan timeout)
		{
			this.source = source;
			this.timeout = timeout;
		}

		public T Run()
		{
			semaphore = new ManualResetEvent(initialState: false);
			using (source.Subscribe(this))
			{
				if (!((!(timeout == InfiniteTimeSpan)) ? semaphore.WaitOne(timeout) : semaphore.WaitOne()))
				{
					throw new TimeoutException("OnCompleted not fired.");
				}
			}
			if (ex != null)
			{
				throw ex;
			}
			if (!seenValue)
			{
				throw new InvalidOperationException("No Elements.");
			}
			return value;
		}

		public void OnNext(T value)
		{
			seenValue = true;
			this.value = value;
		}

		public void OnError(Exception error)
		{
			ex = error;
			semaphore.Set();
		}

		public void OnCompleted()
		{
			semaphore.Set();
		}
	}
}
