using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class ZipObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
	{
		private class Zip : OperatorObserverBase<TResult, TResult>
		{
			private class LeftZipObserver : IObserver<TLeft>
			{
				private readonly Zip parent;

				public LeftZipObserver(Zip parent)
				{
					this.parent = parent;
				}

				public void OnNext(TLeft value)
				{
					lock (parent.gate)
					{
						parent.leftQ.Enqueue(value);
						parent.Dequeue();
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
						parent.leftCompleted = true;
						if (parent.rightCompleted)
						{
							parent.OnCompleted();
						}
					}
				}
			}

			private class RightZipObserver : IObserver<TRight>
			{
				private readonly Zip parent;

				public RightZipObserver(Zip parent)
				{
					this.parent = parent;
				}

				public void OnNext(TRight value)
				{
					lock (parent.gate)
					{
						parent.rightQ.Enqueue(value);
						parent.Dequeue();
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
						parent.rightCompleted = true;
						if (parent.leftCompleted)
						{
							parent.OnCompleted();
						}
					}
				}
			}

			private readonly ZipObservable<TLeft, TRight, TResult> parent;

			private readonly object gate = new object();

			private readonly Queue<TLeft> leftQ = new Queue<TLeft>();

			private bool leftCompleted;

			private readonly Queue<TRight> rightQ = new Queue<TRight>();

			private bool rightCompleted;

			public Zip(ZipObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable disposable = parent.left.Subscribe(new LeftZipObserver(this));
				IDisposable disposable2 = parent.right.Subscribe(new RightZipObserver(this));
				return StableCompositeDisposable.Create(disposable, disposable2, Disposable.Create(delegate
				{
					lock (gate)
					{
						leftQ.Clear();
						rightQ.Clear();
					}
				}));
			}

			private void Dequeue()
			{
				if (leftQ.Count != 0 && rightQ.Count != 0)
				{
					TLeft arg = leftQ.Dequeue();
					TRight arg2 = rightQ.Dequeue();
					TResult value;
					try
					{
						value = parent.selector(arg, arg2);
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
				else if (leftCompleted || rightCompleted)
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

		public ZipObservable(UniRx.IObservable<TLeft> left, UniRx.IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
			: base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
		{
			this.left = left;
			this.right = right;
			this.selector = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			return new Zip(this, observer, cancel).Run();
		}
	}
	internal class ZipObservable<T> : OperatorObservableBase<IList<T>>
	{
		private class Zip : OperatorObserverBase<IList<T>, IList<T>>
		{
			private class ZipObserver : IObserver<T>
			{
				private readonly Zip parent;

				private readonly int index;

				public ZipObserver(Zip parent, int index)
				{
					this.parent = parent;
					this.index = index;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						parent.queues[index].Enqueue(value);
						parent.Dequeue(index);
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
						parent.isDone[index] = true;
						bool flag = true;
						for (int i = 0; i < parent.length; i++)
						{
							if (!parent.isDone[i])
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

			private readonly ZipObservable<T> parent;

			private readonly object gate = new object();

			private Queue<T>[] queues;

			private bool[] isDone;

			private int length;

			public Zip(ZipObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				length = parent.sources.Length;
				queues = new Queue<T>[length];
				isDone = new bool[length];
				for (int i = 0; i < length; i++)
				{
					queues[i] = new Queue<T>();
				}
				IDisposable[] array = new IDisposable[length + 1];
				for (int j = 0; j < length; j++)
				{
					UniRx.IObservable<T> observable = parent.sources[j];
					array[j] = observable.Subscribe(new ZipObserver(this, j));
				}
				array[length] = Disposable.Create(delegate
				{
					lock (gate)
					{
						for (int k = 0; k < length; k++)
						{
							Queue<T> queue = queues[k];
							queue.Clear();
						}
					}
				});
				return StableCompositeDisposable.CreateUnsafe(array);
			}

			private void Dequeue(int index)
			{
				bool flag = true;
				for (int i = 0; i < length; i++)
				{
					if (queues[i].Count == 0)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					bool flag2 = true;
					for (int j = 0; j < length; j++)
					{
						if (j != index && !isDone[j])
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
				else
				{
					T[] array = new T[length];
					for (int k = 0; k < length; k++)
					{
						array[k] = queues[k].Dequeue();
					}
					OnNext(array);
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

		public ZipObservable(UniRx.IObservable<T>[] sources)
			: base(isRequiredSubscribeOnCurrentThread: true)
		{
			this.sources = sources;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<IList<T>> observer, IDisposable cancel)
		{
			return new Zip(this, observer, cancel).Run();
		}
	}
	internal class ZipObservable<T1, T2, T3, TR> : OperatorObservableBase<TR>
	{
		private class Zip : NthZipObserverBase<TR>
		{
			private readonly ZipObservable<T1, T2, T3, TR> parent;

			private readonly object gate = new object();

			private readonly Queue<T1> q1 = new Queue<T1>();

			private readonly Queue<T2> q2 = new Queue<T2>();

			private readonly Queue<T3> q3 = new Queue<T3>();

			public Zip(ZipObservable<T1, T2, T3, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SetQueue(new ICollection[3]
				{
					q1,
					q2,
					q3
				});
				IDisposable disposable = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
				IDisposable disposable2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
				IDisposable disposable3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, Disposable.Create(delegate
				{
					lock (gate)
					{
						q1.Clear();
						q2.Clear();
						q3.Clear();
					}
				}));
			}

			public override TR GetResult()
			{
				return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue());
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

		private ZipFunc<T1, T2, T3, TR> resultSelector;

		public ZipObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, ZipFunc<T1, T2, T3, TR> resultSelector)
			: base((source1.IsRequiredSubscribeOnCurrentThread() || source2.IsRequiredSubscribeOnCurrentThread() || source3.IsRequiredSubscribeOnCurrentThread()) ? true : false)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TR> observer, IDisposable cancel)
		{
			return new Zip(this, observer, cancel).Run();
		}
	}
	internal class ZipObservable<T1, T2, T3, T4, TR> : OperatorObservableBase<TR>
	{
		private class Zip : NthZipObserverBase<TR>
		{
			private readonly ZipObservable<T1, T2, T3, T4, TR> parent;

			private readonly object gate = new object();

			private readonly Queue<T1> q1 = new Queue<T1>();

			private readonly Queue<T2> q2 = new Queue<T2>();

			private readonly Queue<T3> q3 = new Queue<T3>();

			private readonly Queue<T4> q4 = new Queue<T4>();

			public Zip(ZipObservable<T1, T2, T3, T4, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SetQueue(new ICollection[4]
				{
					q1,
					q2,
					q3,
					q4
				});
				IDisposable disposable = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
				IDisposable disposable2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
				IDisposable disposable3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
				IDisposable disposable4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, Disposable.Create(delegate
				{
					lock (gate)
					{
						q1.Clear();
						q2.Clear();
						q3.Clear();
						q4.Clear();
					}
				}));
			}

			public override TR GetResult()
			{
				return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue());
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

		private ZipFunc<T1, T2, T3, T4, TR> resultSelector;

		public ZipObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, ZipFunc<T1, T2, T3, T4, TR> resultSelector)
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
			return new Zip(this, observer, cancel).Run();
		}
	}
	internal class ZipObservable<T1, T2, T3, T4, T5, TR> : OperatorObservableBase<TR>
	{
		private class Zip : NthZipObserverBase<TR>
		{
			private readonly ZipObservable<T1, T2, T3, T4, T5, TR> parent;

			private readonly object gate = new object();

			private readonly Queue<T1> q1 = new Queue<T1>();

			private readonly Queue<T2> q2 = new Queue<T2>();

			private readonly Queue<T3> q3 = new Queue<T3>();

			private readonly Queue<T4> q4 = new Queue<T4>();

			private readonly Queue<T5> q5 = new Queue<T5>();

			public Zip(ZipObservable<T1, T2, T3, T4, T5, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SetQueue(new ICollection[5]
				{
					q1,
					q2,
					q3,
					q4,
					q5
				});
				IDisposable disposable = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
				IDisposable disposable2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
				IDisposable disposable3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
				IDisposable disposable4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
				IDisposable disposable5 = parent.source5.Subscribe(new ZipObserver<T5>(gate, this, 4, q5));
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, disposable5, Disposable.Create(delegate
				{
					lock (gate)
					{
						q1.Clear();
						q2.Clear();
						q3.Clear();
						q4.Clear();
						q5.Clear();
					}
				}));
			}

			public override TR GetResult()
			{
				return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue(), q5.Dequeue());
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

		private ZipFunc<T1, T2, T3, T4, T5, TR> resultSelector;

		public ZipObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, ZipFunc<T1, T2, T3, T4, T5, TR> resultSelector)
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
			return new Zip(this, observer, cancel).Run();
		}
	}
	internal class ZipObservable<T1, T2, T3, T4, T5, T6, TR> : OperatorObservableBase<TR>
	{
		private class Zip : NthZipObserverBase<TR>
		{
			private readonly ZipObservable<T1, T2, T3, T4, T5, T6, TR> parent;

			private readonly object gate = new object();

			private readonly Queue<T1> q1 = new Queue<T1>();

			private readonly Queue<T2> q2 = new Queue<T2>();

			private readonly Queue<T3> q3 = new Queue<T3>();

			private readonly Queue<T4> q4 = new Queue<T4>();

			private readonly Queue<T5> q5 = new Queue<T5>();

			private readonly Queue<T6> q6 = new Queue<T6>();

			public Zip(ZipObservable<T1, T2, T3, T4, T5, T6, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SetQueue(new ICollection[6]
				{
					q1,
					q2,
					q3,
					q4,
					q5,
					q6
				});
				IDisposable disposable = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
				IDisposable disposable2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
				IDisposable disposable3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
				IDisposable disposable4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
				IDisposable disposable5 = parent.source5.Subscribe(new ZipObserver<T5>(gate, this, 4, q5));
				IDisposable disposable6 = parent.source6.Subscribe(new ZipObserver<T6>(gate, this, 5, q6));
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, disposable5, disposable6, Disposable.Create(delegate
				{
					lock (gate)
					{
						q1.Clear();
						q2.Clear();
						q3.Clear();
						q4.Clear();
						q5.Clear();
						q6.Clear();
					}
				}));
			}

			public override TR GetResult()
			{
				return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue(), q5.Dequeue(), q6.Dequeue());
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

		private ZipFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector;

		public ZipObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, ZipFunc<T1, T2, T3, T4, T5, T6, TR> resultSelector)
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
			return new Zip(this, observer, cancel).Run();
		}
	}
	internal class ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR> : OperatorObservableBase<TR>
	{
		private class Zip : NthZipObserverBase<TR>
		{
			private readonly ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent;

			private readonly object gate = new object();

			private readonly Queue<T1> q1 = new Queue<T1>();

			private readonly Queue<T2> q2 = new Queue<T2>();

			private readonly Queue<T3> q3 = new Queue<T3>();

			private readonly Queue<T4> q4 = new Queue<T4>();

			private readonly Queue<T5> q5 = new Queue<T5>();

			private readonly Queue<T6> q6 = new Queue<T6>();

			private readonly Queue<T7> q7 = new Queue<T7>();

			public Zip(ZipObservable<T1, T2, T3, T4, T5, T6, T7, TR> parent, IObserver<TR> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SetQueue(new ICollection[7]
				{
					q1,
					q2,
					q3,
					q4,
					q5,
					q6,
					q7
				});
				IDisposable disposable = parent.source1.Subscribe(new ZipObserver<T1>(gate, this, 0, q1));
				IDisposable disposable2 = parent.source2.Subscribe(new ZipObserver<T2>(gate, this, 1, q2));
				IDisposable disposable3 = parent.source3.Subscribe(new ZipObserver<T3>(gate, this, 2, q3));
				IDisposable disposable4 = parent.source4.Subscribe(new ZipObserver<T4>(gate, this, 3, q4));
				IDisposable disposable5 = parent.source5.Subscribe(new ZipObserver<T5>(gate, this, 4, q5));
				IDisposable disposable6 = parent.source6.Subscribe(new ZipObserver<T6>(gate, this, 5, q6));
				IDisposable disposable7 = parent.source7.Subscribe(new ZipObserver<T7>(gate, this, 6, q7));
				return StableCompositeDisposable.Create(disposable, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, Disposable.Create(delegate
				{
					lock (gate)
					{
						q1.Clear();
						q2.Clear();
						q3.Clear();
						q4.Clear();
						q5.Clear();
						q6.Clear();
						q7.Clear();
					}
				}));
			}

			public override TR GetResult()
			{
				return parent.resultSelector(q1.Dequeue(), q2.Dequeue(), q3.Dequeue(), q4.Dequeue(), q5.Dequeue(), q6.Dequeue(), q7.Dequeue());
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

		private ZipFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector;

		public ZipObservable(UniRx.IObservable<T1> source1, UniRx.IObservable<T2> source2, UniRx.IObservable<T3> source3, UniRx.IObservable<T4> source4, UniRx.IObservable<T5> source5, UniRx.IObservable<T6> source6, UniRx.IObservable<T7> source7, ZipFunc<T1, T2, T3, T4, T5, T6, T7, TR> resultSelector)
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
			return new Zip(this, observer, cancel).Run();
		}
	}
}
