using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class CombineLatestObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
	{
		private class CombineLatest : OperatorObserverBase<TResult, TResult>
		{
			private class LeftObserver : IObserver<TLeft>
			{
				private readonly CombineLatest parent;

				public LeftObserver(CombineLatest parent)
				{
					this.parent = parent;
				}

				public void OnNext(TLeft value)
				{
					lock (parent.gate)
					{
						parent.leftStarted = true;
						parent.leftValue = value;
						parent.Publish();
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						parent.OnError(error);
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						parent.leftCompleted = true;
						if (parent.rightCompleted)
						{
							parent.OnCompleted();
						}
					}
				}
			}

			private class RightObserver : IObserver<TRight>
			{
				private readonly CombineLatest parent;

				public RightObserver(CombineLatest parent)
				{
					this.parent = parent;
				}

				public void OnNext(TRight value)
				{
					lock (parent.gate)
					{
						parent.rightStarted = true;
						parent.rightValue = value;
						parent.Publish();
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						parent.OnError(error);
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						parent.rightCompleted = true;
						if (parent.leftCompleted)
						{
							parent.OnCompleted();
						}
					}
				}
			}

			private readonly CombineLatestObservable<TLeft, TRight, TResult> parent;

			private readonly object gate = new object();

			private TLeft leftValue = default(TLeft);

			private bool leftStarted;

			private bool leftCompleted;

			private TRight rightValue = default(TRight);

			private bool rightStarted;

			private bool rightCompleted;

			public CombineLatest(CombineLatestObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.left.Subscribe(new LeftObserver(this));
				IDisposable disposable2 = parent.right.Subscribe(new RightObserver(this));
				return StableCompositeDisposable.Create(disposable, disposable2);
			}

			public void Publish()
			{
				if ((leftCompleted && !leftStarted) || (rightCompleted && !rightStarted))
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
				else if (leftStarted && rightStarted)
				{
					TResult value;
					try
					{
						value = parent.selector(leftValue, rightValue);
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
						return;
					}
					OnNext(value);
				}
			}

			public override void OnNext(TResult value)
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

		private readonly UniRx.IObservable<TLeft> left;

		private readonly UniRx.IObservable<TRight> right;

		private readonly Func<TLeft, TRight, TResult> selector;

		public CombineLatestObservable(UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
			: base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
		{
			this.left = left;
			this.right = right;
			this.selector = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return new CombineLatest(this, observer, cancel).Run();
		}
	}
	internal class CombineLatestObservable<T> : OperatorObservableBase<IList<T>>
	{
		private class CombineLatest : OperatorObserverBase<IList<T>, IList<T>>
		{
			private class CombineLatestObserver : IObserver<T>
			{
				private readonly CombineLatest parent;

				private readonly int index;

				public CombineLatestObserver(CombineLatest parent, int index)
				{
					this.parent = parent;
					this.index = index;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						parent.values[index] = value;
						parent.Publish(index);
					}
				}

				public void OnError(Exception ex)
				{
					lock (parent.gate)
					{
						parent.OnError(ex);
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						parent.isCompleted[index] = true;
						bool flag = true;
						for (int i = 0; i < parent.length; i++)
						{
							if (!parent.isCompleted[i])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							parent.OnCompleted();
						}
					}
				}
			}

			private readonly CombineLatestObservable<T> parent;

			private readonly object gate = new object();

			private int length;

			private T[] values;

			private bool[] isStarted;

			private bool[] isCompleted;

			private bool isAllValueStarted;

			public CombineLatest(CombineLatestObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				length = parent.sources.Length;
				values = new T[length];
				isStarted = new bool[length];
				isCompleted = new bool[length];
				isAllValueStarted = false;
				IDisposable[] array = new IDisposable[length];
				for (int i = 0; i < length; i++)
				{
					UniRx.IObservable<T> observable = parent.sources[i];
					array[i] = observable.Subscribe(new CombineLatestObserver(this, i));
				}
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			private void Publish(int index)
			{
				isStarted[index] = true;
				if (isAllValueStarted)
				{
					OnNext(new List<T>(values));
					return;
				}
				bool flag = true;
				for (int i = 0; i < length; i++)
				{
					if (!isStarted[i])
					{
						flag = false;
						break;
					}
				}
				isAllValueStarted = flag;
				if (isAllValueStarted)
				{
					OnNext(new List<T>(values));
					return;
				}
				bool flag2 = true;
				for (int j = 0; j < length; j++)
				{
					if (j != index && !isCompleted[j])
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
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

			public override void OnNext(IList<T> value)
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

		public CombineLatestObservable(UniRx.IObservable<T>[] sources)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<T>> observer, IDisposable cancel)
		{
			return new CombineLatest(this, observer, cancel).Run();
		}
	}
	internal class CombineLatestObservable<T1, T2, T3, TR> : OperatorObservableBase<TR>
	{
		private class CombineLatest : NthCombineLatestObserverBase<TR>
		{
			private readonly CombineLatestObservable<T1, T2, T3, TR> parent;

			private readonly object gate = new object();

			private CombineLatestObserver<T1> c1;

			private CombineLatestObserver<T2> c2;

			private CombineLatestObserver<T3> c3;

			public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new CombineLatestObserver<T1>(gate, this, 0);
				c2 = new CombineLatestObserver<T2>(gate, this, 1);
				c3 = new CombineLatestObserver<T3>(gate, this, 2);
				IDisposable disposable = parent.source1.Subscribe(c1);
				IDisposable disposable2 = parent.source2.Subscribe(c2);
				IDisposable disposable3 = parent.source3.Subscribe(c3);
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3);
			}

			public override TR GetResult()
			{
				return parent.resultSelector(c1.Value, c2.Value, c3.Value);
			}

			public override void OnNext(TR value)
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

		private UniRx.IObservable<T1> source1;

		private UniRx.IObservable<T2> source2;

		private UniRx.IObservable<T3> source3;

		private CombineLatestFunc<T1, T2, T3, TR> resultSelector;

		public CombineLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, CombineLatestFunc<T1, T2, T3, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new CombineLatest(3, this, observer, cancel).Run();
		}
	}
	internal class CombineLatestObservable<T1, T2, T3, T4, TR> : OperatorObservableBase<TR>
	{
		private class CombineLatest : NthCombineLatestObserverBase<TR>
		{
			private readonly CombineLatestObservable<T1, T2, T3, T4, TR> parent;

			private readonly object gate = new object();

			private CombineLatestObserver<T1> c1;

			private CombineLatestObserver<T2> c2;

			private CombineLatestObserver<T3> c3;

			private CombineLatestObserver<T4> c4;

			public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new CombineLatestObserver<T1>(gate, this, 0);
				c2 = new CombineLatestObserver<T2>(gate, this, 1);
				c3 = new CombineLatestObserver<T3>(gate, this, 2);
				c4 = new CombineLatestObserver<T4>(gate, this, 3);
				IDisposable disposable = parent.source1.Subscribe(c1);
				IDisposable disposable2 = parent.source2.Subscribe(c2);
				IDisposable disposable3 = parent.source3.Subscribe(c3);
				IDisposable disposable4 = parent.source4.Subscribe(c4);
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4);
			}

			public override TR GetResult()
			{
				return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value);
			}

			public override void OnNext(TR value)
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

		private UniRx.IObservable<T1> source1;

		private UniRx.IObservable<T2> source2;

		private UniRx.IObservable<T3> source3;

		private UniRx.IObservable<T4> source4;

		private CombineLatestFunc<T1, T2, T3, T4, TR> resultSelector;

		public CombineLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, CombineLatestFunc<T1, T2, T3, T4, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread() || source4.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.source4 = source4;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new CombineLatest(4, this, observer, cancel).Run();
		}
	}
	internal class CombineLatestObservable<T1, T2, T3, T4, T5, TR> : OperatorObservableBase<TR>
	{
		private class CombineLatest : NthCombineLatestObserverBase<TR>
		{
			private readonly CombineLatestObservable<T1, T2, T3, T4, T5, TR> parent;

			private readonly object gate = new object();

			private CombineLatestObserver<T1> c1;

			private CombineLatestObserver<T2> c2;

			private CombineLatestObserver<T3> c3;

			private CombineLatestObserver<T4> c4;

			private CombineLatestObserver<T5> c5;

			public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, T5, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new CombineLatestObserver<T1>(gate, this, 0);
				c2 = new CombineLatestObserver<T2>(gate, this, 1);
				c3 = new CombineLatestObserver<T3>(gate, this, 2);
				c4 = new CombineLatestObserver<T4>(gate, this, 3);
				c5 = new CombineLatestObserver<T5>(gate, this, 4);
				IDisposable disposable = parent.source1.Subscribe(c1);
				IDisposable disposable2 = parent.source2.Subscribe(c2);
				IDisposable disposable3 = parent.source3.Subscribe(c3);
				IDisposable disposable4 = parent.source4.Subscribe(c4);
				IDisposable disposable5 = parent.source5.Subscribe(c5);
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, disposable5);
			}

			public override TR GetResult()
			{
				return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value);
			}

			public override void OnNext(TR value)
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

		private UniRx.IObservable<T1> source1;

		private UniRx.IObservable<T2> source2;

		private UniRx.IObservable<T3> source3;

		private UniRx.IObservable<T4> source4;

		private UniRx.IObservable<T5> source5;

		private CombineLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector;

		public CombineLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, CombineLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread() || source4.IsRequiredSubscribeOnCurrentThread() || source5.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.source4 = source4;
			this.source5 = source5;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new CombineLatest(5, this, observer, cancel).Run();
		}
	}
	internal class CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR> : OperatorObservableBase<TR>
	{
		private class CombineLatest : NthCombineLatestObserverBase<TR>
		{
			private readonly CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR> parent;

			private readonly object gate = new object();

			private CombineLatestObserver<T1> c1;

			private CombineLatestObserver<T2> c2;

			private CombineLatestObserver<T3> c3;

			private CombineLatestObserver<T4> c4;

			private CombineLatestObserver<T5> c5;

			private CombineLatestObserver<T6> c6;

			public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, T5, T6, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new CombineLatestObserver<T1>(gate, this, 0);
				c2 = new CombineLatestObserver<T2>(gate, this, 1);
				c3 = new CombineLatestObserver<T3>(gate, this, 2);
				c4 = new CombineLatestObserver<T4>(gate, this, 3);
				c5 = new CombineLatestObserver<T5>(gate, this, 4);
				c6 = new CombineLatestObserver<T6>(gate, this, 5);
				IDisposable disposable = parent.source1.Subscribe(c1);
				IDisposable disposable2 = parent.source2.Subscribe(c2);
				IDisposable disposable3 = parent.source3.Subscribe(c3);
				IDisposable disposable4 = parent.source4.Subscribe(c4);
				IDisposable disposable5 = parent.source5.Subscribe(c5);
				IDisposable disposable6 = parent.source6.Subscribe(c6);
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, disposable5, disposable6);
			}

			public override TR GetResult()
			{
				return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value, c6.Value);
			}

			public override void OnNext(TR value)
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

		private UniRx.IObservable<T1> source1;

		private UniRx.IObservable<T2> source2;

		private UniRx.IObservable<T3> source3;

		private UniRx.IObservable<T4> source4;

		private UniRx.IObservable<T5> source5;

		private UniRx.IObservable<T6> source6;

		private CombineLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector;

		public CombineLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, CombineLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread() || source4.IsRequiredSubscribeOnCurrentThread() || source5.IsRequiredSubscribeOnCurrentThread() || source6.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.source4 = source4;
			this.source5 = source5;
			this.source6 = source6;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new CombineLatest(6, this, observer, cancel).Run();
		}
	}
	internal class CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> : OperatorObservableBase<TR>
	{
		private class CombineLatest : NthCombineLatestObserverBase<TR>
		{
			private readonly CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent;

			private readonly object gate = new object();

			private CombineLatestObserver<T1> c1;

			private CombineLatestObserver<T2> c2;

			private CombineLatestObserver<T3> c3;

			private CombineLatestObserver<T4> c4;

			private CombineLatestObserver<T5> c5;

			private CombineLatestObserver<T6> c6;

			private CombineLatestObserver<T7> c7;

			public CombineLatest(int length, CombineLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new CombineLatestObserver<T1>(gate, this, 0);
				c2 = new CombineLatestObserver<T2>(gate, this, 1);
				c3 = new CombineLatestObserver<T3>(gate, this, 2);
				c4 = new CombineLatestObserver<T4>(gate, this, 3);
				c5 = new CombineLatestObserver<T5>(gate, this, 4);
				c6 = new CombineLatestObserver<T6>(gate, this, 5);
				c7 = new CombineLatestObserver<T7>(gate, this, 6);
				IDisposable disposable = parent.source1.Subscribe(c1);
				IDisposable disposable2 = parent.source2.Subscribe(c2);
				IDisposable disposable3 = parent.source3.Subscribe(c3);
				IDisposable disposable4 = parent.source4.Subscribe(c4);
				IDisposable disposable5 = parent.source5.Subscribe(c5);
				IDisposable disposable6 = parent.source6.Subscribe(c6);
				IDisposable disposable7 = parent.source7.Subscribe(c7);
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7);
			}

			public override TR GetResult()
			{
				return parent.resultSelector(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value, c6.Value, c7.Value);
			}

			public override void OnNext(TR value)
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

		private UniRx.IObservable<T1> source1;

		private UniRx.IObservable<T2> source2;

		private UniRx.IObservable<T3> source3;

		private UniRx.IObservable<T4> source4;

		private UniRx.IObservable<T5> source5;

		private UniRx.IObservable<T6> source6;

		private UniRx.IObservable<T7> source7;

		private CombineLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector;

		public CombineLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, UniRx.IObservable<T7> source7, CombineLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread() || source4.IsRequiredSubscribeOnCurrentThread() || source5.IsRequiredSubscribeOnCurrentThread() || source6.IsRequiredSubscribeOnCurrentThread() || source7.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.source4 = source4;
			this.source5 = source5;
			this.source6 = source6;
			this.source7 = source7;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new CombineLatest(7, this, observer, cancel).Run();
		}
	}
}
