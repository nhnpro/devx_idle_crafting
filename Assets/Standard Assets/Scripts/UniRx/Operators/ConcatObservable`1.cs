using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ConcatObservable<T> : OperatorObservableBase<T>
	{
		private class Concat : OperatorObserverBase<T, T>
		{
			private readonly ConcatObservable<T> parent;

			private readonly object gate = new object();

			private bool isDisposed;

			private IEnumerator<UniRx.IObservable<T>> e;

			private SerialDisposable subscription;

			private Action nextSelf;

			public Concat(ConcatObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				isDisposed = false;
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
							UniRx.IObservable<T> observable2 = observable;
							SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
							subscription.Disposable = singleAssignmentDisposable;
							singleAssignmentDisposable.Disposable = observable2.Subscribe(this);
						}
					}
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
				nextSelf();
			}
		}

		private readonly IEnumerable<UniRx.IObservable<T>> sources;

		public ConcatObservable(IEnumerable<UniRx.IObservable<T>> sources)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
		}

		public UniRx.IObservable<T> Combine(IEnumerable<UniRx.IObservable<T>> combineSources)
		{
			return new ConcatObservable<T>(CombineSources(sources, combineSources));
		}

		private static IEnumerable<UniRx.IObservable<T>> CombineSources(IEnumerable<UniRx.IObservable<T>> first, IEnumerable<UniRx.IObservable<T>> second)
		{
			foreach (UniRx.IObservable<T> item in first)
			{
				yield return item;
			}
			foreach (UniRx.IObservable<T> item2 in second)
			{
				yield return item2;
			}
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Concat(this, observer, cancel).Run();
		}
	}
}
