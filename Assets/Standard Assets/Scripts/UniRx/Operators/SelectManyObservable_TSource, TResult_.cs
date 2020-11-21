using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class SelectManyObservable<TSource, TResult> : OperatorObservableBase<TResult>
	{
		private class SelectManyOuterObserver : OperatorObserverBase<TSource, TResult>
		{
			private class SelectMany : OperatorObserverBase<TResult, TResult>
			{
				private readonly SelectManyOuterObserver parent;

				private readonly IDisposable cancel;

				public SelectMany(SelectManyOuterObserver parent, IDisposable cancel)
					: base(parent.observer, cancel)
				{
					this.parent = parent;
					this.cancel = cancel;
				}

				public override void OnNext(TResult value)
				{
					lock (parent.gate)
					{
						observer.OnNext(value);
					}
				}

				public override void OnError(Exception error)
				{
					lock (parent.gate)
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
				}

				public override void OnCompleted()
				{
					parent.collectionDisposable.Remove(cancel);
					if (parent.isStopped && parent.collectionDisposable.Count == 1)
					{
						lock (parent.gate)
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
			}

			private readonly SelectManyObservable<TSource, TResult> parent;

			private CompositeDisposable collectionDisposable;

			private SingleAssignmentDisposable sourceDisposable;

			private object gate = new object();

			private bool isStopped;

			public SelectManyOuterObserver(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				collectionDisposable = new CompositeDisposable();
				sourceDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(sourceDisposable);
				sourceDisposable.Disposable = parent.source.Subscribe(this);
				return collectionDisposable;
			}

			public override void OnNext(TSource value)
			{
				UniRx.IObservable<TResult> observable;
				try
				{
					observable = parent.selector(value);
				}
				catch (Exception error)
				{
					try
					{
						base.observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(singleAssignmentDisposable);
				SelectMany observer = new SelectMany(this, singleAssignmentDisposable);
				singleAssignmentDisposable.Disposable = observable.Subscribe(observer);
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				isStopped = true;
				if (collectionDisposable.Count == 1)
				{
					lock (gate)
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
					sourceDisposable.Dispose();
				}
			}
		}

		private class SelectManyObserverWithIndex : OperatorObserverBase<TSource, TResult>
		{
			private class SelectMany : OperatorObserverBase<TResult, TResult>
			{
				private readonly SelectManyObserverWithIndex parent;

				private readonly IDisposable cancel;

				public SelectMany(SelectManyObserverWithIndex parent, IDisposable cancel)
					: base(parent.observer, cancel)
				{
					this.parent = parent;
					this.cancel = cancel;
				}

				public override void OnNext(TResult value)
				{
					lock (parent.gate)
					{
						observer.OnNext(value);
					}
				}

				public override void OnError(Exception error)
				{
					lock (parent.gate)
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
				}

				public override void OnCompleted()
				{
					parent.collectionDisposable.Remove(cancel);
					if (parent.isStopped && parent.collectionDisposable.Count == 1)
					{
						lock (parent.gate)
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
			}

			private readonly SelectManyObservable<TSource, TResult> parent;

			private CompositeDisposable collectionDisposable;

			private int index;

			private object gate = new object();

			private bool isStopped;

			private SingleAssignmentDisposable sourceDisposable;

			public SelectManyObserverWithIndex(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				collectionDisposable = new CompositeDisposable();
				sourceDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(sourceDisposable);
				sourceDisposable.Disposable = parent.source.Subscribe(this);
				return collectionDisposable;
			}

			public override void OnNext(TSource value)
			{
				UniRx.IObservable<TResult> observable;
				try
				{
					observable = parent.selectorWithIndex(value, index++);
				}
				catch (Exception error)
				{
					try
					{
						base.observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
					return;
				}
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(singleAssignmentDisposable);
				SelectMany observer = new SelectMany(this, singleAssignmentDisposable);
				singleAssignmentDisposable.Disposable = observable.Subscribe(observer);
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				isStopped = true;
				if (collectionDisposable.Count == 1)
				{
					lock (gate)
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
					sourceDisposable.Dispose();
				}
			}
		}

		private class SelectManyEnumerableObserver : OperatorObserverBase<TSource, TResult>
		{
			private readonly SelectManyObservable<TSource, TResult> parent;

			public SelectManyEnumerableObserver(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(TSource value)
			{
				IEnumerable<TResult> enumerable;
				try
				{
					enumerable = parent.selectorEnumerable(value);
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
				using (IEnumerator<TResult> enumerator = enumerable.GetEnumerator())
				{
					bool flag = true;
					while (flag)
					{
						flag = false;
						TResult value2 = default(TResult);
						try
						{
							flag = enumerator.MoveNext();
							if (flag)
							{
								value2 = enumerator.Current;
							}
						}
						catch (Exception error2)
						{
							try
							{
								observer.OnError(error2);
							}
							finally
							{
								Dispose();
							}
							return;
						}
						if (flag)
						{
							observer.OnNext(value2);
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

		private class SelectManyEnumerableObserverWithIndex : OperatorObserverBase<TSource, TResult>
		{
			private readonly SelectManyObservable<TSource, TResult> parent;

			private int index;

			public SelectManyEnumerableObserverWithIndex(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(TSource value)
			{
				IEnumerable<TResult> enumerable;
				try
				{
					enumerable = parent.selectorEnumerableWithIndex(value, index++);
				}
				catch (Exception error)
				{
					OnError(error);
					return;
				}
				using (IEnumerator<TResult> enumerator = enumerable.GetEnumerator())
				{
					bool flag = true;
					while (flag)
					{
						flag = false;
						TResult value2 = default(TResult);
						try
						{
							flag = enumerator.MoveNext();
							if (flag)
							{
								value2 = enumerator.Current;
							}
						}
						catch (Exception error2)
						{
							try
							{
								observer.OnError(error2);
							}
							finally
							{
								Dispose();
							}
							return;
						}
						if (flag)
						{
							observer.OnNext(value2);
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

		private readonly UniRx.IObservable<TSource> source;

		private readonly Func<TSource, UniRx.IObservable<TResult>> selector;

		private readonly Func<TSource, int, UniRx.IObservable<TResult>> selectorWithIndex;

		private readonly Func<TSource, IEnumerable<TResult>> selectorEnumerable;

		private readonly Func<TSource, int, IEnumerable<TResult>> selectorEnumerableWithIndex;

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, UniRx.IObservable<TResult>> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.selector = selector;
		}

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, int, UniRx.IObservable<TResult>> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			selectorWithIndex = selector;
		}

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			selectorEnumerable = selector;
		}

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			selectorEnumerableWithIndex = selector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			if (selector != null)
			{
				return new SelectManyOuterObserver(this, observer, cancel).Run();
			}
			if (selectorWithIndex != null)
			{
				return new SelectManyObserverWithIndex(this, observer, cancel).Run();
			}
			if (selectorEnumerable != null)
			{
				return new SelectManyEnumerableObserver(this, observer, cancel).Run();
			}
			if (selectorEnumerableWithIndex != null)
			{
				return new SelectManyEnumerableObserverWithIndex(this, observer, cancel).Run();
			}
			throw new InvalidOperationException();
		}
	}
	internal class SelectManyObservable<TSource, TCollection, TResult> : OperatorObservableBase<TResult>
	{
		private class SelectManyOuterObserver : OperatorObserverBase<TSource, TResult>
		{
			private class SelectMany : OperatorObserverBase<TCollection, TResult>
			{
				private readonly SelectManyOuterObserver parent;

				private readonly TSource sourceValue;

				private readonly IDisposable cancel;

				public SelectMany(SelectManyOuterObserver parent, TSource value, IDisposable cancel)
					: base(parent.observer, cancel)
				{
					this.parent = parent;
					sourceValue = value;
					this.cancel = cancel;
				}

				public override void OnNext(TCollection value)
				{
					TResult value2;
					try
					{
						value2 = parent.parent.resultSelector(sourceValue, value);
					}
					catch (Exception error)
					{
						OnError(error);
						return;
					}
					lock (parent.gate)
					{
						observer.OnNext(value2);
					}
				}

				public override void OnError(Exception error)
				{
					lock (parent.gate)
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
				}

				public override void OnCompleted()
				{
					parent.collectionDisposable.Remove(cancel);
					if (parent.isStopped && parent.collectionDisposable.Count == 1)
					{
						lock (parent.gate)
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
			}

			private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

			private CompositeDisposable collectionDisposable;

			private object gate = new object();

			private bool isStopped;

			private SingleAssignmentDisposable sourceDisposable;

			public SelectManyOuterObserver(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				collectionDisposable = new CompositeDisposable();
				sourceDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(sourceDisposable);
				sourceDisposable.Disposable = parent.source.Subscribe(this);
				return collectionDisposable;
			}

			public override void OnNext(TSource value)
			{
				UniRx.IObservable<TCollection> observable;
				try
				{
					observable = parent.collectionSelector(value);
				}
				catch (Exception error)
				{
					OnError(error);
					return;
				}
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(singleAssignmentDisposable);
				SelectMany observer = new SelectMany(this, value, singleAssignmentDisposable);
				singleAssignmentDisposable.Disposable = observable.Subscribe(observer);
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				isStopped = true;
				if (collectionDisposable.Count == 1)
				{
					lock (gate)
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
					sourceDisposable.Dispose();
				}
			}
		}

		private class SelectManyObserverWithIndex : OperatorObserverBase<TSource, TResult>
		{
			private class SelectManyObserver : OperatorObserverBase<TCollection, TResult>
			{
				private readonly SelectManyObserverWithIndex parent;

				private readonly TSource sourceValue;

				private readonly int sourceIndex;

				private readonly IDisposable cancel;

				private int index;

				public SelectManyObserver(SelectManyObserverWithIndex parent, TSource value, int index, IDisposable cancel)
					: base(parent.observer, cancel)
				{
					this.parent = parent;
					sourceValue = value;
					sourceIndex = index;
					this.cancel = cancel;
				}

				public override void OnNext(TCollection value)
				{
					TResult value2;
					try
					{
						value2 = parent.parent.resultSelectorWithIndex(sourceValue, sourceIndex, value, index++);
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
					lock (parent.gate)
					{
						observer.OnNext(value2);
					}
				}

				public override void OnError(Exception error)
				{
					lock (parent.gate)
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
				}

				public override void OnCompleted()
				{
					parent.collectionDisposable.Remove(cancel);
					if (parent.isStopped && parent.collectionDisposable.Count == 1)
					{
						lock (parent.gate)
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
			}

			private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

			private CompositeDisposable collectionDisposable;

			private object gate = new object();

			private bool isStopped;

			private SingleAssignmentDisposable sourceDisposable;

			private int index;

			public SelectManyObserverWithIndex(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				collectionDisposable = new CompositeDisposable();
				sourceDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(sourceDisposable);
				sourceDisposable.Disposable = parent.source.Subscribe(this);
				return collectionDisposable;
			}

			public override void OnNext(TSource value)
			{
				int arg = index++;
				UniRx.IObservable<TCollection> observable;
				try
				{
					observable = parent.collectionSelectorWithIndex(value, arg);
				}
				catch (Exception error)
				{
					OnError(error);
					return;
				}
				SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
				collectionDisposable.Add(singleAssignmentDisposable);
				SelectManyObserver observer = new SelectManyObserver(this, value, arg, singleAssignmentDisposable);
				singleAssignmentDisposable.Disposable = observable.Subscribe(observer);
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				isStopped = true;
				if (collectionDisposable.Count == 1)
				{
					lock (gate)
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
					sourceDisposable.Dispose();
				}
			}
		}

		private class SelectManyEnumerableObserver : OperatorObserverBase<TSource, TResult>
		{
			private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

			public SelectManyEnumerableObserver(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(TSource value)
			{
				IEnumerable<TCollection> enumerable;
				try
				{
					enumerable = parent.collectionSelectorEnumerable(value);
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
				using (IEnumerator<TCollection> enumerator = enumerable.GetEnumerator())
				{
					bool flag = true;
					while (flag)
					{
						flag = false;
						TResult value2 = default(TResult);
						try
						{
							flag = enumerator.MoveNext();
							if (flag)
							{
								value2 = parent.resultSelector(value, enumerator.Current);
							}
						}
						catch (Exception error2)
						{
							try
							{
								observer.OnError(error2);
							}
							finally
							{
								Dispose();
							}
							return;
						}
						if (flag)
						{
							observer.OnNext(value2);
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

		private class SelectManyEnumerableObserverWithIndex : OperatorObserverBase<TSource, TResult>
		{
			private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

			private int index;

			public SelectManyEnumerableObserverWithIndex(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(TSource value)
			{
				int arg = index++;
				IEnumerable<TCollection> enumerable;
				try
				{
					enumerable = parent.collectionSelectorEnumerableWithIndex(value, arg);
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
				using (IEnumerator<TCollection> enumerator = enumerable.GetEnumerator())
				{
					int num = 0;
					bool flag = true;
					while (flag)
					{
						flag = false;
						TResult value2 = default(TResult);
						try
						{
							flag = enumerator.MoveNext();
							if (flag)
							{
								value2 = parent.resultSelectorWithIndex(value, arg, enumerator.Current, num++);
							}
						}
						catch (Exception error2)
						{
							try
							{
								observer.OnError(error2);
							}
							finally
							{
								Dispose();
							}
							return;
						}
						if (flag)
						{
							observer.OnNext(value2);
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

		private readonly UniRx.IObservable<TSource> source;

		private readonly Func<TSource, UniRx.IObservable<TCollection>> collectionSelector;

		private readonly Func<TSource, int, UniRx.IObservable<TCollection>> collectionSelectorWithIndex;

		private readonly Func<TSource, IEnumerable<TCollection>> collectionSelectorEnumerable;

		private readonly Func<TSource, int, IEnumerable<TCollection>> collectionSelectorEnumerableWithIndex;

		private readonly Func<TSource, TCollection, TResult> resultSelector;

		private readonly Func<TSource, int, TCollection, int, TResult> resultSelectorWithIndex;

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, UniRx.IObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.collectionSelector = collectionSelector;
			this.resultSelector = resultSelector;
		}

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, int, UniRx.IObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			collectionSelectorWithIndex = collectionSelector;
			resultSelectorWithIndex = resultSelector;
		}

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			collectionSelectorEnumerable = collectionSelector;
			this.resultSelector = resultSelector;
		}

		public SelectManyObservable(UniRx.IObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			collectionSelectorEnumerableWithIndex = collectionSelector;
			resultSelectorWithIndex = resultSelector;
		}

		protected override IDisposable SubscribeCore(UniRx.IObserver<TResult> observer, IDisposable cancel)
		{
			if (collectionSelector != null)
			{
				return new SelectManyOuterObserver(this, observer, cancel).Run();
			}
			if (collectionSelectorWithIndex != null)
			{
				return new SelectManyObserverWithIndex(this, observer, cancel).Run();
			}
			if (collectionSelectorEnumerable != null)
			{
				return new SelectManyEnumerableObserver(this, observer, cancel).Run();
			}
			if (collectionSelectorEnumerableWithIndex != null)
			{
				return new SelectManyEnumerableObserverWithIndex(this, observer, cancel).Run();
			}
			throw new InvalidOperationException();
		}
	}
}
