using System;

namespace UniRx.Operators
{
	internal class SingleObservable<T> : OperatorObservableBase<T>
	{
		private class Single : OperatorObserverBase<T, T>
		{
			private readonly SingleObservable<T> parent;

			private bool seenValue;

			private T lastValue;

			public Single(SingleObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				seenValue = false;
			}

			public override void OnNext(T value)
			{
				if (seenValue)
				{
					try
					{
						observer.OnError(new InvalidOperationException("sequence is not single"));
					}
					finally
					{
						Dispose();
					}
					return;
				}
				seenValue = true;
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
					if (!seenValue)
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
				else if (!seenValue)
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

		private class Single_ : OperatorObserverBase<T, T>
		{
			private readonly SingleObservable<T> parent;

			private bool seenValue;

			private T lastValue;

			public Single_(SingleObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				seenValue = false;
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
					if (seenValue)
					{
						try
						{
							observer.OnError(new InvalidOperationException("sequence is not single"));
						}
						finally
						{
							Dispose();
						}
						return;
					}
					seenValue = true;
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
					if (!seenValue)
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
				else if (!seenValue)
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

		public SingleObservable(UniRx.IObservable<T> source, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.useDefault = useDefault;
		}

		public SingleObservable(UniRx.IObservable<T> source, Func<T, bool> predicate, bool useDefault)
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
				return source.Subscribe(new Single(this, observer, cancel));
			}
			return source.Subscribe(new Single_(this, observer, cancel));
		}
	}
}
