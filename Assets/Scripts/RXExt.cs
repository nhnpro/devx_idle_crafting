using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public static class RXExt
{
	public static UniRx.IObservable<T> CombineLatest<T>(this IEnumerable<UniRx.IObservable<T>> sources, T seed, Func<T, T, T> combinator)
	{
		return sources.Aggregate(Observable.Return(seed), (UniRx.IObservable<T> left, UniRx.IObservable<T> right) => left.CombineLatest(right, combinator));
	}

	public static UniRx.IObservable<T> CombineLatest<T>(this IEnumerable<UniRx.IObservable<T>> sources, Func<T, T, T> combinator)
	{
		return sources.Aggregate((UniRx.IObservable<T> left, UniRx.IObservable<T> right) => left.CombineLatest(right, combinator));
	}

	public static UniRx.IObservable<bool> And(this UniRx.IObservable<bool> left, UniRx.IObservable<bool> right)
	{
		return left.CombineLatest(right, (bool a, bool b) => a && b);
	}

	public static UniRx.IObservable<bool> Or(this UniRx.IObservable<bool> left, UniRx.IObservable<bool> right)
	{
		return left.CombineLatest(right, (bool a, bool b) => a || b);
	}

	public static UniRx.IObservable<T> Previous<T>(this UniRx.IObservable<T> source)
	{
		return Observable.Create(delegate(UniRx.IObserver<T> observer)
		{
			bool initialized = false;
			T newest = (T)default(T);
			UniRx.IObservable<T> source2 = source;
			Action<T> onNext = delegate(T x)
			{
				if (!initialized)
				{
					initialized = true;
				}
				else
				{
					observer.OnNext((T)newest);
				}
				newest = (T)x;
			};
			UniRx.IObserver<T> observer2 = observer;
			Action<Exception> onError = observer2.OnError;
			UniRx.IObserver<T> observer3 = observer;
			return source2.Subscribe(onNext, onError, observer3.OnCompleted);
		});
	}

	public static UniRx.IObservable<T> Catch<T>(this UniRx.IObservable<T> source, Func<Exception, UniRx.IObservable<T>> onError)
	{
		return Observable.Create(delegate(UniRx.IObserver<T> observer)
		{
			UniRx.IObservable<T> source2 = source;
			Action<T> onNext = delegate(T x)
			{
				observer.OnNext(x);
			};
			Action<Exception> onError2 = delegate(Exception e)
			{
				UniRx.IObservable<T> source3 = onError(e);
				Action<T> onNext2 = delegate(T x)
				{
					observer.OnNext(x);
				};
				UniRx.IObserver<T> observer3 = observer;
				Action<Exception> onError3 = observer3.OnError;
				UniRx.IObserver<T> observer4 = observer;
				source3.Subscribe(onNext2, onError3, observer4.OnCompleted);
			};
			UniRx.IObserver<T> observer2 = observer;
			return source2.Subscribe(onNext, onError2, observer2.OnCompleted);
		});
	}

	public static UniRx.IObservable<T> CatchObserverErrors<T>(this UniRx.IObservable<T> source)
	{
		return Observable.Create((UniRx.IObserver<T> observer) => source.Subscribe(delegate(T v)
		{
			try
			{
				observer.OnNext(v);
			}
			catch (Exception error)
			{
				observer.OnError(error);
			}
		}, delegate(Exception ex)
		{
			observer.OnError(ex);
		}, delegate
		{
			observer.OnCompleted();
		}));
	}

	public static void NotifyRecursively(GameObject gameObject)
	{
		foreach (GameObject item in gameObject.Children())
		{
			NotifyRecursively(item);
			gameObject.Notify();
		}
	}

	public static void DestroyAndNotifyChildren(this GameObject gameObject)
	{
		foreach (GameObject item in gameObject.Children())
		{
			item.DestroyAndNotify();
		}
	}

	public static void Notify(this GameObject gameObject)
	{
		if (!(gameObject == null))
		{
			ObservableDestroyTrigger component = gameObject.GetComponent<ObservableDestroyTrigger>();
			if (component != null)
			{
				component.Invoke("OnDestroy", 0f);
			}
		}
	}

	public static void DestroyAndNotify(this GameObject gameObject)
	{
		NotifyRecursively(gameObject);
		UnityEngine.Object.Destroy(gameObject);
	}

	public static UniRx.IObservable<T> WhenEnabled<T>(this UniRx.IObservable<T> obs, GameObject go)
	{
		UniRx.IObservable<Unit> source = (!go.activeInHierarchy) ? go.OnEnableAsObservable() : Observable.Return(Unit.Default).Concat(go.OnEnableAsObservable());
		return (from _ in source
			select obs.TakeUntil(go.OnDisableAsObservable())).Switch();
	}

	public static UniRx.IObservable<T> EmitWhenEnabled<T>(this UniRx.IObservable<T> obs, UniRx.IObservable<bool> enabler)
	{
		return (from e in enabler
			where e
			select e into _
			select obs.TakeUntil(from e in enabler
				where !e
				select e)).Switch();
	}

	public static T ToEnum<T>(this string val) where T : struct
	{
		return (T)Enum.Parse(typeof(T), val);
	}

	public static T ToEnumOr<T>(this string val, T defaultValue) where T : struct
	{
		try
		{
			return val.ToEnum<T>();
		}
		catch (Exception)
		{
			return defaultValue;
		}
	}

	public static T FirstOr<T>(this IEnumerable<T> items, Func<T> defaultValue)
	{
		try
		{
			return items.First();
		}
		catch (Exception)
		{
			return defaultValue();
		}
	}

	public static T FirstOr<T>(this IEnumerable<T> items, Func<T, bool> predicate, Func<T> defaultValue)
	{
		try
		{
			return items.First(predicate);
		}
		catch (Exception)
		{
			return defaultValue();
		}
	}

	public static T LastOr<T>(this IEnumerable<T> items, Func<T> defaultValue)
	{
		try
		{
			return items.Last();
		}
		catch (Exception)
		{
			return defaultValue();
		}
	}

	public static T LastOr<T>(this IEnumerable<T> items, Func<T, bool> predicate, Func<T> defaultValue)
	{
		try
		{
			return items.Last(predicate);
		}
		catch (Exception)
		{
			return defaultValue();
		}
	}

	public static IEnumerable<T> Yield<T>(this T item)
	{
		if (item != null)
		{
			yield return item;
		}
	}

	public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<IEnumerable<T>> action)
	{
		action(enumerable);
		return enumerable;
	}

	public static IEnumerable<T> DoEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		foreach (T item in enumerable)
		{
			action(item);
		}
		return enumerable;
	}

	public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable)
	{
		return new HashSet<T>(enumerable);
	}

	public static IDisposable SubscribeToText(this UniRx.IObservable<string> source, TextMesh text)
	{
		return source.Subscribe(delegate(string x)
		{
			text.text = x;
		});
	}

	public static IDisposable SubscribeToTextUntilNull(this UniRx.IObservable<string> source, GameObject go, Text text)
	{
		SerialDisposable disposable = new SerialDisposable();
		disposable.Disposable = source.Subscribe(delegate(string t)
		{
			if (go != null)
			{
				text.text = t;
			}
			else
			{
				disposable.Dispose();
			}
		});
		return disposable;
	}

	public static IDisposable SubscribeToActive(this UniRx.IObservable<bool> source, GameObject go)
	{
		return source.Subscribe(delegate(bool active)
		{
			go.SetActive(active);
		});
	}

	public static IDisposable SubscribeToActiveUntilNull(this UniRx.IObservable<bool> source, GameObject go)
	{
		SerialDisposable disposable = new SerialDisposable();
		disposable.Disposable = source.Subscribe(delegate(bool active)
		{
			if (go != null)
			{
				go.SetActive(active);
			}
			else
			{
				disposable.Dispose();
			}
		});
		return disposable;
	}

	public static UniRx.IObservable<TSource> RetryAfterDelay<TSource, TException>(this UniRx.IObservable<TSource> source, TimeSpan retryDelay, int retryCount) where TException : Exception
	{
		return source.Catch((TException ex) => (retryCount <= 0) ? Observable.Throw<TSource>(ex) : source.DelaySubscription(retryDelay).RetryAfterDelay<TSource, TException>(retryDelay, --retryCount));
	}
}
