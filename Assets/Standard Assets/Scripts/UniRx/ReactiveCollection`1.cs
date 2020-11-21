using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UniRx
{
	[Serializable]
	public class ReactiveCollection<T> : Collection<T>, IReactiveCollection<T>, IDisposable, IList<T>, IReadOnlyReactiveCollection<T>, IEnumerable, ICollection<T>, IEnumerable<T>
	{
		[NonSerialized]
		private bool isDisposed;

		[NonSerialized]
		private Subject<int> countChanged;

		[NonSerialized]
		private Subject<Unit> collectionReset;

		[NonSerialized]
		private Subject<CollectionAddEvent<T>> collectionAdd;

		[NonSerialized]
		private Subject<CollectionMoveEvent<T>> collectionMove;

		[NonSerialized]
		private Subject<CollectionRemoveEvent<T>> collectionRemove;

		[NonSerialized]
		private Subject<CollectionReplaceEvent<T>> collectionReplace;

		private bool disposedValue;

		public ReactiveCollection()
		{
		}

		public ReactiveCollection(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (T item in collection)
			{
				Add(item);
			}
		}

		public ReactiveCollection(List<T> list)
			: base((IList<T>)((list == null) ? null : new List<T>(list)))
		{
		}

		protected override void ClearItems()
		{
			int count = base.Count;
			base.ClearItems();
			if (collectionReset != null)
			{
				collectionReset.OnNext(Unit.Default);
			}
			if (count > 0 && countChanged != null)
			{
				countChanged.OnNext(base.Count);
			}
		}

		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);
			if (collectionAdd != null)
			{
				collectionAdd.OnNext(new CollectionAddEvent<T>(index, item));
			}
			if (countChanged != null)
			{
				countChanged.OnNext(base.Count);
			}
		}

		public void Move(int oldIndex, int newIndex)
		{
			MoveItem(oldIndex, newIndex);
		}

		protected virtual void MoveItem(int oldIndex, int newIndex)
		{
			T val = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, val);
			if (collectionMove != null)
			{
				collectionMove.OnNext(new CollectionMoveEvent<T>(oldIndex, newIndex, val));
			}
		}

		protected override void RemoveItem(int index)
		{
			T value = base[index];
			base.RemoveItem(index);
			if (collectionRemove != null)
			{
				collectionRemove.OnNext(new CollectionRemoveEvent<T>(index, value));
			}
			if (countChanged != null)
			{
				countChanged.OnNext(base.Count);
			}
		}

		protected override void SetItem(int index, T item)
		{
			T oldValue = base[index];
			base.SetItem(index, item);
			if (collectionReplace != null)
			{
				collectionReplace.OnNext(new CollectionReplaceEvent<T>(index, oldValue, item));
			}
		}

		public UniRx.IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
		{
			if (isDisposed)
			{
				return Observable.Empty<int>();
			}
			Subject<int> subject = countChanged ?? (countChanged = new Subject<int>());
			if (notifyCurrentCount)
			{
				return ((UniRx.IObservable<int>)subject).StartWith(()=>Count);
			}
			return subject;
		}

		public UniRx.IObservable<Unit> ObserveReset()
		{
			if (isDisposed)
			{
				return Observable.Empty<Unit>();
			}
			return collectionReset ?? (collectionReset = new Subject<Unit>());
		}

		public UniRx.IObservable<CollectionAddEvent<T>> ObserveAdd()
		{
			if (isDisposed)
			{
				return Observable.Empty<CollectionAddEvent<T>>();
			}
			return collectionAdd ?? (collectionAdd = new Subject<CollectionAddEvent<T>>());
		}

		public UniRx.IObservable<CollectionMoveEvent<T>> ObserveMove()
		{
			if (isDisposed)
			{
				return Observable.Empty<CollectionMoveEvent<T>>();
			}
			return collectionMove ?? (collectionMove = new Subject<CollectionMoveEvent<T>>());
		}

		public UniRx.IObservable<CollectionRemoveEvent<T>> ObserveRemove()
		{
			if (isDisposed)
			{
				return Observable.Empty<CollectionRemoveEvent<T>>();
			}
			return collectionRemove ?? (collectionRemove = new Subject<CollectionRemoveEvent<T>>());
		}

		public UniRx.IObservable<CollectionReplaceEvent<T>> ObserveReplace()
		{
			if (isDisposed)
			{
				return Observable.Empty<CollectionReplaceEvent<T>>();
			}
			return collectionReplace ?? (collectionReplace = new Subject<CollectionReplaceEvent<T>>());
		}

		private void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
		{
			if (subject != null)
			{
				try
				{
					subject.OnCompleted();
				}
				finally
				{
					subject.Dispose();
					subject = null;
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					DisposeSubject(ref collectionReset);
					DisposeSubject(ref collectionAdd);
					DisposeSubject(ref collectionMove);
					DisposeSubject(ref collectionRemove);
					DisposeSubject(ref collectionReplace);
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}
	}
}
