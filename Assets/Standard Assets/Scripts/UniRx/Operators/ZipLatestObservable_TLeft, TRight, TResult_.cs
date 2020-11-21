using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ZipLatestObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
	{
		private class ZipLatest : OperatorObserverBase<TResult, TResult>
		{
			private class LeftObserver : IObserver<TLeft>
			{
				private readonly ZipLatest parent;

				public LeftObserver(ZipLatest parent)
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
				private readonly ZipLatest parent;

				public RightObserver(ZipLatest parent)
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

			private readonly ZipLatestObservable<TLeft, TRight, TResult> parent;

			private readonly object gate = new object();

			private TLeft leftValue = default(TLeft);

			private bool leftStarted;

			private bool leftCompleted;

			private TRight rightValue = default(TRight);

			private bool rightStarted;

			private bool rightCompleted;

			public ZipLatest(ZipLatestObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
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
					leftStarted = false;
					rightStarted = false;
					if (leftCompleted || rightCompleted)
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

		public ZipLatestObservable(UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
			: base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
		{
			this.left = left;
			this.right = right;
			this.selector = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return new ZipLatest(this, observer, cancel).Run();
		}
	}
	internal class ZipLatestObservable<T> : OperatorObservableBase<IList<T>>
	{
		private class ZipLatest : OperatorObserverBase<IList<T>, IList<T>>
		{
			private class ZipLatestObserver : IObserver<T>
			{
				private readonly ZipLatest parent;

				private readonly int index;

				public ZipLatestObserver(ZipLatest parent, int index)
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

			private readonly ZipLatestObservable<T> parent;

			private readonly object gate = new object();

			private int length;

			private T[] values;

			private bool[] isStarted;

			private bool[] isCompleted;

			public ZipLatest(ZipLatestObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
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
				IDisposable[] array = new IDisposable[length];
				for (int i = 0; i < length; i++)
				{
					UniRx.IObservable<T> observable = parent.sources[i];
					array[i] = observable.Subscribe(new ZipLatestObserver(this, i));
				}
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			private void Publish(int index)
			{
				isStarted[index] = true;
				bool flag = false;
				bool flag2 = true;
				for (int i = 0; i < length; i++)
				{
					if (!isStarted[i])
					{
						flag2 = false;
						break;
					}
					if (i != index && isCompleted[i])
					{
						flag = true;
					}
				}
				if (flag2)
				{
					OnNext(new List<T>(values));
					if (flag)
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
					else
					{
						Array.Clear(isStarted, 0, length);
					}
					return;
				}
				int num = 0;
				while (true)
				{
					if (num < length)
					{
						if (num != index && isCompleted[num] && !isStarted[num])
						{
							break;
						}
						num++;
						continue;
					}
					return;
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

		public ZipLatestObservable(UniRx.IObservable<T>[] sources)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<T>> observer, IDisposable cancel)
		{
			return new ZipLatest(this, observer, cancel).Run();
		}
	}
	internal class ZipLatestObservable<T1, T2, T3, TR> : OperatorObservableBase<TR>
	{
		private class ZipLatest : NthZipLatestObserverBase<TR>
		{
			private readonly ZipLatestObservable<T1, T2, T3, TR> parent;

			private readonly object gate = new object();

			private ZipLatestObserver<T1> c1;

			private ZipLatestObserver<T2> c2;

			private ZipLatestObserver<T3> c3;

			public ZipLatest(int length, ZipLatestObservable<T1, T2, T3, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new ZipLatestObserver<T1>(gate, this, 0);
				c2 = new ZipLatestObserver<T2>(gate, this, 1);
				c3 = new ZipLatestObserver<T3>(gate, this, 2);
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

		private ZipLatestFunc<T1, T2, T3, TR> resultSelector;

		public ZipLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, ZipLatestFunc<T1, T2, T3, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new ZipLatest(3, this, observer, cancel).Run();
		}
	}
	internal class ZipLatestObservable<T1, T2, T3, T4, TR> : OperatorObservableBase<TR>
	{
		private class ZipLatest : NthZipLatestObserverBase<TR>
		{
			private readonly ZipLatestObservable<T1, T2, T3, T4, TR> parent;

			private readonly object gate = new object();

			private ZipLatestObserver<T1> c1;

			private ZipLatestObserver<T2> c2;

			private ZipLatestObserver<T3> c3;

			private ZipLatestObserver<T4> c4;

			public ZipLatest(int length, ZipLatestObservable<T1, T2, T3, T4, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new ZipLatestObserver<T1>(gate, this, 0);
				c2 = new ZipLatestObserver<T2>(gate, this, 1);
				c3 = new ZipLatestObserver<T3>(gate, this, 2);
				c4 = new ZipLatestObserver<T4>(gate, this, 3);
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

		private ZipLatestFunc<T1, T2, T3, T4, TR> resultSelector;

		public ZipLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, ZipLatestFunc<T1, T2, T3, T4, TR> resultSelector)
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
			return new ZipLatest(4, this, observer, cancel).Run();
		}
	}
	internal class ZipLatestObservable<T1, T2, T3, T4, T5, TR> : OperatorObservableBase<TR>
	{
		private class ZipLatest : NthZipLatestObserverBase<TR>
		{
			private readonly ZipLatestObservable<T1, T2, T3, T4, T5, TR> parent;

			private readonly object gate = new object();

			private ZipLatestObserver<T1> c1;

			private ZipLatestObserver<T2> c2;

			private ZipLatestObserver<T3> c3;

			private ZipLatestObserver<T4> c4;

			private ZipLatestObserver<T5> c5;

			public ZipLatest(int length, ZipLatestObservable<T1, T2, T3, T4, T5, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new ZipLatestObserver<T1>(gate, this, 0);
				c2 = new ZipLatestObserver<T2>(gate, this, 1);
				c3 = new ZipLatestObserver<T3>(gate, this, 2);
				c4 = new ZipLatestObserver<T4>(gate, this, 3);
				c5 = new ZipLatestObserver<T5>(gate, this, 4);
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

		private ZipLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector;

		public ZipLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, ZipLatestFunc<T1, T2, T3, T4, T5, TR> resultSelector)
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
			return new ZipLatest(5, this, observer, cancel).Run();
		}
	}
	internal class ZipLatestObservable<T1, T2, T3, T4, T5, T6, TR> : OperatorObservableBase<TR>
	{
		private class ZipLatest : NthZipLatestObserverBase<TR>
		{
			private readonly ZipLatestObservable<T1, T2, T3, T4, T5, T6, TR> parent;

			private readonly object gate = new object();

			private ZipLatestObserver<T1> c1;

			private ZipLatestObserver<T2> c2;

			private ZipLatestObserver<T3> c3;

			private ZipLatestObserver<T4> c4;

			private ZipLatestObserver<T5> c5;

			private ZipLatestObserver<T6> c6;

			public ZipLatest(int length, ZipLatestObservable<T1, T2, T3, T4, T5, T6, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new ZipLatestObserver<T1>(gate, this, 0);
				c2 = new ZipLatestObserver<T2>(gate, this, 1);
				c3 = new ZipLatestObserver<T3>(gate, this, 2);
				c4 = new ZipLatestObserver<T4>(gate, this, 3);
				c5 = new ZipLatestObserver<T5>(gate, this, 4);
				c6 = new ZipLatestObserver<T6>(gate, this, 5);
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

		private ZipLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector;

		public ZipLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, ZipLatestFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
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
			return new ZipLatest(6, this, observer, cancel).Run();
		}
	}
	internal class ZipLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> : OperatorObservableBase<TR>
	{
		private class ZipLatest : NthZipLatestObserverBase<TR>
		{
			private readonly ZipLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent;

			private readonly object gate = new object();

			private ZipLatestObserver<T1> c1;

			private ZipLatestObserver<T2> c2;

			private ZipLatestObserver<T3> c3;

			private ZipLatestObserver<T4> c4;

			private ZipLatestObserver<T5> c5;

			private ZipLatestObserver<T6> c6;

			private ZipLatestObserver<T7> c7;

			public ZipLatest(int length, ZipLatestObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(length, observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				c1 = new ZipLatestObserver<T1>(gate, this, 0);
				c2 = new ZipLatestObserver<T2>(gate, this, 1);
				c3 = new ZipLatestObserver<T3>(gate, this, 2);
				c4 = new ZipLatestObserver<T4>(gate, this, 3);
				c5 = new ZipLatestObserver<T5>(gate, this, 4);
				c6 = new ZipLatestObserver<T6>(gate, this, 5);
				c7 = new ZipLatestObserver<T7>(gate, this, 6);
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

		private ZipLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector;

		public ZipLatestObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, UniRx.IObservable<T7> source7, ZipLatestFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
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
			return new ZipLatest(7, this, observer, cancel).Run();
		}
	}
}
