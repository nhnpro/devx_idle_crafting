using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Operators
{
	internal class RepeatUntilObservable<T> : OperatorObservableBase<T>
	{
		private class RepeatUntil : OperatorObserverBase<T, T>
		{
			private readonly RepeatUntilObservable<T> parent;

			private readonly object gate = new object();

			private IEnumerator<UniRx.IObservable<T>> e;

			private SerialDisposable subscription;

			private SingleAssignmentDisposable schedule;

			private Action nextSelf;

			private bool isStopped;

			private bool isDisposed;

			private bool isFirstSubscribe;

			private IDisposable stopper;

			public RepeatUntil(RepeatUntilObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				isFirstSubscribe = true;
				isDisposed = false;
				isStopped = false;
				e = parent.sources.GetEnumerator();
				subscription = new SerialDisposable();
				schedule = new SingleAssignmentDisposable();
				UniRx.IObservable<Unit> trigger = parent.trigger;
				Action<Unit> onNext = delegate
				{
					lock (gate)
					{
						isStopped = true;
						e.Dispose();
						subscription.Dispose();
						schedule.Dispose();
						base.observer.OnCompleted();
					}
				};
				UniRx.IObserver<T> observer = base.observer;
				stopper = trigger.Subscribe(onNext, observer.OnError);
				schedule.Disposable = Scheduler.CurrentThread.Schedule(RecursiveRun);
				return new CompositeDisposable(schedule, subscription, stopper, Disposable.Create(delegate
				{
					lock (gate)
					{
						isDisposed = true;
						e.Dispose();
					}
				}));
			}

			private void RecursiveRun(Action self)
			{
				lock (gate)
				{
					nextSelf = self;
					if (!isDisposed && !isStopped)
					{
						UniRx.IObservable<T> observable = null;
						bool flag = false;
						Exception ex = null;
						try
						{
							flag = e.MoveNext();
							if (flag)
							{
								observable = e.Current;
								if (observable == null)
								{
									throw new InvalidOperationException("sequence is null.");
								}
							}
							else
							{
								e.Dispose();
							}
						}
						catch (Exception ex2)
						{
							ex = ex2;
							e.Dispose();
						}
						if (ex != null)
						{
							stopper.Dispose();
							observer.OnError(ex);
						}
						else if (!flag)
						{
							stopper.Dispose();
							observer.OnCompleted();
						}
						else
						{
							UniRx.IObservable<T> current = e.Current;
							SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
							subscription.Disposable = singleAssignmentDisposable;
							if (isFirstSubscribe)
							{
								isFirstSubscribe = false;
								singleAssignmentDisposable.Disposable = current.Subscribe(this);
							}
							else
							{
								MainThreadDispatcher.SendStartCoroutine(SubscribeAfterEndOfFrame(singleAssignmentDisposable, current, this, parent.lifeTimeChecker));
							}
						}
					}
				}
			}

			private static IEnumerator SubscribeAfterEndOfFrame(SingleAssignmentDisposable d, UniRx.IObservable<T> source, IObserver<T> observer, GameObject lifeTimeChecker)
			{
				yield return YieldInstructionCache.WaitForEndOfFrame;
				if (!d.IsDisposed && lifeTimeChecker != null)
				{
					d.Disposable = source.Subscribe(observer);
				}
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
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
				if (!isDisposed)
				{
					nextSelf();
					return;
				}
				e.Dispose();
				if (!isDisposed)
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

		private readonly IEnumerable<UniRx.IObservable<T>> sources;

		private readonly UniRx.IObservable<Unit> trigger;

		private readonly GameObject lifeTimeChecker;

		public RepeatUntilObservable(IEnumerable<UniRx.IObservable<T>> sources, UniRx.IObservable<Unit> trigger, GameObject lifeTimeChecker)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
			this.trigger = trigger;
			this.lifeTimeChecker = lifeTimeChecker;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new RepeatUntil(this, observer, cancel).Run();
		}
	}
}
