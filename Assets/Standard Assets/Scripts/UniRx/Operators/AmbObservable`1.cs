using System;
using UniRx.InternalUtil;

namespace UniRx.Operators
{
	internal class AmbObservable<T> : OperatorObservableBase<T>
	{
		private class AmbOuterObserver : OperatorObserverBase<T, T>
		{
			private enum AmbState
			{
				Left,
				Right,
				Neither
			}

			private class Amb : IObserver<T>
			{
				public IObserver<T> targetObserver;

				public IDisposable targetDisposable;

				public void OnNext(T value)
				{
					targetObserver.OnNext(value);
				}

				public void OnError(Exception error)
				{
					try
					{
						targetObserver.OnError(error);
					}
					finally
					{
						targetObserver = EmptyObserver<T>.Instance;
						targetDisposable.Dispose();
					}
				}

				public void OnCompleted()
				{
					try
					{
						targetObserver.OnCompleted();
					}
					finally
					{
						targetObserver = EmptyObserver<T>.Instance;
						targetDisposable.Dispose();
					}
				}
			}

			private class AmbDecisionObserver : IObserver<T>
			{
				private readonly AmbOuterObserver parent;

				private readonly AmbState me;

				private readonly IDisposable otherSubscription;

				private readonly Amb self;

				public AmbDecisionObserver(AmbOuterObserver parent, AmbState me, IDisposable otherSubscription, Amb self)
				{
					this.parent = parent;
					this.me = me;
					this.otherSubscription = otherSubscription;
					this.self = self;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (parent.choice == AmbState.Neither)
						{
							parent.choice = me;
							otherSubscription.Dispose();
							self.targetObserver = parent.observer;
						}
						if (parent.choice == me)
						{
							self.targetObserver.OnNext(value);
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (parent.choice == AmbState.Neither)
						{
							parent.choice = me;
							otherSubscription.Dispose();
							self.targetObserver = parent.observer;
						}
						if (parent.choice == me)
						{
							self.targetObserver.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (parent.choice == AmbState.Neither)
						{
							parent.choice = me;
							otherSubscription.Dispose();
							self.targetObserver = parent.observer;
						}
						if (parent.choice == me)
						{
							self.targetObserver.OnCompleted();
						}
					}
				}
			}

			private readonly AmbObservable<T> parent;

			private readonly object gate = new object();

			private SingleAssignmentDisposable leftSubscription;

			private SingleAssignmentDisposable rightSubscription;

			private AmbState choice = AmbState.Neither;

			public AmbOuterObserver(AmbObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				leftSubscription = new SingleAssignmentDisposable();
				rightSubscription = new SingleAssignmentDisposable();
				ICancelable cancelable = StableCompositeDisposable.Create(leftSubscription, rightSubscription);
				Amb amb = new Amb();
				amb.targetDisposable = cancelable;
				amb.targetObserver = new AmbDecisionObserver(this, AmbState.Left, rightSubscription, amb);
				Amb amb2 = new Amb();
				amb2.targetDisposable = cancelable;
				amb2.targetObserver = new AmbDecisionObserver(this, AmbState.Right, leftSubscription, amb2);
				leftSubscription.Disposable = parent.source.Subscribe(amb);
				rightSubscription.Disposable = parent.second.Subscribe(amb2);
				return cancelable;
			}

			public override void OnNext(T value)
			{
			}

			public override void OnError(Exception error)
			{
			}

			public override void OnCompleted()
			{
			}
		}

		private readonly UniRx.IObservable<T> source;

		private readonly UniRx.IObservable<T> second;

		public AmbObservable(UniRx.IObservable<T> source, UniRx.IObservable<T> second)
			: base(source.IsRequiredSubscribeOnCurrentThread() || second.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.second = second;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new AmbOuterObserver(this, observer, cancel).Run();
		}
	}
}
