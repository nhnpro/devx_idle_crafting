using System;
using UniRx.InternalUtil;

namespace UniRx
{
	public class AsyncReactiveCommand : AsyncReactiveCommand<Unit>
	{
		public AsyncReactiveCommand()
		{
		}

		public AsyncReactiveCommand(UniRx.IObservable<bool> canExecuteSource)
			: base(canExecuteSource)
		{
		}

		public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
			: base(sharedCanExecute)
		{
		}

		public IDisposable Execute()
		{
			return Execute(Unit.Default);
		}
	}
	public class AsyncReactiveCommand<T> : IAsyncReactiveCommand<T>
	{
		private class Subscription : IDisposable
		{
			private readonly AsyncReactiveCommand<T> parent;

			private readonly Func<T, UniRx.IObservable<Unit>> asyncAction;

			public Subscription(AsyncReactiveCommand<T> parent, Func<T, UniRx.IObservable<Unit>> asyncAction)
			{
				this.parent = parent;
				this.asyncAction = asyncAction;
			}

			public void Dispose()
			{
				lock (parent.gate)
				{
					parent.asyncActions = parent.asyncActions.Remove(asyncAction);
				}
			}
		}

		private ImmutableList<Func<T, UniRx.IObservable<Unit>>> asyncActions = ImmutableList<Func<T, UniRx.IObservable<Unit>>>.Empty;

		private readonly object gate = new object();

		private readonly IReactiveProperty<bool> canExecuteSource;

		private readonly IReadOnlyReactiveProperty<bool> canExecute;

		public IReadOnlyReactiveProperty<bool> CanExecute => canExecute;

		public bool IsDisposed
		{
			get;
			private set;
		}

		public AsyncReactiveCommand()
		{
			canExecuteSource = new ReactiveProperty<bool>(initialValue: true);
			canExecute = canExecuteSource;
		}

		public AsyncReactiveCommand(UniRx.IObservable<bool> canExecuteSource)
		{
			this.canExecuteSource = new ReactiveProperty<bool>(initialValue: true);
			canExecute = canExecute.CombineLatest(canExecuteSource, (bool x, bool y) => x && y).ToReactiveProperty();
		}

		public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
		{
			canExecuteSource = sharedCanExecute;
			canExecute = sharedCanExecute;
		}

		public IDisposable Execute(T parameter)
		{
			if (canExecute.Value)
			{
				canExecuteSource.Value = false;
				Func<T, UniRx.IObservable<Unit>>[] data = asyncActions.Data;
				if (data.Length == 1)
				{
					try
					{
						UniRx.IObservable<Unit> source = data[0](parameter) ?? Observable.ReturnUnit();
						return source.Finally(delegate
						{
							canExecuteSource.Value = true;
						}).Subscribe();
					}
					catch
					{
						canExecuteSource.Value = true;
						throw;
					}
				}
				UniRx.IObservable<Unit>[] array = new UniRx.IObservable<Unit>[data.Length];
				try
				{
					for (int i = 0; i < data.Length; i++)
					{
						array[i] = (data[i](parameter) ?? Observable.ReturnUnit());
					}
				}
				catch
				{
					canExecuteSource.Value = true;
					throw;
				}
				return Observable.WhenAll(array).Finally(delegate
				{
					canExecuteSource.Value = true;
				}).Subscribe();
			}
			return Disposable.Empty;
		}

		public IDisposable Subscribe(Func<T, UniRx.IObservable<Unit>> asyncAction)
		{
			lock (gate)
			{
				asyncActions = asyncActions.Add(asyncAction);
			}
			return new Subscription(this, asyncAction);
		}
	}
}
