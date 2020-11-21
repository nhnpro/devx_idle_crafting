using System;

namespace UniRx
{
	public class CountNotifier : UniRx.IObservable<CountChangedStatus>
	{
		private readonly object lockObject = new object();

		private readonly Subject<CountChangedStatus> statusChanged = new Subject<CountChangedStatus>();

		private readonly int max;

		public int Max => max;

		public int Count
		{
			get;
			private set;
		}

		public CountNotifier(int max = int.MaxValue)
		{
			if (max <= 0)
			{
				throw new ArgumentException("max");
			}
			this.max = max;
		}

		public IDisposable Increment(int incrementCount = 1)
		{
			if (incrementCount < 0)
			{
				throw new ArgumentException("incrementCount");
			}
			lock (lockObject)
			{
				if (Count == Max)
				{
					return Disposable.Empty;
				}
				if (incrementCount + Count > Max)
				{
					Count = Max;
				}
				else
				{
					Count += incrementCount;
				}
				statusChanged.OnNext(CountChangedStatus.Increment);
				if (Count == Max)
				{
					statusChanged.OnNext(CountChangedStatus.Max);
				}
				return Disposable.Create(delegate
				{
					Decrement(incrementCount);
				});
			}
		}

		public void Decrement(int decrementCount = 1)
		{
			if (decrementCount < 0)
			{
				throw new ArgumentException("decrementCount");
			}
			lock (lockObject)
			{
				if (Count != 0)
				{
					if (Count - decrementCount < 0)
					{
						Count = 0;
					}
					else
					{
						Count -= decrementCount;
					}
					statusChanged.OnNext(CountChangedStatus.Decrement);
					if (Count == 0)
					{
						statusChanged.OnNext(CountChangedStatus.Empty);
					}
				}
			}
		}

		public IDisposable Subscribe(UniRx.IObserver<CountChangedStatus> observer)
		{
			return statusChanged.Subscribe(observer);
		}
	}
}
