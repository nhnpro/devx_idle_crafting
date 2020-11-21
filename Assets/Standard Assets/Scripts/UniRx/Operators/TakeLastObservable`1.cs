using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class TakeLastObservable<T> : OperatorObservableBase<T>
	{
		private class TakeLast : OperatorObserverBase<T, T>
		{
			private readonly TakeLastObservable<T> parent;

			private readonly Queue<T> q;

			public TakeLast(TakeLastObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				q = new Queue<T>();
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				q.Enqueue(value);
				if (q.Count > parent.count)
				{
					q.Dequeue();
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
				foreach (T item in q)
				{
					observer.OnNext(item);
				}
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

		private class TakeLast_ : OperatorObserverBase<T, T>
		{
			private DateTimeOffset startTime;

			private readonly TakeLastObservable<T> parent;

			private readonly Queue<TimeInterval<T>> q;

			public TakeLast_(TakeLastObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				q = new Queue<TimeInterval<T>>();
			}

			public IDisposable Run()
			{
				startTime = parent.scheduler.Now;
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				DateTimeOffset now = parent.scheduler.Now;
				TimeSpan timeSpan = now - startTime;
				q.Enqueue(new TimeInterval<T>(value, timeSpan));
				Trim(timeSpan);
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
				DateTimeOffset now = parent.scheduler.Now;
				TimeSpan now2 = now - startTime;
				Trim(now2);
				foreach (TimeInterval<T> item in q)
				{
					observer.OnNext(item.Value);
				}
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}

			private void Trim(TimeSpan now)
			{
				while (q.Count > 0 && now - q.Peek().Interval >= parent.duration)
				{
					q.Dequeue();
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly int count;

		private readonly TimeSpan duration;

		private readonly IScheduler scheduler;

		public TakeLastObservable(UniRx.IObservable<T> source, int count)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.count = count;
		}

		public TakeLastObservable(UniRx.IObservable<T> source, TimeSpan duration, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.duration = duration;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (scheduler == null)
			{
				return new TakeLast(this, observer, cancel).Run();
			}
			return new TakeLast_(this, observer, cancel).Run();
		}
	}
}
