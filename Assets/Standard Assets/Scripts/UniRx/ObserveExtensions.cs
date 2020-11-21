using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

namespace UniRx
{
	public static class ObserveExtensions
	{
		public static UniRx.IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector, FrameCountType frameCountType = FrameCountType.Update, bool fastDestroyCheck = false) where TSource : class
		{
			return source.ObserveEveryValueChanged(propertySelector, frameCountType, UnityEqualityComparer.GetDefault<TProperty>(), fastDestroyCheck);
		}

		public static UniRx.IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector, FrameCountType frameCountType, IEqualityComparer<TProperty> comparer) where TSource : class
		{
			return source.ObserveEveryValueChanged(propertySelector, frameCountType, comparer, fastDestroyCheck: false);
		}

		public static UniRx.IObservable<TProperty> ObserveEveryValueChanged<TSource, TProperty>(this TSource source, Func<TSource, TProperty> propertySelector, FrameCountType frameCountType, IEqualityComparer<TProperty> comparer, bool fastDestroyCheck) where TSource : class
		{
			if (source == null)
			{
				return Observable.Empty<TProperty>();
			}
			if (comparer == null)
			{
				comparer = UnityEqualityComparer.GetDefault<TProperty>();
			}
			UnityEngine.Object unityObject = source as UnityEngine.Object;
			bool flag = source is UnityEngine.Object;
			if (flag && unityObject == null)
			{
				return Observable.Empty<TProperty>();
			}
			if (flag)
			{
				return Observable.FromMicroCoroutine(delegate(UniRx.IObserver<TProperty> observer, CancellationToken cancellationToken)
				{
					if (unityObject != null)
					{
						TProperty val2 = default(TProperty);
						try
						{
							val2 = propertySelector((TSource)(object)unityObject);
						}
						catch (Exception error2)
						{
							observer.OnError(error2);
							return EmptyEnumerator();
						}
						observer.OnNext(val2);
						return PublishUnityObjectValueChanged(unityObject, val2, propertySelector, comparer, observer, cancellationToken, fastDestroyCheck);
					}
					observer.OnCompleted();
					return EmptyEnumerator();
				}, frameCountType);
			}
			WeakReference reference = new WeakReference(source);
			source = (TSource)null;
			return Observable.FromMicroCoroutine(delegate(UniRx.IObserver<TProperty> observer, CancellationToken cancellationToken)
			{
				object target = reference.Target;
				if (target != null)
				{
					TProperty val = default(TProperty);
					try
					{
						val = propertySelector((TSource)target);
					}
					catch (Exception error)
					{
						observer.OnError(error);
						return EmptyEnumerator();
					}
					finally
					{
						target = null;
					}
					observer.OnNext(val);
					return PublishPocoValueChanged(reference, val, propertySelector, comparer, observer, cancellationToken);
				}
				observer.OnCompleted();
				return EmptyEnumerator();
			}, frameCountType);
		}

		private static IEnumerator EmptyEnumerator()
		{
			yield break;
		}

		private static IEnumerator PublishPocoValueChanged<TSource, TProperty>(WeakReference sourceReference, TProperty firstValue, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty> comparer, IObserver<TProperty> observer, CancellationToken cancellationToken)
		{
			TProperty currentValue = default(TProperty);
			TProperty prevValue = firstValue;
			while (true)
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					object target = sourceReference.Target;
					if (target == null)
					{
						break;
					}
					try
					{
						currentValue = propertySelector((TSource)target);
					}
					catch (Exception error)
					{
						observer.OnError(error);
						yield break;
					}
					finally
					{
					}
					if (!comparer.Equals(currentValue, prevValue))
					{
						observer.OnNext(currentValue);
						prevValue = currentValue;
					}
					yield return null;
					continue;
				}
				yield break;
			}
			observer.OnCompleted();
		}

		private static IEnumerator PublishUnityObjectValueChanged<TSource, TProperty>(UnityEngine.Object unityObject, TProperty firstValue, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty> comparer, IObserver<TProperty> observer, CancellationToken cancellationToken, bool fastDestroyCheck)
		{
			TProperty prevValue = firstValue;
			TSource source = (TSource)(object)unityObject;
			if (fastDestroyCheck)
			{
				GameObject gameObject = unityObject as GameObject;
				if (gameObject == null)
				{
					Component component = unityObject as Component;
					if (component != null)
					{
						gameObject = component.gameObject;
					}
				}
				if (!(gameObject == null))
				{
					ObservableDestroyTrigger destroyTrigger = GetOrAddDestroyTrigger(gameObject);
					while (true)
					{
						if (!cancellationToken.IsCancellationRequested)
						{
							if (!((!destroyTrigger.IsActivated) ? (unityObject != null) : (!destroyTrigger.IsCalledOnDestroy)))
							{
								break;
							}
							TProperty currentValue2;
							try
							{
								currentValue2 = propertySelector(source);
							}
							catch (Exception error)
							{
								observer.OnError(error);
								yield break;
							}
							if (!comparer.Equals(currentValue2, prevValue))
							{
								observer.OnNext(currentValue2);
								prevValue = currentValue2;
							}
							yield return null;
							continue;
						}
						yield break;
					}
					observer.OnCompleted();
					yield break;
				}
			}
			while (true)
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					if (!(unityObject != null))
					{
						break;
					}
					TProperty currentValue2;
					try
					{
						currentValue2 = propertySelector(source);
					}
					catch (Exception error2)
					{
						observer.OnError(error2);
						yield break;
					}
					if (!comparer.Equals(currentValue2, prevValue))
					{
						observer.OnNext(currentValue2);
						prevValue = currentValue2;
					}
					yield return null;
					continue;
				}
				yield break;
			}
			observer.OnCompleted();
		}

		private static ObservableDestroyTrigger GetOrAddDestroyTrigger(GameObject go)
		{
			ObservableDestroyTrigger observableDestroyTrigger = go.GetComponent<ObservableDestroyTrigger>();
			if (observableDestroyTrigger == null)
			{
				observableDestroyTrigger = go.AddComponent<ObservableDestroyTrigger>();
			}
			return observableDestroyTrigger;
		}
	}
}
