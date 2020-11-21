using System;

namespace UniRx.Operators
{
	internal class ScanObservable<TSource> : OperatorObservableBase<TSource>
	{
		private class Scan : OperatorObserverBase<TSource, TSource>
		{
			private readonly ScanObservable<TSource> parent;

			private TSource accumulation;

			private bool isFirst;

			public Scan(ScanObservable<TSource> parent, IObserver<TSource> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				isFirst = true;
			}

			public override void OnNext(TSource value)
			{
				if (isFirst)
				{
					isFirst = false;
					accumulation = value;
				}
				else
				{
					try
					{
						accumulation = parent.accumulator(accumulation, value);
					}
					catch (Exception error)
					{
						try
						{
							observer.OnError(error);
						}
						finally
						{
							Dispose();
						}
						return;
					}
				}
				observer.OnNext(accumulation);
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

		private readonly UniRx.IObservable<TSource> source;

		private readonly Func<TSource, TSource, TSource> accumulator;

		public ScanObservable(UniRx.IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.accumulator = accumulator;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TSource> observer, IDisposable cancel)
		{
			return source.Subscribe(new Scan(this, observer, cancel));
		}
	}
	internal class ScanObservable<TSource, TAccumulate> : OperatorObservableBase<TAccumulate>
	{
		private class Scan : OperatorObserverBase<TSource, TAccumulate>
		{
			private readonly ScanObservable<TSource, TAccumulate> parent;

			private TAccumulate accumulation;

			private bool isFirst;

			public Scan(ScanObservable<TSource, TAccumulate> parent, IObserver<TAccumulate> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				isFirst = true;
			}

			public override void OnNext(TSource value)
			{
				if (isFirst)
				{
					isFirst = false;
					accumulation = parent.seed;
				}
				try
				{
					accumulation = parent.accumulator(accumulation, value);
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				observer.OnNext(accumulation);
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

		private readonly UniRx.IObservable<TSource> source;

		private readonly TAccumulate seed;

		private readonly Func<TAccumulate, TSource, TAccumulate> accumulator;

		public ScanObservable(UniRx.IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.seed = seed;
			this.accumulator = accumulator;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TAccumulate> observer, IDisposable cancel)
		{
			return source.Subscribe(new Scan(this, observer, cancel));
		}
	}
}
