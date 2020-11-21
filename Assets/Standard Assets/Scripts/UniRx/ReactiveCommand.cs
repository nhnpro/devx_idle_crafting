using System;

namespace UniRx
{
	public class ReactiveCommand : ReactiveCommand<Unit>
	{
		public ReactiveCommand()
		{
		}

		public ReactiveCommand(UniRx.IObservable<bool> canExecuteSource, bool initialValue = true)
			: base(canExecuteSource, initialValue)
		{
		}

		public bool Execute()
		{
			return Execute(Unit.Default);
		}

		public void ForceExecute()
		{
			ForceExecute(Unit.Default);
		}
	}
	public class ReactiveCommand<T> : IReactiveCommand<T>, IDisposable, UniRx.IObservable<T>
	{
		private readonly Subject<T> trigger = new Subject<T>();

		private readonly IDisposable canExecuteSubscription;

		private ReactiveProperty<bool> canExecute;

		public IReadOnlyReactiveProperty<bool> CanExecute => canExecute;

		public bool IsDisposed
		{
			get;
			private set;
		}

		public ReactiveCommand()
		{
			canExecute = new ReactiveProperty<bool>(initialValue: true);
			canExecuteSubscription = Disposable.Empty;
		}

		public ReactiveCommand(UniRx.IObservable<bool> canExecuteSource, bool initialValue = true)
		{
			canExecute = new ReactiveProperty<bool>(initialValue);
			canExecuteSubscription = canExecuteSource.DistinctUntilChanged().SubscribeWithState(canExecute, delegate(bool b, ReactiveProperty<bool> c)
			{
				c.Value = b;
			});
		}

		public bool Execute(T parameter)
		{
			if (canExecute.Value)
			{
				trigger.OnNext(parameter);
				return true;
			}
			return false;
		}

		public void ForceExecute(T parameter)
		{
			trigger.OnNext(parameter);
		}

		public IDisposable Subscribe(UniRx.IObserver<T> observer)
		{
			return trigger.Subscribe(observer);
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				canExecute.Dispose();
				trigger.OnCompleted();
				trigger.Dispose();
				canExecuteSubscription.Dispose();
			}
		}
	}
}
