using System;
using System.Collections.Generic;
using UniRx.InternalUtil;

namespace UniRx
{
	public class AsyncMessageBroker : IAsyncMessageBroker, IDisposable, IAsyncMessagePublisher, IAsyncMessageReceiver
	{
		private class Subscription<T> : IDisposable
		{
			private readonly AsyncMessageBroker parent;

			private readonly Func<T, UniRx.IObservable<Unit>> asyncMessageReceiver;

			public Subscription(AsyncMessageBroker parent, Func<T, UniRx.IObservable<Unit>> asyncMessageReceiver)
			{
				this.parent = parent;
				this.asyncMessageReceiver = asyncMessageReceiver;
			}

			public void Dispose()
			{
				lock (parent.notifiers)
				{
					if (parent.notifiers.TryGetValue(typeof(T), out object value))
					{
						ImmutableList<Func<T, UniRx.IObservable<Unit>>> immutableList = (ImmutableList<Func<T, UniRx.IObservable<Unit>>>)value;
						immutableList = immutableList.Remove(asyncMessageReceiver);
						parent.notifiers[typeof(T)] = immutableList;
					}
				}
			}
		}

		public static readonly IAsyncMessageBroker Default = new AsyncMessageBroker();

		private bool isDisposed;

		private readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();

		public UniRx.IObservable<Unit> PublishAsync<T>(T message)
		{
			ImmutableList<Func<T, UniRx.IObservable<Unit>>> immutableList;
			lock (notifiers)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("AsyncMessageBroker");
				}
				if (!notifiers.TryGetValue(typeof(T), out object value))
				{
					return Observable.ReturnUnit();
				}
				immutableList = (ImmutableList<Func<T, UniRx.IObservable<Unit>>>)value;
			}
			Func<T, UniRx.IObservable<Unit>>[] data = immutableList.Data;
			UniRx.IObservable<Unit>[] array = new UniRx.IObservable<Unit>[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				array[i] = data[i](message);
			}
			return Observable.WhenAll(array);
		}

		public IDisposable Subscribe<T>(Func<T, UniRx.IObservable<Unit>> asyncMessageReceiver)
		{
			lock (notifiers)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("AsyncMessageBroker");
				}
				if (!notifiers.TryGetValue(typeof(T), out object value))
				{
					ImmutableList<Func<T, UniRx.IObservable<Unit>>> empty = ImmutableList<Func<T, UniRx.IObservable<Unit>>>.Empty;
					empty = empty.Add(asyncMessageReceiver);
					notifiers.Add(typeof(T), empty);
				}
				else
				{
					ImmutableList<Func<T, UniRx.IObservable<Unit>>> immutableList = (ImmutableList<Func<T, UniRx.IObservable<Unit>>>)value;
					immutableList = immutableList.Add(asyncMessageReceiver);
					notifiers[typeof(T)] = immutableList;
				}
			}
			return new Subscription<T>(this, asyncMessageReceiver);
		}

		public void Dispose()
		{
			lock (notifiers)
			{
				if (!isDisposed)
				{
					isDisposed = true;
					notifiers.Clear();
				}
			}
		}
	}
}
