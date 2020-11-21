using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class DelayObservable<T> : OperatorObservableBase<T>
	{
		private class Delay : OperatorObserverBase<T, T>
		{
			private readonly DelayObservable<T> parent;

			private readonly object gate = new object();

			private bool hasFailed;

			private bool running;

			private bool active;

			private Exception exception;

			private Queue<Timestamped<T>> queue;

			private bool onCompleted;

			private DateTimeOffset completeAt;

			private IDisposable sourceSubscription;

			private TimeSpan delay;

			private bool ready;

			private SerialDisposable cancelable;

			public Delay(DelayObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				cancelable = new SerialDisposable();
				active = false;
				running = false;
				queue = new Queue<Timestamped<T>>();
				onCompleted = false;
				completeAt = default(DateTimeOffset);
				hasFailed = false;
				exception = null;
				ready = true;
				delay = Scheduler.Normalize(parent.dueTime);
				((SingleAssignmentDisposable)(sourceSubscription = new SingleAssignmentDisposable())).Disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(sourceSubscription, cancelable);
			}

			public override void OnNext(T value)
			{
				DateTimeOffset timestamp = parent.scheduler.Now.Add(delay);
				bool flag = false;
				lock (gate)
				{
					queue.Enqueue(new Timestamped<T>(value, timestamp));
					flag = (ready && !active);
					active = true;
				}
				if (flag)
				{
					cancelable.Disposable = parent.scheduler.Schedule(delay, DrainQueue);
				}
			}

			public override void OnError(Exception error)
			{
				sourceSubscription.Dispose();
				bool flag = false;
				lock (gate)
				{
					queue.Clear();
					exception = error;
					hasFailed = true;
					flag = !running;
				}
				if (flag)
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
				sourceSubscription.Dispose();
				DateTimeOffset dateTimeOffset = parent.scheduler.Now.Add(delay);
				bool flag = false;
				lock (gate)
				{
					completeAt = dateTimeOffset;
					onCompleted = true;
					flag = (ready && !active);
					active = true;
				}
				if (flag)
				{
					cancelable.Disposable = parent.scheduler.Schedule(delay, DrainQueue);
				}
			}

			private void DrainQueue(Action<TimeSpan> recurse)
			{
				lock (gate)
				{
					if (hasFailed)
					{
						return;
					}
					running = true;
				}
				bool flag = false;
				bool flag2;
				Exception error;
				bool flag4;
				bool flag5;
				TimeSpan obj;
				while (true)
				{
					flag2 = false;
					error = null;
					bool flag3 = false;
					T value = default(T);
					flag4 = false;
					flag5 = false;
					obj = default(TimeSpan);
					lock (gate)
					{
						if (flag2)
						{
							error = exception;
							flag2 = true;
							running = false;
						}
						else if (queue.Count > 0)
						{
							DateTimeOffset timestamp = queue.Peek().Timestamp;
							if (timestamp.CompareTo(parent.scheduler.Now) <= 0 && !flag)
							{
								value = queue.Dequeue().Value;
								flag3 = true;
							}
							else
							{
								flag5 = true;
								obj = Scheduler.Normalize(timestamp.Subtract(parent.scheduler.Now));
								running = false;
							}
						}
						else if (onCompleted)
						{
							if (completeAt.CompareTo(parent.scheduler.Now) <= 0 && !flag)
							{
								flag4 = true;
							}
							else
							{
								flag5 = true;
								obj = Scheduler.Normalize(completeAt.Subtract(parent.scheduler.Now));
								running = false;
							}
						}
						else
						{
							running = false;
							active = false;
						}
					}
					if (!flag3)
					{
						break;
					}
					observer.OnNext(value);
					flag = true;
				}
				if (flag4)
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
				else if (flag2)
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
				else if (flag5)
				{
					recurse(obj);
				}
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly TimeSpan dueTime;

		private readonly IScheduler scheduler;

		public DelayObservable(UniRx.IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.dueTime = dueTime;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Delay(this, observer, cancel).Run();
		}
	}
}
