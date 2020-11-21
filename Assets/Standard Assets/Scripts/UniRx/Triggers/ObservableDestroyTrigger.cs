using System;
using UnityEngine;

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableDestroyTrigger : MonoBehaviour
	{
		private bool calledDestroy;

		private Subject<Unit> onDestroy;

		private CompositeDisposable disposablesOnDestroy;

		[Obsolete("Internal Use.")]
		internal bool IsMonitoredActivate
		{
			get;
			set;
		}

		public bool IsActivated
		{
			get;
			private set;
		}

		public bool IsCalledOnDestroy => calledDestroy;

		private void Awake()
		{
			IsActivated = true;
		}

		private void OnDestroy()
		{
			if (!calledDestroy)
			{
				calledDestroy = true;
				if (disposablesOnDestroy != null)
				{
					disposablesOnDestroy.Dispose();
				}
				if (onDestroy != null)
				{
					onDestroy.OnNext(Unit.Default);
					onDestroy.OnCompleted();
				}
			}
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

		public void ForceRaiseOnDestroy()
		{
			OnDestroy();
		}

		public void AddDisposableOnDestroy(IDisposable disposable)
		{
			if (calledDestroy)
			{
				disposable.Dispose();
				return;
			}
			if (disposablesOnDestroy == null)
			{
				disposablesOnDestroy = new CompositeDisposable();
			}
			disposablesOnDestroy.Add(disposable);
		}
	}
}
