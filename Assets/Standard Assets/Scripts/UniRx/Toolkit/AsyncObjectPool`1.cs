using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Toolkit
{
	public abstract class AsyncObjectPool<T> : IDisposable where T : Component
	{
		private bool disposedValue;

		private Queue<T> q;

		protected int MaxPoolCount => int.MaxValue;

		public int Count
		{
			get
			{
				if (q == null)
				{
					return 0;
				}
				return q.Count;
			}
		}

		protected abstract UniRx.IObservable<T> CreateInstanceAsync();

		protected virtual void OnBeforeRent(T instance)
		{
			instance.gameObject.SetActive(value: true);
		}

		protected virtual void OnBeforeReturn(T instance)
		{
			instance.gameObject.SetActive(value: false);
		}

		protected virtual void OnClear(T instance)
		{
			if (!((UnityEngine.Object)instance == (UnityEngine.Object)null))
			{
				GameObject gameObject = instance.gameObject;
				if (!(gameObject == null))
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}

		public UniRx.IObservable<T> RentAsync()
		{
			if (disposedValue)
			{
				throw new ObjectDisposedException("ObjectPool was already disposed.");
			}
			if (q == null)
			{
				q = new Queue<T>();
			}
			if (q.Count > 0)
			{
				T val = q.Dequeue();
				OnBeforeRent(val);
				return Observable.Return(val);
			}
			UniRx.IObservable<T> source = CreateInstanceAsync();
			return source.Do(delegate(T x)
			{
				OnBeforeRent(x);
			});
		}

		public void Return(T instance)
		{
			if (disposedValue)
			{
				throw new ObjectDisposedException("ObjectPool was already disposed.");
			}
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				throw new ArgumentNullException("instance");
			}
			if (q == null)
			{
				q = new Queue<T>();
			}
			if (q.Count + 1 == MaxPoolCount)
			{
				throw new InvalidOperationException("Reached Max PoolSize");
			}
			OnBeforeReturn(instance);
			q.Enqueue(instance);
		}

		public void Shrink(float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
		{
			if (q == null)
			{
				return;
			}
			if (instanceCountRatio <= 0f)
			{
				instanceCountRatio = 0f;
			}
			if (instanceCountRatio >= 1f)
			{
				instanceCountRatio = 1f;
			}
			int val = (int)((float)q.Count * instanceCountRatio);
			val = Math.Max(minSize, val);
			while (q.Count > val)
			{
				T instance = q.Dequeue();
				if (callOnBeforeRent)
				{
					OnBeforeRent(instance);
				}
				OnClear(instance);
			}
		}

		public IDisposable StartShrinkTimer(TimeSpan checkInterval, float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
		{
			return Observable.Interval(checkInterval).TakeWhile((long _) => disposedValue).Subscribe(delegate
			{
				Shrink(instanceCountRatio, minSize, callOnBeforeRent);
			});
		}

		public void Clear(bool callOnBeforeRent = false)
		{
			if (q == null)
			{
				return;
			}
			while (q.Count != 0)
			{
				T instance = q.Dequeue();
				if (callOnBeforeRent)
				{
					OnBeforeRent(instance);
				}
				OnClear(instance);
			}
		}

		public UniRx.IObservable<Unit> PreloadAsync(int preloadCount, int threshold)
		{
			if (q == null)
			{
				q = new Queue<T>(preloadCount);
			}
			return Observable.FromMicroCoroutine((UniRx.IObserver<Unit> observer, CancellationToken cancel) => PreloadCore(preloadCount, threshold, observer, cancel));
		}

		private IEnumerator PreloadCore(int preloadCount, int threshold, IObserver<Unit> observer, CancellationToken cancellationToken)
		{
			while (Count < preloadCount && !cancellationToken.IsCancellationRequested)
			{
				int requireCount = preloadCount - Count;
				if (requireCount <= 0)
				{
					break;
				}
				int createCount = Math.Min(requireCount, threshold);
				UniRx.IObservable<Unit>[] loaders = new UniRx.IObservable<Unit>[createCount];
				for (int i = 0; i < createCount; i++)
				{
					UniRx.IObservable<T> source = CreateInstanceAsync();
					loaders[i] = source.ForEachAsync(delegate(T x)
					{
						Return(x);
					});
				}
				ObservableYieldInstruction<Unit> awaiter = Observable.WhenAll(loaders).ToYieldInstruction(throwOnError: false, cancellationToken);
				while (!awaiter.HasResult && !awaiter.IsCanceled && !awaiter.HasError)
				{
					yield return null;
				}
				if (awaiter.HasError)
				{
					observer.OnError(awaiter.Error);
					yield break;
				}
				if (awaiter.IsCanceled)
				{
					yield break;
				}
			}
			observer.OnNext(Unit.Default);
			observer.OnCompleted();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Clear();
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
