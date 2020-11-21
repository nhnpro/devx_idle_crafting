using System;

namespace UniRx.Operators
{
	internal class FromEventObservable_<T> : OperatorObservableBase<T>
	{
		private class FromEvent : IDisposable
		{
			private readonly FromEventObservable_<T> parent;

			private readonly IObserver<T> observer;

			private Action<T> handler;

			public FromEvent(FromEventObservable_<T> parent, IObserver<T> observer)
			{
				this.parent = parent;
				this.observer = observer;
				handler = OnNext;
			}

			public bool Register()
			{
				try
				{
					parent.addHandler(handler);
				}
				catch (Exception error)
				{
					observer.OnError(error);
					return false;
				}
				return true;
			}

			private void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = null;
				}
			}
		}

		private readonly Action<Action<T>> addHandler;

		private readonly Action<Action<T>> removeHandler;

		public FromEventObservable_(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
		{
			FromEvent fromEvent = new FromEvent(this, observer);
			return (!fromEvent.Register()) ? Disposable.Empty : fromEvent;
		}
	}
}
