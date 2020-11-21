using System;
using UnityEngine.Events;

namespace UniRx
{
	public static class UnityEventExtensions
	{
		public static UniRx.IObservable<Unit> AsObservable(this UnityEvent unityEvent)
		{
			return Observable.FromEvent<UnityAction>((Action h) => h.Invoke, delegate(UnityAction h)
			{
				unityEvent.AddListener(h);
			}, delegate(UnityAction h)
			{
				unityEvent.RemoveListener(h);
			});
		}

		public static UniRx.IObservable<T> AsObservable<T>(this UnityEvent<T> unityEvent)
		{
			return Observable.FromEvent<UnityAction<T>, T>((Action<T> h) => h.Invoke, delegate(UnityAction<T> h)
			{
				unityEvent.AddListener(h);
			}, delegate(UnityAction<T> h)
			{
				unityEvent.RemoveListener(h);
			});
		}

		public static UniRx.IObservable<UniRx.Tuple<T0, T1>> AsObservable<T0, T1>(this UnityEvent<T0, T1> unityEvent)
		{
			return Observable.FromEvent<UnityAction<T0, T1>, Tuple<T0, T1>>((Action<UniRx.Tuple<T0, T1>> h) => delegate(T0 t0, T1 t1)
			{
				h(UniRx.Tuple.Create(t0, t1));
			}, delegate(UnityAction<T0, T1> h)
			{
				unityEvent.AddListener(h);
			}, delegate(UnityAction<T0, T1> h)
			{
				unityEvent.RemoveListener(h);
			});
		}

		public static UniRx.IObservable<UniRx.Tuple<T0, T1, T2>> AsObservable<T0, T1, T2>(this UnityEvent<T0, T1, T2> unityEvent)
		{
			return Observable.FromEvent<UnityAction<T0, T1, T2>, Tuple<T0, T1, T2>>((Action<UniRx.Tuple<T0, T1, T2>> h) => delegate(T0 t0, T1 t1, T2 t2)
			{
				h(UniRx.Tuple.Create(t0, t1, t2));
			}, delegate(UnityAction<T0, T1, T2> h)
			{
				unityEvent.AddListener(h);
			}, delegate(UnityAction<T0, T1, T2> h)
			{
				unityEvent.RemoveListener(h);
			});
		}

		public static UniRx.IObservable<UniRx.Tuple<T0, T1, T2, T3>> AsObservable<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent)
		{
			return Observable.FromEvent<UnityAction<T0, T1, T2, T3>, Tuple<T0, T1, T2, T3>>((Action<UniRx.Tuple<T0, T1, T2, T3>> h) => delegate(T0 t0, T1 t1, T2 t2, T3 t3)
			{
				h(UniRx.Tuple.Create(t0, t1, t2, t3));
			}, delegate(UnityAction<T0, T1, T2, T3> h)
			{
				unityEvent.AddListener(h);
			}, delegate(UnityAction<T0, T1, T2, T3> h)
			{
				unityEvent.RemoveListener(h);
			});
		}
	}
}
