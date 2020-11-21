using System;

namespace UniRx.Operators
{
	internal class WithLatestFromObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
	{
		private class WithLatestFrom : OperatorObserverBase<TResult, TResult>
		{
			private class LeftObserver : IObserver<TLeft>
			{
				private readonly WithLatestFrom parent;

				public LeftObserver(WithLatestFrom parent)
				{
					this.parent = parent;
				}

				public void OnNext(TLeft value)
				{
					if (parent.hasLatest)
					{
						TResult val = default(TResult);
						try
						{
							val = parent.parent.selector(value, parent.latestValue);
						}
						catch (Exception error)
						{
							lock (parent.gate)
							{
								parent.OnError(error);
							}
							return;
						}
						lock (parent.gate)
						{
							parent.OnNext(val);
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						parent.OnError(error);
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						parent.OnCompleted();
					}
				}
			}

			private class RightObserver : IObserver<TRight>
			{
				private readonly WithLatestFrom parent;

				private readonly IDisposable selfSubscription;

				public RightObserver(WithLatestFrom parent, IDisposable subscription)
				{
					this.parent = parent;
					selfSubscription = subscription;
				}

				public void OnNext(TRight value)
				{
					parent.latestValue = value;
					parent.hasLatest = true;
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						parent.OnError(error);
					}
				}

				public void OnCompleted()
				{
					selfSubscription.Dispose();
				}
			}

			private readonly WithLatestFromObservable<TLeft, TRight, TResult> parent;

			private readonly object gate = new object();

			private volatile bool hasLatest;

			private TRight latestValue = default(TRight);

			public WithLatestFrom(WithLatestFromObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.left.Subscribe(new LeftObserver(this));
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				singleAssignmentDisposable.Disposable = parent.right.Subscribe(new RightObserver(this, singleAssignmentDisposable));
				return StableCompositeDisposable.Create(disposable, singleAssignmentDisposable);
			}

			public override void OnNext(TResult value)
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

		private readonly UniRx.IObservable<TLeft> left;

		private readonly UniRx.IObservable<TRight> right;

		private readonly Func<TLeft, TRight, TResult> selector;

		public WithLatestFromObservable(UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
			: base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
		{
			this.left = left;
			this.right = right;
			this.selector = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return new WithLatestFrom(this, observer, cancel).Run();
		}
	}
}
