using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class BufferObservable<T> : OperatorObservableBase<IList<T>>
	{
		private class Buffer : OperatorObserverBase<T, IList<T>>
		{
			private readonly BufferObservable<T> parent;

			private List<T> list;

			public Buffer(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<T>(parent.count);
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				list.Add(value);
				if (list.Count == parent.count)
				{
					observer.OnNext(list);
					list = new List<T>(parent.count);
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
				if (list.Count > 0)
				{
					observer.OnNext(list);
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

		private class Buffer_ : OperatorObserverBase<T, IList<T>>
		{
			private readonly BufferObservable<T> parent;

			private Queue<List<T>> q;

			private int index;

			public Buffer_(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				q = new Queue<List<T>>();
				index = -1;
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				index++;
				if (index % parent.skip == 0)
				{
					q.Enqueue(new List<T>(parent.count));
				}
				int count = q.Count;
				for (int i = 0; i < count; i++)
				{
					List<T> list = q.Dequeue();
					list.Add(value);
					if (list.Count == parent.count)
					{
						observer.OnNext(list);
					}
					else
					{
						q.Enqueue(list);
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
				foreach (List<T> item in q)
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

		private class BufferT : OperatorObserverBase<T, IList<T>>
		{
			private class Buffer : IObserver<long>
			{
				private BufferT parent;

				public Buffer(BufferT parent)
				{
					this.parent = parent;
				}

				public void OnNext(long value)
				{
					bool flag = false;
					List<T> list;
					lock (parent.gate)
					{
						list = parent.list;
						if (list.Count != 0)
						{
							parent.list = new List<T>();
						}
						else
						{
							flag = true;
						}
					}
					parent.observer.OnNext((IList<T>)((!flag) ? ((object)list) : ((object)BufferT.EmptyArray)));
				}

				public void OnError(Exception error)
				{
				}

				public void OnCompleted()
				{
				}
			}

			private static readonly T[] EmptyArray = new T[0];

			private readonly BufferObservable<T> parent;

			private readonly object gate = new object();

			private List<T> list;

			public BufferT(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<T>();
				IDisposable disposable = Observable.Interval(parent.timeSpan, parent.scheduler).Subscribe(new Buffer(this));
				IDisposable disposable2 = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, disposable2);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					list.Add(value);
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
				List<T> value;
				lock (gate)
				{
					value = list;
				}
				observer.OnNext(value);
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

		private class BufferTS : OperatorObserverBase<T, IList<T>>
		{
			private readonly BufferObservable<T> parent;

			private readonly object gate = new object();

			private Queue<IList<T>> q;

			private TimeSpan totalTime;

			private TimeSpan nextShift;

			private TimeSpan nextSpan;

			private SerialDisposable timerD;

			public BufferTS(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				totalTime = TimeSpan.Zero;
				nextShift = parent.timeShift;
				nextSpan = parent.timeSpan;
				q = new Queue<IList<T>>();
				timerD = new SerialDisposable();
				q.Enqueue(new List<T>());
				CreateTimer();
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, timerD);
			}

			private void CreateTimer()
			{
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				timerD.Disposable = singleAssignmentDisposable;
				bool isSpan = false;
				bool isShift = false;
				if (nextSpan == nextShift)
				{
					isSpan = true;
					isShift = true;
				}
				else if (nextSpan < nextShift)
				{
					isSpan = true;
				}
				else
				{
					isShift = true;
				}
				TimeSpan t = (!isSpan) ? nextShift : nextSpan;
				TimeSpan dueTime = t - totalTime;
				totalTime = t;
				if (isSpan)
				{
					nextSpan += parent.timeShift;
				}
				if (isShift)
				{
					nextShift += parent.timeShift;
				}
				singleAssignmentDisposable.Disposable = parent.scheduler.Schedule(dueTime, delegate
				{
					lock (gate)
					{
						if (isShift)
						{
							List<T> item = new List<T>();
							q.Enqueue(item);
						}
						if (isSpan)
						{
							IList<T> value = q.Dequeue();
							observer.OnNext(value);
						}
					}
					CreateTimer();
				});
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					foreach (IList<T> item in q)
					{
						item.Add(value);
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
				lock (gate)
				{
					foreach (IList<T> item in q)
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
		}

		private class BufferTC : OperatorObserverBase<T, IList<T>>
		{
			private static readonly T[] EmptyArray = new T[0];

			private readonly BufferObservable<T> parent;

			private readonly object gate = new object();

			private List<T> list;

			private long timerId;

			private SerialDisposable timerD;

			public BufferTC(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<T>();
				timerId = 0L;
				timerD = new SerialDisposable();
				CreateTimer();
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, timerD);
			}

			private void CreateTimer()
			{
				long currentTimerId = timerId;
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				timerD.Disposable = singleAssignmentDisposable;
				ISchedulerPeriodic schedulerPeriodic = parent.scheduler as ISchedulerPeriodic;
				if (schedulerPeriodic != null)
				{
					singleAssignmentDisposable.Disposable = schedulerPeriodic.SchedulePeriodic(parent.timeSpan, delegate
					{
						OnNextTick(currentTimerId);
					});
				}
				else
				{
					singleAssignmentDisposable.Disposable = parent.scheduler.Schedule(parent.timeSpan, delegate(Action<TimeSpan> self)
					{
						OnNextRecursive(currentTimerId, self);
					});
				}
			}

			private void OnNextTick(long currentTimerId)
			{
				bool flag = false;
				List<T> list;
				lock (gate)
				{
					if (currentTimerId != timerId)
					{
						return;
					}
					list = this.list;
					if (list.Count != 0)
					{
						this.list = new List<T>();
					}
					else
					{
						flag = true;
					}
				}
				observer.OnNext((IList<T>)((!flag) ? ((object)list) : ((object)EmptyArray)));
			}

			private void OnNextRecursive(long currentTimerId, Action<TimeSpan> self)
			{
				bool flag = false;
				List<T> list;
				lock (gate)
				{
					if (currentTimerId != timerId)
					{
						return;
					}
					list = this.list;
					if (list.Count != 0)
					{
						this.list = new List<T>();
					}
					else
					{
						flag = true;
					}
				}
				observer.OnNext((IList<T>)((!flag) ? ((object)list) : ((object)EmptyArray)));
				self(parent.timeSpan);
			}

			public override void OnNext(T value)
			{
				List<T> list = null;
				lock (gate)
				{
					this.list.Add(value);
					if (this.list.Count == parent.count)
					{
						list = this.list;
						this.list = new List<T>();
						timerId++;
						CreateTimer();
					}
				}
				if (list != null)
				{
					observer.OnNext(list);
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
				List<T> value;
				lock (gate)
				{
					timerId++;
					value = list;
				}
				observer.OnNext(value);
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

		private readonly int skip;

		private readonly TimeSpan timeSpan;

		private readonly TimeSpan timeShift;

		private readonly IScheduler scheduler;

		public BufferObservable(UniRx.IObservable<T> source, int count, int skip)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.count = count;
			this.skip = skip;
		}

		public BufferObservable(UniRx.IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.timeSpan = timeSpan;
			this.timeShift = timeShift;
			this.scheduler = scheduler;
		}

		public BufferObservable(UniRx.IObservable<T> source, TimeSpan timeSpan, int count, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.timeSpan = timeSpan;
			this.count = count;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<T>> observer, IDisposable cancel)
		{
			if (scheduler == null)
			{
				if (skip == 0)
				{
					return new Buffer(this, observer, cancel).Run();
				}
				return new Buffer_(this, observer, cancel).Run();
			}
			if (count > 0)
			{
				return new BufferTC(this, observer, cancel).Run();
			}
			if (timeSpan == timeShift)
			{
				return new BufferT(this, observer, cancel).Run();
			}
			return new BufferTS(this, observer, cancel).Run();
		}
	}
	internal class BufferObservable<TSource, TWindowBoundary> : OperatorObservableBase<IList<TSource>>
	{
		private class Buffer : OperatorObserverBase<TSource, IList<TSource>>
		{
			private class Buffer_ : IObserver<TWindowBoundary>
			{
				private readonly Buffer parent;

				public Buffer_(Buffer parent)
				{
					this.parent = parent;
				}

				public void OnNext(TWindowBoundary value)
				{
					bool flag = false;
					List<TSource> list;
					lock (parent.gate)
					{
						list = parent.list;
						if (list.Count != 0)
						{
							parent.list = new List<TSource>();
						}
						else
						{
							flag = true;
						}
					}
					if (flag)
					{
						parent.observer.OnNext(Buffer.EmptyArray);
					}
					else
					{
						parent.observer.OnNext(list);
					}
				}

				public void OnError(Exception error)
				{
					parent.OnError(error);
				}

				public void OnCompleted()
				{
					parent.OnCompleted();
				}
			}

			private static readonly TSource[] EmptyArray = new TSource[0];

			private readonly BufferObservable<TSource, TWindowBoundary> parent;

			private object gate = new object();

			private List<TSource> list;

			public Buffer(BufferObservable<TSource, TWindowBoundary> parent, IObserver<IList<TSource>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<TSource>();
				IDisposable disposable = parent.source.Subscribe(this);
				IDisposable disposable2 = parent.windowBoundaries.Subscribe(new Buffer_(this));
				return StableCompositeDisposable.Create(disposable, disposable2);
			}

			public override void OnNext(TSource value)
			{
				lock (gate)
				{
					list.Add(value);
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
					List<TSource> value = list;
					list = new List<TSource>();
					observer.OnNext(value);
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

		private readonly UniRx.IObservable<TSource> source;

		private readonly UniRx.IObservable<TWindowBoundary> windowBoundaries;

		public BufferObservable(UniRx.IObservable<TSource> source, UniRx.IObservable<TWindowBoundary> windowBoundaries)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.windowBoundaries = windowBoundaries;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<TSource>> observer, IDisposable cancel)
		{
			return new Buffer(this, observer, cancel).Run();
		}
	}
}
