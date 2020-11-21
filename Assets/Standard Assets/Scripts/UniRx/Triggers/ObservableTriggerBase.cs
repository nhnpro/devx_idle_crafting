using UnityEngine;

namespace UniRx.Triggers
{
	public abstract class ObservableTriggerBase : MonoBehaviour
	{
		private bool calledAwake;

		private Subject<Unit> awake;

		private bool calledStart;

		private Subject<Unit> start;

		private bool calledDestroy;

		private Subject<Unit> onDestroy;

		private void Awake()
		{
			calledAwake = true;
			if (awake != null)
			{
				awake.OnNext(Unit.Default);
				awake.OnCompleted();
			}
		}

		public UniRx.IObservable<Unit> AwakeAsObservable()
		{
			if (calledAwake)
			{
				return Observable.Return(Unit.Default);
			}
			return awake ?? (awake = new Subject<Unit>());
		}

		private void Start()
		{
			calledStart = true;
			if (start != null)
			{
				start.OnNext(Unit.Default);
				start.OnCompleted();
			}
		}

		public UniRx.IObservable<Unit> StartAsObservable()
		{
			if (calledStart)
			{
				return Observable.Return(Unit.Default);
			}
			return start ?? (start = new Subject<Unit>());
		}

		private void OnDestroy()
		{
			calledDestroy = true;
			if (onDestroy != null)
			{
				onDestroy.OnNext(Unit.Default);
				onDestroy.OnCompleted();
			}
			RaiseOnCompletedOnDestroy();
		}

		public UniRx.IObservable<Unit> OnDestroyAsObservable()
		{
			if (this == null)
			{
				return Observable.Return(Unit.Default);
			}
			if (calledDestroy)
			{
				return Observable.Return(Unit.Default);
			}
			return onDestroy ?? (onDestroy = new Subject<Unit>());
		}

		protected abstract void RaiseOnCompletedOnDestroy();
	}
}
