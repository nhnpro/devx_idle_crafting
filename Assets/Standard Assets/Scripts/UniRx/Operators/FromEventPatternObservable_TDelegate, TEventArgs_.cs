using System;

namespace UniRx.Operators
{
	internal class FromEventPatternObservable<TDelegate, TEventArgs> : OperatorObservableBase<EventPattern<TEventArgs>> where TEventArgs : EventArgs
	{
		private class FromEventPattern : IDisposable
		{
			private readonly FromEventPatternObservable<TDelegate, TEventArgs> parent;

			private readonly IObserver<EventPattern<TEventArgs>> observer;

			private TDelegate handler;

			public FromEventPattern(FromEventPatternObservable<TDelegate, TEventArgs> parent, IObserver<EventPattern<TEventArgs>> observer)
			{
				this.parent = parent;
				this.observer = observer;
			}

			public bool Register()
			{
				handler = parent.conversion(OnNext);
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

			private void OnNext(object sender, TEventArgs eventArgs)
			{
				observer.OnNext(new EventPattern<TEventArgs>(sender, eventArgs));
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = default(TDelegate);
				}
			}
		}

		private readonly Func<EventHandler<TEventArgs>, TDelegate> conversion;

		private readonly Action<TDelegate> addHandler;

		private readonly Action<TDelegate> removeHandler;

		public FromEventPatternObservable(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.conversion = conversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<EventPattern<TEventArgs>> observer, IDisposable cancel)
		{
			FromEventPattern fromEventPattern = new FromEventPattern(this, observer);
			return (!fromEventPattern.Register()) ? Disposable.Empty : fromEventPattern;
		}
	}
}
