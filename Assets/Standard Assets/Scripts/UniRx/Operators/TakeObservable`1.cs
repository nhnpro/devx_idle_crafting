using System;

namespace UniRx.Operators
{
	internal class TakeObservable<T> : OperatorObservableBase<T>
	{
		private class Take : OperatorObserverBase<T, T>
		{
			private int rest;

			public Take(TakeObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				rest = parent.count;
			}

			public override void OnNext(T value)
			{
				if (rest > 0)
				{
					rest--;
					observer.OnNext(value);
					if (rest == 0)
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

		private class Take_ : OperatorObserverBase<T, T>
		{
			private readonly TakeObservable<T> parent;

			private readonly object gate = new object();

			public Take_(TakeObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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
				lock (gate)
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

			public override void OnNext(T value)
			{
				lock (gate)
				{
					observer.OnNext(value);
				}
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				lock (gate)
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
		}

		private readonly UniRx.IObservable<T> source;

		private readonly int count;

		private readonly TimeSpan duration;

		internal readonly IScheduler scheduler;

		public TakeObservable(UniRx.IObservable<T> source, int count)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.count = count;
		}

		public TakeObservable(UniRx.IObservable<T> source, TimeSpan duration, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.duration = duration;
			this.scheduler = scheduler;
		}

		public UniRx.IObservable<T> Combine(int count)
		{
			return (this.count > count) ? new TakeObservable<T>(source, count) : this;
		}

		public UniRx.IObservable<T> Combine(TimeSpan duration)
		{
			return (!(this.duration <= duration)) ? new TakeObservable<T>(source, duration, scheduler) : this;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (scheduler == null)
			{
				return source.Subscribe(new Take(this, observer, cancel));
			}
			return new Take_(this, observer, cancel).Run();
		}
	}
}
