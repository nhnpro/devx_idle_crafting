using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ToObservableObservable<T> : OperatorObservableBase<T>
	{
		private class ToObservable : OperatorObserverBase<T, T>
		{
			private readonly ToObservableObservable<T> parent;

			public ToObservable(ToObservableObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IEnumerator<T> e = null;
				try
				{
					e = parent.source.GetEnumerator();
				}
				catch (Exception error)
				{
					OnError(error);
					return Disposable.Empty;
				}
				if (parent.scheduler == Scheduler.Immediate)
				{
					while (true)
					{
						T value = default(T);
						bool flag2;
						try
						{
							flag2 = e.MoveNext();
							if (flag2)
							{
								value = e.Current;
							}
						}
						catch (Exception error2)
						{
							e.Dispose();
							try
							{
								observer.OnError(error2);
							}
							finally
							{
								Dispose();
							}
							break;
						}
						if (flag2)
						{
							observer.OnNext(value);
							continue;
						}
						e.Dispose();
						try
						{
							observer.OnCompleted();
						}
						finally
						{
							Dispose();
						}
						break;
					}
					return Disposable.Empty;
				}
				SingleAssignmentDisposable flag = new SingleAssignmentDisposable();
				flag.Disposable = parent.scheduler.Schedule(delegate(Action self)
				{
					if (flag.IsDisposed)
					{
						e.Dispose();
					}
					else
					{
						T value2 = default(T);
						bool flag3;
						try
						{
							flag3 = e.MoveNext();
							if (flag3)
							{
								value2 = e.Current;
							}
						}
						catch (Exception error3)
						{
							e.Dispose();
							try
							{
								observer.OnError(error3);
							}
							finally
							{
								Dispose();
							}
							return;
						}
						if (flag3)
						{
							observer.OnNext(value2);
							self();
						}
						else
						{
							e.Dispose();
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
				});
				return flag;
			}

			public override void OnNext(T value)
			{
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

		private readonly IEnumerable<T> source;

		private readonly IScheduler scheduler;

		public ToObservableObservable(IEnumerable<T> source, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			this.source = source;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new ToObservable(this, observer, cancel).Run();
		}
	}
}
