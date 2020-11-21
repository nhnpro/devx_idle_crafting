using System;

namespace UniRx.Operators
{
	internal class FromEventObservable<TDelegate> : OperatorObservableBase<Unit>
	{
		private class FromEvent : IDisposable
		{
			private readonly FromEventObservable<TDelegate> parent;

			private readonly IObserver<Unit> observer;

			private TDelegate handler;

			public FromEvent(FromEventObservable<TDelegate> parent, IObserver<Unit> observer)
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

			private void OnNext()
			{
				observer.OnNext(Unit.Default);
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

		private readonly Func<Action, TDelegate> conversion;

		private readonly Action<TDelegate> addHandler;

		private readonly Action<TDelegate> removeHandler;

		public FromEventObservable(Func<Action, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.conversion = conversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<Unit> observer, IDisposable cancel)
		{
			FromEvent fromEvent = new FromEvent(this, observer);
			return (!fromEvent.Register()) ? Disposable.Empty : fromEvent;
		}
	}
	internal class FromEventObservable<TDelegate, TEventArgs> : OperatorObservableBase<TEventArgs>
	{
		private class FromEvent : IDisposable
		{
			private readonly FromEventObservable<TDelegate, TEventArgs> parent;

			private readonly IObserver<TEventArgs> observer;

			private TDelegate handler;

			public FromEvent(FromEventObservable<TDelegate, TEventArgs> parent, IObserver<TEventArgs> observer)
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

			private void OnNext(TEventArgs args)
			{
				observer.OnNext(args);
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

		private readonly Func<Action<TEventArgs>, TDelegate> conversion;

		private readonly Action<TDelegate> addHandler;

		private readonly Action<TDelegate> removeHandler;

		public FromEventObservable(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.conversion = conversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TEventArgs> observer, IDisposable cancel)
		{
			FromEvent fromEvent = new FromEvent(this, observer);
			return (!fromEvent.Register()) ? Disposable.Empty : fromEvent;
		}
	}
	internal class FromEventObservable : OperatorObservableBase<Unit>
	{
		private class FromEvent : IDisposable
		{
			private readonly FromEventObservable parent;

			private readonly IObserver<Unit> observer;

			private Action handler;

			public FromEvent(FromEventObservable parent, IObserver<Unit> observer)
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

			private void OnNext()
			{
				observer.OnNext(Unit.Default);
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

		private readonly Action<Action> addHandler;

		private readonly Action<Action> removeHandler;

		public FromEventObservable(Action<Action> addHandler, Action<Action> removeHandler)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<Unit> observer, IDisposable cancel)
		{
			FromEvent fromEvent = new FromEvent(this, observer);
			return (!fromEvent.Register()) ? Disposable.Empty : fromEvent;
		}
	}
}
