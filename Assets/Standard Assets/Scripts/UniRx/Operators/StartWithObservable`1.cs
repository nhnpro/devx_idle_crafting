using System;

namespace UniRx.Operators
{
	internal class StartWithObservable<T> : OperatorObservableBase<T>
	{
		private class StartWith : OperatorObserverBase<T, T>
		{
			private readonly StartWithObservable<T> parent;

			public StartWith(StartWithObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				T value;
				if (parent.valueFactory == null)
				{
					value = parent.value;
				}
				else
				{
					try
					{
						value = parent.valueFactory();
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
						return Disposable.Empty;
					}
				}
				OnNext(value);
				return parent.source.Subscribe(observer);
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

		private readonly T value;

		private readonly Func<T> valueFactory;

		public StartWithObservable(UniRx.IObservable<T> source, T value)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.value = value;
		}

		public StartWithObservable(UniRx.IObservable<T> source, Func<T> valueFactory)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.valueFactory = valueFactory;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return new StartWith(this, observer, cancel).Run();
		}
	}
}
