using System;

namespace UniRx.Operators
{
	internal class LastObservable<T> : OperatorObservableBase<T>
	{
		private class Last : OperatorObserverBase<T, T>
		{
			private readonly LastObservable<T> parent;

			private bool notPublished;

			private T lastValue;

			public Last(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				notPublished = true;
			}

			public override void OnNext(T value)
			{
				notPublished = false;
				lastValue = value;
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
				if (parent.useDefault)
				{
					if (notPublished)
					{
						observer.OnNext(default(T));
					}
					else
					{
						observer.OnNext(lastValue);
					}
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				}
				else if (notPublished)
				{
					try
					{
						observer.OnError(new InvalidOperationException("sequence is empty"));
					}
					finally
					{
						Dispose();
					}
				}
				else
				{
					observer.OnNext(lastValue);
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

		private class Last_ : OperatorObserverBase<T, T>
		{
			private readonly LastObservable<T> parent;

			private bool notPublished;

			private T lastValue;

			public Last_(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				notPublished = true;
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
					notPublished = false;
					lastValue = value;
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
				if (parent.useDefault)
				{
					if (notPublished)
					{
						observer.OnNext(default(T));
					}
					else
					{
						observer.OnNext(lastValue);
					}
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				}
				else if (notPublished)
				{
					try
					{
						observer.OnError(new InvalidOperationException("sequence is empty"));
					}
					finally
					{
						Dispose();
					}
				}
				else
				{
					observer.OnNext(lastValue);
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

		private readonly UniRx.IObservable<T> source;

		private readonly bool useDefault;

		private readonly Func<T, bool> predicate;

		public LastObservable(UniRx.IObservable<T> source, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.useDefault = useDefault;
		}

		public LastObservable(UniRx.IObservable<T> source, Func<T, bool> predicate, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
			this.useDefault = useDefault;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			if (predicate == null)
			{
				return source.Subscribe(new Last(this, observer, cancel));
			}
			return source.Subscribe(new Last_(this, observer, cancel));
		}
	}
}
