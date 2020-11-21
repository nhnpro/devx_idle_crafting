using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class BatchFrameObservable<T> : OperatorObservableBase<IList<T>>
	{
		private class BatchFrame : OperatorObserverBase<T, IList<T>>
		{
			private class ReusableEnumerator : IEnumerator
			{
				private readonly BatchFrame parent;

				private int currentFrame;

				public object Current => null;

				public ReusableEnumerator(BatchFrame parent)
				{
					this.parent = parent;
				}

				public bool MoveNext()
				{
					if (parent.cancellationToken.IsDisposed)
					{
						return false;
					}
					List<T> list;
					lock (parent.gate)
					{
						if (currentFrame++ != parent.parent.frameCount)
						{
							return true;
						}
						if (parent.isCompleted)
						{
							return false;
						}
						list = parent.list;
						parent.list = new List<T>();
						parent.isRunning = false;
					}
					parent.observer.OnNext(list);
					return false;
				}

				public void Reset()
				{
					currentFrame = 0;
				}
			}

			private readonly BatchFrameObservable<T> parent;

			private readonly object gate = new object();

			private readonly BooleanDisposable cancellationToken = new BooleanDisposable();

			private readonly IEnumerator timer;

			private bool isRunning;

			private bool isCompleted;

			private List<T> list;

			public BatchFrame(BatchFrameObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				timer = new ReusableEnumerator(this);
			}

			public IDisposable Run()
			{
				list = new List<T>();
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, cancellationToken);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					if (!isCompleted)
					{
						list.Add(value);
						if (!isRunning)
						{
							isRunning = true;
							timer.Reset();
							switch (parent.frameCountType)
							{
							case FrameCountType.Update:
								MainThreadDispatcher.StartUpdateMicroCoroutine(timer);
								break;
							case FrameCountType.FixedUpdate:
								MainThreadDispatcher.StartFixedUpdateMicroCoroutine(timer);
								break;
							case FrameCountType.EndOfFrame:
								MainThreadDispatcher.StartEndOfFrameMicroCoroutine(timer);
								break;
							}
						}
					}
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
				List<T> list;
				lock (gate)
				{
					isCompleted = true;
					list = this.list;
				}
				if (list.Count != 0)
				{
					observer.OnNext(list);
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

		private readonly int frameCount;

		private readonly FrameCountType frameCountType;

		public BatchFrameObservable(UniRx.IObservable<T> source, int frameCount, FrameCountType frameCountType)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<T>> observer, IDisposable cancel)
		{
			return new BatchFrame(this, observer, cancel).Run();
		}
	}
	internal class BatchFrameObservable : OperatorObservableBase<Unit>
	{
		private class BatchFrame : OperatorObserverBase<Unit, Unit>
		{
			private class ReusableEnumerator : IEnumerator
			{
				private readonly BatchFrame parent;

				private int currentFrame;

				public object Current => null;

				public ReusableEnumerator(BatchFrame parent)
				{
					this.parent = parent;
				}

				public bool MoveNext()
				{
					if (parent.cancellationToken.IsDisposed)
					{
						return false;
					}
					lock (parent.gate)
					{
						if (currentFrame++ != parent.parent.frameCount)
						{
							return true;
						}
						if (parent.isCompleted)
						{
							return false;
						}
						parent.isRunning = false;
					}
					parent.observer.OnNext(Unit.Default);
					return false;
				}

				public void Reset()
				{
					currentFrame = 0;
				}
			}

			private readonly BatchFrameObservable parent;

			private readonly object gate = new object();

			private readonly BooleanDisposable cancellationToken = new BooleanDisposable();

			private readonly IEnumerator timer;

			private bool isRunning;

			private bool isCompleted;

			public BatchFrame(BatchFrameObservable parent, IObserver<Unit> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				timer = new ReusableEnumerator(this);
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.source.Subscribe(this);
				return StableCompositeDisposable.Create(disposable, cancellationToken);
			}

			public override void OnNext(Unit value)
			{
				lock (gate)
				{
					if (!isRunning)
					{
						isRunning = true;
						timer.Reset();
						switch (parent.frameCountType)
						{
						case FrameCountType.Update:
							MainThreadDispatcher.StartUpdateMicroCoroutine(timer);
							break;
						case FrameCountType.FixedUpdate:
							MainThreadDispatcher.StartFixedUpdateMicroCoroutine(timer);
							break;
						case FrameCountType.EndOfFrame:
							MainThreadDispatcher.StartEndOfFrameMicroCoroutine(timer);
							break;
						}
					}
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
				bool flag;
				lock (gate)
				{
					flag = isRunning;
					isCompleted = true;
				}
				if (flag)
				{
					observer.OnNext(Unit.Default);
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

		private readonly UniRx.IObservable<Unit> source;

		private readonly int frameCount;

		private readonly FrameCountType frameCountType;

		public BatchFrameObservable(UniRx.IObservable<Unit> source, int frameCount, FrameCountType frameCountType)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<Unit> observer, IDisposable cancel)
		{
			return new BatchFrame(this, observer, cancel).Run();
		}
	}
}
