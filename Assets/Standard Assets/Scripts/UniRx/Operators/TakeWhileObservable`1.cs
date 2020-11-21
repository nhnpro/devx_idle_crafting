using System;

namespace UniRx.Operators
{
	internal class TakeWhileObservable<T> : OperatorObservableBase<T>
	{
		private class TakeWhile : OperatorObserverBase<T, T>
		{
			private readonly TakeWhileObservable<T> parent;

			public TakeWhile(TakeWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				bool flag;
				try
				{
					flag = parent.predicate(value);
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				if (flag)
				{
					observer.OnNext(value);
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

		private class TakeWhile_ : OperatorObserverBase<T, T>
		{
			private readonly TakeWhileObservable<T> parent;

			private int index;

			public TakeWhile_(TakeWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				bool flag;
				try
				{
					flag = parent.predicateWithIndex(value, index++);
				}
				catch (Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				if (flag)
				{
					observer.OnNext(value);
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

		private readonly UniRx.IObservable<T> source;

		private readonly Func<T, bool> predicate;

		private readonly Func<T, int, bool> predicateWithIndex;

		public TakeWhileObservable(UniRx.IObservable<T> source, Func<T, bool> predicate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
		}

		public TakeWhileObservable(UniRx.IObservable<T> source, Func<T, int, bool> predicateWithIndex)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicateWithIndex = predicateWithIndex;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (predicate != null)
			{
				return new TakeWhile(this, observer, cancel).Run();
			}
			return new TakeWhile_(this, observer, cancel).Run();
		}
	}
}
