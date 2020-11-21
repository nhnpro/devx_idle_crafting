using System;

namespace UniRx.Operators
{
	internal class DefaultIfEmptyObservable<T> : OperatorObservableBase<T>
	{
		private class DefaultIfEmpty : OperatorObserverBase<T, T>
		{
			private readonly DefaultIfEmptyObservable<T> parent;

			private bool hasValue;

			public DefaultIfEmpty(DefaultIfEmptyObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				hasValue = false;
			}

			public override void OnNext(T value)
			{
				hasValue = true;
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
				if (!hasValue)
				{
					observer.OnNext(parent.defaultValue);
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
		}

		private readonly UniRx.IObservable<T> source;

		private readonly T defaultValue;

		public DefaultIfEmptyObservable(UniRx.IObservable<T> source, T defaultValue)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.defaultValue = defaultValue;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new DefaultIfEmpty(this, observer, cancel));
		}
	}
}
