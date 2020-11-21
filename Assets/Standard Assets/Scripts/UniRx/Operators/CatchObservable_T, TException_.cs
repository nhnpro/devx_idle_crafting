using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UniRx.Operators
{
	internal class CatchObservable<T, TException> : OperatorObservableBase<T> where TException : Exception
	{
		private class Catch : OperatorObserverBase<T, T>
		{
			private readonly CatchObservable<T, TException> parent;

			private SerialDisposable serialDisposable;

			[CompilerGenerated]
			private static Func<TException, UniRx.IObservable<T>> _003C_003Ef__mg_0024cache0;

			public Catch(CatchObservable<T, TException> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				serialDisposable = new SerialDisposable();
				serialDisposable.Disposable = parent.source.Subscribe(this);
				return serialDisposable;
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				TException val = error as TException;
				if (val != null)
				{
					UniRx.IObservable<T> observable;
					try
					{
						observable = ((!(parent.errorHandler == new Func<TException, UniRx.IObservable<T>>(Stubs.CatchIgnore<T>))) ? parent.errorHandler(val) : Observable.Empty<T>());
					}
					catch (Exception error2)
					{
						try
						{
							observer.OnError(error2);
						}
						finally
						{
							Dispose();
						}
						return;
					}
					SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
					serialDisposable.Disposable = singleAssignmentDisposable;
					singleAssignmentDisposable.Disposable = observable.Subscribe(observer);
				}
				else
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

		private readonly Func<TException, UniRx.IObservable<T>> errorHandler;

		public CatchObservable(UniRx.IObservable<T> source, Func<TException, UniRx.IObservable<T>> errorHandler)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.errorHandler = errorHandler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Catch(this, observer, cancel).Run();
		}
	}
	internal class CatchObservable<T> : OperatorObservableBase<T>
	{
		private class Catch : OperatorObserverBase<T, T>
		{
			private readonly CatchObservable<T> parent;

			private readonly object gate = new object();

			private bool isDisposed;

			private IEnumerator<UniRx.IObservable<T>> e;

			private SerialDisposable subscription;

			private Exception lastException;

			private Action nextSelf;

			public Catch(CatchObservable<T> parent, IObserver<T> observer, IDisposable cancel)
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
							if (lastException != null)
							{
								try
								{
									observer.OnError(lastException);
								}
								finally
								{
									Dispose();
								}
							}
							else
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
				lastException = error;
				nextSelf();
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

		private readonly IEnumerable<UniRx.IObservable<T>> sources;

		public CatchObservable(IEnumerable<UniRx.IObservable<T>> sources)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new Catch(this, observer, cancel).Run();
		}
	}
}
