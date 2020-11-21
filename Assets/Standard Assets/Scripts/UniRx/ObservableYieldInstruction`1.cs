using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
	public class ObservableYieldInstruction<T> : IEnumerator<T>, ICustomYieldInstructionErrorHandler, IEnumerator, IDisposable
	{
		private class ToYieldInstruction : IObserver<T>
		{
			private readonly ObservableYieldInstruction<T> parent;

			public ToYieldInstruction(ObservableYieldInstruction<T> parent)
			{
				this.parent = parent;
			}

			public void OnNext(T value)
			{
				parent.current = value;
			}

			public void OnError(Exception error)
			{
				parent.moveNext = false;
				parent.error = error;
			}

			public void OnCompleted()
			{
				parent.moveNext = false;
				parent.hasResult = true;
				parent.result = parent.current;
			}
		}

		private readonly IDisposable subscription;

		private readonly CancellationToken cancel;

		private bool reThrowOnError;

		private T current;

		private T result;

		private bool moveNext;

		private bool hasResult;

		private Exception error;

		T IEnumerator<T>.Current => current;

		object IEnumerator.Current => current;

		bool ICustomYieldInstructionErrorHandler.IsReThrowOnError => reThrowOnError;

		public bool HasError => error != null;

		public bool HasResult => hasResult;

		public bool IsCanceled
		{
			get
			{
				if (hasResult)
				{
					return false;
				}
				if (error != null)
				{
					return false;
				}
				return cancel.IsCancellationRequested;
			}
		}

		public bool IsDone => HasResult || HasError || cancel.IsCancellationRequested;

		public T Result => result;

		public Exception Error => error;

		public ObservableYieldInstruction(UniRx.IObservable<T> source, bool reThrowOnError, CancellationToken cancel)
		{
			moveNext = true;
			this.reThrowOnError = reThrowOnError;
			this.cancel = cancel;
			try
			{
				subscription = source.Subscribe(new ToYieldInstruction(this));
			}
			catch
			{
				moveNext = false;
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			if (!moveNext)
			{
				if (reThrowOnError && HasError)
				{
					throw Error;
				}
				return false;
			}
			if (cancel.IsCancellationRequested)
			{
				subscription.Dispose();
				return false;
			}
			return true;
		}

		void ICustomYieldInstructionErrorHandler.ForceDisableRethrowOnError()
		{
			reThrowOnError = false;
		}

		void ICustomYieldInstructionErrorHandler.ForceEnableRethrowOnError()
		{
			reThrowOnError = true;
		}

		public void Dispose()
		{
			subscription.Dispose();
		}

		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}
}
