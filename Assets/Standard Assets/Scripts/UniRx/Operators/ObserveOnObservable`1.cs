using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ObserveOnObservable<T> : OperatorObservableBase<T>
	{
		private class ObserveOn : OperatorObserverBase<T, T>
		{
			private readonly ObserveOnObservable<T> parent;

			private readonly LinkedList<IDisposable> scheduleDisposables = new LinkedList<IDisposable>();

			private bool isDisposed;

			public ObserveOn(ObserveOnObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				isDisposed = false;
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, Disposable.Create(delegate
				{
					lock (scheduleDisposables)
					{
						isDisposed = true;
						foreach (IDisposable scheduleDisposable in scheduleDisposables)
						{
							scheduleDisposable.Dispose();
						}
						scheduleDisposables.Clear();
					}
				}));
			}

			public override void OnNext(T value)
			{
				SingleAssignmentDisposable self = new SingleAssignmentDisposable();
				LinkedListNode<IDisposable> node;
				lock (scheduleDisposables)
				{
					if (isDisposed)
					{
						return;
					}
					node = scheduleDisposables.AddLast(self);
				}
				self.Disposable = parent.scheduler.Schedule(delegate
				{
					self.Dispose();
					lock (scheduleDisposables)
					{
						if (node.List != null)
						{
							node.List.Remove(node);
						}
					}
					observer.OnNext(value);
				});
			}

			public override void OnError(Exception error)
			{
				SingleAssignmentDisposable self = new SingleAssignmentDisposable();
				LinkedListNode<IDisposable> node;
				lock (scheduleDisposables)
				{
					if (isDisposed)
					{
						return;
					}
					node = scheduleDisposables.AddLast(self);
				}
				self.Disposable = parent.scheduler.Schedule(delegate
				{
					self.Dispose();
					lock (scheduleDisposables)
					{
						if (node.List != null)
						{
							node.List.Remove(node);
						}
					}
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
				});
			}

			public override void OnCompleted()
			{
				SingleAssignmentDisposable self = new SingleAssignmentDisposable();
				LinkedListNode<IDisposable> node;
				lock (scheduleDisposables)
				{
					if (isDisposed)
					{
						return;
					}
					node = scheduleDisposables.AddLast(self);
				}
				self.Disposable = parent.scheduler.Schedule(delegate
				{
					self.Dispose();
					lock (scheduleDisposables)
					{
						if (node.List != null)
						{
							node.List.Remove(node);
						}
					}
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				});
			}
		}

		private class ObserveOn_ : OperatorObserverBase<T, T>
		{
			private readonly ObserveOnObservable<T> parent;

			private readonly ISchedulerQueueing scheduler;

			private readonly BooleanDisposable isDisposed;

			private readonly Action<T> onNext;

			public ObserveOn_(ObserveOnObservable<T> parent, ISchedulerQueueing scheduler, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				this.scheduler = scheduler;
				isDisposed = new BooleanDisposable();
				onNext = OnNext_;
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, isDisposed);
			}

			private void OnNext_(T value)
			{
				observer.OnNext(value);
			}

			private void OnError_(Exception error)
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

			private void OnCompleted_(Unit _)
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

			public override void OnNext(T value)
			{
				scheduler.ScheduleQueueing(isDisposed, value, onNext);
			}

			public override void OnError(Exception error)
			{
				scheduler.ScheduleQueueing(isDisposed, error, OnError_);
			}

			public override void OnCompleted()
			{
				scheduler.ScheduleQueueing(isDisposed, Unit.Default, OnCompleted_);
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly IScheduler scheduler;

		public ObserveOnObservable(UniRx.IObservable<T> source, IScheduler scheduler)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			ISchedulerQueueing schedulerQueueing = scheduler as ISchedulerQueueing;
			if (schedulerQueueing == null)
			{
				return new ObserveOn(this, observer, cancel).Run();
			}
			return new ObserveOn_(this, schedulerQueueing, observer, cancel).Run();
		}
	}
}
