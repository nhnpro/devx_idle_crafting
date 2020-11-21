using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class WhenAllObservable<T> : OperatorObservableBase<T[]>
	{
		private class WhenAll : OperatorObserverBase<T[], T[]>
		{
			private class WhenAllCollectionObserver : IObserver<T>
			{
				private readonly WhenAll parent;

				private readonly int index;

				private bool isCompleted;

				public WhenAllCollectionObserver(WhenAll parent, int index)
				{
					this.parent = parent;
					this.index = index;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.values[index] = value;
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(parent.values);
								parent.OnCompleted();
							}
						}
					}
				}
			}

			private readonly UniRx.IObservable<T>[] sources;

			private readonly object gate = new object();

			private int completedCount;

			private int length;

			private T[] values;

			public WhenAll(UniRx.IObservable<T>[] sources, IObserver<T[]> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Length;
				if (length == 0)
				{
					OnNext(new T[0]);
					try
					{
						base.observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
					return Disposable.Empty;
				}
				completedCount = 0;
				values = new T[length];
				IDisposable[] array = new IDisposable[length];
				for (int i = 0; i < length; i++)
				{
					UniRx.IObservable<T> observable = sources[i];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this, i);
					array[i] = observable.Subscribe(observer);
				}
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			public override void OnNext(T[] value)
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

		private class WhenAll_ : OperatorObserverBase<T[], T[]>
		{
			private class WhenAllCollectionObserver : IObserver<T>
			{
				private readonly WhenAll_ parent;

				private readonly int index;

				private bool isCompleted;

				public WhenAllCollectionObserver(WhenAll_ parent, int index)
				{
					this.parent = parent;
					this.index = index;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.values[index] = value;
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(parent.values);
								parent.OnCompleted();
							}
						}
					}
				}
			}

			private readonly IList<UniRx.IObservable<T>> sources;

			private readonly object gate = new object();

			private int completedCount;

			private int length;

			private T[] values;

			public WhenAll_(IList<UniRx.IObservable<T>> sources, IObserver<T[]> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Count;
				if (length == 0)
				{
					OnNext(new T[0]);
					try
					{
						base.observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
					return Disposable.Empty;
				}
				completedCount = 0;
				values = new T[length];
				IDisposable[] array = new IDisposable[length];
				for (int i = 0; i < length; i++)
				{
					UniRx.IObservable<T> observable = sources[i];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this, i);
					array[i] = observable.Subscribe(observer);
				}
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			public override void OnNext(T[] value)
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

		private readonly UniRx.IObservable<T>[] sources;

		private readonly IEnumerable<UniRx.IObservable<T>> sourcesEnumerable;

		public WhenAllObservable(UniRx.IObservable<T>[] sources)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.sources = sources;
		}

		public WhenAllObservable(IEnumerable<UniRx.IObservable<T>> sources)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			sourcesEnumerable = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<T[]> observer, IDisposable cancel)
		{
			if (sources != null)
			{
				return new WhenAll(sources, observer, cancel).Run();
			}
			IList<UniRx.IObservable<T>> list = sourcesEnumerable as IList<UniRx.IObservable<T>>;
			if (list == null)
			{
				list = new List<UniRx.IObservable<T>>(sourcesEnumerable);
			}
			return new WhenAll_(list, observer, cancel).Run();
		}
	}
	internal class WhenAllObservable : OperatorObservableBase<Unit>
	{
		private class WhenAll : OperatorObserverBase<Unit, Unit>
		{
			private class WhenAllCollectionObserver : IObserver<Unit>
			{
				private readonly WhenAll parent;

				private bool isCompleted;

				public WhenAllCollectionObserver(WhenAll parent)
				{
					this.parent = parent;
				}

				public void OnNext(Unit value)
				{
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(Unit.Default);
								parent.OnCompleted();
							}
						}
					}
				}
			}

			private readonly UniRx.IObservable<Unit>[] sources;

			private readonly object gate = new object();

			private int completedCount;

			private int length;

			public WhenAll(UniRx.IObservable<Unit>[] sources, IObserver<Unit> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Length;
				if (length == 0)
				{
					OnNext(Unit.Default);
					try
					{
						base.observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
					return Disposable.Empty;
				}
				completedCount = 0;
				IDisposable[] array = new IDisposable[length];
				for (int i = 0; i < sources.Length; i++)
				{
					UniRx.IObservable<Unit> observable = sources[i];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this);
					array[i] = observable.Subscribe(observer);
				}
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			public override void OnNext(Unit value)
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

		private class WhenAll_ : OperatorObserverBase<Unit, Unit>
		{
			private class WhenAllCollectionObserver : IObserver<Unit>
			{
				private readonly WhenAll_ parent;

				private bool isCompleted;

				public WhenAllCollectionObserver(WhenAll_ parent)
				{
					this.parent = parent;
				}

				public void OnNext(Unit value)
				{
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(Unit.Default);
								parent.OnCompleted();
							}
						}
					}
				}
			}

			private readonly IList<UniRx.IObservable<Unit>> sources;

			private readonly object gate = new object();

			private int completedCount;

			private int length;

			public WhenAll_(IList<UniRx.IObservable<Unit>> sources, IObserver<Unit> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Count;
				if (length == 0)
				{
					OnNext(Unit.Default);
					try
					{
						base.observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
					return Disposable.Empty;
				}
				completedCount = 0;
				IDisposable[] array = new IDisposable[length];
				for (int i = 0; i < length; i++)
				{
					UniRx.IObservable<Unit> observable = sources[i];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this);
					array[i] = observable.Subscribe(observer);
				}
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			public override void OnNext(Unit value)
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

		private readonly UniRx.IObservable<Unit>[] sources;

		private readonly IEnumerable<UniRx.IObservable<Unit>> sourcesEnumerable;

		public WhenAllObservable(UniRx.IObservable<Unit>[] sources)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			this.sources = sources;
		}

		public WhenAllObservable(IEnumerable<UniRx.IObservable<Unit>> sources)
			: base(isRequiredSubscribeOnCurrentThread: false)
		{
			sourcesEnumerable = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<Unit> observer, IDisposable cancel)
		{
			if (sources != null)
			{
				return new WhenAll(sources, observer, cancel).Run();
			}
			IList<UniRx.IObservable<Unit>> list = sourcesEnumerable as IList<UniRx.IObservable<Unit>>;
			if (list == null)
			{
				list = new List<UniRx.IObservable<Unit>>(sourcesEnumerable);
			}
			return new WhenAll_(list, observer, cancel).Run();
		}
	}
}
