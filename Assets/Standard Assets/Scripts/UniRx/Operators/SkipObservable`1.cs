using System;

namespace UniRx.Operators
{
	internal class SkipObservable<T> : OperatorObservableBase<T>
	{
		private class Skip : OperatorObserverBase<T, T>
		{
			private int remaining;

			public Skip(SkipObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				remaining = parent.count;
			}

			public override void OnNext(T value)
			{
				if (remaining <= 0)
				{
					observer.OnNext(value);
				}
				else
				{
					remaining--;
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

		private class Skip_ : OperatorObserverBase<T, T>
		{
			private readonly SkipObservable<T> parent;

			private volatile bool open;

			public Skip_(SkipObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.scheduler.Schedule(parent.duration, Tick);
				IDisposable disposable2 = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, disposable2);
			}

			private void Tick()
			{
				open = true;
			}

			public override void OnNext(T value)
			{
				if (open)
				{
					observer.OnNext(value);
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

		private readonly UniRx.IObservable<T> source;

		private readonly int count;

		private readonly TimeSpan duration;

		internal readonly IScheduler scheduler;

		public SkipObservable(UniRx.IObservable<T> source, int count)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.count = count;
		}

		public SkipObservable(UniRx.IObservable<T> source, TimeSpan duration, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.duration = duration;
			this.scheduler = scheduler;
		}

		public UniRx.IObservable<T> Combine(int count)
		{
			return new SkipObservable<T>(source, this.count + count);
		}

		public UniRx.IObservable<T> Combine(TimeSpan duration)
		{
			return (!(duration <= this.duration)) ? new SkipObservable<T>(source, duration, scheduler) : this;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (scheduler == null)
			{
				return source.Subscribe(new Skip(this, observer, cancel));
			}
			return new Skip_(this, observer, cancel).Run();
		}
	}
}
