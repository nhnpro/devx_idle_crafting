using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class RepeatSafeObservable<T> : OperatorObservableBase<T>
	{
		private class RepeatSafe : OperatorObserverBase<T, T>
		{
			private readonly RepeatSafeObservable<T> parent;

			private readonly object gate = new object();

			private IEnumerator<UniRx.IObservable<T>> e;

			private SerialDisposable subscription;

			private Action nextSelf;

			private bool isDisposed;

			private bool isRunNext;

			public RepeatSafe(RepeatSafeObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				isDisposed = false;
				isRunNext = false;
				e = parent.sources.GetEnumerator();
				subscription = new SerialDisposable();
				IDisposable disposable = Scheduler.DefaultSchedulers.TailRecursion.Schedule(RecursiveRun);
				return StableCompositeDisposable.Create(disposable, subscription, Disposable.Create(delegate
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
					if (!isDisposed)
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
							try
							{
								observer.OnError(ex);
							}
							finally
							{
								Dispose();
							}
						}
						else if (!flag)
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
						else
						{
							UniRx.IObservable<T> current = e.Current;
							SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
							subscription.Disposable = singleAssignmentDisposable;
							singleAssignmentDisposable.Disposable = current.Subscribe(this);
						}
					}
				}
			}

			public override void OnNext(T value)
			{
				isRunNext = true;
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
				if (isRunNext && !isDisposed)
				{
					isRunNext = false;
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

		public RepeatSafeObservable(IEnumerable<UniRx.IObservable<T>> sources, bool isRequiredSubscribeOnCurrentThread)
			: base(isRequiredSubscribeOnCurrentThread)
		{
			this.sources = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new RepeatSafe(this, observer, cancel).Run();
		}
	}
}
